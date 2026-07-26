// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#else
using CrissCross.WPF.Plot;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Tests for IObservable-first WPF plot source adapters and binder lifecycle behavior.</summary>
public sealed partial class ReactivePlotBinderTests
{
    /// <summary>Verifies Bind MarshalsAdapterMutationToConfiguredUiScheduler.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_MarshalsAdapterMutationToConfiguredUiScheduler()
    {
        var scheduler = new ManualPumpScheduler();
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate(
            "Scheduled",
            PlotType.SignalXY,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Replace,
            [FirstSampleValue],
            [SecondSampleValue]);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))],
            new() { UiScheduler = scheduler });

        await Assert.That(factory.Adapters).IsEmpty();
        scheduler.RunAll();
        await Assert.That(GetSingle(GetSingle(factory.Adapters).Updates)).IsEqualTo(update);
    }

    /// <summary>Verifies Bind LiveSourceEmitsImmediatelyWhenNoBatchWindowIsConfigured.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_LiveSourceEmitsImmediatelyWhenNoBatchWindowIsConfigured()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var updates = new Signal<ReactivePlotUpdate>();
        var update = CreateUpdate(
            "Live",
            PlotType.Signal,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Append,
            [FirstSampleValue],
            [SecondSampleValue]);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, updates)],
            new() { UiScheduler = ImmediateScheduler.Instance });
        updates.OnNext(update);

        await Assert.That(GetSingle(GetSingle(factory.Adapters).Updates)).IsEqualTo(update);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Active);
    }

    /// <summary>Verifies Bind BatchesHighFrequencyUpdatesBeforeUiScheduler.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchesHighFrequencyUpdatesBeforeUiScheduler()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var scheduler = new ManualPumpScheduler();
        var updates = CreateSequentialSignalUpdates("Fast", HighFrequencyUpdateCount);
        PlotSeriesKey key = new("Fast", DefaultAxisIndex);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(
                    key,
                    PlotType.Signal,
                    updates.ToObservable()),],
            new() { UiScheduler = scheduler, BatchWindow = TimeSpan.FromMilliseconds(BatchWindowMilliseconds), MaxBatchSize = HighFrequencyMaxBatchSize });
        scheduler.RunAll();

        var adapter = GetSingle(factory.Adapters);
        await Assert.That(adapter.ApplyCallCount).IsLessThan(HighFrequencyUpdateCount);
        await Assert.That(adapter.Updates[^1].Sequence).IsEqualTo(LastHighFrequencySequence);
    }

    /// <summary>Verifies Bind DropOldestOverflowKeepsLatestVisiblePoints.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_DropOldestOverflowKeepsLatestVisiblePoints()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var updates = CreateSequentialSignalUpdates("Bounded", OverflowUpdateCount);
        PlotSeriesKey key = new("Bounded", DefaultAxisIndex);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(
                    key,
                    PlotType.Signal,
                    updates.ToObservable()),],
            new() { UiScheduler = ImmediateScheduler.Instance, MaxVisiblePoints = VisiblePointLimit });

        var retained = GetSingle(factory.Adapters).Updates[^1];
        await Assert.That(retained.X[DefaultAxisIndex]).IsEqualTo(FirstRetainedVisiblePoint);
        await Assert.That(retained.Y[retained.Y.Count - 1]).IsEqualTo(LastRetainedVisiblePoint);
        await Assert.That(retained.X.Count).IsEqualTo(VisiblePointLimit);
    }

    /// <summary>Verifies DataLoggerAdapter PreservesLegacyMaxPointCount.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataLoggerAdapter_PreservesLegacyMaxPointCount()
    {
        IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> points = Observable.Return(
            (
                Name: (string?)"Logger",
                Value: (IList<double>?)[LoggerFirstValue, LoggerSecondValue],
                Axis: DefaultAxisIndex,
                nMaxPoints: LegacyMaxPointCount));
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
        ReactivePlotUpdate[] updates =
        [
            CreateUpdate(
                "A",
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [FirstSampleValue],
                [FirstNumericXValue]),
            CreateUpdate(
                "B",
                PlotType.Signal,
                SecondaryAxisIndex,
                ReactivePlotUpdateKind.Append,
                [SecondSampleValue],
                [SecondNumericXValue]),
        ];

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(firstKey, PlotType.Signal, updates.ToObservable())],
            new() { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = MixedSeriesBatchSize });

        await Assert.That(GetSingle(factory.Find(firstKey).Updates).Y).IsEquivalentTo([FirstNumericXValue]);
        await Assert.That(GetSingle(factory.Find(secondKey).Updates).Y).IsEquivalentTo([SecondNumericXValue]);
    }

    /// <summary>Verifies Bind BatchingPreservesReplaceAndClearBoundaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingPreservesReplaceAndClearBoundaries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey(WindowSeriesName, DefaultAxisIndex);
        ReactivePlotUpdate[] updates =
        [
            CreateUpdate(
                WindowSeriesName,
                PlotType.Scatter,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [FirstSampleValue],
                [FirstSampleValue]),
            CreateUpdate(WindowSeriesName, PlotType.Scatter, DefaultAxisIndex, ReactivePlotUpdateKind.Clear, [], []),
            CreateUpdate(
                WindowSeriesName,
                PlotType.Scatter,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Replace,
                [SecondSampleValue],
                [SecondSampleValue]),
        ];

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Scatter, updates.ToObservable())],
            new() { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = ReplaceClearBatchSize });

        await Assert
            .That(GetUpdateKinds(factory.Find(key).Updates))
            .IsEquivalentTo([
                ReactivePlotUpdateKind.Append,
                ReactivePlotUpdateKind.Clear,
                ReactivePlotUpdateKind.Replace,]);
    }

    /// <summary>Verifies Bind BatchingDoesNotHideInvalidAppendUpdates.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_BatchingDoesNotHideInvalidAppendUpdates()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey(InvalidBatchSeriesName, DefaultAxisIndex);
        ReactivePlotUpdate[] updates =
        [
            CreateUpdate(
                InvalidBatchSeriesName,
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [],
                []),
            CreateUpdate(
                InvalidBatchSeriesName,
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [FirstSampleValue],
                [FirstSampleValue]),
        ];

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates.ToObservable())],
            new() { UiScheduler = ImmediateScheduler.Instance, MaxBatchSize = MixedSeriesBatchSize });

        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters).IsEmpty();
    }

    /// <summary>Verifies Bind ClearResetsPerUpdateRetainedPoints.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_ClearResetsPerUpdateRetainedPoints()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var key = new PlotSeriesKey(RetainedSeriesName, DefaultAxisIndex);
        ReactivePlotUpdate[] updates =
        [
            CreateUpdate(
                RetainedSeriesName,
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [FirstSampleValue],
                [FirstSampleValue]) with
            {
                MaxPoints = RetainedSeriesMaxPoints,
            },
            CreateUpdate(RetainedSeriesName, PlotType.Signal, DefaultAxisIndex, ReactivePlotUpdateKind.Clear, [], []),
            CreateUpdate(
                RetainedSeriesName,
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [SecondSampleValue],
                [SecondSampleValue]) with
            {
                MaxPoints = RetainedSeriesMaxPoints,
            },
        ];

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates.ToObservable())],
            new() { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(factory.Find(key).Updates[^1].Y).IsEquivalentTo([SecondSampleValue]);
    }
}
