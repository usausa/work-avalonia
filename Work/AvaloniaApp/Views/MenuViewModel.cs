namespace AvaloniaApp.Views;

public sealed class MenuViewModel : ViewModelBase
{
    public string Message { get; set; }

    public MenuViewModel()
    {
        Message = "Hello from MenuViewModel!";
    }
}
