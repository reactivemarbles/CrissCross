// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScottPlot;
using ScottPlot.Plottables;
using TUnit.Core.Executors;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#else
using CrissCross.WPF.Plot;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Exercises the library's actual routed pointer handler.</summary>
[NotInParallel]
[TestExecutor<WpfHoverTestExecutor>]
public sealed class LiveChartHoverRegressionTests
{
    /// <summary>The X value deliberately outside the supported OLE Automation date range.</summary>
    private const double UnsupportedDate = 4_000_000;

    /// <summary>The upper X value of the unsupported date view.</summary>
    private const double UnsupportedDateViewMaximum = UnsupportedDate + 100;

    /// <summary>The test plot Y maximum.</summary>
    private const double YMaximum = 10;

    /// <summary>The rendered test image width.</summary>
    private const int ImageWidth = 800;

    /// <summary>The rendered test image height.</summary>
    private const int ImageHeight = 600;

    /// <summary>The chart's named plot host.</summary>
    private const string PlotHostName = "MainChartGrid";

    /// <summary>Verifies that an unrendered empty time plot has no coordinate tooltip.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MouseMove_EmptyUnrenderedTimeChart_DoesNotConvertInvalidCoordinate()
    {
        var chart = new LiveChart();
        using var model = chart.ViewModel!;
        model.IsXAxisDateTime = true;
        var host = (Grid)chart.FindName(PlotHostName);
        var plotControl = model.WpfPlot1vm!;
        var coordinates = plotControl.Plot.GetCoordinates(new(0, 0), model.XAxis1, model.YAxisList[0]);
        await Assert.That(double.IsFinite(coordinates.X)).IsFalse();
        host.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseMoveEvent });
        await Assert.That(plotControl.ToolTip).IsNull();
    }

    /// <summary>Verifies that unsupported date coordinates cannot be converted by the routed hover handler.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MouseMove_UnsupportedDateCoordinate_DoesNotConvertInvalidCoordinate()
    {
        var chart = new LiveChart();
        using var model = chart.ViewModel!;
        var plotControl = model.WpfPlot1vm!;
        model.CreateAxisWithPoints();
        model.IsXAxisDateTime = true;
        plotControl.Plot.Axes.SetLimitsX(UnsupportedDate, UnsupportedDateViewMaximum, model.XAxis1);
        plotControl.Plot.Axes.SetLimitsY(0, YMaximum, model.YAxisList[0]);
        using var image = plotControl.Plot.GetImage(ImageWidth, ImageHeight);
        var host = (Grid)chart.FindName(PlotHostName);
        host.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseMoveEvent });
        await Assert.That(plotControl.ToolTip).IsNull();
    }

    /// <summary>Verifies that valid numeric and date hover coordinates still produce a tooltip without consuming the event.</summary>
    /// <param name="dateTime">Whether hover text uses date formatting.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task MouseMove_ValidCoordinate_FormatsTooltipAndLeavesEventUnhandled(bool dateTime)
    {
        var chart = new LiveChart();
        using var model = chart.ViewModel!;
        var plotControl = model.WpfPlot1vm!;
        model.CreateAxisWithPoints();
        model.IsXAxisDateTime = dateTime;
        var center = dateTime ? new DateTime(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc).ToOADate() : UnsupportedDate;
        plotControl.Plot.Axes.SetLimitsX(center - 1, center + 1, model.XAxis1);
        plotControl.Plot.Axes.SetLimitsY(0, YMaximum, model.YAxisList[0]);
        using var image = plotControl.Plot.GetImage(ImageWidth, ImageHeight);
        var host = (Grid)chart.FindName(PlotHostName);
        var mouseEvent = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseMoveEvent };
        var position = mouseEvent.GetPosition(host);
        var dpi = VisualTreeHelper.GetDpi(host);
        var coordinate = plotControl.Plot.GetCoordinates(new(position.X * dpi.DpiScaleX, position.Y * dpi.DpiScaleY), model.XAxis1, model.YAxisList[0]);
        var expectedX = dateTime
            ? DateTime.FromOADate(coordinate.X).ToString("G", System.Globalization.CultureInfo.CurrentCulture)
            : coordinate.X.ToString("F3", System.Globalization.CultureInfo.CurrentCulture);
        host.RaiseEvent(mouseEvent);
        await Assert.That((string)plotControl.ToolTip).Contains($"X: {expectedX}");
        await Assert.That((string)plotControl.ToolTip).DoesNotContain("NaN");
        await Assert.That(mouseEvent.Handled).IsFalse();
    }

    /// <summary>Verifies that a drag rectangle rounded above the safe date maximum cannot change a vertical line.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MouseMove_DragRectangleRoundsPastDateMaximum_PreservesVerticalLine()
    {
        const double maximumDragView = 1_076_700_290;
        var chart = new LiveChart();
        using var model = chart.ViewModel!;
        var plotControl = model.WpfPlot1vm!;
        model.CreateAxisWithPoints();
        model.IsXAxisDateTime = true;
        plotControl.Plot.Axes.ContinuouslyAutoscale = false;
        var maximumDate = DateTime.MaxValue.ToOADate();
        plotControl.Plot.Axes.SetLimitsX(maximumDate, maximumDragView, model.XAxis1);
        plotControl.Plot.Axes.SetLimitsY(0, YMaximum, model.YAxisList[0]);
        using var image = plotControl.Plot.GetImage(ImageWidth, ImageHeight);
        var host = (Grid)chart.FindName(PlotHostName);
        var mouseEvent = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseMoveEvent };
        var position = mouseEvent.GetPosition(host);
        var coordinate = plotControl.Plot.GetCoordinates(new(position.X, position.Y), model.XAxis1, model.YAxisList[0]);
        var rectangle = plotControl.Plot.GetCoordinateRect((float)position.X, (float)position.Y, radius: 5, model.XAxis1, model.YAxisList[0]);
        await Assert.That(coordinate.X).IsEqualTo(maximumDate);
        await Assert.That(rectangle.HorizontalCenter).IsGreaterThan(maximumDate);
        var line = plotControl.Plot.Add.VerticalLine(1);
        DraggedPlottable(chart) = line;
        host.RaiseEvent(mouseEvent);
        await Assert.That(line.X).IsEqualTo(1);
        await Assert.That(mouseEvent.Handled).IsFalse();
    }

    /// <summary>Verifies that an unsupported date X does not prevent a horizontal line's Y-only drag.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task MouseMove_UnsupportedDateX_ContinuesHorizontalDrag()
    {
        const double initialY = 1;
        var chart = new LiveChart();
        using var model = chart.ViewModel!;
        var plotControl = model.WpfPlot1vm!;
        model.CreateAxisWithPoints();
        model.IsXAxisDateTime = true;
        plotControl.Plot.Axes.ContinuouslyAutoscale = false;
        plotControl.Plot.Axes.SetLimitsX(UnsupportedDate, UnsupportedDateViewMaximum, model.XAxis1);
        plotControl.Plot.Axes.SetLimitsY(0, YMaximum, model.YAxisList[0]);
        using var image = plotControl.Plot.GetImage(ImageWidth, ImageHeight);
        var host = (Grid)chart.FindName(PlotHostName);
        var mouseEvent = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseMoveEvent };
        var position = mouseEvent.GetPosition(host);
        var dpi = VisualTreeHelper.GetDpi(host);
        var rectangle = plotControl.Plot.GetCoordinateRect((float)(position.X * dpi.DpiScaleX), (float)(position.Y * dpi.DpiScaleY), radius: 5, model.XAxis1, model.YAxisList[0]);
        await Assert.That(rectangle.VerticalCenter).IsNotEqualTo(initialY);
        var line = plotControl.Plot.Add.HorizontalLine(initialY);
        DraggedPlottable(chart) = line;
        host.RaiseEvent(mouseEvent);
        await Assert.That(line.Y).IsEqualTo(rectangle.VerticalCenter);
        await Assert.That(plotControl.ToolTip).IsNull();
        await Assert.That(mouseEvent.Handled).IsFalse();
    }

    /// <summary>Sets up the same drag state as a chart pointer-down event.</summary>
    /// <param name="chart">The chart under test.</param>
    /// <returns>The dragged plottable field reference.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_plottableBeingDragged")]
    private static extern ref AxisLine? DraggedPlottable(LiveChart chart);
}
