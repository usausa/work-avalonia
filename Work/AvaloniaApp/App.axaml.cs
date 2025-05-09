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

using Smart.Avalonia.Resolver;

// ReSharper disable once PartialTypeWithSinglePart
public partial class App : Application
{
    private IHost host = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        host = CreateHost();
        ResolveProvider.Default.Provider = host.Services;
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

            var navigator = host.Services.GetRequiredService<Navigator>();
            await navigator.ForwardAsync(ViewId.Menu);
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
        // TODO
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddTransient<MainWindowViewModel>();
        builder.Services.AddTransient<MenuView>();
        builder.Services.AddTransient<MenuViewModel>();
        builder.Services.AddTransient<SubView>();
        builder.Services.AddTransient<SubViewModel>();

        // Navigator
        builder.Services.AddSingleton<Navigator>(p =>
        {
            var navigator = new NavigatorConfig()
                .UseAvaloniaNavigationProvider()
                .UseServiceProvider(p)
                .UseIdViewMapper(static m =>
                {
                    // TODO
                    m.Register(ViewId.Menu, typeof(MenuView));
                    m.Register(ViewId.Sub, typeof(SubView));
                })
                .ToNavigator();
            navigator.Navigated += (_, args) =>
            {
                // for debug
                System.Diagnostics.Debug.WriteLine(
                    $"Navigated: [{args.Context.FromId}]->[{args.Context.ToId}] : stacked=[{navigator.StackedCount}]");
            };

            return navigator;
        });

        return builder.Build();
    }
}
