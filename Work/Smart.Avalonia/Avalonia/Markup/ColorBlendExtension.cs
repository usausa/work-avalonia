namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;
using global::Avalonia.Media;

using Smart.Avalonia.Data;

public sealed class ColorBlendExtension : MarkupExtension
{
    public Color Color { get; set; }

    public double Raito { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new ColorBlendConverter { Color = Color, Raito = Raito };
}
