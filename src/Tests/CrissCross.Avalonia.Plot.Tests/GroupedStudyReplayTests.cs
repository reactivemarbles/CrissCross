// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.Plot;
#else
using CrissCross.Avalonia.Plot;
#endif

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies grouped studies replay synchronous sources and share live subscriptions.</summary>
public sealed class GroupedStudyReplayTests
{
    /// <summary>The static history length used by default study periods.</summary>
    private const int HistoryLength = 100;

    /// <summary>Verifies every grouped indicator receives a synchronous static source.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task StaticSource_AllGroupedStudies_ReceiveSnapshot()
    {
        double[] x = new double[HistoryLength];
        double[] y = new double[HistoryLength];
        for (var index = 0; index < x.Length; index++)
        {
            x[index] = index;
            y[index] = index + 1;
        }

        var source = ReactivePlotSource.FromStatic(PlotSeriesData.Numeric("price", 0, x, y), PlotType.Line);
        var groups = new[]
        {
            ReactivePlotStudies.BollingerBands(source),
            ReactivePlotStudies.Ichimoku(source),
            ReactivePlotStudies.MovingAverageConvergenceDivergence(source),
        };
        foreach (var group in groups)
        {
            foreach (var study in group)
            {
                List<ReactivePlotUpdate> updates = [];
                using var subscription = study.Updates.Subscribe(updates.Add);
                await Assert.That(updates.Count).IsEqualTo(1);
                await Assert.That(updates[0].Y.Count).IsGreaterThan(0);
            }
        }
    }

    /// <summary>Verifies replay sharing retains a single upstream subscription and disconnects the last observer.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task LiveSource_GroupedStudies_ShareAndDisconnectUpstream()
    {
        using var input = new Signal<ReactivePlotUpdate>();
        var counters = new SubscriptionCounters();
        var tracked = Observable.Create<ReactivePlotUpdate>(observer =>
        {
            counters.Subscriptions++;
            var subscription = input.Subscribe(observer);
            return Disposable.Create((subscription, counters), static state =>
            {
                state.subscription.Dispose();
                state.counters.Disconnections++;
            });
        });
        var key = new PlotSeriesKey(nameof(tracked), 0);
        var source = ReactivePlotSource.FromUpdates(key, PlotType.Line, tracked);
        var group = ReactivePlotStudies.BollingerBands(source);
        var emissions = 0;
        using (var connections = new CompositeDisposable())
        {
            foreach (var study in group)
            {
                connections.Add(study.Updates.Subscribe(_ => emissions++));
            }

            input.OnNext(new(key, PlotType.Line, ReactivePlotUpdateKind.Replace, [1D], [1D], PlotXAxisKind.Numeric, 1));
            await Assert.That(counters.Subscriptions).IsEqualTo(1);
            await Assert.That(counters.Disconnections).IsEqualTo(0);
            await Assert.That(emissions).IsEqualTo(group.Count);
        }

        await Assert.That(counters.Disconnections).IsEqualTo(1);
    }

    /// <summary>Tracks upstream subscription lifetime.</summary>
    private sealed class SubscriptionCounters
    {
        /// <summary>Gets or sets the upstream subscription count.</summary>
        public int Subscriptions { get; set; }

        /// <summary>Gets or sets the upstream disconnection count.</summary>
        public int Disconnections { get; set; }
    }
}
