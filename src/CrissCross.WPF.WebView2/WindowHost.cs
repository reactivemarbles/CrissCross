// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CrissCross.WPF;

/// <summary>Window Host.</summary>
/// <typeparam name="TWindow">The type of the window to host.</typeparam>
/// <seealso cref="HwndHost" />
public class WindowHost<TWindow> : HwndHost
    where TWindow : Window, new()
{
    /// <summary>Stores the gWLSTYLE value.</summary>
    private const int GWLSTYLE = -0x10;

    /// <summary>Stores the wSCHILD value.</summary>
    private const uint WSCHILD = 0x40000000U;

    /// <summary>Initializes a new instance of the <see cref="WindowHost{TWindow}" /> class.</summary>
    /// <param name="name">The name.</param>
    public WindowHost(string name)
        : this(name, null) { }

    /// <summary>Initializes a new instance of the <see cref="WindowHost{TWindow}" /> class.</summary>
    /// <param name="name">The name.</param>
    /// <param name="window">The window.</param>
    public WindowHost(string name, TWindow? window)
    {
        Window = window ??= new();
        Window.Name = name;
        Window.ResizeMode = ResizeMode.NoResize;
        Window.WindowStyle = WindowStyle.None;
        Window.ShowInTaskbar = false;
        Window.AllowsTransparency = true;
        Window.BorderBrush = Brushes.Transparent;
        Window.BorderThickness = new(0);
        Window.Background = Brushes.Transparent;
        Window.Loaded += HideFromAltTab;
        Window.Show();
    }

    /// <summary>Gets the window handle.</summary>
    /// <value>
    /// The window handle.
    /// </value>
    public IntPtr WindowHandle { get; private set; }

    /// <summary>Gets the window.</summary>
    /// <value>
    /// The window.
    /// </value>
    public TWindow Window { get; }

    /// <summary>Closes this instance.</summary>
    public void Close()
    {
        Window.Close();
        DestroyWindowCore(new HandleRef(Window, IntPtr.Zero));
    }

    /// <summary>When overridden in a derived class, creates the window to be hosted.</summary>
    /// <param name="hwndParent">The window handle of the parent window.</param>
    /// <returns>
    /// The handle to the child Win32 window to create.
    /// </returns>
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        HandleRef href = default;

        if (WindowHandle != IntPtr.Zero)
        {
            _ = NativeMethods.SetWindowLong(WindowHandle, GWLSTYLE, WSCHILD);
            _ = NativeMethods.SetParent(WindowHandle, hwndParent.Handle);
            href = new(this, WindowHandle);
        }

        return href;
    }

    /// <summary>When overridden in a derived class, destroys the hosted window.</summary>
    /// <param name="hwnd">A structure that contains the window handle.</param>
    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        if (WindowHandle == hwnd.Handle)
        {
            return;
        }

        _ = NativeMethods.SetParent(WindowHandle, hwnd.Handle);
    }

    /// <summary>Runs the hide From Alt Tab operation.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The routed event arguments.</param>
    private void HideFromAltTab(object sender, RoutedEventArgs e)
    {
        Window.Loaded -= HideFromAltTab;
        WindowHandle = new WindowInteropHelper(Window).Handle;
        NativeMethods.HideFromAltTab(WindowHandle);
    }
}
