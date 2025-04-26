namespace Smart.Avalonia.Data;

using System.Globalization;

using global::Avalonia.Data.Converters;

public sealed class ParameterEqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, true) ? parameter : null;
    }
}
