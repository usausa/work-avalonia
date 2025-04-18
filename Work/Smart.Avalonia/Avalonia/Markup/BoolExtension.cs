namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

public sealed class BoolExtension : MarkupExtension
{
    public bool Value { get; set; }

    public BoolExtension(bool value)
    {
        Value = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Value;
}
