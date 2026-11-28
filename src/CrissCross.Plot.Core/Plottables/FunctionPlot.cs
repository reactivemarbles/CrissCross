// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Plots a function by sampling it across the visible x-axis range.</summary>
/// <param name="source">The function data source.</param>
public class FunctionPlot(IFunctionSource source) : IPlottable, IHasLine, IHasLegendText
{
    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this function.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this function in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the line style used to render this function.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the minimum x-coordinate in the plot range.</summary>
    public double MinX
    {
        get => double.IsInfinity(Source.RangeX.Min)
            ? Axes.XAxis.Min
            : Source.RangeX.Min;
        set => Source.RangeX = new(value, Source.RangeX.Max);
    }

    /// <summary>Gets or sets the maximum x-coordinate in the plot range.</summary>
    public double MaxX
    {
        get => double.IsInfinity(Source.RangeX.Max)
            ? Axes.XAxis.Max
            : Source.RangeX.Max;
        set => Source.RangeX = new(Source.RangeX.Min, value);
    }

    /// <summary>Gets the legend items for this function plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle);

    /// <summary>Gets the function source used to sample this plot.</summary>
    private IFunctionSource Source { get; } = source;

    /// <summary>Gets or sets the greatest y-coordinate range observed while rendering.</summary>
    private CoordinateRange MaxObservedRangeY { get; set; } = CoordinateRange.NotSet;

    /// <summary>Gets or sets the horizontal span used during the previous render.</summary>
    private CoordinateRange LastRenderHorizontalSpan { get; set; } = new(Constants.DefaultRangeMinimum, Constants.DefaultRangeMaximum);

    /// <summary>Gets the function value at the specified x-coordinate.</summary>
    /// <param name="x">The x-coordinate to evaluate.</param>
    /// <returns>The function value at <paramref name="x"/>.</returns>
    public double GetY(double x) => Source.Get(x);

    /// <summary>Gets the axis limits occupied by this function.</summary>
    /// <returns>The horizontal range and greatest y-range observed during rendering.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (MaxObservedRangeY == CoordinateRange.NotSet)
        {
            return new(Constants.DefaultRangeMinimum, Constants.DefaultRangeMaximum, Constants.DefaultRangeMinimum, Constants.DefaultRangeMaximum);
        }

        bool sourceRangeIsFinite = !(double.IsInfinity(Source.RangeX.Min) || double.IsInfinity(Source.RangeX.Max));
        CoordinateRange horizontalRange = sourceRangeIsFinite ? Source.RangeX : LastRenderHorizontalSpan;
        return new(horizontalRange, MaxObservedRangeY);
    }

    /// <summary>Renders sampled values across the visible x-axis range.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        var unitsPerPixel = Axes.XAxis.GetCoordinateDistance(1, rp.DataRect);
        double max = double.MinValue;
        double min = double.MaxValue;

        using SKPathBuilder builder = new();
        bool penIsDown = false;
        double minX = Math.Max(MinX, Axes.XAxis.Min);
        double maxX = Math.Min(MaxX, Axes.XAxis.Max);

        for (double x = minX; x <= maxX; x += unitsPerPixel)
        {
            double y = Source.Get(x);
            if (!IsFinite(y))
            {
                penIsDown = false;
                continue;
            }

            max = Math.Max(max, y);
            min = Math.Min(min, y);
            Pixel pixel = Axes.GetPixel(new(x, y));

            if (penIsDown)
            {
                builder.LineTo(pixel.ToSKPoint());
            }
            else
            {
                builder.MoveTo(pixel.ToSKPoint());
                penIsDown = true;
            }
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);

        if (min <= max)
        {
            if (min < MaxObservedRangeY.Min)
            {
                MaxObservedRangeY = MaxObservedRangeY.Expanded(min);
            }

            if (max > MaxObservedRangeY.Max)
            {
                MaxObservedRangeY = MaxObservedRangeY.Expanded(max);
            }
        }

        LastRenderHorizontalSpan = Axes.XAxis.Range.ToCoordinateRange;
    }

    /// <summary>Determines whether the specified value is finite.</summary>
    /// <param name="x">The value to inspect.</param>
    /// <returns><see langword="true"/> when finite; otherwise, <see langword="false"/>.</returns>
    private static bool IsFinite(double x) => !(double.IsInfinity(x) || double.IsNaN(x));

    /// <summary>Contains default values for an unobserved function range.</summary>
    private static class Constants
    {
        /// <summary>The minimum default coordinate.</summary>
        public const double DefaultRangeMinimum = -10;

        /// <summary>The maximum default coordinate.</summary>
        public const double DefaultRangeMaximum = 10;
    }
}
