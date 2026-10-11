// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
#if REACTIVE_SHIM
using ReactiveUI.Reactive;
#else
using ReactiveUI;
#endif
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Maui.Plot;
#else
using CrissCross.Maui.Plot;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Exercises actual ScottPlot rendering through the public Maui binding surface.</summary>
[NotInParallel]
public sealed class MauiPlotRenderingTests
{
    /// <summary>The ImageHeight test value.</summary>
    private const int ImageHeight = 360;

    /// <summary>The ImageWidth test value.</summary>
    private const int ImageWidth = 640;

    /// <summary>The MinimumImageBytes test value.</summary>
    private const int MinimumImageBytes = 100;

    /// <summary>The NextResetX test value.</summary>
    private const double NextResetX = 11D;

    /// <summary>The ResetX test value.</summary>
    private const double ResetX = 10D;

    /// <summary>The SecondValue test value.</summary>
    private const double SecondValue = 2D;

    /// <summary>The FourthValue test value.</summary>
    private const double FourthValue = 4D;

    /// <summary>The SixthValue test value.</summary>
    private const double SixthValue = 6D;

    /// <summary>The FifthValue test value.</summary>
    private const double FifthValue = 5D;

    /// <summary>The ThirdValue test value.</summary>
    private const double ThirdValue = 3D;

    /// <summary>The ClearSequence test value.</summary>
    private const int ClearSequence = 2;

    /// <summary>The ResetSequence test value.</summary>
    private const int ResetSequence = 3;

    /// <summary>Verifies each chart type renders, clears, and can accept another update.</summary>
    /// <param name="type">The chart type under test.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments(PlotType.Signal)]
    [Arguments(PlotType.Scatter)]
    [Arguments(PlotType.DataLogger)]
    [Arguments(PlotType.Streamer)]
    [Arguments(PlotType.SignalXY)]
    [Arguments(PlotType.Line)]
    [Arguments(PlotType.StepLine)]
    [Arguments(PlotType.Area)]
    [Arguments(PlotType.Bar)]
    [Arguments(PlotType.Stem)]
    [Arguments(PlotType.Points)]
    public async Task Bind_AllChartTypes_RenderAndReset(PlotType type)
    {
        var result = await PlotTestApp.Run(() =>
        {
            var host = new Grid();
            using var vm = new LiveChartViewModel(host);
            var key = new PlotSeriesKey("render", 0);
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = new ReactivePlotBinder().Bind(
                vm,
                [ReactivePlotSource.FromUpdates(key, type, updates)],
                new() { UiScheduler = ImmediateScheduler.Instance });
            updates.OnNext(new(key, type, ReactivePlotUpdateKind.Replace, [1D, SecondValue, ThirdValue], [SecondValue, FourthValue, ThirdValue], PlotXAxisKind.Numeric, 1));
            var plot = vm.MauiPlot1vm!.Plot;
            plot.Axes.AutoScale();
            var firstImage = plot.GetImage(ImageWidth, ImageHeight).GetImageBytes();
            var seriesCount = vm.SeriesSettings.Count;
            updates.OnNext(new(key, type, ReactivePlotUpdateKind.Clear, [], [], PlotXAxisKind.Numeric, ClearSequence));
            updates.OnNext(new(key, type, ReactivePlotUpdateKind.Replace, [ResetX, NextResetX], [FifthValue, SixthValue], PlotXAxisKind.Numeric, ResetSequence));
            plot.Axes.AutoScale();
            var secondImage = plot.GetImage(ImageWidth, ImageHeight).GetImageBytes();
            return (FirstLength: firstImage.Length, SecondLength: secondImage.Length,
                seriesCount,
                HasHostedControl: host.Children.Contains(vm.MauiPlot1vm));
        });
        await Assert.That(result.FirstLength).IsGreaterThan(MinimumImageBytes);
        await Assert.That(result.SecondLength).IsGreaterThan(MinimumImageBytes);
        await Assert.That(result.seriesCount).IsEqualTo(1);
        await Assert.That(result.HasHostedControl).IsTrue();
    }

    /// <summary>Verifies a scatter replacement completes without a second UI dispatch leaving empty data.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Bind_ScatterReplacement_IsCompleteBeforeDeferredUiWork()
    {
        var count = await PlotTestApp.Run(static () =>
        {
            RxSchedulers.MainThreadScheduler = new ManualPumpScheduler();
            using var vm = new LiveChartViewModel(new Grid());
            var key = new PlotSeriesKey("scatter", 0);
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = new ReactivePlotBinder().Bind(
                vm,
                [ReactivePlotSource.FromUpdates(key, PlotType.Scatter, updates)],
                new() { UiScheduler = ImmediateScheduler.Instance });
            updates.OnNext(new(key, PlotType.Scatter, ReactivePlotUpdateKind.Replace, [1D], [SecondValue], PlotXAxisKind.Numeric, 1));
            updates.OnNext(new(key, PlotType.Scatter, ReactivePlotUpdateKind.Replace, [ResetX, NextResetX], [FifthValue, SixthValue], PlotXAxisKind.Numeric, ResetSequence));
            foreach (var plottable in vm.MauiPlot1vm!.Plot.GetPlottables())
            {
                if (plottable is ScottPlot.Plottables.Scatter scatter)
                {
                    return scatter.Data.GetScatterPoints().Count;
                }
            }

            return 0;
        });
        await Assert.That(count).IsEqualTo(ClearSequence);
    }
}
