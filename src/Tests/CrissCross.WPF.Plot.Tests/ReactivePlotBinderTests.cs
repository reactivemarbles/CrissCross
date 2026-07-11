// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Tests for IObservable-first WPF plot source adapters and binder lifecycle behavior.</summary>
public sealed class ReactivePlotBinderTests
{
    /// <summary>The default plot axis index.</summary>
    private const int DefaultAxisIndex = 0;

    /// <summary>The secondary plot axis index.</summary>
    private const int SecondaryAxisIndex = 1;

    /// <summary>The motor plot axis index.</summary>
    private const int MotorAxisIndex = 2;

    /// <summary>The expected count for a single item.</summary>
    private const int ExpectedSingleItemCount = 1;

    /// <summary>The expected count for a pair of items.</summary>
    private const int ExpectedPairItemCount = 2;

    /// <summary>The number of updates in a mixed-series batch.</summary>
    private const int MixedSeriesBatchSize = 2;

    /// <summary>The number of updates in a replace-and-clear batch.</summary>
    private const int ReplaceClearBatchSize = 3;

    /// <summary>The maximum retained points for the bounded series test.</summary>
    private const int RetainedSeriesMaxPoints = 2;

    /// <summary>The number of updates used by the high-frequency test.</summary>
    private const int HighFrequencyUpdateCount = 1_000;

    /// <summary>The final sequence number in the high-frequency test.</summary>
    private const int LastHighFrequencySequence = 999;

    /// <summary>The batch-window duration in milliseconds.</summary>
    private const int BatchWindowMilliseconds = 16;

    /// <summary>The maximum number of updates in one high-frequency batch.</summary>
    private const int HighFrequencyMaxBatchSize = 128;

    /// <summary>The number of updates used by the overflow test.</summary>
    private const int OverflowUpdateCount = 500;

    /// <summary>The visible-point limit used by the overflow test.</summary>
    private const int VisiblePointLimit = 100;

    /// <summary>The legacy data logger point limit.</summary>
    private const int LegacyMaxPointCount = 25;

    /// <summary>The first sample value.</summary>
    private const double FirstSampleValue = 1.0;

    /// <summary>The second sample value.</summary>
    private const double SecondSampleValue = 2.0;

    /// <summary>The third sample value.</summary>
    private const double ThirdSampleValue = 3.0;

    /// <summary>The fourth sample value.</summary>
    private const double FourthSampleValue = 4.0;

    /// <summary>The first numeric x-axis value.</summary>
    private const double FirstNumericXValue = 10.0;

    /// <summary>The second numeric x-axis value.</summary>
    private const double SecondNumericXValue = 20.0;

    /// <summary>The third numeric x-axis value.</summary>
    private const double ThirdNumericXValue = 30.0;

    /// <summary>The fourth numeric x-axis value.</summary>
    private const double FourthNumericXValue = 40.0;

    /// <summary>The first tick value.</summary>
    private const double FirstTickValue = 100.0;

    /// <summary>The second tick value.</summary>
    private const double SecondTickValue = 200.0;

    /// <summary>The sample value emitted before completion.</summary>
    private const double CompletedSampleValue = 42.0;

    /// <summary>The first data logger value.</summary>
    private const double LoggerFirstValue = 10.0;

    /// <summary>The second data logger value.</summary>
    private const double LoggerSecondValue = 11.0;

    /// <summary>The first point retained after overflow.</summary>
    private const double FirstRetainedVisiblePoint = 400.0;

    /// <summary>The final point retained after overflow.</summary>
    private const double LastRetainedVisiblePoint = 499.0;

