// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Describes a class that has logic to watch user actions and manipulate the plot accordingly.</summary>
public interface IUserActionResponse
{
    /// <summary>Performs the action on the plot and indicates what should happen next.</summary>
    /// <param name="plotControl">The plot control affected by this action.</param>
    /// <param name="userActions">The user action being processed.</param>
    /// <param name="keys">The current keyboard state.</param>
    /// <returns>Information about the actions required after execution.</returns>
    ResponseInfo Execute(IPlotControl plotControl, IUserAction userActions, KeyboardState keys);

    /// <summary>
    /// Reset state to what it was when the action response was first created.
    /// This method is called when the user processor resets (e.g., when the 
    /// control loses and re-gains focus or is disabled and re-enabled) and
    /// is designed to reset responses like mouse-drag events that accumulate state.
    /// </summary>
    /// <param name="plotControl">The plot control whose response state is reset.</param>
    void ResetState(IPlotControl plotControl);
}
