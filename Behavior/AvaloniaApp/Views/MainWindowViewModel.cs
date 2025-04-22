namespace AvaloniaApp.Views;

using AvaloniaApp.Models;

using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial Navigator? Navigator { get; set; }
}
