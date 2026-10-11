// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Plottables;
using PlotColor = ScottPlot.Color;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Provides the interactive Avalonia live chart.</summary>
public partial class LiveChart
{
    /// <summary>Handles the ConfigureReactivePlotXAxis operation.</summary>
    /// <param name="sources">The sources value.</param>
    private void ConfigureReactivePlotXAxis(IEnumerable<IReactivePlotSource> sources)
    {
        HashSet<PlotXAxisKind> axisKinds = [];
        foreach (var source in sources)
        {
            if (source is ReactivePlotSource { XAxisKind: { } axisKind })
            {
                _ = axisKinds.Add(axisKind);
            }
        }

        if (
            axisKinds.Count == 1
            && (axisKinds.Contains(PlotXAxisKind.OADate) || axisKinds.Contains(PlotXAxisKind.Ticks)))
        {
            ViewModel!.CreateAxisWithTimeStamp();
            return;
        }

        ViewModel!.CreateAxisWithPoints();
    }

    /// <summary>Handles the MainChartGrid_MouseDown operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseDown(object? sender, PointerEventArgs e)
    {
        if (
            e.GetCurrentPoint(MainChartGrid).Properties.IsLeftButtonPressed
            && (e.KeyModifiers & KeyModifiers.Control) != 0)
        {
            AddCoordinateMarker(e);
            return;
        }

        BeginAxisLineDrag(e);
    }

    /// <summary>Enables autoscale after locking if needed.</summary>
    private void EnsureAutoScaleAfterLock()
    {
        if (_autoScaled)
        {
            return;
        }

        _needAutoScale = true;
        ExecuteManAutoScale();
    }

    /// <summary>Adds a coordinate marker at the current mouse position.</summary>
    /// <param name="e">The mouse event arguments.</param>
    private void AddCoordinateMarker(PointerEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        Pixel mousePixel = new(adjustedX, adjustedY);
        var mouseLocation = ViewModel!.AvaPlot1vm!.Plot.GetCoordinates(
            mousePixel,
            ViewModel!.XAxis1,
            ViewModel.YAxisList[0]);
        var horizontalCoordinate = mouseLocation.X;
        var verticalCoordinate = mouseLocation.Y;
        var text = ViewModel.IsXAxisDateTime
            ? $"X : {DateTime.FromOADate(horizontalCoordinate).ToLongTimeString()}\nY : {verticalCoordinate:F2}"
            : $"X : {horizontalCoordinate:F2}\nY : {verticalCoordinate:F2}";

        var marker = ViewModel.AvaPlot1vm.Plot.Add.Marker(mouseLocation);
        var markerText = ViewModel.AvaPlot1vm.Plot.Add.Text(text, mouseLocation);
        markerText.OffsetX = CoordinateMarkerTextOffset;
        markerText.OffsetY = -CoordinateMarkerTextOffset;
        markerText!.LabelFontColor = PlotColor.FromColor(System.Drawing.Color.FromName("White"));
        marker.Axes.XAxis = ViewModel!.AvaPlot1vm!.Plot.Axes.Bottom;
        marker.Axes.YAxis = ViewModel.YAxisList[0];
        markerText.Axes.XAxis = ViewModel!.AvaPlot1vm!.Plot.Axes.Bottom;
        markerText.Axes.YAxis = ViewModel.YAxisList[0];
        ViewModel.LabelCollection.Add((marker, markerText));
    }

    /// <summary>Begins dragging an axis line when one is under the mouse pointer.</summary>
    /// <param name="e">The mouse event arguments.</param>
    private void BeginAxisLineDrag(PointerEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        var lineUnderMouse = ViewModel?.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
        if (lineUnderMouse is null)
        {
            return;
        }

        _plottableBeingDragged = lineUnderMouse;
        ViewModel!.AvaPlot1vm!.UserInputProcessor.Disable();
    }

