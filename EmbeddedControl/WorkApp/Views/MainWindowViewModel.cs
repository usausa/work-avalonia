namespace WorkApp.Views;

using System.Diagnostics;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private const double AccelVelocity = 32d / 60;
    private const double BreakVelocity = 48d / 60;
    private const double DefaultVelocity = 20d / 60;

    private readonly PeriodicTimer timer;
    private readonly CancellationTokenSource cancellationTokenSource;

    [ObservableProperty]
    public partial int Fps { get; set; }

    [ObservableProperty]
    public partial int Speed { get; set; }

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

            var watch = Stopwatch.StartNew();
            while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
            {
                // Speed
                if (KeyPad.Instance.BrakePressed)
                {
                    speed = Math.Max(0, speed - BreakVelocity);
                }
                else if (KeyPad.Instance.AccelPressed)
                {
                    speed = Math.Min(255, speed + AccelVelocity);
                }
                else
                {
                    speed = Math.Max(0, speed - DefaultVelocity);
                }

                var currentSpeed = (int)speed;
                if (currentSpeed != prevSpeed)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Speed = currentSpeed;
                    });

                    prevSpeed = currentSpeed;
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
