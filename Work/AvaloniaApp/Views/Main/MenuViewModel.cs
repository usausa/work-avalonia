namespace AvaloniaApp.Views.Main;

using System.IO.Ports;

using Avalonia.Controls;
using Avalonia.Threading;

using AvaloniaApp.Settings;

public sealed partial class MenuViewModel : AppViewModelBase
{
    private readonly SerialPort port;

    [ObservableProperty]
    public partial string Barcode { get; set; } = default!;

    public ICommand StartCommand { get; }

    public ICommand StopCommand { get; }

    public MenuViewModel(Setting setting)
    {
        if (Design.IsDesignMode)
        {
            port = default!;
            StartCommand = default!;
            StopCommand = default!;
            return;
        }

        port = new SerialPort(setting.QrPort)
        {
            BaudRate = 19200,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None,

            ReadTimeout = 1000,
            WriteTimeout = 1000
        };

        port.Open();
        port.DiscardInBuffer();
        port.DiscardOutBuffer();

        port.DataReceived += (sender, _) =>
        {
            var sp = (SerialPort)sender;
            var data = sp.ReadExisting();
            Dispatcher.UIThread.Post(() => Barcode = data);
        };

        StartCommand = MakeDelegateCommand(() =>
        {
            var buffer = "R\r"u8.ToArray();
            port.Write(buffer, 0, buffer.Length);
        });
        StopCommand = MakeDelegateCommand(() =>
        {
            var buffer = "Z\r"u8.ToArray();
            port.Write(buffer, 0, buffer.Length);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            port.Close();
        }

        base.Dispose(disposing);
    }
}
