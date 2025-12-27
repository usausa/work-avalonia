namespace Example.Video4Linux2.AvaloniaApp.Controls;

using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Example.Video4Linux2.AvaloniaApp.Helper;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public class FaceBoxOverlay : Avalonia.Controls.Control
{
    public static readonly StyledProperty<ObservableCollection<FaceBox>?> FaceBoxesProperty =
        AvaloniaProperty.Register<FaceBoxOverlay, ObservableCollection<FaceBox>?>(nameof(FaceBoxes));

    public static readonly StyledProperty<Color> LowScoreColorProperty =
        AvaloniaProperty.Register<FaceBoxOverlay, Color>(nameof(LowScoreColor), Colors.Yellow);

    public static readonly StyledProperty<Color> HighScoreColorProperty =
        AvaloniaProperty.Register<FaceBoxOverlay, Color>(nameof(HighScoreColor), Colors.Red);

    public ObservableCollection<FaceBox>? FaceBoxes
    {
        get => GetValue(FaceBoxesProperty);
        set => SetValue(FaceBoxesProperty, value);
    }

    public Color LowScoreColor
    {
        get => GetValue(LowScoreColorProperty);
        set => SetValue(LowScoreColorProperty, value);
    }

    public Color HighScoreColor
    {
        get => GetValue(HighScoreColorProperty);
        set => SetValue(HighScoreColorProperty, value);
    }

    static FaceBoxOverlay()
    {
        AffectsRender<FaceBoxOverlay>(FaceBoxesProperty, LowScoreColorProperty, HighScoreColorProperty);
    }

    public FaceBoxOverlay()
    {
        ClipToBounds = false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FaceBoxesProperty)
        {
            if (change.OldValue is ObservableCollection<FaceBox> oldCollection)
            {
                oldCollection.CollectionChanged -= OnFaceBoxesCollectionChanged;
            }

            if (change.NewValue is ObservableCollection<FaceBox> newCollection)
            {
                newCollection.CollectionChanged += OnFaceBoxesCollectionChanged;
            }
        }
    }

    private void OnFaceBoxesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (FaceBoxes is null || FaceBoxes.Count == 0)
        {
            return;
        }

        var customDrawOp = new FaceBoxDrawOperation(
            new Rect(0, 0, Bounds.Width, Bounds.Height),
            FaceBoxes.ToArray(),
            LowScoreColor,
            HighScoreColor);

        context.Custom(customDrawOp);
    }

    private class FaceBoxDrawOperation : ICustomDrawOperation
    {
        private readonly Rect bounds;
        private readonly FaceBox[] faceBoxes;
        private readonly Color lowScoreColor;
        private readonly Color highScoreColor;

        public FaceBoxDrawOperation(Rect bounds, FaceBox[] faceBoxes, Color lowScoreColor, Color highScoreColor)
        {
            this.bounds = bounds;
            this.faceBoxes = faceBoxes;
            this.lowScoreColor = lowScoreColor;
            this.highScoreColor = highScoreColor;
        }

        public Rect Bounds => bounds;

        public void Dispose()
        {
        }

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }

        public bool HitTest(Point p)
        {
            return false;
        }

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null)
            {
                return;
            }

            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;

            var scaleX = (float)bounds.Width;
            var scaleY = (float)bounds.Height;

            using var paint = new SKPaint();
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            paint.IsAntialias = true;

            using var textPaint = new SKPaint();
            textPaint.Style = SKPaintStyle.Fill;
            textPaint.TextSize = 12;
            textPaint.IsAntialias = true;

            foreach (var faceBox in faceBoxes)
            {
                var left = faceBox.Left * scaleX;
                var top = faceBox.Top * scaleY;
                var right = faceBox.Right * scaleX;
                var bottom = faceBox.Bottom * scaleY;

                var rect = new SKRect(left, top, right, bottom);

                // Interpolate color based on confidence (0.0 - 1.0)
                var color = InterpolateColor(lowScoreColor, highScoreColor, faceBox.Confidence);
                paint.Color = new SKColor(color.R, color.G, color.B, color.A);
                textPaint.Color = new SKColor(color.R, color.G, color.B, color.A);

                // Draw rectangle
                canvas.DrawRect(rect, paint);

                // Draw confidence text at bottom-right inside the box
                var confidenceText = $"{faceBox.Confidence:P0}";
                var textBounds = new SKRect();
                textPaint.MeasureText(confidenceText, ref textBounds);

                var textX = right - textBounds.Width - 4;
                var textY = bottom - 4;

                canvas.DrawText(confidenceText, textX, textY, textPaint);
            }
        }

        private static Color InterpolateColor(Color lowColor, Color highColor, float confidence)
        {
            // Clamp confidence to [0, 1]
            var t = Math.Clamp(confidence, 0f, 1f);

            var r = (byte)(lowColor.R + (highColor.R - lowColor.R) * t);
            var g = (byte)(lowColor.G + (highColor.G - lowColor.G) * t);
            var b = (byte)(lowColor.B + (highColor.B - lowColor.B) * t);
            var a = (byte)(lowColor.A + (highColor.A - lowColor.A) * t);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
