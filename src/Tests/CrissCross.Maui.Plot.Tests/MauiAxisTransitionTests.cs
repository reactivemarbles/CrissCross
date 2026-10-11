// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ScottPlot;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Maui.Plot;
#else
using CrissCross.Maui.Plot;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Guards axis geometry while a replacement live source waits for its first update.</summary>
[NotInParallel]
public sealed class MauiAxisTransitionTests
{
    /// <summary>The test image width.</summary>
    private const int ImageWidth = 640;

    /// <summary>The test image height.</summary>
    private const int ImageHeight = 360;

    /// <summary>The previous horizontal lower limit.</summary>
    private const double PreviousMinimum = 12;

    /// <summary>The previous horizontal upper limit.</summary>
    private const double PreviousMaximum = 24;

    /// <summary>The first live horizontal value.</summary>
    private const double FirstLiveX = 100;

    /// <summary>The second live horizontal value.</summary>
    private const double SecondLiveX = 200;

    /// <summary>Verifies numeric axis replacement preserves finite limits immediately, before rendering.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task NumericAxisReplacement_PreservesFiniteLimitsAndBindings()
    {
        var result = await PlotTestApp.Run(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            var plot = vm.MauiPlot1vm!.Plot;
            plot.Axes.SetLimitsX(PreviousMinimum, PreviousMaximum);
            var logger = plot.Add.DataLogger();
            logger.ManageAxisLimits = false;
            logger.Axes.XAxis = vm.XAxis1;
            vm.CreateAxisWithPoints();
            vm.ApplyTheme(ReactivePlotTheme.Dark);
            vm.UseFixedNumberOfPoints = true;
            return (vm.XAxis1.Min, vm.XAxis1.Max,
                GridUpdated: ReferenceEquals(plot.Axes.DefaultGrid.XAxis, vm.XAxis1),
                SeriesUpdated: ReferenceEquals(logger.Axes.XAxis, vm.XAxis1));
        });
        await Assert.That(result.Min).IsEqualTo(PreviousMinimum);
        await Assert.That(result.Max).IsEqualTo(PreviousMaximum);
        await Assert.That(result.GridUpdated).IsTrue();
        await Assert.That(result.SeriesUpdated).IsTrue();
    }

    /// <summary>Verifies an unset replacement axis has usable numeric limits before rendering.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task NumericAxisReplacement_UnsetRange_IsFiniteImmediately()
    {
        var finite = await PlotTestApp.Run(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            vm.XAxis1.Range.Reset();
            vm.CreateAxisWithPoints();
            return IsFinite(vm.XAxis1);
        });
        await Assert.That(finite).IsTrue();
    }

    /// <summary>Verifies empty continuous autoscaling retains limits and later follows live data.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ContinuousAutoscale_EmptyLoggerRetainsLimits_ThenScalesLiveData()
    {
        var result = await PlotTestApp.Run(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            vm.CreateAxisWithPoints();
            var plot = vm.MauiPlot1vm!.Plot;
            plot.Axes.SetLimits(PreviousMinimum, PreviousMaximum, -1, 1);
            var logger = plot.Add.DataLogger();
            logger.ManageAxisLimits = false;
            logger.Axes.XAxis = vm.XAxis1;
            logger.Axes.YAxis = vm.YAxisList[0];
            plot.Axes.SetLimitsY(-1, 1, vm.YAxisList[0]);
            plot.Axes.ContinuouslyAutoscale = true;
            plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleAll();
            _ = plot.GetImage(ImageWidth, ImageHeight);
            var emptyMin = vm.XAxis1.Min;
            var emptyMax = vm.XAxis1.Max;
            var emptyYFinite = IsFinite(vm.YAxisList[0]);
            logger.Add(FirstLiveX, 0);
            logger.Add(SecondLiveX, 1);
            _ = plot.GetImage(ImageWidth, ImageHeight);
            return (emptyMin, emptyMax, emptyYFinite,
                LiveMin: vm.XAxis1.Min, LiveMax: vm.XAxis1.Max,
                LiveFinite: IsFinite(vm.XAxis1));
        });
        await Assert.That(result.emptyMin).IsEqualTo(PreviousMinimum);
        await Assert.That(result.emptyMax).IsEqualTo(PreviousMaximum);
        await Assert.That(result.emptyYFinite).IsTrue();
        await Assert.That(result.LiveFinite).IsTrue();
        await Assert.That(result.LiveMin).IsLessThan(FirstLiveX);
        await Assert.That(result.LiveMax).IsGreaterThan(SecondLiveX);
    }

    /// <summary>Verifies date axes start with a practical finite window while awaiting data.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task DateAxisReplacement_EmptyPlot_UsesFiniteDayWindow()
    {
        var span = await PlotTestApp.Run(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            vm.CreateAxisWithPoints();
            vm.CreateAxisWithTimeStamp();
            return vm.XAxis1.Max - vm.XAxis1.Min;
        });
        await Assert.That(span).IsEqualTo(1);
    }

    /// <summary>Checks finite, nonzero axis geometry.</summary>
    /// <param name="axis">The axis.</param>
    /// <returns>Whether its range is usable.</returns>
    private static bool IsFinite(IAxis axis) => double.IsFinite(axis.Min) && double.IsFinite(axis.Max) && Math.Abs(axis.Max - axis.Min) > 0;
}
