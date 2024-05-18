// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using CrissCross.WPF.UI.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// TitleBarButton.
/// </summary>
/// <seealso cref="Button" />
public class TitleBarButton : Button
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

    /// <summary>
    /// Property for <see cref="MouseOverButtonsForeground"/>.
    /// </summary>
    public static readonly DependencyProperty MouseOverButtonsForegroundProperty = DependencyProperty.Register(
        nameof(MouseOverButtonsForeground),
        typeof(Brush),
        typeof(TitleBarButton),
        new FrameworkPropertyMetadata(
            SystemColors.ControlTextBrush,
            FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Property for <see cref="RenderButtonsForeground"/>.
    /// </summary>
    public static readonly DependencyProperty RenderButtonsForegroundProperty = DependencyProperty.Register(
        nameof(RenderButtonsForeground),
        typeof(Brush),
        typeof(TitleBarButton),
        new FrameworkPropertyMetadata(
            SystemColors.ControlTextBrush,
            FrameworkPropertyMetadataOptions.Inherits));

    private readonly Brush _defaultBackgroundBrush = Brushes.Transparent; // TODO: Should it be transparent?
    private User32.WM_NCHITTEST _returnValue;
    private bool _isClickedDown;

    /// <summary>
    /// Initializes a new instance of the <see cref="TitleBarButton"/> class.
    /// </summary>
    public TitleBarButton()
    {
        Loaded += TitleBarButton_Loaded;
        Unloaded += TitleBarButton_Unloaded;
    }

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
    public Brush? ButtonsForeground
    {
        get => (Brush?)GetValue(ButtonsForegroundProperty);
        set => SetValue(ButtonsForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets foreground of the navigation buttons while mouse over.
    /// </summary>
    public Brush? MouseOverButtonsForeground
    {
        get => (Brush?)GetValue(MouseOverButtonsForegroundProperty);
        set => SetValue(MouseOverButtonsForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the render buttons foreground.
    /// </summary>
    /// <value>
    /// The render buttons foreground.
    /// </value>
    public Brush? RenderButtonsForeground
    {
        get => (Brush?)GetValue(RenderButtonsForegroundProperty);
        set => SetValue(RenderButtonsForegroundProperty, value);
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
        if (MouseOverButtonsForeground != null)
        {
            RenderButtonsForeground = MouseOverButtonsForeground;
        }

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
        RenderButtonsForeground = ButtonsForeground;

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

    private void TitleBarButton_Unloaded(object sender, RoutedEventArgs e) =>
        DependencyPropertyDescriptor.FromProperty(ButtonsForegroundProperty, typeof(Brush))
            .RemoveValueChanged(this, OnButtonsForegroundChanged);

    private void TitleBarButton_Loaded(object sender, RoutedEventArgs e)
    {
        RenderButtonsForeground = ButtonsForeground;
        DependencyPropertyDescriptor.FromProperty(ButtonsForegroundProperty, typeof(Brush))
            .AddValueChanged(this, OnButtonsForegroundChanged);
    }

    private void OnButtonsForegroundChanged(object? sender, EventArgs e) =>
        SetCurrentValue(RenderButtonsForegroundProperty, IsHovered ? MouseOverButtonsForeground : ButtonsForeground);

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
