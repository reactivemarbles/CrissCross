// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>Allows you to manage the animations of the window icon in the taskbar.</summary>
public class TaskBarService : ITaskBarService
{
    /// <summary>Stores the _progressStates value.</summary>
    private readonly Dictionary<IntPtr, TaskBarProgressState> _progressStates = [];

    /// <inheritdoc />
    public virtual TaskBarProgressState GetState(IntPtr windowHandle)
    {
        return !_progressStates.TryGetValue(windowHandle, out var progressState)
            ? TaskBarProgressState.None
            : progressState;
    }

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
    public virtual bool SetState(Window? window, TaskBarProgressState taskBarProgressState)
    {
        return window is null ? false : TaskBarProgress.SetState(window, taskBarProgressState);
    }

    /// <inheritdoc />
    public virtual bool SetState(IntPtr windowHandle, TaskBarProgressState taskBarProgressState) =>
        TaskBarProgress.SetState(windowHandle, taskBarProgressState);

    /// <inheritdoc />
    public virtual bool SetValue(Window? window, TaskBarProgressState taskBarProgressState, int current, int total)
    {
        return window is null ? false : TaskBarProgress.SetValue(window, taskBarProgressState, current, total);
    }

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
    public virtual bool SetValue(IntPtr windowHandle, int current, int max)
    {
        return !_progressStates.TryGetValue(windowHandle, out var progressState)
            ? TaskBarProgress.SetValue(windowHandle, TaskBarProgressState.Normal, current, max)
            : TaskBarProgress.SetValue(windowHandle, progressState, current, max);
    }
}
