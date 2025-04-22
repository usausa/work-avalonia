namespace AvaloniaApp.Models;

using Avalonia.Controls;
using Avalonia;

public static class NavigationContainerBehavior
{
    public static readonly AttachedProperty<INavigator?> NavigatorProperty =
        AvaloniaProperty.RegisterAttached<Canvas, INavigator?>("Navigator", typeof(NavigationContainerBehavior));

    public static INavigator? GetNavigator(Canvas canvas) =>
        canvas.GetValue(NavigatorProperty);

    public static void SetNavigator(Canvas canvas, INavigator? value) =>
        canvas.SetValue(NavigatorProperty, value);

    static NavigationContainerBehavior()
    {
        NavigatorProperty.Changed.AddClassHandler<Canvas>(OnNavigatorChanged);
    }

    private static void OnNavigatorChanged(Canvas canvas, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.OldValue is INavigator oldNavigator)
        {
            oldNavigator.UpdateCanvas(null);
        }
        if (e.NewValue is INavigator navigator)
        {
            navigator.UpdateCanvas(canvas);
        }
    }
}
