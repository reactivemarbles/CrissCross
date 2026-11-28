// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CrissCross.Plot;
using CrissCross.Plot.Interactivity;
using CrissCross.Plot.Interactivity.UserActions;
using AvaCursor = Avalonia.Input.Cursor;
using AvaKey = Avalonia.Input.Key;
using PlotCursor = CrissCross.Plot.Cursor;
using PlotKey = CrissCross.Plot.Interactivity.Key;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Provides Avalonia input conversions and plot input processing.</summary>
internal static class AvaPlotExtensions
{
    /// <summary>The modifier keys mapped from Avalonia to plot input state.</summary>
    private static readonly (KeyModifiers Modifier, PlotKey Key)[] ModifierKeyMappings =
    [
        (KeyModifiers.Alt, StandardKeys.Alt),
        (KeyModifiers.Control, StandardKeys.Control),
        (KeyModifiers.Shift, StandardKeys.Shift),
    ];

    /// <summary>Converts Avalonia keys to plot keys.</summary>
    /// <param name="key">The Avalonia key.</param>
    extension(AvaKey key)
    {
        /// <summary>Gets the equivalent plot key.</summary>
        /// <returns>The plot key represented by the Avalonia key.</returns>
        internal PlotKey GetKey() => key switch
        {
            AvaKey.LeftAlt or AvaKey.RightAlt => StandardKeys.Alt,
            AvaKey.LeftShift or AvaKey.RightShift => StandardKeys.Shift,
            AvaKey.LeftCtrl or AvaKey.RightCtrl => StandardKeys.Control,
            _ => new(key.ToString()),
        };
    }

    /// <summary>Converts plot cursors to Avalonia cursors.</summary>
    /// <param name="cursor">The plot cursor.</param>
    extension(PlotCursor cursor)
    {
        /// <summary>Gets the Avalonia cursor represented by the plot cursor.</summary>
        /// <returns>The corresponding Avalonia cursor.</returns>
        internal AvaCursor GetCursor() => cursor switch
        {
            PlotCursor.Arrow => new(StandardCursorType.Arrow),
            PlotCursor.No => new(StandardCursorType.No),
            PlotCursor.Wait => new(StandardCursorType.Wait),
            PlotCursor.Hand => new(StandardCursorType.Hand),
            PlotCursor.Cross => new(StandardCursorType.Cross),
            PlotCursor.SizeAll => new(StandardCursorType.SizeAll),
            PlotCursor.SizeNorthSouth => new(StandardCursorType.SizeNorthSouth),
            PlotCursor.SizeWestEast => new(StandardCursorType.SizeWestEast),
            _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, "Unknown plot cursor."),
        };
    }

    /// <summary>Converts Avalonia pointer positions to plot pixels.</summary>
    /// <param name="e">The pointer event data.</param>
    extension(PointerEventArgs e)
    {
        /// <summary>Gets the pointer position relative to the specified visual.</summary>
        /// <param name="visual">The visual used as the coordinate origin.</param>
        /// <returns>The pointer position in plot pixels.</returns>
        internal Pixel ToPixel(Visual visual)
        {
            Point position = e.GetPosition(visual);
            return new((float)position.X, (float)position.Y);
        }
    }

    /// <summary>Processes Avalonia input events for the plot.</summary>
    /// <param name="processor">The plot input processor.</param>
    extension(UserInputProcessor processor)
    {
        /// <summary>Processes a pointer press.</summary>
        /// <param name="pixel">The pointer position in plot pixels.</param>
        /// <param name="kind">The pointer button transition.</param>
        /// <param name="mods">The active modifier keys.</param>
        internal void ProcessMouseDown(Pixel pixel, PointerUpdateKind kind, KeyModifiers mods)
        {
            processor.SyncModifierKeys(mods);
            IUserAction action = kind switch
            {
                PointerUpdateKind.LeftButtonPressed => new LeftMouseDown(pixel),
                PointerUpdateKind.MiddleButtonPressed => new MiddleMouseDown(pixel),
                PointerUpdateKind.RightButtonPressed => new RightMouseDown(pixel),
                _ => new Unknown("mouse down", kind.ToString()),
            };

            processor.Process(action);
        }

        /// <summary>Processes a pointer release.</summary>
        /// <param name="pixel">The pointer position in plot pixels.</param>
        /// <param name="kind">The pointer button transition.</param>
        /// <param name="mods">The active modifier keys.</param>
        internal void ProcessMouseUp(Pixel pixel, PointerUpdateKind kind, KeyModifiers mods)
        {
            processor.SyncModifierKeys(mods);
            IUserAction action = kind switch
            {
                PointerUpdateKind.LeftButtonReleased => new LeftMouseUp(pixel),
                PointerUpdateKind.MiddleButtonReleased => new MiddleMouseUp(pixel),
                PointerUpdateKind.RightButtonReleased => new RightMouseUp(pixel),
                _ => new Unknown("mouse up", kind.ToString()),
            };

            processor.Process(action);
        }

        /// <summary>Processes pointer movement.</summary>
        /// <param name="pixel">The pointer position in plot pixels.</param>
        /// <param name="mods">The active modifier keys.</param>
        internal void ProcessMouseMove(Pixel pixel, KeyModifiers mods)
        {
            processor.SyncModifierKeys(mods);
            processor.Process(new MouseMove(pixel));
        }

        /// <summary>Processes a mouse-wheel update.</summary>
        /// <param name="pixel">The pointer position in plot pixels.</param>
        /// <param name="delta">The wheel delta.</param>
        /// <param name="mods">The active modifier keys.</param>
        internal void ProcessMouseWheel(Pixel pixel, double delta, KeyModifiers mods)
        {
            processor.SyncModifierKeys(mods);
            IUserAction action = delta > 0 ? new MouseWheelUp(pixel) : new MouseWheelDown(pixel);
            processor.Process(action);
        }

        /// <summary>Processes a key press.</summary>
        /// <param name="e">The key event data.</param>
        internal void ProcessKeyDown(KeyEventArgs e) => processor.Process(new KeyDown(e.Key.GetKey()));

        /// <summary>Processes a key release.</summary>
        /// <param name="e">The key event data.</param>
        internal void ProcessKeyUp(KeyEventArgs e) => processor.Process(new KeyUp(e.Key.GetKey()));

        /// <summary>Synchronizes plot modifier-key state with Avalonia input.</summary>
        /// <param name="mods">The active modifier keys.</param>
        private void SyncModifierKeys(KeyModifiers mods)
        {
            processor.KeyState.Remove(StandardKeys.Alt);
            processor.KeyState.Remove(StandardKeys.Control);
            processor.KeyState.Remove(StandardKeys.Shift);

            foreach ((KeyModifiers modifier, PlotKey key) in ModifierKeyMappings)
            {
                if ((mods & modifier) != 0)
                {
                    processor.KeyState.Add(key);
                }
            }
        }
    }
}
