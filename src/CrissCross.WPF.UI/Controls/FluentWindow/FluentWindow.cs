// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Drawing;
using System.Windows.Shell;

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// A custom WinUI Window with more convenience methods.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(FluentWindow), "FluentWindow.bmp")]
public class FluentWindow : System.Windows.Window, ICanShowMessages
{
    /// <summary>
    /// Property for <see cref="WindowCornerPreference"/>.
    /// </summary>
    public static readonly DependencyProperty WindowCornerPreferenceProperty = DependencyProperty.Register(
        nameof(WindowCornerPreference),
        typeof(WindowCornerPreference),
        typeof(FluentWindow),
        new PropertyMetadata(WindowCornerPreference.Round, OnCornerPreferenceChanged));

    /// <summary>
    /// Property for <see cref="WindowBackdropType"/>.
    /// </summary>
    public static readonly DependencyProperty WindowBackdropTypeProperty = DependencyProperty.Register(
        nameof(WindowBackdropType),
        typeof(WindowBackdropType),
        typeof(FluentWindow),
        new PropertyMetadata(WindowBackdropType.None, OnBackdropTypeChanged));

    /// <summary>
    /// Property for <see cref="ExtendsContentIntoTitleBar"/>.
    /// </summary>
    public static readonly DependencyProperty ExtendsContentIntoTitleBarProperty =
        DependencyProperty.Register(
            nameof(ExtendsContentIntoTitleBar),
            typeof(bool),
            typeof(FluentWindow),
            new PropertyMetadata(false, OnExtendsContentIntoTitleBarChanged));

    private WindowInteropHelper? _interopHelper;

    /// <summary>
    /// Initializes static members of the <see cref="FluentWindow"/> class.
    /// </summary>
    static FluentWindow() => DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FluentWindow),
            new FrameworkPropertyMetadata(typeof(FluentWindow)));

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentWindow"/> class.
    /// </summary>
    public FluentWindow() => SetResourceReference(StyleProperty, typeof(FluentWindow));

    /// <summary>
    /// Gets or sets a value determining corner preference for current <see cref="Window"/>.
    /// </summary>
    public WindowCornerPreference WindowCornerPreference
    {
        get => (WindowCornerPreference)GetValue(WindowCornerPreferenceProperty);
        set => SetValue(WindowCornerPreferenceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value determining preferred backdrop type for current <see cref="Window"/>.
    /// </summary>
    public WindowBackdropType WindowBackdropType
    {
        get => (WindowBackdropType)GetValue(WindowBackdropTypeProperty);
        set => SetValue(WindowBackdropTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that specifies whether the default title bar of the window should be hidden to create space for app content.
    /// </summary>
    public bool ExtendsContentIntoTitleBar
    {
        get => (bool)GetValue(ExtendsContentIntoTitleBarProperty);
        set => SetValue(ExtendsContentIntoTitleBarProperty, value);
    }

    /// <summary>
    /// Gets contains helper for accessing this window handle.
    /// </summary>
    protected WindowInteropHelper InteropHelper => _interopHelper ??= new WindowInteropHelper(this);

    /// <inheritdoc />
    protected override void OnSourceInitialized(EventArgs e)
    {
        OnCornerPreferenceChanged(default, WindowCornerPreference);
        OnExtendsContentIntoTitleBarChanged(default, ExtendsContentIntoTitleBar);
        OnBackdropTypeChanged(default, WindowBackdropType);

        base.OnSourceInitialized(e);
    }

    /// <summary>
    /// This virtual method is called when <see cref="WindowCornerPreference" /> is changed.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    protected virtual void OnCornerPreferenceChanged(
        WindowCornerPreference oldValue,
        WindowCornerPreference newValue)
    {
        if (InteropHelper.Handle == IntPtr.Zero)
        {
            return;
        }

        UnsafeNativeMethods.ApplyWindowCornerPreference(InteropHelper.Handle, newValue);
    }

    /// <summary>
    /// This virtual method is called when <see cref="WindowBackdropType" /> is changed.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <exception cref="System.InvalidOperationException">Cannot apply backdrop effect if {nameof(ExtendsContentIntoTitleBar)} is false.</exception>
    protected virtual void OnBackdropTypeChanged(WindowBackdropType oldValue, WindowBackdropType newValue)
    {
        if (Appearance.ApplicationThemeManager.GetAppTheme() == Appearance.ApplicationTheme.HighContrast)
        {
            newValue = WindowBackdropType.None;
        }

        if (InteropHelper.Handle == IntPtr.Zero)
        {
            return;
        }

        if (newValue == WindowBackdropType.None)
        {
            WindowBackdrop.RemoveBackdrop(this);
            return;
        }

        if (!ExtendsContentIntoTitleBar)
        {
            throw new InvalidOperationException($"Cannot apply backdrop effect if {nameof(ExtendsContentIntoTitleBar)} is false.");
        }

        if (WindowBackdrop.IsSupported(newValue) && WindowBackdrop.RemoveBackground(this))
        {
            WindowBackdrop.ApplyBackdrop(this, newValue);
        }
    }

    /// <summary>
    /// This virtual method is called when <see cref="ExtendsContentIntoTitleBar" /> is changed.
    /// </summary>
    /// <param name="oldValue">if set to <c>true</c> [old value].</param>
    /// <param name="newValue">if set to <c>true</c> [new value].</param>
    protected virtual void OnExtendsContentIntoTitleBarChanged(bool oldValue, bool newValue)
    {
        WindowStyle = WindowStyle.SingleBorderWindow;

        WindowChrome.SetWindowChrome(
            this,
            new WindowChrome
            {
                CaptionHeight = 0,
                CornerRadius = default,
                GlassFrameThickness = new Thickness(-1),
                ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                UseAeroCaptionButtons = false
            });

        UnsafeNativeMethods.RemoveWindowTitlebarContents(this);
    }

    /// <summary>
    /// Private <see cref="WindowCornerPreference"/> property callback.
    /// </summary>
    private static void OnCornerPreferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FluentWindow window)
        {
            return;
        }

        if (e.OldValue == e.NewValue)
        {
            return;
        }

        window.OnCornerPreferenceChanged(
            (WindowCornerPreference)e.OldValue,
            (WindowCornerPreference)e.NewValue);
    }

    /// <summary>
    /// Private <see cref="WindowBackdropType"/> property callback.
    /// </summary>
    private static void OnBackdropTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FluentWindow window)
        {
            return;
        }

        if (e.OldValue == e.NewValue)
        {
            return;
        }

        window.OnBackdropTypeChanged((WindowBackdropType)e.OldValue, (WindowBackdropType)e.NewValue);
    }

    /// <summary>
    /// Private <see cref="ExtendsContentIntoTitleBar"/> property callback.
    /// </summary>
    private static void OnExtendsContentIntoTitleBarChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FluentWindow window)
        {
            return;
        }

        if (e.OldValue == e.NewValue)
        {
            return;
        }

        window.OnExtendsContentIntoTitleBarChanged((bool)e.OldValue, (bool)e.NewValue);
    }
}
