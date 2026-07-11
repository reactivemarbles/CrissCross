// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>Represents a contract with a service that provides methods for manipulating the taskbar.</summary>
public interface ITaskBarService
{
    /// <summary>Gets taskbar state of the selected window handle.</summary>
    /// <param name="windowHandle">The windowHandle value.</param>
    /// <returns>The current state of system TaskBar.</returns>
    TaskBarProgressState GetState(IntPtr windowHandle);

    /// <summary>Gets taskbar state of the selected window.</summary>
    /// <param name="window">The window value.</param>
    /// <returns>The current state of system TaskBar.</returns>
    TaskBarProgressState GetState(Window? window);

    /// <summary>Sets taskbar state of the selected window handle.</summary>
    /// <param name="windowHandle">The windowHandle value.</param>
    /// <param name="taskBarProgressState">The taskBarProgressState value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetState(IntPtr windowHandle, TaskBarProgressState taskBarProgressState);

    /// <summary>Sets taskbar value of the selected window handle.</summary>
    /// <param name="windowHandle">The windowHandle value.</param>
    /// <param name="taskBarProgressState">The taskBarProgressState value.</param>
    /// <param name="current">The current value.</param>
    /// <param name="total">The total value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetValue(IntPtr windowHandle, TaskBarProgressState taskBarProgressState, int current, int total);

    /// <summary>Sets taskbar value of the selected window handle.</summary>
    /// <param name="windowHandle">The windowHandle value.</param>
    /// <param name="current">The current value.</param>
    /// <param name="max">The max value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetValue(IntPtr windowHandle, int current, int max);

    /// <summary>Sets taskbar state of the selected window.</summary>
    /// <param name="window">The window value.</param>
    /// <param name="taskBarProgressState">The taskBarProgressState value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetState(Window? window, TaskBarProgressState taskBarProgressState);

    /// <summary>Sets taskbar value of the selected window.</summary>
    /// <param name="window">The window value.</param>
    /// <param name="taskBarProgressState">The taskBarProgressState value.</param>
    /// <param name="current">The current value.</param>
    /// <param name="total">The total value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetValue(Window? window, TaskBarProgressState taskBarProgressState, int current, int total);

    /// <summary>Sets taskbar value of the selected window.</summary>
    /// <param name="window">The window value.</param>
    /// <param name="current">The current value.</param>
    /// <param name="total">The total value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetValue(Window? window, int current, int total);
}
