namespace AvaloniaApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;

using Smart.Navigation;

public class MainWindowViewModel : ObservableObject
{
    public Navigator Navigator { get; set; }

    public MainWindowViewModel(Navigator navigator)
    {
        Navigator = navigator;
    }
}
