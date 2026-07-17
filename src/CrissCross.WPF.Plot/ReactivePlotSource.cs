// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Default implementation and factory methods for observable-first plot sources.</summary>
/// <param name="Key">The stable series key.</param>
/// <param name="PlotType">The chart type rendered by the source.</param>
/// <param name="Updates">The normalized update stream.</param>
public sealed partial record ReactivePlotSource(
    PlotSeriesKey Key,
    PlotType PlotType,
    IObservable<ReactivePlotUpdate> Updates) : IReactivePlotSource
{
    /// <summary>Gets the declared X-axis interpretation when it is known upfront.</summary>
    public PlotXAxisKind? XAxisKind { get; init; }

    /// <summary>Creates a source from an already-normalized update stream.</summary>
    /// <param name="key">The stable series key.</param>
    /// <param name="plotType">The chart type.</param>
    /// <param name="updates">The update stream.</param>
    /// <returns>A reactive plot source.</returns>
    public static IReactivePlotSource FromUpdates(
        PlotSeriesKey key,
        PlotType plotType,
        IObservable<ReactivePlotUpdate> updates) => FromUpdates(key, plotType, updates, null);

    /// <summary>Creates a source from an already-normalized update stream.</summary>
    /// <param name="key">The stable series key.</param>
    /// <param name="plotType">The chart type.</param>
    /// <param name="updates">The update stream.</param>
    /// <param name="axisKind">The X-axis interpretation used by the source when known upfront.</param>
    /// <returns>A reactive plot source.</returns>
    public static IReactivePlotSource FromUpdates(
        PlotSeriesKey key,
        PlotType plotType,
        IObservable<ReactivePlotUpdate> updates,
        PlotXAxisKind? axisKind)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        return new ReactivePlotSource(key, plotType, updates) { XAxisKind = axisKind };
    }

    /// <summary>Adapts legacy signal tick observables to append updates.</summary>
    /// <param name="updates">The legacy signal stream.</param>
    /// <returns>A reactive signal source.</returns>
    public static IReactivePlotSource FromSignalTicks(
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update =>
        {
            var currentSequence = sequence;
            sequence++;
            return CreateUpdate(
                new(update.Name, update.Axis, PlotType.Signal, ReactivePlotUpdateKind.Append),
                update.DateTime,
                update.Value,
                PlotXAxisKind.Ticks,
                currentSequence,
                null);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Signal, projected)
        {
            XAxisKind = PlotXAxisKind.Ticks,
        };
    }

    /// <summary>Adapts legacy signal point observables to append updates with numeric X-axis values.</summary>
    /// <param name="updates">The legacy signal point stream.</param>
    /// <returns>A reactive signal source.</returns>
    public static IReactivePlotSource FromSignalPoints(
        IObservable<(string? Name, IList<double>? Value, IList<double> X, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update =>
        {
            var currentSequence = sequence;
            sequence++;
            return CreateUpdate(
                new(update.Name, update.Axis, PlotType.Signal, ReactivePlotUpdateKind.Append),
                update.X,
                update.Value,
                PlotXAxisKind.Numeric,
                currentSequence,
                null);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Signal, projected)
        {
            XAxisKind = PlotXAxisKind.Numeric,
        };
    }

    /// <summary>Adapts legacy scatter point observables to replace updates.</summary>
    /// <param name="updates">The legacy scatter stream.</param>
    /// <returns>A reactive scatter source.</returns>
    public static IReactivePlotSource FromScatterPoints(
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update =>
        {
            var currentSequence = sequence;
            sequence++;
            return CreateUpdate(
                new(update.Name, update.Axis, PlotType.Scatter, ReactivePlotUpdateKind.Replace),
                update.X,
                update.Y,
                PlotXAxisKind.Numeric,
                currentSequence,
                null);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Scatter, projected)
        {
            XAxisKind = PlotXAxisKind.Numeric,
        };
    }

    /// <summary>Adapts legacy data logger observables to append updates.</summary>
    /// <param name="updates">The legacy data logger stream.</param>
    /// <returns>A reactive data logger source.</returns>
    public static IReactivePlotSource FromDataLoggerPoints(
        IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> updates)
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
            var currentSequence = sequence;
            sequence++;
            return CreateUpdate(
                new(update.Name, update.Axis, PlotType.DataLogger, ReactivePlotUpdateKind.Append),
                x,
                update.Value,
                PlotXAxisKind.Numeric,
                currentSequence,
                maxPoints);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.DataLogger, projected)
        {
            XAxisKind = PlotXAxisKind.Numeric,
        };
    }

    /// <summary>Adapts legacy streamer observables to append updates.</summary>
    /// <param name="updates">The legacy streamer stream.</param>
    /// <returns>A reactive streamer source.</returns>
    public static IReactivePlotSource FromStreamerPoints(
        IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update =>
        {
            var currentSequence = sequence;
            sequence++;
            return CreateUpdate(
                new(update.Name, update.Axis, PlotType.Streamer, ReactivePlotUpdateKind.Append),
                update.X,
                update.Y,
                PlotXAxisKind.Numeric,
                currentSequence,
                null);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.Streamer, projected)
        {
            XAxisKind = PlotXAxisKind.Numeric,
        };
    }

    /// <summary>Adapts SignalXY observable snapshots to replace updates.</summary>
    /// <param name="updates">The SignalXY snapshot stream.</param>
    /// <returns>A reactive SignalXY source.</returns>
    public static IReactivePlotSource FromSignalXyPoints(
        IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> updates)
    {
        if (updates is null)
        {
            throw new ArgumentNullException(nameof(updates));
        }

        var sequence = 0L;
        var projected = updates.Select(update =>
        {
            var currentSequence = sequence;
            sequence++;
            return CreateUpdate(
                new(update.Name, update.Axis, PlotType.SignalXY, ReactivePlotUpdateKind.Replace),
                update.X,
                update.Y,
                PlotXAxisKind.Numeric,
                currentSequence,
                null);
        });

        return new ReactivePlotSource(new PlotSeriesKey(string.Empty, 0), PlotType.SignalXY, projected)
        {
            XAxisKind = PlotXAxisKind.Numeric,
        };
    }

    /// <summary>Adapts a SignalXY snapshot to a single replace update.</summary>
    /// <param name="data">The SignalXY snapshot.</param>
    /// <returns>A reactive SignalXY source.</returns>
    public static IReactivePlotSource FromSignalXySnapshot(
        (string? Name, IList<double>? Y, IList<double> X, int Axis) data) =>
        FromSignalXyPoints(Observable.Return(data));

    /// <summary>Handles the CreateUpdate operation.</summary>
    /// <param name="identity">The stable update identity.</param>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <param name="axisKind">The X-axis kind value.</param>
    /// <param name="sequence">The sequence value.</param>
    /// <param name="maxPoints">The maxPoints value.</param>
    /// <returns>The result.</returns>
    private static ReactivePlotUpdate CreateUpdate(
        UpdateIdentity identity,
        IEnumerable<double>? x,
        IEnumerable<double>? y,
        PlotXAxisKind axisKind,
        long sequence,
        int? maxPoints)
    {
        var key = new PlotSeriesKey(
            string.IsNullOrWhiteSpace(identity.Name) ? identity.PlotType.ToString() : identity.Name!,
            identity.Axis);
        return new ReactivePlotUpdate(
            key,
            identity.PlotType,
            identity.Kind,
            x?.ToArray() ?? [],
            y?.ToArray() ?? [],
            axisKind,
            sequence,
            maxPoints);
    }

    /// <summary>Handles the CreateOrdinalX operation.</summary>
    /// <param name="count">The count value.</param>
    /// <returns>The result.</returns>
    private static double[] CreateOrdinalX(int count)
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

    /// <summary>Groups the stable identity of a legacy update.</summary>
    /// <param name="Name">The optional series name.</param>
    /// <param name="Axis">The Y-axis index.</param>
    /// <param name="PlotType">The target plot type.</param>
    /// <param name="Kind">The update kind.</param>
    private sealed record UpdateIdentity(string? Name, int Axis, PlotType PlotType, ReactivePlotUpdateKind Kind);
}
