// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Invokes an action when the configured mouse button is double-clicked.</summary>
/// <param name="button">The mouse button to monitor.</param>
/// <param name="action">The action to invoke on a double-click.</param>
public class DoubleClickResponse(MouseButton button, Action<IPlotControl, Pixel> action) : IUserActionResponse
{
    /// <summary>
    /// Consecutive clicks are only considered a double-click if the time between the first
    /// click mouse down and second click mouse up does not exceed this value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained as a public field for compatibility with existing callers.")]
    public TimeSpan MaximumTimeBetweenClicks = TimeSpan.FromMilliseconds(DefaultMaximumTimeBetweenClicksMilliseconds);

    /// <summary>The default maximum interval between double-clicks, in milliseconds.</summary>
    private const int DefaultMaximumTimeBetweenClicksMilliseconds = 500;

    /// <summary>The timestamp of the most recent mouse-down event.</summary>
    private DateTime _latestMouseDownTime = DateTime.MinValue;

    /// <summary>The timestamp of the prior mouse-down event.</summary>
    private DateTime _previousMouseDownTime = DateTime.MinValue;

    /// <summary>Gets the mouse button monitored for double-clicks.</summary>
    public MouseButton MouseButton { get; } = button;

    /// <summary>Gets the action invoked when a double-click occurs.</summary>
    public Action<IPlotControl, Pixel> ResponseAction { get; } = action;

    /// <summary>Resets state when the plot control is reset.</summary>
    /// <param name="plotControl">The plot control that was reset.</param>
    public void ResetState(IPlotControl plotControl)
    {
        // Don't reset mouse click times because resets may occur after MouseUp events
        // and double-clicks are unlikely to last long enough to persist through legitimate resets.
    }

    /// <summary>Processes an input action and invokes the response for a double-click.</summary>
    /// <param name="plotControl">The plot control receiving the action.</param>
    /// <param name="userActions">The current user action.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response indicating whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (userActions is IMouseButtonAction mouseAction && mouseAction.Button == MouseButton)
        {
            if (mouseAction.IsPressed)
            {
                _previousMouseDownTime = _latestMouseDownTime;
                _latestMouseDownTime = mouseAction.DateTime;
                return ResponseInfo.NoActionRequired;
            }

            TimeSpan timeSinceFirstMouseDown = mouseAction.DateTime - _previousMouseDownTime;
            if (timeSinceFirstMouseDown < MaximumTimeBetweenClicks)
            {
                ResponseAction.Invoke(plotControl, mouseAction.Pixel);
                _latestMouseDownTime = DateTime.MinValue;
                return ResponseInfo.Refresh;
            }
        }

        return ResponseInfo.NoActionRequired;
    }
}
