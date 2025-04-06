namespace AvaloniaApp.Views;

using Avalonia.Controls;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if !DEBUG
        Position = new Avalonia.PixelPoint(0, 0);
        WindowState = WindowState.FullScreen;
#endif
    }
}
