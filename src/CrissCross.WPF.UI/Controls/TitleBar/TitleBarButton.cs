// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Extensions;
#else
using CrissCross.WPF.UI.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents TitleBarButton.</summary>
/// <seealso cref="Button" />
public class TitleBarButton : Button
{
    /// <summary>Property for <see cref="ButtonType"/>.</summary>
    public static readonly DependencyProperty ButtonTypeProperty = DependencyProperty.Register(
        nameof(ButtonType),
        typeof(TitleBarButtonType),
        typeof(TitleBarButton),
        new PropertyMetadata(TitleBarButtonType.Unknown, ButtonTypePropertyCallback));

    /// <summary>Property for <see cref="ButtonsForeground"/>.</summary>
    public static readonly DependencyProperty ButtonsForegroundProperty = DependencyProperty.Register(
        nameof(ButtonsForeground),
        typeof(Brush),
        typeof(TitleBarButton),
        new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Property for <see cref="MouseOverButtonsForeground"/>.</summary>
    public static readonly DependencyProperty MouseOverButtonsForegroundProperty = DependencyProperty.Register(
        nameof(MouseOverButtonsForeground),
        typeof(Brush),
        typeof(TitleBarButton),
        new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Property for <see cref="RenderButtonsForeground"/>.</summary>
    public static readonly DependencyProperty RenderButtonsForegroundProperty = DependencyProperty.Register(
        nameof(RenderButtonsForeground),
        typeof(Brush),
        typeof(TitleBarButton),
        new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Stores the _defaultBackgroundBrush value.</summary>
    private readonly Brush _defaultBackgroundBrush = Brushes.Transparent; // TODO: Should it be transparent?

    /// <summary>Stores the _returnvalue.</summary>
    private User32.WM_NCHITTEST _returnValue;

    /// <summary>Stores the _isClickedDown value.</summary>
    private bool _isClickedDown;

    /// <summary>Initializes a new instance of the <see cref="TitleBarButton"/> class.</summary>
    public TitleBarButton()
    {
        Loaded += (_, _) => TitleBarButton_Loaded();
        Unloaded += (_, _) => TitleBarButton_Unloaded();
    }

    /// <summary>Gets or sets the type of the button.</summary>
    /// <value>
    /// The type of the button.
    /// </value>
    public TitleBarButtonType ButtonType
    {
        get => (TitleBarButtonType)GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    /// <summary>Gets or sets the buttons foreground.</summary>
    /// <value>
    /// The buttons foreground.
    /// </value>
    public Brush? ButtonsForeground
    {
        get => (Brush?)GetValue(ButtonsForegroundProperty);
        set => SetValue(ButtonsForegroundProperty, value);
    }

    /// <summary>Gets or sets foreground of the navigation buttons while mouse over.</summary>
    public Brush? MouseOverButtonsForeground
    {
        get => (Brush?)GetValue(MouseOverButtonsForegroundProperty);
        set => SetValue(MouseOverButtonsForegroundProperty, value);
    }

    /// <summary>Gets or sets the render buttons foreground.</summary>
    /// <value>
    /// The render buttons foreground.
    /// </value>
    public Brush? RenderButtonsForeground
    {
        get => (Brush?)GetValue(RenderButtonsForegroundProperty);
        set => SetValue(RenderButtonsForegroundProperty, value);
    }

    /// <summary>Gets a value indicating whether this instance is hovered.</summary>
    /// <value>
    ///   <c>true</c> if this instance is hovered; otherwise, <c>false</c>.
    /// </value>
    public bool IsHovered { get; private set; }

    /// <summary>Forces button background to change.</summary>
    public void Hover()
    {
        if (IsHovered)
        {
            return;
        }

        Background = MouseOverBackground;
        if (MouseOverButtonsForeground is not null)
        {
            RenderButtonsForeground = MouseOverButtonsForeground;
        }

        IsHovered = true;
    }

    /// <summary>Forces button background to change.</summary>
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

    /// <summary>Invokes click on the button.</summary>
    public void InvokeClick()
    {
        if (new ButtonAutomationPeer(this).GetPattern(PatternInterface.Invoke) is IInvokeProvider invokeProvider)
        {
            invokeProvider.Invoke();
        }

        _isClickedDown = false;
    }

    /// <summary>Provides the ReactToHwndHook member.</summary>
    /// <param name="msg">The msg value.</param>
    /// <param name="messageParameter">The message parameter value.</param>
    /// <param name="returnIntPtr">The returnIntPtr value.</param>
    /// <returns>The result.</returns>
    internal bool ReactToHwndHook(User32.WM msg, IntPtr messageParameter, out IntPtr returnIntPtr)
    {
        returnIntPtr = IntPtr.Zero;

        switch (msg)
        {
            case User32.WM.NCHITTEST:
            {
                if (this.IsMouseOverElement(messageParameter))
                {
                    Hover();
                    returnIntPtr = (IntPtr)_returnValue;
                    return true;
                }

                RemoveHover();
                return false;
            }

            case User32.WM.NCMOUSELEAVE: // Mouse leaves the window
            {
                RemoveHover();
                return false;
            }

            case User32.WM.NCLBUTTONDOWN when this.IsMouseOverElement(messageParameter): // Left button clicked down
            {
                _isClickedDown = true;
                return true;
            }

            // Left button clicked up.
            case User32.WM.NCLBUTTONUP when _isClickedDown && this.IsMouseOverElement(messageParameter):
            {
                InvokeClick();
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>Provides the ButtonTypePropertyCallback member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void ButtonTypePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var titleBarButton = (TitleBarButton)d;
        titleBarButton.UpdateReturnValue((TitleBarButtonType)e.NewValue);
    }

    /// <summary>Provides the TitleBarButton_Unloaded member.</summary>
    private void TitleBarButton_Unloaded() =>
        DependencyPropertyDescriptor
            .FromProperty(ButtonsForegroundProperty, typeof(Brush))
            .RemoveValueChanged(this, OnButtonsForegroundChanged);

    /// <summary>Provides the TitleBarButton_Loaded member.</summary>
    private void TitleBarButton_Loaded()
    {
        RenderButtonsForeground = ButtonsForeground;
        DependencyPropertyDescriptor
            .FromProperty(ButtonsForegroundProperty, typeof(Brush))
            .AddValueChanged(this, OnButtonsForegroundChanged);
    }

    /// <summary>Provides the OnButtonsForegroundChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnButtonsForegroundChanged(object? sender, EventArgs e) =>
        SetCurrentValue(RenderButtonsForegroundProperty, IsHovered ? MouseOverButtonsForeground : ButtonsForeground);

    /// <summary>Provides the UpdateReturnValue member.</summary>
    /// <param name="buttonType">The buttonType value.</param>
    private void UpdateReturnValue(TitleBarButtonType buttonType) =>
        _returnValue = buttonType switch
        {
            TitleBarButtonType.Unknown => User32.WM_NCHITTEST.HTNOWHERE,
            TitleBarButtonType.Help => User32.WM_NCHITTEST.HTHELP,
            TitleBarButtonType.Minimize => User32.WM_NCHITTEST.HTMINBUTTON,
            TitleBarButtonType.Close => User32.WM_NCHITTEST.HTCLOSE,
            TitleBarButtonType.Restore or TitleBarButtonType.Maximize => User32.WM_NCHITTEST.HTMAXBUTTON,
            _ => throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null),
        };
}
