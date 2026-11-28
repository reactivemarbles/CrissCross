// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Allows a plot to be panned by dragging with a mouse button.</summary>
/// <param name="button">The mouse button that initiates the drag.</param>
public class MouseDragPan(MouseButton button) : IUserActionResponse
{
    /// <summary>The minimum pointer movement required to begin panning.</summary>
    private const double MinimumDragDistance = 5;

    /// <summary>The pointer position where the drag began.</summary>
    private Pixel _mouseDownPixel = Pixel.NaN;

    /// <summary>The axis limits captured when the drag began.</summary>
    private MultiAxisLimits? _rememberedLimits;

    /// <summary>Gets which mouse button to watch for click-drag events.</summary>
    public MouseButton MouseButton { get; } = button;

    /// <summary>Gets vertical panning is disabled if any of these keys are pressed.</summary>
    public List<Key> KeysThatLockY { get; } = [StandardKeys.Control];

    /// <summary>Gets horizontal panning is disabled if any of these keys are pressed.</summary>
    public List<Key> KeysThatLockX { get; } = [StandardKeys.Shift];

    /// <summary>Gets or sets vertical panning is disabled when this is set.</summary>
    public bool LockY { get; set; }

    /// <summary>Gets or sets horizontal panning is disabled when this is set.</summary>
    public bool LockX { get; set; }

    /// <summary>Gets or sets whether interactions over one axis affect all parallel axes.</summary>
    public bool ChangeOpposingAxesTogether { get; set; }

    /// <summary>Resets the active drag state.</summary>
    /// <param name="plotControl">The plot control whose input state is reset.</param>
    public void ResetState(IPlotControl plotControl)
    {
        _rememberedLimits = null;
        _mouseDownPixel = Pixel.NaN;
    }

    /// <summary>Processes mouse actions for a drag-pan interaction.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (userActions is IMouseButtonAction mouseDownAction
            && mouseDownAction.Button == MouseButton
            && mouseDownAction.IsPressed)
        {
            return StartDrag(plotControl, mouseDownAction);
        }

        if (userActions is IMouseButtonAction mouseUpAction
            && mouseUpAction.Button == MouseButton
            && !mouseUpAction.IsPressed
            && _rememberedLimits is { } rememberedLimits)
        {
            return EndDrag(mouseUpAction, rememberedLimits, keys);
        }

        return userActions is IMouseAction mouseAction && _rememberedLimits is { } currentLimits
            ? ContinueDrag(mouseAction, currentLimits, keys)
            : ResponseInfo.NoActionRequired;
    }

    /// <summary>Captures the starting pointer position and axis limits.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="mouseDownAction">The mouse-down action.</param>
    /// <returns>The response for the mouse-down action.</returns>
    private ResponseInfo StartDrag(IPlotControl plotControl, IMouseButtonAction mouseDownAction)
    {
        Plot? plot = plotControl.GetPlotAtPixel(mouseDownAction.Pixel);
        if (plot is not null)
        {
            _rememberedLimits = new(plot);
            _mouseDownPixel = mouseDownAction.Pixel;
        }

        return ResponseInfo.NoActionRequired;
    }

    /// <summary>Restores the starting limits and completes the drag.</summary>
    /// <param name="mouseUpAction">The mouse-up action.</param>
    /// <param name="rememberedLimits">The axis limits captured at drag start.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>A response indicating that the plot should refresh.</returns>
    private ResponseInfo EndDrag(IMouseButtonAction mouseUpAction, MultiAxisLimits rememberedLimits, KeyboardState keys)
    {
        rememberedLimits.Recall();
        Plot plot = rememberedLimits.Plot;
        _rememberedLimits = null;
        ApplyToPlot(plot, _mouseDownPixel, mouseUpAction.Pixel, keys);
        return ResponseInfo.Refresh;
    }

    /// <summary>Updates the pan while the pointer is moving during a drag.</summary>
    /// <param name="mouseAction">The current mouse action.</param>
    /// <param name="rememberedLimits">The axis limits captured at drag start.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response for the in-progress drag.</returns>
    private ResponseInfo ContinueDrag(IMouseAction mouseAction, MultiAxisLimits rememberedLimits, KeyboardState keys)
    {
        double horizontalDragDistance = Math.Abs(mouseAction.Pixel.X - _mouseDownPixel.X);
        double verticalDragDistance = Math.Abs(mouseAction.Pixel.Y - _mouseDownPixel.Y);
        double maximumDragDistance = Math.Max(horizontalDragDistance, verticalDragDistance);
        if (maximumDragDistance < MinimumDragDistance)
        {
            return ResponseInfo.NoActionRequired;
        }

        rememberedLimits.Recall();
        ApplyToPlot(rememberedLimits.Plot, _mouseDownPixel, mouseAction.Pixel, keys);
        return new() { RefreshNeeded = true, IsPrimary = true };
    }

    /// <summary>Applies axis locks and pans the plot between two pixel positions.</summary>
    /// <param name="plot">The plot to pan.</param>
    /// <param name="startPixel">The initial pointer position.</param>
    /// <param name="endPixel">The current pointer position.</param>
    /// <param name="keys">The current keyboard state.</param>
    private void ApplyToPlot(Plot plot, Pixel startPixel, Pixel endPixel, KeyboardState keys)
    {
        if (LockX || keys.IsPressed(KeysThatLockX))
        {
            endPixel.X = startPixel.X;
        }

        if (LockY || keys.IsPressed(KeysThatLockY))
        {
            endPixel.Y = startPixel.Y;
        }

        MouseAxisManipulation.DragPan(plot, startPixel, endPixel, ChangeOpposingAxesTogether);
    }
}
