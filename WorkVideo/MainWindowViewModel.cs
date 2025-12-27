namespace Example.Video4Linux2.AvaloniaApp;

using Avalonia;

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

using Example.Video4Linux2.AvaloniaApp.Helper;
using SkiaSharp;
using System.Runtime.Versioning;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public partial class MainWindowViewModel : ExtendViewModelBase
{
    private const int Width = 640;
    private const int Height = 480;
    private const int BitmapBufferSize = Width * Height * 4;
    private const double Alpha = 0.5;

    private readonly IDispatcher dispatcher;

    private readonly BufferManager bufferManager;

    private readonly VideoCapture capture;

    private readonly WriteableBitmap? bitmap;

    private readonly DispatcherTimer statsTimer;

    private readonly FaceDetector faceDetector;

    private int frameCount;
    private int lastFrameCount;
    private int[] lastGcCounts = new int[3];

    private double smoothedFps;
    private double smoothedGc0;
    private double smoothedGc1;
    private double smoothedGc2;

    private byte[] faceData = [];

    [ObservableProperty]
    public partial WriteableBitmap? Bitmap { get; set; }

    public ObservableCollection<FaceBox> FaceBoxes { get; } = new();

    [ObservableProperty]
    public partial float Fps { get; set; }

    [ObservableProperty]
    public partial float Gc0PerSec { get; set; }

    [ObservableProperty]
    public partial float Gc1PerSec { get; set; }

    [ObservableProperty]
    public partial float Gc2PerSec { get; set; }

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

        statsTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        statsTimer.Tick += OnStatsTimerTick;

        for (var i = 0; i < 3; i++)
        {
            lastGcCounts[i] = GC.CollectionCount(i);
        }

        faceDetector = new FaceDetector("version-RFB-320.onnx");

        // Load face data
        var file = @"D:\学習データ\people640x480.png";
        if (File.Exists(file))
        {
            using var inputStream = File.OpenRead(file);
            using var originalBitmap = SKBitmap.Decode(inputStream);

            var width = originalBitmap.Width;
            var height = originalBitmap.Height;

            // Read
            faceData = new byte[width * height * 3];
            var index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixel = originalBitmap.GetPixel(x, y);
                    faceData[index++] = pixel.Red;
                    faceData[index++] = pixel.Green;
                    faceData[index++] = pixel.Blue;
                }
            }

        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            statsTimer.Stop();
            statsTimer.Tick -= OnStatsTimerTick;
            capture.Dispose();
            capture.FrameCaptured -= CaptureOnFrameCaptured;
            bufferManager.Dispose();
            bitmap?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void StartCapture()
    {
        capture.Open();
        capture.StartCapture();
        statsTimer.Start();
    }

    private void StopCapture()
    {
        statsTimer.Stop();
        capture.StopCapture();
        capture.Close();
    }

    private void CaptureOnFrameCaptured(FrameBuffer frame)
    {
        var slot = bufferManager.NextSlot();
        lock (slot.Lock)
        {
            ImageHelper.ConvertYUYV2RGBA(frame.AsSpan(), slot.Buffer);

            // 画像認識
            faceDetector.Detect(faceData, Width, Height);
            slot.FaceBoxes.Clear();
            slot.FaceBoxes.AddRange(faceDetector.DetectedFaceBoxes);

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

            FaceBoxes.Clear();
            foreach (var box in slot.FaceBoxes)
            {
                FaceBoxes.Add(box);
            }
        }

        // Disable cache & update
        Bitmap = null;
        Bitmap = bitmap;

        Interlocked.Increment(ref frameCount);
    }

    private void OnStatsTimerTick(object? sender, EventArgs e)
    {
        var currentFrameCount = frameCount;
        var fps = currentFrameCount - lastFrameCount;
        lastFrameCount = currentFrameCount;

        smoothedFps = (Alpha * fps) + ((1 - Alpha) * smoothedFps);
        Fps = (float)smoothedFps;

        for (var i = 0; i < 3; i++)
        {
            var currentGcCount = GC.CollectionCount(i);
            var gcPerSec = currentGcCount - lastGcCounts[i];
            lastGcCounts[i] = currentGcCount;

            switch (i)
            {
                case 0:
                    smoothedGc0 = (Alpha * gcPerSec) + ((1 - Alpha) * smoothedGc0);
                    Gc0PerSec = (float)smoothedGc0;
                    break;
                case 1:
                    smoothedGc1 = (Alpha * gcPerSec) + ((1 - Alpha) * smoothedGc1);
                    Gc1PerSec = (float)smoothedGc1;
                    break;
                case 2:
                    smoothedGc2 = (Alpha * gcPerSec) + ((1 - Alpha) * smoothedGc2);
                    Gc2PerSec = (float)smoothedGc2;
                    break;
            }
        }
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
        fd = 1;

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
        // ReSharper disable once AsyncVoidLambda
        captureThread = new Thread(async () => await CaptureLoop(captureCts.Token))
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

    private async Task CaptureLoop(CancellationToken cancellationToken)
    {
        var image = new ImageGenerator();
        image.UpdateGrayLevel();

        while (!cancellationToken.IsCancellationRequested)
        {
            var handler = FrameCaptured;
            handler?.Invoke(new FrameBuffer(image.GetBuffer(), image.GetBuffer().Length));

            image.UpdateGrayLevel();
            await Task.Delay(33, cancellationToken);
        }
    }
}

public sealed class ImageGenerator
{
    private const int Width = 640;
    private const int Height = 480;
    private const int BytesPerPixel = 2; // YUYV形式: 2ピクセルで4バイト、1ピクセルあたり2バイト
    private const int BufferSize = Width * Height * BytesPerPixel;

    private readonly byte[] yuyvBuffer;
    private float grayLevel;
    private float direction;

    public ImageGenerator()
    {
        yuyvBuffer = new byte[BufferSize];
        grayLevel = 128f;
        direction = 1f;

        // 初期化：グレーで塗りつぶし
        FillWithGray((byte)grayLevel);
    }

    /// <summary>
    /// YUYV形式でグレーで塗りつぶす
    /// YUYV形式: Y0 U Y1 V (2ピクセル分で4バイト)
    /// グレーの場合: Y=輝度値, U=128, V=128
    /// </summary>
    private void FillWithGray(byte yValue)
    {
        var buffer = yuyvBuffer.AsSpan();

        // 4バイト単位で処理（2ピクセル分）
        for (var i = 0; i < buffer.Length; i += 4)
        {
            buffer[i] = yValue; // Y0
            buffer[i + 1] = 128;    // U (色差: グレーの場合は128)
            buffer[i + 2] = yValue; // Y1
            buffer[i + 3] = 128;    // V (色差: グレーの場合は128)
        }
    }

    /// <summary>
    /// グレーレベルを更新（輝度値のみを変更）
    /// </summary>
    public void UpdateGrayLevel()
    {
        // グレーレベルを変化させる（16〜235の範囲で往復）
        grayLevel += direction * 2f;

        if (grayLevel >= 235f)
        {
            grayLevel = 235f;
            direction = -1f;
        }
        else if (grayLevel <= 16f)
        {
            grayLevel = 16f;
            direction = 1f;
        }

        var yValue = (byte)grayLevel;
        var buffer = yuyvBuffer.AsSpan();

        // 輝度値(Y)のみを効率的に更新
        // Y0とY1の位置だけを更新（U, Vは変更不要）
        for (var i = 0; i < buffer.Length; i += 4)
        {
            buffer[i] = yValue; // Y0
            buffer[i + 2] = yValue; // Y1
        }
    }

    public byte[] GetBuffer() => yuyvBuffer;
}
