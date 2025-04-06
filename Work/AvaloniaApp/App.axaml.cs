namespace AvaloniaApp;

using System.Runtime.InteropServices;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using AvaloniaApp.Settings;
using AvaloniaApp.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

// ReSharper disable once PartialTypeWithSinglePart
public partial class App : Application
{
    private IHost host = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        host = CreateHost();
        // TODO
        //ResolveProvider.Default.Provider = host.Services;
    }

    // ReSharper disable once AsyncVoidMethod
    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.Exit += async (_, _) =>
            {
                await host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                host.Dispose();
            };

            // TODO
            desktop.MainWindow = host.Services.GetRequiredService<MainWindow>();

            // Log
            var log = host.Services.GetRequiredService<ILogger<App>>();
            var environment = host.Services.GetRequiredService<IHostEnvironment>();
            ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);

            log.InfoStartup();
            log.InfoStartupSettingsRuntime(RuntimeInformation.OSDescription, RuntimeInformation.FrameworkDescription, RuntimeInformation.RuntimeIdentifier);
            log.InfoStartupSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
            log.InfoStartupSettingsThreadPool(workerThreads, completionPortThreads);
            log.InfoStartupApplication(environment.ApplicationName, typeof(App).Assembly.GetName().Version);
            log.InfoStartupEnvironment(environment.EnvironmentName, environment.ContentRootPath);
        }

        base.OnFrameworkInitializationCompleted();

        await host.StartAsync().ConfigureAwait(false);
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder();

        // Log
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog(options =>
        {
            options.ReadFrom.Configuration(builder.Configuration);
        });

        // Setting
        builder.Services.Configure<Setting>(builder.Configuration.GetSection("Setting"));

        // View
        builder.Services.AddSingleton<MainWindow>();

        // TODO

        return builder.Build();
    }
}
