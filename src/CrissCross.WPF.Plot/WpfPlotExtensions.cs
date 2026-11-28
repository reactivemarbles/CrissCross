// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PlotCursor = CrissCross.Plot.Cursor;
using PlotModel = CrissCross.Plot.Plot;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Converts WPF input and graphics to plot equivalents.</summary>
internal static class WpfPlotExtensions
{
    /// <summary>Extension members for <c>FrameworkElement</c>.</summary>
    /// <param name="fe">The plot element.</param>
    extension(FrameworkElement fe)
    {
        /// <summary>Converts a position to device pixels.</summary>
        /// <param name="position">The position in WPF units.</param>
        /// <returns>The position in device pixels.</returns>
        internal Pixel ToPixel(Point position)
        {
            DpiScale dpiScale = VisualTreeHelper.GetDpi(fe);
            return new((float)(position.X * dpiScale.DpiScaleX), (float)(position.Y * dpiScale.DpiScaleY));
        }
    }

    /// <summary>Extension members for <c>Interactivity.UserInputProcessor</c>.</summary>
    /// <param name="processor">The input processor.</param>
    extension(Interactivity.UserInputProcessor processor)
    {
        /// <summary>Processes a mouse button press.</summary>
        /// <param name="fe">The plot element.</param>
        /// <param name="e">The WPF event.</param>
        internal void ProcessMouseDown(FrameworkElement fe, MouseButtonEventArgs e)
        {
            SyncModifierKeys(processor);
            Pixel pixel = e.ToPixel(fe);

            Interactivity.IUserAction action = e.ChangedButton switch
            {
                MouseButton.Left => new Interactivity.UserActions.LeftMouseDown(pixel),
                MouseButton.Middle => new Interactivity.UserActions.MiddleMouseDown(pixel),
                MouseButton.Right => new Interactivity.UserActions.RightMouseDown(pixel),
                _ => new Interactivity.UserActions.Unknown(e.ChangedButton.ToString(), "pressed"),
            };

            processor.Process(action);
        }

        /// <summary>Processes a mouse button release.</summary>
        /// <param name="fe">The plot element.</param>
        /// <param name="e">The WPF event.</param>
        internal void ProcessMouseUp(FrameworkElement fe, MouseButtonEventArgs e)
        {
            SyncModifierKeys(processor);
            Pixel pixel = e.ToPixel(fe);

            Interactivity.IUserAction action = e.ChangedButton switch
            {
                MouseButton.Left => new Interactivity.UserActions.LeftMouseUp(pixel),
                MouseButton.Middle => new Interactivity.UserActions.MiddleMouseUp(pixel),
                MouseButton.Right => new Interactivity.UserActions.RightMouseUp(pixel),
                _ => new Interactivity.UserActions.Unknown(e.ChangedButton.ToString(), "released"),
            };

            processor.Process(action);
        }

        /// <summary>Processes a mouse movement.</summary>
        /// <param name="fe">The plot element.</param>
        /// <param name="e">The WPF event.</param>
        internal void ProcessMouseMove(FrameworkElement fe, MouseEventArgs e)
        {
            SyncModifierKeys(processor);
            Pixel pixel = e.ToPixel(fe);
            Interactivity.IUserAction action = new Interactivity.UserActions.MouseMove(pixel);
            processor.Process(action);
        }

        /// <summary>Processes mouse wheel input.</summary>
        /// <param name="fe">The plot element.</param>
        /// <param name="e">The WPF event.</param>
        internal void ProcessMouseWheel(FrameworkElement fe, MouseWheelEventArgs e)
        {
            SyncModifierKeys(processor);
            Pixel pixel = e.ToPixel(fe);

            Interactivity.IUserAction action = e.Delta > 0
                ? new Interactivity.UserActions.MouseWheelUp(pixel)
                : new Interactivity.UserActions.MouseWheelDown(pixel);

            processor.Process(action);
        }

        /// <summary>Processes a key press.</summary>
        /// <param name="e">The WPF event.</param>
        internal void ProcessKeyDown(KeyEventArgs e)
        {
            Interactivity.Key key = e.ToKey();
            Interactivity.IUserAction action = new Interactivity.UserActions.KeyDown(key);
            processor.Process(action);
        }

