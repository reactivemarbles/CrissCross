// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>
/// This plottable renders date tick labels for financial charts where
/// data is displayed sequentially along the horizontal axis despite
/// DateTimes not being evenly spaced (e.g., data may include gaps).
/// </summary>
/// <param name="dateTimes">The date values represented by this financial time axis.</param>
public class FinancialTimeAxis(DateTime[] dateTimes) : IPlottable
{
    /// <summary>The format used to measure time tick labels.</summary>
    private const string LabelFormat = "   HH:mm:ss   ";

    /// <summary>The default label font size.</summary>
    private const float DefaultLabelFontSize = 14;

    /// <summary>The first tick index used by the current generator.</summary>
    private int _startIndex = -1;

    /// <summary>The number of data points skipped between generated ticks.</summary>
    private int _candlesToSkip;

    /// <summary>The width of one candle in pixels.</summary>
    private double _widthOfCandleInPixels;

    /// <summary>Gets the date values represented by the axis.</summary>
    public DateTime[] DateTimes { get; } = dateTimes;

    /// <summary>Gets or sets whether this plottable is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used by this plottable.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this plottable.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the style used to render date labels.</summary>
    public LabelStyle LabelStyle { get; set; } = new() { FontSize = DefaultLabelFontSize, Alignment = Alignment.UpperCenter, };

    /// <summary>Gets the axis limits occupied by this plottable.</summary>
    /// <returns>No axis limits, because this plottable only renders labels.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Renders date labels for the visible portion of the axis.</summary>
    /// <param name="rp">The render state used to draw the labels.</param>
    public virtual void Render(RenderPack rp)
    {
        if (DateTimes.Length == 0)
        {
            return;
        }

        // allow drawing outside the data area
        rp.CanvasState.DisableClipping();

        // get the best tick generator given the field of view
        int minIndexInView = (int)(Math.Max(0, Axes.XAxis.Range.Min));
        int maxIndexInView = (int)(Math.Min(DateTimes.Length - 1, Axes.XAxis.Range.Max));
        if (maxIndexInView <= minIndexInView)
        {
            return;
        }

        _widthOfCandleInPixels = rp.DataRect.Width / Axes.XAxis.Range.Span;
        TickGenerators.Financial.EveryNthUnit tickGenerator = GetBestTickGenerator(rp.Paint);
        List<(int Index, string Label)> ticks = tickGenerator.GetTicks(DateTimes, minIndexInView, maxIndexInView);
        if (ticks.Count == 0)
        {
            return;
        }

        _startIndex = ticks[0].Index;

        // render each tick label
        foreach ((int index, string label) in ticks)
        {
            Pixel px = new(Axes.XAxis.GetPixel(index, rp.DataRect), rp.DataRect.Bottom);
            LabelStyle.Render(rp.Canvas, px, rp.Paint, label);
        }
    }

    /// <summary>Creates the tick generator for the current label width and candle spacing.</summary>
    /// <param name="paint">The paint used to measure the tick label.</param>
    /// <returns>The generator configured for the current candle spacing.</returns>
    private TickGenerators.Financial.EveryNthUnit GetBestTickGenerator(Paint paint)
    {
        var maxWidth = LabelStyle.Measure(LabelFormat, paint).Size.Width;
        var newCandlesToSkip = (int)Math.Ceiling(maxWidth / _widthOfCandleInPixels);
        if (_candlesToSkip != newCandlesToSkip)
        {
            _startIndex = -1;
            _candlesToSkip = newCandlesToSkip;
        }

        return new(_candlesToSkip, _startIndex);
    }
}
