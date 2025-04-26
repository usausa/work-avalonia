namespace Smart.Avalonia.Markup;

using global::Avalonia.Markup.Xaml;
using global::Avalonia.Media;

using Smart.Avalonia.Data;

public sealed class NullToBoolExtension : MarkupExtension
{
    public bool Invert { get; set; }

    public bool HandleEmptyString { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new NullToBoolConverter { NullValue = !Invert, NonNullValue = Invert, HandleEmptyString = HandleEmptyString };
}

public sealed class NullToTextExtension : MarkupExtension
{
    public string? Null { get; set; }

    public string? NonNull { get; set; }

    public bool HandleEmptyString { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new NullToTextConverter { NullValue = Null, NonNullValue = NonNull, HandleEmptyString = HandleEmptyString };
}

public sealed class NullToBrushExtension : MarkupExtension
{
    public IBrush Null { get; set; } = Brushes.Transparent;

    public IBrush NonNull { get; set; } = Brushes.Transparent;

    public bool HandleEmptyString { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new NullToBrushConverter { NullValue = Null, NonNullValue = NonNull, HandleEmptyString = HandleEmptyString };
}

public sealed class NullToColorExtension : MarkupExtension
{
    public Color Null { get; set; } = Colors.Transparent;

    public Color NonNull { get; set; } = Colors.Transparent;

    public bool HandleEmptyString { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new NullToColorConverter { NullValue = Null, NonNullValue = NonNull, HandleEmptyString = HandleEmptyString };
}
