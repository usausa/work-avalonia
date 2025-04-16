namespace WorkBuildHat;

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;

internal static class Program
{
    public static void Main()
    {
        // TODO
    }
}

public static class StreamFactory
{
#pragma warning disable CA2000
    public static async ValueTask<Stream> CreateSocketStream(string address, int port)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            NoDelay = true
        };

        try
        {
            await socket.ConnectAsync(new IPEndPoint(IPAddress.Parse(address), port)).ConfigureAwait(false);
            return new NetworkStream(socket, true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
#pragma warning restore CA2000
}

public sealed class BuildHatRemote : IDisposable
{
    private const int Timeout = 60_000;

    private readonly Stream stream;

    private readonly PipeWriter output;

    private readonly PipeReader input;

    private readonly PooledBufferWriter<byte> receiveBuffer = new(1024);

    public BuildHatRemote(Stream stream)
    {
        this.stream = stream;
        output = PipeWriter.Create(stream);
        input = PipeReader.Create(stream);
    }

    public void Dispose()
    {
        stream.Dispose();
        receiveBuffer.Dispose();
    }

    public async ValueTask<bool> TestAsync()
    {
        // Exit
        output.WriteAndAdvance("refresh\r\n"u8);
        await output.FlushAsync().ConfigureAwait(false);

        receiveBuffer.Clear();
        await ReceiveResponseAsync().ConfigureAwait(false);

        return IsSuccessResponse();
    }

    private bool IsSuccessResponse() => receiveBuffer.WrittenSpan.StartsWith("ok"u8);

    private async ValueTask ReceiveResponseAsync()
    {
        // Receive
        using var cancel = new ReusableCancellationTokenSource();
        while (true)
        {
            cancel.CancelAfter(Timeout);
            var result = await input.ReadAsync(cancel.Token).ConfigureAwait(false);

            var buffer = result.Buffer;

            var read = false;
            if (!buffer.IsEmpty && ReadLine(ref buffer, out var line))
            {
                var length = (int)line.Length;
                line.CopyTo(receiveBuffer.GetSpan(length));
                receiveBuffer.Advance(length);
                read = true;
            }

            input.AdvanceTo(buffer.Start, buffer.End);

            if (read || result.IsCompleted || result.IsCanceled)
            {
                break;
            }

            cancel.Reset();
        }
    }

    private static bool ReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var reader = new SequenceReader<byte>(buffer);
        if (reader.TryReadTo(out ReadOnlySequence<byte> l, "\r\n"u8))
        {
            buffer = buffer.Slice(reader.Position);
            line = l;
            return true;
        }

        line = default;
        return false;
    }
}
