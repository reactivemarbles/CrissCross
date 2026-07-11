// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Shell;
using Size = System.Windows.Size;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// If you use <see cref="WindowChrome"/> to extend the UI elements to the non-client area, you can include this container
/// in the template of <see cref="Window"/> so that the content inside automatically fills the client area.
/// Using this container can let you get rid of various margin adaptations done in
/// Setter/Trigger of the style of <see cref="Window"/> when the window state changes.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;Style
///     x:Key="MyWindowCustomStyle"
///     BasedOn="{StaticResource {x:Type Window}}"
///     TargetType="{x:Type controls:FluentWindow}"&gt;
///     &lt;Setter Property="Template" &gt;
///         &lt;Setter.Value&gt;
///             &lt;ControlTemplate TargetType="{x:Type Window}"&gt;
///                 &lt;AdornerDecorator&gt;
///                     &lt;controls:ClientAreaBorder
///                         Background="{TemplateBinding Background}"
///                         BorderBrush="{TemplateBinding BorderBrush}"
///                         BorderThickness="{TemplateBinding BorderThickness}"&gt;
///                         &lt;ContentPresenter x:Name="ContentPresenter" /&gt;
///                     &lt;/controls:ClientAreaBorder&gt;
///                 &lt;/AdornerDecorator&gt;
///             &lt;/ControlTemplate&gt;
///         &lt;/Setter.Value&gt;
///     &lt;/Setter&gt;
/// &lt;/Style&gt;
/// </code>
/// </example>
public class ClientAreaBorder : System.Windows.Controls.Border, IThemeControl
{
    /// <summary>Stores the _paddedBorderThickness value.</summary>
    private static Thickness? _paddedBorderThickness;

    /// <summary>Stores the _resizeFrameBorderThickness value.</summary>
    private static Thickness? _resizeFrameBorderThickness;

    /// <summary>Stores the _windowChromeNonClientFrameThickness value.</summary>
    private static Thickness? _windowChromeNonClientFrameThickness;

    /// <summary>Stores the _windowStateChangedSubscription value.</summary>
    private IDisposable? _windowStateChangedSubscription;

    /// <summary>Stores the _windowClosingSubscription value.</summary>
    private IDisposable? _windowClosingSubscription;

    /// <summary>Stores the _borderBrushApplied value.</summary>
    private bool _borderBrushApplied;

    /// <summary>Stores the _oldWindow value.</summary>
    private System.Windows.Window? _oldWindow;

    /// <summary>Initializes a new instance of the <see cref="ClientAreaBorder"/> class.</summary>
    public ClientAreaBorder()
    {
        ApplicationTheme = ApplicationThemeManager.GetAppTheme();
        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    /// <summary>Gets get the system SM_CXFRAME and SM_CYFRAME values in WPF units.</summary>
    public static Thickness ResizeFrameBorderThickness =>
        _resizeFrameBorderThickness ??= new Thickness(
            SystemParameters.ResizeFrameVerticalBorderWidth,
            SystemParameters.ResizeFrameHorizontalBorderHeight,
            SystemParameters.ResizeFrameVerticalBorderWidth,
            SystemParameters.ResizeFrameHorizontalBorderHeight);

    /// <summary>Gets or sets the theme is currently set.</summary>
    public ApplicationTheme ApplicationTheme { get; set; } = ApplicationTheme.Unknown;

    /// <summary>Gets get the system SM_CXPADDEDBORDER value in WPF units.</summary>
    public Thickness PaddedBorderThickness
    {
        get
        {
            if (_paddedBorderThickness is not null)
            {
                return _paddedBorderThickness.Value;
            }

            var paddedBorder = User32.GetSystemMetrics(User32.SM.CXPADDEDBORDER);

            var (factorX, factorY) = GetDpi();

            var frameSize = new Size(paddedBorder, paddedBorder);
            var frameSizeInDips = new Size(frameSize.Width / factorX, frameSize.Height / factorY);

            _paddedBorderThickness = new Thickness(
                frameSizeInDips.Width,
                frameSizeInDips.Height,
                frameSizeInDips.Width,
                frameSizeInDips.Height);

            return _paddedBorderThickness.Value;
        }
    }

    /// <summary>
    /// Gets if you use a <see cref="WindowChrome"/> to extend the client area of a window to the non-client area, you need to handle the edge margin issue when the window is maximized.
    /// Use this property to get the correct margin value when the window is maximized, so that when the window is maximized, the client area can completely cover the screen client area by no less than a single pixel at any DPI.
    /// The<see cref="User32.GetSystemMetrics"/> method cannot obtain this value directly.
    /// </summary>
    public Thickness WindowChromeNonClientFrameThickness =>
        _windowChromeNonClientFrameThickness ??= new Thickness(
            ResizeFrameBorderThickness.Left + PaddedBorderThickness.Left,
            ResizeFrameBorderThickness.Top + PaddedBorderThickness.Top,
            ResizeFrameBorderThickness.Right + PaddedBorderThickness.Right,
            ResizeFrameBorderThickness.Bottom + PaddedBorderThickness.Bottom);

    /// <inheritdoc />
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);

