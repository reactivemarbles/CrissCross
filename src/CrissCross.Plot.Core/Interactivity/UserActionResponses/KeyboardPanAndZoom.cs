// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Interactivity.UserActions;

namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Provides keyboard-based panning and zooming for a plot.</summary>
public class KeyboardPanAndZoom : IUserActionResponse
{
    /// <summary>The latest pointer position used to select the plot.</summary>
    private Pixel _mousePixel = Pixel.Zero;

    /// <summary>Gets or sets the key used to pan left.</summary>
    public Key PanLeftKey { get; set; } = StandardKeys.Left;

    /// <summary>Gets or sets the key used to pan right.</summary>
    public Key PanRightKey { get; set; } = StandardKeys.Right;

    /// <summary>Gets or sets the key used to pan down.</summary>
    public Key PanDownKey { get; set; } = StandardKeys.Down;

    /// <summary>Gets or sets the key used to pan up.</summary>
    public Key PanUpKey { get; set; } = StandardKeys.Up;

    /// <summary>Gets or sets the modifier key that switches arrow actions to zoom.</summary>
    public Key ZoomModifierKey { get; set; } = StandardKeys.Control;

    /// <summary>Gets or sets the modifier key that selects larger pan steps.</summary>
    public Key LargeStepKey { get; set; } = StandardKeys.Shift;

    /// <summary>Gets or sets the modifier key that selects single-pixel pan steps.</summary>
    public Key FineStepKey { get; set; } = StandardKeys.Alt;

    /// <summary>Gets or sets the standard pan distance in pixels.</summary>
    public float StepDistance { get; set; } = 20;

    /// <summary>Gets or sets the large pan distance in pixels.</summary>
    public float LargeStepDistance { get; set; } = 100;

    /// <summary>Gets or sets the fine pan distance in pixels.</summary>
    public float FineStepDistance { get; set; } = 1;

    /// <summary>Gets or sets the zoom factor for zooming in.</summary>
    public double DeltaZoomIn { get; set; } = 0.85F;

    /// <summary>Gets or sets the zoom factor for zooming out.</summary>
    public double DeltaZoomOut { get; set; } = 1.15F;

    /// <summary>Resets state when the plot control is reset.</summary>
    /// <param name="plotControl">The plot control that was reset.</param>
    public void ResetState(IPlotControl plotControl) { }

    /// <summary>Processes pointer and key actions to pan or zoom a plot.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (userActions is MouseMove mouseMove)
        {
            _mousePixel = mouseMove.Pixel;
            return ResponseInfo.NoActionRequired;
        }

        return userActions is KeyDown keyDown
            ? HandleKeyDown(plotControl, keyDown, keys)
            : ResponseInfo.NoActionRequired;
    }

    /// <summary>Zooms the plot axes by the supplied horizontal and vertical factors.</summary>
    /// <param name="plot">The plot to zoom.</param>
    /// <param name="deltaX">The horizontal zoom factor.</param>
    /// <param name="deltaY">The vertical zoom factor.</param>
    /// <returns>A response indicating that the plot should refresh.</returns>
    private static ResponseInfo ApplyZoom(Plot plot, double deltaX, double deltaY)
    {
        plot.Axes.Zoom(deltaX, deltaY);
        return ResponseInfo.Refresh;
    }

    /// <summary>Pans the plot axes by the supplied horizontal and vertical offsets.</summary>
    /// <param name="plot">The plot to pan.</param>
    /// <param name="deltaX">The horizontal pan distance.</param>
    /// <param name="deltaY">The vertical pan distance.</param>
    /// <returns>A response indicating that the plot should refresh.</returns>
    private static ResponseInfo ApplyPan(Plot plot, float deltaX, float deltaY)
    {
        PixelOffset pixelOffset = new(deltaX, deltaY);
        plot.Axes.Pan(pixelOffset);
        return ResponseInfo.Refresh;
    }

    /// <summary>Handles a key-down action when the pointer is over a plot.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="keyDown">The key-down action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    private ResponseInfo HandleKeyDown(IPlotControl plotControl, KeyDown keyDown, KeyboardState keys)
    {
        Plot? plot = plotControl.GetPlotAtPixel(_mousePixel);
        if (plot is null)
        {
            return ResponseInfo.NoActionRequired;
        }

        return keys.IsPressed(ZoomModifierKey)
            ? ApplyZoomForKey(plot, keyDown.Key)
            : ApplyPanForKey(plot, keyDown.Key, keys);
    }

    /// <summary>Applies zoom for an arrow key when the zoom modifier is held.</summary>
    /// <param name="plot">The plot to zoom.</param>
    /// <param name="key">The pressed key.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    private ResponseInfo ApplyZoomForKey(Plot plot, Key key)
    {
        if (key == PanLeftKey)
        {
            return ApplyZoom(plot, DeltaZoomIn, 1);
        }

        if (key == PanRightKey)
        {
            return ApplyZoom(plot, DeltaZoomOut, 1);
        }

        if (key == PanDownKey)
        {
            return ApplyZoom(plot, 1, DeltaZoomIn);
        }

        return key == PanUpKey
            ? ApplyZoom(plot, 1, DeltaZoomOut)
            : ResponseInfo.NoActionRequired;
    }

    /// <summary>Applies a pan action for an arrow key.</summary>
    /// <param name="plot">The plot to pan.</param>
    /// <param name="key">The pressed key.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    private ResponseInfo ApplyPanForKey(Plot plot, Key key, KeyboardState keys)
    {
        float delta = StepDistance;
        if (keys.IsPressed(LargeStepKey))
        {
            delta = LargeStepDistance;
        }

        if (keys.IsPressed(FineStepKey))
        {
            delta = FineStepDistance;
        }

        if (key == PanLeftKey)
        {
            return ApplyPan(plot, -delta, 0);
        }

        if (key == PanRightKey)
        {
            return ApplyPan(plot, delta, 0);
        }

        if (key == PanDownKey)
        {
            return ApplyPan(plot, 0, -delta);
        }

        return key == PanUpKey
            ? ApplyPan(plot, 0, delta)
            : ResponseInfo.NoActionRequired;
    }
}
