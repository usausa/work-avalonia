namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;
using global::Avalonia.Media;

using Smart.Avalonia.Data;
using Smart.Avalonia.Expressions;

public sealed class CompareToBoolExtension : MarkupExtension
{
    public ICompareExpression? Expression { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new CompareToBoolConverter { Expression = Expression ?? CompareExpressions.Equal, TrueValue = true, FalseValue = false };
}

public sealed class CompareToTextExtension : MarkupExtension
{
    public ICompareExpression? Expression { get; set; }

    public string? True { get; set; }

    public string? False { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new CompareToTextConverter { Expression = Expression ?? CompareExpressions.Equal, TrueValue = True, FalseValue = False };
}

public sealed class CompareToBrushExtension : MarkupExtension
{
    public ICompareExpression? Expression { get; set; }

    public IBrush True { get; set; } = Brushes.Transparent;

    public IBrush False { get; set; } = Brushes.Transparent;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new CompareToBrushConverter { Expression = Expression ?? CompareExpressions.Equal, TrueValue = True, FalseValue = False };
}

public sealed class CompareToColorExtension : MarkupExtension
{
    public ICompareExpression? Expression { get; set; }

    public Color True { get; set; } = Colors.Transparent;

    public Color False { get; set; } = Colors.Transparent;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new CompareToColorConverter { Expression = Expression ?? CompareExpressions.Equal, TrueValue = True, FalseValue = False };
}
