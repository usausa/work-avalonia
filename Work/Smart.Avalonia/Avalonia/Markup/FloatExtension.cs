namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

public sealed class FloatExtension : MarkupExtension
{
    public float Value { get; set; }

    public FloatExtension(float value)
    {
        Value = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Value;
}
