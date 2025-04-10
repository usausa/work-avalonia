namespace Smart.Avalonia.Data;

using System.Globalization;

using global::Avalonia.Data.Converters;

using Smart.Converter;

public sealed class ObjectConvertConverter : IValueConverter
{
    public IObjectConverter Converter { get; set; } = ObjectConverter.Default;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Converter.Convert(value, targetType);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Converter.Convert(value, targetType);
    }
}
