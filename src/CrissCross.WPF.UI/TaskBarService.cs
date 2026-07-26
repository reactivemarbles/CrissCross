// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Allows you to manage the animations of the window icon in the taskbar.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class TaskBarService : ITaskBarService
{
    /// <summary>Stores the _progressStates value.</summary>
    private readonly Dictionary<IntPtr, TaskBarProgressState> _progressStates = [];

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <inheritdoc />
    public virtual TaskBarProgressState GetState(IntPtr windowHandle) => !_progressStates.TryGetValue(windowHandle, out var progressState)
        ? TaskBarProgressState.None
        : progressState;

    /// <inheritdoc />
    public virtual TaskBarProgressState GetState(Window? window)
    {
        if (window is null)
        {
            return TaskBarProgressState.None;
        }

        var windowHandle = new WindowInteropHelper(window).Handle;

        return !_progressStates.TryGetValue(windowHandle, out var progressState)
            ? TaskBarProgressState.None
            : progressState;
    }

    /// <inheritdoc />
    public virtual bool SetState(Window? window, TaskBarProgressState taskBarProgressState) => window is null ? false : TaskBarProgress.SetState(window, taskBarProgressState);

    /// <inheritdoc />
    public virtual bool SetState(IntPtr windowHandle, TaskBarProgressState taskBarProgressState) =>
        TaskBarProgress.SetState(windowHandle, taskBarProgressState);

    /// <inheritdoc />
    public virtual bool SetValue(Window? window, TaskBarProgressState taskBarProgressState, int current, int total) =>
        window is not null && TaskBarProgress.SetValue(window, taskBarProgressState, current, total);

    /// <inheritdoc />
    public virtual bool SetValue(Window? window, int current, int total)
    {
        if (window is null)
        {
            return false;
        }

        var windowHandle = new WindowInteropHelper(window).Handle;

        return !_progressStates.TryGetValue(windowHandle, out var progressState)
            ? TaskBarProgress.SetValue(window, TaskBarProgressState.Normal, current, total)
            : TaskBarProgress.SetValue(window, progressState, current, total);
    }

    /// <inheritdoc/>
    public virtual bool SetValue(
        IntPtr windowHandle,
        TaskBarProgressState taskBarProgressState,
        int current,
        int total) => TaskBarProgress.SetValue(windowHandle, taskBarProgressState, current, total);

    /// <inheritdoc />
    public virtual bool SetValue(IntPtr windowHandle, int current, int max) => !_progressStates.TryGetValue(windowHandle, out var progressState)
        ? TaskBarProgress.SetValue(windowHandle, TaskBarProgressState.Normal, current, max)
        : TaskBarProgress.SetValue(windowHandle, progressState, current, max);
}
