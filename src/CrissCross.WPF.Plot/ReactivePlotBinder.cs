// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.Plot;

/// <summary>Binds reactive plot sources with validation, batching, lifecycle, and UI dispatch.</summary>
public sealed class ReactivePlotBinder : IReactivePlotBinder
{
    /// <summary>Stores the adapter factory value.</summary>
    private readonly IReactivePlotAdapterFactory? _adapterFactory;

    /// <summary>Initializes a new instance of the <see cref="ReactivePlotBinder"/> class.</summary>
    public ReactivePlotBinder() { }

    /// <summary>Initializes a new instance of the <see cref="ReactivePlotBinder"/> class.</summary>
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
    public IReactivePlotConnection Bind(LiveChartViewModel chart, IEnumerable<IReactivePlotSource> sources) =>
        Bind(chart, sources, null);

    /// <inheritdoc />
    public IReactivePlotConnection Bind(
        LiveChartViewModel chart,
        IEnumerable<IReactivePlotSource> sources,
        ReactivePlotBindingOptions? options)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        return BindCore(sources, new WpfReactivePlotAdapterFactory(chart), options);
    }

    /// <summary>Binds the supplied sources using the adapter factory supplied to the constructor.</summary>
    /// <param name="sources">The sources to bind.</param>
    /// <returns>An owned connection.</returns>
    public IReactivePlotConnection Bind(IEnumerable<IReactivePlotSource> sources) => Bind(sources, null);

    /// <summary>Binds the supplied sources using the adapter factory supplied to the constructor.</summary>
    /// <param name="sources">The sources to bind.</param>
    /// <param name="options">Optional binding options.</param>
    /// <returns>An owned connection.</returns>
    public IReactivePlotConnection Bind(IEnumerable<IReactivePlotSource> sources, ReactivePlotBindingOptions? options)
    {
        if (_adapterFactory is null)
        {
            throw new InvalidOperationException("This binder was not created with an adapter factory.");
        }

        return BindCore(sources, _adapterFactory, options);
    }

    /// <summary>Handles the BindCore operation.</summary>
    /// <param name="sources">The sources value.</param>
    /// <param name="adapterFactory">The adapterFactory value.</param>
    /// <param name="options">The options value.</param>
    /// <returns>The result.</returns>
    private static ReactivePlotConnection BindCore(
        IEnumerable<IReactivePlotSource> sources,
        IReactivePlotAdapterFactory adapterFactory,
        ReactivePlotBindingOptions? options)
    {
        ValidateBindingArguments(sources, adapterFactory);

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
                update =>
                    ApplyUpdate(update, bindingOptions, adapterFactory, adapters, retained, stoppedSeries, connection),
                error => HandleSourceError(source, error, bindingOptions, stoppedSeries, connection),
                () =>
                {
                    completed++;
                    if (
                        completed != sourceArray.Length
                        || connection.CurrentState == ReactivePlotConnectionState.Faulted)
                    {
                        return;
                    }

                    connection.MarkCompleted();
                });
            subscriptions.Add(subscription);
        }

        return connection;
    }

    /// <summary>Validates the required binding collaborators.</summary>
    /// <param name="sources">The sources to bind.</param>
    /// <param name="adapterFactory">The adapter factory.</param>
    private static void ValidateBindingArguments(
        IEnumerable<IReactivePlotSource> sources,
        IReactivePlotAdapterFactory adapterFactory)
    {
        ValidateBindingArgument(sources, nameof(sources));
        ValidateBindingArgument(adapterFactory, nameof(adapterFactory));
    }

    /// <summary>Validates one required binding collaborator.</summary>
    /// <typeparam name="T">The collaborator type.</typeparam>
    /// <param name="value">The collaborator value.</param>
    /// <param name="parameterName">The public parameter name.</param>
    private static void ValidateBindingArgument<T>(T value, string parameterName)
        where T : class
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(parameterName);
    }

    /// <summary>Handles the ApplyBatching operation.</summary>
    /// <param name="updates">The updates value.</param>
    /// <param name="options">The options value.</param>
    /// <returns>The result.</returns>
    private static IObservable<ReactivePlotUpdate> ApplyBatching(
        IObservable<ReactivePlotUpdate> updates,
        ReactivePlotBindingOptions options)
    {
        if (options.BatchWindow is { } batchWindow && batchWindow > TimeSpan.Zero)
        {
            return updates
                .Buffer(batchWindow, options.SourceScheduler ?? RxSchedulers.TaskpoolScheduler)
                .Where(batch => batch.Count > 0)
                .SelectMany(AggregateBatch);
        }

        return options.MaxBatchSize > 1
            ? updates.Buffer(options.MaxBatchSize).Where(batch => batch.Count > 0).SelectMany(AggregateBatch)
            : updates;
    }

    /// <summary>Handles the AggregateBatch operation.</summary>
    /// <param name="batch">The batch value.</param>
    /// <returns>The result.</returns>
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
            if (TryAggregateAppend(ref pendingAppend, update))
            {
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

        if (pendingAppend is null)
        {
            yield break;
        }

        yield return pendingAppend;
    }

    /// <summary>Attempts to aggregate an append update into a pending append update.</summary>
    /// <param name="pendingAppend">The pending append update.</param>
    /// <param name="update">The next update.</param>
    /// <returns><see langword="true"/> when aggregation was applied; otherwise, <see langword="false"/>.</returns>
    private static bool TryAggregateAppend(ref ReactivePlotUpdate? pendingAppend, ReactivePlotUpdate update)
    {
        if (pendingAppend is null || !CanAggregateAppend(pendingAppend, update))
        {
            return false;
        }

        pendingAppend = pendingAppend with
        {
            X = pendingAppend.X.Concat(update.X).ToArray(),
            Y = pendingAppend.Y.Concat(update.Y).ToArray(),
            Sequence = update.Sequence,
            MaxPoints = update.MaxPoints ?? pendingAppend.MaxPoints,
        };
        return true;
    }

    /// <summary>Handles the CanAggregateAppend operation.</summary>
    /// <param name="pending">The pending value.</param>
    /// <param name="next">The next value.</param>
    /// <returns>The result.</returns>
    private static bool CanAggregateAppend(ReactivePlotUpdate pending, ReactivePlotUpdate next) =>
        pending.Kind == ReactivePlotUpdateKind.Append
        && next.Kind == ReactivePlotUpdateKind.Append
        && pending.Key == next.Key
        && pending.PlotType == next.PlotType
        && pending.XAxisKind == next.XAxisKind
        && pending.Style == next.Style
        && IsAppendPayloadValid(pending)
        && IsAppendPayloadValid(next);

    /// <summary>Handles the IsAppendPayloadValid operation.</summary>
    /// <param name="update">The update value.</param>
    /// <returns>The result.</returns>
    private static bool IsAppendPayloadValid(ReactivePlotUpdate update) =>
        update.X is not null
        && update.Y is not null
        && update.X.Count > 0
        && update.Y.Count > 0
        && update.X.Count == update.Y.Count;

    /// <summary>Handles the ApplyUpdate operation.</summary>
    /// <param name="update">The update value.</param>
    /// <param name="options">The options value.</param>
    /// <param name="adapterFactory">The adapterFactory value.</param>
    /// <param name="adapters">The adapters value.</param>
    /// <param name="retained">The retained value.</param>
    /// <param name="stoppedSeries">The stoppedSeries value.</param>
    /// <param name="connection">The connection value.</param>
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
        if (connection.CurrentState is not ReactivePlotConnectionState.Connecting)
        {
            return;
        }

        connection.SetState(ReactivePlotConnectionState.Active);
    }

    /// <summary>Handles the Validate operation.</summary>
    /// <param name="update">The update value.</param>
    /// <param name="options">The options value.</param>
    /// <param name="stoppedSeries">The stoppedSeries value.</param>
    /// <param name="connection">The connection value.</param>
    /// <returns>The result.</returns>
    private static bool Validate(
        ReactivePlotUpdate update,
        ReactivePlotBindingOptions options,
        HashSet<PlotSeriesKey> stoppedSeries,
        ReactivePlotConnection connection)
    {
        if (update.Key.Axis < 0 || update.Key.Axis >= options.MaxAxisCount)
        {
            return SurfaceValidationError(
                update,
                options,
                stoppedSeries,
                connection,
                $"Invalid Y-axis index: {update.Key.Axis}");
        }

        if (update.X is null || update.Y is null)
        {
            return SurfaceValidationError(
                update,
                options,
                stoppedSeries,
                connection,
                "Reactive plot update X and Y collections must be non-null.");
        }

        if (update.Kind == ReactivePlotUpdateKind.Clear)
        {
            return true;
        }

        if (update.X.Count == 0 || update.Y.Count == 0)
        {
            return SurfaceValidationError(
                update,
                options,
                stoppedSeries,
                connection,
                "Reactive plot update must contain at least one X and Y value.");
        }

        return update.X.Count != update.Y.Count
            ? SurfaceValidationError(
                update,
                options,
                stoppedSeries,
                connection,
                "Reactive plot update X and Y collections must have matching counts.")
            : true;
    }

    /// <summary>Handles the SurfaceValidationError operation.</summary>
    /// <param name="update">The update value.</param>
    /// <param name="options">The options value.</param>
    /// <param name="stoppedSeries">The stoppedSeries value.</param>
    /// <param name="connection">The connection value.</param>
    /// <param name="message">The message value.</param>
    /// <returns>The result.</returns>
    private static bool SurfaceValidationError(
        ReactivePlotUpdate update,
        ReactivePlotBindingOptions options,
        HashSet<PlotSeriesKey> stoppedSeries,
        ReactivePlotConnection connection,
        string message)
    {
        if (options.ErrorMode == ReactivePlotErrorMode.IgnoreInvalidUpdates)
        {
            return false;
        }

        connection.AddError(
            new InvalidOperationException($"{message} Series='{update.Key.Name}', PlotType='{update.PlotType}'."));
        if (options.ErrorMode != ReactivePlotErrorMode.SurfaceAndStopSeries)
        {
            return false;
        }

        _ = stoppedSeries.Add(update.Key);
        connection.SetState(ReactivePlotConnectionState.Faulted);

        return false;
    }

    /// <summary>Handles the HandleSourceError operation.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="error">The error value.</param>
    /// <param name="options">The options value.</param>
    /// <param name="stoppedSeries">The stoppedSeries value.</param>
    /// <param name="connection">The connection value.</param>
    private static void HandleSourceError(
        IReactivePlotSource source,
        Exception error,
        ReactivePlotBindingOptions options,
        HashSet<PlotSeriesKey> stoppedSeries,
        ReactivePlotConnection connection)
    {
        connection.AddError(error);
        if (options.ErrorMode != ReactivePlotErrorMode.SurfaceAndStopSeries)
        {
            return;
        }

        _ = stoppedSeries.Add(source.Key);
        connection.SetState(ReactivePlotConnectionState.Faulted);
    }

    /// <summary>Handles the ApplyRetention operation.</summary>
    /// <param name="update">The update value.</param>
    /// <param name="options">The options value.</param>
    /// <param name="retained">The retained value.</param>
    /// <returns>The result.</returns>
    private static ReactivePlotUpdate ApplyRetention(
        ReactivePlotUpdate update,
        ReactivePlotBindingOptions options,
        Dictionary<PlotSeriesKey, RetainedSeries> retained)
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
            series = new();
            retained[update.Key] = series;
        }

        series.Append(update.X, update.Y, visiblePoints, options.OverflowStrategy);
        return update with { X = series.X.ToArray(), Y = series.Y.ToArray(), MaxPoints = visiblePoints };
    }

    /// <summary>Provides the <see cref="RetainedSeries"/> type.</summary>
    private sealed class RetainedSeries
    {
        /// <summary>Gets the retained X values.</summary>
        public List<double> X { get; } = [];

        /// <summary>Gets the retained Y values.</summary>
        public List<double> Y { get; } = [];

        /// <summary>Handles the Append operation.</summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="maxVisiblePoints">The maxVisiblePoints value.</param>
        /// <param name="overflowStrategy">The overflowStrategy value.</param>
        public void Append(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            int maxVisiblePoints,
            ReactivePlotOverflowStrategy overflowStrategy)
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

        /// <summary>Handles the Clear operation.</summary>
        public void Clear()
        {
            X.Clear();
            Y.Clear();
        }
    }
}
