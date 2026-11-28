// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Draws a zoom rectangle that becomes the plot field of view when released.</summary>
/// <param name="button">The mouse button that initiates the zoom rectangle.</param>
public class MouseDragZoomRectangle(MouseButton button) : IUserActionResponse
{
    /// <summary>The minimum pointer movement required to display the zoom rectangle.</summary>
    private const double MinimumDragDistance = 5;

    /// <summary>The pointer position where the drag began.</summary>
    private Pixel _mouseDownPixel = Pixel.NaN;

    /// <summary>The plot currently being zoomed.</summary>
    private Plot? _plotBeingDragged;

    /// <summary>Gets or sets the button that starts a zoom rectangle when <see cref="SecondaryKey"/> is held.</summary>
    public MouseButton SecondaryMouseButton { get; set; } = StandardMouseButtons.Left;

    /// <summary>Gets or sets the key required with <see cref="SecondaryMouseButton"/> to start a zoom rectangle.</summary>
    public Key SecondaryKey { get; set; } = StandardKeys.Alt;

    /// <summary>Gets or sets the key that locks horizontal axis limits.</summary>
    public Key HorizontalLockKey { get; set; } = StandardKeys.Control;

    /// <summary>Gets or sets the key that locks vertical axis limits.</summary>
    public Key VerticalLockKey { get; set; } = StandardKeys.Shift;

    /// <summary>Gets the mouse button used to start the zoom rectangle directly.</summary>
    private MouseButton MouseButton { get; } = button;

    /// <summary>Resets the active zoom-rectangle state.</summary>
    /// <param name="plotControl">The plot control whose input state is reset.</param>
    public void ResetState(IPlotControl plotControl)
    {
        _mouseDownPixel = Pixel.NaN;

        if (_plotBeingDragged is not null)
        {
            _plotBeingDragged.ZoomRectangle.IsVisible = false;
        }

        _plotBeingDragged = null;
    }

    /// <summary>Processes mouse actions for a zoom-rectangle interaction.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (TryStartZoomRectangle(plotControl, userActions, keys, out ResponseInfo startResponse))
        {
            return startResponse;
        }

        if (_mouseDownPixel == Pixel.NaN || _plotBeingDragged is not { } plot)
        {
            return ResponseInfo.NoActionRequired;
        }

        if (userActions is IMouseAction mouseMoveAction && userActions is not IMouseButtonAction)
        {
            return UpdateZoomRectangle(plot, mouseMoveAction, keys);
        }

        return userActions is IMouseButtonAction mouseUpAction && !mouseUpAction.IsPressed
            ? CompleteZoomRectangle(plot)
            : ResponseInfo.NoActionRequired;
    }

    /// <summary>Starts a zoom rectangle for a configured mouse-button action.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <param name="response">The start response when a drag begins.</param>
    /// <returns><see langword="true"/> when the action starts a zoom rectangle.</returns>
    private bool TryStartZoomRectangle(IPlotControl plotControl, IUserAction userActions, KeyboardState keys, out ResponseInfo response)
    {
        response = ResponseInfo.NoActionRequired;
        if (userActions is not IMouseButtonAction buttonAction || !buttonAction.IsPressed)
        {
            return false;
        }

        bool isPrimaryButton = buttonAction.Button == MouseButton;
        bool isSecondaryButtonAndKey = buttonAction.Button == SecondaryMouseButton && keys.IsPressed(SecondaryKey);
        if (!isPrimaryButton && !isSecondaryButtonAndKey)
        {
            return false;
        }

        Plot? plotUnderMouse = plotControl.GetPlotAtPixel(buttonAction.Pixel);
        if (plotUnderMouse is null)
        {
            return false;
        }

        _mouseDownPixel = buttonAction.Pixel;
        _plotBeingDragged = plotUnderMouse;
        response = new() { IsPrimary = isSecondaryButtonAndKey };
        return true;
    }

    /// <summary>Updates the visible zoom rectangle during a mouse drag.</summary>
    /// <param name="plot">The plot being zoomed.</param>
    /// <param name="mouseMoveAction">The current mouse action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response for the in-progress drag.</returns>
    private ResponseInfo UpdateZoomRectangle(Plot plot, IMouseAction mouseMoveAction, KeyboardState keys)
    {
        double horizontalDragDistance = Math.Abs(_mouseDownPixel.X - mouseMoveAction.Pixel.X);
        double verticalDragDistance = Math.Abs(_mouseDownPixel.Y - mouseMoveAction.Pixel.Y);
        if (horizontalDragDistance < MinimumDragDistance && verticalDragDistance < MinimumDragDistance)
        {
            bool wasVisible = plot.ZoomRectangle.IsVisible;
            plot.ZoomRectangle.IsVisible = false;
            return new() { RefreshNeeded = wasVisible, IsPrimary = false };
        }

        plot.ZoomRectangle.IsVisible = true;
        plot.ZoomRectangle.HorizontalSpan = keys.IsPressed(VerticalLockKey);
        plot.ZoomRectangle.VerticalSpan = keys.IsPressed(HorizontalLockKey);
        MouseAxisManipulation.PlaceZoomRectangle(plot, _mouseDownPixel, mouseMoveAction.Pixel);
        return new() { RefreshNeeded = true, IsPrimary = true };
    }

    /// <summary>Applies the visible zoom rectangle to the plot axes.</summary>
    /// <param name="plot">The plot being zoomed.</param>
    /// <returns>The response for the mouse-up action.</returns>
    private ResponseInfo CompleteZoomRectangle(Plot plot)
    {
        _mouseDownPixel = Pixel.NaN;
        if (!plot.ZoomRectangle.IsVisible)
        {
            return ResponseInfo.NoActionRequired;
        }

        foreach (IAxis axis in plot.Axes.GetAxes())
        {
            if (axis is IXAxis horizontalAxis)
            {
                plot.ZoomRectangle.Apply(horizontalAxis);
            }
        }

        foreach (IAxis axis in plot.Axes.GetAxes())
        {
            if (axis is IYAxis verticalAxis)
            {
                plot.ZoomRectangle.Apply(verticalAxis);
            }
        }

        plot.ZoomRectangle.IsVisible = false;
        return ResponseInfo.Refresh;
    }
}
