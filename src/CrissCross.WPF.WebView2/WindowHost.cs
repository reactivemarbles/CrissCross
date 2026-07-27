// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF;
#else
namespace CrissCross.WPF;
#endif

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

    /// <summary>Stores the non-owning window handle wrapper.</summary>
    private readonly WindowSafeHandle _windowHandle = new();

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

    /// <summary>Gets a safe, non-owning wrapper around the hosted window handle.</summary>
    /// <value>
    /// The window handle.
    /// </value>
    public SafeHandle WindowHandle => _windowHandle;

    /// <summary>Gets the window.</summary>
    /// <value>
    /// The window.
    /// </value>
    public TWindow Window { get; }

    /// <summary>Closes this instance.</summary>
    public void Close()
    {
        Window.Close();
        DestroyWindowCore(new(Window, IntPtr.Zero));
        _windowHandle.Dispose();
    }

    /// <summary>When overridden in a derived class, creates the window to be hosted.</summary>
    /// <param name="hwndParent">The window handle of the parent window.</param>
    /// <returns>
    /// The handle to the child Win32 window to create.
    /// </returns>
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        HandleRef href = default;

        if (!_windowHandle.IsInvalid)
        {
            _ = NativeMethods.SetWindowLong(_windowHandle, GWLSTYLE, WSCHILD);
            _ = NativeMethods.SetParent(_windowHandle, hwndParent.Handle);
            href = _windowHandle.CreateHandleRef(this);
        }

        return href;
    }

    /// <summary>When overridden in a derived class, destroys the hosted window.</summary>
    /// <param name="hwnd">A structure that contains the window handle.</param>
    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        if (_windowHandle.Matches(hwnd.Handle))
        {
            return;
        }

        _ = NativeMethods.SetParent(_windowHandle, hwnd.Handle);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _windowHandle.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Runs the hide From Alt Tab operation.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The routed event arguments.</param>
    private void HideFromAltTab(object sender, RoutedEventArgs e)
    {
        Window.Loaded -= HideFromAltTab;
        _windowHandle.Initialize(new WindowInteropHelper(Window).Handle);
        NativeMethods.HideFromAltTab(_windowHandle);
    }

    /// <summary>Provides safe, non-owning access to a WPF-owned window handle.</summary>
    private sealed class WindowSafeHandle : SafeHandle
    {
        /// <summary>Initializes a new instance of the <see cref="WindowSafeHandle" /> class.</summary>
        public WindowSafeHandle()
            : base(IntPtr.Zero, ownsHandle: false)
        {
        }

        /// <inheritdoc />
        public override bool IsInvalid => handle == IntPtr.Zero || handle == new IntPtr(-1);

        /// <summary>Sets the wrapped handle after WPF creates the hosted window.</summary>
        /// <param name="windowHandle">The WPF-owned window handle.</param>
        public void Initialize(IntPtr windowHandle) => SetHandle(windowHandle);

        /// <summary>Creates a handle reference for the WPF hosting infrastructure.</summary>
        /// <param name="wrapper">The managed object that owns the returned reference.</param>
        /// <returns>The handle reference used exclusively by WPF.</returns>
        public HandleRef CreateHandleRef(object wrapper) => new(wrapper, handle);

        /// <summary>Determines whether this wrapper contains the specified handle.</summary>
        /// <param name="windowHandle">The handle to compare.</param>
        /// <returns><see langword="true"/> when both handles are equal; otherwise, <see langword="false"/>.</returns>
        public bool Matches(IntPtr windowHandle) => handle == windowHandle;

        /// <inheritdoc />
        protected override bool ReleaseHandle() => true;
    }
}
