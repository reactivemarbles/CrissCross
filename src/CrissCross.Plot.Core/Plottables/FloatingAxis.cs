// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders an axis line, ticks, and tick labels inside the data area.</summary>
public class FloatingAxis : IPlottable
{
    /// <summary>Gets or sets the axis position as a fraction of the data area.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public double Position = 0.5;

    /// <summary>Initializes a new instance of the <see cref="FloatingAxis"/> class for a horizontal axis.</summary>
    /// <param name="axis">The x-axis providing the tick generator.</param>
    public FloatingAxis(IXAxis axis)
    {
        IsVertical = false;
        TickGenerator = axis.TickGenerator;
        TickLabelStyle.Alignment = Alignment.UpperCenter;
    }

    /// <summary>Initializes a new instance of the <see cref="FloatingAxis"/> class for a vertical axis.</summary>
    /// <param name="axis">The y-axis providing the tick generator.</param>
    public FloatingAxis(IYAxis axis)
    {
        IsVertical = true;
        TickGenerator = axis.TickGenerator;
        TickLabelStyle.Alignment = Alignment.MiddleRight;
    }

    /// <summary>Gets or sets whether this axis is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render this floating axis.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this axis.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets whether this floating axis is vertical.</summary>
    public bool IsVertical { get; }

    /// <summary>Gets the tick generator used by this axis.</summary>
    public ITickGenerator TickGenerator { get; }

    /// <summary>Gets the line style used to render the axis spine.</summary>
    private LineStyle SpineLineStyle { get; } = new(1, Colors.Black);

    /// <summary>Gets the style used to render major tick marks.</summary>
    private TickMarkStyle MajorTickMarkStyle { get; } = new() { Color = Colors.Black };

    /// <summary>Gets the style used to render minor tick marks.</summary>
    private TickMarkStyle MinorTickMarkStyle { get; } = new() { Color = Colors.Black.WithAlpha(Constants.MinorTickAlpha) };

    /// <summary>Gets the style used to render tick labels.</summary>
    private LabelStyle TickLabelStyle { get; } = LabelStyle.Default;

    /// <summary>Gets the pixel length of each tick mark.</summary>
    private float TickLength { get; } = 5;

    /// <summary>Gets whether the label at the center tick is skipped.</summary>
    private bool SkipCenterTickLabel { get; } = true;

    /// <summary>Gets the axis limits occupied by this floating axis.</summary>
    /// <returns>No axis limits.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Renders this floating axis in its configured orientation.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (IsVertical)
        {
            RenderVerticalAxis(rp);
        }
        else
        {
            RenderHorizontalAxis(rp);
        }
    }

    /// <summary>Renders the vertical floating axis, its ticks, and its labels.</summary>
    /// <param name="rp">The render context.</param>
    public void RenderVerticalAxis(RenderPack rp)
    {
        float x = (float)((rp.DataRect.Width * Position) + rp.DataRect.Left);
        PixelLine spineLine = new(x, rp.DataRect.Top, x, rp.DataRect.Bottom);
        Drawing.DrawLine(rp.Canvas, rp.Paint, spineLine, SpineLineStyle);

        float x1 = x - (TickLength * Constants.Half);
        float x2 = x + (TickLength * Constants.Half);
        foreach (var tick in TickGenerator.Ticks)
        {
            float y = Axes.YAxis.GetPixel(tick.Position, rp.DataRect);
            PixelLine tickLine = new(x1, y, x2, y);
            if (tick.IsMajor)
            {
                MajorTickMarkStyle.Render(rp.Canvas, rp.Paint, tickLine);
                if (SkipCenterTickLabel && (Math.Abs(y - rp.DataRect.VerticalCenter) < Constants.CenterLabelTolerance))
                {
                    continue;
                }

                TickLabelStyle.Render(rp.Canvas, tickLine.Pixel1, rp.Paint, tick.Label);
            }
            else
            {
                MinorTickMarkStyle.Render(rp.Canvas, rp.Paint, tickLine);
            }
        }
    }

    /// <summary>Renders the horizontal floating axis, its ticks, and its labels.</summary>
    /// <param name="rp">The render context.</param>
    public void RenderHorizontalAxis(RenderPack rp)
    {
        float y = (float)((rp.DataRect.Height * Position) + rp.DataRect.Top);
        PixelLine spineLine = new(rp.DataRect.Left, y, rp.DataRect.Right, y);
        Drawing.DrawLine(rp.Canvas, rp.Paint, spineLine, SpineLineStyle);

        float y1 = y - (TickLength * Constants.Half);
        float y2 = y + (TickLength * Constants.Half);
        foreach (var tick in TickGenerator.Ticks)
        {
            float x = Axes.XAxis.GetPixel(tick.Position, rp.DataRect);
            PixelLine tickLine = new(x, y1, x, y2);
            if (tick.IsMajor)
            {
                MajorTickMarkStyle.Render(rp.Canvas, rp.Paint, tickLine);
                if (SkipCenterTickLabel && (Math.Abs(x - rp.DataRect.HorizontalCenter) < Constants.CenterLabelTolerance))
                {
                    continue;
                }

                TickLabelStyle.Render(rp.Canvas, tickLine.Pixel2, rp.Paint, tick.Label);
            }
            else
            {
                MinorTickMarkStyle.Render(rp.Canvas, rp.Paint, tickLine);
            }
        }
    }

    /// <summary>Contains numeric constants used when rendering floating axes.</summary>
    private static class Constants
    {
        /// <summary>The multiplier used to calculate half a tick length.</summary>
        public const float Half = 0.5F;

        /// <summary>The alpha applied to the minor tick color.</summary>
        public const float MinorTickAlpha = 0.2F;

        /// <summary>The pixel threshold around the center tick label.</summary>
        public const double CenterLabelTolerance = 1.5;
    }
}
