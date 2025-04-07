namespace ControlApp.Views;

using System.Diagnostics;

using Gamepad;

using Iot.Device.BuildHat;

public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly Brick brick = new("/dev/serial0");

    private readonly GamepadController controller = new();

    public string Greeting { get; } = "Welcome to Avalonia!";

    public MainWindowViewModel()
    {
        // TODO timer方式？
        controller.AxisChanged += (_, args) =>
        {
            // Handle axis change
            Debug.WriteLine($"Axis {args.Axis} changed to {args.Value}");
        };
        controller.ButtonChanged += (_, args) =>
        {
            // Handle button change
            Debug.WriteLine($"Button {args.Button} changed to {args.Pressed}");
        };
    }

    public void Dispose()
    {
        controller.Dispose();
        brick.Dispose();
    }
}
