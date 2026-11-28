// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross.Plot.Plottables;

/// <summary>Renders summary statistics and sampled population values.</summary>
/// <param name="population">The population represented by the symbol.</param>
public class PopulationSymbol(Population population) : IPlottable
{
    /// <summary>Gets or sets the fractional width occupied by the individual markers.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained as a public field to preserve the existing binary API.")]
    public double MarkerWidthFraction = 0.8;

    /// <summary>Gets or sets the fractional width occupied by the bar.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained as a public field to preserve the existing binary API.")]
    public double BarWidthFraction = 0.9;

    /// <summary>The cached random x-coordinates for population markers.</summary>
    private readonly List<double> _randomXs = [];

    /// <summary>The label style used to hide labels on the summary bar.</summary>
    private readonly LabelStyle _emptyLabel = new() { IsVisible = false };

    /// <summary>The deterministic random-number generator used to place markers.</summary>
    private readonly RandomDataGenerator _rand = new(0);

    /// <summary>Gets the population represented by this symbol.</summary>
    public Population Population { get; } = population;

    /// <summary>Gets or sets whether this symbol is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this symbol.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this symbol.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the horizontal center of this symbol.</summary>
    public double X { get; set; }

    /// <summary>Gets or sets the total width of this symbol.</summary>
    public double Width { get; set; } = 0.8;

    /// <summary>Gets or sets the horizontal alignment of the population markers.</summary>
    public HorizontalAlignment MarkerAlignment { get; set; } = HorizontalAlignment.Left;

    /// <summary>Gets or sets the horizontal alignment of the summary bar.</summary>
    public HorizontalAlignment BarAlignment { get; set; } = HorizontalAlignment.Right;

    /// <summary>Gets or sets the horizontal alignment of the box using the bar alignment.</summary>
    public HorizontalAlignment BoxAlignment { get => BarAlignment; set => BarAlignment = value; }

    /// <summary>Gets or sets the summary bar.</summary>
    public Bar Bar { get; set; } = new() { IsVisible = true };

    /// <summary>Gets or sets the summary box.</summary>
    public Box Box { get; set; } = new() { IsVisible = false };

    /// <summary>Gets or sets the marker style for individual population values.</summary>
    public Marker Marker { get; set; } = new() { Size = Constants.DefaultMarkerSize, Shape = MarkerShape.OpenCircle };

    /// <summary>Gets or sets the function that configures the summary box values.</summary>
    public Func<Box, Population, Population> BoxValueConfig { get; set; } = BoxValueConfigurator_MedianQuantileExtrema;

    /// <summary>Configures a box using the mean, standard error, and standard deviation.</summary>
    /// <param name="box">The box to populate.</param>
    /// <param name="pop">The source population.</param>
    /// <returns>The input population.</returns>
    public static Population BoxValueConfigurator_MeanStdErrStDev(Box box, Population pop)
    {
        box.BoxMiddle = pop.Mean;
        box.BoxMin = pop.Mean - pop.StandardError;
        box.BoxMax = pop.Mean + pop.StandardError;
        box.WhiskerMin = pop.Mean - pop.StandardDeviation;
        box.WhiskerMax = pop.Mean + pop.StandardDeviation;
        return pop;
    }

    /// <summary>Configures a box using the median, quartiles, and extrema.</summary>
    /// <param name="box">The box to populate.</param>
    /// <param name="pop">The source population.</param>
    /// <returns>The input population.</returns>
    public static Population BoxValueConfigurator_MedianQuantileExtrema(Box box, Population pop)
    {
        box.BoxMiddle = pop.Median;
        box.BoxMin = pop.GetPercentile(Constants.LowerQuartilePercentile);
        box.BoxMax = pop.GetPercentile(Constants.UpperQuartilePercentile);
        box.WhiskerMin = pop.Min;
        box.WhiskerMax = pop.Max;
        return pop;
    }

    /// <summary>Gets the axis limits occupied by this symbol.</summary>
    /// <returns>The axis limits of the symbol.</returns>
    public AxisLimits GetAxisLimits() => new(GetRect());

