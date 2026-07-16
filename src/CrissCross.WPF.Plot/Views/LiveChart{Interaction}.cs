// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ScottPlot;
using ScottPlot.Plottables;
using PlotColor = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>Interaction logic for WPF Chart AICS.</summary>
public partial class LiveChart
{
    /// <summary>Handles the ConfigureReactivePlotXAxis operation.</summary>
    /// <param name="sources">The sources value.</param>
    private void ConfigureReactivePlotXAxis(IEnumerable<IReactivePlotSource> sources)
    {
        var axisKinds = sources
            .OfType<ReactivePlotSource>()
            .Select(source => source.XAxisKind)
            .Where(kind => kind is not null)
            .Select(kind => kind!.Value)
            .Distinct()
            .ToArray();

        if (axisKinds.Length == 1 && axisKinds[0] is PlotXAxisKind.OADate or PlotXAxisKind.Ticks)
        {
            ViewModel!.CreateAxisWithTimeStamp();
            return;
        }

        ViewModel!.CreateAxisWithPoints();
    }

    /// <summary>Handles the MainChartGrid_MouseDown operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseDown(object sender, MouseEventArgs e)
    {
        if (
            e.LeftButton == MouseButtonState.Pressed
            && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
        )
        {
            AddCoordinateMarker(e);
            return;
        }

        BeginAxisLineDrag(e);
    }

    /// <summary>Toggles the left panel visibility.</summary>
    private void ToggleLeftPanelVisibility() =>
        ViewModel!.LeftPanelVisibility =
            ViewModel.LeftPanelVisibility == Visibility.Hidden
                ? Visibility.Visible
                : Visibility.Hidden;

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
    private void AddCoordinateMarker(MouseEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        Pixel mousePixel = new(adjustedX, adjustedY);
        var mouseLocation = ViewModel!.WpfPlot1vm!.Plot.GetCoordinates(
            mousePixel,
            ViewModel!.XAxis1,
            ViewModel.YAxisList[0]);
        var horizontalCoordinate = mouseLocation.X;
        var verticalCoordinate = mouseLocation.Y;
        var text = ViewModel.IsXAxisDateTime
            ? "X : "
                + DateTime.FromOADate(horizontalCoordinate).ToLongTimeString()
                + "\nY : "
                + verticalCoordinate.ToString("F2")
            : "X : "
                + horizontalCoordinate.ToString("F2")
                + "\nY : "
                + verticalCoordinate.ToString("F2");

        var marker = ViewModel.WpfPlot1vm.Plot.Add.Marker(mouseLocation);
        var markerText = ViewModel.WpfPlot1vm.Plot.Add.Text(text, mouseLocation);
        markerText.OffsetX = CoordinateMarkerTextOffset;
        markerText.OffsetY = -CoordinateMarkerTextOffset;
        markerText!.LabelFontColor = PlotColor.FromColor(System.Drawing.Color.FromName("White"));
        marker.Axes.XAxis = ViewModel!.WpfPlot1vm!.Plot.Axes.Bottom;
        marker.Axes.YAxis = ViewModel.YAxisList[0];
        markerText.Axes.XAxis = ViewModel!.WpfPlot1vm!.Plot.Axes.Bottom;
        markerText.Axes.YAxis = ViewModel.YAxisList[0];
        ViewModel.LabelCollection.Add((marker, markerText));
    }

    /// <summary>Begins dragging an axis line when one is under the mouse pointer.</summary>
    /// <param name="e">The mouse event arguments.</param>
    private void BeginAxisLineDrag(MouseEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        var lineUnderMouse = ViewModel?.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
        if (lineUnderMouse is null)
        {
            return;
        }

        _plottableBeingDragged = lineUnderMouse;
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Disable();
    }

    /// <summary>Handles the MainChartGrid_MouseUp operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseUp(object sender, MouseEventArgs e)
    {
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Enable(); // enable panning again
        _plottableBeingDragged = null;
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Enable(); // enable panning again
        ViewModel!.WpfPlot1vm!.Refresh();
    }

    /// <summary>Handles the MainChartGrid_MouseMove operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseMove(object sender, MouseEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        UpdateHoverTooltip(adjustedX, adjustedY);
        var rect = ViewModel!.WpfPlot1vm!.Plot.GetCoordinateRect(
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

        ViewModel!.WpfPlot1vm!.Refresh();
    }

    /// <summary>Updates the tooltip with the plot coordinates under the pointer.</summary>
    /// <param name="adjustedX">The pointer X position in plot pixels.</param>
    /// <param name="adjustedY">The pointer Y position in plot pixels.</param>
    private void UpdateHoverTooltip(double adjustedX, double adjustedY)
    {
        Pixel mousePixel = new(adjustedX, adjustedY);
        var coordinates = ViewModel!.WpfPlot1vm!.Plot.GetCoordinates(
            mousePixel,
            ViewModel.XAxis1,
            ViewModel.YAxisList[0]);
        var horizontalText = ViewModel.IsXAxisDateTime
            ? DateTime.FromOADate(coordinates.X).ToString("G", CultureInfo.CurrentCulture)
            : coordinates.X.ToString("F3", CultureInfo.CurrentCulture);
        var tooltip = $"X: {horizontalText}{Environment.NewLine}Y: {coordinates.Y:F3}";
        ViewModel.WpfPlot1vm.SetCurrentValue(FrameworkElement.ToolTipProperty, tooltip);
    }

    /// <summary>Gets the DPI-adjusted mouse position.</summary>
    /// <param name="e">The mouse event arguments.</param>
    /// <returns>The adjusted position.</returns>
    private (double X, double Y) GetAdjustedMousePosition(MouseEventArgs e)
    {
        var position = e.GetPosition(MainChartGrid);
        var dpiInfo = VisualTreeHelper.GetDpi(MainChartGrid);
        return (position.X * dpiInfo.DpiScaleX, position.Y * dpiInfo.DpiScaleY);
    }

    /// <summary>Updates the mouse cursor for the line under the pointer.</summary>
    /// <param name="adjustedX">The adjusted X coordinate.</param>
    /// <param name="adjustedY">The adjusted Y coordinate.</param>
    private void UpdateMouseCursor(double adjustedX, double adjustedY)
    {
        var lineUnderMouse = ViewModel!.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
        if (lineUnderMouse is null)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }

        if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine)
        {
            Mouse.OverrideCursor = Cursors.SizeWE;
            return;
        }

        if (!lineUnderMouse.IsDraggable || lineUnderMouse is not HorizontalLine)
        {
            return;
        }

        Mouse.OverrideCursor = Cursors.SizeNS;
    }
}
