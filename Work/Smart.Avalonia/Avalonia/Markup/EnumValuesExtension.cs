namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

public sealed class EnumValuesExtension : MarkupExtension
{
    public Type Type { get; set; }

    public EnumValuesExtension(Type type)
    {
        Type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => Enum.GetValues(Type);
}
