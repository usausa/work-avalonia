namespace Smart.Avalonia.Interactivity;

using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

using Smart.Avalonia.Messaging;

[TypeConstraint(typeof(DependencyObject))]
public sealed class ValidationErrorFocusAction : TriggerAction<DependencyObject>
{
    protected override void Invoke(object parameter)
    {
        var element = AssociatedObject.FindChildren<UIElement>().FirstOrDefault(Validation.GetHasError);
        if (element is not null)
        {
            element.Focus();

            if (parameter is CancelMessage message)
            {
                message.Cancel = true;
            }
        }
    }
}
