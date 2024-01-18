// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CP.BBCode.WPF;
using CP.Extensions.Hosting.Wpf;
using CP.WPF.Controls;
using ReactiveUI;
using Button = Wpf.Ui.Controls.Button;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace CrissCross.WPF.UI;

/// <summary>
/// Represents a Modern UI styled window.
/// </summary>
public class ModernWindow
    : NavigationWindow, IWpfShell, IListenForMessages, ICanShowMessages, IHaveAppBar, ICancelable
{
    /// <summary>
    /// The application bar enabled property.
    /// </summary>
    public static readonly DependencyProperty AppBarEnabledProperty = DependencyProperty.Register(nameof(AppBarEnabled), typeof(bool), typeof(ModernWindow), new PropertyMetadata(true));

    /// <summary>
    /// Holds AppBar open until explicitly closed.
    /// </summary>
    public static readonly DependencyProperty AppBarIsStickyProperty = DependencyProperty.Register(nameof(AppBarIsSticky), typeof(bool), typeof(ModernWindow), new PropertyMetadata(false));

    /// <summary>
    /// Recommended Height 88.
    /// </summary>
    public static readonly DependencyProperty AppBarLeftProperty = DependencyProperty.Register(nameof(AppBarLeft), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// Recommended Height 88.
    /// </summary>
    public static readonly DependencyProperty AppBarRightProperty = DependencyProperty.Register(nameof(AppBarRight), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// Identifies the BackgroundContent dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register(nameof(BackgroundContent), typeof(object), typeof(ModernWindow));

    /// <summary>
    /// Identifies the Buttons dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(nameof(Buttons), typeof(IEnumerable<Button>), typeof(ModernWindow));

    /// <summary>
    /// The foreground content property.
    /// </summary>
    public static readonly DependencyProperty ForegroundContentProperty = DependencyProperty.Register(nameof(ForegroundContent), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// Identifies the IsTitleVisible dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register(nameof(IsTitleVisible), typeof(bool), typeof(ModernWindow), new PropertyMetadata(false));

    /// <summary>
    /// Identifies the LogoData dependency property.
    /// </summary>
    public static readonly DependencyProperty LogoDataProperty = DependencyProperty.Register(nameof(LogoData), typeof(Geometry), typeof(ModernWindow));

    /// <summary>
    /// Identifies the Logo dependency property.
    /// </summary>
    public static readonly DependencyProperty LogoProperty = DependencyProperty.Register(nameof(Logo), typeof(ImageSource), typeof(ModernWindow));

    /// <summary>
    /// The main menu visible property.
    /// </summary>
    public static readonly DependencyProperty MainMenuVisibleProperty = DependencyProperty.Register(nameof(MainMenuVisible), typeof(Visibility), typeof(ModernWindow), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The main title font property.
    /// </summary>
    public static readonly DependencyProperty MainTitleFontProperty = DependencyProperty.Register(nameof(MainTitleFont), typeof(FontFamily), typeof(ModernWindow), new PropertyMetadata(new FontFamily("Segoe UI")));

    /// <summary>
    /// The main title property.
    /// </summary>
    public static readonly DependencyProperty MainTitleProperty = DependencyProperty.Register(nameof(MainTitle), typeof(string), typeof(ModernWindow), new PropertyMetadata("CrissCross"));

    /// <summary>
    /// Identifies the MenuLinkGroups dependency property.
    /// </summary>
    public static readonly DependencyProperty MainMenuProperty = DependencyProperty.Register(nameof(MainMenu), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// The message content property.
    /// </summary>
    public static readonly DependencyProperty MessageContentProperty = DependencyProperty.Register(nameof(MessageContent), typeof(object), typeof(ModernWindow), new PropertyMetadata(0));

    /// <summary>
    /// Title of Message.
    /// </summary>
    public static readonly DependencyProperty MessageTitleProperty = DependencyProperty.Register(nameof(MessageTitle), typeof(string), typeof(ModernWindow), new PropertyMetadata(string.Empty));

    /// <summary>
    /// The message visible property.
    /// </summary>
    public static readonly DependencyProperty MessageVisibleProperty = DependencyProperty.Register(nameof(MessageVisible), typeof(Visibility), typeof(ModernWindow), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// The navigation bar back button visible property.
    /// </summary>
    public static readonly DependencyProperty NavBarBackButtonVisibleProperty = DependencyProperty.Register(nameof(NavBarBackButtonVisible), typeof(Visibility), typeof(ModernWindow), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The navigation bar left property.
    /// </summary>
    public static readonly DependencyProperty NavBarLeftProperty = DependencyProperty.Register(nameof(NavBarLeft), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// The navigation bar logo visible property.
    /// </summary>
    public static readonly DependencyProperty NavBarLogoVisibleProperty = DependencyProperty.Register(nameof(NavBarLogoVisible), typeof(Visibility), typeof(ModernWindow), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The navigation bar property.
    /// </summary>
    public static readonly DependencyProperty NavBarProperty = DependencyProperty.Register(nameof(NavBar), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// The navigation bar visible property.
    /// </summary>
    public static readonly DependencyProperty NavBarVisibleProperty = DependencyProperty.Register(nameof(NavBarVisible), typeof(Visibility), typeof(ModernWindow), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The status bar property.
    /// </summary>
    public static readonly DependencyProperty StatusBarProperty = DependencyProperty.Register(nameof(StatusBar), typeof(ObservableCollection<FrameworkElement>), typeof(ModernWindow));

    /// <summary>
    /// The title logo data property.
    /// </summary>
    public static readonly DependencyProperty TitleLogoDataProperty = DependencyProperty.Register(nameof(TitleLogoData), typeof(Geometry), typeof(ModernWindow));

    /// <summary>
    /// The title logo property.
    /// </summary>
    public static readonly DependencyProperty TitleLogoProperty = DependencyProperty.Register(nameof(TitleLogo), typeof(ImageSource), typeof(ModernWindow));

    /// <summary>
    /// The title margin property.
    /// </summary>
    public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register(nameof(TitleMargin), typeof(Thickness), typeof(ModernWindow), new PropertyMetadata(new Thickness(0)));

    /// <summary>
    /// The browse back property.
    /// </summary>
    public static readonly DependencyProperty BrowseBackProperty = DependencyProperty.Register(nameof(BrowseBack), typeof(ICommand), typeof(ModernWindow), new PropertyMetadata(null));

    private readonly ReactiveCommand<Unit, MessageBoxResult>? _closeOkCommand;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom0Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom1Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom2Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom3Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom4Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom5Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom6Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom7Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom8Command;
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom9Command;
    private readonly ReplaySubject<string> _busyStatusTextSubject = new(1);
    private readonly ReplaySubject<Visibility> _busyVisibilitySubject = new(1);
    private readonly CompositeDisposable _cleanUp = [];
    private Grid? _appBar;
    private bool _appBarVisible;
    private Storyboard? _backgroundAnimation;
    private Button? _cancelButton;
    private Button? _closeButton;
    private CustomMessageBoxResult _customMessageBoxResult = CustomMessageBoxResult.None;
    private Storyboard? _hide;
    private Grid? _message;
    private MessageBoxResult _messageBoxResult = MessageBoxResult.None;
    private bool _mouseIsOverAppBar;
    private bool _mouseIsOverMessage;
    private Button? _noButton;
    private Button? _okbutton;
    private Storyboard? _show;
    private Button? _yesButton;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModernWindow"/> class.
    /// </summary>
    public ModernWindow()
    {
        _cleanUp.Add(_busyVisibilitySubject);
        _cleanUp.Add(_busyStatusTextSubject);

        DefaultStyleKey = typeof(ModernWindow);

        NavBar = [];
        StatusBar = [];
        AppBarLeft = [];
        AppBarRight = [];
        NavBarLeft = [];
        ForegroundContent = [];
        SetCurrentValue(MainMenuProperty, new ObservableCollection<FrameworkElement>());

        // associate window commands with this instance
        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));

        // Message
        _closeOkCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.OK);
        CloseTrueCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.Yes);
        CloseFalseCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.No);
        CloseCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.Cancel);
        _custom0Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom0);
        _custom1Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom1);
        _custom2Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom2);
        _custom3Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom3);
        _custom4Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom4);
        _custom5Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom5);
        _custom6Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom6);
        _custom7Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom7);
        _custom8Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom8);
        _custom9Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom9);

        Buttons = new[] { CloseButton };

        // listen for theme changes
        PreviewMouseDown += ModernWindow_PreviewMouseDown;
        PreviewKeyDown += ModernWindow_PreviewKeyDown;
    }

    /// <summary>
    /// Gets or sets the browse back.
    /// </summary>
    /// <value>
    /// The browse back.
    /// </value>
    public ICommand BrowseBack
    {
        get => (ICommand)GetValue(BrowseBackProperty);
        set => SetValue(BrowseBackProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [application bar enabled].
    /// </summary>
    /// <value><c>true</c> if [application bar enabled]; otherwise, <c>false</c> .</value>
    [Description("Gets or Sets the Visibility of the Nav Bar")]
    [Category("CrissCross Metro")]
    public bool AppBarEnabled
    {
        get => (bool)GetValue(AppBarEnabledProperty);

        set
        {
            SetValue(AppBarEnabledProperty, value);
            if (!value)
            {
                HideAppBar();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether [application bar is sticky].
    /// </summary>
    /// <value><c>true</c> if [application bar is sticky]; otherwise, <c>false</c> .</value>
    [Description("Gets the AppBar Sticky state.")]
    [Category("CrissCross Metro")]
    public bool AppBarIsSticky
    {
        get => (bool)GetValue(AppBarIsStickyProperty);

        private set => SetValue(AppBarIsStickyProperty, value);
    }

    /// <summary>
    /// Gets or sets the AppBarLeft content.
    /// </summary>
    [Description("Gets or sets the AppBarLeft content.")]
    [Category("CrissCross Metro")]
    public ObservableCollection<FrameworkElement> AppBarLeft
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(AppBarLeftProperty);
        set => SetValue(AppBarLeftProperty, value);
    }

    /// <summary>
    /// Gets or sets the AppBarRight content.
    /// </summary>
    [Description("Gets or sets the AppBarRight content.")]
    [Category("CrissCross Metro")]
    public ObservableCollection<FrameworkElement> AppBarRight
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(AppBarRightProperty);
        set => SetValue(AppBarRightProperty, value);
    }

    /// <summary>
    /// Gets or sets the background content of this window instance.
    /// </summary>
    [Description("Gets or sets the Background Content.")]
    [Category("CrissCross Metro")]
    public object BackgroundContent
    {
        get => GetValue(BackgroundContentProperty);
        set => SetValue(BackgroundContentProperty, value);
    }

    /// <summary>
    /// Gets the busy calls.
    /// </summary>
    /// <value>The busy calls.</value>
    public Dictionary<string, string> BusyCalls { get; } = [];

    /// <summary>
    /// Gets or sets the dialog buttons.
    /// </summary>
    public IEnumerable<Button> Buttons
    {
        get => (IEnumerable<Button>)GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    /// <summary>
    /// Gets the Cancel button.
    /// </summary>
    public Button CancelButton => _cancelButton ??= CreateDialogButton("Cancel", false, true, CloseCommand);

    /// <summary>
    /// Gets the Close button.
    /// </summary>
    public Button CloseButton => _closeButton ??= CreateDialogButton("Close", true, false, CloseCommand);

    /// <summary>
    /// Gets the close window command that sets the dialog result to a null value.
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <summary>
    /// Gets the close window command that sets the dialog result to false.
    /// </summary>
    public ICommand CloseFalseCommand { get; }

    /// <summary>
    /// Gets the close window command that sets the dialog result to True.
    /// </summary>
    public ICommand CloseTrueCommand { get; }

    /// <summary>
    /// Gets the custom button0.
    /// </summary>
    /// <value>The custom button0.</value>
    public Button? CustomButton0 { get; private set; }

    /// <summary>
    /// Gets the custom button1.
    /// </summary>
    /// <value>The custom button1.</value>
    public Button? CustomButton1 { get; private set; }

    /// <summary>
    /// Gets the custom button2.
    /// </summary>
    /// <value>The custom button2.</value>
    public Button? CustomButton2 { get; private set; }

    /// <summary>
    /// Gets the custom button3.
    /// </summary>
    /// <value>The custom button3.</value>
    public Button? CustomButton3 { get; private set; }

    /// <summary>
    /// Gets the custom button4.
    /// </summary>
    /// <value>The custom button4.</value>
    public Button? CustomButton4 { get; private set; }

    /// <summary>
    /// Gets the custom button5.
    /// </summary>
    /// <value>The custom button5.</value>
    public Button? CustomButton5 { get; private set; }

    /// <summary>
    /// Gets the custom button6.
    /// </summary>
    /// <value>The custom button6.</value>
    public Button? CustomButton6 { get; private set; }

    /// <summary>
    /// Gets the custom button7.
    /// </summary>
    /// <value>The custom button7.</value>
    public Button? CustomButton7 { get; private set; }

    /// <summary>
    /// Gets the custom button8.
    /// </summary>
    /// <value>The custom button8.</value>
    public Button? CustomButton8 { get; private set; }

    /// <summary>
    /// Gets the custom button9.
    /// </summary>
    /// <value>The custom button9.</value>
    public Button? CustomButton9 { get; private set; }

    /// <summary>
    /// Gets or sets the content of the foreground.
    /// </summary>
    /// <value>The content of the foreground.</value>
    [Description("Gets or sets the Foreground Content.")]
    [Category("CrissCross Metro")]
    public ObservableCollection<FrameworkElement> ForegroundContent
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(ForegroundContentProperty);
        set => SetValue(ForegroundContentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window title is visible in the UI.
    /// </summary>
    [Description("Gets or sets if the Title is visible.")]
    [Category("AICS MVVM")]
    public bool IsTitleVisible
    {
        get => (bool)GetValue(IsTitleVisibleProperty);
        set => SetValue(IsTitleVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the Image for the logo displayed in the title area of the window.
    /// </summary>
    [Description("Gets or sets the Image for the logo displayed in the title area of the window.")]
    [Category("CrissCross Metro")]
    public ImageSource Logo
    {
        get => (ImageSource)GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }

    /// <summary>
    /// Gets or sets the path data for the logo displayed in the title area of the window.
    /// </summary>
    [Description("Gets or sets the path data for the logo displayed in the title area of the window.")]
    [Category("CrissCross Metro")]
    public Geometry LogoData
    {
        get => (Geometry)GetValue(LogoDataProperty);
        set => SetValue(LogoDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the main menu visible.
    /// </summary>
    /// <value>The main menu visible.</value>
    [Description("Gets or Sets the Visibility of the Main Navigation Menu (MenuLinkGroups)")]
    [Category("CrissCross Metro")]
    public Visibility MainMenuVisible
    {
        get => (Visibility)GetValue(MainMenuVisibleProperty);
        set => SetValue(MainMenuVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the main title.
    /// </summary>
    /// <value>The main title.</value>
    [Description("Gets or sets the Main Page Title.")]
    [Category("CrissCross Metro")]
    public string MainTitle
    {
        get => (string)GetValue(MainTitleProperty);
        set => SetValue(MainTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the main title font.
    /// </summary>
    /// <value>The main title font.</value>
    [Description("Gets or sets the Title Font.")]
    [Category("CrissCross Metro")]
    public FontFamily MainTitleFont
    {
        get => (FontFamily)GetValue(MainTitleFontProperty);
        set => SetValue(MainTitleFontProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of link groups shown in the window's menu.
    /// </summary>
    /// <value>The menu link groups.</value>
    [Description("Gets or sets the collection of link groups shown in the window's menu.")]
    [Category("AICS MVVM")]
    public ObservableCollection<FrameworkElement> MainMenu
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(MainMenuProperty);
        set => SetValue(MainMenuProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    /// <value>The content of the message.</value>
    [Description("Gets or sets the Message content.")]
    [Category("AICS MVVM")]
    public object MessageContent
    {
        get => GetValue(MessageContentProperty);
        set => SetValue(MessageContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the message title.
    /// </summary>
    /// <value>The message title.</value>
    [Description("Gets or sets the Message Title.")]
    [Category("AICS MVVM")]
    public string MessageTitle
    {
        get => (string)GetValue(MessageTitleProperty);
        set => SetValue(MessageTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the message visible.
    /// </summary>
    /// <value>The message visible.</value>
    [Description("Gets or sets the Message visibility.")]
    [Category("AICS MVVM")]
    public Visibility MessageVisible
    {
        get => (Visibility)GetValue(MessageVisibleProperty);
        set => SetValue(MessageVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the Navigation Bar content.
    /// </summary>
    /// <value>The navigation bar.</value>
    [Description("Gets or sets the NavBar content.")]
    [Category("CrissCross Metro")]
    public ObservableCollection<FrameworkElement> NavBar
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(NavBarProperty);
        set => SetValue(NavBarProperty, value);
    }

    /// <summary>
    /// Gets or sets Visibility of Navigation Bar Back Button.
    /// </summary>
    /// <value>The navigation bar back button visible.</value>
    [Description("Gets or Sets Visibility of Nav Bar Back Button")]
    [Category("CrissCross Metro")]
    public Visibility NavBarBackButtonVisible
    {
        get => (Visibility)GetValue(NavBarBackButtonVisibleProperty);

        set => SetValue(NavBarBackButtonVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the Navigation Bar Left content.
    /// </summary>
    [Description("Gets or sets the NavBarLeft content.")]
    [Category("CrissCross Metro")]
    public ObservableCollection<FrameworkElement> NavBarLeft
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(NavBarLeftProperty);
        set => SetValue(NavBarLeftProperty, value);
    }

    /// <summary>
    /// Gets or sets Visibility of Navigation Bar Logo.
    /// </summary>
    /// <value>The navigation bar logo visible.</value>
    [Description("Gets or Sets Visibility of Nav Bar Logo")]
    [Category("CrissCross Metro")]
    public Visibility NavBarLogoVisible
    {
        get => (Visibility)GetValue(NavBarLogoVisibleProperty);
        set => SetValue(NavBarLogoVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the Visibility of the Navigation Bar.
    /// </summary>
    /// <value>The navigation bar visible.</value>
    [Description("Gets or sets Sets the Visibility of the Nav Bar")]
    [Category("CrissCross Metro")]
    public Visibility NavBarVisible
    {
        get => (Visibility)GetValue(NavBarVisibleProperty);
        set => SetValue(NavBarVisibleProperty, value);
    }

    /// <summary>
    /// Gets the No button.
    /// </summary>
    public Button NoButton => _noButton ??= CreateDialogButton("No", false, true, CloseFalseCommand);

    /// <summary>
    /// Gets the OK button.
    /// </summary>
    public Button OkButton => _okbutton ??= CreateDialogButton("Ok", true, false, _closeOkCommand);

    /// <summary>
    /// Gets or sets the StatusBar content.
    /// </summary>
    /// <value>The status bar.</value>
    [Description("Gets or sets the StatusBar content.")]
    [Category("CrissCross Metro")]
    public ObservableCollection<FrameworkElement> StatusBar
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(StatusBarProperty);
        set => SetValue(StatusBarProperty, value);
    }

    /// <summary>
    /// Gets or sets the title logo.
    /// </summary>
    /// <value>The title logo.</value>
    [Description("Gets or sets the Title Logo Image.")]
    [Category("CrissCross Metro")]
    public ImageSource? TitleLogo
    {
        get => (ImageSource?)GetValue(TitleLogoProperty);
        set => SetValue(TitleLogoProperty, value);
    }

    /// <summary>
    /// Gets or sets the title logo data.
    /// </summary>
    /// <value>The title logo data.</value>
    [Description("Gets or sets the Title logo.")]
    [Category("AICS MVVM")]
    public Geometry? TitleLogoData
    {
        get => (Geometry?)GetValue(TitleLogoDataProperty);
        set => SetValue(TitleLogoDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the title margin.
    /// </summary>
    /// <value>The title margin.</value>
    [Description("Gets or sets the Title Margin.")]
    [Category("CrissCross Metro")]
    public Thickness TitleMargin
    {
        get => (Thickness)GetValue(TitleMarginProperty);
        set => SetValue(TitleMarginProperty, value);
    }

    /// <summary>
    /// Gets the Yes button.
    /// </summary>
    public Button YesButton => _yesButton ??= CreateDialogButton("Yes", true, false, CloseTrueCommand);

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    public bool IsDisposed => _cleanUp.IsDisposed;

    /// <summary>
    /// Gets the status bar status text.
    /// </summary>
    /// <value>The status bar status text.</value>
    protected IObservable<string> BusyStatusText => _busyStatusTextSubject.Publish().RefCount();

    /// <summary>
    /// Gets the busy visibility.
    /// </summary>
    /// <value>The busy visibility.</value>
    protected IObservable<Visibility> BusyVisibility => _busyVisibilitySubject.Publish().RefCount();

    /// <summary>
    /// Themes the changed.
    /// </summary>
    /// <param name="theme">The theme.</param>
    public virtual void ThemeChanged(string theme)
    {
    }

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal
    /// processes call System.Windows.FrameworkElement.ApplyTemplate().
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // retrieve BackgroundAnimation storyboard
        if (GetTemplateChild("WindowBorder") is Border border)
        {
            _backgroundAnimation = border.Resources["BackgroundAnimation"] as Storyboard;

            _backgroundAnimation?.Begin();
        }

        BrowseBack = ReactiveCommand.Create<object>(o => this.NavigateBack(o), this.CanNavigateBack());
        var backButton = (AppBarButton)Template.FindName("BackButton", this);
        if (backButton != null)
        {
            backButton.Command = BrowseBack;
        }

        _appBar = (Grid)Template.FindName("BottomAppBar", this);
        if (_appBar != null)
        {
            _hide = _appBar.Resources["Hide"] as Storyboard;
            _show = _appBar.Resources["Show"] as Storyboard;
            _appBar.MouseEnter += AppBar_MouseEnter;
            _appBar.MouseLeave += AppBar_MouseLeave;
        }

        HideAppBar();
        _message = (Grid)Template.FindName("Message", this);
        if (_message != null)
        {
            _message.MouseEnter += (o, e) => _mouseIsOverMessage = true;
            _message.MouseLeave += (o, e) => _mouseIsOverMessage = false;
        }

        // Set up magic functions
        this.ListenForMessages(message => MessageBoxShow(message.Item1, message.Item2, message.Item3));
        ////this.ListenForCustomMessages(
        ////    async message =>
        ////    await MessageBoxShow(
        ////                         message.Item1,
        ////                         message.Item2,
        ////                         message.Item3,
        ////                         message.Item4,
        ////                         message.Item5,
        ////                         message.Item6,
        ////                         message.Item7,
        ////                         message.Rest.Item1,
        ////                         message.Rest.Item2,
        ////                         message.Rest.Item3,
        ////                         message.Rest.Item4).ConfigureAwait(false));
        this.AppBarIsStickyListener(() => AppBarIsSticky, isSticky => AppBarIsSticky = isSticky);
        this.ShowAppBarListener(ShowAppBar);
        this.HideAppBarListener(HideAppBar);
        this.AppBarLeftListener(() => AppBarLeft);
        this.AppBarRightListener(() => AppBarRight);
        this.NavBarLeftListener(() => NavBarLeft);
        this.NavBarListener(() => NavBar);
        this.MainMenuListener(() => MainMenu);
        this.ListenForBusy(IsBusy);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _busyStatusTextSubject?.Dispose();
                _busyVisibilitySubject?.Dispose();
                _closeOkCommand?.Dispose();
                _custom0Command?.Dispose();
                _custom1Command?.Dispose();
                _custom2Command?.Dispose();
                _custom3Command?.Dispose();
                _custom4Command?.Dispose();
                _custom5Command?.Dispose();
                _custom6Command?.Dispose();
                _custom7Command?.Dispose();
                _custom8Command?.Dispose();
                _custom9Command?.Dispose();
                _cleanUp.Dispose();
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Creates the dialog button.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="isDefault">if set to <c>true</c> [is default].</param>
    /// <param name="isCancel">if set to <c>true</c> [is cancel].</param>
    /// <param name="command">The command.</param>
    /// <returns>A Button.</returns>
    private static Button CreateDialogButton(string content, bool isDefault, bool isCancel, ICommand? command) => new()
    {
        Content = content,
        Command = command,
        IsDefault = isDefault,
        IsCancel = isCancel,
        MinHeight = 21,
        MinWidth = 65,
        Margin = new Thickness(4, 0, 0, 0)
    };

    /// <summary>
    /// Hides the application bar.
    /// </summary>
    private void HideAppBar()
    {
        if (_hide != null && _appBarVisible)
        {
            _hide.Begin();
            _appBarVisible = false;
        }
    }

    /// <summary>
    /// Determines whether the specified call is busy.
    /// </summary>
    /// <param name="call">The call.</param>
    /// <param name="busy">if set to <c>true</c> [busy].</param>
    /// <param name="message">The message.</param>
    private void IsBusy(string call, bool busy, string message = "")
    {
        if (busy && !BusyCalls.ContainsKey(call))
        {
            BusyCalls.Add(call, message);
        }
        else
        {
            BusyCalls.Remove(call);
        }

        if (BusyCalls.Count == 0)
        {
            _busyVisibilitySubject.OnNext(Visibility.Collapsed);
            _busyStatusTextSubject.OnNext(string.Empty);
        }
        else
        {
            _busyVisibilitySubject.OnNext(Visibility.Visible);
            _busyStatusTextSubject.OnNext(BusyCalls[BusyCalls.Keys.Last()]);
        }
    }

    /// <summary>
    /// Messages the box show.
    /// </summary>
    /// <param name="bbcode">The bbcode.</param>
    /// <param name="title">The title.</param>
    /// <param name="custom0">The custom0.</param>
    /// <param name="custom1">The custom1.</param>
    /// <param name="custom2">The custom2.</param>
    /// <param name="custom3">The custom3.</param>
    /// <param name="custom4">The custom4.</param>
    /// <param name="custom5">The custom5.</param>
    /// <param name="custom6">The custom6.</param>
    /// <param name="custom7">The custom7.</param>
    /// <param name="custom8">The custom8.</param>
    /// <param name="custom9">The custom9.</param>
    /// <returns>A Value.</returns>
    private async Task<CustomMessageBoxResult> MessageBoxShow(string bbcode, string title, string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null)
    {
        if (custom0 == null)
        {
            throw new ArgumentNullException(nameof(custom0));
        }

        // If message box is already shown wait for it to be actioned
        while (MessageVisible == Visibility.Visible)
        {
            await Task.Delay(100).ConfigureAwait(true);
        }

        await Dispatcher.InvokeAsync(() =>
        {
            MessageTitle = title;
            MessageContent = new BBCodeBlock { BBCode = bbcode, Margin = new Thickness(0, 0, 0, 8) };
            Buttons = GetButtons(custom0, custom1, custom2, custom3, custom4, custom5, custom6, custom7, custom8, custom9);
            MessageVisible = Visibility.Visible;
        });

        // Reset the result
        _customMessageBoxResult = CustomMessageBoxResult.None;

        // Wait for response
        while (_customMessageBoxResult == CustomMessageBoxResult.None)
        {
            await Task.Delay(100).ConfigureAwait(true);
        }

        await Dispatcher.InvokeAsync(() =>

        // hide the message box and return result
        MessageVisible = Visibility.Collapsed);
        return _customMessageBoxResult;
    }

    /// <summary>
    /// Displays a dismiss-able message-box. Click outside of the message area to dismiss.
    /// </summary>
    /// <param name="bbcode">The text. Use BBCode to style the text.</param>
    /// <param name="title">The title.</param>
    /// <param name="button">The buttons to show.</param>
    /// <returns>Task of MessageBoxResult.</returns>
    private async Task<MessageBoxResult> MessageBoxShow(string bbcode, string title = "", MessageBoxButton button = MessageBoxButton.OK)
    {
        // If message box is already shown wait for it to be actioned
        while (MessageVisible == Visibility.Visible)
        {
            await Task.Delay(100).ConfigureAwait(true);
        }

        await Dispatcher.InvokeAsync(() =>
        {
            MessageTitle = title;
            MessageContent = new BBCodeBlock { BBCode = bbcode, Margin = new Thickness(0, 0, 0, 8) };
            Buttons = GetButtons(button);
            MessageVisible = Visibility.Visible;
        });

        // Reset the result
        _messageBoxResult = MessageBoxResult.None;

        // Wait for response
        while (_messageBoxResult == MessageBoxResult.None)
        {
            await Task.Delay(100).ConfigureAwait(true);
        }

        await Dispatcher.InvokeAsync(() =>

            // hide the message box and return result
            MessageVisible = Visibility.Collapsed);
        return _messageBoxResult;
    }

    /// <summary>
    /// Shows the application bar.
    /// </summary>
    /// <param name="isSticky">if set to <c>true</c> [is sticky].</param>
    private void ShowAppBar(bool isSticky = false)
    {
        if (!AppBarEnabled)
        {
            HideAppBar();
            return;
        }

        AppBarIsSticky = isSticky;
        if (_show != null && !_appBarVisible)
        {
            _show.Begin();
            _appBarVisible = true;
        }
    }

    /// <summary>
    /// Handles the MouseEnter event of the _AppBar control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void AppBar_MouseEnter(object sender, MouseEventArgs e) => _mouseIsOverAppBar = true;

    /// <summary>
    /// Handles the MouseLeave event of the _AppBar control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void AppBar_MouseLeave(object sender, MouseEventArgs e) => _mouseIsOverAppBar = false;

    private IEnumerable<Button> GetButtons(string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null)
    {
        var owner = this;
        owner.CustomButton0 = CreateDialogButton(custom0, true, false, _custom0Command);
        yield return owner.CustomButton0;
        if (custom1 != null)
        {
            owner.CustomButton1 = CreateDialogButton(custom1, false, false, _custom1Command);
            yield return owner.CustomButton1;
            if (custom2 != null)
            {
                owner.CustomButton2 = CreateDialogButton(custom2, false, false, _custom2Command);
                yield return owner.CustomButton2;
                if (custom3 != null)
                {
                    owner.CustomButton3 = CreateDialogButton(custom3, false, false, _custom3Command);
                    yield return owner.CustomButton3;
                    if (custom4 != null)
                    {
                        owner.CustomButton4 = CreateDialogButton(custom4, false, false, _custom4Command);
                        yield return owner.CustomButton4;
                        if (custom5 != null)
                        {
                            owner.CustomButton5 = CreateDialogButton(custom5, false, false, _custom5Command);
                            yield return owner.CustomButton5;
                            if (custom6 != null)
                            {
                                owner.CustomButton6 = CreateDialogButton(custom6, false, false, _custom6Command);
                                yield return owner.CustomButton6;
                                if (custom7 != null)
                                {
                                    owner.CustomButton7 = CreateDialogButton(custom7, false, false, _custom7Command);
                                    yield return owner.CustomButton7;
                                    if (custom8 != null)
                                    {
                                        owner.CustomButton8 = CreateDialogButton(custom8, false, false, _custom8Command);
                                        yield return owner.CustomButton8;
                                        if (custom9 != null)
                                        {
                                            owner.CustomButton9 = CreateDialogButton(custom9, false, false, _custom9Command);
                                            yield return owner.CustomButton9;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the buttons.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>A IEnumerable of Buttons.</returns>
    private IEnumerable<Button> GetButtons(MessageBoxButton button)
    {
        var owner = this;
        switch (button)
        {
            case MessageBoxButton.OK:
                yield return owner.OkButton;
                break;

            case MessageBoxButton.OKCancel:
                yield return owner.OkButton;
                yield return owner.CancelButton;
                break;

            case MessageBoxButton.YesNo:
                yield return owner.YesButton;
                yield return owner.NoButton;
                break;

            case MessageBoxButton.YesNoCancel:
                yield return owner.YesButton;
                yield return owner.NoButton;
                yield return owner.CancelButton;
                break;
        }
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the ModernWindow control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    private void ModernWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Z && (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin)))
        {
            if (!_appBarVisible)
            {
                ShowAppBar();
            }
            else if (_appBarVisible && !AppBarIsSticky)
            {
                HideAppBar();
            }
        }
    }

    /// <summary>
    /// Handles the PreviewMouseDown event of the ModernWindow control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    /// The <see cref="MouseButtonEventArgs"/> instance containing the event data.
    /// </param>
    private void ModernWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!_mouseIsOverAppBar)
        {
            if (!_appBarVisible && e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Right)
            {
                ShowAppBar();
            }
            else if (_appBarVisible && e.ChangedButton != MouseButton.Right && !AppBarIsSticky)
            {
                HideAppBar();
            }
        }

        if (!_mouseIsOverMessage
            && MessageVisible == Visibility.Visible && e.ButtonState == MouseButtonState.Pressed)
        {
            MessageVisible = Visibility.Collapsed;
            _messageBoxResult = MessageBoxResult.Cancel;
            _customMessageBoxResult = CustomMessageBoxResult.Cancel;
        }
    }

    /// <summary>
    /// Called when [can minimize window].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    /// The <see cref="CanExecuteRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ResizeMode != ResizeMode.NoResize;

    /// <summary>
    /// Called when [can resize window].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    /// The <see cref="CanExecuteRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;

    /// <summary>
    /// Called when [close window].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="e">
    /// The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnCloseWindow(object target, ExecutedRoutedEventArgs e) => SystemCommands.CloseWindow(this);

    /// <summary>
    /// Called when [maximize window].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="e">
    /// The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e) => SystemCommands.MaximizeWindow(this);

    /// <summary>
    /// Called when [minimize window].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="e">
    /// The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e) => SystemCommands.MinimizeWindow(this);

    /// <summary>
    /// Called when [restore window].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="e">
    /// The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e) => SystemCommands.RestoreWindow(this);
}
