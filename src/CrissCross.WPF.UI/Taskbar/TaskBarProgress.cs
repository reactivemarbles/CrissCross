// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

namespace CrissCross.WPF.UI.TaskBar;

/// <summary>Allows to change the status of the displayed notification in the application icon on the TaskBar.</summary>
public static class TaskBarProgress
{
    /// <summary>Provides the maximum percentage-based taskbar progress value.</summary>
    private const int MaximumProgressValue = 100;

    /// <summary>Allows to change the status of the progress bar in the task bar.</summary>
    /// <param name="window">Window to manipulate.</param>
    /// <param name="taskBarProgressState">State of the progress indicator.</param>
    /// <returns>A bool.</returns>
    public static bool SetState(Window? window, TaskBarProgressState taskBarProgressState)
    {
        if (window is null)
        {
            return false;
        }

        if (window.IsLoaded)
        {
            return SetState(new WindowInteropHelper(window).Handle, taskBarProgressState);
        }

        window.Loaded += (_, _) => _ = SetState(new WindowInteropHelper(window).Handle, taskBarProgressState);

        return true;
    }

    /// <summary>Allows to change the status of the progress bar in the task bar.</summary>
    /// <exception cref="InvalidOperationException">Taskbar functions not available.</exception>
    /// <param name="windowHandle">Window handle.</param>
    /// <param name="taskBarProgressState">State of the progress indicator.</param>
    /// <returns>A bool.</returns>
    public static bool SetState(IntPtr windowHandle, TaskBarProgressState taskBarProgressState)
    {
        if (!IsSupported())
        {
            throw new InvalidOperationException("Taskbar functions not available.");
        }

        return UnsafeNativeMethods.SetTaskbarState(windowHandle, UnsafeReflection.Cast(taskBarProgressState));
    }

    /// <summary>Allows to change the fill of the task bar.</summary>
    /// <param name="window">Window to manipulate.</param>
    /// <param name="taskBarProgressState">Progress sate to set.</param>
    /// <param name="current">Current value to display.</param>
    /// <returns>A bool.</returns>
    public static bool SetValue(Window window, TaskBarProgressState taskBarProgressState, int current)
    {
        if (current > MaximumProgressValue)
        {
            current = MaximumProgressValue;
        }

        if (current < 0)
        {
            current = 0;
        }

        return SetValue(window, taskBarProgressState, current, MaximumProgressValue);
    }

    /// <summary>Allows to change the fill of the task bar.</summary>
    /// <param name="window">Window to manipulate.</param>
    /// <param name="taskBarProgressState">Progress sate to set.</param>
    /// <param name="current">Current value to display.</param>
    /// <param name="total">Total number for division.</param>
    /// <returns>A bool.</returns>
    public static bool SetValue(Window? window, TaskBarProgressState taskBarProgressState, int current, int total)
    {
        if (window is null)
        {
            return false;
        }

        if (window.IsLoaded)
        {
            return SetValue(new WindowInteropHelper(window).Handle, taskBarProgressState, current, total);
        }

        window.Loaded += (_, _) =>
            _ = SetValue(new WindowInteropHelper(window).Handle, taskBarProgressState, current, total);

        return false;
    }

    /// <summary>Allows to change the fill of the task bar.</summary>
    /// <param name="windowHandle">Window handle.</param>
    /// <param name="taskBarProgressState">Progress sate to set.</param>
    /// <param name="current">Current value to display.</param>
    /// <returns>A bool.</returns>
    public static bool SetValue(IntPtr windowHandle, TaskBarProgressState taskBarProgressState, int current)
    {
        if (current > MaximumProgressValue)
        {
            current = MaximumProgressValue;
        }

        if (current < 0)
        {
            current = 0;
        }

        return SetValue(windowHandle, taskBarProgressState, current, MaximumProgressValue);
    }

    /// <summary>Allows to change the fill of the task bar.</summary>
    /// <exception cref="Exception">Taskbar functions not available.</exception>
    /// <param name="windowHandle">Window handle.</param>
    /// <param name="taskBarProgressState">Progress sate to set.</param>
    /// <param name="current">Current value to display.</param>
    /// <param name="total">Total number for division.</param>
    /// <returns>A bool.</returns>
    public static bool SetValue(IntPtr windowHandle, TaskBarProgressState taskBarProgressState, int current, int total)
    {
        if (!IsSupported())
        {
            throw new InvalidOperationException("Taskbar functions not available.");
        }

        return UnsafeNativeMethods.SetTaskbarValue(
            windowHandle,
            UnsafeReflection.Cast(taskBarProgressState),
            current,
            total);
    }

    /// <summary>Gets a value indicating whether the current operating system supports task bar manipulation.</summary>
    /// <returns>The result.</returns>
    private static bool IsSupported() => Win32.Utilities.IsOSWindows7OrNewer;
}
