// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A line at a defined y-position that spans the horizontal data area.</summary>
public class HorizontalLine : AxisLine
{
    /// <summary>Gets or sets the y-coordinate of the line.</summary>
    public double Y
    {
        get => Position;
        set => Position = value;
    }

    /// <summary>Gets or sets the minimum x-coordinate occupied by the line.</summary>
    public double Minimum { get; set; } = double.NegativeInfinity;

    /// <summary>Gets or sets the maximum x-coordinate occupied by the line.</summary>
    public double Maximum { get; set; } = double.PositiveInfinity;

    /// <summary>Gets or sets the label style used by this line.</summary>
    public override LabelStyle LabelStyle { get; set; } = new()
    {
        Rotation = Constants.LabelRotation,
        Alignment = Alignment.LowerCenter,
        FontSize = Constants.LabelFontSize,
        Bold = true,
        ForeColor = Colors.White,
        Padding = Constants.LabelPadding
    };

    /// <summary>Determines whether the line is under the specified coordinate rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns><see langword="true"/> when the line is draggable and its y-coordinate is contained by the rectangle.</returns>
    public override bool IsUnderMouse(CoordinateRect rect) => IsDraggable && rect.ContainsY(Y);

    /// <summary>Gets the axis limits contributed by this line.</summary>
    /// <returns>The vertical limits of the line when autoscaling is enabled; otherwise, no limits.</returns>
    public override AxisLimits GetAxisLimits() => EnableAutoscale ? AxisLimits.VerticalOnly(Y, Y) : AxisLimits.NoLimits;

    /// <summary>Renders the horizontal line.</summary>
    /// <param name="rp">The render context.</param>
    public override void Render(RenderPack rp)
    {
        if (!IsVisible || !Axes.YAxis.Range.Contains(Y))
        {
            return;
        }

        double x1 = Math.Max(Minimum, Math.Min(Axes.XAxis.Min, Axes.XAxis.Max));
        double x2 = Math.Min(Maximum, Math.Max(Axes.XAxis.Min, Axes.XAxis.Max));
        Coordinates firstPoint = new(x1, Y);
        Coordinates secondPoint = new(x2, Y);
        CoordinateLine line = new(firstPoint, secondPoint);
        PixelLine pixelLine = Axes.GetPixelLine(line);
        LineStyle.Render(rp.Canvas, pixelLine, rp.Paint);
    }

    /// <summary>Renders the line label after other plot content.</summary>
    /// <param name="rp">The render context.</param>
    public override void RenderLast(RenderPack rp)
    {
        if (!LabelStyle.IsVisible || string.IsNullOrEmpty(LabelStyle.Text))
        {
            return;
        }

        float y = Axes.GetPixelY(Y);
        if (!rp.DataRect.ContainsY(y))
        {
            return;
        }

        float x = LabelOppositeAxis
            ? rp.DataRect.Right + LabelStyle.PixelPadding.Right + rp.Layout.PanelOffsets[Axes.YAxis]
            : rp.DataRect.Left - LabelStyle.PixelPadding.Left - rp.Layout.PanelOffsets[Axes.YAxis];

        Alignment defaultAlignment = LabelOppositeAxis
            ? Alignment.UpperCenter
            : Alignment.LowerCenter;
        LabelStyle.Alignment = ManualLabelAlignment ?? defaultAlignment;
        rp.CanvasState.DisableClipping();
        LabelStyle.Render(rp.Canvas, new(x, y), rp.Paint);
    }

    /// <summary>Contains default values for horizontal-line label rendering.</summary>
    private static class Constants
    {
        /// <summary>The default label rotation in degrees.</summary>
        public const float LabelRotation = -90;

        /// <summary>The default label font size.</summary>
        public const float LabelFontSize = 14;

        /// <summary>The default label padding in pixels.</summary>
        public const float LabelPadding = 5;
    }
}
