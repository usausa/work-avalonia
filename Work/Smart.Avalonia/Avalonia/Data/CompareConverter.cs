namespace Smart.Avalonia.Data;

using System.Globalization;

using global::Avalonia.Data.Converters;
using global::Avalonia.Media;

using Smart.Avalonia.Expressions;

public abstract class CompareConverter<T> : IValueConverter
{
    public ICompareExpression Expression { get; set; } = CompareExpressions.Equal;

    public T TrueValue { get; set; } = default!;

    public T FalseValue { get; set; } = default!;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Expression.Eval(value, parameter) ? TrueValue : FalseValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public sealed class CompareToBoolConverter : CompareConverter<bool>
{
    public CompareToBoolConverter()
    {
        TrueValue = true;
        FalseValue = false;
    }
}

public sealed class CompareToTextConverter : CompareConverter<string?>
{
}

public sealed class CompareToBrushConverter : CompareConverter<IBrush>
{
    public CompareToBrushConverter()
    {
        TrueValue = Brushes.Transparent;
        FalseValue = Brushes.Transparent;
    }
}

public sealed class CompareToColorConverter : CompareConverter<Color>
{
    public CompareToColorConverter()
    {
        TrueValue = Colors.Transparent;
        FalseValue = Colors.Transparent;
    }
}
