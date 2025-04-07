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
        //builder.StartLinuxFbDev(args, "/dev/fb0");
        builder.StartLinuxDrm(args, "/dev/dri/card0", 1);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildControlApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            //.WithInterFont()
            .LogToTrace();
}
