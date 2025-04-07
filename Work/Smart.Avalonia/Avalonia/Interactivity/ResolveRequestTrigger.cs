namespace Smart.Avalonia.Interactivity;

using Smart.Avalonia.Messaging;

public sealed class ResolveRequestTrigger : RequestTriggerBase<ResultEventArgs>
{
    protected override void OnEventRequest(object? sender, ResultEventArgs e)
    {
        InvokeActions(e);
    }
}
