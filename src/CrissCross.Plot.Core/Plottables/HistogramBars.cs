// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot.Statistics;

namespace CrissCross.Plot.Plottables;

/// <summary>Renders histogram counts as a series of bars.</summary>
public class HistogramBars : IPlottable
{
    /// <summary>The divisor used to calculate the center of a bin.</summary>
    private const double Half = 2.0;

    /// <summary>The source histogram used to update and size the bars.</summary>
    private readonly Histogram _histogram;

    /// <summary>Initializes a new instance of the <see cref="HistogramBars"/> class.</summary>
    /// <param name="histogram">The histogram to render.</param>
    public HistogramBars(Histogram histogram)
    {
        ArgumentChecks.ThrowIfNull(histogram, nameof(histogram));
        _histogram = histogram;
        Bars = new Bar[histogram.Bins.Length];
        for (int i = 0; i < histogram.Bins.Length; i++)
        {
            double left = histogram.Edges[i];
            double right = histogram.Edges[i + 1];
            double center = (left + right) / Half;
            Bars[i] = new() { Position = center };
        }

        BarWidthFraction = 1.0;
    }

    /// <summary>Gets or sets whether this plottable is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used by this plottable.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this plottable. Histogram bars do not produce legend items.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the bars rendered for histogram bins.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2305",
        Justification = "The public setter is retained for compatibility with callers that replace the histogram bar collection.")]
    public Bar[] Bars { get; set; }

    /// <summary>Gets or sets the label style applied to the bars.</summary>
    public LabelStyle LabelStyle { get; set; } = new() { IsVisible = false };

    /// <summary>Gets or sets the fraction of each bin width used as the bar width.</summary>
    public double BarWidthFraction
    {
        get => field;
        set
        {
            field = value;
            for (int i = 0; i < _histogram.Bins.Length; i++)
            {
                double left = _histogram.Edges[i];
                double right = _histogram.Edges[i + 1];
                Bars[i].Size = (right - left) * value;
            }
        }
    }

    /// <summary>Gets the axis limits required to display the bars.</summary>
    /// <returns>The horizontal bin range and vertical count range.</returns>
    public AxisLimits GetAxisLimits() => new(
            left: _histogram.Bins[0],
            right: _histogram.Bins[_histogram.Bins.Length - 1],
            bottom: 0,
            top: GetMaximumCount(_histogram.Counts));

    /// <summary>Renders the histogram bars.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        for (int i = 0; i < _histogram.Bins.Length; i++)
        {
            Bars[i].Value = _histogram.Counts[i];
            Bars[i].Render(rp, Axes, LabelStyle);
        }
    }

    /// <summary>Finds the maximum count in a histogram.</summary>
    /// <param name="counts">The bin counts.</param>
    /// <returns>The largest bin count.</returns>
    private static int GetMaximumCount(int[] counts)
    {
        if (counts.Length == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        int maximumCount = counts[0];
        for (int i = 1; i < counts.Length; i++)
        {
            maximumCount = Math.Max(maximumCount, counts[i]);
        }

        return maximumCount;
    }
}
