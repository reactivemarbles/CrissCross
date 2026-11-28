// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Invokes a callback when the specified mouse button is clicked without dragging.</summary>
/// <param name="button">The mouse button to monitor.</param>
/// <param name="action">The callback to invoke with the plot control and click position.</param>
public class SingleClickResponse(MouseButton button, Action<IPlotControl, Pixel> action) : IUserActionResponse
{
    /// <summary>The maximum movement allowed for a click, in pixels.</summary>
    private const double MaximumClickDragDistance = 5;

    /// <summary>Gets the location of the previous mouse-down event.</summary>
    private Pixel _mouseDownPixel = Pixel.NaN;

    /// <summary>Gets which mouse button to watch for single-click events.</summary>
    public MouseButton MouseButton { get; } = button;

    /// <summary>Gets this action is invoked when a single-click occurs.</summary>
    public Action<IPlotControl, Pixel> ResponseAction { get; } = action;

    /// <summary>Clears the stored mouse-down location.</summary>
    /// <param name="plotControl">The plot control associated with this response.</param>
    public void ResetState(IPlotControl plotControl) => _mouseDownPixel = Pixel.NaN;

    /// <summary>Processes mouse actions and invokes the callback for a click without dragging.</summary>
    /// <param name="plotControl">The plot control that received the action.</param>
    /// <param name="userActions">The user action to process.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response describing whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (userActions is IMouseButtonAction buttonAction && buttonAction.Button == MouseButton)
        {
            Plot? plot = plotControl.GetPlotAtPixel(buttonAction.Pixel);
            if (plot is null)
            {
                return ResponseInfo.NoActionRequired;
            }

            if (buttonAction.IsPressed)
            {
                _mouseDownPixel = buttonAction.Pixel;
                return ResponseInfo.NoActionRequired;
            }

            if (double.IsNaN(_mouseDownPixel.X))
            {
                return ResponseInfo.NoActionRequired;
            }

            // do not respond to mouse dragging
            double horizontalDistance = Math.Abs(_mouseDownPixel.X - buttonAction.Pixel.X);
            double verticalDistance = Math.Abs(_mouseDownPixel.Y - buttonAction.Pixel.Y);
            double dragDistance = Math.Max(horizontalDistance, verticalDistance);
            if (dragDistance >= MaximumClickDragDistance)
            {
                _mouseDownPixel = Pixel.NaN;
                return ResponseInfo.NoActionRequired;
            }

            ResponseAction.Invoke(plotControl, buttonAction.Pixel);
            _mouseDownPixel = Pixel.NaN;

            return new() { RefreshNeeded = true };
        }

        return ResponseInfo.NoActionRequired;
    }
}
