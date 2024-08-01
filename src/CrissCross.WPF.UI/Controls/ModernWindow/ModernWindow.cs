// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CP.WPF.Controls;
using ReactiveMarbles.Extensions.Hosting.Wpf;
using ReactiveUI;

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

    private readonly ReplaySubject<string> _busyStatusTextSubject = new(1);
    private readonly ReplaySubject<Visibility> _busyVisibilitySubject = new(1);
    private readonly CompositeDisposable _cleanUp = [];
    private Grid? _appBar;
    private bool _appBarVisible;
    private Storyboard? _backgroundAnimation;
    private Storyboard? _hide;
    private bool _mouseIsOverAppBar;
    private Storyboard? _show;
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

        // Set up magic functions
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
                _cleanUp.Dispose();
            }

            _disposedValue = true;
        }
    }

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