    /// <summary>Handles the MainChartGrid_MouseUp operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseUp(object? sender, PointerEventArgs e)
    {
        _plottableBeingDragged = null;
        if (!_locked)
        {
            ViewModel!.AvaPlot1vm!.UserInputProcessor.Enable();
        }

        ViewModel!.AvaPlot1vm!.Refresh();
    }

    /// <summary>Handles the MainChartGrid_MouseMove operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseMove(object? sender, PointerEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        UpdateHoverTooltip(adjustedX, adjustedY);
        var rect = ViewModel!.AvaPlot1vm!.Plot.GetCoordinateRect(
            (float)adjustedX,
            (float)adjustedY,
            radius: 5,
            ViewModel!.XAxis1,
            ViewModel.YAxisList[0]);
        if (_plottableBeingDragged is null)
        {
            UpdateMouseCursor(adjustedX, adjustedY);
            return;
        }

        if (_plottableBeingDragged is HorizontalLine horizontalLine)
        {
            horizontalLine.Y = rect.VerticalCenter;
            horizontalLine.Text = $"{horizontalLine.Y:0.00}";
        }
        else if (_plottableBeingDragged is VerticalLine verticalLine)
        {
            verticalLine.X = rect.HorizontalCenter;
            verticalLine.Text = ViewModel.IsXAxisDateTime
                ? DateTime.FromOADate(Convert.ToDouble(verticalLine.X)).ToLongTimeString()
                : $"{verticalLine.X:0.00}";
        }

        ViewModel!.AvaPlot1vm!.Refresh();
    }

    /// <summary>Updates the tooltip with the plot coordinates under the pointer.</summary>
    /// <param name="adjustedX">The pointer X position in plot pixels.</param>
    /// <param name="adjustedY">The pointer Y position in plot pixels.</param>
    private void UpdateHoverTooltip(double adjustedX, double adjustedY)
    {
        Pixel mousePixel = new(adjustedX, adjustedY);
        var coordinates = ViewModel!.AvaPlot1vm!.Plot.GetCoordinates(
            mousePixel,
            ViewModel.XAxis1,
            ViewModel.YAxisList[0]);
        var horizontalText = ViewModel.IsXAxisDateTime
            ? DateTime.FromOADate(coordinates.X).ToString("G", CultureInfo.CurrentCulture)
            : coordinates.X.ToString("F3", CultureInfo.CurrentCulture);
        var tooltip = $"X: {horizontalText}{Environment.NewLine}Y: {coordinates.Y:F3}";
        ToolTip.SetTip(ViewModel.AvaPlot1vm, tooltip);
    }

    /// <summary>Gets the DPI-adjusted mouse position.</summary>
    /// <param name="e">The mouse event arguments.</param>
    /// <returns>The adjusted position.</returns>
    private (double X, double Y) GetAdjustedMousePosition(PointerEventArgs e)
    {
        var position = e.GetPosition(MainChartGrid);
        var scaling = TopLevel.GetTopLevel(MainChartGrid)?.RenderScaling ?? 1;
        return (position.X * scaling, position.Y * scaling);
    }

    /// <summary>Updates the mouse cursor for the line under the pointer.</summary>
    /// <param name="adjustedX">The adjusted X coordinate.</param>
    /// <param name="adjustedY">The adjusted Y coordinate.</param>
    private void UpdateMouseCursor(double adjustedX, double adjustedY)
    {
        var lineUnderMouse = ViewModel!.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
        if (lineUnderMouse is null)
        {
            MainChartGrid.Cursor = new(StandardCursorType.Arrow);
            return;
        }

        if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine)
        {
            MainChartGrid.Cursor = new(StandardCursorType.SizeWestEast);
            return;
        }

        if (!lineUnderMouse.IsDraggable || lineUnderMouse is not HorizontalLine)
        {
            return;
        }

        MainChartGrid.Cursor = new(StandardCursorType.SizeNorthSouth);
    }
}
