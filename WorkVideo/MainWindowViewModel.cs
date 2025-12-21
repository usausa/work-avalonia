namespace Example.Video4Linux2.AvaloniaApp;

using Avalonia;

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

using Example.Video4Linux2.AvaloniaApp.Helper;
using System.Runtime.Versioning;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public partial class MainWindowViewModel : ExtendViewModelBase
{
    private const int Width = 640;
    private const int Height = 480;
    private const int BitmapBufferSize = Width * Height * 4;

    private readonly IDispatcher dispatcher;

    private readonly BufferManager bufferManager;

    private readonly VideoCapture capture;

    private readonly WriteableBitmap? bitmap;

    [ObservableProperty]
    public partial WriteableBitmap? Bitmap { get; set; }

    public IObserveCommand StartCommand { get; }

    public IObserveCommand StopCommand { get; }

    public MainWindowViewModel(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
        bufferManager = new BufferManager(4, BitmapBufferSize);
        capture = new VideoCapture();
        capture.FrameCaptured += CaptureOnFrameCaptured;
        bitmap = new WriteableBitmap(new PixelSize(Width, Height), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Premul);

        StartCommand = MakeDelegateCommand(StartCapture, () => !capture.IsCapturing);
        StopCommand = MakeDelegateCommand(StopCapture, () => capture.IsCapturing);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            capture.Dispose();
            capture.FrameCaptured -= CaptureOnFrameCaptured;
            bufferManager.Dispose();
            bitmap?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void StartCapture()
    {
        // TODO fix size
        capture.Open();
        capture.StartCapture();
    }

    private void StopCapture()
    {
        capture.StopCapture();
        capture.Close();
    }

    private void CaptureOnFrameCaptured(FrameBuffer frame)
    {
        // TODO show fps
        var slot = bufferManager.NextSlot();
        lock (slot.Lock)
        {
            ImageHelper.ConvertYUYV2RGBA(frame.AsSpan(), slot.Buffer);
            slot.MarkUpdated();
        }

        dispatcher.Post(UpdateBitmap);
    }

    private unsafe void UpdateBitmap()
    {
        if (IsDisposed)
        {
            return;
        }

        var slot = bufferManager.LastUpdateSlot();
        if (slot is null)
        {
            return;
        }

        // Lock
        lock (slot.Lock)
        {
            using var lockedBitmap = bitmap!.Lock();
            var buffer = new Span<byte>(lockedBitmap.Address.ToPointer(), BitmapBufferSize);
            slot.Buffer.CopyTo(buffer);
        }

        // Disable cache & update
        Bitmap = null;
        Bitmap = bitmap;
    }
}

// ReSharper disable StructCanBeMadeReadOnly
#pragma warning disable CA1815
public readonly struct FrameBuffer
{
    private readonly byte[] buffer;

    private readonly int length;

    public int Length => length;

    public bool IsEmpty => buffer.AsSpan().IsEmpty;

    internal FrameBuffer(byte[] buffer, int length)
    {
        this.buffer = buffer;
        this.length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AsSpan()
    {
        return IsEmpty ? Span<byte>.Empty : buffer.AsSpan();
    }

    public byte[] ToArray()
    {
        if (IsEmpty)
        {
            return [];
        }

        var array = new byte[length];
        AsSpan().CopyTo(array);
        return array;
    }
}
#pragma warning restore CA1815
// ReSharper restore StructCanBeMadeReadOnly

#pragma warning disable CA1806
[SupportedOSPlatform("linux")]
public sealed class VideoCapture : IDisposable
{
    public event Action<FrameBuffer>? FrameCaptured;

    private int fd = -1;

    private Thread? captureThread;

    private CancellationTokenSource? captureCts;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool IsOpen => fd >= 0;

    public bool IsCapturing => captureCts is { IsCancellationRequested: false };

    public void Dispose()
    {
        Close();
    }

    public bool Open(int width = 640, int height = 480)
    {
        if (IsOpen)
        {
            return false;
        }

        Width = width;
        Height = height;

        return true;
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        StopCapture();
        CloseInternal();
    }

    private void CloseInternal()
    {
        fd = -1;

        Width = 0;
        Height = 0;
    }

    public bool StartCapture()
    {
        if (!IsOpen || IsCapturing)
        {
            return false;
        }

        // Start capture loop
        captureCts = new CancellationTokenSource();
        captureThread = new Thread(() => CaptureLoop(captureCts.Token))
        {
            IsBackground = true,
            Name = "V4L2 Capture"
        };
        captureThread.Start();

        return true;
    }

    public void StopCapture()
    {
        if (!IsCapturing)
        {
            return;
        }

        captureCts?.Cancel();
        captureThread?.Join();
        captureThread = null;

        captureCts?.Dispose();
        captureCts = null;
    }

    private void CaptureLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // TODO
            var buffer = new byte[Width * Height * 2];

            var handler = FrameCaptured;
            handler?.Invoke(new FrameBuffer(buffer, buffer.Length));
        }
    }
}
