// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Interactivity.UserActions;

namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Invokes a callback when a specified key is pressed.</summary>
/// <param name="key">The key that triggers the callback.</param>
/// <param name="action">The callback to invoke with the plot control and last mouse position.</param>
public class KeyPressResponse(Key key, Action<IPlotControl, Pixel> action) : IUserActionResponse
{
    /// <summary>The most recently observed mouse position.</summary>
    private Pixel _mousePixel = Pixel.Zero;

    /// <summary>Gets the key that triggers the callback.</summary>
    private Key Key { get; } = key;

    /// <summary>Gets the callback invoked when the key is pressed.</summary>
    private Action<IPlotControl, Pixel> ResponseAction { get; } = action;

    /// <summary>Resets any state maintained by this response.</summary>
    /// <param name="plotControl">The plot control associated with the response.</param>
    public void ResetState(IPlotControl plotControl) { }

    /// <summary>Processes a user action and invokes the callback when its key is pressed.</summary>
    /// <param name="plotControl">The plot control associated with the action.</param>
    /// <param name="userActions">The user action to process.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>The response describing whether the plot should refresh.</returns>
    public ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys)
    {
        if (userActions is MouseMove mouseMove)
        {
            _mousePixel = mouseMove.Pixel;
            return ResponseInfo.NoActionRequired;
        }

        if (userActions is KeyDown keyDownAction && keyDownAction.Key == Key)
        {
            ResponseAction.Invoke(plotControl, _mousePixel);
            return ResponseInfo.Refresh;
        }

        return ResponseInfo.NoActionRequired;
    }
}
