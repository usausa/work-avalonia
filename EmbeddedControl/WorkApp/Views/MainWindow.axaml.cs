namespace WorkApp.Views;

using Avalonia.Controls;
using Avalonia.Input;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.A)
        {
            KeyPad.Instance.AccelPressed = true;
        }
        if (e.Key == Key.S)
        {
            KeyPad.Instance.BrakePressed = true;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.A)
        {
            KeyPad.Instance.AccelPressed = false;
        }
        if (e.Key == Key.S)
        {
            KeyPad.Instance.BrakePressed = false;
        }
    }
}
