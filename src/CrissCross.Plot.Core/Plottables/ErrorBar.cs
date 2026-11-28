// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Plots x/y values with optional positive and negative error ranges.</summary>
/// <param name="xs">The x-coordinate values.</param>
/// <param name="ys">The y-coordinate values.</param>
/// <param name="xErrorsPositive">The positive x errors, if supplied.</param>
/// <param name="xErrorsNegative">The negative x errors, if supplied.</param>
/// <param name="yErrorsPositive">The positive y errors, if supplied.</param>
/// <param name="yErrorsNegative">The negative y errors, if supplied.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter names are retained for existing named-argument callers.")]
public class ErrorBar(
    IReadOnlyList<double> xs,
    IReadOnlyList<double> ys,
    IReadOnlyList<double>? xErrorsPositive,
    IReadOnlyList<double>? xErrorsNegative,
    IReadOnlyList<double>? yErrorsPositive,
    IReadOnlyList<double>? yErrorsNegative) : IPlottable, IHasLine, IHasLegendText
{
    /// <summary>The tolerance for treating negligible error ranges as zero-width.</summary>
    private const double ErrorValueTolerance = 1e-12;

    /// <summary>Gets or sets whether this error bar is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this error bar.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the x-coordinate values.</summary>
    public IReadOnlyList<double> Xs { get; set; } = xs;

    /// <summary>Gets or sets the y-coordinate values.</summary>
    public IReadOnlyList<double> Ys { get; set; } = ys;

    /// <summary>Gets or sets the positive x-error values.</summary>
    public IReadOnlyList<double>? XErrorPositive { get; set; } = xErrorsPositive;

    /// <summary>Gets or sets the negative x-error values.</summary>
    public IReadOnlyList<double>? XErrorNegative { get; set; } = xErrorsNegative;

    /// <summary>Gets or sets the positive y-error values.</summary>
    public IReadOnlyList<double>? YErrorPositive { get; set; } = yErrorsPositive;

    /// <summary>Gets or sets the negative y-error values.</summary>
    public IReadOnlyList<double>? YErrorNegative { get; set; } = yErrorsNegative;

    /// <summary>Gets or sets the line style used to render error bars.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the half-width of the error-bar caps in pixels.</summary>
    public float CapSize { get; set; } = 3;

    /// <summary>Gets or sets the line color.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set => LineStyle.Color = value;
    }

    /// <summary>Gets the legend items for this error bar.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle);

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this error bar in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets the axis limits occupied by the error bars.</summary>
    /// <returns>The combined x and y limits of the error bars.</returns>
    public AxisLimits GetAxisLimits()
    {
        ExpandingAxisLimits limits = new();

        for (int i = 0; i < Xs.Count; i++)
        {
            double minimumX = XErrorNegative is null ? Xs[i] : Xs[i] - XErrorNegative[i];
            double maximumX = XErrorPositive is null ? Xs[i] : Xs[i] + XErrorPositive[i];
            double minimumY = YErrorNegative is null ? Ys[i] : Ys[i] - YErrorNegative[i];
            double maximumY = YErrorPositive is null ? Ys[i] : Ys[i] + YErrorPositive[i];

            limits.ExpandX(minimumX);
            limits.ExpandX(maximumX);
            limits.ExpandY(minimumY);
            limits.ExpandY(maximumY);
        }

        return limits.AxisLimits;
    }

    /// <summary>Renders the horizontal and vertical error bars.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        RenderErrorBars(rp.Canvas, rp.Paint, Xs, Ys, YErrorPositive, YErrorNegative);
        RenderErrorBars(rp.Canvas, rp.Paint, Ys, Xs, XErrorPositive, XErrorNegative, true);
    }

    /// <summary>Renders the error bars for one axis.</summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="paint">The paint to use.</param>
    /// <param name="positions">The fixed-axis positions.</param>
    /// <param name="values">The measured values.</param>
    /// <param name="positiveErrors">The positive error values, if supplied.</param>
    /// <param name="negativeErrors">The negative error values, if supplied.</param>
    /// <param name="horizontal">Whether the error bars are horizontal.</param>
    private void RenderErrorBars(
        SKCanvas canvas,
        Paint paint,
        IReadOnlyList<double> positions,
        IReadOnlyList<double> values,
        IReadOnlyList<double>? positiveErrors,
        IReadOnlyList<double>? negativeErrors,
        bool horizontal = false)
    {
        using SKPathBuilder builder = new();
        LineStyle.ApplyToPaint(paint);

        for (int i = 0; i < values.Count; i++)
        {
            RenderErrorBar(builder, positions, values, positiveErrors, negativeErrors, i, horizontal);
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, LineStyle);
    }

    /// <summary>Adds one error bar and its caps to the path.</summary>
    /// <param name="path">The path receiving the error bar.</param>
    /// <param name="positions">The fixed-axis positions.</param>
    /// <param name="values">The measured values.</param>
    /// <param name="positiveErrors">The positive error values, if supplied.</param>
    /// <param name="negativeErrors">The negative error values, if supplied.</param>
    /// <param name="index">The error-bar index.</param>
    /// <param name="horizontal">Whether the error bar is horizontal.</param>
    private void RenderErrorBar(
        SKPathBuilder path,
        IReadOnlyList<double> positions,
        IReadOnlyList<double> values,
        IReadOnlyList<double>? positiveErrors,
        IReadOnlyList<double>? negativeErrors,
        int index,
        bool horizontal)
    {
        double bottom = values[index] - (negativeErrors?[index] ?? 0);
        double top = values[index] + (positiveErrors?[index] ?? 0);
        if (Math.Abs(bottom - top) <= ErrorValueTolerance && Math.Abs(bottom - values[index]) <= ErrorValueTolerance)
        {
            return;
        }

        Coordinates centreBottom = horizontal ? new(bottom, positions[index]) : new(positions[index], bottom);
        Coordinates centreTop = horizontal ? new(top, positions[index]) : new(positions[index], top);
        Pixel capOffset = horizontal ? new(0, CapSize) : new(CapSize, 0);
        Pixel bottomPixel = Axes.GetPixel(centreBottom);
        Pixel topPixel = Axes.GetPixel(centreTop);

        path.MoveTo((bottomPixel - capOffset).ToSKPoint());
        path.LineTo((bottomPixel + capOffset).ToSKPoint());
        path.MoveTo(bottomPixel.ToSKPoint());
        path.LineTo(topPixel.ToSKPoint());
        path.MoveTo((topPixel - capOffset).ToSKPoint());
        path.LineTo((topPixel + capOffset).ToSKPoint());
    }
}
