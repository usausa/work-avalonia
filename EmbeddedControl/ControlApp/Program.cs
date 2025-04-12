namespace ControlApp;

using Avalonia;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = BuildControlApp();
        if (args.Length > 0)
        {
            if (args[0].StartsWith("/dev/fb", StringComparison.Ordinal))
            {
                builder.StartLinuxFbDev(args, args[0]);
            }
            else if (args[0].StartsWith("/dev/dri/card", StringComparison.Ordinal))
            {
                builder.StartLinuxDrm(args, args[0], 1);
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildControlApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .WithInterFont()
            .LogToTrace();
}
