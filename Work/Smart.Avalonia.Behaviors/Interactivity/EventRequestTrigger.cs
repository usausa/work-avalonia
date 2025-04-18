namespace Smart.Avalonia.Interactivity;

using Smart.Avalonia.Messaging;

public sealed class EventRequestTrigger : RequestTriggerBase<ParameterEventArgs>
{
    protected override void OnEventRequest(object? sender, ParameterEventArgs e)
    {
        InvokeActions(e.Parameter);
    }
}
