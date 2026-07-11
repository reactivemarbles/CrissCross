// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace CrissCross.WPF;

/// <summary>Provides user32 interop helpers for hosted WPF windows.</summary>
#if NET7_0_OR_GREATER
internal static partial class NativeMethods
#else
internal static class NativeMethods
#endif
{
    /// <summary>Specifies the extended window style index.</summary>
    private const int GWLEXSTYLE = -0x14;

    /// <summary>Specifies the application-window extended style.</summary>
    private const uint WSEXAPPWINDOW = 0x00040000u;

    /// <summary>Specifies the tool-window extended style.</summary>
    private const uint WSEXTOOLWINDOW = 0x00000080u;

#if NET7_0_OR_GREATER
    /// <summary>Changes the parent window of the specified child window.</summary>
    /// <param name="childWindowHandle">The child window handle.</param>
    /// <param name="newParentWindowHandle">The new parent window handle.</param>
    /// <returns>The previous parent window handle.</returns>
    [LibraryImport("user32.dll", EntryPoint = "SetParent", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial IntPtr SetParent(IntPtr childWindowHandle, IntPtr newParentWindowHandle);

    /// <summary>Changes an attribute of the specified window.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="index">The window attribute index.</param>
    /// <param name="newValue">The new attribute value.</param>
    /// <returns>The previous attribute value.</returns>
    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int SetWindowLong(IntPtr windowHandle, int index, uint newValue);

    /// <summary>Gets an attribute of the specified window.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="index">The window attribute index.</param>
    /// <returns>The current attribute value.</returns>
    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial uint GetWindowLong(IntPtr windowHandle, int index);
#else
    /// <summary>Changes the parent window of the specified child window.</summary>
    /// <param name="childWindowHandle">The child window handle.</param>
    /// <param name="newParentWindowHandle">The new parent window handle.</param>
    /// <returns>The previous parent window handle.</returns>
    [DllImport("user32.dll", EntryPoint = "SetParent", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern IntPtr SetParent(IntPtr childWindowHandle, IntPtr newParentWindowHandle);

    /// <summary>Changes an attribute of the specified window.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="index">The window attribute index.</param>
    /// <param name="newValue">The new attribute value.</param>
    /// <returns>The previous attribute value.</returns>
    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern int SetWindowLong(IntPtr windowHandle, int index, uint newValue);

    /// <summary>Gets an attribute of the specified window.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="index">The window attribute index.</param>
    /// <returns>The current attribute value.</returns>
    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern uint GetWindowLong(IntPtr windowHandle, int index);
#endif

    /// <summary>Removes the specified window from the Alt+Tab application list.</summary>
    /// <param name="windowHandle">The window handle.</param>
    internal static void HideFromAltTab(IntPtr windowHandle) =>
        _ = SetWindowLong(windowHandle, GWLEXSTYLE, (GetWindowLong(windowHandle, GWLEXSTYLE) | WSEXTOOLWINDOW) & ~WSEXAPPWINDOW);
}
