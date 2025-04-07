namespace Smart.Avalonia.Data;

using System.Globalization;

using global::Avalonia.Data.Converters;

public sealed class AnyConverter : IMultiValueConverter
{
    public bool Invert { get; set; }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.Any(value => System.Convert.ToBoolean(value, culture)) ? !Invert : Invert;
    }
}
