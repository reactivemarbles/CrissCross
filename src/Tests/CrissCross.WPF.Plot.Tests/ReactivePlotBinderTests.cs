// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Tests for IObservable-first WPF plot source adapters and binder lifecycle behavior.</summary>
public sealed class ReactivePlotBinderTests
{
    /// <summary>Verifies SignalTicksAdapter EmitsAppendUpdateWithTicksXAxisKind.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SignalTicksAdapter_EmitsAppendUpdateWithTicksXAxisKind()
    {
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> ticks =
            Observable.Return((Name: (string?)"Motor", Value: (IList<double>?)[1.0, 2.0], DateTime: (IList<double>)[100.0, 200.0], Axis: 2));
        var source = ReactivePlotSource.FromSignalTicks(ticks);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.XAxisKind).IsEqualTo(PlotXAxisKind.Ticks);
        await Assert.That(update.Key).IsEqualTo(new PlotSeriesKey("Motor", 2));
        await Assert.That(update.X.Count).IsEqualTo(2);
        await Assert.That(update.Y.Count).IsEqualTo(2);
    }

    /// <summary>Verifies SignalPointsAdapter EmitsAppendUpdateWithNumericXAxisKind.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SignalPointsAdapter_EmitsAppendUpdateWithNumericXAxisKind()
    {
        IObservable<(string? Name, IList<double>? Value, IList<double> X, int Axis)> points =
            Observable.Return((Name: (string?)"Motor", Value: (IList<double>?)[1.0, 2.0], X: (IList<double>)[10.0, 20.0], Axis: 2));
        var source = ReactivePlotSource.FromSignalPoints(points);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.XAxisKind).IsEqualTo(PlotXAxisKind.Numeric);
        await Assert.That(((ReactivePlotSource)source).XAxisKind).IsEqualTo(PlotXAxisKind.Numeric);
        await Assert.That(update.Key).IsEqualTo(new PlotSeriesKey("Motor", 2));
        await Assert.That(update.X).IsEquivalentTo([10.0, 20.0]);
        await Assert.That(update.Y).IsEquivalentTo([1.0, 2.0]);
    }

    /// <summary>Verifies SignalXyAdapter SupportsObservableReplaceUpdates.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SignalXyAdapter_SupportsObservableReplaceUpdates()
    {
        var snapshots = new Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)>();
        var source = ReactivePlotSource.FromSignalXyPoints(snapshots);
        var updates = new List<ReactivePlotUpdate>();

        using var subscription = source.Updates.Subscribe(updates.Add);
        snapshots.OnNext(("Vector", [1.0, 2.0], [10.0, 20.0], 1));
        snapshots.OnNext(("Vector", [3.0, 4.0], [30.0, 40.0], 1));

        await Assert.That(updates).Count().IsEqualTo(2);
        await Assert.That(updates.All(x => x.PlotType == PlotType.SignalXY)).IsTrue();
        await Assert.That(updates.All(x => x.Kind == ReactivePlotUpdateKind.Replace)).IsTrue();
    }

    /// <summary>Verifies ScatterPointsAdapter RejectsMismatchedXAndYCounts.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ScatterPointsAdapter_RejectsMismatchedXAndYCounts()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var binder = new ReactivePlotBinder(factory);
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> scatter =
            Observable.Return((Name: (string?)"Scatter", X: (IList<double>?)[1.0], Y: (IList<double>)[1.0, 2.0], Axis: 0));
        var source = ReactivePlotSource.FromScatterPoints(scatter);
        var errors = new List<Exception>();

        using var connection = binder.Bind([source], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var errorSubscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(factory.Adapters).IsEmpty();
        await Assert.That(errors).Count().IsEqualTo(1);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
    }

    /// <summary>Verifies Bind ReturnsConnectionThatDisposesSourceSubscription.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ReturnsConnectionThatDisposesSourceSubscription()
    {
        var disposed = false;
        var source = new ReactivePlotSource(
            new PlotSeriesKey("Signal", 0),
            PlotType.Signal,
            Observable.Create<ReactivePlotUpdate>(_ => new ActionDisposable(() => disposed = true)));
        var binder = new ReactivePlotBinder(new RecordingReactivePlotAdapterFactory());
        var states = new List<ReactivePlotConnectionState>();

        using var connection = binder.Bind([source], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var stateSubscription = connection.State.Subscribe(states.Add);
        connection.Dispose();

        await Assert.That(disposed).IsTrue();
        await Assert.That(states).Contains(ReactivePlotConnectionState.Disposed);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Disposed);
    }

    /// <summary>Verifies Bind UsesSingleSubscriptionPerSource.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_UsesSingleSubscriptionPerSource()
    {
        var subscriptions = 0;
        var update = CreateUpdate("Cold", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [2.0]);
        var source = new ReactivePlotSource(
            update.Key,
            update.PlotType,
            Observable.Create<ReactivePlotUpdate>(observer =>
            {
                subscriptions++;
                observer.OnNext(update);
                return EmptyDisposable.Instance;
            }));

        using var connection = new ReactivePlotBinder(new RecordingReactivePlotAdapterFactory())
            .Bind([source], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(subscriptions).IsEqualTo(1);
    }

    /// <summary>Verifies Bind CompletionEmitsCompletedStateWithoutDisposingAdapters.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_CompletionEmitsCompletedStateWithoutDisposingAdapters()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate("Finite", PlotType.DataLogger, 0, ReactivePlotUpdateKind.Append, [0.0], [42.0]);
        var states = new List<ReactivePlotConnectionState>();

        using var connection = new ReactivePlotBinder(factory)
            .Bind([ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var stateSubscription = connection.State.Subscribe(states.Add);

        await Assert.That(states).Contains(ReactivePlotConnectionState.Completed);
        await Assert.That(connection.IsCompleted).IsTrue();
        await Assert.That(factory.Adapters.Single().IsDisposed).IsFalse();
    }

    /// <summary>Verifies Bind SourceErrorEmitsFaultedStateAndError.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_SourceErrorEmitsFaultedStateAndError()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var expected = new InvalidOperationException("boom");
        var update = CreateUpdate("Fault", PlotType.Streamer, 0, ReactivePlotUpdateKind.Append, [0.0], [1.0]);
        var errors = new List<Exception>();

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Concat(Observable.Return(update), Observable.Throw<ReactivePlotUpdate>(expected)))],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var errorSubscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(errors.Single()).IsSameReferenceAs(expected);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters.Single().Updates).Count().IsEqualTo(1);
    }

    /// <summary>Verifies Bind MarshalsAdapterMutationToConfiguredUiScheduler.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_MarshalsAdapterMutationToConfiguredUiScheduler()
    {
        var scheduler = new ManualPumpScheduler();
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate("Scheduled", PlotType.SignalXY, 0, ReactivePlotUpdateKind.Replace, [1.0], [2.0]);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))],
            new ReactivePlotBindingOptions { UiScheduler = scheduler });

        await Assert.That(factory.Adapters).IsEmpty();
        scheduler.RunAll();
        await Assert.That(factory.Adapters.Single().Updates.Single()).IsEqualTo(update);
    }

    /// <summary>Verifies Bind LiveSourceEmitsImmediatelyWhenNoBatchWindowIsConfigured.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_LiveSourceEmitsImmediatelyWhenNoBatchWindowIsConfigured()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var updates = new Signal<ReactivePlotUpdate>();
        var update = CreateUpdate("Live", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [2.0]);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, updates)],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        updates.OnNext(update);

        await Assert.That(factory.Adapters.Single().Updates.Single()).IsEqualTo(update);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Active);
    }

    /// <summary>Verifies Bind BatchesHighFrequencyUpdatesBeforeUiScheduler.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchesHighFrequencyUpdatesBeforeUiScheduler()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var scheduler = new ManualPumpScheduler();
        var updates = Enumerable.Range(0, 1_000)
            .Select(i => CreateUpdate("Fast", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [(double)i], [(double)i], i));

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(new PlotSeriesKey("Fast", 0), PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = scheduler, BatchWindow = TimeSpan.FromMilliseconds(16), MaxBatchSize = 128 });
        scheduler.RunAll();

        var adapter = factory.Adapters.Single();
        await Assert.That(adapter.ApplyCallCount).IsLessThan(1_000);
        await Assert.That(adapter.Updates[^1].Sequence).IsEqualTo(999);
    }

    /// <summary>Verifies Bind DropOldestOverflowKeepsLatestVisiblePoints.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_DropOldestOverflowKeepsLatestVisiblePoints()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var updates = Enumerable.Range(0, 500)
            .Select(i => CreateUpdate("Bounded", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [(double)i], [(double)i], i));

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(new PlotSeriesKey("Bounded", 0), PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxVisiblePoints = 100 });

        var retained = factory.Adapters.Single().Updates[^1];
        await Assert.That(retained.X[0]).IsEqualTo(400.0);
        await Assert.That(retained.Y[retained.Y.Count - 1]).IsEqualTo(499.0);
        await Assert.That(retained.X.Count).IsEqualTo(100);
    }

    /// <summary>Verifies DataLoggerAdapter PreservesLegacyMaxPointCount.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataLoggerAdapter_PreservesLegacyMaxPointCount()
    {
        IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> points =
            Observable.Return((Name: (string?)"Logger", Value: (IList<double>?)[10.0, 11.0], Axis: 0, nMaxPoints: 25));
        var source = ReactivePlotSource.FromDataLoggerPoints(points);

        var update = await source.Updates.FirstAsync();
        var maxPoints = typeof(ReactivePlotUpdate).GetProperty("MaxPoints")?.GetValue(update);

        await Assert.That(maxPoints).IsEqualTo(25);
    }

    /// <summary>Verifies Bind BatchingPreservesMixedSeriesKeys.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingPreservesMixedSeriesKeys()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var firstKey = new PlotSeriesKey("A", 0);
        var secondKey = new PlotSeriesKey("B", 1);
        var updates = new[]
        {
            CreateUpdate("A", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [10.0]),
            CreateUpdate("B", PlotType.Signal, 1, ReactivePlotUpdateKind.Append, [2.0], [20.0]),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(firstKey, PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = 2 });

        await Assert.That(factory.Find(firstKey).Updates.Single().Y).IsEquivalentTo([10.0]);
        await Assert.That(factory.Find(secondKey).Updates.Single().Y).IsEquivalentTo([20.0]);
    }

    /// <summary>Verifies Bind BatchingPreservesReplaceAndClearBoundaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingPreservesReplaceAndClearBoundaries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey("Window", 0);
        var updates = new[]
        {
            CreateUpdate("Window", PlotType.Scatter, 0, ReactivePlotUpdateKind.Append, [1.0], [1.0]),
            CreateUpdate("Window", PlotType.Scatter, 0, ReactivePlotUpdateKind.Clear, [], []),
            CreateUpdate("Window", PlotType.Scatter, 0, ReactivePlotUpdateKind.Replace, [2.0], [2.0]),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Scatter, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = 3 });

        await Assert.That(factory.Find(key).Updates.Select(x => x.Kind).ToArray())
            .IsEquivalentTo([ReactivePlotUpdateKind.Append, ReactivePlotUpdateKind.Clear, ReactivePlotUpdateKind.Replace]);
    }

    /// <summary>Verifies Bind BatchingDoesNotHideInvalidAppendUpdates.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingDoesNotHideInvalidAppendUpdates()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey("InvalidBatch", 0);
        var updates = new[]
        {
            CreateUpdate("InvalidBatch", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [], []),
            CreateUpdate("InvalidBatch", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [1.0]),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = 2 });

        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters).IsEmpty();
    }

    /// <summary>Verifies Bind ClearResetsPerUpdateRetainedPoints.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ClearResetsPerUpdateRetainedPoints()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey("Retained", 0);
        var updates = new[]
        {
            CreateUpdate("Retained", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [1.0]) with { MaxPoints = 2 },
            CreateUpdate("Retained", PlotType.Signal, 0, ReactivePlotUpdateKind.Clear, [], []),
            CreateUpdate("Retained", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [2.0], [2.0]) with { MaxPoints = 2 },
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(factory.Find(key).Updates[^1].Y).IsEquivalentTo([2.0]);
    }

    /// <summary>Verifies SurfaceAndStopSeries StopsOnlyFaultedSeries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SurfaceAndStopSeries_StopsOnlyFaultedSeries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var first = new Signal<ReactivePlotUpdate>();
        var second = new Signal<ReactivePlotUpdate>();
        var firstKey = new PlotSeriesKey("Faulted", 0);
        var secondKey = new PlotSeriesKey("Healthy", 1);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(firstKey, PlotType.Signal, first),
                ReactivePlotSource.FromUpdates(secondKey, PlotType.Signal, second),
            ],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries });

        first.OnNext(CreateUpdate("Faulted", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [], []));
        first.OnNext(CreateUpdate("Faulted", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [1.0]));
        second.OnNext(CreateUpdate("Healthy", PlotType.Signal, 1, ReactivePlotUpdateKind.Append, [2.0], [2.0]));

        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters.Select(x => x.Key).ToArray()).IsEquivalentTo([secondKey]);
        await Assert.That(factory.Find(secondKey).Updates.Single().Y).IsEquivalentTo([2.0]);
    }

    /// <summary>Verifies Bind CreatesAdapterForEveryPlotType.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_CreatesAdapterForEveryPlotType()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var sources = Enum.GetValues<PlotType>()
            .Select(plotType => CreateUpdate(plotType.ToString(), plotType, 0, ReactivePlotUpdateKind.Append, [1.0], [2.0]))
            .Select(update => ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update)))
            .ToArray();

        using var connection = new ReactivePlotBinder(factory)
            .Bind(sources, new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(factory.Adapters.Select(x => x.PlotType).OrderBy(x => x.ToString()).ToArray())
            .IsEquivalentTo(Enum.GetValues<PlotType>().OrderBy(x => x.ToString()).ToArray());
    }

    /// <summary>Verifies Bind DoesNotReinitializeUnchangedSeries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_DoesNotReinitializeUnchangedSeries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var first = new Signal<ReactivePlotUpdate>();
        var second = new Signal<ReactivePlotUpdate>();
        var firstKey = new PlotSeriesKey("A", 0);
        var secondKey = new PlotSeriesKey("B", 1);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(firstKey, PlotType.Scatter, first),
                ReactivePlotSource.FromUpdates(secondKey, PlotType.Scatter, second),
            ],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        first.OnNext(CreateUpdate("A", PlotType.Scatter, 0, ReactivePlotUpdateKind.Replace, [1.0], [1.0]));
        second.OnNext(CreateUpdate("B", PlotType.Scatter, 1, ReactivePlotUpdateKind.Replace, [2.0], [2.0]));
        first.OnNext(CreateUpdate("A", PlotType.Scatter, 0, ReactivePlotUpdateKind.Replace, [3.0], [3.0]));

        await Assert.That(factory.CreatedAdapters).IsEqualTo(2);
        await Assert.That(factory.Find(firstKey).Updates).Count().IsEqualTo(2);
        await Assert.That(factory.Find(secondKey).Updates).Count().IsEqualTo(1);
    }

    /// <summary>Verifies Bind ClearUpdateClearsOnlyTargetSeries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ClearUpdateClearsOnlyTargetSeries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var firstKey = new PlotSeriesKey("A", 0);
        var secondKey = new PlotSeriesKey("B", 1);
        var updates = new[]
        {
            CreateUpdate("A", PlotType.SignalXY, 0, ReactivePlotUpdateKind.Replace, [1.0], [1.0]),
            CreateUpdate("B", PlotType.SignalXY, 1, ReactivePlotUpdateKind.Replace, [2.0], [2.0]),
            CreateUpdate("A", PlotType.SignalXY, 0, ReactivePlotUpdateKind.Clear, [], []),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            updates.Select(x => ReactivePlotSource.FromUpdates(x.Key, x.PlotType, Observable.Return(x))),
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(factory.Find(firstKey).ClearCount).IsEqualTo(1);
        await Assert.That(factory.Find(secondKey).ClearCount).IsEqualTo(0);
    }

    /// <summary>Verifies InvalidAxisIndex EmitsValidationErrorBeforeUiMutation.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task InvalidAxisIndex_EmitsValidationErrorBeforeUiMutation()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate("BadAxis", PlotType.Signal, -1, ReactivePlotUpdateKind.Append, [1.0], [2.0]);
        var errors = new List<Exception>();

        using var connection = new ReactivePlotBinder(factory)
            .Bind([ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var subscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(factory.Adapters).IsEmpty();
        await Assert.That(errors).Count().IsEqualTo(1);
    }

    /// <summary>Verifies NullOrEmptySeriesData IsIgnoredOrFaultedAccordingToOptions.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NullOrEmptySeriesData_IsIgnoredOrFaultedAccordingToOptions()
    {
        var bad = CreateUpdate("Bad", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [], []);
        var ignoredFactory = new RecordingReactivePlotAdapterFactory();
        var faultedFactory = new RecordingReactivePlotAdapterFactory();

        using var ignored = new ReactivePlotBinder(ignoredFactory).Bind(
            [ReactivePlotSource.FromUpdates(bad.Key, bad.PlotType, Observable.Return(bad))],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, ErrorMode = ReactivePlotErrorMode.IgnoreInvalidUpdates });
        using var faulted = new ReactivePlotBinder(faultedFactory).Bind(
            [ReactivePlotSource.FromUpdates(bad.Key, bad.PlotType, Observable.Return(bad))],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries });

        await Assert.That(ignored.CurrentState).IsNotEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(ignoredFactory.Adapters).IsEmpty();
        await Assert.That(faulted.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(faultedFactory.Adapters).IsEmpty();
    }

    /// <summary>Verifies SurfaceAndStopSeries DoesNotMutateAfterValidationFault.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SurfaceAndStopSeries_DoesNotMutateAfterValidationFault()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var updates = new Signal<ReactivePlotUpdate>();
        var bad = CreateUpdate("Faulted", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [], []);
        var good = CreateUpdate("Faulted", PlotType.Signal, 0, ReactivePlotUpdateKind.Append, [1.0], [2.0]);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(bad.Key, bad.PlotType, updates)],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries });
        updates.OnNext(bad);
        updates.OnNext(good);

        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters).IsEmpty();
    }

    /// <summary>Creates a normalized plot update for binder tests.</summary>
    /// <param name="name">The series name.</param>
    /// <param name="plotType">The plot type.</param>
    /// <param name="axis">The axis index.</param>
    /// <param name="kind">The update kind.</param>
    /// <param name="x">The X values.</param>
    /// <param name="y">The Y values.</param>
    /// <param name="sequence">The sequence number.</param>
    /// <returns>The plot update.</returns>
    private static ReactivePlotUpdate CreateUpdate(
        string name,
        PlotType plotType,
        int axis,
        ReactivePlotUpdateKind kind,
        IReadOnlyList<double> x,
        IReadOnlyList<double> y,
        long sequence = 0) =>
        new(new PlotSeriesKey(name, axis), plotType, kind, x, y, PlotXAxisKind.Numeric, sequence);
}
