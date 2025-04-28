namespace AvaloniaApp.Views;

using Smart.Navigation;

public class MainWindowViewModel : ViewModelBase
{
    public Navigator Navigator { get; set; }

    public MainWindowViewModel(Navigator navigator)
    {
        Navigator = navigator;
    }
}
