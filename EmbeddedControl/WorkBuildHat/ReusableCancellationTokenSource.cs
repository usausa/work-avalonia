namespace WorkBuildHat;

public sealed class ReusableCancellationTokenSource : IDisposable
{
    private CancellationTokenSource cts = new();

    public CancellationToken Token => cts.Token;

    public void Dispose()
    {
        cts.Dispose();
    }

    public void Reset()
    {
        if (!cts.TryReset())
        {
            cts.Dispose();
            cts = new CancellationTokenSource();
        }
    }

    public void CancelAfter(int millisecondsDelay) => cts.CancelAfter(millisecondsDelay);
}
