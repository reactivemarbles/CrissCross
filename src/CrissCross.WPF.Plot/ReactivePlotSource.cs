// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Default implementation and factory methods for observable-first plot sources.
/// </summary>
/// <param name="Key">The stable series key.</param>
/// <param name="PlotType">The chart type rendered by the source.</param>
/// <param name="Updates">The normalized update stream.</param>
public sealed record ReactivePlotSource(PlotSeriesKey Key, PlotType PlotType, IObservable<ReactivePlotUpdate> Updates) : IReactivePlotSource
{
    /// <summary>
    /// Creates a source from an already-normalized update stream.
    /// </summary>
    /// <param name="key">The stable series key.</param>
    /// <param name="plotType">The chart type.</param>
    /// <param name="updates">The update stream.</param>
    /// <returns>A reactive plot source.</returns>
    public static IReactivePlotSource FromUpdates(PlotSeriesKey key, PlotType plotType, IObservable<ReactivePlotUpdate> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        return new ReactivePlotSource(key, plotType, updates);
    }

    /// <summary>
    /// Adapts legacy signal tick observables to append updates.
    /// </summary>
    /// <param name="updates">The legacy signal stream.</param>
    /// <returns>A reactive signal source.</returns>
    public static IReactivePlotSource FromSignalTicks(IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update => CreateUpdate(
            update.Name,
            update.Axis,
            PlotType.Signal,
            ReactivePlotUpdateKind.Append,
            update.DateTime,
            update.Value,
            PlotXAxisKind.Ticks,
            sequence++));

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Signal, projected);
    }

    /// <summary>
    /// Adapts legacy scatter point observables to replace updates.
    /// </summary>
    /// <param name="updates">The legacy scatter stream.</param>
    /// <returns>A reactive scatter source.</returns>
    public static IReactivePlotSource FromScatterPoints(IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update => CreateUpdate(
            update.Name,
            update.Axis,
            PlotType.Scatter,
            ReactivePlotUpdateKind.Replace,
            update.X,
            update.Y,
            PlotXAxisKind.Numeric,
            sequence++));

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Scatter, projected);
    }

    /// <summary>
    /// Adapts legacy data logger observables to append updates.
    /// </summary>
    /// <param name="updates">The legacy data logger stream.</param>
    /// <returns>A reactive data logger source.</returns>
    public static IReactivePlotSource FromDataLoggerPoints(IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update =>
        {
            var x = CreateOrdinalX(update.Value?.Count ?? 0);
            var maxPoints = update.nMaxPoints > 0 ? update.nMaxPoints : (int?)null;
            return CreateUpdate(update.Name, update.Axis, PlotType.DataLogger, ReactivePlotUpdateKind.Append, x, update.Value, PlotXAxisKind.Numeric, sequence++, maxPoints);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.DataLogger, projected);
    }

    /// <summary>
    /// Adapts legacy streamer observables to append updates.
    /// </summary>
    /// <param name="updates">The legacy streamer stream.</param>
    /// <returns>A reactive streamer source.</returns>
    public static IReactivePlotSource FromStreamerPoints(IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update => CreateUpdate(
            update.Name,
            update.Axis,
            PlotType.Streamer,
            ReactivePlotUpdateKind.Append,
            update.X,
            update.Y,
            PlotXAxisKind.Numeric,
            sequence++));

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Streamer, projected);
    }

    /// <summary>
    /// Adapts SignalXY observable snapshots to replace updates.
    /// </summary>
    /// <param name="updates">The SignalXY snapshot stream.</param>
    /// <returns>A reactive SignalXY source.</returns>
    public static IReactivePlotSource FromSignalXyPoints(IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update => CreateUpdate(
            update.Name,
            update.Axis,
            PlotType.SignalXY,
            ReactivePlotUpdateKind.Replace,
            update.X,
            update.Y,
            PlotXAxisKind.Numeric,
            sequence++));

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.SignalXY, projected);
    }

    /// <summary>
    /// Adapts a SignalXY snapshot to a single replace update.
    /// </summary>
    /// <param name="data">The SignalXY snapshot.</param>
    /// <returns>A reactive SignalXY source.</returns>
    public static IReactivePlotSource FromSignalXySnapshot((string? Name, IList<double>? Y, IList<double> X, int Axis) data) =>
        FromSignalXyPoints(Observable.Return(data));

    private static ReactivePlotUpdate CreateUpdate(
        string? name,
        int axis,
        PlotType plotType,
        ReactivePlotUpdateKind kind,
        IEnumerable<double>? x,
        IEnumerable<double>? y,
        PlotXAxisKind xAxisKind,
        long sequence,
        int? maxPoints = null)
    {
        var key = new PlotSeriesKey(string.IsNullOrWhiteSpace(name) ? plotType.ToString() : name!, axis);
        return new ReactivePlotUpdate(key, plotType, kind, x?.ToArray() ?? [], y?.ToArray() ?? [], xAxisKind, sequence, maxPoints);
    }

    private static IReadOnlyList<double> CreateOrdinalX(int count)
    {
        if (count <= 0)
        {
            return [];
        }

        var x = new double[count];
        for (var i = 0; i < x.Length; i++)
        {
            x[i] = i;
        }

        return x;
    }
}
