// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//// This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using CrissCross.WPF.UI.Extensions;

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// TitleBarButton.
/// </summary>
/// <seealso cref="CrissCross.WPF.UI.Controls.Button" />
internal class TitleBarButton : CrissCross.WPF.UI.Controls.Button
{
    /// <summary>
    /// Property for <see cref="ButtonType"/>.
    /// </summary>
    public static readonly DependencyProperty ButtonTypeProperty = DependencyProperty.Register(
        nameof(ButtonType),
        typeof(TitleBarButtonType),
        typeof(TitleBarButton),
        new PropertyMetadata(TitleBarButtonType.Unknown, ButtonTypePropertyCallback));

    /// <summary>
    /// Property for <see cref="ButtonsForeground"/>.
    /// </summary>
    public static readonly DependencyProperty ButtonsForegroundProperty = DependencyProperty.Register(
        nameof(ButtonsForeground),
        typeof(Brush),
        typeof(TitleBarButton),
        new FrameworkPropertyMetadata(
            SystemColors.ControlTextBrush,
            FrameworkPropertyMetadataOptions.Inherits));

    private readonly Brush _defaultBackgroundBrush = Brushes.Transparent; // TODO: Should it be transparent?
    private User32.WM_NCHITTEST _returnValue;
    private bool _isClickedDown;

    /// <summary>
    /// Gets or sets the type of the button.
    /// </summary>
    /// <value>
    /// The type of the button.
    /// </value>
    public TitleBarButtonType ButtonType
    {
        get => (TitleBarButtonType)GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the buttons foreground.
    /// </summary>
    /// <value>
    /// The buttons foreground.
    /// </value>
    public Brush ButtonsForeground
    {
        get => (Brush)GetValue(ButtonsForegroundProperty);
        set => SetValue(ButtonsForegroundProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is hovered.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is hovered; otherwise, <c>false</c>.
    /// </value>
    public bool IsHovered { get; private set; }

    /// <summary>
    /// Forces button background to change.
    /// </summary>
    public void Hover()
    {
        if (IsHovered)
        {
            return;
        }

        Background = MouseOverBackground;
        IsHovered = true;
    }

    /// <summary>
    /// Forces button background to change.
    /// </summary>
    public void RemoveHover()
    {
        if (!IsHovered)
        {
            return;
        }

        Background = _defaultBackgroundBrush;

        IsHovered = false;
        _isClickedDown = false;
    }

    /// <summary>
    /// Invokes click on the button.
    /// </summary>
    public void InvokeClick()
    {
        if (
            new ButtonAutomationPeer(this).GetPattern(PatternInterface.Invoke)
            is IInvokeProvider invokeProvider)
        {
            invokeProvider.Invoke();
        }

        _isClickedDown = false;
    }

    internal bool ReactToHwndHook(User32.WM msg, IntPtr lParam, out IntPtr returnIntPtr)
    {
        returnIntPtr = IntPtr.Zero;

        switch (msg)
        {
            case User32.WM.NCHITTEST:
                if (this.IsMouseOverElement(lParam))
                {
                    Hover();
                    returnIntPtr = (IntPtr)_returnValue;
                    return true;
                }

                RemoveHover();
                return false;

            case User32.WM.NCMOUSELEAVE: // Mouse leaves the window
                RemoveHover();
                return false;
            case User32.WM.NCLBUTTONDOWN when this.IsMouseOverElement(lParam): // Left button clicked down
                _isClickedDown = true;
                return true;
            case User32.WM.NCLBUTTONUP when _isClickedDown && this.IsMouseOverElement(lParam): // Left button clicked up
                InvokeClick();
                return true;
            default:
                return false;
        }
    }

    private static void ButtonTypePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var titleBarButton = (TitleBarButton)d;
        titleBarButton.UpdateReturnValue((TitleBarButtonType)e.NewValue);
    }

    private void UpdateReturnValue(TitleBarButtonType buttonType) =>
        _returnValue = buttonType switch
        {
            TitleBarButtonType.Unknown => User32.WM_NCHITTEST.HTNOWHERE,
            TitleBarButtonType.Help => User32.WM_NCHITTEST.HTHELP,
            TitleBarButtonType.Minimize => User32.WM_NCHITTEST.HTMINBUTTON,
            TitleBarButtonType.Close => User32.WM_NCHITTEST.HTCLOSE,
            TitleBarButtonType.Restore => User32.WM_NCHITTEST.HTMAXBUTTON,
            TitleBarButtonType.Maximize => User32.WM_NCHITTEST.HTMAXBUTTON,
            _ => throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null)
        };
}
