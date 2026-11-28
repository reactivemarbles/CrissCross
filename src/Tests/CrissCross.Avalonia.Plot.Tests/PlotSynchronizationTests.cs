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

/// <summary>Verifies streaming mutations and rendering share the same synchronization boundary.</summary>
[NotInParallel]
public sealed class PlotSynchronizationTests
{
    /// <summary>The render width.</summary>
    private const int ImageWidth = 640;

    /// <summary>The render height.</summary>
    private const int ImageHeight = 360;

    /// <summary>The number of concurrent updates and renders.</summary>
    private const int IterationCount = 64;

    /// <summary>The replacement update sequence.</summary>
    private const long ReplacementSequence = 2;

    /// <summary>Verifies initialization owns the render lock and cursor replacement renders with valid axes.</summary>
    /// <param name="type">The series type.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments(PlotType.Signal)]
    [Arguments(PlotType.Scatter)]
    [Arguments(PlotType.Streamer)]
    [Arguments(PlotType.DataLogger)]
    [Arguments(PlotType.SignalXY)]
    public async Task Bind_InitializationAndCursorReplacement_OwnRenderLock(PlotType type)
    {
        var result = await PlotTestApp.Run(() => ObserveLifecycle(type));
        await Assert.That(result.allLocked).IsTrue();
        await Assert.That(result.initializationNotifications).IsGreaterThan(0);
        await Assert.That(result.axesInitialized).IsTrue();
        await Assert.That(result.RemainingSeries).IsEqualTo(0);
        var expectedCursors = type is PlotType.Signal or PlotType.Streamer or PlotType.SignalXY ? 1 : 0;
        await Assert.That(result.cursorCount).IsEqualTo(expectedCursors);
    }

    /// <summary>Verifies new overlays and deferred data replacement cannot corrupt concurrent rendering.</summary>
    /// <param name="type">The series type.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments(PlotType.Signal)]
    [Arguments(PlotType.Scatter)]
    [Arguments(PlotType.Streamer)]
    [Arguments(PlotType.DataLogger)]
    public async Task Bind_ConcurrentRendering_CompletesEveryRender(PlotType type)
    {
        var rendered = await PlotTestApp.RunAsync(async () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            var plot = vm.AvaPlot1vm!.Plot;
            using var updates = new Signal<ReactivePlotUpdate>();
            var key = new PlotSeriesKey("concurrent", 0);
            using var connection = new ReactivePlotBinder().Bind(
                vm,
                [ReactivePlotSource.FromUpdates(key, type, updates)],
                new() { UiScheduler = ImmediateScheduler.Instance });
            var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var rendering = Task.Run(() =>
            {
                started.SetResult();
                for (var i = 0; i < IterationCount; i++)
                {
                    using var image = plot.GetImage(ImageWidth, ImageHeight);
                }

                return IterationCount;
            });
            await started.Task;
            var axisKind = PlotXAxisKind.Numeric;
            for (var i = 0; i < IterationCount; i++)
            {
                updates.OnNext(new(key, type, ReactivePlotUpdateKind.Replace, [(double)i], [1D], axisKind, i));
                axisKind = axisKind == PlotXAxisKind.Numeric ? PlotXAxisKind.OADate : PlotXAxisKind.Numeric;
            }

            return await rendering;
        });
        await Assert.That(rendered).IsEqualTo(IterationCount);
    }

    /// <summary>Observes lock ownership through a complete series lifecycle.</summary>
    /// <param name="type">The series type.</param>
    /// <returns>The observed synchronization and rendering state.</returns>
    private static (bool allLocked, int initializationNotifications, bool axesInitialized, int cursorCount, int RemainingSeries) ObserveLifecycle(PlotType type)
    {
        using var vm = new LiveChartViewModel(new Grid());
        var plot = vm.AvaPlot1vm!.Plot;
        var allLocked = true;
        var initializationNotifications = 0;
        using var updates = new Signal<ReactivePlotUpdate>();
        var key = new PlotSeriesKey("synchronized", 0);
        bool axesInitialized;
        var cursorCount = 0;
        using (var connection = new ReactivePlotBinder().Bind(
            vm,
            [ReactivePlotSource.FromUpdates(key, type, updates)],
            new() { UiScheduler = ImmediateScheduler.Instance }))
        {
            updates.OnNext(new(key, type, ReactivePlotUpdateKind.Clear, [], [], PlotXAxisKind.Numeric, 0));
            vm.SeriesSettings[0].PropertyChanged += (_, _) =>
            {
                allLocked &= Monitor.IsEntered(plot.Sync);
                initializationNotifications++;
            };
            updates.OnNext(new(key, type, ReactivePlotUpdateKind.Append, [0D], [1D], PlotXAxisKind.Numeric, 1) { Style = new() { Color = "Blue" } });
            updates.OnNext(new(key, type, ReactivePlotUpdateKind.Replace, [1D], [1D], PlotXAxisKind.OADate, ReplacementSequence));
            using var image = plot.GetImage(ImageWidth, ImageHeight);
            axesInitialized = plot.PlottableList.TrueForAll(static item => item.Axes.XAxis is not null && item.Axes.YAxis is not null);
            cursorCount = CountCrosshairs(plot);
        }

        return (allLocked, initializationNotifications, axesInitialized, cursorCount, vm.SeriesSettings.Count);
    }

    /// <summary>Counts cursor overlays without allocating an iterator.</summary>
    /// <param name="plot">The destination plot.</param>
    /// <returns>The cursor overlay count.</returns>
    private static int CountCrosshairs(CrissCross.Plot.Plot plot)
    {
        var count = 0;
        foreach (var item in plot.PlottableList)
        {
            if (item is CrissCross.Plot.Plottables.Crosshair)
            {
                count++;
            }
        }

        return count;
    }
}
