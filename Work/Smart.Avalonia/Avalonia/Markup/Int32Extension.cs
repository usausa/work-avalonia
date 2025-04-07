namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

public sealed class Int32Extension : MarkupExtension
{
    public int Value { get; set; }

    public Int32Extension(int value)
    {
        Value = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Value;
}
