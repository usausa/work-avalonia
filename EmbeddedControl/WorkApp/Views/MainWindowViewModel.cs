namespace WorkApp.Views;

using System.Diagnostics;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private const double AccelVelocity1 = 64d / 60;
    private const double AccelVelocity2 = 48d / 60;
    private const double AccelVelocity3 = 32d / 60;
    private const double AccelVelocity4 = 16d / 60;
    private const double BrakeVelocity = 80d / 60;
    private const double DefaultVelocity = 20d / 60;

    private readonly PeriodicTimer timer;
    private readonly CancellationTokenSource cancellationTokenSource;

    [ObservableProperty]
    public partial int Fps { get; set; }

    [ObservableProperty]
    public partial int Speed { get; set; }

    [ObservableProperty]
    public partial bool Accel { get; set; }

    [ObservableProperty]
    public partial bool Brake { get; set; }

    public MainWindowViewModel()
    {
        timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000d / 60));
        cancellationTokenSource = new CancellationTokenSource();

        _ = StartTimerAsync();
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        timer.Dispose();
    }

    private async Task StartTimerAsync()
    {
        //IsRunning = true;

        try
        {
            // Low resolution
            var fps = 0;
            var speed = 0d;
            var prevSpeed = 0;
            var prevAccel = false;
            var prevBrake = false;

            var watch = Stopwatch.StartNew();
            while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
            {
                // Speed
                var brake = KeyPad.Instance.BrakePressed;
                var accel = KeyPad.Instance.AccelPressed;

                if (brake)
                {
                    speed = Math.Max(0, speed - BrakeVelocity);
                }
                else if (accel)
                {
                    var velocity = speed switch
                    {
                        < 128 => AccelVelocity1,
                        < 192 => AccelVelocity2,
                        < 224 => AccelVelocity3,
                        _ => AccelVelocity4
                    };
                    speed = Math.Min(255, speed + velocity);
                }
                else
                {
                    speed = Math.Max(0, speed - DefaultVelocity);
                }

                var currentSpeed = (int)speed;
                if ((currentSpeed != prevSpeed) ||
                    (accel != prevAccel) ||
                    (brake != prevBrake))
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Speed = currentSpeed;
                        Accel = accel;
                        Brake = brake;
                    });

                    prevSpeed = currentSpeed;
                    prevAccel = accel;
                    prevBrake = brake;
                }

                // FPS
                fps++;
                if (watch.ElapsedMilliseconds > 1000)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Fps = fps;
                    });

                    fps = 0;
                    watch.Restart();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        finally
        {
            //IsRunning = false;
        }
    }
}
