// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Zooms a plot in response to mouse-wheel actions.</summary>
/// <param name="horizontalLockKey">The key that prevents horizontal zoom.</param>
/// <param name="verticalLockKey">The key that prevents vertical zoom.</param>
public class MouseWheelZoom(Key horizontalLockKey, Key verticalLockKey) : IUserActionResponse
{
    /// <summary>Gets or sets whether only the axis under the pointer is zoomed.</summary>
    public bool ZoomAxisUnderMouse { get; set; } = true;

    /// <summary>Gets or sets whether parallel axes zoom together when an axis is targeted.</summary>
    public bool LockParallelAxes { get; set; }

    /// <summary>Gets or sets the fraction of the axis range used to zoom in or out.</summary>
    public double ZoomFraction { get; set; } = 0.15;

    /// <summary>Gets the key that locks horizontal zoom.</summary>
    private Key LockHorizontalKey { get; } = horizontalLockKey;

    /// <summary>Gets the key that locks vertical zoom.</summary>
    private Key LockVerticalKey { get; } = verticalLockKey;

    /// <summary>Gets the zoom-in fraction.</summary>
    private double ZoomInFraction => 1 + ZoomFraction;

    /// <summary>Gets the zoom-out fraction.</summary>
    private double ZoomOutFraction => 1 / ZoomInFraction;

    /// <summary>Resets state when the plot control is reset.</summary>
    /// <param name="plotControl">The plot control whose input state is reset.</param>
    public void ResetState(IPlotControl plotControl) { }

    /// <summary>Processes mouse-wheel actions to zoom the plot.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (userActions is UserActions.MouseWheelUp wheelUpAction)
        {
            return Zoom(plotControl, wheelUpAction.Pixel, ZoomInFraction, keys);
        }

        return userActions is UserActions.MouseWheelDown wheelDownAction
            ? Zoom(plotControl, wheelDownAction.Pixel, ZoomOutFraction, keys)
            : ResponseInfo.NoActionRequired;
    }

    /// <summary>Applies a wheel zoom at the specified pointer position.</summary>
    /// <param name="plotControl">The plot control containing the plot.</param>
    /// <param name="pixel">The pointer position.</param>
    /// <param name="zoomFraction">The zoom fraction to apply.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    private ResponseInfo Zoom(IPlotControl plotControl, Pixel pixel, double zoomFraction, KeyboardState keys)
    {
        Plot? plot = plotControl.GetPlotAtPixel(pixel);
        if (plot is null)
        {
            return ResponseInfo.NoActionRequired;
        }

        double horizontalFraction = keys.IsPressed(LockHorizontalKey) ? 1 : zoomFraction;
        double verticalFraction = keys.IsPressed(LockVerticalKey) ? 1 : zoomFraction;
        MouseAxisManipulation.MouseWheelZoom(plot, horizontalFraction, verticalFraction, pixel, LockParallelAxes);
        return new() { RefreshNeeded = true };
    }
}
