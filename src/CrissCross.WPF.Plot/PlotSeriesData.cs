// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Represents an immutable static or historic XY series.</summary>
public sealed record PlotSeriesData
{
    /// <summary>Initializes a new instance of the <see cref="PlotSeriesData"/> class.</summary>
    /// <param name="key">The stable series identity and axis assignment.</param>
    /// <param name="x">The X values.</param>
    /// <param name="y">The Y values.</param>
    public PlotSeriesData(
        PlotSeriesKey key,
        IReadOnlyList<double> x,
        IReadOnlyList<double> y)
        : this(key, x, y, PlotXAxisKind.Numeric)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PlotSeriesData"/> class.</summary>
    /// <param name="key">The stable series identity and axis assignment.</param>
    /// <param name="x">The X values.</param>
    /// <param name="y">The Y values.</param>
    /// <param name="axisKind">The interpretation of X values.</param>
    public PlotSeriesData(
        PlotSeriesKey key,
        IReadOnlyList<double> x,
        IReadOnlyList<double> y,
        PlotXAxisKind axisKind)
    {
        if (x is null)
        {
            throw new ArgumentNullException(nameof(x));
        }

        if (y is null)
        {
            throw new ArgumentNullException(nameof(y));
        }

        if (x.Count != y.Count)
        {
            throw new ArgumentException(
                "Plot series X and Y collections must contain the same number of values.",
                nameof(y));
        }

        Key = key;
        X = x;
        Y = y;
        XAxisKind = axisKind;
    }

    /// <summary>Gets the stable series identity and axis assignment.</summary>
    public PlotSeriesKey Key { get; }

    /// <summary>Gets the X values.</summary>
    public IReadOnlyList<double> X { get; }

    /// <summary>Gets the Y values.</summary>
    public IReadOnlyList<double> Y { get; }

    /// <summary>Gets the interpretation of the X values.</summary>
    public PlotXAxisKind XAxisKind { get; }

    /// <summary>Creates a numeric XY series.</summary>
    /// <param name="name">The series name.</param>
    /// <param name="axis">The Y-axis index.</param>
    /// <param name="x">The numeric X values.</param>
    /// <param name="y">The Y values.</param>
    /// <returns>The normalized series.</returns>
    public static PlotSeriesData Numeric(
        string name,
        int axis,
        IReadOnlyList<double> x,
        IReadOnlyList<double> y) => new(new PlotSeriesKey(name, axis), x, y);

    /// <summary>Creates a DateTime XY series using OLE Automation date values for ScottPlot compatibility.</summary>
    /// <param name="name">The series name.</param>
    /// <param name="axis">The Y-axis index.</param>
    /// <param name="timestamps">The timestamps.</param>
    /// <param name="y">The Y values.</param>
    /// <returns>The normalized series.</returns>
    public static PlotSeriesData DateTime(
        string name,
        int axis,
        IReadOnlyList<DateTime> timestamps,
        IReadOnlyList<double> y)
    {
        if (timestamps is null)
        {
            throw new ArgumentNullException(nameof(timestamps));
        }

        var x = new double[timestamps.Count];
        for (var i = 0; i < timestamps.Count; i++)
        {
            x[i] = timestamps[i].ToOADate();
        }

        return new(new PlotSeriesKey(name, axis), x, y, PlotXAxisKind.OADate);
    }

    /// <summary>Creates an update that replaces a rendered series with this snapshot.</summary>
    /// <param name="plotType">The target chart type.</param>
    /// <returns>A normalized replace update.</returns>
    public ReactivePlotUpdate ToUpdate(PlotType plotType) =>
        ToUpdate(plotType, 0, null);

    /// <summary>Creates a styled update that replaces a rendered series with this snapshot.</summary>
    /// <param name="plotType">The target chart type.</param>
    /// <param name="style">The series styling.</param>
    /// <returns>A normalized replace update.</returns>
    public ReactivePlotUpdate ToUpdate(
        PlotType plotType,
        ReactivePlotSeriesStyle? style) =>
        ToUpdate(plotType, 0, style);

    /// <summary>Creates an update with a sequence that replaces a rendered series.</summary>
    /// <param name="plotType">The target chart type.</param>
    /// <param name="sequence">The source sequence.</param>
    /// <returns>A normalized replace update.</returns>
    public ReactivePlotUpdate ToUpdate(PlotType plotType, long sequence) =>
        ToUpdate(plotType, sequence, null);

    /// <summary>Creates an update that replaces a rendered series with this snapshot.</summary>
    /// <param name="plotType">The target chart type.</param>
    /// <param name="sequence">The source sequence.</param>
    /// <param name="style">Optional series styling.</param>
    /// <returns>A normalized replace update.</returns>
    public ReactivePlotUpdate ToUpdate(
        PlotType plotType,
        long sequence,
        ReactivePlotSeriesStyle? style) =>
        new(Key, plotType, ReactivePlotUpdateKind.Replace, X, Y, XAxisKind, sequence, Style: style);

    /// <summary>Creates a copy of this series with a different key and Y values.</summary>
    /// <param name="key">The replacement key.</param>
    /// <param name="y">The replacement Y values.</param>
    /// <returns>The derived series.</returns>
    public PlotSeriesData Derive(PlotSeriesKey key, IReadOnlyList<double> y) =>
        new(key, X, y, XAxisKind);
}
