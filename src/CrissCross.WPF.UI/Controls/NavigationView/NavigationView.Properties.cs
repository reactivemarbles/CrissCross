// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using CrissCross.WPF.UI.Animations;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigationView.
/// </summary>
/// <seealso cref="Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    private static readonly DependencyPropertyKey MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly(
       nameof(MenuItems),
       typeof(ObservableCollection<object>),
       typeof(NavigationView),
       new PropertyMetadata(null));

    private static readonly DependencyPropertyKey FooterMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly(
       nameof(FooterMenuItems),
       typeof(ObservableCollection<object>),
       typeof(NavigationView),
       new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty EnableDebugMessagesProperty = DependencyProperty.Register(
#pragma warning restore SA1202 // Elements should be ordered by access
        nameof(EnableDebugMessages),
        typeof(bool),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="HeaderVisibility"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register(
        nameof(HeaderVisibility),
        typeof(Visibility),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Property for <see cref="AlwaysShowHeader"/>.
    /// </summary>
    public static readonly DependencyProperty AlwaysShowHeaderProperty = DependencyProperty.Register(
        nameof(AlwaysShowHeader),
        typeof(bool),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(false));

    /// <summary>Identifies the <see cref="MenuItems"/> dependency property.</summary>
    public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

    /// <summary>Identifies the <see cref="FooterMenuItems"/> dependency property.</summary>
    public static readonly DependencyProperty FooterMenuItemsProperty = FooterMenuItemsPropertyKey.DependencyProperty;

    /// <summary>
    /// Property for <see cref="MenuItemsSource"/>.
    /// </summary>
    public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register(
        nameof(MenuItemsSource),
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null, OnMenuItemsSourceChanged));

    /// <summary>
    /// Property for <see cref="FooterMenuItemsSource"/>.
    /// </summary>
    public static readonly DependencyProperty FooterMenuItemsSourceProperty = DependencyProperty.Register(
        nameof(FooterMenuItemsSource),
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null, OnFooterMenuItemsSourceChanged));

    /// <summary>
    /// Property for <see cref="ContentOverlay"/>.
    /// </summary>
    public static readonly DependencyProperty ContentOverlayProperty = DependencyProperty.Register(
        nameof(ContentOverlay),
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="IsBackEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsBackEnabledProperty = DependencyProperty.Register(
        nameof(IsBackEnabled),
        typeof(bool),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="IsBackButtonVisible"/>.
    /// </summary>
    public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register(
        nameof(IsBackButtonVisible),
        typeof(NavigationViewBackButtonVisible),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(NavigationViewBackButtonVisible.Auto));

    /// <summary>
    /// Property for <see cref="IsPaneToggleVisible"/>.
    /// </summary>
    public static readonly DependencyProperty IsPaneToggleVisibleProperty = DependencyProperty.Register(
        nameof(IsPaneToggleVisible),
        typeof(bool),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="IsPaneOpen"/>.
    /// </summary>
    public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(
        nameof(IsPaneOpen),
        typeof(bool),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(true, IsPaneOpenChangedCallback));

    /// <summary>
    /// Property for <see cref="IsPaneVisible"/>.
    /// </summary>
    public static readonly DependencyProperty IsPaneVisibleProperty = DependencyProperty.Register(
        nameof(IsPaneVisible),
        typeof(bool),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="OpenPaneLength"/>.
    /// </summary>
    public static readonly DependencyProperty OpenPaneLengthProperty = DependencyProperty.Register(
        nameof(OpenPaneLength),
        typeof(double),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(0D));

    /// <summary>
    /// Property for <see cref="CompactPaneLength"/>.
    /// </summary>
    public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register(
        nameof(CompactPaneLength),
        typeof(double),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(0D));

    /// <summary>
    /// Property for <see cref="PaneHeader"/>.
    /// </summary>
    public static readonly DependencyProperty PaneHeaderProperty = DependencyProperty.Register(
        nameof(PaneHeader),
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="PaneTitle"/>.
    /// </summary>
    public static readonly DependencyProperty PaneTitleProperty = DependencyProperty.Register(
        nameof(PaneTitle),
        typeof(string),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="PaneFooter"/>.
    /// </summary>
    public static readonly DependencyProperty PaneFooterProperty = DependencyProperty.Register(
        nameof(PaneFooter),
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="PaneDisplayMode"/>.
    /// </summary>
    public static readonly DependencyProperty PaneDisplayModeProperty = DependencyProperty.Register(
        nameof(PaneDisplayMode),
        typeof(NavigationViewPaneDisplayMode),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(NavigationViewPaneDisplayMode.Left, OnPaneDisplayModePropertyChanged));

    /// <summary>
    /// Property for <see cref="AutoSuggestBox"/>.
    /// </summary>
    public static readonly DependencyProperty AutoSuggestBoxProperty = DependencyProperty.Register(
        nameof(AutoSuggestBox),
        typeof(AutoSuggestBox),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null, OnAutoSuggestBoxPropertyChangedCallback));

    /// <summary>
    /// Property for <see cref="TitleBar"/>.
    /// </summary>
    public static readonly DependencyProperty TitleBarProperty = DependencyProperty.Register(
        nameof(TitleBar),
        typeof(TitleBar),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null, OnTitleBarPropertyChangedCallback));

    /// <summary>
    /// Property for <see cref="BreadcrumbBar"/>.
    /// </summary>
    public static readonly DependencyProperty BreadcrumbBarProperty = DependencyProperty.Register(
        nameof(BreadcrumbBar),
        typeof(BreadcrumbBar),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null, OnBreadcrumbBarPropertyChangedCallback));

    /// <summary>
    /// Property for <see cref="ItemTemplate"/>.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(ControlTemplate),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            OnItemTemplatePropertyChanged));

    /// <summary>
    /// Property for <see cref="TransitionDuration"/>.
    /// </summary>
    public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register(
        nameof(TransitionDuration),
        typeof(int),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(200));

    /// <summary>
    /// Property for <see cref="Transition"/>.
    /// </summary>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition),
        typeof(Transition),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(Transition.FadeInWithSlide));

    /// <summary>
    /// Property for <see cref="FrameMargin"/>.
    /// </summary>
    public static readonly DependencyProperty FrameMarginProperty = DependencyProperty.Register(
        nameof(FrameMargin),
        typeof(Thickness),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(default(Thickness)));

    private ObservableCollection<object>? _menuItems;
    private ObservableCollection<object>? _footerMenuItems;

    /// <summary>
    /// Gets or sets a value indicating whether enables or disables debugging messages for this control.
    /// </summary>
    public bool EnableDebugMessages
    {
        get => (bool)GetValue(EnableDebugMessagesProperty);
        set => SetValue(EnableDebugMessagesProperty, value);
    }

    /// <inheritdoc/>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <inheritdoc/>
    public Visibility HeaderVisibility
    {
        get => (Visibility)GetValue(HeaderVisibilityProperty);
        set => SetValue(HeaderVisibilityProperty, value);
    }

    /// <inheritdoc/>
    public bool AlwaysShowHeader
    {
        get => (bool)GetValue(AlwaysShowHeaderProperty);
        set => SetValue(AlwaysShowHeaderProperty, value);
    }

    /// <inheritdoc/>
    public IList MenuItems
    {
        get
        {
            if (_menuItems == null)
            {
                _menuItems = [];
                _menuItems.CollectionChanged += OnMenuItems_CollectionChanged;
            }

            return _menuItems;
        }
    }

    /// <inheritdoc/>
    [Bindable(true)]
    public object? MenuItemsSource
    {
        get => GetValue(MenuItemsSourceProperty);
        set
        {
            if (value is null)
            {
                ClearValue(MenuItemsSourceProperty);
            }
            else
            {
                SetValue(MenuItemsSourceProperty, value);
            }
        }
    }

    /// <inheritdoc/>
    public IList FooterMenuItems
    {
        get
        {
            if (_footerMenuItems == null)
            {
                _footerMenuItems = [];
                _footerMenuItems.CollectionChanged += OnFooterMenuItems_CollectionChanged;
            }

            return _footerMenuItems;
        }
    }

    /// <inheritdoc/>
    [Bindable(true)]
    public object? FooterMenuItemsSource
    {
        get => GetValue(FooterMenuItemsSourceProperty);
        set
        {
            if (value is null)
            {
                ClearValue(FooterMenuItemsSourceProperty);
            }
            else
            {
                SetValue(FooterMenuItemsSourceProperty, value);
            }
        }
    }

    /// <inheritdoc/>
    public object? ContentOverlay
    {
        get => GetValue(ContentOverlayProperty);
        set => SetValue(ContentOverlayProperty, value);
    }

    /// <inheritdoc/>
    public bool IsBackEnabled
    {
        get => (bool)GetValue(IsBackEnabledProperty);
        protected set => SetValue(IsBackEnabledProperty, value);
    }

    /// <inheritdoc/>
    public NavigationViewBackButtonVisible IsBackButtonVisible
    {
        get => (NavigationViewBackButtonVisible)GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <inheritdoc/>
    public bool IsPaneToggleVisible
    {
        get => (bool)GetValue(IsPaneToggleVisibleProperty);
        set => SetValue(IsPaneToggleVisibleProperty, value);
    }

    /// <inheritdoc/>
    public bool IsPaneOpen
    {
        get => (bool)GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <inheritdoc/>
    public bool IsPaneVisible
    {
        get => (bool)GetValue(IsPaneVisibleProperty);
        set => SetValue(IsPaneVisibleProperty, value);
    }

    /// <inheritdoc/>
    public double OpenPaneLength
    {
        get => (double)GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    /// <inheritdoc/>
    public double CompactPaneLength
    {
        get => (double)GetValue(CompactPaneLengthProperty);
        set => SetValue(CompactPaneLengthProperty, value);
    }

    /// <inheritdoc/>
    public object? PaneHeader
    {
        get => GetValue(PaneHeaderProperty);
        set => SetValue(PaneHeaderProperty, value);
    }

    /// <inheritdoc/>
    public string? PaneTitle
    {
        get => (string?)GetValue(PaneTitleProperty);
        set => SetValue(PaneTitleProperty, value);
    }

    /// <inheritdoc/>
    public object? PaneFooter
    {
        get => GetValue(PaneFooterProperty);
        set => SetValue(PaneFooterProperty, value);
    }

    /// <inheritdoc/>
    public NavigationViewPaneDisplayMode PaneDisplayMode
    {
        get => (NavigationViewPaneDisplayMode)GetValue(PaneDisplayModeProperty);
        set => SetValue(PaneDisplayModeProperty, value);
    }

    /// <inheritdoc/>
    public AutoSuggestBox? AutoSuggestBox
    {
        get => (AutoSuggestBox?)GetValue(AutoSuggestBoxProperty);
        set => SetValue(AutoSuggestBoxProperty, value);
    }

    /// <inheritdoc/>
    public TitleBar? TitleBar
    {
        get => (TitleBar?)GetValue(TitleBarProperty);
        set => SetValue(TitleBarProperty, value);
    }

    /// <inheritdoc/>
    public BreadcrumbBar? BreadcrumbBar
    {
        get => (BreadcrumbBar?)GetValue(BreadcrumbBarProperty);
        set => SetValue(BreadcrumbBarProperty, value);
    }

    /// <inheritdoc/>
    public ControlTemplate? ItemTemplate
    {
        get => (ControlTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <inheritdoc/>
    [Bindable(true)]
    [Category("Appearance")]
    public int TransitionDuration
    {
        get => (int)GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <inheritdoc/>
    public Transition Transition
    {
        get => (Transition)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <inheritdoc/>
    public Thickness FrameMargin
    {
        get => (Thickness)GetValue(FrameMarginProperty);
        set => SetValue(FrameMarginProperty, value);
    }

    private static void OnMenuItemsSourceChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        navigationView.MenuItems.Clear();

        if (e.NewValue is IEnumerable newItemsSource and not string)
        {
            foreach (var item in newItemsSource)
            {
                navigationView.MenuItems.Add(item);
            }
        }
        else if (e.NewValue != null)
        {
            navigationView.MenuItems.Add(e.NewValue);
        }
    }

    private static void OnFooterMenuItemsSourceChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        navigationView.FooterMenuItems.Clear();

        if (e.NewValue is IEnumerable newItemsSource and not string)
        {
            foreach (var item in newItemsSource)
            {
                navigationView.FooterMenuItems.Add(item);
            }
        }
        else if (e.NewValue != null)
        {
            navigationView.FooterMenuItems.Add(e.NewValue);
        }
    }

    private static void OnPaneDisplayModePropertyChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        navigationView.OnPaneDisplayModeChanged();
    }

    private static void OnItemTemplatePropertyChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        navigationView.OnItemTemplateChanged();
    }

    private static void IsPaneOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        if ((bool)e.NewValue == (bool)e.OldValue)
        {
            return;
        }

        if (navigationView.IsPaneOpen)
        {
            navigationView.OnPaneOpened();
        }
        else
        {
            navigationView.OnPaneClosed();
        }

        navigationView.CloseNavigationViewItemMenus();

        navigationView.TitleBar?.SetCurrentValue(
                MarginProperty,
                navigationView.IsPaneOpen ? titleBarPaneOpenMargin : titleBarPaneCompactMargin);

        UpdateVisualState(navigationView);
    }

    private static void OnTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        if (e.NewValue is null && e.OldValue is TitleBar oldValue)
        {
            navigationView.FrameMargin = new Thickness(0);
            oldValue.Margin = new Thickness(0);

            if (navigationView.AutoSuggestBox?.Margin == autoSuggestBoxMargin)
            {
                navigationView.AutoSuggestBox.SetCurrentValue(MarginProperty, new Thickness(0));
            }

            return;
        }

        if (e.NewValue is not TitleBar titleBar)
        {
            return;
        }

        navigationView.FrameMargin = frameMargin;
        titleBar.Margin = titleBarPaneOpenMargin;

        if (navigationView.AutoSuggestBox?.Margin is { Bottom: 0, Left: 0, Right: 0, Top: 0 })
        {
            navigationView.AutoSuggestBox.SetCurrentValue(MarginProperty, autoSuggestBoxMargin);
        }
    }

    private static void OnAutoSuggestBoxPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        if (e.NewValue is null && e.OldValue is AutoSuggestBox oldValue)
        {
            oldValue.SuggestionChosen -= navigationView.AutoSuggestBoxOnSuggestionChosen;
            oldValue.QuerySubmitted -= navigationView.AutoSuggestBoxOnQuerySubmitted;
            return;
        }

        if (e.NewValue is not AutoSuggestBox autoSuggestBox)
        {
            return;
        }

        autoSuggestBox.OriginalItemsSource = navigationView._autoSuggestBoxItems;
        autoSuggestBox.SuggestionChosen += navigationView.AutoSuggestBoxOnSuggestionChosen;
        autoSuggestBox.QuerySubmitted += navigationView.AutoSuggestBoxOnQuerySubmitted;

        if (navigationView.TitleBar?.Margin == titleBarPaneOpenMargin && autoSuggestBox.Margin is { Bottom: 0, Left: 0, Right: 0, Top: 0 })
        {
            autoSuggestBox.Margin = autoSuggestBoxMargin;
        }
    }

    private static void OnBreadcrumbBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationView navigationView)
        {
            return;
        }

        if (e.NewValue is null && e.OldValue is BreadcrumbBar oldValue)
        {
            oldValue.ItemClicked -= navigationView.BreadcrumbBarOnItemClicked;
            {
                return;
            }
        }

        if (e.NewValue is not BreadcrumbBar breadcrumbBar)
        {
            return;
        }

        breadcrumbBar.ItemsSource = navigationView._breadcrumbBarItems;
        breadcrumbBar.ItemTemplate ??=
            UiApplication.Current.TryFindResource("NavigationViewItemDataTemplate") as DataTemplate;
        breadcrumbBar.ItemClicked += navigationView.BreadcrumbBarOnItemClicked;
    }

    private void OnMenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null)
        {
            return;
        }

        UpdateMenuItemsTemplate(e.NewItems);
        AddItemsToDictionaries(e.NewItems);
    }

    private void OnFooterMenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null)
        {
            return;
        }

        UpdateMenuItemsTemplate(e.NewItems);
        AddItemsToDictionaries(e.NewItems);
    }
}
