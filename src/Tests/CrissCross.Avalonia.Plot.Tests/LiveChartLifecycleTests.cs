// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.Plot;
#else
using CrissCross.Avalonia.Plot;
#endif

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the public Avalonia control binds sources and releases subscriptions.</summary>
[NotInParallel]
public sealed class LiveChartLifecycleTests
{
    /// <summary>The WindowHeight test value.</summary>
    private const int WindowHeight = 480;

    /// <summary>The IgnoredValue test value.</summary>
    private const double IgnoredValue = 99D;

    /// <summary>The WindowWidth test value.</summary>
    private const int WindowWidth = 640;

    /// <summary>The SecondValue test value.</summary>
    private const double SecondValue = 2D;

    /// <summary>The ThirdValue test value.</summary>
    private const double ThirdValue = 3D;

    /// <summary>The SecondSequence test value.</summary>
    private const int SecondSequence = 2;

    /// <summary>Verifies swapping and clearing sources removes obsolete series.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Sources_SwapAndClear_ReleaseSeries()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            using var vm = chart.ViewModel!;
            using var first = new Signal<ReactivePlotUpdate>();
            using var second = new Signal<ReactivePlotUpdate>();
            var firstKey = new PlotSeriesKey(nameof(first), 0);
            var secondKey = new PlotSeriesKey(nameof(second), 0);
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(firstKey, PlotType.Line, first)];
            first.OnNext(new(firstKey, PlotType.Line, ReactivePlotUpdateKind.Replace, [1D], [SecondValue], PlotXAxisKind.Numeric, 1));
            var firstName = vm.SeriesSettings[0].Key.Name;
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(secondKey, PlotType.Points, second)];
            second.OnNext(new(secondKey, PlotType.Points, ReactivePlotUpdateKind.Replace, [1D], [ThirdValue], PlotXAxisKind.Numeric, 1));
            first.OnNext(new(firstKey, PlotType.Line, ReactivePlotUpdateKind.Append, [SecondValue], [IgnoredValue], PlotXAxisKind.Numeric, SecondSequence));
            var secondName = vm.SeriesSettings[0].Key.Name;
            var countAfterSwap = vm.SeriesSettings.Count;
            chart.ReactivePlotSources = null;
            second.OnNext(new(secondKey, PlotType.Points, ReactivePlotUpdateKind.Append, [SecondValue], [IgnoredValue], PlotXAxisKind.Numeric, SecondSequence));
            return (firstName, secondName, countAfterSwap, CountAfterClear: vm.SeriesSettings.Count);
        });
        await Assert.That(observed.firstName).IsEqualTo("first");
        await Assert.That(observed.secondName).IsEqualTo("second");
        await Assert.That(observed.countAfterSwap).IsEqualTo(1);
        await Assert.That(observed.CountAfterClear).IsEqualTo(0);
    }

    /// <summary>Verifies a chart can attach to and detach from a real Avalonia visual tree.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Window_ShowAndClose_ReleasesSources()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            using var vm = chart.ViewModel!;
            using var updates = new Signal<ReactivePlotUpdate>();
            var key = new PlotSeriesKey("window", 0);
            var window = new Window { Content = chart, Width = WindowWidth, Height = WindowHeight };
            window.Show();
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(key, PlotType.Line, updates)];
            updates.OnNext(new(key, PlotType.Line, ReactivePlotUpdateKind.Replace, [1D, SecondValue], [SecondValue, ThirdValue], PlotXAxisKind.Numeric, 1));
            var active = vm.SeriesSettings.Count;
            window.Close();
            updates.OnNext(new(key, PlotType.Line, ReactivePlotUpdateKind.Append, [ThirdValue], [IgnoredValue], PlotXAxisKind.Numeric, SecondSequence));
            return (active, Remaining: vm.SeriesSettings.Count);
        });
        await Assert.That(observed.active).IsEqualTo(1);
        await Assert.That(observed.Remaining).IsEqualTo(0);
    }

    /// <summary>Verifies reattaching the same chart reconnects a replayable source.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Window_DetachAndReattach_ReplaysSources()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            using var vm = chart.ViewModel!;
            var window = new Window { Content = chart };
            window.Show();
            chart.ReactivePlotSources = [ReactivePlotSource.FromStatic(PlotSeriesData.Numeric("replay", 0, [1D], [1D]), PlotType.Line)];
            var initialCount = vm.SeriesSettings.Count;
            window.Content = null;
            var detachedCount = vm.SeriesSettings.Count;
            window.Content = chart;
            var restoredCount = vm.SeriesSettings.Count;
            window.Close();
            return (initialCount, detachedCount, restoredCount);
        });
        await Assert.That(observed.initialCount).IsEqualTo(1);
        await Assert.That(observed.detachedCount).IsEqualTo(0);
        await Assert.That(observed.restoredCount).IsEqualTo(1);
    }
}
