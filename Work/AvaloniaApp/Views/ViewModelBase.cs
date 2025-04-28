namespace AvaloniaApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract class ViewModelBase : ObservableObject, INavigatorAware, INavigationEventSupport
{
    public INavigator Navigator { get; set; } = default!;

    public void OnNavigatingFrom(INavigationContext context)
    {
    }

    public void OnNavigatingTo(INavigationContext context)
    {
    }

    public void OnNavigatedTo(INavigationContext context)
    {
    }
}
