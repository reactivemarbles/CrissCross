// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.Plot;
#else
using CrissCross.Avalonia.Plot;
#endif

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Exercises native Avalonia pointer routing for plot markers and crosshair dragging.</summary>
[NotInParallel]
public sealed class AvaloniaPointerInteractionTests
{
    /// <summary>The tested axis maximum.</summary>
    private const double AxisMaximum = 4;

    /// <summary>The dragged crosshair coordinate.</summary>
    private const double DragCoordinate = 2;

    /// <summary>Verifies native Ctrl-click, hover tooltip, and crosshair dragging.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task NativePointer_AddsMarkerAndDragsCrosshair()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            using var vm = chart.ViewModel!;
            var window = new Window { Content = chart };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            var control = vm.AvaPlot1vm!;
            var plot = control.Plot;
            plot.Axes.ContinuouslyAutoscale = false;
            plot.Axes.SetLimitsX(0, AxisMaximum, vm.XAxis1);
            plot.Axes.SetLimitsY(0, AxisMaximum, vm.YAxisList[0]);
            vm.AddCrosshair();
            var crosshair = vm.CrosshairCollection[0].PlotLine!;
            crosshair.X = 1;
            crosshair.Y = 1;
            _ = plot.GetImage((int)control.Bounds.Width, (int)control.Bounds.Height);
            var startPixel = plot.GetPixel(new(1, DragCoordinate), vm.XAxis1, vm.YAxisList[0]);
            var endPixel = plot.GetPixel(new(DragCoordinate, DragCoordinate), vm.XAxis1, vm.YAxisList[0]);
            var start = control.TranslatePoint(new(startPixel.X, startPixel.Y), window)!.Value;
            var end = control.TranslatePoint(new(endPixel.X, endPixel.Y), window)!.Value;
            window.MouseDown(end, MouseButton.Left, RawInputModifiers.Control);
            window.MouseUp(end, MouseButton.Left, RawInputModifiers.Control);
            var markerCount = vm.LabelCollection.Count;
            window.MouseMove(start);
            var tooltip = ToolTip.GetTip(control)?.ToString();
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(end, RawInputModifiers.LeftMouseButton);
            window.MouseUp(end, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            var finalX = crosshair.X;
            window.Close();
            return (markerCount, tooltip, finalX);
        });
        await Assert.That(observed.markerCount).IsEqualTo(1);
        await Assert.That(observed.tooltip).Contains("X:");
        await Assert.That(observed.finalX).IsGreaterThan(1);
    }
}
