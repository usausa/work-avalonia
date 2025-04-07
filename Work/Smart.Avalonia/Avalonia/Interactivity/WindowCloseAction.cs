namespace Smart.Avalonia.Interactivity;

using System.Windows;

using Microsoft.Xaml.Behaviors;

[TypeConstraint(typeof(Window))]
public sealed class WindowCloseAction : TriggerAction<Window>
{
    protected override void Invoke(object parameter)
    {
        AssociatedObject.Close();
    }
}
