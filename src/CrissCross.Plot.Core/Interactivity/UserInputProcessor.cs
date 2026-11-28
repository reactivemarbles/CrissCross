// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot.Interactivity;

/// <summary>
/// This class collects user inputs and performs responses to manipulate a Plot.
/// Custom user input actions may be supplied, and the list of responses can be 
/// modified to achieve total control over interaction behavior.
/// </summary>
public class UserInputProcessor
{
    /// <summary>Tracks which keys are currently pressed.</summary>
    [SuppressMessage("StyleSharp", "SST1401", Justification = "KeyState is an existing public field; replacing it with a property would break binary compatibility.")]
    public readonly KeyboardState KeyState = new();

    /// <summary>
    /// Gets the ordered user action responses.
    /// Callers may modify this list to change default interaction behavior or add custom responses.
    /// </summary>
    [SuppressMessage("StyleSharp", "SST1401", Justification = "UserActionResponses is an existing public field; replacing it with a property would break binary compatibility.")]
    [SuppressMessage("StyleSharp", "SST2322", Justification = "The mutable response list is the supported public customization surface for interaction behavior.")]
    public readonly List<IUserActionResponse> UserActionResponses = [];

    /// <summary>Gets or sets the action invoked when the plot control loses focus.</summary>
    [SuppressMessage("StyleSharp", "SST1401", Justification = "LostFocusAction is an existing public field; replacing it with a property would break binary compatibility.")]
    public Action<IPlotControl> LostFocusAction = static (plotControl) =>
    {
        ResetState(plotControl);
        plotControl.Refresh();
    };

    /// <summary>Tracks the response that currently owns an active input sequence.</summary>
    private IUserActionResponse? _primaryResponse;

    /// <summary>Initializes a new instance of the <see cref="UserInputProcessor"/> class.</summary>
    /// <param name="plotControl">The plot control to manipulate.</param>
    public UserInputProcessor(IPlotControl plotControl)
    {
        PlotControl = plotControl;
        Reset();
        IsEnabled = true;
    }

    /// <summary>Gets the plot this input processor will act on.</summary>
    public IPlotControl PlotControl { get; }

    /// <summary>Gets or sets whether new events are processed. Enabling this disables the older <c>Interaction</c> system.</summary>
    public bool IsEnabled
    {
        get => field; set
        {
            field = value;
            if (value)
            {
                return;
            }

            ProcessLostFocus();
        }
    }

    /// <summary>Resets all user action responses and the keyboard state.</summary>
    /// <param name="plotControl">The plot control whose input state is reset.</param>
    public static void ResetState(IPlotControl plotControl)
    {
        foreach (var response in plotControl.UserInputProcessor.UserActionResponses)
        {
            response.ResetState(plotControl);
        }

        plotControl.UserInputProcessor.KeyState.Reset();
    }

    /// <summary>Creates the default user action responses used at construction or reset.</summary>
    /// <returns>The default user action responses.</returns>
    public static List<IUserActionResponse> DefaultUserResponses() =>
    [

        // drag events
        new UserActionResponses.MouseInteractWithPlottables(StandardMouseButtons.Left),
        new UserActionResponses.MouseDragZoomRectangle(StandardMouseButtons.Middle),
        new UserActionResponses.MouseDragPan(StandardMouseButtons.Left),
        new UserActionResponses.MouseDragZoom(StandardMouseButtons.Right),

        // click events
        new UserActionResponses.MouseWheelZoom(StandardKeys.Shift, StandardKeys.Control),
        new UserActionResponses.SingleClickAutoscale(StandardMouseButtons.Middle),
        new UserActionResponses.SingleClickContextMenu(StandardMouseButtons.Right),
        new UserActionResponses.DoubleClickBenchmark(StandardMouseButtons.Left),

        // keypress events
        new UserActionResponses.KeyboardPanAndZoom(),
        new UserActionResponses.KeyboardAutoscale(StandardKeys.A),
    ];

    /// <summary>Enable processing of user input events.</summary>
    public void Enable() => IsEnabled = true;

    /// <summary>Disable processing of user input events. Effectively makes this control non-interactive.</summary>
    public void Disable() => IsEnabled = false;

