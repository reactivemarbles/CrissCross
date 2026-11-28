// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Places a triangular axis on the plot and converts between ternary and Cartesian coordinates.</summary>
/// <param name="clockwise">Whether the triangle's coordinates are oriented clockwise.</param>
public class TriangularAxis(bool clockwise) : IPlottable
{
    /// <summary>Gets whether the triangle's coordinates are oriented clockwise.</summary>
    public bool Clockwise { get; } = clockwise;

    /// <summary>Gets or sets whether this triangular axis is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position this triangular axis.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this triangular axis.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the padding fraction applied to the triangle.</summary>
    public double Padding { get; set; } = Constants.DefaultPadding;

    /// <summary>Gets or sets the background fill style.</summary>
    public FillStyle FillStyle { get; set; } = new() { IsVisible = true, Color = Colors.Yellow.WithAlpha(Constants.BackgroundAlpha) };

    /// <summary>Gets or sets the grid line style.</summary>
    public LineStyle GridLineStyle { get; set; } = new() { IsVisible = true, Width = Constants.DefaultLineWidth, Color = Colors.Gray.WithAlpha(Constants.GridLineAlpha) };

    /// <summary>Gets the left edge of the triangular axis.</summary>
    public TriangularAxisEdge Left { get; } = TriangularAxisEdge.Left(clockwise);

    /// <summary>Gets the right edge of the triangular axis.</summary>
    public TriangularAxisEdge Right { get; } = TriangularAxisEdge.Right(clockwise);

    /// <summary>Gets the bottom edge of the triangular axis.</summary>
    public TriangularAxisEdge Bottom { get; } = TriangularAxisEdge.Bottom(clockwise);

    /// <summary>Gets the axis limits occupied by this triangular axis.</summary>
    /// <returns>The axis limits of the triangular axis.</returns>
    public AxisLimits GetAxisLimits() => TriangularAxisEdge.AxisLimits.WithZoom(1 - Padding, 1 - Padding);

    /// <summary>Gets a point on the triangle from fractional positions along the bottom and left axes.</summary>
    /// <param name="bottomFraction">The fraction along the bottom edge, from zero to one.</param>
    /// <param name="leftFraction">The fraction along the left edge, from zero to one.</param>
    /// <returns>The corresponding Cartesian coordinates.</returns>
    public Coordinates GetCoordinates(double bottomFraction, double leftFraction)
    {
        double x = Bottom.GetCoordinates(bottomFraction).X;
        double y = Left.GetCoordinates(leftFraction).Y;
        return new(x, y);
    }

    /// <summary>Converts ternary coordinates to Cartesian coordinates.</summary>
    /// <param name="bottomFraction">The fraction assigned to the bottom vertex.</param>
    /// <param name="leftFraction">The fraction assigned to the left vertex.</param>
    /// <param name="rightFraction">The fraction assigned to the right vertex.</param>
    /// <returns>The corresponding Cartesian coordinates.</returns>
    public Coordinates GetCoordinates(double bottomFraction, double leftFraction, double rightFraction)
    {
        if (Math.Abs(bottomFraction + leftFraction + rightFraction - Constants.One) > Constants.FractionTolerance)
        {
            throw new ArgumentException("The sum of bottom, left, and right fractions must equal 1.");
        }

        double x;
        double y;

        if (!Clockwise)
        {
            // Counterclockwise transformation
            x = Constants.Half * ((Constants.Double * bottomFraction) + rightFraction);
            y = (Math.Sqrt(Constants.Three) / Constants.Double) * rightFraction;
        }
        else
        {
            // Clockwise transformation
            x = Constants.Half * ((Constants.Double * rightFraction) + leftFraction);
            y = (Math.Sqrt(Constants.Three) / Constants.Double) * leftFraction;
        }

        return new(x, y);
    }

    /// <summary>Renders the triangular background, edges, and grid lines.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        Paint paint = rp.Paint;

        RenderBackground(rp, paint);

        RenderEdge(rp, paint, Left);
        RenderEdge(rp, paint, Right);
        RenderEdge(rp, paint, Bottom);

        RenderGridLines(rp, paint, Bottom, Right);
        RenderGridLines(rp, paint, Bottom, Left);
        RenderGridLines(rp, paint, Left, Right);
    }

    /// <summary>Renders the filled triangle background.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void RenderBackground(RenderPack rp, Paint paint)
    {
        Coordinates[] corners = [Left.Start, Right.Start, Bottom.Start];
        Pixel[] cornerPixels = new Pixel[corners.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            cornerPixels[i] = Axes.GetPixel(corners[i]);
        }

        Drawing.FillPath(rp.Canvas, paint, cornerPixels, FillStyle, rp.DataRect);
    }

    /// <summary>Renders one edge, including its ticks and label.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="edge">The edge to render.</param>
    private void RenderEdge(RenderPack rp, Paint paint, TriangularAxisEdge edge)
    {
        // edge line
        Drawing.DrawLine(rp.Canvas, paint, Axes.GetPixelLine(edge.Line), edge.EdgeLineStyle);

        // ticks
        foreach (var tick in edge.Ticks)
        {
            Pixel px1 = Axes.GetPixel(tick.Point);
            Pixel px2 = px1.WithOffset(edge.TickOffset);
            PixelLine tickLine = new(px1, px2);
            edge.TickMarkStyle.Render(rp.Canvas, paint, tickLine);
            edge.TickLabelStyle.Render(rp.Canvas, px2, paint, tick.Label);
        }

        // label
        edge.LabelStyle.Render(rp.Canvas, Axes.GetPixel(edge.Line.Center), paint, edge.LabelText);
    }

    /// <summary>Renders grid lines between two triangular-axis edges.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="edge1">The edge providing the grid-line positions.</param>
    /// <param name="edge2">The edge intersected by the grid lines.</param>
    /// <param name="reverse">Whether the second edge's tick order is reversed.</param>
    private void RenderGridLines(RenderPack rp, Paint paint, TriangularAxisEdge edge1, TriangularAxisEdge edge2, bool reverse = true)
    {
        for (int i = 0; i < edge1.Ticks.Count; i++)
        {
            int matchingTickIndex = reverse ? edge1.Ticks.Count - 1 - i : i;
            CoordinateLine tickLine = new(edge1.Ticks[i].Point, edge2.Ticks[matchingTickIndex].Point);
            PixelLine pixelLine = Axes.GetPixelLine(tickLine);
            Drawing.DrawLine(rp.Canvas, paint, pixelLine, GridLineStyle);
        }
    }

    /// <summary>Contains constants used by triangular axes.</summary>
    internal static class Constants
    {
        /// <summary>The default padding fraction.</summary>
        internal const double DefaultPadding = 0.25;

        /// <summary>The default background opacity.</summary>
        internal const double BackgroundAlpha = 0.1;

        /// <summary>The default grid-line opacity.</summary>
        internal const double GridLineAlpha = 0.2;

        /// <summary>The default grid-line width.</summary>
        internal const float DefaultLineWidth = 1;

        /// <summary>The expected sum of ternary coordinates.</summary>
        internal const double One = 1;

        /// <summary>The tolerance for the ternary coordinate sum.</summary>
        internal const double FractionTolerance = 1e-6;

        /// <summary>The value one half.</summary>
        internal const double Half = 0.5;

        /// <summary>The value two.</summary>
        internal const double Double = 2;

        /// <summary>The value three.</summary>
        internal const double Three = 3;
    }
}
