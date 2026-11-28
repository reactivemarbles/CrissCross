// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A line at a defined X position that spans the entire vertical space of the data area.</summary>
public class VerticalLine : AxisLine
{
    /// <summary>Gets or sets the label style used for this vertical line.</summary>
    public override LabelStyle LabelStyle { get; set; } = new() { ForeColor = Colors.White, FontSize = Constants.DefaultLabelFontSize, Bold = true, Padding = Constants.DefaultLabelPadding };

    /// <summary>Gets or sets the horizontal coordinate of this line.</summary>
    public double X
    {
        get => Position;
        set => Position = value;
    }

    /// <summary>Gets or sets the minimum vertical extent of the line.</summary>
    public double Minimum { get; set; } = double.NegativeInfinity;

    /// <summary>Gets or sets the maximum vertical extent of the line.</summary>
    public double Maximum { get; set; } = double.PositiveInfinity;

    /// <summary>Determines whether the line is under the specified coordinate rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns><see langword="true"/> when the line is draggable and intersects the rectangle.</returns>
    public override bool IsUnderMouse(CoordinateRect rect) => IsDraggable && rect.ContainsX(X);

    /// <summary>Gets the axis limits contributed by this line.</summary>
    /// <returns>The horizontal limits when autoscaling is enabled; otherwise no limits.</returns>
    public override AxisLimits GetAxisLimits() => EnableAutoscale ? AxisLimits.HorizontalOnly(X, X) : AxisLimits.NoLimits;

    /// <summary>Renders this vertical line.</summary>
    /// <param name="rp">The render context.</param>
    public override void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        if (!Axes.XAxis.Range.Contains(X))
        {
            return;
        }

        double y1 = Math.Max(Minimum, Math.Min(Axes.YAxis.Min, Axes.YAxis.Max));
        double y2 = Math.Min(Maximum, Math.Max(Axes.YAxis.Min, Axes.YAxis.Max));
        Coordinates pt1 = new(X, y1);
        Coordinates pt2 = new(X, y2);

        CoordinateLine line = new(pt1, pt2);
        PixelLine pixelLine = Axes.GetPixelLine(line);
        LineStyle.Render(rp.Canvas, pixelLine, rp.Paint);
    }

    /// <summary>Renders the vertical line's label after other plot content.</summary>
    /// <param name="rp">The render context.</param>
    public override void RenderLast(RenderPack rp)
    {
        if (!LabelStyle.IsVisible || string.IsNullOrEmpty(LabelStyle.Text))
        {
            return;
        }

        // determine location
        float x = Axes.GetPixelX(X);

        // do not render if the axis line is outside the data area
        if (!rp.DataRect.ContainsX(x))
        {
            return;
        }

        float y = LabelOppositeAxis
            ? rp.DataRect.Top - LabelStyle.PixelPadding.Top
            : rp.DataRect.Bottom + LabelStyle.PixelPadding.Bottom;

        Alignment defaultAlignment = LabelOppositeAxis
            ? Alignment.LowerCenter
            : Alignment.UpperCenter;

        LabelStyle.Alignment = ManualLabelAlignment ?? defaultAlignment;

        // draw label outside the data area
        rp.CanvasState.DisableClipping();

        LabelStyle.Render(rp.Canvas, new(x, y), rp.Paint);
    }

    /// <summary>Contains constants used by vertical lines.</summary>
    internal static class Constants
    {
        /// <summary>The default line-label font size in pixels.</summary>
        internal const float DefaultLabelFontSize = 14;

        /// <summary>The default line-label padding in pixels.</summary>
        internal const float DefaultLabelPadding = 5;
    }
}
