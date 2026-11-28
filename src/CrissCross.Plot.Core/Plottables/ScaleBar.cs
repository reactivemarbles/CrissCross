// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a scale bar with optional horizontal and vertical labels.</summary>
public class ScaleBar : IPlottable, IHasLine
{
    /// <summary>Gets or sets whether this scale bar is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and scale this bar.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this scale bar.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the distance from the data area edges to the scale bar corner.</summary>
    public PixelPadding EdgePadding { get; set; } = new(Constants.DefaultEdgePadding);

    /// <summary>Gets or sets the spacing between labels and scale bar lines.</summary>
    public PixelPadding LabelPadding { get; set; } = new(Constants.DefaultLabelPadding);

    /// <summary>Gets or sets the horizontal scale bar length in axis units.</summary>
    public double Width { get; set; } = Constants.DefaultLength;

    /// <summary>Gets or sets the vertical scale bar length in axis units.</summary>
    public double Height { get; set; } = Constants.DefaultLength;

    /// <summary>Gets or sets the scale bar line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { IsVisible = true, Width = Constants.DefaultLineWidth, Color = Colors.Black, };

    /// <summary>Gets or sets the scale bar line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the scale bar line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the scale bar line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the horizontal label style.</summary>
    public LabelStyle XLabelStyle { get; set; } = new() { Alignment = Alignment.UpperCenter };

    /// <summary>Gets or sets the vertical label style.</summary>
    public LabelStyle YLabelStyle { get; set; } = new() { Alignment = Alignment.MiddleLeft };

    /// <summary>Gets or sets the horizontal scale bar label.</summary>
    public string XLabel { get; set; } = string.Empty;

    /// <summary>Gets or sets the vertical scale bar label.</summary>
    public string YLabel { get; set; } = string.Empty;

    /// <summary>Gets the axis limits occupied by this scale bar.</summary>
    /// <returns>No axis limits, because this scale bar is positioned in pixel space.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Renders this scale bar and its labels.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        Pixel corner = Axes.GetPixel(new(Axes.XAxis.Max, Axes.YAxis.Min));
        corner = corner.WithOffset(-EdgePadding.Left, -EdgePadding.Bottom);

        PixelSize horizontalLabelSize = XLabelStyle.Measure(XLabel, rp.Paint).Size;
        PixelSize verticalLabelSize = YLabelStyle.Measure(YLabel, rp.Paint).Size;
        corner = corner.WithOffset(-verticalLabelSize.Width, -horizontalLabelSize.Height);

        using SKPathBuilder builder = new();
        builder.MoveTo(corner.ToSKPoint());

        if (Width > 0)
        {
            double pixelsPerUnitHorizontal = rp.DataRect.Width / Axes.XAxis.Width;
            double horizontalPixelWidth = pixelsPerUnitHorizontal * Width;
            builder.RLineTo(new((float)-horizontalPixelWidth, 0));
            builder.RLineTo(new((float)+horizontalPixelWidth, 0));

            Pixel horizontalLabelCenter = new(corner.X - (horizontalPixelWidth / Constants.HalfLengthDivisor), corner.Y + LabelPadding.Top);
            XLabelStyle.Render(rp.Canvas, horizontalLabelCenter, rp.Paint, XLabel);
        }

        if (Height > 0)
        {
            double pixelsPerUnitVertical = rp.DataRect.Height / Axes.YAxis.Height;
            double verticalPixelHeight = pixelsPerUnitVertical * Height;
            builder.RLineTo(new(0, (float)-verticalPixelHeight));
            builder.RLineTo(new(0, (float)+verticalPixelHeight));

            Pixel verticalLabelCenter = new(corner.X + LabelPadding.Right, corner.Y - (verticalPixelHeight / Constants.HalfLengthDivisor));
            YLabelStyle.Render(rp.Canvas, verticalLabelCenter, rp.Paint, YLabel);
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);
    }

    /// <summary>Contains constants used by scale bars.</summary>
    internal static class Constants
    {
        /// <summary>The default distance from data area edges in pixels.</summary>
        internal const int DefaultEdgePadding = 10;

        /// <summary>The default label padding in pixels.</summary>
        internal const int DefaultLabelPadding = 3;

        /// <summary>The default scale bar length in axis units.</summary>
        internal const double DefaultLength = 1;

        /// <summary>The default scale bar line width in pixels.</summary>
        internal const float DefaultLineWidth = 1;

        /// <summary>The divisor used to calculate half of a line length.</summary>
        internal const double HalfLengthDivisor = 2;
    }
}
