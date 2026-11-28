// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using CrissCross.Plot.Interactivity;
using SkiaSharp;
using SkiaSharp.Views.Maui;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>Provides MAUI input conversions and plot input processing.</summary>
internal static class MauiPlotExtensions
{
    /// <summary>Converts a pan gesture position to plot pixels.</summary>
    /// <param name="e">The pan event data.</param>
    extension(PanUpdatedEventArgs e)
    {
        /// <summary>Converts the accumulated pan distance to pixels.</summary>
        /// <returns>The pan position in pixels.</returns>
        internal Pixel ToPixel() => new(e.TotalX, e.TotalY);
    }

    /// <summary>Converts MAUI point positions to plot pixels.</summary>
    /// <param name="point">The MAUI point to convert.</param>
    extension(Point point)
    {
        /// <summary>Converts a point to pixels.</summary>
        /// <returns>The converted point.</returns>
        internal Pixel ToPixel() => new(point.X, point.Y);

        /// <summary>Converts a normalized point to scaled pixels.</summary>
        /// <param name="width">The width of the drawing area.</param>
        /// <param name="height">The height of the drawing area.</param>
        /// <returns>The converted point.</returns>
        internal Pixel ToPixelScaled(float width, float height) => new(point.X * width, point.Y * height);
    }

    /// <summary>Converts Skia point positions to plot pixels.</summary>
    /// <param name="point">The Skia point to convert.</param>
    extension(SKPoint point)
    {
        /// <summary>Converts a Skia point to pixels.</summary>
        /// <returns>The converted point.</returns>
        internal Pixel ToPixel() => new(point.X, point.Y);
    }

    /// <summary>Processes MAUI and Skia input events for the plot.</summary>
    /// <param name="processor">The plot input processor.</param>
    extension(UserInputProcessor processor)
    {
        /// <summary>Processes a pan gesture update.</summary>
        /// <param name="plot">The plot control.</param>
        /// <param name="e">The pan event data.</param>
        internal void ProcessPanUpdated(MauiPlot plot, PanUpdatedEventArgs e)
        {
            IUserAction action = e.StatusType switch
            {
                GestureStatus.Started => new Interactivity.UserActions.LeftMouseDown(new(1, 1)),
                GestureStatus.Running => new Interactivity.UserActions.MouseMove(e.ToPixel()),
                GestureStatus.Completed or GestureStatus.Canceled => new Interactivity.UserActions.LeftMouseUp(plot.LastPixel),
                _ => default(Interactivity.UserActions.Unknown),
            };

            if (e.StatusType == GestureStatus.Running)
            {
                plot.LastPixel = e.ToPixel();
            }

            processor.Process(action);
        }

        /// <summary>Processes a pinch gesture update.</summary>
        /// <param name="plot">The plot control.</param>
        /// <param name="e">The pinch event data.</param>
        /// <param name="width">The width of the drawing area.</param>
        /// <param name="height">The height of the drawing area.</param>
        internal void ProcessPinchUpdated(MauiPlot plot, PinchGestureUpdatedEventArgs e, float width, float height)
        {
            if (e.Status != GestureStatus.Running || !processor.IsEnabled)
            {
                return;
            }

            Pixel pixel = e.ScaleOrigin.ToPixelScaled(width, height);
            MouseAxisManipulation.MouseWheelZoom(plot.Plot, e.Scale, e.Scale, pixel, false);
            plot.Refresh();
        }

        /// <summary>Processes a desktop pointer press.</summary>
        /// <param name="e">The touch event data.</param>
        internal void ProcessMouseDown(SKTouchEventArgs e)
        {
            Pixel pixel = e.Location.ToPixel();
            IUserAction action = e.MouseButton switch
            {
                SKMouseButton.Left => new Interactivity.UserActions.LeftMouseDown(pixel),
                SKMouseButton.Middle => new Interactivity.UserActions.MiddleMouseDown(pixel),
                SKMouseButton.Right => new Interactivity.UserActions.RightMouseDown(pixel),
                _ => default(Interactivity.UserActions.Unknown),
            };

            processor.Process(action);
        }

        /// <summary>Processes a desktop pointer release.</summary>
        /// <param name="e">The touch event data.</param>
        internal void ProcessMouseUp(SKTouchEventArgs e)
        {
            Pixel pixel = e.Location.ToPixel();
            IUserAction action = e.MouseButton switch
            {
                SKMouseButton.Left => new Interactivity.UserActions.LeftMouseUp(pixel),
                SKMouseButton.Middle => new Interactivity.UserActions.MiddleMouseUp(pixel),
                SKMouseButton.Right => new Interactivity.UserActions.RightMouseUp(pixel),
                _ => default(Interactivity.UserActions.Unknown),
            };

            processor.Process(action);
        }

        /// <summary>Processes desktop pointer movement.</summary>
        /// <param name="e">The touch event data.</param>
        internal void ProcessMouseMove(SKTouchEventArgs e) => processor.Process(new Interactivity.UserActions.MouseMove(e.Location.ToPixel()));

        /// <summary>Processes a desktop mouse-wheel update.</summary>
        /// <param name="e">The touch event data.</param>
        internal void ProcessWheelChanged(SKTouchEventArgs e)
        {
            Pixel pixel = e.Location.ToPixel();
            IUserAction action = e.WheelDelta switch
            {
                > 0 => new Interactivity.UserActions.MouseWheelUp(pixel),
                _ => new Interactivity.UserActions.MouseWheelDown(pixel),
            };

            processor.Process(action);
        }
    }
}
