// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows.Controls;
using System.Windows.Input;
using CrissCross.WPF.UI.Designer;
using CrissCross.WPF.UI.Extensions;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Custom navigation buttons for the window.
/// </summary>
[TemplatePart(Name = ElementIcon, Type = typeof(System.Windows.Controls.Image))]
[TemplatePart(Name = ElementHelpButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementMinimizeButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementMaximizeButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementRestoreButton, Type = typeof(TitleBarButton))]
[TemplatePart(Name = ElementCloseButton, Type = typeof(TitleBarButton))]
public partial class TitleBar : Control, IThemeControl
{
    /// <summary>
    /// Property for <see cref="ApplicationTheme"/>.
    /// </summary>
    public static readonly DependencyProperty ApplicationThemeProperty = DependencyProperty.Register(
        nameof(ApplicationTheme),
        typeof(ApplicationTheme),
        typeof(TitleBar),
        new PropertyMetadata(ApplicationTheme.Unknown));

    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>
    /// The title content property.
    /// </summary>
    public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
        nameof(TitleContent),
        typeof(object),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="ButtonsForeground"/>.
    /// </summary>
    public static readonly DependencyProperty ButtonsForegroundProperty = DependencyProperty.Register(
        nameof(ButtonsForeground),
        typeof(Brush),
        typeof(TitleBar),
        new FrameworkPropertyMetadata(
            SystemColors.ControlTextBrush,
            FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Property for <see cref="ButtonsBackground"/>.
    /// </summary>
    public static readonly DependencyProperty ButtonsBackgroundProperty = DependencyProperty.Register(
        nameof(ButtonsBackground),
        typeof(Brush),
        typeof(TitleBar),
        new FrameworkPropertyMetadata(SystemColors.ControlBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Property for <see cref="IsMaximized"/>.
    /// </summary>
    public static readonly DependencyProperty IsMaximizedProperty = DependencyProperty.Register(
        nameof(IsMaximized),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="ForceShutdown"/>.
    /// </summary>
    public static readonly DependencyProperty ForceShutdownProperty = DependencyProperty.Register(
        nameof(ForceShutdown),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="ShowMaximize"/>.
    /// </summary>
    public static readonly DependencyProperty ShowMaximizeProperty = DependencyProperty.Register(
        nameof(ShowMaximize),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="ShowMinimize"/>.
    /// </summary>
    public static readonly DependencyProperty ShowMinimizeProperty = DependencyProperty.Register(
        nameof(ShowMinimize),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="ShowHelp"/>.
    /// </summary>
    public static readonly DependencyProperty ShowHelpProperty = DependencyProperty.Register(
        nameof(ShowHelp),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="ShowClose"/>.
    /// </summary>
    public static readonly DependencyProperty ShowCloseProperty = DependencyProperty.Register(
        nameof(ShowClose),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="CanMaximize"/>.
    /// </summary>
    public static readonly DependencyProperty CanMaximizeProperty = DependencyProperty.Register(
        nameof(CanMaximize),
        typeof(bool),
        typeof(TitleBar),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(TitleBar),
        new PropertyMetadata(null));

    /// <summary>
    /// The content property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(ObservableCollection<FrameworkElement>),
        typeof(TitleBar));

    /// <summary>
    /// Property for <see cref="CloseWindowByDoubleClickOnIcon"/>.
    /// </summary>
    public static readonly DependencyProperty CloseWindowByDoubleClickOnIconProperty =
        DependencyProperty.Register(
            nameof(CloseWindowByDoubleClickOnIcon),
            typeof(bool),
            typeof(TitleBar),
            new PropertyMetadata(false));

    /// <summary>
    /// Routed event for <see cref="CloseClicked"/>.
    /// </summary>
    public static readonly RoutedEvent CloseClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(CloseClicked),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<TitleBar, RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>
    /// Routed event for <see cref="MaximizeClicked"/>.
    /// </summary>
    public static readonly RoutedEvent MaximizeClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(MaximizeClicked),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<TitleBar, RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>
    /// Routed event for <see cref="MinimizeClicked"/>.
    /// </summary>
    public static readonly RoutedEvent MinimizeClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(MinimizeClicked),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<TitleBar, RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>
    /// Routed event for <see cref="HelpClicked"/>.
    /// </summary>
    public static readonly RoutedEvent HelpClickedEvent = EventManager.RegisterRoutedEvent(
        nameof(HelpClicked),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<TitleBar, RoutedEventArgs>),
        typeof(TitleBar));

    /// <summary>
    /// Property for <see cref="TemplateButtonCommand"/>.
    /// </summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(IReactiveCommand),
        typeof(TitleBar),
        new PropertyMetadata(null));

    private const string ElementIcon = "PART_Icon";
    private const string ElementHelpButton = "PART_HelpButton";
    private const string ElementMinimizeButton = "PART_MinimizeButton";
    private const string ElementMaximizeButton = "PART_MaximizeButton";
    private const string ElementRestoreButton = "PART_RestoreButton";
    private const string ElementCloseButton = "PART_CloseButton";

    private static DpiScale? dpiScale;
    private readonly TitleBarButton[] _buttons = new TitleBarButton[4];
    private System.Windows.Window _currentWindow = null!;
    private ContentPresenter _icon = null!;
    private CompositeDisposable _disposables = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TitleBar"/> class.
    /// </summary>
    public TitleBar()
    {
        Content = [];
        SetValue(TemplateButtonCommandProperty, OnTemplateButtonClickCommand);
        dpiScale ??= VisualTreeHelper.GetDpi(this);

        _disposables.Add(this.Events().Loaded.Subscribe(OnLoaded));
        _disposables.Add(this.Events().Unloaded.Subscribe(OnUnloaded));
    }

    /// <summary>
    /// Event triggered after clicking close button.
    /// </summary>
    public event TypedEventHandler<TitleBar, RoutedEventArgs> CloseClicked
    {
        add => AddHandler(CloseClickedEvent, value);
        remove => RemoveHandler(CloseClickedEvent, value);
    }

    /// <summary>
    /// Event triggered after clicking maximize or restore button.
    /// </summary>
    public event TypedEventHandler<TitleBar, RoutedEventArgs> MaximizeClicked
    {
        add => AddHandler(MaximizeClickedEvent, value);
        remove => RemoveHandler(MaximizeClickedEvent, value);
    }

    /// <summary>
    /// Event triggered after clicking minimize button.
    /// </summary>
    public event TypedEventHandler<TitleBar, RoutedEventArgs> MinimizeClicked
    {
        add => AddHandler(MinimizeClickedEvent, value);
        remove => RemoveHandler(MinimizeClickedEvent, value);
    }

    /// <summary>
    /// Event triggered after clicking help button
    /// </summary>
    public event TypedEventHandler<TitleBar, RoutedEventArgs> HelpClicked
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

    /// <summary>
    /// Gets or sets title displayed on the left.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed in the <see cref="TitleBar"/>.
    /// </summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the title.
    /// </summary>
    /// <value>
    /// The content of the title.
    /// </value>
    public object TitleContent
    {
        get => GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }

    /// <summary>
    /// Gets or sets foreground of the navigation buttons.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public Brush ButtonsForeground
    {
        get => (Brush)GetValue(ButtonsForegroundProperty);
        set => SetValue(ButtonsForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets background of the navigation buttons when hovered.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public Brush ButtonsBackground
    {
        get => (Brush)GetValue(ButtonsBackgroundProperty);
        set => SetValue(ButtonsBackgroundProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether gets or sets information whether the current window is maximized.
    /// </summary>
    public bool IsMaximized
    {
        get => (bool)GetValue(IsMaximizedProperty);
        internal set => SetValue(IsMaximizedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether the controls affect main application window.
    /// </summary>
    public bool ForceShutdown
    {
        get => (bool)GetValue(ForceShutdownProperty);
        set => SetValue(ForceShutdownProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether to show maximize button.
    /// </summary>
    public bool ShowMaximize
    {
        get => (bool)GetValue(ShowMaximizeProperty);
        set => SetValue(ShowMaximizeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether to show minimize button.
    /// </summary>
    public bool ShowMinimize
    {
        get => (bool)GetValue(ShowMinimizeProperty);
        set => SetValue(ShowMinimizeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether to show help button.
    /// </summary>
    public bool ShowHelp
    {
        get => (bool)GetValue(ShowHelpProperty);
        set => SetValue(ShowHelpProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether to show close button.
    /// </summary>
    public bool ShowClose
    {
        get => (bool)GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether enables or disables the maximize functionality if disables the MaximizeActionOverride action won't be called.
    /// </summary>
    public bool CanMaximize
    {
        get => (bool)GetValue(CanMaximizeProperty);
        set => SetValue(CanMaximizeProperty, value);
    }

    /// <summary>
    /// Gets or sets titlebar icon.
    /// </summary>
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>
    /// The content.
    /// </value>
    public ObservableCollection<FrameworkElement> Content
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether enables or disable closing the window by double clicking on the icon.
    /// </summary>
    public bool CloseWindowByDoubleClickOnIcon
    {
        get => (bool)GetValue(CloseWindowByDoubleClickOnIconProperty);
        set => SetValue(CloseWindowByDoubleClickOnIconProperty, value);
    }

    /// <summary>
    /// Gets command triggered after clicking the titlebar button.
    /// </summary>
    public IReactiveCommand TemplateButtonCommand => (IReactiveCommand)GetValue(TemplateButtonCommandProperty);

    /// <summary>
    /// Gets or sets lets you override the behavior of the Maximize/Restore button with an <see cref="Action"/>.
    /// </summary>
    public Action<TitleBar, System.Windows.Window>? MaximizeActionOverride { get; set; }

    /// <summary>
    /// Gets or sets lets you override the behavior of the Minimize button with an <see cref="Action"/>.
    /// </summary>
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

        _buttons[0] = maximizeButton;
        _buttons[1] = minimizeButton;
        _buttons[2] = closeButton;
        _buttons[3] = helpButton;
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        ApplicationTheme = ApplicationThemeManager.GetAppTheme();
        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    /// <summary>
    /// Called when [loaded].
    /// </summary>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    /// <exception cref="ArgumentNullException">Window is null.</exception>
    protected virtual void OnLoaded(RoutedEventArgs e)
    {
        if (DesignerHelper.IsInDesignMode)
        {
            return;
        }

        _currentWindow = System.Windows.Window.GetWindow(this) ?? throw new ArgumentNullException("Window is null");
        AddWindowEvents(_currentWindow);
    }

    /// <summary>
    /// This virtual method is triggered when the app's theme changes.
    /// </summary>
    /// <param name="currentApplicationTheme">The current application theme.</param>
    /// <param name="systemAccent">The system accent.</param>
    protected virtual void OnThemeChanged(
        ApplicationTheme currentApplicationTheme,
        Color systemAccent)
    {
        Debug.WriteLine(
            $"INFO | {typeof(TitleBar)} received theme -  {currentApplicationTheme}",
            "CrissCross.WPF.UI.TitleBar");

        ApplicationTheme = currentApplicationTheme;
    }

    /// <summary>
    /// Show 'SystemMenu' on mouse right button up.
    /// </summary>
    private void TitleBar_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        var point = PointToScreen(e.GetPosition(this));
        SystemCommands.ShowSystemMenu(_currentWindow, new Point(point.X / dpiScale!.Value.DpiScaleX, point.Y / dpiScale.Value.DpiScaleY));
    }

    private void OnUnloaded(RoutedEventArgs e)
    {
        _disposables.Dispose();

        ApplicationThemeManager.Changed -= OnThemeChanged;
    }

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

    private void MinimizeWindow()
    {
        if (MinimizeActionOverride is not null)
        {
            MinimizeActionOverride(this, _currentWindow);

            return;
        }

        _currentWindow.WindowState = WindowState.Minimized;
    }

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

    private void OnParentWindowStateChanged(object? sender, EventArgs e)
    {
        if (IsMaximized != (_currentWindow.WindowState == WindowState.Maximized))
        {
            IsMaximized = _currentWindow.WindowState == WindowState.Maximized;
        }
    }

    [ReactiveCommand]
    private void OnTemplateButtonClick(TitleBarButtonType buttonType)
    {
        switch (buttonType)
        {
            case TitleBarButtonType.Maximize
            or TitleBarButtonType.Restore:
                RaiseEvent(new RoutedEventArgs(MaximizeClickedEvent, this));
                MaximizeWindow();
                break;

            case TitleBarButtonType.Close:
                RaiseEvent(new RoutedEventArgs(CloseClickedEvent, this));
                CloseWindow();
                break;

            case TitleBarButtonType.Minimize:
                RaiseEvent(new RoutedEventArgs(MinimizeClickedEvent, this));
                MinimizeWindow();
                break;

            case TitleBarButtonType.Help:
                RaiseEvent(new RoutedEventArgs(HelpClickedEvent, this));
                break;
        }
    }

    /// <summary>
    ///     Listening window hooks after rendering window content to SizeToContent support.
    /// </summary>
    private void AddWindowEvents(System.Windows.Window window)
    {
        window.StateChanged += OnParentWindowStateChanged;
        var handle = new WindowInteropHelper(window).Handle;
        var windowSource = HwndSource.FromHwnd(handle) ?? throw new ArgumentNullException("Window source is null");
        windowSource.AddHook(HwndSourceHook);
    }

    private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = (User32.WM)msg;

        if (
            message
            is not (
                User32.WM.NCHITTEST
                or User32.WM.NCMOUSELEAVE
                or User32.WM.NCLBUTTONDOWN
                or User32.WM.NCLBUTTONUP))
        {
            return IntPtr.Zero;
        }

        foreach (var button in _buttons)
        {
            if (!button.ReactToHwndHook(message, lParam, out var returnIntPtr))
            {
                continue;
            }

            // It happens that the background is not removed from the buttons and you can make all the buttons are in the IsHovered=true
            // It cleans up
            foreach (var anotherButton in _buttons)
            {
                if (anotherButton == button)
                {
                    continue;
                }

                if (anotherButton.IsHovered && button.IsHovered)
                {
                    anotherButton.RemoveHover();
                }
            }

            handled = true;
            return returnIntPtr;
        }

        var isMouseOverHeaderContent = false;

        if (message == User32.WM.NCHITTEST && Header is UIElement headerUiElement)
        {
            isMouseOverHeaderContent = headerUiElement.IsMouseOverElement(lParam);
        }

        if (message == User32.WM.NCHITTEST && TitleContent is UIElement titleUiElement)
        {
            isMouseOverHeaderContent = titleUiElement.IsMouseOverElement(lParam);
        }

        switch (message)
        {
            case User32.WM.NCHITTEST
                when CloseWindowByDoubleClickOnIcon && _icon.IsMouseOverElement(lParam):
                handled = true;

                // Ideally, clicking on the icon should open the system menu, but when the system menu is opened manually, double-clicking on the icon does not close the window
                return (IntPtr)User32.WM_NCHITTEST.HTSYSMENU;
            case User32.WM.NCHITTEST when this.IsMouseOverElement(lParam) && !isMouseOverHeaderContent:
                handled = true;
                return (IntPtr)User32.WM_NCHITTEST.HTCAPTION;
            default:
                return IntPtr.Zero;
        }
    }

    private T GetTemplateChild<T>(string name)
        where T : DependencyObject
    {
        var element = GetTemplateChild(name);

        if (element is null)
        {
            throw new ArgumentNullException($"{name} is null");
        }

        return (T)element;
    }
}