        /// <summary>Processes a key release.</summary>
        /// <param name="e">The WPF event.</param>
        internal void ProcessKeyUp(KeyEventArgs e)
        {
            Interactivity.Key key = e.ToKey();
            Interactivity.IUserAction action = new Interactivity.UserActions.KeyUp(key);
            processor.Process(action);
        }
    }

    /// <summary>Extension members for <c>KeyEventArgs</c>.</summary>
    /// <param name="e">The WPF event.</param>
    extension(KeyEventArgs e)
    {
        /// <summary>Maps a WPF key to a plot key.</summary>
        /// <returns>The plot key.</returns>
        internal Interactivity.Key ToKey()
        {
            Key key = e.Key == Key.System ? e.SystemKey : e.Key; // required to capture Alt

            return key switch
            {
                Key.LeftCtrl or Key.RightCtrl => Interactivity.StandardKeys.Control,
                Key.LeftAlt or Key.RightAlt => Interactivity.StandardKeys.Alt,
                Key.LeftShift or Key.RightShift => Interactivity.StandardKeys.Shift,
                _ => new Interactivity.Key(key.ToString()),
            };
        }
    }

    /// <summary>Extension members for mouse events.</summary>
    /// <param name="e">The WPF event.</param>
    extension(MouseEventArgs e)
    {
        /// <summary>Gets the mouse position in device pixels.</summary>
        /// <param name="fe">The plot element.</param>
        /// <returns>The mouse position.</returns>
        internal Pixel ToPixel(FrameworkElement fe) => fe.ToPixel(e.GetPosition(fe));
    }

    /// <summary>Extension members for <c>PlotCursor</c>.</summary>
    /// <param name="cursor">The plot cursor.</param>
    extension(PlotCursor cursor)
    {
        /// <summary>Maps a plot cursor to its WPF equivalent.</summary>
        /// <returns>The WPF cursor.</returns>
        internal System.Windows.Input.Cursor GetCursor() => cursor switch
        {
            PlotCursor.Arrow => Cursors.Arrow,
            PlotCursor.No => Cursors.No,
            PlotCursor.Wait => Cursors.Wait,
            PlotCursor.Hand => Cursors.Hand,
            PlotCursor.Cross => Cursors.Cross,
            PlotCursor.SizeAll => Cursors.SizeAll,
            PlotCursor.SizeNorthSouth => Cursors.SizeNS,
            PlotCursor.SizeWestEast => Cursors.SizeWE,
            _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, "Unknown plot cursor."),
        };
    }

    /// <summary>Extension members for <c>PlotModel</c>.</summary>
    /// <param name="plot">The plot to render.</param>
    extension(PlotModel plot)
    {
        /// <summary>Renders the plot to a WPF bitmap.</summary>
        /// <param name="width">The image width in pixels.</param>
        /// <param name="height">The image height in pixels.</param>
        /// <returns>The rendered bitmap.</returns>
        internal BitmapImage GetBitmapImage(int width, int height)
        {
            byte[] bytes = plot.GetImageBytes(width, height);
            using MemoryStream ms = new(bytes);

            BitmapImage bmp = new();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        }
    }

    /// <summary>Synchronizes the processor's modifier keys with WPF.</summary>
    /// <param name="processor">The input processor.</param>
    private static void SyncModifierKeys(Interactivity.UserInputProcessor processor)
    {
        ModifierKeys mods = Keyboard.Modifiers;
        processor.KeyState.Remove(Interactivity.StandardKeys.Alt);
        processor.KeyState.Remove(Interactivity.StandardKeys.Control);
        processor.KeyState.Remove(Interactivity.StandardKeys.Shift);

        if ((mods & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
            processor.KeyState.Add(Interactivity.StandardKeys.Alt);
        }

        if ((mods & ModifierKeys.Control) == ModifierKeys.Control)
        {
            processor.KeyState.Add(Interactivity.StandardKeys.Control);
        }

        if ((mods & ModifierKeys.Shift) != ModifierKeys.Shift)
        {
            return;
        }

        processor.KeyState.Add(Interactivity.StandardKeys.Shift);
    }
}
