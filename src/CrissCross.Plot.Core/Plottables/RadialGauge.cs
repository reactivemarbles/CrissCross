// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A radial gauge rendered with configurable angles, labels, and styles.</summary>
internal class RadialGauge
{
    /// <summary>The number of degrees in a half turn.</summary>
    private const double HalfTurnDegrees = 180;

    /// <summary>The number of degrees in a full turn.</summary>
    private const double FullTurnDegrees = 360;

    /// <summary>The divisor used to convert a label's angular width to its center.</summary>
    private const double HalfTurnDivisor = 2;

    /// <summary>The tolerance used when comparing a label position to an endpoint.</summary>
    private const double EndpointTolerance = 1e-10;

    /// <summary>Converts radians to degrees.</summary>
    private const double DegreesPerRadian = HalfTurnDegrees / Math.PI;

    /// <summary>Gets or sets the base angle of the gauge in degrees.</summary>
    public double StartAngle { get; set; }

    /// <summary>Gets or sets the current gauge sweep in degrees.</summary>
    public double SweepAngle { get; set; }

    /// <summary>Gets or sets the maximum gauge size in degrees.</summary>
    public double MaximumSizeAngle { get; set; }

    /// <summary>Gets or sets the angle where the background starts in degrees.</summary>
    public double BackStartAngle { get; set; }

    /// <summary>Gets or sets whether the gauge sweeps clockwise.</summary>
    public bool Clockwise { get; set; }

    /// <summary>Gets or sets whether the background is a complete circle.</summary>
    public bool CircularBackground { get; set; } = true;

    /// <summary>Gets or sets the font used to render the gauge label.</summary>
    public FontStyle Font { get; set; } = new();

    /// <summary>Gets or sets the font size relative to the line thickness.</summary>
    public double FontSizeFraction { get; set; }

    /// <summary>Gets or sets the text displayed along the gauge.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the label position along the gauge sweep.</summary>
    public double LabelPositionFraction { get; set; }

    /// <summary>Gets or sets the gauge width in pixels.</summary>
    public double Width { get; set; }

    /// <summary>Gets or sets the gauge foreground color.</summary>
    public Color Color { get; set; }

    /// <summary>Gets or sets the gauge background color.</summary>
    public Color BackgroundColor { get; set; }

    /// <summary>Gets or sets the stroke cap at the gauge base.</summary>
    public SKStrokeCap StartCap { get; set; } = SKStrokeCap.Round;

    /// <summary>Gets or sets the stroke cap at the gauge tip.</summary>
    public SKStrokeCap EndCap { get; set; } = SKStrokeCap.Round;

    /// <summary>Gets or sets how the gauge is positioned relative to its center.</summary>
    public RadialGaugeMode Mode { get; set; }

    /// <summary>Gets or sets whether gauge labels are rendered.</summary>
    public bool ShowLabels { get; set; }

    /// <summary>Gets the angle swept by the gauge background.</summary>
    private double BackAngleSweep
    {
        get
        {
            double maximumBackgroundAngle = CircularBackground ? FullTurnDegrees : MaximumSizeAngle;
            return Clockwise ? maximumBackgroundAngle : -maximumBackgroundAngle;
        }
    }

    /// <summary>Reduces an angle to the range from zero to one full turn.</summary>
    /// <param name="angle">The angle to reduce, in degrees.</param>
    /// <returns>The equivalent angle in the range from zero to one full turn.</returns>
    internal static double ReduceAngle(double angle)
    {
        angle %= FullTurnDegrees;
        if (angle < 0)
        {
            angle += FullTurnDegrees;
        }

        return angle;
    }

    /// <summary>Renders the gauge background, foreground, and label.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="radius">The gauge radius in pixels.</param>
    internal virtual void Render(RenderPack rp, float radius)
    {
        RenderBackground(rp, radius);
        RenderGaugeForeground(rp, radius);
        RenderGaugeLabels(rp, radius);
    }

