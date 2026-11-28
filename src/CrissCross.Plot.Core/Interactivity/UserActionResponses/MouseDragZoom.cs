// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Allows a plot to be zoomed by dragging with a mouse button.</summary>
/// <param name="button">The mouse button that initiates the drag.</param>
public class MouseDragZoom(MouseButton button) : IUserActionResponse
{
    /// <summary>The tolerance for treating a sensitivity factor as its default value.</summary>
    private const double SensitivityTolerance = 1e-12;

    /// <summary>The pointer position where the drag began.</summary>
    private Pixel _mouseDownPixel = Pixel.NaN;

    /// <summary>The axis limits captured when the drag began.</summary>
    private MultiAxisLimits? _rememberedLimits;

    /// <summary>Gets vertical panning is disabled if any of these keys are pressed.</summary>
    public List<Key> KeysThatLockY { get; } = [StandardKeys.Control];

    /// <summary>Gets horizontal panning is disabled if any of these keys are pressed.</summary>
    public List<Key> KeysThatLockX { get; } = [StandardKeys.Shift];

    /// <summary>Gets or sets vertical panning is disabled when this is set.</summary>
    public bool LockY { get; set; }

    /// <summary>Gets or sets horizontal panning is disabled when this is set.</summary>
    public bool LockX { get; set; }

    /// <summary>Gets or sets the horizontal zoom sensitivity.</summary>
    public double SensitivityX { get; set; } = 1.0;

    /// <summary>Gets or sets the vertical zoom sensitivity.</summary>
    public double SensitivityY { get; set; } = 1.0;

    /// <summary>Gets or sets whether interactions over one axis affect all parallel axes.</summary>
    public bool ChangeOpposingAxesTogether { get; set; }

    /// <summary>Gets the mouse button used to initiate the drag.</summary>
    private MouseButton MouseButton { get; } = button;

    /// <summary>Resets the active drag state.</summary>
    /// <param name="plotControl">The plot control whose input state is reset.</param>
    public void ResetState(IPlotControl plotControl)
    {
        _rememberedLimits = null;
        _mouseDownPixel = Pixel.NaN;
    }

    /// <summary>Processes mouse actions for a drag-zoom interaction.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys) =>
        userActions is IMouseButtonAction mouseDownAction
            && mouseDownAction.Button == MouseButton
            && mouseDownAction.IsPressed
            ? StartDrag(plotControl, mouseDownAction)
            : ProcessDragAction(userActions, keys);

    /// <summary>Processes mouse-up and move actions while a drag is active.</summary>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response for the active drag.</returns>
    private ResponseInfo ProcessDragAction(IUserAction userActions, KeyboardState keys)
    {
        if (_mouseDownPixel == Pixel.NaN)
        {
            return ResponseInfo.NoActionRequired;
        }

        if (userActions is IMouseButtonAction mouseUpAction
            && mouseUpAction.Button == MouseButton
            && !mouseUpAction.IsPressed
            && _rememberedLimits is { } rememberedLimits)
        {
            return EndDrag(mouseUpAction, rememberedLimits, keys);
        }

        return userActions is IMouseAction mouseMoveAction && _rememberedLimits is { } currentLimits
            ? ContinueDrag(mouseMoveAction, currentLimits, keys)
            : new() { IsPrimary = true };
    }

    /// <summary>Captures the starting pointer position and axis limits.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="mouseDownAction">The mouse-down action.</param>
    /// <returns>The response for the mouse-down action.</returns>
    private ResponseInfo StartDrag(IPlotControl plotControl, IMouseButtonAction mouseDownAction)
    {
        _mouseDownPixel = mouseDownAction.Pixel;
        Plot? plot = plotControl.GetPlotAtPixel(mouseDownAction.Pixel);
        if (plot is null)
        {
            return new() { IsPrimary = true };
        }

        _rememberedLimits = new(plot);
        return new() { IsPrimary = false };
    }

    /// <summary>Restores the starting limits and completes the drag.</summary>
    /// <param name="mouseUpAction">The mouse-up action.</param>
    /// <param name="rememberedLimits">The axis limits captured at drag start.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>A response indicating that the plot should refresh.</returns>
    private ResponseInfo EndDrag(IMouseButtonAction mouseUpAction, MultiAxisLimits rememberedLimits, KeyboardState keys)
    {
        rememberedLimits.Recall();
        ApplyToPlot(rememberedLimits.Plot, _mouseDownPixel, mouseUpAction.Pixel, keys);
        _mouseDownPixel = Pixel.NaN;
        return ResponseInfo.Refresh;
    }

    /// <summary>Updates the zoom while the pointer moves during a drag.</summary>
    /// <param name="mouseMoveAction">The current mouse action.</param>
    /// <param name="rememberedLimits">The axis limits captured at drag start.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response for the in-progress drag.</returns>
    private ResponseInfo ContinueDrag(IMouseAction mouseMoveAction, MultiAxisLimits rememberedLimits, KeyboardState keys)
    {
        rememberedLimits.Recall();
        ApplyToPlot(rememberedLimits.Plot, _mouseDownPixel, mouseMoveAction.Pixel, keys);
        return new() { RefreshNeeded = true, IsPrimary = true };
    }

    /// <summary>Applies axis locks and sensitivity before zooming the plot.</summary>
    /// <param name="plot">The plot to zoom.</param>
    /// <param name="startPixel">The initial pointer position.</param>
    /// <param name="endPixel">The current pointer position.</param>
    /// <param name="keys">The current keyboard state.</param>
    private void ApplyToPlot(Plot plot, Pixel startPixel, Pixel endPixel, KeyboardState keys)
    {
        float startX = startPixel.X;
        float startY = startPixel.Y;
        float endX = endPixel.X;
        float endY = endPixel.Y;

        if (LockX || keys.IsPressed(KeysThatLockX))
        {
            endX = startX;
        }

        if (LockY || keys.IsPressed(KeysThatLockY))
        {
            endY = startY;
        }

        if (!(Math.Abs(SensitivityX - 1.0) <= SensitivityTolerance))
        {
            endX = startX + ((endX - startX) * (float)SensitivityX);
        }

        if (!(Math.Abs(SensitivityY - 1.0) <= SensitivityTolerance))
        {
            endY = startY + ((endY - startY) * (float)SensitivityY);
        }

        endPixel = new(endX, endY);
        MouseAxisManipulation.DragZoom(plot, startPixel, endPixel, ChangeOpposingAxesTogether);
    }
}
