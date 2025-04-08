namespace WorkApp;

using Smart.Threading;

public class KeyPad
{
    public static KeyPad Instance { get; } = new();

    private readonly AtomicBoolean accelPressed = new();

    private readonly AtomicBoolean brakePressed = new();

    public bool AccelPressed
    {
        get => accelPressed.Value;
        set => accelPressed.Value = value;
    }

    public bool BrakePressed
    {
        get => brakePressed.Value;
        set => brakePressed.Value = value;
    }
}
