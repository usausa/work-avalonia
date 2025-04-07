namespace Smart.Avalonia.Data;

using System.Globalization;

using global::Avalonia.Data.Converters;

public sealed class AllConverter : IMultiValueConverter
{
    public bool Invert { get; set; }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.All(value => System.Convert.ToBoolean(value, culture)) ? !Invert : Invert;
    }
}
