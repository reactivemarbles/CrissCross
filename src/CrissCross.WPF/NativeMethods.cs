// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace CrissCross.WPF;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
}
