namespace AvaloniaApp.Views;

using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaApp.Models;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void SetButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (MainWindowViewModel)DataContext!;
        vm.Navigator = new Navigator();
    }

    private void ClearButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (MainWindowViewModel)DataContext!;
        vm.Navigator = null;
    }
}
