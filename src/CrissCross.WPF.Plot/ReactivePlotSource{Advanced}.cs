// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Advanced static, historic, and live source factories.</summary>
public sealed partial record ReactivePlotSource
{
    /// <summary>Creates a finite line source from a static series snapshot.</summary>
    /// <param name="series">The static series.</param>
    /// <returns>The finite plot source.</returns>
    public static IReactivePlotSource FromStatic(PlotSeriesData series) =>
        FromStatic(series, PlotType.Line, null);

    /// <summary>Creates a finite source from a static series snapshot.</summary>
    /// <param name="series">The static series.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <returns>The finite plot source.</returns>
    public static IReactivePlotSource FromStatic(PlotSeriesData series, PlotType plotType) =>
        FromStatic(series, plotType, null);

    /// <summary>Creates a finite source from a static series snapshot.</summary>
    /// <param name="series">The static series.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <param name="style">Optional series styling.</param>
    /// <returns>The finite plot source.</returns>
    public static IReactivePlotSource FromStatic(
        PlotSeriesData series,
        PlotType plotType,
        ReactivePlotSeriesStyle? style)
    {
        if (series is null)
        {
            throw new ArgumentNullException(nameof(series));
        }

        return new ReactivePlotSource(
            series.Key,
            plotType,
            Observable.Return(series.ToUpdate(plotType, style: style)))
        {
            XAxisKind = series.XAxisKind,
        };
    }

    /// <summary>Creates a reduced historic line source.</summary>
    /// <param name="series">The full historic series.</param>
    /// <param name="targetPointCount">The maximum rendered point count.</param>
    /// <returns>The reduced finite plot source.</returns>
    public static IReactivePlotSource FromHistoric(
        PlotSeriesData series,
        int targetPointCount) =>
        FromHistoric(series, targetPointCount, PlotType.Line, null);

    /// <summary>Creates a reduced historic source.</summary>
    /// <param name="series">The full historic series.</param>
    /// <param name="targetPointCount">The maximum rendered point count.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <returns>The reduced finite plot source.</returns>
    public static IReactivePlotSource FromHistoric(
        PlotSeriesData series,
        int targetPointCount,
        PlotType plotType) =>
        FromHistoric(series, targetPointCount, plotType, null);

    /// <summary>Creates a finite source from historic data reduced to a rendering budget with LTTB.</summary>
    /// <param name="series">The full historic series.</param>
    /// <param name="targetPointCount">The maximum rendered point count.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <param name="style">Optional series styling.</param>
    /// <returns>The reduced finite plot source.</returns>
    public static IReactivePlotSource FromHistoric(
        PlotSeriesData series,
        int targetPointCount,
        PlotType plotType,
        ReactivePlotSeriesStyle? style) =>
        FromStatic(
            PlotDataReducer.LargestTriangleThreeBuckets(series, targetPointCount),
            plotType,
            style);

    /// <summary>Creates a numeric signal source from live points.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The live point stream.</param>
    /// <returns>The live plot source.</returns>
    public static IReactivePlotSource FromLive(
        PlotSeriesKey key,
        IObservable<PlotDataPoint> points) =>
        FromLive(
            key,
            points,
            PlotType.Signal,
            PlotXAxisKind.Numeric,
            null,
            null);

    /// <summary>Creates a live source with explicit chart and axis types.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The live point stream.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <param name="axisKind">The X-axis interpretation.</param>
    /// <returns>The live plot source.</returns>
    public static IReactivePlotSource FromLive(
        PlotSeriesKey key,
        IObservable<PlotDataPoint> points,
        PlotType plotType,
        PlotXAxisKind axisKind) =>
        FromLive(key, points, plotType, axisKind, null, null);

    /// <summary>Creates an append source from a stream of normalized numeric points.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The live point stream.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <param name="axisKind">The X-axis interpretation.</param>
    /// <param name="maxPoints">An optional rolling retention limit.</param>
    /// <param name="style">Optional series styling.</param>
    /// <returns>The live plot source.</returns>
    public static IReactivePlotSource FromLive(
        PlotSeriesKey key,
        IObservable<PlotDataPoint> points,
        PlotType plotType,
        PlotXAxisKind axisKind,
        int? maxPoints,
        ReactivePlotSeriesStyle? style)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        var updates = Observable.Defer(() =>
        {
            var sequence = 0L;
            return points.Select(point =>
            {
                var currentSequence = sequence;
                sequence++;
                return new ReactivePlotUpdate(
                    key,
                    plotType,
                    ReactivePlotUpdateKind.Append,
                    [point.X],
                    [point.Y],
                    axisKind,
                    currentSequence,
                    maxPoints,
                    style);
            });
        });

        return new ReactivePlotSource(key, plotType, updates) { XAxisKind = axisKind };
    }

    /// <summary>Creates a timestamped signal source.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The timestamped value stream.</param>
    /// <returns>The live date-time plot source.</returns>
    public static IReactivePlotSource FromDateTimeLive(
        PlotSeriesKey key,
        IObservable<(DateTime Timestamp, double Value)> points) =>
        FromDateTimeLive(key, points, PlotType.Signal, null, null);

    /// <summary>Creates a timestamped signal source with retention and styling.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The timestamped value stream.</param>
    /// <param name="maxPoints">The rolling retention limit.</param>
    /// <param name="style">The series styling.</param>
    /// <returns>The live date-time plot source.</returns>
    public static IReactivePlotSource FromDateTimeLive(
        PlotSeriesKey key,
        IObservable<(DateTime Timestamp, double Value)> points,
        int? maxPoints,
        ReactivePlotSeriesStyle? style) =>
        FromDateTimeLive(key, points, PlotType.Signal, maxPoints, style);

    /// <summary>Creates a timestamped source with an explicit chart type.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The timestamped value stream.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <returns>The live date-time plot source.</returns>
    public static IReactivePlotSource FromDateTimeLive(
        PlotSeriesKey key,
        IObservable<(DateTime Timestamp, double Value)> points,
        PlotType plotType) =>
        FromDateTimeLive(key, points, plotType, null, null);

    /// <summary>Creates an append source from timestamped live values.</summary>
    /// <param name="key">The stable output key.</param>
    /// <param name="points">The timestamped value stream.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <param name="maxPoints">An optional rolling retention limit.</param>
    /// <param name="style">Optional series styling.</param>
    /// <returns>The live date-time plot source.</returns>
    public static IReactivePlotSource FromDateTimeLive(
        PlotSeriesKey key,
        IObservable<(DateTime Timestamp, double Value)> points,
        PlotType plotType,
        int? maxPoints,
        ReactivePlotSeriesStyle? style)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        return FromLive(
            key,
            points.Select(point => new PlotDataPoint(point.Timestamp.ToOADate(), point.Value)),
            plotType,
            PlotXAxisKind.OADate,
            maxPoints,
            style);
    }
}
