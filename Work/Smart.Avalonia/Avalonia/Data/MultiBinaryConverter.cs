namespace Smart.Avalonia.Data;

using System.Globalization;

using global::Avalonia.Data.Converters;

using Smart.Avalonia.Expressions;

public sealed class MultiBinaryConverter : IMultiValueConverter
{
    public IBinaryExpression Expression { get; set; } = default!;

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var value = values[0];
        for (var i = 1; i < values.Count; i++)
        {
            value = Expression.Eval(value, values[i]);
        }

        return value;
    }
}
