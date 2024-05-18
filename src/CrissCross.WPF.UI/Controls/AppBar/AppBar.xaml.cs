// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ReactiveMarbles.ObservableEvents;
using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>
/// Interaction logic for AppBar.xaml.
/// </summary>
public partial class AppBar : IHaveAppBar
{
    /// <summary>
    /// The application bar enabled property.
    /// </summary>
    public static readonly DependencyProperty AppBarEnabledProperty = DependencyProperty.Register(nameof(AppBarEnabled), typeof(bool), typeof(AppBar), new PropertyMetadata(true));

    /// <summary>
    /// Holds AppBar open until explicitly closed.
    /// </summary>
    public static readonly DependencyProperty AppBarIsStickyProperty = DependencyProperty.Register(nameof(AppBarIsSticky), typeof(bool), typeof(AppBar), new PropertyMetadata(false));

    /// <summary>
    /// Recommended Height 88.
    /// </summary>
    public static readonly DependencyProperty AppBarLeftProperty = DependencyProperty.Register(nameof(AppBarLeft), typeof(ObservableCollection<FrameworkElement>), typeof(AppBar));

    /// <summary>
    /// Recommended Height 88.
    /// </summary>
    public static readonly DependencyProperty AppBarRightProperty = DependencyProperty.Register(nameof(AppBarRight), typeof(ObservableCollection<FrameworkElement>), typeof(AppBar));

    private readonly Storyboard? _hide;
    private readonly Storyboard? _show;
    private readonly CompositeDisposable _disposables = [];
    private bool _appBarVisible;
    private bool _mouseIsOverAppBar;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBar"/> class.
    /// </summary>
    public AppBar()
    {
        InitializeComponent();
        AppBarLeft = [];
        AppBarRight = [];
        AppBarLeftControl.ItemsSource = AppBarLeft;
        AppBarRightControl.ItemsSource = AppBarRight;
        _hide = BottomAppBar.Resources["Hide"] as Storyboard;
        _show = BottomAppBar.Resources["Show"] as Storyboard;
        BottomAppBar.Events().MouseEnter.Select(_ => true)
            .Merge(BottomAppBar.Events().MouseLeave.Select(_ => false))
            .Subscribe(isOver => _mouseIsOverAppBar = isOver)
            .DisposeWith(_disposables);

        HideAppBar();
        this.Events().Loaded.Subscribe(_ =>
        {
            // Find the parent window
            var parentWindow = Window.GetWindow(this);
            parentWindow?.Events().PreviewMouseDown.Subscribe(e =>
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
            })
            .DisposeWith(_disposables);

            this.AppBarIsStickyListener(() => AppBarIsSticky, isSticky => AppBarIsSticky = isSticky);
            this.AppBarLeftListener(() => AppBarLeft);
            this.AppBarRightListener(() => AppBarRight);
            this.ShowAppBarListener(ShowAppBar);
            this.HideAppBarListener(HideAppBar);
        }).DisposeWith(_disposables);
        this.Events().Unloaded.Subscribe(_ => _disposables.Dispose()).DisposeWith(_disposables);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [application bar enabled].
    /// </summary>
    /// <value><c>true</c> if [application bar enabled]; otherwise, <c>false</c> .</value>
    [Description("Gets or Sets the Visibility of the Nav Bar")]
    [Category("CrissCross")]
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
    [Category("CrissCross")]
    public bool AppBarIsSticky
    {
        get => (bool)GetValue(AppBarIsStickyProperty);

        private set => SetValue(AppBarIsStickyProperty, value);
    }

    /// <summary>
    /// Gets or sets the AppBarLeft content.
    /// </summary>
    [Description("Gets or sets the AppBarLeft content.")]
    [Category("CrissCross")]
    public ObservableCollection<FrameworkElement> AppBarLeft
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(AppBarLeftProperty);
        set => SetValue(AppBarLeftProperty, value);
    }

    /// <summary>
    /// Gets or sets the AppBarRight content.
    /// </summary>
    [Description("Gets or sets the AppBarRight content.")]
    [Category("CrissCross")]
    public ObservableCollection<FrameworkElement> AppBarRight
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(AppBarRightProperty);
        set => SetValue(AppBarRightProperty, value);
    }

    /// <summary>
    /// Gets the template child.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="name">The name.</param>
    /// <returns>The Instance if exists.</returns>
    /// <exception cref="ArgumentNullException">name.</exception>
    protected T GetTemplateChild<T>(string name)
        where T : DependencyObject
    {
        if (GetTemplateChild(name) is not T dependencyObject)
        {
            throw new ArgumentNullException(name);
        }

        return dependencyObject;
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
}