    /// <summary>Removes all user input responses of the specified type.</summary>
    /// <typeparam name="T">The response type to remove.</typeparam>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2307",
        Justification = "The generic type selects response types for removal; changing the signature would break callers and using a runtime Type parameter would weaken type safety.")]
    public void RemoveAll<T>()
        where T : IUserActionResponse => _ = UserActionResponses.RemoveAll(static x => x is T);

    /// <summary>Resets the user input responses to use the default interactivity settings.</summary>
    public void Reset()
    {
        KeyState.Reset();
        UserActionResponses.Clear();
        UserActionResponses.AddRange(DefaultUserResponses());
    }

    /// <summary>Enables or disables the default double-left-click benchmark behavior.</summary>
    /// <param name="enable">Whether to enable the benchmark behavior.</param>
    public void DoubleLeftClickBenchmark(bool enable)
    {
        RemoveAll<UserActionResponses.DoubleClickBenchmark>();

        if (!enable)
        {
            return;
        }

        var button = StandardMouseButtons.Left;
        var response = new UserActionResponses.DoubleClickBenchmark(button);
        UserActionResponses.Add(response);
    }

    /// <summary>Enables the default left-click-drag pan behavior on both axes.</summary>
    /// <param name="enable">Whether to enable the pan behavior.</param>
    public void LeftClickDragPan(bool enable) => LeftClickDragPan(enable, true, true);

    /// <summary>Enables the default left-click-drag pan behavior.</summary>
    /// <param name="enable">Whether to enable the pan behavior.</param>
    /// <param name="horizontal">Whether horizontal panning is enabled.</param>
    public void LeftClickDragPan(bool enable, bool horizontal) => LeftClickDragPan(enable, horizontal, true);

    /// <summary>Enables the default left-click-drag pan behavior.</summary>
    /// <param name="enable">Whether to enable the pan behavior.</param>
    /// <param name="horizontal">Whether horizontal panning is enabled.</param>
    /// <param name="vertical">Whether vertical panning is enabled.</param>
    public void LeftClickDragPan(bool enable, bool horizontal, bool vertical)
    {
        RemoveAll<UserActionResponses.MouseDragPan>();

        if (!enable)
        {
            return;
        }

        var button = StandardMouseButtons.Left;
        var response = new UserActionResponses.MouseDragPan(button) { LockX = !horizontal, LockY = !vertical, };
        UserActionResponses.Add(response);
    }

    /// <summary>Enables the default right-click-drag zoom behavior on both axes.</summary>
    /// <param name="enable">Whether to enable the zoom behavior.</param>
    public void RightClickDragZoom(bool enable) => RightClickDragZoom(enable, true, true);

    /// <summary>Enables the default right-click-drag zoom behavior.</summary>
    /// <param name="enable">Whether to enable the zoom behavior.</param>
    /// <param name="horizontal">Whether horizontal zooming is enabled.</param>
    public void RightClickDragZoom(bool enable, bool horizontal) => RightClickDragZoom(enable, horizontal, true);

    /// <summary>Enables the default right-click-drag zoom behavior.</summary>
    /// <param name="enable">Whether to enable the zoom behavior.</param>
    /// <param name="horizontal">Whether horizontal zooming is enabled.</param>
    /// <param name="vertical">Whether vertical zooming is enabled.</param>
    public void RightClickDragZoom(bool enable, bool horizontal, bool vertical)
    {
        RemoveAll<UserActionResponses.MouseDragZoom>();

        if (!enable)
        {
            return;
        }

        var button = StandardMouseButtons.Right;
        var response = new UserActionResponses.MouseDragZoom(button) { LockX = !horizontal, LockY = !vertical, };
        UserActionResponses.Add(response);
    }

    /// <summary>Processes a user input and refreshes the plot when needed.</summary>
    /// <param name="userAction">The user action to process.</param>
    public void Process(IUserAction userAction)
    {
        if (!IsEnabled)
        {
            return;
        }

        UpdateKeyboardState(userAction);

        bool refreshNeeded = ExecuteUserInputResponses(userAction);

        if (!refreshNeeded)
        {
            return;
        }

        PlotControl.Refresh();
    }

    /// <summary>Processes the loss of focus using the configured action.</summary>
    public void ProcessLostFocus() => LostFocusAction.Invoke(PlotControl);

    /// <summary>Updates the tracked keyboard state for a key action.</summary>
    /// <param name="userAction">The user action to inspect.</param>
    private void UpdateKeyboardState(IUserAction userAction)
    {
        if (userAction is UserActions.KeyDown keyDown)
        {
            KeyState.Add(keyDown.Key);
        }

        if (userAction is not UserActions.KeyUp keyUp)
        {
            return;
        }

        KeyState.Remove(keyUp.Key);
    }

    /// <summary>Executes responses for an input action while synchronized with rendering.</summary>
    /// <param name="userAction">The user action being processed.</param>
    /// <returns><see langword="true"/> when the plot should be refreshed.</returns>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1901",
        Justification = "Plot.Sync is the shared monitor used by rendering; replacing it with a private lock would allow input updates during a render.")]
    private bool ExecuteUserInputResponses(IUserAction userAction)
    {
        bool refreshNeeded = false;

        // lock onto the sync object to prevent actions from being applied while a render is in progress
        lock (PlotControl.Plot.Sync)
        {
            foreach (IUserActionResponse response in UserActionResponses)
            {
                if (_primaryResponse is not null && _primaryResponse != response)
                {
                    continue;
                }

                ResponseInfo info = response.Execute(PlotControl, userAction, KeyState);
                if (info.RefreshNeeded)
                {
                    refreshNeeded = true;
                }

                if (_primaryResponse is null && info.IsPrimary)
                {
                    _primaryResponse = response;
                }

                if (_primaryResponse == response && !info.IsPrimary)
                {
                    _primaryResponse = null;
                }
            }
        }

        return refreshNeeded;
    }
}
