// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>
/// This class stores logic for changing axis limits according to mouse inputs in pixel units.
/// Methods here are similar to those in <see cref="Plot.Axes"/> except their inputs are all mouse events.
/// </summary>
public static class MouseAxisManipulation
{
    /// <summary>Zooms the axes around the mouse position in response to a wheel action.</summary>
    /// <param name="plot">The plot whose axes should be zoomed.</param>
    /// <param name="fracX">The horizontal zoom fraction.</param>
    /// <param name="fracY">The vertical zoom fraction.</param>
    /// <param name="pixel">The mouse position in pixels.</param>
    /// <param name="changeOpposingAxesTogether">Whether parallel axes should be zoomed together.</param>
    public static void MouseWheelZoom(Plot plot, double fracX, double fracY, Pixel pixel, bool changeOpposingAxesTogether)
    {
        pixel = pixel.Divide(plot.ScaleFactorF);
        MultiAxisLimits originalLimits = new(plot);
        PixelRect dataRect = plot.RenderManager.LastRender.DataRect;

        // restore MouseDown limits
        originalLimits.Recall();

        var axisUnderMouse = plot.GetAxis(pixel);

        if (axisUnderMouse is not null)
        {
            if (changeOpposingAxesTogether && axisUnderMouse.IsHorizontal())
            {
                plot.Axes.XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomFrac(fracX, horizontalAxis.GetCoordinate(pixel.X, dataRect)));
            }
            else if (changeOpposingAxesTogether && axisUnderMouse.IsVertical())
            {
                plot.Axes.YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomFrac(fracY, verticalAxis.GetCoordinate(pixel.Y, dataRect)));
            }
            else
            {
                double frac = axisUnderMouse.IsHorizontal() ? fracX : fracY;
                float scaledCoord = (axisUnderMouse.IsHorizontal() ? pixel.X : pixel.Y);
                axisUnderMouse.Range.ZoomFrac(frac, axisUnderMouse.GetCoordinate(scaledCoord, dataRect));
            }
        }
        else
        {
            // modify all axes
            plot.Axes.XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomFrac(fracX, horizontalAxis.GetCoordinate(pixel.X, dataRect)));
            plot.Axes.YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomFrac(fracY, verticalAxis.GetCoordinate(pixel.Y, dataRect)));
        }
    }

    /// <summary>Pans the axes by the drag distance.</summary>
    /// <param name="plot">The plot whose axes should be panned.</param>
    /// <param name="mouseDown">The initial mouse position.</param>
    /// <param name="mouseNow">The current mouse position.</param>
    /// <param name="changeOpposingAxesTogether">Whether parallel axes should be panned together.</param>
    public static void DragPan(Plot plot, Pixel mouseDown, Pixel mouseNow, bool changeOpposingAxesTogether)
    {
        mouseDown = mouseDown.Divide(plot.ScaleFactorF);
        mouseNow = mouseNow.Divide(plot.ScaleFactorF);

        float pixelDeltaX = -(mouseNow.X - mouseDown.X);
        float pixelDeltaY = mouseNow.Y - mouseDown.Y;

        float scaledDeltaX = pixelDeltaX / plot.ScaleFactorF;
        float scaledDeltaY = pixelDeltaY / plot.ScaleFactorF;

        IAxis? axisUnderMouse = plot.GetAxis(mouseDown);

        PixelRect dataRect = plot.RenderManager.LastRender.DataRect;

        if (axisUnderMouse is not null)
        {
            if (changeOpposingAxesTogether && axisUnderMouse.IsHorizontal())
            {
                plot.Axes.XAxes.ForEach(horizontalAxis => horizontalAxis.Range.PanMouse(scaledDeltaX, dataRect.Width));
            }
            else if (changeOpposingAxesTogether && axisUnderMouse.IsVertical())
            {
                plot.Axes.YAxes.ForEach(verticalAxis => verticalAxis.Range.PanMouse(scaledDeltaY, dataRect.Height));
            }
            else
            {
                float scaledDelta = axisUnderMouse.IsHorizontal() ? scaledDeltaX : scaledDeltaY;
                float dataSize = axisUnderMouse.IsHorizontal() ? dataRect.Width : dataRect.Height;
                axisUnderMouse.Range.PanMouse(scaledDelta, dataSize);
            }
        }
        else
        {
            // modify all axes
            plot.Axes.XAxes.ForEach(horizontalAxis => horizontalAxis.Range.PanMouse(scaledDeltaX, dataRect.Width));
            plot.Axes.YAxes.ForEach(verticalAxis => verticalAxis.Range.PanMouse(scaledDeltaY, dataRect.Height));
        }
    }

    /// <summary>Zooms the axes by the drag distance.</summary>
    /// <param name="plot">The plot whose axes should be zoomed.</param>
    /// <param name="mouseDown">The initial mouse position.</param>
    /// <param name="mouseNow">The current mouse position.</param>
    /// <param name="changeOpposingAxesTogether">Whether parallel axes should be zoomed together.</param>
    public static void DragZoom(Plot plot, Pixel mouseDown, Pixel mouseNow, bool changeOpposingAxesTogether)
    {
        mouseDown = mouseDown.Divide(plot.ScaleFactorF);
        mouseNow = mouseNow.Divide(plot.ScaleFactorF);

        float pixelDeltaX = mouseNow.X - mouseDown.X;
        float pixelDeltaY = -(mouseNow.Y - mouseDown.Y);

        IAxis? axisUnderMouse = plot.GetAxis(mouseDown);

        PixelRect lastRenderDataRect = plot.RenderManager.LastRender.DataRect;

        if (axisUnderMouse is not null)
        {
            if (changeOpposingAxesTogether && axisUnderMouse.IsHorizontal())
            {
                plot.Axes.XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomMouseDelta(pixelDeltaX, lastRenderDataRect.Width));
            }
            else if (changeOpposingAxesTogether && axisUnderMouse.IsVertical())
            {
                plot.Axes.YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomMouseDelta(pixelDeltaY, lastRenderDataRect.Height));
            }
            else
            {
                // modify a single axis
                float pixelDelta = axisUnderMouse.IsHorizontal() ? pixelDeltaX : pixelDeltaY;
                float dataSize = axisUnderMouse.IsHorizontal() ? lastRenderDataRect.Width : lastRenderDataRect.Height;
                axisUnderMouse.Range.ZoomMouseDelta(pixelDelta, dataSize);
            }
        }
        else
        {
            // modify all axes
            plot.Axes.XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomMouseDelta(pixelDeltaX, lastRenderDataRect.Width));
            plot.Axes.YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomMouseDelta(pixelDeltaY, lastRenderDataRect.Height));
        }
    }

    /// <summary>Updates the zoom rectangle based on the mouse drag.</summary>
    /// <param name="plot">The plot whose zoom rectangle should be updated.</param>
    /// <param name="mouseDown">The initial mouse position.</param>
    /// <param name="mouseNow">The current mouse position.</param>
    public static void PlaceZoomRectangle(Plot plot, Pixel mouseDown, Pixel mouseNow)
    {
        mouseDown = mouseDown.Divide(plot.ScaleFactorF);
        mouseNow = mouseNow.Divide(plot.ScaleFactorF);

        IAxis? axisUnderMouse = plot.GetAxis(mouseDown);
        if (axisUnderMouse is not null)
        {
            // Do not respond if the axis under the mouse has no data
            bool axisIsUsedByPlottables = false;
            foreach (var plottable in plot.GetPlottables())
            {
                if (EqualityComparer<IAxis>.Default.Equals(axisUnderMouse, plottable.Axes.XAxis)
                    || EqualityComparer<IAxis>.Default.Equals(axisUnderMouse, plottable.Axes.YAxis))
                {
                    axisIsUsedByPlottables = true;
                    break;
                }
            }

            if (!axisIsUsedByPlottables)
            {
                return;
            }

            // NOTE: this function only changes shape of the rectangle.
            // It doesn't modify axis limits, so no action is required on the opposite axis.
            plot.ZoomRectangle.VerticalSpan = axisUnderMouse.IsHorizontal();
            plot.ZoomRectangle.HorizontalSpan = axisUnderMouse.IsVertical();
        }

        plot.ZoomRectangle.MouseDown = mouseDown;
        plot.ZoomRectangle.MouseUp = mouseNow;
    }

    /// <summary>Auto-scales the relevant axes based on the mouse position.</summary>
    /// <param name="plot">The plot whose axes should be auto-scaled.</param>
    /// <param name="pixel">The mouse position in pixels.</param>
    /// <param name="allParallelAxes">Whether all parallel axes should be auto-scaled.</param>
    public static void AutoScale(Plot plot, Pixel pixel, bool allParallelAxes)
    {
        pixel = pixel.Divide(plot.ScaleFactorF);
        IAxis? axisUnderMouse = plot.GetAxis(pixel);

        if (axisUnderMouse is null)
        {
            plot.Axes.AutoScale();
            return;
        }

        if (allParallelAxes)
        {
            if (axisUnderMouse.IsHorizontal())
            {
                plot.Axes.XAxes.ForEach(plot.Axes.AutoScaleX);
            }
            else if (axisUnderMouse.IsVertical())
            {
                plot.Axes.YAxes.ForEach(plot.Axes.AutoScaleY);
            }
        }
        else if (axisUnderMouse is IYAxis verticalAxisUnderMouse)
        {
            plot.Axes.AutoScaleY(verticalAxisUnderMouse);
        }
        else if (axisUnderMouse is IXAxis horizontalAxisUnderMouse)
        {
            plot.Axes.AutoScaleX(horizontalAxisUnderMouse);
        }
    }
}
