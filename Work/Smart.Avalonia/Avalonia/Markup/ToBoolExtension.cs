namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;

using Smart.Avalonia.Data;

public sealed class TextToBoolExtension : MarkupExtension
{
    public string? True { get; set; }

    public string? False { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new TextToBoolConverter { TrueValue = True, FalseValue = False };
}

public sealed class IntToBoolExtension : MarkupExtension
{
    public int True { get; set; }

    public int False { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new IntToBoolConverter { TrueValue = True, FalseValue = False };
}
