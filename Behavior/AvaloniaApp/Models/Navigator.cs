namespace AvaloniaApp.Models;

using System.Diagnostics;

using Avalonia.Controls;

public interface INavigator
{
    void UpdateCanvas(Canvas? canvas);
}

public sealed class Navigator : INavigator
{
    public void UpdateCanvas(Canvas? canvas)
    {
        Debug.WriteLine($"* {canvas?.GetHashCode()}");
    }
}
