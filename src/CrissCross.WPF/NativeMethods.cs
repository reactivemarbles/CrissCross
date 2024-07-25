// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace CrissCross.WPF;

internal static class NativeMethods
{
    private const int GWLEXSTYLE = -0x14;
    private const uint WSEXAPPWINDOW = 0x00040000u;
    private const uint WSEXTOOLWINDOW = 0x00000080u;

    [DllImport("user32.dll")]
    internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    internal static void HideFromAltTab(IntPtr hWnd) =>
        _ = NativeMethods.SetWindowLong(hWnd, GWLEXSTYLE, (NativeMethods.GetWindowLong(hWnd, GWLEXSTYLE) | WSEXTOOLWINDOW) & ~WSEXAPPWINDOW);
}
