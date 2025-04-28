namespace AvaloniaApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class MenuViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Message { get; set; }

    public MenuViewModel()
    {
        Message = "Hello from MenuViewModel!";
    }
}
