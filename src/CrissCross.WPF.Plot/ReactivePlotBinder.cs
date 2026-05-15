// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Default reactive plot binder that owns source subscriptions, lifecycle state, validation, batching, and UI marshalling.
/// </summary>
public sealed class ReactivePlotBinder : IReactivePlotBinder
{
    private readonly IReactivePlotAdapterFactory? _adapterFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePlotBinder"/> class.
    /// </summary>
    public ReactivePlotBinder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePlotBinder"/> class.
    /// </summary>
    /// <param name="adapterFactory">The adapter factory used by pure binder tests and non-WPF hosts.</param>
    public ReactivePlotBinder(IReactivePlotAdapterFactory adapterFactory)
    {
        if (adapterFactory is null)
        {
            throw new ArgumentNullException(nameof(adapterFactory));
        }

        _adapterFactory = adapterFactory;
    }

    /// <inheritdoc />
    public IReactivePlotConnection Bind(LiveChartViewModel chart, IEnumerable<IReactivePlotSource> sources, ReactivePlotBindingOptions? options = null)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        return BindCore(sources, new WpfReactivePlotAdapterFactory(chart), options);
    }

    /// <summary>
    /// Binds the supplied sources using the adapter factory supplied to the constructor.
    /// </summary>
    /// <param name="sources">The sources to bind.</param>
    /// <param name="options">Optional binding options.</param>
    /// <returns>An owned connection.</returns>
    public IReactivePlotConnection Bind(IEnumerable<IReactivePlotSource> sources, ReactivePlotBindingOptions? options = null)
    {
        if (_adapterFactory is null)
        {
            throw new InvalidOperationException("This binder was not created with an adapter factory.");
        }

        return BindCore(sources, _adapterFactory, options);
    }

    private static IReactivePlotConnection BindCore(IEnumerable<IReactivePlotSource> sources, IReactivePlotAdapterFactory adapterFactory, ReactivePlotBindingOptions? options)
    {
        if (sources is null)
        {
            throw new ArgumentNullException(nameof(sources));
        }

        if (adapterFactory is null)
        {
            throw new ArgumentNullException(nameof(adapterFactory));
        }

        var bindingOptions = options ?? new ReactivePlotBindingOptions();
        var connection = new ReactivePlotConnection();
        var adapters = new Dictionary<PlotSeriesKey, IReactivePlotAdapter>();
        var retained = new Dictionary<PlotSeriesKey, RetainedSeries>();
        var stoppedSeries = new HashSet<PlotSeriesKey>();
        var subscriptions = new CompositeDisposable();

        connection.Attach(subscriptions, adapters.Values);
        connection.SetState(ReactivePlotConnectionState.Connecting);

        var sourceArray = sources.ToArray();
        if (sourceArray.Length == 0)
        {
            connection.MarkCompleted();
            return connection;
        }

        var completed = 0;
        foreach (var source in sourceArray)
        {
            var updateStream = source.Updates;
            if (bindingOptions.SourceScheduler is not null)
            {
                updateStream = updateStream.SubscribeOn(bindingOptions.SourceScheduler);
            }

            updateStream = ApplyBatching(updateStream, bindingOptions);
            updateStream = updateStream.ObserveOn(bindingOptions.UiScheduler ?? RxSchedulers.MainThreadScheduler);

            var subscription = updateStream.Subscribe(
                update => ApplyUpdate(update, bindingOptions, adapterFactory, adapters, retained, stoppedSeries, connection),
                error => HandleSourceError(source, error, bindingOptions, stoppedSeries, connection),
                () =>
                {
                    completed++;
                    if (completed == sourceArray.Length && connection.CurrentState != ReactivePlotConnectionState.Faulted)
                    {
                        connection.MarkCompleted();
                    }
                });
            subscriptions.Add(subscription);
        }

        return connection;
    }

    private static IObservable<ReactivePlotUpdate> ApplyBatching(IObservable<ReactivePlotUpdate> updates, ReactivePlotBindingOptions options)
    {
        if (options.BatchWindow is { } batchWindow && batchWindow > TimeSpan.Zero)
        {
            return updates.Buffer(batchWindow, Math.Max(1, options.MaxBatchSize)).Where(batch => batch.Count > 0).SelectMany(AggregateBatch);
        }

        if (options.MaxBatchSize > 1)
        {
            return updates.Buffer(options.MaxBatchSize).Where(batch => batch.Count > 0).SelectMany(AggregateBatch);
        }

        return updates;
    }

    private static IEnumerable<ReactivePlotUpdate> AggregateBatch(IList<ReactivePlotUpdate> batch)
    {
        if (batch.Count == 1)
        {
            yield return batch[0];
            yield break;
        }

        ReactivePlotUpdate? pendingAppend = null;
        foreach (var update in batch)
        {
            if (pendingAppend is not null && CanAggregateAppend(pendingAppend, update))
            {
                pendingAppend = pendingAppend with
                {
                    X = pendingAppend.X.Concat(update.X).ToArray(),
                    Y = pendingAppend.Y.Concat(update.Y).ToArray(),
                    Sequence = update.Sequence,
                    MaxPoints = update.MaxPoints ?? pendingAppend.MaxPoints,
                };
                continue;
            }

            if (pendingAppend is not null)
            {
                yield return pendingAppend;
                pendingAppend = null;
            }

            if (update.Kind == ReactivePlotUpdateKind.Append)
            {
                pendingAppend = update;
            }
            else
            {
                yield return update;
            }
        }

        if (pendingAppend is not null)
        {
            yield return pendingAppend;
        }
    }

    private static bool CanAggregateAppend(ReactivePlotUpdate pending, ReactivePlotUpdate next) =>
        pending.Kind == ReactivePlotUpdateKind.Append &&
        next.Kind == ReactivePlotUpdateKind.Append &&
        pending.Key == next.Key &&
        pending.PlotType == next.PlotType &&
        pending.XAxisKind == next.XAxisKind &&
        IsAppendPayloadValid(pending) &&
        IsAppendPayloadValid(next);

    private static bool IsAppendPayloadValid(ReactivePlotUpdate update) =>
        update.X is not null &&
        update.Y is not null &&
        update.X.Count > 0 &&
        update.Y.Count > 0 &&
        update.X.Count == update.Y.Count;

    private static void ApplyUpdate(
        ReactivePlotUpdate update,
        ReactivePlotBindingOptions options,
        IReactivePlotAdapterFactory adapterFactory,
        Dictionary<PlotSeriesKey, IReactivePlotAdapter> adapters,
        Dictionary<PlotSeriesKey, RetainedSeries> retained,
        HashSet<PlotSeriesKey> stoppedSeries,
        ReactivePlotConnection connection)
    {
        if (stoppedSeries.Contains(update.Key))
        {
            return;
        }

        if (!Validate(update, options, stoppedSeries, connection))
        {
            return;
        }

        var boundedUpdate = ApplyRetention(update, options, retained);
        if (!adapters.TryGetValue(boundedUpdate.Key, out var adapter))
        {
            adapter = adapterFactory.Create(boundedUpdate.Key, boundedUpdate.PlotType);
            adapters.Add(boundedUpdate.Key, adapter);
        }

        adapter.Apply(boundedUpdate);
        if (connection.CurrentState is ReactivePlotConnectionState.Connecting)
        {
            connection.SetState(ReactivePlotConnectionState.Active);
        }
    }

    private static bool Validate(ReactivePlotUpdate update, ReactivePlotBindingOptions options, HashSet<PlotSeriesKey> stoppedSeries, ReactivePlotConnection connection)
    {
        if (update.Key.Axis < 0 || update.Key.Axis >= options.MaxAxisCount)
        {
            return SurfaceValidationError(update, options, stoppedSeries, connection, $"Invalid Y-axis index: {update.Key.Axis}");
        }

        if (update.X is null || update.Y is null)
        {
            return SurfaceValidationError(update, options, stoppedSeries, connection, "Reactive plot update X and Y collections must be non-null.");
        }

        if (update.Kind == ReactivePlotUpdateKind.Clear)
        {
            return true;
        }

        if (update.X.Count == 0 || update.Y.Count == 0)
        {
            return SurfaceValidationError(update, options, stoppedSeries, connection, "Reactive plot update must contain at least one X and Y value.");
        }

        if (update.X.Count != update.Y.Count)
        {
            return SurfaceValidationError(update, options, stoppedSeries, connection, "Reactive plot update X and Y collections must have matching counts.");
        }

        return true;
    }

    private static bool SurfaceValidationError(ReactivePlotUpdate update, ReactivePlotBindingOptions options, HashSet<PlotSeriesKey> stoppedSeries, ReactivePlotConnection connection, string message)
    {
        if (options.ErrorMode == ReactivePlotErrorMode.IgnoreInvalidUpdates)
        {
            return false;
        }

        connection.AddError(new InvalidOperationException($"{message} Series='{update.Key.Name}', PlotType='{update.PlotType}'."));
        if (options.ErrorMode == ReactivePlotErrorMode.SurfaceAndStopSeries)
        {
            stoppedSeries.Add(update.Key);
            connection.SetState(ReactivePlotConnectionState.Faulted);
        }

        return false;
    }

    private static void HandleSourceError(IReactivePlotSource source, Exception error, ReactivePlotBindingOptions options, HashSet<PlotSeriesKey> stoppedSeries, ReactivePlotConnection connection)
    {
        connection.AddError(error);
        if (options.ErrorMode == ReactivePlotErrorMode.SurfaceAndStopSeries)
        {
            stoppedSeries.Add(source.Key);
            connection.SetState(ReactivePlotConnectionState.Faulted);
        }
    }

    private static ReactivePlotUpdate ApplyRetention(ReactivePlotUpdate update, ReactivePlotBindingOptions options, Dictionary<PlotSeriesKey, RetainedSeries> retained)
    {
        if (update.Kind == ReactivePlotUpdateKind.Clear)
        {
            if (retained.TryGetValue(update.Key, out var retainedSeries))
            {
                retainedSeries.Clear();
            }

            return update;
        }

        var maxVisiblePoints = update.MaxPoints ?? options.MaxVisiblePoints;
        if (maxVisiblePoints is not { } visiblePoints || visiblePoints <= 0)
        {
            return update;
        }

        if (update.PlotType == PlotType.DataLogger && update.Kind == ReactivePlotUpdateKind.Append)
        {
            return update with { MaxPoints = visiblePoints };
        }

        if (!retained.TryGetValue(update.Key, out var series) || update.Kind == ReactivePlotUpdateKind.Replace)
        {
            series = new RetainedSeries();
            retained[update.Key] = series;
        }

        series.Append(update.X, update.Y, visiblePoints, options.OverflowStrategy);
        return update with { X = series.X.ToArray(), Y = series.Y.ToArray(), MaxPoints = visiblePoints };
    }

    private sealed class RetainedSeries
    {
        public List<double> X { get; } = [];

        public List<double> Y { get; } = [];

        public void Append(IReadOnlyList<double> x, IReadOnlyList<double> y, int maxVisiblePoints, ReactivePlotOverflowStrategy overflowStrategy)
        {
            if (overflowStrategy == ReactivePlotOverflowStrategy.DropNewest && X.Count >= maxVisiblePoints)
            {
                return;
            }

            X.AddRange(x);
            Y.AddRange(y);

            if (X.Count <= maxVisiblePoints)
            {
                return;
            }

            var excess = X.Count - maxVisiblePoints;
            if (overflowStrategy == ReactivePlotOverflowStrategy.DropNewest)
            {
                X.RemoveRange(maxVisiblePoints, excess);
                Y.RemoveRange(maxVisiblePoints, excess);
            }
            else
            {
                X.RemoveRange(0, excess);
                Y.RemoveRange(0, excess);
            }
        }

        public void Clear()
        {
            X.Clear();
            Y.Clear();
        }
    }
}