    /// <summary>Verifies SignalTicksAdapter EmitsAppendUpdateWithTicksXAxisKind.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SignalTicksAdapter_EmitsAppendUpdateWithTicksXAxisKind()
    {
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> ticks =
            Observable.Return((Name: (string?)"Motor", Value: (IList<double>?)[FirstSampleValue, SecondSampleValue], DateTime: (IList<double>)[FirstTickValue, SecondTickValue], Axis: MotorAxisIndex));
        var source = ReactivePlotSource.FromSignalTicks(ticks);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.XAxisKind).IsEqualTo(PlotXAxisKind.Ticks);
        await Assert.That(update.Key).IsEqualTo(new PlotSeriesKey("Motor", MotorAxisIndex));
        await Assert.That(update.X.Count).IsEqualTo(ExpectedPairItemCount);
        await Assert.That(update.Y.Count).IsEqualTo(ExpectedPairItemCount);
    }

    /// <summary>Verifies SignalPointsAdapter EmitsAppendUpdateWithNumericXAxisKind.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SignalPointsAdapter_EmitsAppendUpdateWithNumericXAxisKind()
    {
        IObservable<(string? Name, IList<double>? Value, IList<double> X, int Axis)> points =
            Observable.Return((Name: (string?)"Motor", Value: (IList<double>?)[FirstSampleValue, SecondSampleValue], X: (IList<double>)[FirstNumericXValue, SecondNumericXValue], Axis: MotorAxisIndex));
        var source = ReactivePlotSource.FromSignalPoints(points);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.XAxisKind).IsEqualTo(PlotXAxisKind.Numeric);
        await Assert.That(((ReactivePlotSource)source).XAxisKind).IsEqualTo(PlotXAxisKind.Numeric);
        await Assert.That(update.Key).IsEqualTo(new PlotSeriesKey("Motor", MotorAxisIndex));
        await Assert.That(update.X).IsEquivalentTo([FirstNumericXValue, SecondNumericXValue]);
        await Assert.That(update.Y).IsEquivalentTo([FirstSampleValue, SecondSampleValue]);
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
        snapshots.OnNext(("Vector", [FirstSampleValue, SecondSampleValue], [FirstNumericXValue, SecondNumericXValue], SecondaryAxisIndex));
        snapshots.OnNext(("Vector", [ThirdSampleValue, FourthSampleValue], [ThirdNumericXValue, FourthNumericXValue], SecondaryAxisIndex));

        await Assert.That(updates).Count().IsEqualTo(ExpectedPairItemCount);
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
            Observable.Return((Name: (string?)"Scatter", X: (IList<double>?)[FirstSampleValue], Y: (IList<double>)[FirstSampleValue, SecondSampleValue], Axis: DefaultAxisIndex));
        var source = ReactivePlotSource.FromScatterPoints(scatter);
        var errors = new List<Exception>();

        using var connection = binder.Bind([source], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var errorSubscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(factory.Adapters).IsEmpty();
        await Assert.That(errors).Count().IsEqualTo(ExpectedSingleItemCount);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
    }

    /// <summary>Verifies Bind ReturnsConnectionThatDisposesSourceSubscription.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ReturnsConnectionThatDisposesSourceSubscription()
    {
        var disposed = false;
        var source = new ReactivePlotSource(
            new PlotSeriesKey(nameof(Signal), DefaultAxisIndex),
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
        var update = CreateUpdate("Cold", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [SecondSampleValue]);
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

        await Assert.That(subscriptions).IsEqualTo(ExpectedSingleItemCount);
    }

    /// <summary>Verifies Bind CompletionEmitsCompletedStateWithoutDisposingAdapters.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_CompletionEmitsCompletedStateWithoutDisposingAdapters()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate("Finite", PlotType.DataLogger, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [0.0], [CompletedSampleValue]);
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
        var update = CreateUpdate("Fault", PlotType.Streamer, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [0.0], [FirstSampleValue]);
        var errors = new List<Exception>();

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Concat(Observable.Return(update), Observable.Throw<ReactivePlotUpdate>(expected)))],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var errorSubscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(errors.Single()).IsSameReferenceAs(expected);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters.Single().Updates).Count().IsEqualTo(ExpectedSingleItemCount);
    }

    /// <summary>Verifies Bind MarshalsAdapterMutationToConfiguredUiScheduler.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_MarshalsAdapterMutationToConfiguredUiScheduler()
    {
        var scheduler = new ManualPumpScheduler();
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate("Scheduled", PlotType.SignalXY, DefaultAxisIndex, ReactivePlotUpdateKind.Replace, [FirstSampleValue], [SecondSampleValue]);

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
        var update = CreateUpdate("Live", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [SecondSampleValue]);

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
        var updates = Enumerable.Range(DefaultAxisIndex, HighFrequencyUpdateCount)
            .Select(i => CreateUpdate("Fast", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [(double)i], [(double)i], i));

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(new PlotSeriesKey("Fast", DefaultAxisIndex), PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = scheduler, BatchWindow = TimeSpan.FromMilliseconds(BatchWindowMilliseconds), MaxBatchSize = HighFrequencyMaxBatchSize });
        scheduler.RunAll();

        var adapter = factory.Adapters.Single();
        await Assert.That(adapter.ApplyCallCount).IsLessThan(HighFrequencyUpdateCount);
        await Assert.That(adapter.Updates[^1].Sequence).IsEqualTo(LastHighFrequencySequence);
    }

    /// <summary>Verifies Bind DropOldestOverflowKeepsLatestVisiblePoints.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_DropOldestOverflowKeepsLatestVisiblePoints()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var updates = Enumerable.Range(DefaultAxisIndex, OverflowUpdateCount)
            .Select(i => CreateUpdate("Bounded", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [(double)i], [(double)i], i));

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(new PlotSeriesKey("Bounded", DefaultAxisIndex), PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxVisiblePoints = VisiblePointLimit });

        var retained = factory.Adapters.Single().Updates[^1];
        await Assert.That(retained.X[DefaultAxisIndex]).IsEqualTo(FirstRetainedVisiblePoint);
        await Assert.That(retained.Y[retained.Y.Count - 1]).IsEqualTo(LastRetainedVisiblePoint);
        await Assert.That(retained.X.Count).IsEqualTo(VisiblePointLimit);
    }

    /// <summary>Verifies DataLoggerAdapter PreservesLegacyMaxPointCount.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataLoggerAdapter_PreservesLegacyMaxPointCount()
    {
        IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> points =
            Observable.Return((Name: (string?)"Logger", Value: (IList<double>?)[LoggerFirstValue, LoggerSecondValue], Axis: DefaultAxisIndex, nMaxPoints: LegacyMaxPointCount));
        var source = ReactivePlotSource.FromDataLoggerPoints(points);

        var update = await source.Updates.FirstAsync();
        var maxPoints = typeof(ReactivePlotUpdate).GetProperty("MaxPoints")?.GetValue(update);

        await Assert.That(maxPoints).IsEqualTo(LegacyMaxPointCount);
    }

    /// <summary>Verifies Bind BatchingPreservesMixedSeriesKeys.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingPreservesMixedSeriesKeys()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var firstKey = new PlotSeriesKey("A", DefaultAxisIndex);
        var secondKey = new PlotSeriesKey("B", SecondaryAxisIndex);
        var updates = new[]
        {
            CreateUpdate("A", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [FirstNumericXValue]),
            CreateUpdate("B", PlotType.Signal, SecondaryAxisIndex, ReactivePlotUpdateKind.Append, [SecondSampleValue], [SecondNumericXValue]),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(firstKey, PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = MixedSeriesBatchSize });

        await Assert.That(factory.Find(firstKey).Updates.Single().Y).IsEquivalentTo([FirstNumericXValue]);
        await Assert.That(factory.Find(secondKey).Updates.Single().Y).IsEquivalentTo([SecondNumericXValue]);
    }

    /// <summary>Verifies Bind BatchingPreservesReplaceAndClearBoundaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingPreservesReplaceAndClearBoundaries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey("Window", DefaultAxisIndex);
        var updates = new[]
        {
            CreateUpdate("Window", PlotType.Scatter, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [FirstSampleValue]),
            CreateUpdate("Window", PlotType.Scatter, DefaultAxisIndex, ReactivePlotUpdateKind.Clear, [], []),
            CreateUpdate("Window", PlotType.Scatter, DefaultAxisIndex, ReactivePlotUpdateKind.Replace, [SecondSampleValue], [SecondSampleValue]),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Scatter, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = ReplaceClearBatchSize });

        await Assert.That(factory.Find(key).Updates.Select(x => x.Kind).ToArray())
            .IsEquivalentTo([ReactivePlotUpdateKind.Append, ReactivePlotUpdateKind.Clear, ReactivePlotUpdateKind.Replace]);
    }

    /// <summary>Verifies Bind BatchingDoesNotHideInvalidAppendUpdates.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingDoesNotHideInvalidAppendUpdates()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey("InvalidBatch", DefaultAxisIndex);
        var updates = new[]
        {
            CreateUpdate("InvalidBatch", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [], []),
            CreateUpdate("InvalidBatch", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [FirstSampleValue]),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = MixedSeriesBatchSize });

        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters).IsEmpty();
    }

    /// <summary>Verifies Bind ClearResetsPerUpdateRetainedPoints.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ClearResetsPerUpdateRetainedPoints()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey("Retained", DefaultAxisIndex);
        var updates = new[]
        {
            CreateUpdate("Retained", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [FirstSampleValue]) with { MaxPoints = RetainedSeriesMaxPoints },
            CreateUpdate("Retained", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Clear, [], []),
            CreateUpdate("Retained", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [SecondSampleValue], [SecondSampleValue]) with { MaxPoints = RetainedSeriesMaxPoints },
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates.ToObservable())],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(factory.Find(key).Updates[^1].Y).IsEquivalentTo([SecondSampleValue]);
    }

    /// <summary>Verifies SurfaceAndStopSeries StopsOnlyFaultedSeries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SurfaceAndStopSeries_StopsOnlyFaultedSeries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var first = new Signal<ReactivePlotUpdate>();
        var second = new Signal<ReactivePlotUpdate>();
        var firstKey = new PlotSeriesKey("Faulted", DefaultAxisIndex);
        var secondKey = new PlotSeriesKey("Healthy", SecondaryAxisIndex);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(firstKey, PlotType.Signal, first),
                ReactivePlotSource.FromUpdates(secondKey, PlotType.Signal, second),
            ],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance, ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries });

        first.OnNext(CreateUpdate("Faulted", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [], []));
        first.OnNext(CreateUpdate("Faulted", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [FirstSampleValue]));
        second.OnNext(CreateUpdate("Healthy", PlotType.Signal, SecondaryAxisIndex, ReactivePlotUpdateKind.Append, [SecondSampleValue], [SecondSampleValue]));

        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters.Select(x => x.Key).ToArray()).IsEquivalentTo([secondKey]);
        await Assert.That(factory.Find(secondKey).Updates.Single().Y).IsEquivalentTo([SecondSampleValue]);
    }

    /// <summary>Verifies Bind CreatesAdapterForEveryPlotType.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_CreatesAdapterForEveryPlotType()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var sources = Enum.GetValues<PlotType>()
            .Select(plotType => CreateUpdate(plotType.ToString(), plotType, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [SecondSampleValue]))
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
        var firstKey = new PlotSeriesKey("A", DefaultAxisIndex);
        var secondKey = new PlotSeriesKey("B", SecondaryAxisIndex);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(firstKey, PlotType.Scatter, first),
                ReactivePlotSource.FromUpdates(secondKey, PlotType.Scatter, second),
            ],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        first.OnNext(CreateUpdate("A", PlotType.Scatter, DefaultAxisIndex, ReactivePlotUpdateKind.Replace, [FirstSampleValue], [FirstSampleValue]));
        second.OnNext(CreateUpdate("B", PlotType.Scatter, SecondaryAxisIndex, ReactivePlotUpdateKind.Replace, [SecondSampleValue], [SecondSampleValue]));
        first.OnNext(CreateUpdate("A", PlotType.Scatter, DefaultAxisIndex, ReactivePlotUpdateKind.Replace, [ThirdSampleValue], [ThirdSampleValue]));

        await Assert.That(factory.CreatedAdapters).IsEqualTo(ExpectedPairItemCount);
        await Assert.That(factory.Find(firstKey).Updates).Count().IsEqualTo(ExpectedPairItemCount);
        await Assert.That(factory.Find(secondKey).Updates).Count().IsEqualTo(ExpectedSingleItemCount);
    }

    /// <summary>Verifies Bind ClearUpdateClearsOnlyTargetSeries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ClearUpdateClearsOnlyTargetSeries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var firstKey = new PlotSeriesKey("A", DefaultAxisIndex);
        var secondKey = new PlotSeriesKey("B", SecondaryAxisIndex);
        var updates = new[]
        {
            CreateUpdate("A", PlotType.SignalXY, DefaultAxisIndex, ReactivePlotUpdateKind.Replace, [FirstSampleValue], [FirstSampleValue]),
            CreateUpdate("B", PlotType.SignalXY, SecondaryAxisIndex, ReactivePlotUpdateKind.Replace, [SecondSampleValue], [SecondSampleValue]),
            CreateUpdate("A", PlotType.SignalXY, DefaultAxisIndex, ReactivePlotUpdateKind.Clear, [], []),
        };

        using var connection = new ReactivePlotBinder(factory).Bind(
            updates.Select(x => ReactivePlotSource.FromUpdates(x.Key, x.PlotType, Observable.Return(x))),
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(factory.Find(firstKey).ClearCount).IsEqualTo(ExpectedSingleItemCount);
        await Assert.That(factory.Find(secondKey).ClearCount).IsEqualTo(DefaultAxisIndex);
    }

    /// <summary>Verifies InvalidAxisIndex EmitsValidationErrorBeforeUiMutation.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task InvalidAxisIndex_EmitsValidationErrorBeforeUiMutation()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate("BadAxis", PlotType.Signal, -1, ReactivePlotUpdateKind.Append, [FirstSampleValue], [SecondSampleValue]);
        var errors = new List<Exception>();

        using var connection = new ReactivePlotBinder(factory)
            .Bind([ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))], new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var subscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(factory.Adapters).IsEmpty();
        await Assert.That(errors).Count().IsEqualTo(ExpectedSingleItemCount);
    }

    /// <summary>Verifies NullOrEmptySeriesData IsIgnoredOrFaultedAccordingToOptions.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NullOrEmptySeriesData_IsIgnoredOrFaultedAccordingToOptions()
    {
        var bad = CreateUpdate("Bad", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [], []);
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
        var bad = CreateUpdate("Faulted", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [], []);
        var good = CreateUpdate("Faulted", PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Append, [FirstSampleValue], [SecondSampleValue]);

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
