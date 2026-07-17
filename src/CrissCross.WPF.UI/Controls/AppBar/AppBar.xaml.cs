// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Interaction logic for AppBar.xaml.</summary>
public partial class AppBar : IHaveAppBar
{
    /// <summary>The application bar enabled property.</summary>
    public static readonly DependencyProperty AppBarEnabledProperty = DependencyProperty.Register(
        nameof(AppBarEnabled),
        typeof(bool),
        typeof(AppBar),
        new PropertyMetadata(true));

    /// <summary>Holds AppBar open until explicitly closed.</summary>
    public static readonly DependencyProperty AppBarIsStickyProperty = DependencyProperty.Register(
        nameof(AppBarIsSticky),
        typeof(bool),
        typeof(AppBar),
        new PropertyMetadata(false));

    /// <summary>Recommended Height 88.</summary>
    public static readonly DependencyProperty AppBarLeftProperty = DependencyProperty.Register(
        nameof(AppBarLeft),
        typeof(ObservableCollection<FrameworkElement>),
        typeof(AppBar));

    /// <summary>Recommended Height 88.</summary>
    public static readonly DependencyProperty AppBarRightProperty = DependencyProperty.Register(
        nameof(AppBarRight),
        typeof(ObservableCollection<FrameworkElement>),
        typeof(AppBar));

    /// <summary>Stores the _hide value.</summary>
    private readonly Storyboard? _hide;

    /// <summary>Stores the _show value.</summary>
    private readonly Storyboard? _show;

    /// <summary>Stores the _disposables value.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>Stores the _appBarVisible value.</summary>
    private bool _appBarVisible;

    /// <summary>Stores the _mouseIsOverAppBar value.</summary>
    private bool _mouseIsOverAppBar;

    /// <summary>Initializes a new instance of the <see cref="AppBar"/> class.</summary>
    public AppBar()
    {
        InitializeComponent();
        AppBarLeft = [];
        AppBarRight = [];
        AppBarLeftControl.ItemsSource = AppBarLeft;
        AppBarRightControl.ItemsSource = AppBarRight;
        _hide = BottomAppBar.Resources["Hide"] as Storyboard;
        _show = BottomAppBar.Resources["Show"] as Storyboard;
        ObservePointerState();
        HideAppBar();
        Loaded += OnLoaded;
        Unloaded += (_, _) => _disposables.Dispose();
    }

    /// <summary>Gets or sets a value indicating whether [application bar enabled].</summary>
    /// <value><c>true</c> if [application bar enabled]; otherwise, <c>false</c> .</value>
    [Description("Gets or Sets the Visibility of the Nav Bar")]
    [Category("CrissCross")]
    public bool AppBarEnabled
    {
        get => (bool)GetValue(AppBarEnabledProperty);
        set
        {
            SetValue(AppBarEnabledProperty, value);
            if (value)
            {
                return;
            }

            HideAppBar();
        }
    }

    /// <summary>Gets a value indicating whether [application bar is sticky].</summary>
    /// <value><c>true</c> if [application bar is sticky]; otherwise, <c>false</c> .</value>
    [Description("Gets the AppBar Sticky state.")]
    [Category("CrissCross")]
    public bool AppBarIsSticky
    {
        get => (bool)GetValue(AppBarIsStickyProperty);
        private set => SetValue(AppBarIsStickyProperty, value);
    }

    /// <summary>Gets or sets the AppBarLeft content.</summary>
    [Description("Gets or sets the AppBarLeft content.")]
    [Category("CrissCross")]
    public ObservableCollection<FrameworkElement> AppBarLeft
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(AppBarLeftProperty);
        set => SetValue(AppBarLeftProperty, value);
    }

    /// <summary>Gets or sets the AppBarRight content.</summary>
    [Description("Gets or sets the AppBarRight content.")]
    [Category("CrissCross")]
    public ObservableCollection<FrameworkElement> AppBarRight
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(AppBarRightProperty);
        set => SetValue(AppBarRightProperty, value);
    }

    /// <summary>Shows the application bar.</summary>
    /// <param name="isSticky">if set to <c>true</c> [is sticky].</param>
    private void ShowAppBar(bool isSticky = false)
    {
        if (!AppBarEnabled)
        {
            HideAppBar();
            return;
        }

        AppBarIsSticky = isSticky;
        if (_show is null || _appBarVisible)
        {
            return;
        }

        _show.Begin();
        _appBarVisible = true;
    }

    /// <summary>Hides the application bar.</summary>
    private void HideAppBar()
    {
        if (_hide is null || !_appBarVisible)
        {
            return;
        }

        _hide.Begin();
        _appBarVisible = false;
    }
}
