// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Defines native top-level window operations.</summary>
[ComVisible(true)]
[Guid("987df77b-db06-4d77-8f8a-86a9c3bb90b9")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsWindowProvider
{
    /// <summary>Changes the native window presentation.</summary>
    /// <param name="state">The native visual state.</param>
    void SetVisualState(int state);

    /// <summary>Closes the window.</summary>
    void Close();

    /// <summary>Waits for the process to become idle.</summary>
    /// <param name="milliseconds">The maximum wait.</param>
    /// <returns>Whether the process became idle.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool WaitForInputIdle(int milliseconds);

    /// <summary>Gets whether the window supports maximizing.</summary>
    /// <returns>The native window value.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadCanMaximize();

    /// <summary>Gets whether the window supports minimizing.</summary>
    /// <returns>The native window value.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadCanMinimize();

    /// <summary>Gets whether the window is modal.</summary>
    /// <returns>The native window value.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadIsModal();

    /// <summary>Gets the native visual state.</summary>
    /// <returns>The native window value.</returns>
    int GetWindowVisualState();

    /// <summary>Gets the native interaction state.</summary>
    /// <returns>The native window value.</returns>
    int GetWindowInteractionState();

    /// <summary>Gets whether the window stays above other windows.</summary>
    /// <returns>The native window value.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadIsTopmost();
}