    /// <summary>Renders this population symbol.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        RenderBar(rp);
        RenderBox(rp);
        RenderMarkers(rp);
    }

    /// <summary>Gets the full coordinate rectangle occupied by this symbol.</summary>
    /// <returns>The coordinate rectangle of the symbol.</returns>
    private CoordinateRect GetRect()
    {
        double left = X - (Width / Constants.HalfWidthDivisor);
        double right = X + (Width / Constants.HalfWidthDivisor);
        double minimum = Population.Min;
        double maximum = Population.Max;
        if (Bar.IsVisible)
        {
            minimum = Math.Min(Bar.ValueBase, minimum);
            maximum = Math.Max(Bar.ValueBase, maximum);
        }

        return new(left, right, minimum, maximum);
    }

    /// <summary>Gets the coordinate rectangle occupied by population markers.</summary>
    /// <returns>The marker coordinate rectangle.</returns>
    private CoordinateRect GetMarkerRect()
    {
        CoordinateRect rect = GetRect();
        double padding = (1 - MarkerWidthFraction) / Constants.HalfWidthDivisor;
        (double left, double right) = MarkerAlignment switch
        {
            HorizontalAlignment.Left => (rect.Left + padding, rect.HorizontalCenter - padding),
            HorizontalAlignment.Center => (rect.Left + padding, rect.Right - padding),
            HorizontalAlignment.Right => (rect.HorizontalCenter + padding, rect.Right - padding),
            _ => throw new ArgumentOutOfRangeException(nameof(MarkerAlignment)),
        };

        return new(left, right, rect.Bottom, rect.Top);
    }

    /// <summary>Gets the coordinate rectangle occupied by the bar and box.</summary>
    /// <returns>The summary coordinate rectangle.</returns>
    private CoordinateRect GetBarRect()
    {
        CoordinateRect rect = GetRect();
        double padding = (1 - BarWidthFraction) / Constants.HalfWidthDivisor;
        (double left, double right) = BarAlignment switch
        {
            HorizontalAlignment.Left => (rect.Left + padding, rect.HorizontalCenter - padding),
            HorizontalAlignment.Center => (rect.Left + padding, rect.Right - padding),
            HorizontalAlignment.Right => (rect.HorizontalCenter + padding, rect.Right - padding),
            _ => throw new ArgumentOutOfRangeException(nameof(BarAlignment)),
        };

        return new(left, right, rect.Bottom, rect.Top);
    }

    /// <summary>Renders the individual population markers.</summary>
    /// <param name="rp">The render context.</param>
    private void RenderMarkers(RenderPack rp)
    {
        if (!Marker.IsVisible)
        {
            return;
        }

        CoordinateRect rect = GetMarkerRect();
        if (_randomXs.Count < Population.Count)
        {
            int valuesNeeded = Population.Count - _randomXs.Count;
            _randomXs.AddRange(_rand.RandomSample(valuesNeeded, rect.Width, rect.Left));
        }

        for (int i = 0; i < Population.Count; i++)
        {
            Coordinates location = new(_randomXs[i], Population.Values[i]);
            Pixel pixel = Axes.GetPixel(location);
            Drawing.DrawMarker(rp.Canvas, rp.Paint, pixel, Marker.MarkerStyle);
        }
    }

    /// <summary>Renders the population summary bar.</summary>
    /// <param name="rp">The render context.</param>
    private void RenderBar(RenderPack rp)
    {
        if (!Bar.IsVisible)
        {
            return;
        }

        CoordinateRect rect = GetBarRect();
        Bar.Position = rect.HorizontalCenter;
        Bar.Size = rect.Width;
        Bar.Value = Population.Mean;
        Bar.Error = Population.StandardError;
        Bar.Render(rp, Axes, _emptyLabel);
    }

    /// <summary>Renders the population summary box.</summary>
    /// <param name="rp">The render context.</param>
    private void RenderBox(RenderPack rp)
    {
        if (!Box.IsVisible)
        {
            return;
        }

        CoordinateRect rect = GetBarRect();
        Box.Position = rect.HorizontalCenter;
        Box.Width = rect.Width;
        _ = BoxValueConfig.Invoke(Box, Population);
        Box.Render(rp, Axes);
    }

    /// <summary>Contains constants used by population symbols.</summary>
    internal static class Constants
    {
        /// <summary>The default marker size.</summary>
        internal const float DefaultMarkerSize = 10;

        /// <summary>The divisor used to compute half-widths and padding.</summary>
        internal const double HalfWidthDivisor = 2;

        /// <summary>The lower quartile percentile.</summary>
        internal const double LowerQuartilePercentile = 25;

        /// <summary>The upper quartile percentile.</summary>
        internal const double UpperQuartilePercentile = 75;
    }
}
