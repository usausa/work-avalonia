namespace AvaloniaApp.Views;

using System.Diagnostics;

using Avalonia.Xaml.Interactivity;

using CommunityToolkit.Mvvm.ComponentModel;

using Smart.Avalonia.Input;
using Smart.Avalonia.Messaging;

public sealed partial class MenuViewModel : ViewModelBase
{
    public string Message { get; set; }

    public Messenger Messenger { get; } = new();

    public EventRequest<string> EventRequest { get; } = new();

    public ResolveRequest<bool> ResolveRequest { get; } = new();

    public CancelRequest CancelRequest { get; } = new();

    [ObservableProperty]
    public partial bool State { get; set; }

    [ObservableProperty]
    public partial int Value1 { get; set; }

    [ObservableProperty]
    public partial int Value2 { get; set; }

    public ICommand Test1Command { get; }

    public ICommand Test2Command { get; }

    public ICommand Test3Command { get; }

    public ICommand Test4Command { get; }

    public ICommand Test5Command { get; }

    public ICommand Test6Command { get; }

    public ICommand Test7Command { get; }

    public ICommand Test8Command { get; }

    public MenuViewModel()
    {
        Message = "Hello from MenuViewModel!";
        Test1Command = new DelegateCommand(() =>
        {
            Value1++;
        });
        Test2Command = new DelegateCommand(() =>
        {
            Value2++;
        });
        Test3Command = new DelegateCommand(() =>
        {
            Messenger.Send(string.Empty, "Test");
        });
        Test4Command = new DelegateCommand(() =>
        {
            EventRequest.Request("123");
        });
        Test5Command = new DelegateCommand(() =>
        {
            var ret = ResolveRequest.Resolve();
            Debug.WriteLine($"*Result={ret}");
        });
        Test6Command = new DelegateCommand(() =>
        {
            var ret = CancelRequest.IsCancel();
            Debug.WriteLine($"*Cancel={ret}");
        });
        Test7Command = new DelegateCommand(() =>
        {
            State = !State;
        });
        Test8Command = new DelegateCommand(() =>
        {
            Navigator.Forward(ViewId.Sub);
        });
    }
}

public sealed class DebugAction : StyledElementAction
{
    public override object Execute(object? sender, object? parameter)
    {
        Debug.WriteLine($"*Action called. parameter=[{parameter}]");
        return true;
    }
}
