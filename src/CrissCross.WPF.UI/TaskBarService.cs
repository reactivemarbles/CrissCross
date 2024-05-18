// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>
/// Allows you to manage the animations of the window icon in the taskbar.
/// </summary>
public class TaskBarService : ITaskBarService
{
    private readonly Dictionary<IntPtr, TaskBarProgressState> _progressStates = [];

    /// <inheritdoc />
    public virtual TaskBarProgressState GetState(IntPtr hWnd)
    {
        if (!_progressStates.TryGetValue(hWnd, out var progressState))
        {
            return TaskBarProgressState.None;
        }

        return progressState;
    }

    /// <inheritdoc />
    public virtual TaskBarProgressState GetState(Window? window)
    {
        if (window is null)
        {
            return TaskBarProgressState.None;
        }

        var windowHandle = new WindowInteropHelper(window).Handle;

        if (!_progressStates.TryGetValue(windowHandle, out var progressState))
        {
            return TaskBarProgressState.None;
        }

        return progressState;
    }

    /// <inheritdoc />
    public virtual bool SetState(Window? window, TaskBarProgressState taskBarProgressState)
    {
        if (window is null)
        {
            return false;
        }

        return TaskBarProgress.SetState(window, taskBarProgressState);
    }

    /// <inheritdoc />
    public virtual bool SetValue(
        Window? window,
        TaskBarProgressState taskBarProgressState,
        int current,
        int total)
    {
        if (window is null)
        {
            return false;
        }

        return TaskBarProgress.SetValue(window, taskBarProgressState, current, total);
    }

    /// <inheritdoc />
    public virtual bool SetValue(Window? window, int current, int total)
    {
        if (window == null)
        {
            return false;
        }

        var windowHandle = new WindowInteropHelper(window).Handle;

        if (!_progressStates.TryGetValue(windowHandle, out var progressState))
        {
            return TaskBarProgress.SetValue(window, TaskBarProgressState.Normal, current, total);
        }

        return TaskBarProgress.SetValue(window, progressState, current, total);
    }

    /// <inheritdoc />
    public virtual bool SetState(IntPtr hWnd, TaskBarProgressState taskBarProgressState) =>
        TaskBarProgress.SetState(hWnd, taskBarProgressState);

    /// <inheritdoc/>
    public virtual bool SetValue(
        IntPtr hWnd,
        TaskBarProgressState taskBarProgressState,
        int current,
        int total) => TaskBarProgress.SetValue(hWnd, taskBarProgressState, current, total);

    /// <inheritdoc />
    public virtual bool SetValue(IntPtr hWnd, int current, int max)
    {
        if (!_progressStates.TryGetValue(hWnd, out var progressState))
        {
            return TaskBarProgress.SetValue(hWnd, TaskBarProgressState.Normal, current, max);
        }

        return TaskBarProgress.SetValue(hWnd, progressState, current, max);
    }
}
