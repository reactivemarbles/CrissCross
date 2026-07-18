// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Designer;
#else
using CrissCross.WPF.UI.Designer;
#endif
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Extensions;
#else
using CrissCross.WPF.UI.Extensions;
#endif
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Custom navigation buttons for the window.</summary>
[TemplatePart(Name = ElementIcon, Type = typeof(System.Windows.Controls.Image))]
[TemplatePart(Name = ElementHelpButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementMinimizeButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementMaximizeButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementRestoreButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementCloseButton, Type = typeof(TitleBarButton))]
public class TitleBar : Control, IThemeControl
{
    /// <summary>Property for <see cref="ApplicationTheme"/>.</summary>
    public static readonly DependencyProperty ApplicationThemeProperty = DependencyProperty.Register(
        nameof(ApplicationTheme),
        typeof(ApplicationTheme),
        typeof(TitleBar),
        new PropertyMetadata(ApplicationTheme.Unknown));

    /// <summary>Property for <see cref="Title"/>.</summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="Header"/>.</summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>The title content property.</summary>
    public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
        nameof(TitleContent),
        typeof(object),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="ButtonsForeground"/>.</summary>
    public static readonly DependencyProperty ButtonsForegroundProperty = DependencyProperty.Register(
        nameof(ButtonsForeground),
        typeof(Brush),
        typeof(TitleBar),
        new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Property for <see cref="ButtonsBackground"/>.</summary>
    public static readonly DependencyProperty ButtonsBackgroundProperty = DependencyProperty.Register(
        nameof(ButtonsBackground),
        typeof(Brush),
        typeof(TitleBar),
        new FrameworkPropertyMetadata(SystemColors.ControlBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Property for <see cref="IsMaximized"/>.</summary>
    public static readonly DependencyProperty IsMaximizedProperty = DependencyProperty.Register(
        nameof(IsMaximized),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="ForceShutdown"/>.</summary>
    public static readonly DependencyProperty ForceShutdownProperty = DependencyProperty.Register(
        nameof(ForceShutdown),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="ShowMaximize"/>.</summary>
    public static readonly DependencyProperty ShowMaximizeProperty = DependencyProperty.Register(
        nameof(ShowMaximize),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>Property for <see cref="ShowMinimize"/>.</summary>
    public static readonly DependencyProperty ShowMinimizeProperty = DependencyProperty.Register(
        nameof(ShowMinimize),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>Property for <see cref="ShowHelp"/>.</summary>
    public static readonly DependencyProperty ShowHelpProperty = DependencyProperty.Register(
        nameof(ShowHelp),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="ShowClose"/>.</summary>
    public static readonly DependencyProperty ShowCloseProperty = DependencyProperty.Register(
        nameof(ShowClose),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>Property for <see cref="CanMaximize"/>.</summary>
    public static readonly DependencyProperty CanMaximizeProperty = DependencyProperty.Register(
        nameof(CanMaximize),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>The content property.</summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(ObservableCollection<FrameworkElement>),
        typeof(TitleBar));

    /// <summary>Property for <see cref="CloseWindowByDoubleClickOnIcon"/>.</summary>
    public static readonly DependencyProperty CloseWindowByDoubleClickOnIconProperty = DependencyProperty.Register(
        nameof(CloseWindowByDoubleClickOnIcon),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>Routed event for <see cref="CloseClicked"/>.</summary>
    public static readonly RoutedEvent CloseClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(CloseClicked),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>Routed event for <see cref="MaximizeClicked"/>.</summary>
    public static readonly RoutedEvent MaximizeClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(MaximizeClicked),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>Routed event for <see cref="MinimizeClicked"/>.</summary>
    public static readonly RoutedEvent MinimizeClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(MinimizeClicked),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>Routed event for <see cref="HelpClicked"/>.</summary>
    public static readonly RoutedEvent HelpClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(HelpClicked),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>Property for <see cref="TemplateButtonCommand"/>.</summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(IReactiveCommand),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>Provides the ElementIcon member.</summary>
    private const string ElementIcon = "PART_Icon";

    /// <summary>Provides the ElementHelpButton member.</summary>
    private const string ElementHelpButton = "PART_HelpButton";

    /// <summary>Provides the ElementMinimizeButton member.</summary>
    private const string ElementMinimizeButton = "PART_MinimizeButton";

    /// <summary>Provides the ElementMaximizeButton member.</summary>
    private const string ElementMaximizeButton = "PART_MaximizeButton";

    /// <summary>Provides the ElementRestoreButton member.</summary>
    private const string ElementRestoreButton = "PART_RestoreButton";

    /// <summary>Provides the ElementCloseButton member.</summary>
    private const string ElementCloseButton = "PART_CloseButton";

    /// <summary>Provides the number of title bar buttons tracked by the control.</summary>
    private const int TitleBarButtonCount = 4;

    /// <summary>Provides the maximize button index.</summary>
    private const int MaximizeButtonIndex = 0;

    /// <summary>Provides the minimize button index.</summary>
    private const int MinimizeButtonIndex = 1;

    /// <summary>Provides the close button index.</summary>
    private const int CloseButtonIndex = 2;

    /// <summary>Provides the help button index.</summary>
    private const int HelpButtonIndex = 3;

    /// <summary>Provides the dpiScale member.</summary>
    private static DpiScale? _dpiScale;

    /// <summary>Stores the _buttons value.</summary>
    private readonly TitleBarButton[] _buttons = new TitleBarButton[TitleBarButtonCount];

    /// <summary>Stores the _currentWindow value.</summary>
    private System.Windows.Window _currentWindow = null!;

    /// <summary>Stores the _icon value.</summary>
    private ContentPresenter _icon = null!;

    /// <summary>Initializes a new instance of the <see cref="TitleBar"/> class.</summary>
    public TitleBar()
    {
        Content = [];
    }

    /// <summary>Event triggered after clicking close button.</summary>
    public event EventHandler<RoutedEventArgs> CloseClicked
    {
        add => AddHandler(CloseClickedEvent, value);
        remove => RemoveHandler(CloseClickedEvent, value);
    }

    /// <summary>Event triggered after clicking maximize or restore button.</summary>
    public event EventHandler<RoutedEventArgs> MaximizeClicked
    {
        add => AddHandler(MaximizeClickedEvent, value);
        remove => RemoveHandler(MaximizeClickedEvent, value);
    }

    /// <summary>Event triggered after clicking minimize button.</summary>
    public event EventHandler<RoutedEventArgs> MinimizeClicked
    {
        add => AddHandler(MinimizeClickedEvent, value);
        remove => RemoveHandler(MinimizeClickedEvent, value);
    }

    /// <summary>Event triggered after clicking help button</summary>
    public event EventHandler<RoutedEventArgs> HelpClicked
    {
        add => AddHandler(HelpClickedEvent, value);
        remove => RemoveHandler(HelpClickedEvent, value);
    }

    /// <inheritdoc />
    public ApplicationTheme ApplicationTheme
    {
        get => (ApplicationTheme)GetValue(ApplicationThemeProperty);
        set => SetValue(ApplicationThemeProperty, value);
    }

    /// <summary>Gets or sets title displayed on the left.</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets the content displayed in the <see cref="TitleBar"/>.</summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Gets or sets the content of the title.</summary>
    /// <value>
    /// The content of the title.
    /// </value>
    public object TitleContent
    {
        get => GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }

    /// <summary>Gets or sets foreground of the navigation buttons.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public Brush ButtonsForeground
    {
        get => (Brush)GetValue(ButtonsForegroundProperty);
        set => SetValue(ButtonsForegroundProperty, value);
    }

    /// <summary>Gets or sets background of the navigation buttons when hovered.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public Brush ButtonsBackground
    {
        get => (Brush)GetValue(ButtonsBackgroundProperty);
        set => SetValue(ButtonsBackgroundProperty, value);
    }

    /// <summary>Gets whether gets or sets information whether the current window is maximized.</summary>
    public bool IsMaximized
    {
        get => (bool)GetValue(IsMaximizedProperty);
        internal set => SetValue(IsMaximizedProperty, value);
    }

    /// <summary>Gets or sets the GetValue value.</summary>
    public bool ForceShutdown
    {
        get => (bool)GetValue(ForceShutdownProperty);
        set => SetValue(ForceShutdownProperty, value);
    }

    /// <summary>Gets or sets whether gets or sets information whether to show maximize button.</summary>
    public bool ShowMaximize
    {
        get => (bool)GetValue(ShowMaximizeProperty);
        set => SetValue(ShowMaximizeProperty, value);
    }

    /// <summary>Gets or sets whether gets or sets information whether to show minimize button.</summary>
    public bool ShowMinimize
    {
        get => (bool)GetValue(ShowMinimizeProperty);
        set => SetValue(ShowMinimizeProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets information whether to show help button.</summary>
    public bool ShowHelp
    {
        get => (bool)GetValue(ShowHelpProperty);
        set => SetValue(ShowHelpProperty, value);
    }

    /// <summary>Gets or sets whether gets or sets information whether to show close button.</summary>
    public bool ShowClose
    {
        get => (bool)GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether enables or disables the maximize functionality if disables the
    /// MaximizeActionOverride action won't be called.</summary>
    public bool CanMaximize
    {
        get => (bool)GetValue(CanMaximizeProperty);
        set => SetValue(CanMaximizeProperty, value);
    }

    /// <summary>Gets or sets titlebar icon.</summary>
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets the content.</summary>
    /// <value>
    /// The content.
    /// </value>
    public ObservableCollection<FrameworkElement> Content
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>Gets or sets whether enables or disable closing the window by double clicking on the icon.</summary>
    public bool CloseWindowByDoubleClickOnIcon
    {
        get => (bool)GetValue(CloseWindowByDoubleClickOnIconProperty);
        set => SetValue(CloseWindowByDoubleClickOnIconProperty, value);
    }

    /// <summary>Gets command triggered after clicking the titlebar button.</summary>
    public IReactiveCommand TemplateButtonCommand => (IReactiveCommand)GetValue(TemplateButtonCommandProperty);

    /// <summary>Gets or sets lets you override the behavior of the Maximize/Restore button with an Action.</summary>
    public Action<TitleBar, System.Windows.Window>? MaximizeActionOverride { get; set; }

    /// <summary>Gets or sets lets you override the behavior of the Minimize button with an Action.</summary>
    public Action<TitleBar, System.Windows.Window>? MinimizeActionOverride { get; set; }

    /// <summary>
    /// Invoked whenever application code or an internal process,
    /// such as a rebuilding layout pass, calls the ApplyTemplate method.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        MouseRightButtonUp += TitleBar_MouseRightButtonUp;
        _icon = GetTemplateChild<ContentPresenter>(ElementIcon);

        var helpButton = GetTemplateChild<TitleBarButton>(ElementHelpButton);
        var minimizeButton = GetTemplateChild<TitleBarButton>(ElementMinimizeButton);
        var maximizeButton = GetTemplateChild<TitleBarButton>(ElementMaximizeButton);
        var closeButton = GetTemplateChild<TitleBarButton>(ElementCloseButton);

        _buttons[MaximizeButtonIndex] = maximizeButton;
        _buttons[MinimizeButtonIndex] = minimizeButton;
        _buttons[CloseButtonIndex] = closeButton;
        _buttons[HelpButtonIndex] = helpButton;
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        ApplicationTheme = ApplicationThemeManager.GetAppTheme();
        ApplicationThemeManager.Changed += OnThemeChanged;
        SetValue(TemplateButtonCommandProperty, ReactiveCommand.Create<TitleBarButtonType>(OnTemplateButtonClick));
        _dpiScale ??= VisualTreeHelper.GetDpi(this);
        Loaded += (_, args) => OnLoaded(args);
        Unloaded += (_, _) => OnUnloaded();
    }

    /// <summary>Called when [loaded].</summary>
    /// <exception cref="ArgumentNullException">Window is null.</exception>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnLoaded(RoutedEventArgs e)
    {
        if (DesignerHelper.IsInDesignMode)
        {
            return;
        }

        _currentWindow = System.Windows.Window.GetWindow(this) ?? throw new ArgumentNullException("Window is null");
        AddWindowEvents(_currentWindow);
    }

    /// <summary>This virtual method is triggered when the app's theme changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    protected virtual void OnThemeChanged(object? sender, ThemeChangedEventArgs args)
    {
        Debug.WriteLine(
            $"INFO | {typeof(TitleBar)} received theme -  {args.CurrentApplicationTheme}",
            "CrissCross.WPF.UI.TitleBar");

        ApplicationTheme = args.CurrentApplicationTheme;
    }

    /// <summary>Show 'SystemMenu' on mouse right button up.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void TitleBar_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        var point = PointToScreen(e.GetPosition(this));
        SystemCommands.ShowSystemMenu(
            _currentWindow,
            new Point(point.X / _dpiScale!.Value.DpiScaleX, point.Y / _dpiScale.Value.DpiScaleY));
    }

    /// <summary>Provides the OnUnloaded member.</summary>
    private void OnUnloaded()
    {
        ApplicationThemeManager.Changed -= OnThemeChanged;
    }

    /// <summary>Provides the CloseWindow member.</summary>
    private void CloseWindow()
    {
        Debug.WriteLine(
            $"INFO | {typeof(TitleBar)}.CloseWindow:ForceShutdown -  {ForceShutdown}",
            "CrissCross.WPF.UI.TitleBar");

        if (ForceShutdown)
        {
            UiApplication.Current.Shutdown();
            return;
        }

        _currentWindow.Close();
    }

    /// <summary>Provides the MinimizeWindow member.</summary>
    private void MinimizeWindow()
    {
        if (MinimizeActionOverride is not null)
        {
            MinimizeActionOverride(this, _currentWindow);

            return;
        }

        _currentWindow.WindowState = WindowState.Minimized;
    }

    /// <summary>Provides the MaximizeWindow member.</summary>
    private void MaximizeWindow()
    {
        if (!CanMaximize)
        {
            return;
        }

        if (MaximizeActionOverride is not null)
        {
            MaximizeActionOverride(this, _currentWindow);

            return;
        }

        if (_currentWindow.WindowState == WindowState.Normal)
        {
            IsMaximized = true;
            _currentWindow.WindowState = WindowState.Maximized;
        }
        else
        {
            IsMaximized = false;
            _currentWindow.WindowState = WindowState.Normal;
        }
    }

    /// <summary>Provides the OnParentWindowStateChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnParentWindowStateChanged(object? sender, EventArgs e)
    {
        if (IsMaximized == (_currentWindow.WindowState == WindowState.Maximized))
        {
            return;
        }

        IsMaximized = _currentWindow.WindowState == WindowState.Maximized;
    }

    /// <summary>Provides the OnTemplateButtonClick member.</summary>
    /// <param name="buttonType">The buttonType value.</param>
    private void OnTemplateButtonClick(TitleBarButtonType buttonType)
    {
        switch (buttonType)
        {
            case TitleBarButtonType.Maximize or TitleBarButtonType.Restore:
            {
                RaiseEvent(new RoutedEventArgs(MaximizeClickedEvent, this));
                MaximizeWindow();
                break;
            }

            case TitleBarButtonType.Close:
            {
                RaiseEvent(new RoutedEventArgs(CloseClickedEvent, this));
                CloseWindow();
                break;
            }

            case TitleBarButtonType.Minimize:
            {
                RaiseEvent(new RoutedEventArgs(MinimizeClickedEvent, this));
                MinimizeWindow();
                break;
            }

            case TitleBarButtonType.Help:
            {
                RaiseEvent(new RoutedEventArgs(HelpClickedEvent, this));
                break;
            }

            case TitleBarButtonType.Unknown:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null);
        }
    }

    /// <summary>Listening window hooks after rendering window content to SizeToContent support.</summary>
    /// <param name="window">The window value.</param>
    private void AddWindowEvents(System.Windows.Window window)
    {
        window.StateChanged += OnParentWindowStateChanged;
        var handle = new WindowInteropHelper(window).Handle;
        var windowSource = HwndSource.FromHwnd(handle) ?? throw new ArgumentNullException("Window source is null");
        windowSource.AddHook(
            (IntPtr _, int msg, IntPtr _, IntPtr longParameter, ref bool handled) =>
                HwndSourceHook(msg, longParameter, ref handled));
    }

    /// <summary>Provides the HwndSourceHook member.</summary>
    /// <param name="msg">The msg value.</param>
    /// <param name="longParameter">The long parameter value.</param>
    /// <param name="handled">The handled value.</param>
    /// <returns>The result.</returns>
    private IntPtr HwndSourceHook(int msg, IntPtr longParameter, ref bool handled)
    {
        var message = (User32.WM)msg;

        if (
            message
            is not (User32.WM.NCHITTEST or User32.WM.NCMOUSELEAVE or User32.WM.NCLBUTTONDOWN or User32.WM.NCLBUTTONUP))
        {
            return IntPtr.Zero;
        }

        if (TryHandleButtonHook(message, longParameter, out var handledButtonResult))
        {
            handled = true;
            return handledButtonResult;
        }

        var isMouseOverHeaderContent = IsMouseOverHeaderContent(message, longParameter);

        switch (message)
        {
            case User32.WM.NCHITTEST when CloseWindowByDoubleClickOnIcon && _icon.IsMouseOverElement(longParameter):
            {
                handled = true;

                // Ideally, clicking on the icon should open the system menu, but when the system menu is opened
                // manually, double-clicking on the icon does not close the window
                return (IntPtr)User32.WM_NCHITTEST.HTSYSMENU;
            }

            case User32.WM.NCHITTEST when this.IsMouseOverElement(longParameter) && !isMouseOverHeaderContent:
            {
                handled = true;
                return (IntPtr)User32.WM_NCHITTEST.HTCAPTION;
            }

            default:
                return IntPtr.Zero;
        }
    }

    /// <summary>Determines whether the mouse is over title/header content.</summary>
    /// <param name="message">The window message.</param>
    /// <param name="longParameter">The message long parameter.</param>
    /// <returns><c>true</c> if the mouse is over header content; otherwise, <c>false</c>.</returns>
    private bool IsMouseOverHeaderContent(User32.WM message, IntPtr longParameter)
    {
        var isHitTestMessage = message == User32.WM.NCHITTEST;
        var isOverHeader = Header is UIElement headerUiElement && headerUiElement.IsMouseOverElement(longParameter);
        var isOverTitleContent =
            TitleContent is UIElement titleUiElement && titleUiElement.IsMouseOverElement(longParameter);
        return isHitTestMessage && (isOverHeader || isOverTitleContent);
    }

    /// <summary>Removes hover state from all buttons except the active button.</summary>
    /// <param name="activeButton">The active button.</param>
    private void RemoveHoverFromOtherButtons(TitleBarButton activeButton)
    {
        foreach (var button in _buttons)
        {
            if (!ReferenceEquals(button, activeButton) && button.IsHovered && activeButton.IsHovered)
            {
                button.RemoveHover();
            }
        }
    }

    /// <summary>Tries to handle a hook message with a title bar button.</summary>
    /// <param name="message">The window message.</param>
    /// <param name="longParameter">The message long parameter.</param>
    /// <param name="result">The result pointer.</param>
    /// <returns><c>true</c> if a button handled the hook; otherwise, <c>false</c>.</returns>
    private bool TryHandleButtonHook(User32.WM message, IntPtr longParameter, out IntPtr result)
    {
        foreach (var button in _buttons)
        {
            if (!button.ReactToHwndHook(message, longParameter, out result))
            {
                continue;
            }

            RemoveHoverFromOtherButtons(button);
            return true;
        }

        result = IntPtr.Zero;
        return false;
    }

    /// <summary>Provides the GetTemplateChild member.</summary>
    /// <param name="name">The name value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The result.</returns>
    private T GetTemplateChild<T>(string name)
        where T : DependencyObject
    {
        var element = GetTemplateChild(name) ?? throw new ArgumentNullException($"{name} is null");

        return (T)element;
    }
}
