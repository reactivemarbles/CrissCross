// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Routes pointer actions to interactive plottable handles.</summary>
/// <param name="button">The mouse button used to engage with handles.</param>
public class MouseInteractWithPlottables(MouseButton button) : IUserActionResponse
{
    /// <summary>The handle currently under the pointer.</summary>
    private InteractiveHandle? _hoveredHandle;

    /// <summary>The handle currently being moved.</summary>
    private InteractiveHandle? _interactingHandle;

    /// <summary>The plot that owns the handle being moved.</summary>
    private Plot? _interactingPlot;

    /// <summary>Gets which mouse button to watch to engage with interactive plottables.</summary>
    public MouseButton MouseButton { get; } = button;

    /// <summary>Releases any active handle interaction.</summary>
    /// <param name="plotControl">The plot control whose input state is reset.</param>
    public void ResetState(IPlotControl plotControl)
    {
        _interactingHandle?.Parent.ReleaseHandle(_interactingHandle);
        _interactingHandle = null;
        _interactingPlot = null;
    }

    /// <summary>Processes pointer actions for interactive plottables.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (TryReleaseHandle(userActions, out ResponseInfo releaseResponse))
        {
            return releaseResponse;
        }

        if (TryMoveHandle(userActions, out ResponseInfo moveResponse))
        {
            return moveResponse;
        }

        if (TryStartHandle(plotControl, userActions, out ResponseInfo startResponse))
        {
            return startResponse;
        }

        UpdateHoveredHandle(plotControl, userActions);
        return ResponseInfo.NoActionRequired;
    }

    /// <summary>Releases the active handle when its initiating button is released.</summary>
    /// <param name="userActions">The current user action.</param>
    /// <param name="response">The response generated for a release action.</param>
    /// <returns><see langword="true"/> when the active handle was released.</returns>
    private bool TryReleaseHandle(IUserAction userActions, out ResponseInfo response)
    {
        response = ResponseInfo.NoActionRequired;
        if (_interactingHandle is not { } handle
            || _interactingPlot is not { } plot
            || userActions is not IMouseButtonAction buttonAction
            || buttonAction.Button != MouseButton
            || buttonAction.IsPressed)
        {
            return false;
        }

        handle.Parent.ReleaseHandle(handle);
        plot.HandleReleased?.Invoke(this, handle);
        _interactingHandle = null;
        _interactingPlot = null;
        response = new() { RefreshNeeded = true, IsPrimary = false };
        return true;
    }

    /// <summary>Moves the active handle to the current pointer position.</summary>
    /// <param name="userActions">The current user action.</param>
    /// <param name="response">The response generated for a move action.</param>
    /// <returns><see langword="true"/> when an active handle was moved.</returns>
    private bool TryMoveHandle(IUserAction userActions, out ResponseInfo response)
    {
        response = ResponseInfo.NoActionRequired;
        if (_interactingHandle is not { } handle
            || _interactingPlot is not { } plot
            || userActions is not IMouseAction mouseAction)
        {
            return false;
        }

        Coordinates coordinates = plot.GetCoordinates(mouseAction.Pixel);
        handle.Parent.MoveHandle(handle, coordinates);
        plot.HandleMoved?.Invoke(this, handle);
        response = ResponseInfo.Refresh;
        return true;
    }

    /// <summary>Starts an interaction when the configured mouse button presses a handle.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="response">The response generated for a press action.</param>
    /// <returns><see langword="true"/> when an interactive handle was pressed.</returns>
    private bool TryStartHandle(IPlotControl plotControl, IUserAction userActions, out ResponseInfo response)
    {
        response = ResponseInfo.NoActionRequired;
        if (userActions is not IMouseButtonAction buttonAction
            || buttonAction.Button != MouseButton
            || !buttonAction.IsPressed)
        {
            return false;
        }

        Plot? plot = plotControl.GetPlotAtPixel(buttonAction.Pixel);
        if (plot is null)
        {
            return false;
        }

        InteractiveHandle? handle = plot.GetInteractiveHandle(buttonAction.Pixel.X, buttonAction.Pixel.Y);
        if (handle is null)
        {
            return false;
        }

        handle.Parent.PressHandle(handle);
        plot.HandlePressed?.Invoke(this, handle);
        _interactingHandle = handle;
        _interactingPlot = plot;
        response = new() { RefreshNeeded = true, IsPrimary = true };
        return true;
    }

    /// <summary>Updates the hovered handle and pointer cursor when necessary.</summary>
    /// <param name="plotControl">The plot control containing the pointer.</param>
    /// <param name="userActions">The current user action.</param>
    private void UpdateHoveredHandle(IPlotControl plotControl, IUserAction userActions)
    {
        if (userActions is not IMouseAction mouseAction)
        {
            return;
        }

        Plot? plot = plotControl.GetPlotAtPixel(mouseAction.Pixel);
        if (plot is null)
        {
            return;
        }

        InteractiveHandle? hoveredHandle = plot.GetInteractiveHandle(mouseAction.Pixel.X, mouseAction.Pixel.Y);
        if (hoveredHandle == _hoveredHandle)
        {
            return;
        }

        plot.HandleHoverChanged?.Invoke(this, hoveredHandle);
        plot.PlotControl?.SetCursor(hoveredHandle?.Cursor ?? Cursor.Arrow);
        _hoveredHandle = hoveredHandle;
    }
}
