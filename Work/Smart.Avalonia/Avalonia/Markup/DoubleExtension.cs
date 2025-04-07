namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

public sealed class DoubleExtension : MarkupExtension
{
    public double Value { get; set; }

    public DoubleExtension(double value)
    {
        Value = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Value;
}
