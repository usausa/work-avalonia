namespace Smart.Avalonia.Resolver;

using global::Avalonia;
using global::Avalonia.Controls;

public static class DataContextResolver
{
    public static readonly AttachedProperty<Type> TypeProperty =
        AvaloniaProperty.RegisterAttached<Control, Type>("Type", typeof(DataContextResolver));

    public static readonly AttachedProperty<bool> DisposeOnChangedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("DisposeOnChanged", typeof(DataContextResolver));

    public static Type GetType(Control control) =>
        control.GetValue(TypeProperty);

    public static void SetType(Control control, Type value) =>
        control.SetValue(TypeProperty, value);

    public static bool GetDisposeOnChanged(Control control) =>
        control.GetValue(DisposeOnChangedProperty);

    public static void SetDisposeOnChanged(Control control, bool value) =>
        control.SetValue(DisposeOnChangedProperty, value);

    static DataContextResolver()
    {
        TypeProperty.Changed.AddClassHandler<Control>(HandleTypePropertyChanged);
    }

    private static void HandleTypePropertyChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (control.DataContext is IDisposable disposable && GetDisposeOnChanged(control))
        {
            disposable.Dispose();
        }

        control.DataContext = e.NewValue is not null ? ResolveProvider.Default.GetService((Type)e.NewValue) : null;
    }
}