        if (_oldWindow is not null)
        {
            _windowStateChangedSubscription?.Dispose();
            _windowClosingSubscription?.Dispose();
        }

        var newWindow = (System.Windows.Window?)System.Windows.Window.GetWindow(this);

        if (newWindow is not null)
        {
            _windowStateChangedSubscription?.Dispose();
            _windowStateChangedSubscription = EventSignal
                .From<EventHandler, EventArgs>(handler => handler.Invoke, handler => newWindow.StateChanged += handler, handler => newWindow.StateChanged -= handler)
                .Subscribe(e => OnWindowStateChanged(newWindow, e));
            _windowClosingSubscription = EventSignal
                .From<CancelEventHandler, CancelEventArgs>(handler => handler.Invoke, handler => newWindow.Closing += handler, handler => newWindow.Closing -= handler)
                .Subscribe(OnWindowClosing);
        }

        _oldWindow = newWindow;

        ApplyDefaultWindowBorder();
    }

    /// <summary>Provides the OnThemeChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnThemeChanged(object? sender, ThemeChangedEventArgs args)
    {
        ApplicationTheme = args.CurrentApplicationTheme;

        if (!_borderBrushApplied || _oldWindow is null)
        {
            return;
        }

        ApplyDefaultWindowBorder();
    }

    /// <summary>Provides the OnWindowClosing member.</summary>
    /// <param name="eventArgs">The event arguments.</param>
    private void OnWindowClosing(CancelEventArgs eventArgs)
    {
        _ = eventArgs;

        ApplicationThemeManager.Changed -= OnThemeChanged;
        if (_oldWindow is null)
        {
            return;
        }

        _windowClosingSubscription?.Dispose();
    }

    /// <summary>Provides the OnWindowStateChanged member.</summary>
    /// <param name="window">The window value.</param>
    /// <param name="eventArgs">The event arguments.</param>
    private void OnWindowStateChanged(System.Windows.Window window, EventArgs eventArgs)
    {
        _ = eventArgs;

        Padding = window.WindowState switch
        {
            WindowState.Maximized => WindowChromeNonClientFrameThickness,
            _ => default,
        };
    }

    /// <summary>Provides the ApplyDefaultWindowBorder member.</summary>
    private void ApplyDefaultWindowBorder()
    {
        if (Win32.Utilities.IsOSWindows11OrNewer || _oldWindow is null)
        {
            return;
        }

        _borderBrushApplied = true;
        _oldWindow.BorderThickness = new(1);
        _oldWindow.BorderBrush = new SolidColorBrush(
            ApplicationTheme == ApplicationTheme.Light
                ? Color.FromArgb(0xFF, 0x7A, 0x7A, 0x7A)
                : Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A));
    }

    /// <summary>Provides the GetDpi member.</summary>
    /// <returns>The result.</returns>
    private (double factorX, double factorY) GetDpi()
    {
        if (PresentationSource.FromVisual(this) is { } source)
        {
            return (
                source.CompositionTarget.TransformToDevice.M11, // Possible null reference
                source.CompositionTarget.TransformToDevice.M22);
        }

        var systemDPi = DpiHelper.GetSystemDpi();

        return (systemDPi.DpiScaleX, systemDPi.DpiScaleY);
    }
}
