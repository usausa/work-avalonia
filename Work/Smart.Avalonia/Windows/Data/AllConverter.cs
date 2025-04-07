namespace Smart.Windows.Data;

using System.Globalization;

using Avalonia.Data.Converters;

public sealed class AllConverter : IMultiValueConverter
{
    public bool Invert { get; set; }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.All(value => System.Convert.ToBoolean(value, culture)) ? !Invert : Invert;
    }
}
