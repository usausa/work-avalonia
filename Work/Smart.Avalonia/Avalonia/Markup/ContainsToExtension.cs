namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;
using global::Avalonia.Media;

using Smart.Avalonia.Data;

public sealed class ContainsToBoolExtension : MarkupExtension
{
    public bool Invert { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new ContainsToBoolConverter { TrueValue = !Invert, FalseValue = Invert };
}

public sealed class ContainsToTextExtension : MarkupExtension
{
    public string? True { get; set; }

    public string? False { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new ContainsToTextConverter { TrueValue = True, FalseValue = False };
}

public sealed class ContainsToBrushExtension : MarkupExtension
{
    public IBrush True { get; set; } = Brushes.Transparent;

    public IBrush False { get; set; } = Brushes.Transparent;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new ContainsToBrushConverter { TrueValue = True, FalseValue = False };
}

public sealed class ContainsToColorExtension : MarkupExtension
{
    public Color True { get; set; } = Colors.Transparent;

    public Color False { get; set; } = Colors.Transparent;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new ContainsToColorConverter { TrueValue = True, FalseValue = False };
}
