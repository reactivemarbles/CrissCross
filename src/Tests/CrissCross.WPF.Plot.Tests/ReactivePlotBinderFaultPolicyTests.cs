// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Tests for IObservable-first WPF plot source adapters and binder lifecycle behavior.</summary>
public sealed partial class ReactivePlotBinderTests
{
    /// <summary>Verifies SurfaceAndStopSeries StopsOnlyFaultedSeries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SurfaceAndStopSeries_StopsOnlyFaultedSeries()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var first = new Signal<ReactivePlotUpdate>();
        var second = new Signal<ReactivePlotUpdate>();
        var firstKey = new PlotSeriesKey(FaultedSeriesName, DefaultAxisIndex);
        var secondKey = new PlotSeriesKey("Healthy", SecondaryAxisIndex);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(firstKey, PlotType.Signal, first),
                ReactivePlotSource.FromUpdates(secondKey, PlotType.Signal, second),],
            new ReactivePlotBindingOptions
            {
                UiScheduler = ImmediateScheduler.Instance,
                ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries,
            });

        first.OnNext(
            CreateUpdate(
                FaultedSeriesName,
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [],
                []));
        first.OnNext(
            CreateUpdate(
                FaultedSeriesName,
                PlotType.Signal,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Append,
                [FirstSampleValue],
                [FirstSampleValue]));
        second.OnNext(
            CreateUpdate(
                "Healthy",
                PlotType.Signal,
                SecondaryAxisIndex,
                ReactivePlotUpdateKind.Append,
                [SecondSampleValue],
                [SecondSampleValue]));

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
            .Select(plotType =>
                CreateUpdate(
                    plotType.ToString(),
                    plotType,
                    DefaultAxisIndex,
                    ReactivePlotUpdateKind.Append,
                    [FirstSampleValue],
                    [SecondSampleValue]))
            .Select(update => ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update)))
            .ToArray();

        using var connection = new ReactivePlotBinder(factory).Bind(
            sources,
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert
            .That(factory.Adapters.Select(x => x.PlotType).OrderBy(x => x.ToString()).ToArray())
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
                ReactivePlotSource.FromUpdates(secondKey, PlotType.Scatter, second),],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        first.OnNext(
            CreateUpdate(
                "A",
                PlotType.Scatter,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Replace,
                [FirstSampleValue],
                [FirstSampleValue]));
        second.OnNext(
            CreateUpdate(
                "B",
                PlotType.Scatter,
                SecondaryAxisIndex,
                ReactivePlotUpdateKind.Replace,
                [SecondSampleValue],
                [SecondSampleValue]));
        first.OnNext(
            CreateUpdate(
                "A",
                PlotType.Scatter,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Replace,
                [ThirdSampleValue],
                [ThirdSampleValue]));

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
            CreateUpdate(
                "A",
                PlotType.SignalXY,
                DefaultAxisIndex,
                ReactivePlotUpdateKind.Replace,
                [FirstSampleValue],
                [FirstSampleValue]),
            CreateUpdate(
                "B",
                PlotType.SignalXY,
                SecondaryAxisIndex,
                ReactivePlotUpdateKind.Replace,
                [SecondSampleValue],
                [SecondSampleValue]),
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
        var update = CreateUpdate(
            "BadAxis",
            PlotType.Signal,
            -1,
            ReactivePlotUpdateKind.Append,
            [FirstSampleValue],
            [SecondSampleValue]);
        var errors = new List<Exception>();

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
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
            new ReactivePlotBindingOptions
            {
                UiScheduler = ImmediateScheduler.Instance,
                ErrorMode = ReactivePlotErrorMode.IgnoreInvalidUpdates,
            });
        using var faulted = new ReactivePlotBinder(faultedFactory).Bind(
            [ReactivePlotSource.FromUpdates(bad.Key, bad.PlotType, Observable.Return(bad))],
            new ReactivePlotBindingOptions
            {
                UiScheduler = ImmediateScheduler.Instance,
                ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries,
            });

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
        var bad = CreateUpdate(
            FaultedSeriesName,
            PlotType.Signal,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Append,
            [],
            []);
        var good = CreateUpdate(
            FaultedSeriesName,
            PlotType.Signal,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Append,
            [FirstSampleValue],
            [SecondSampleValue]);

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(bad.Key, bad.PlotType, updates)],
            new ReactivePlotBindingOptions
            {
                UiScheduler = ImmediateScheduler.Instance,
                ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries,
            });
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
        long sequence = 0) => new(new PlotSeriesKey(name, axis), plotType, kind, x, y, PlotXAxisKind.Numeric, sequence);
}
