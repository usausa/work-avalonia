namespace AvaloniaApp.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

using static System.Net.Mime.MediaTypeNames;

public partial class MainWindow : Window
{
    public static readonly StyledProperty<Brush> BackgroundProperty =
        AvaloniaProperty.Register<Control, Brush>(
            nameof(Background),
            defaultValue: Brushes.Transparent);

    public MainWindow()
    {
        InitializeComponent();
    }
}
