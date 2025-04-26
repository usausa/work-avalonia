namespace Smart.Avalonia.Markup;

using Smart.Avalonia.Data;

using global::Avalonia.Markup.Xaml;
using global::Avalonia.Media;

public sealed class BoolToTextExtension : MarkupExtension
{
    public string? True { get; set; }

    public string? False { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new BoolToTextConverter { TrueValue = True, FalseValue = False };
}

public sealed class BoolToBrushExtension : MarkupExtension
{
    public IBrush True { get; set; } = Brushes.Transparent;

    public IBrush False { get; set; } = Brushes.Transparent;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new BoolToBrushConverter { TrueValue = True, FalseValue = False };
}

public sealed class BoolToColorExtension : MarkupExtension
{
    public Color True { get; set; } = Colors.Transparent;

    public Color False { get; set; } = Colors.Transparent;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new BoolToColorConverter { TrueValue = True, FalseValue = False };
}
