// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Tests for IObservable-first WPF plot source adapters and binder lifecycle behavior.</summary>
public sealed partial class ReactivePlotBinderTests
{
    /// <summary>The motor series name.</summary>
    private const string MotorSeriesName = "Motor";

    /// <summary>The batching window series name.</summary>
    private const string WindowSeriesName = "Window";

    /// <summary>The invalid batch series name.</summary>
    private const string InvalidBatchSeriesName = "InvalidBatch";

    /// <summary>The retained-points series name.</summary>
    private const string RetainedSeriesName = "Retained";

    /// <summary>The faulted series name.</summary>
    private const string FaultedSeriesName = "Faulted";

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
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> ticks = Observable.Return(
            (
                Name: (string?)MotorSeriesName,
                Value: (IList<double>?)[FirstSampleValue, SecondSampleValue],
                DateTime: (IList<double>)[FirstTickValue, SecondTickValue],
                Axis: MotorAxisIndex));
        var source = ReactivePlotSource.FromSignalTicks(ticks);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.XAxisKind).IsEqualTo(PlotXAxisKind.Ticks);
        await Assert.That(update.Key).IsEqualTo(new PlotSeriesKey(MotorSeriesName, MotorAxisIndex));
        await Assert.That(update.X.Count).IsEqualTo(ExpectedPairItemCount);
        await Assert.That(update.Y.Count).IsEqualTo(ExpectedPairItemCount);
    }

    /// <summary>Verifies SignalPointsAdapter EmitsAppendUpdateWithNumericXAxisKind.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SignalPointsAdapter_EmitsAppendUpdateWithNumericXAxisKind()
    {
        IObservable<(string? Name, IList<double>? Value, IList<double> X, int Axis)> points = Observable.Return(
            (
                Name: (string?)MotorSeriesName,
                Value: (IList<double>?)[FirstSampleValue, SecondSampleValue],
                X: (IList<double>)[FirstNumericXValue, SecondNumericXValue],
                Axis: MotorAxisIndex));
        var source = ReactivePlotSource.FromSignalPoints(points);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.PlotType).IsEqualTo(PlotType.Signal);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.XAxisKind).IsEqualTo(PlotXAxisKind.Numeric);
        await Assert.That(((ReactivePlotSource)source).XAxisKind).IsEqualTo(PlotXAxisKind.Numeric);
        await Assert.That(update.Key).IsEqualTo(new PlotSeriesKey(MotorSeriesName, MotorAxisIndex));
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
        snapshots.OnNext(
            (
                "Vector",
                [FirstSampleValue, SecondSampleValue],
                [FirstNumericXValue, SecondNumericXValue],
                SecondaryAxisIndex));
        snapshots.OnNext(
            (
                "Vector",
                [ThirdSampleValue, FourthSampleValue],
                [ThirdNumericXValue, FourthNumericXValue],
                SecondaryAxisIndex));

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
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> scatter = Observable.Return(
            (
                Name: (string?)"Scatter",
                X: (IList<double>?)[FirstSampleValue],
                Y: (IList<double>)[FirstSampleValue, SecondSampleValue],
                Axis: DefaultAxisIndex));
        var source = ReactivePlotSource.FromScatterPoints(scatter);
        var errors = new List<Exception>();

        using var connection = binder.Bind(
            [source],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
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

        using var connection = binder.Bind(
            [source],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
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
        var update = CreateUpdate(
            "Cold",
            PlotType.Signal,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Append,
            [FirstSampleValue],
            [SecondSampleValue]);
        var source = new ReactivePlotSource(
            update.Key,
            update.PlotType,
            Observable.Create<ReactivePlotUpdate>(observer =>
            {
                subscriptions++;
                observer.OnNext(update);
                return EmptyDisposable.Instance;
            }));

        using var connection = new ReactivePlotBinder(new RecordingReactivePlotAdapterFactory()).Bind(
            [source],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });

        await Assert.That(subscriptions).IsEqualTo(ExpectedSingleItemCount);
    }

    /// <summary>Verifies Bind CompletionEmitsCompletedStateWithoutDisposingAdapters.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Bind_CompletionEmitsCompletedStateWithoutDisposingAdapters()
    {
        var factory = new RecordingReactivePlotAdapterFactory();
        var update = CreateUpdate(
            "Finite",
            PlotType.DataLogger,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Append,
            [0.0],
            [CompletedSampleValue]);
        var states = new List<ReactivePlotConnectionState>();

        using var connection = new ReactivePlotBinder(factory).Bind(
            [ReactivePlotSource.FromUpdates(update.Key, update.PlotType, Observable.Return(update))],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
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
        var update = CreateUpdate(
            "Fault",
            PlotType.Streamer,
            DefaultAxisIndex,
            ReactivePlotUpdateKind.Append,
            [0.0],
            [FirstSampleValue]);
        var errors = new List<Exception>();

        using var connection = new ReactivePlotBinder(factory).Bind(
            [
                ReactivePlotSource.FromUpdates(
                    update.Key,
                    update.PlotType,
                    Observable.Concat(Observable.Return(update), Observable.Throw<ReactivePlotUpdate>(expected))),],
            new ReactivePlotBindingOptions { UiScheduler = ImmediateScheduler.Instance });
        using var errorSubscription = connection.Errors.Subscribe(errors.Add);

        await Assert.That(errors.Single()).IsSameReferenceAs(expected);
        await Assert.That(connection.CurrentState).IsEqualTo(ReactivePlotConnectionState.Faulted);
        await Assert.That(factory.Adapters.Single().Updates).Count().IsEqualTo(ExpectedSingleItemCount);
    }
}
