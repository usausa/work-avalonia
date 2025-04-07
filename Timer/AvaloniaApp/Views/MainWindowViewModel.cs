namespace AvaloniaApp.Views;

using System.Diagnostics;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly PeriodicTimer timer;
    private readonly CancellationTokenSource cancellationTokenSource;

    [ObservableProperty]
    public partial int Fps { get; set; }

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
            var watch = Stopwatch.StartNew();
            while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
            {
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
