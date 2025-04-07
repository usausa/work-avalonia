namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

public sealed class Int64Extension : MarkupExtension
{
    public long Value { get; set; }

    public Int64Extension(long value)
    {
        Value = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Value;
}
