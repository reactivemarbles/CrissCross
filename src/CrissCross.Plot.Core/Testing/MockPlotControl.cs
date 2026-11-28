// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Interactivity;
using CrissCross.Plot.Interactivity.UserActions;

namespace CrissCross.Plot.Testing;

/// <summary>A plot control that renders in-memory and has functionality useful for testing interactivity.</summary>
public class MockPlotControl : IPlotControl
{
    /// <summary>The divisor used to calculate the center point of the control.</summary>
    private const float HalfDimensionDivisor = 2;

    /// <summary>Initializes a new instance of the <see cref="MockPlotControl"/> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2403",
        Justification = "The mock must pass itself to its plot and input processor to emulate a live control during initialization.")]
    public MockPlotControl()
    {
        Plot = new() { PlotControl = this };
        Multiplot = new Multiplot(Plot);
        UserInputProcessor = new(this) { IsEnabled = true };

        // Force a render on startup so pixel drag actions can be used immediately.
        Refresh();
    }

    /// <summary>Gets or sets the mock control width in pixels.</summary>
    public int Width { get; set; } = 400;

    /// <summary>Gets or sets the mock control height in pixels.</summary>
    public int Height { get; set; } = 300;

    /// <summary>Gets the center point of the mock control in pixels.</summary>
    public Pixel Center => new((float)Width / HalfDimensionDivisor, (float)Height / HalfDimensionDivisor);

    /// <summary>Gets the plot displayed by this control.</summary>
    public Plot Plot { get; private set; }

    /// <summary>Gets or sets the multiplot managed by this control.</summary>
    public IMultiplot Multiplot { get; set; }

    /// <summary>Gets the graphics context; this mock does not use hardware acceleration.</summary>
    public GRContext? GRContext => null;

    /// <summary>Gets the processor that handles simulated user input.</summary>
    public UserInputProcessor UserInputProcessor { get; }

    /// <summary>Gets or sets the optional menu used by this mock control.</summary>
    public IPlotMenu? Menu { get; set; }

    /// <summary>Gets or sets the simulated display scale.</summary>
    public float DisplayScale { get; set; } = 1;

    /// <summary>Gets the number of times this control has been refreshed.</summary>
    public int RefreshCount { get; private set; }

    /// <summary>Gets the number of context-menu requests made by this control.</summary>
    public int ContextMenuLaunchCount { get; private set; }

    /// <summary>Returns the simulated display scale.</summary>
    /// <returns>The current <see cref="DisplayScale"/> value.</returns>
    public float DetectDisplayScale() => DisplayScale;

    /// <summary>Renders the multiplot using the current mock dimensions.</summary>
    public void Refresh()
    {
        RefreshCount++;
        _ = Multiplot.Render(Width, Height);
    }

    /// <summary>Disposes the current plot and creates a new plot associated with this control.</summary>
    public void Reset() => Reset(new() { PlotControl = this });

    /// <summary>Loads the specified plot into this control, disposing the previous plot.</summary>
    /// <param name="plot">The plot to load.</param>
    public void Reset(Plot plot)
    {
        Plot oldPlot = Plot;
        Plot = plot;
        Plot.PlotControl = this;
        oldPlot.Dispose();
        Multiplot.Reset(plot);
    }

    /// <summary>Records a request to show the context menu at the specified position.</summary>
    /// <param name="position">The position where the menu should be shown.</param>
    public void ShowContextMenu(Pixel position) => ++ContextMenuLaunchCount;

    /// <summary>Simulates a scroll-wheel-up action at the specified pixel.</summary>
    /// <param name="pixel">The pixel position of the pointer.</param>
    public void ScrollWheelUp(Pixel pixel)
    {
        IUserAction action = new MouseWheelUp(pixel);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates a scroll-wheel-down action at the specified pixel.</summary>
    /// <param name="pixel">The pixel position of the pointer.</param>
    public void ScrollWheelDown(Pixel pixel)
    {
        IUserAction action = new MouseWheelDown(pixel);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates pressing the Shift key.</summary>
    public void PressShift() => PressKey(StandardKeys.Shift);

    /// <summary>Simulates releasing the Shift key.</summary>
    public void ReleaseShift() => ReleaseKey(StandardKeys.Shift);

    /// <summary>Simulates pressing the Control key.</summary>
    public void PressCtrl() => PressKey(StandardKeys.Control);

    /// <summary>Simulates releasing the Control key.</summary>
    public void ReleaseCtrl() => ReleaseKey(StandardKeys.Control);

    /// <summary>Simulates pressing the Alt key.</summary>
    public void PressAlt() => PressKey(StandardKeys.Alt);

    /// <summary>Simulates releasing the Alt key.</summary>
    public void ReleaseAlt() => ReleaseKey(StandardKeys.Alt);

    /// <summary>Simulates tapping the right arrow key.</summary>
    public void TapRightArrow() => TapKey(StandardKeys.Right);

    /// <summary>Simulates tapping the left arrow key.</summary>
    public void TapLeftArrow() => TapKey(StandardKeys.Left);

    /// <summary>Simulates tapping the up arrow key.</summary>
    public void TapUpArrow() => TapKey(StandardKeys.Up);

    /// <summary>Simulates tapping the down arrow key.</summary>
    public void TapDownArrow() => TapKey(StandardKeys.Down);

    /// <summary>Simulates pressing the specified key.</summary>
    /// <param name="key">The key to press.</param>
    public void PressKey(Key key)
    {
        IUserAction action = new KeyDown(key);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates releasing the specified key.</summary>
    /// <param name="key">The key to release.</param>
    public void ReleaseKey(Key key)
    {
        IUserAction action = new KeyUp(key);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates tapping the specified key.</summary>
    /// <param name="key">The key to tap.</param>
    public void TapKey(Key key)
    {
        PressKey(key);
        ReleaseKey(key);
    }

    /// <summary>Simulates moving the pointer to the specified pixel.</summary>
    /// <param name="px">The pixel position to move to.</param>
    public void MoveMouse(Pixel px)
    {
        IUserAction action = new MouseMove(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates pressing the left mouse button at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void LeftMouseDown(Pixel px)
    {
        IUserAction action = new LeftMouseDown(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates releasing the left mouse button at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void LeftMouseUp(Pixel px)
    {
        IUserAction action = new LeftMouseUp(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates a left click at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void LeftClick(Pixel px)
    {
        LeftMouseDown(px);
        LeftMouseUp(px);
    }

    /// <summary>Simulates dragging from the first pixel to the second using the left mouse button.</summary>
    /// <param name="px1">The starting pixel position.</param>
    /// <param name="px2">The ending pixel position.</param>
    public void LeftClickDrag(Pixel px1, Pixel px2)
    {
        LeftMouseDown(px1);
        MoveMouse(px2);
        LeftMouseUp(px2);
    }

    /// <summary>Simulates pressing the right mouse button at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void RightMouseDown(Pixel px)
    {
        IUserAction action = new RightMouseDown(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates releasing the right mouse button at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void RightMouseUp(Pixel px)
    {
        IUserAction action = new RightMouseUp(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates a right click at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void RightClick(Pixel px)
    {
        RightMouseDown(px);
        RightMouseUp(px);
    }

    /// <summary>Simulates dragging from the first pixel to the second using the right mouse button.</summary>
    /// <param name="px1">The starting pixel position.</param>
    /// <param name="px2">The ending pixel position.</param>
    public void RightClickDrag(Pixel px1, Pixel px2)
    {
        RightMouseDown(px1);
        MoveMouse(px2);
        RightMouseUp(px2);
    }

    /// <summary>Simulates pressing the middle mouse button at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void MiddleMouseDown(Pixel px)
    {
        IUserAction action = new MiddleMouseDown(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates releasing the middle mouse button at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void MiddleMouseUp(Pixel px)
    {
        IUserAction action = new MiddleMouseUp(px);
        UserInputProcessor.Process(action);
    }

    /// <summary>Simulates a middle click at the specified pixel.</summary>
    /// <param name="px">The pixel position of the pointer.</param>
    public void MiddleClick(Pixel px)
    {
        MiddleMouseDown(px);
        MiddleMouseUp(px);
    }

    /// <summary>Simulates dragging from the first pixel to the second using the middle mouse button.</summary>
    /// <param name="px1">The starting pixel position.</param>
    /// <param name="px2">The ending pixel position.</param>
    public void MiddleClickDrag(Pixel px1, Pixel px2)
    {
        MiddleMouseDown(px1);
        MoveMouse(px2);
        MiddleMouseUp(px2);
    }

    /// <summary>Ignores cursor changes because the mock has no graphical cursor.</summary>
    /// <param name="cursor">The requested cursor.</param>
    public void SetCursor(Cursor cursor)
    {
    }
}