    /// <summary>Renders the foreground arc of the gauge.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="radius">The gauge radius in pixels.</param>
    internal void RenderGaugeForeground(RenderPack rp, float radius)
    {
        using SKPaint paint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = (float)Width, StrokeCap = StartCap, Color = new(Color.ARGB) };
        using SKPathBuilder builder = new();
        builder.AddArc(
            new(rp.FigureRect.BottomCenter.X - radius, rp.FigureRect.LeftCenter.Y - radius, rp.FigureRect.BottomCenter.X + radius, rp.FigureRect.LeftCenter.Y + radius),
            (float)StartAngle,
            (float)SweepAngle);
        using SKPath path = builder.Detach();
        rp.Canvas.DrawPath(path, paint);
    }

    /// <summary>Renders the background arc unless the gauge is standalone.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="radius">The gauge radius in pixels.</param>
    private void RenderBackground(RenderPack rp, float radius)
    {
        if (Mode == RadialGaugeMode.SingleGauge)
        {
            return;
        }

        using SKPaint paint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = (float)Width, StrokeCap = StartCap, Color = new(BackgroundColor.ARGB) };
        using SKPathBuilder builder = new();
        builder.AddArc(
            new(rp.FigureRect.BottomCenter.X - radius, rp.FigureRect.LeftCenter.Y - radius, rp.FigureRect.BottomCenter.X + radius, rp.FigureRect.LeftCenter.Y + radius),
            (float)BackStartAngle,
            (float)BackAngleSweep);
        using SKPath path = builder.Detach();
        rp.Canvas.DrawPath(path, paint);
    }

    /// <summary>Renders the label along the gauge arc when enabled.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="radius">The gauge radius in pixels.</param>
    private void RenderGaugeLabels(RenderPack rp, float radius)
    {
        if (!ShowLabels || Label.Length == 0)
        {
            return;
        }

        rp.Paint.TextSize = (float)Width * (float)FontSizeFraction;
        rp.Paint.IsAntialias = true;
        rp.Paint.SubpixelText = true;
        rp.Paint.Color = Font.Color;
        rp.Paint.SKTypeface = Font.Typeface;
        PixelRect textBounds = rp.Paint.MeasureText(Label);
        double textAngle = DegreesPerRadian * textBounds.Width / radius;
        double angle = ReduceAngle(StartAngle + (SweepAngle * LabelPositionFraction) - (textAngle / HalfTurnDivisor));
        bool isBelow = angle <= HalfTurnDegrees && angle > 0;
        float additionalSpace = 0;
        if (Math.Abs(LabelPositionFraction - 1) < EndpointTolerance || Math.Abs(LabelPositionFraction) < EndpointTolerance)
        {
            additionalSpace += SweepAngle > 0 == isBelow ? 1 : -1;
        }

        if (SweepAngle > 0 == isBelow)
        {
            ReversePath();
        }

        using SKPathBuilder builder = new();
        builder.AddArc(
            new(rp.FigureRect.BottomCenter.X - radius, rp.FigureRect.LeftCenter.Y - radius, rp.FigureRect.BottomCenter.X + radius, rp.FigureRect.LeftCenter.Y + radius),
            (float)StartAngle,
            (float)SweepAngle);
        using SKPath path = builder.Detach();
        using SKPathMeasure pathMeasure = new(path);
        float verticalTextOffset = textBounds.VerticalCenter;
        float horizontalTextOffset = ((float)(LabelPositionFraction / HalfTurnDivisor) * (pathMeasure.Length - textBounds.Width)) + additionalSpace;
        Drawing.DrawTextOnPath(rp.Canvas, rp.Paint, path, Label, horizontalTextOffset, verticalTextOffset);

        void ReversePath()
        {
            StartAngle += SweepAngle;
            SweepAngle = -SweepAngle;
            LabelPositionFraction = 1 - LabelPositionFraction;
        }
    }
}
