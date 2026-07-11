// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using CrissCross;
using CrissCross.Avalonia.UI.Animations;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>NavigationView provides a collapsible navigation menu for top-level navigation.</summary>
public class NavigationView : TemplatedControl, INavigationView
{
    /// <summary>Property for <see cref="MenuItems"/>.</summary>
    public static readonly DirectProperty<NavigationView, IList> MenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, IList>(
            nameof(MenuItems),
            o => o.MenuItems);

    /// <summary>Property for <see cref="MenuItemsSource"/>.</summary>
    public static readonly StyledProperty<object?> MenuItemsSourceProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(MenuItemsSource));

    /// <summary>Property for <see cref="FooterMenuItems"/>.</summary>
    public static readonly DirectProperty<NavigationView, IList> FooterMenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, IList>(
            nameof(FooterMenuItems),
            o => o.FooterMenuItems);

    /// <summary>Property for <see cref="FooterMenuItemsSource"/>.</summary>
    public static readonly StyledProperty<object?> FooterMenuItemsSourceProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(FooterMenuItemsSource));

    /// <summary>Property for <see cref="Header"/>.</summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Header));

    /// <summary>Property for <see cref="HeaderVisibility"/>.</summary>
    public static readonly StyledProperty<bool> HeaderVisibilityProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(HeaderVisibility), true);

    /// <summary>Property for <see cref="AlwaysShowHeader"/>.</summary>
    public static readonly StyledProperty<bool> AlwaysShowHeaderProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(AlwaysShowHeader), false);

    /// <summary>Property for <see cref="Content"/>.</summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Content));

    /// <summary>Property for <see cref="ContentOverlay"/>.</summary>
    public static readonly StyledProperty<object?> ContentOverlayProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(ContentOverlay));

    /// <summary>Property for <see cref="IsPaneOpen"/>.</summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneOpen), true);

    /// <summary>Property for <see cref="IsPaneVisible"/>.</summary>
    public static readonly StyledProperty<bool> IsPaneVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneVisible), true);

    /// <summary>Property for <see cref="IsPaneToggleVisible"/>.</summary>
    public static readonly StyledProperty<bool> IsPaneToggleVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneToggleVisible), true);

    /// <summary>Property for <see cref="IsBackEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsBackEnabledProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsBackEnabled), false);

    /// <summary>Property for <see cref="IsBackButtonVisible"/>.</summary>
    public static readonly StyledProperty<NavigationViewBackButtonVisible> IsBackButtonVisibleProperty =
        AvaloniaProperty.Register<NavigationView, NavigationViewBackButtonVisible>(
            nameof(IsBackButtonVisible),
            NavigationViewBackButtonVisible.Auto);

    /// <summary>Property for <see cref="OpenPaneLength"/>.</summary>
    public static readonly StyledProperty<double> OpenPaneLengthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(OpenPaneLength), 320.0);

    /// <summary>Property for <see cref="CompactPaneLength"/>.</summary>
    public static readonly StyledProperty<double> CompactPaneLengthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(CompactPaneLength), 48.0);

    /// <summary>Property for <see cref="PaneHeader"/>.</summary>
    public static readonly StyledProperty<object?> PaneHeaderProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(PaneHeader));

    /// <summary>Property for <see cref="PaneTitle"/>.</summary>
    public static readonly StyledProperty<string?> PaneTitleProperty =
        AvaloniaProperty.Register<NavigationView, string?>(nameof(PaneTitle));

    /// <summary>Property for <see cref="PaneFooter"/>.</summary>
    public static readonly StyledProperty<object?> PaneFooterProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(PaneFooter));

    /// <summary>Property for <see cref="PaneDisplayMode"/>.</summary>
    public static readonly StyledProperty<NavigationViewPaneDisplayMode> PaneDisplayModeProperty =
        AvaloniaProperty.Register<NavigationView, NavigationViewPaneDisplayMode>(
            nameof(PaneDisplayMode),
            NavigationViewPaneDisplayMode.Left);

    /// <summary>Property for <see cref="TitleBar"/>.</summary>
    public static readonly StyledProperty<TitleBar?> TitleBarProperty =
        AvaloniaProperty.Register<NavigationView, TitleBar?>(nameof(TitleBar));

    /// <summary>Property for <see cref="AutoSuggestBox"/>.</summary>
    public static readonly StyledProperty<AutoSuggestBox?> AutoSuggestBoxProperty =
        AvaloniaProperty.Register<NavigationView, AutoSuggestBox?>(nameof(AutoSuggestBox));

    /// <summary>Property for <see cref="BreadcrumbBar"/>.</summary>
    public static readonly StyledProperty<BreadcrumbBar?> BreadcrumbBarProperty =
        AvaloniaProperty.Register<NavigationView, BreadcrumbBar?>(nameof(BreadcrumbBar));

    /// <summary>Property for <see cref="ItemTemplate"/>.</summary>
    public static readonly StyledProperty<IControlTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<NavigationView, IControlTemplate?>(nameof(ItemTemplate));

    /// <summary>Property for <see cref="TransitionDuration"/>.</summary>
    public static readonly StyledProperty<int> TransitionDurationProperty =
        AvaloniaProperty.Register<NavigationView, int>(nameof(TransitionDuration), 200);

    /// <summary>Property for <see cref="Transition"/>.</summary>
    public static readonly StyledProperty<Transition> TransitionProperty =
        AvaloniaProperty.Register<NavigationView, Transition>(nameof(Transition), Transition.FadeInWithSlide);

    /// <summary>Property for <see cref="FrameMargin"/>.</summary>
    public static readonly StyledProperty<Thickness> FrameMarginProperty =
        AvaloniaProperty.Register<NavigationView, Thickness>(nameof(FrameMargin), default);

    /// <summary>Routed event for <see cref="PaneOpened"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> PaneOpenedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(nameof(PaneOpened), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="PaneClosed"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> PaneClosedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(nameof(PaneClosed), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="SelectionChanged"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="ItemInvoked"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> ItemInvokedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(nameof(ItemInvoked), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="BackRequested"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> BackRequestedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(nameof(BackRequested), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="Navigating"/>.</summary>
    public static readonly RoutedEvent<NavigatingCancelEventArgs> NavigatingEvent =
        RoutedEvent.Register<NavigationView, NavigatingCancelEventArgs>(nameof(Navigating), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="Navigated"/>.</summary>
    public static readonly RoutedEvent<NavigatedEventArgs> NavigatedEvent =
        RoutedEvent.Register<NavigationView, NavigatedEventArgs>(nameof(Navigated), RoutingStrategies.Bubble);

    /// <summary>The template element navigation view content presenter.</summary>
    protected const string TemplateElementNavigationViewContentPresenter = "PART_NavigationViewContentPresenter";

    /// <summary>The template element menu items control.</summary>
    protected const string TemplateElementMenuItemsItemsControl = "PART_MenuItemsItemsControl";

    /// <summary>The template element footer menu items control.</summary>
    protected const string TemplateElementFooterMenuItemsItemsControl = "PART_FooterMenuItemsItemsControl";

    /// <summary>The template element back button.</summary>
    protected const string TemplateElementBackButton = "PART_BackButton";

    /// <summary>The template element toggle button.</summary>
    protected const string TemplateElementToggleButton = "PART_ToggleButton";

    /// <summary>The template element auto suggest box symbol button.</summary>
    protected const string TemplateElementAutoSuggestBoxSymbolButton = "PART_AutoSuggestBoxSymbolButton";

    /// <summary>The journal.</summary>
    private readonly List<string> _journal = new(50);

    /// <summary>The navigation stack.</summary>
    private readonly ObservableCollection<INavigationViewItem> _navigationStack = [];

    /// <summary>The page identifier or target tag navigation views dictionary.</summary>
    private readonly Dictionary<string, INavigationViewItem> _pageIdOrTargetTagNavigationViewsDictionary = [];

    /// <summary>The page type navigation views dictionary.</summary>
    private readonly Dictionary<Type, INavigationViewItem> _pageTypeNavigationViewsDictionary = [];

    /// <summary>Provides the _menuItems member.</summary>
    private readonly ObservableCollection<object> _menuItems = [];

    /// <summary>Provides the _footerMenuItems member.</summary>
    private readonly ObservableCollection<object> _footerMenuItems = [];

    /// <summary>Provides the _cache member.</summary>
    private readonly NavigationCache _cache = new();

    /// <summary>Provides the _serviceProvider member.</summary>
    private IServiceProvider? _serviceProvider;

    /// <summary>Provides the _pageService member.</summary>
    private IPageService? _pageService;

    /// <summary>Provides the _currentIndexInJournal member.</summary>
    private int _currentIndexInJournal = -1;

    /// <summary>Provides the _navigationViewContentPresenter member.</summary>
    private ContentPresenter? _navigationViewContentPresenter;

    /// <summary>Provides the _backButton member.</summary>
    private Button? _backButton;

    /// <summary>Provides the _toggleButton member.</summary>
    private ToggleButton? _toggleButton;

    /// <summary>Provides the NavigationView member.</summary>
    static NavigationView()
    {
        _ = IsPaneOpenProperty.Changed.AddClassHandler<NavigationView>((x, e) => x.OnIsPaneOpenChanged(e));
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationView"/> class.</summary>
    public NavigationView()
    {
        _menuItems.CollectionChanged += OnMenuItemsCollectionChanged;
        _footerMenuItems.CollectionChanged += OnMenuItemsCollectionChanged;
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs>? PaneOpened
    {
        add => AddHandler(PaneOpenedEvent, value);
        remove => RemoveHandler(PaneOpenedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs>? PaneClosed
    {
        add => AddHandler(PaneClosedEvent, value);
        remove => RemoveHandler(PaneClosedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs>? ItemInvoked
    {
        add => AddHandler(ItemInvokedEvent, value);
        remove => RemoveHandler(ItemInvokedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs>? BackRequested
    {
        add => AddHandler(BackRequestedEvent, value);
        remove => RemoveHandler(BackRequestedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<NavigatingCancelEventArgs>? Navigating
    {
        add => AddHandler(NavigatingEvent, value);
        remove => RemoveHandler(NavigatingEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<NavigatedEventArgs>? Navigated
    {
        add => AddHandler(NavigatedEvent, value);
        remove => RemoveHandler(NavigatedEvent, value);
    }

    /// <inheritdoc/>
    public IList MenuItems => _menuItems;

    /// <inheritdoc/>
    public object? MenuItemsSource
    {
        get => GetValue(MenuItemsSourceProperty);
        set => SetValue(MenuItemsSourceProperty, value);
    }

    /// <inheritdoc/>
    public IList FooterMenuItems => _footerMenuItems;

    /// <inheritdoc/>
    public object? FooterMenuItemsSource
    {
        get => GetValue(FooterMenuItemsSourceProperty);
        set => SetValue(FooterMenuItemsSourceProperty, value);
    }

    /// <inheritdoc/>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <inheritdoc/>
    public bool HeaderVisibility
    {
        get => GetValue(HeaderVisibilityProperty);
        set => SetValue(HeaderVisibilityProperty, value);
    }

    /// <inheritdoc/>
    public bool AlwaysShowHeader
    {
        get => GetValue(AlwaysShowHeaderProperty);
        set => SetValue(AlwaysShowHeaderProperty, value);
    }

    /// <summary>Gets or sets the main content.</summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <inheritdoc/>
    public object? ContentOverlay
    {
        get => GetValue(ContentOverlayProperty);
        set => SetValue(ContentOverlayProperty, value);
    }

    /// <inheritdoc/>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <inheritdoc/>
    public bool IsPaneVisible
    {
        get => GetValue(IsPaneVisibleProperty);
        set => SetValue(IsPaneVisibleProperty, value);
    }

    /// <inheritdoc/>
    public bool IsPaneToggleVisible
    {
        get => GetValue(IsPaneToggleVisibleProperty);
        set => SetValue(IsPaneToggleVisibleProperty, value);
    }

    /// <inheritdoc/>
    public bool IsBackEnabled
    {
        get => GetValue(IsBackEnabledProperty);
        set => SetValue(IsBackEnabledProperty, value);
    }

    /// <inheritdoc/>
    public NavigationViewBackButtonVisible IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <inheritdoc/>
    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    /// <inheritdoc/>
    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
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
        get => GetValue(PaneTitleProperty);
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
        get => GetValue(PaneDisplayModeProperty);
        set => SetValue(PaneDisplayModeProperty, value);
    }

    /// <inheritdoc/>
    public TitleBar? TitleBar
    {
        get => GetValue(TitleBarProperty);
        set => SetValue(TitleBarProperty, value);
    }

    /// <inheritdoc/>
    public AutoSuggestBox? AutoSuggestBox
    {
        get => GetValue(AutoSuggestBoxProperty);
        set => SetValue(AutoSuggestBoxProperty, value);
    }

    /// <inheritdoc/>
    public BreadcrumbBar? BreadcrumbBar
    {
        get => GetValue(BreadcrumbBarProperty);
        set => SetValue(BreadcrumbBarProperty, value);
    }

    /// <inheritdoc/>
    public IControlTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <inheritdoc/>
    public int TransitionDuration
    {
        get => GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <inheritdoc/>
    public Transition Transition
    {
        get => GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <inheritdoc/>
    public Thickness FrameMargin
    {
        get => GetValue(FrameMarginProperty);
        set => SetValue(FrameMarginProperty, value);
    }

    /// <inheritdoc/>
    public INavigationViewItem? SelectedItem { get; protected set; }

    /// <inheritdoc/>
    public bool CanGoBack => NavigationJournal.CanGoBack(_journal, _currentIndexInJournal);

    /// <inheritdoc/>
    public bool CanGoForward => NavigationJournal.CanGoForward(_journal, _currentIndexInJournal);

    /// <inheritdoc/>
    public void SetPageService(IPageService pageService) => _pageService = pageService;

    /// <inheritdoc/>
    public void SetServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public virtual bool Navigate(Type pageType, object? dataContext = null)
    {
        return !_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem) ? TryToNavigateWithoutINavigationViewItem(pageType, dataContext) : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc/>
    public virtual bool Navigate(string pageIdOrTargetTag, object? dataContext = null)
    {
        return !_pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(pageIdOrTargetTag, out var navigationViewItem) ? false : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc/>
    public virtual bool NavigateWithHierarchy(Type pageType, object? dataContext = null)
    {
        return !_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem) ? TryToNavigateWithoutINavigationViewItem(pageType, dataContext) : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc/>
    public virtual bool ReplaceContent(Type? pageTypeToEmbed)
    {
        if (pageTypeToEmbed is null)
        {
            return false;
        }

        if (_serviceProvider is not null)
        {
            UpdateContent(_serviceProvider.GetService(pageTypeToEmbed));
            return true;
        }

        if (_pageService is null)
        {
            return false;
        }

        UpdateContent(_pageService.GetPage(pageTypeToEmbed));
        return true;
    }

    /// <inheritdoc/>
    public virtual bool ReplaceContent(Control pageInstanceToEmbed, object? dataContext = null)
    {
        UpdateContent(pageInstanceToEmbed, dataContext);
        return true;
    }

    /// <inheritdoc/>
    public virtual bool GoForward()
    {
        return !NavigationJournal.TryMoveForward(_journal, _currentIndexInJournal, out var nextIndex, out var itemId) || itemId is null ? false : _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(itemId, out var navigationViewItem) &&
            NavigateInternal(navigationViewItem, isJournalNavigation: true, journalIndex: nextIndex);
    }

    /// <inheritdoc/>
    public virtual bool GoBack()
    {
        if (!NavigationJournal.TryMoveBack(_journal, _currentIndexInJournal, out var nextIndex, out var itemId) || itemId is null)
        {
            return false;
        }

        RaiseEvent(new RoutedEventArgs(BackRequestedEvent));
        return _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(itemId, out var navigationViewItem) &&
            NavigateInternal(navigationViewItem, isJournalNavigation: true, journalIndex: nextIndex);
    }

    /// <inheritdoc/>
    public virtual void ClearJournal()
    {
        NavigationJournal.Clear(_journal, ref _currentIndexInJournal);
        _cache.Clear();
    }

    /// <summary>Called when a navigation view item is clicked.</summary>
    /// <param name="navigationViewItem">The navigation view item.</param>
    internal void OnNavigationViewItemClick(NavigationViewItem navigationViewItem)
    {
        RaiseEvent(new RoutedEventArgs(ItemInvokedEvent));
        _ = NavigateInternal(navigationViewItem);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ArgumentNullException.ThrowIfNull(e);

        _navigationViewContentPresenter = e.NameScope.Find<ContentPresenter>(TemplateElementNavigationViewContentPresenter);
        _backButton = e.NameScope.Find<Button>(TemplateElementBackButton);
        _toggleButton = e.NameScope.Find<ToggleButton>(TemplateElementToggleButton);

        if (_backButton is not null)
        {
            _backButton.Click += OnBackButtonClick;
        }

        if (_toggleButton is not null)
        {
            _toggleButton.Click += OnToggleButtonClick;
        }

        AddItemsToDictionaries();
    }

    /// <summary>Called when back button is clicked.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnBackButtonClick(object? sender, RoutedEventArgs e) => GoBack();

    /// <summary>Called when toggle button is clicked.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnToggleButtonClick(object? sender, RoutedEventArgs e) => IsPaneOpen = !IsPaneOpen;

    /// <summary>Called when IsPaneOpen changes.</summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnIsPaneOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        var isOpen = (bool)e.NewValue!;
        RaiseEvent(new RoutedEventArgs(isOpen ? PaneOpenedEvent : PaneClosedEvent));
    }

    /// <summary>Adds items to dictionaries.</summary>
    protected virtual void AddItemsToDictionaries()
    {
        AddItemsToDictionaries(MenuItems);
        AddItemsToDictionaries(FooterMenuItems);
    }

    /// <summary>Adds items to dictionaries.</summary>
    /// <param name="list">The list.</param>
    protected virtual void AddItemsToDictionaries(IEnumerable? list)
    {
        if (list is null)
        {
            return;
        }

        foreach (var singleNavigationViewItem in list.OfType<NavigationViewItem>())
        {
            if (!_pageIdOrTargetTagNavigationViewsDictionary.ContainsKey(singleNavigationViewItem.Id))
            {
                _pageIdOrTargetTagNavigationViewsDictionary.Add(singleNavigationViewItem.Id, singleNavigationViewItem);
            }

            if (!string.IsNullOrEmpty(singleNavigationViewItem.TargetPageTag) &&
                !_pageIdOrTargetTagNavigationViewsDictionary.ContainsKey(singleNavigationViewItem.TargetPageTag))
            {
                _pageIdOrTargetTagNavigationViewsDictionary.Add(singleNavigationViewItem.TargetPageTag, singleNavigationViewItem);
            }

            if (singleNavigationViewItem.TargetPageType is not null &&
                !_pageTypeNavigationViewsDictionary.ContainsKey(singleNavigationViewItem.TargetPageType))
            {
                _pageTypeNavigationViewsDictionary.Add(singleNavigationViewItem.TargetPageType, singleNavigationViewItem);
            }

            singleNavigationViewItem.IsMenuElement = true;

            if (singleNavigationViewItem.HasMenuItems)
            {
                AddItemsToDictionaries(singleNavigationViewItem.MenuItems);
            }
        }
    }

    /// <summary>Provides the TryToNavigateWithoutINavigationViewItem member.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private bool TryToNavigateWithoutINavigationViewItem(Type pageType, object? dataContext = null)
    {
        var navigationViewItem = new NavigationViewItem(pageType);

        if (!NavigateInternal(navigationViewItem, dataContext))
        {
            return false;
        }

        _pageTypeNavigationViewsDictionary.Add(pageType, navigationViewItem);
        _pageIdOrTargetTagNavigationViewsDictionary.Add(navigationViewItem.Id, navigationViewItem);

        return true;
    }

    /// <summary>Provides the NavigateInternal member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <param name="isJournalNavigation">The isJournalNavigation value.</param>
    /// <param name="journalIndex">The journalIndex value.</param>
    /// <returns>The result.</returns>
    private bool NavigateInternal(
        INavigationViewItem viewItem,
        object? dataContext = null,
        bool isJournalNavigation = false,
        int journalIndex = -1)
    {
        if (_navigationStack.Count > 0 && _navigationStack[^1] == viewItem)
        {
            if (isJournalNavigation)
            {
                AddToJournal(viewItem, true, journalIndex);
                return true;
            }

            return false;
        }

        var pageInstance = GetNavigationItemInstance(viewItem);

        var navigatingArgs = new NavigatingCancelEventArgs(NavigatingEvent, this) { Page = pageInstance };
        RaiseEvent(navigatingArgs);

        if (navigatingArgs.Cancel)
        {
            return false;
        }

        var navigatedArgs = new NavigatedEventArgs(NavigatedEvent, this) { Page = pageInstance };
        RaiseEvent(navigatedArgs);

        UpdateContent(pageInstance, dataContext);

        AddToJournal(viewItem, isJournalNavigation, journalIndex);

        if (_navigationStack.Count == 0 || SelectedItem == _navigationStack[0] || !_navigationStack[0].IsMenuElement)
        {
            return true;
        }

        SelectedItem = _navigationStack[0];
        RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));

        return true;
    }

    /// <summary>Provides the AddToJournal member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="isJournalNavigation">The isJournalNavigation value.</param>
    /// <param name="journalIndex">The journalIndex value.</param>
    private void AddToJournal(INavigationViewItem viewItem, bool isJournalNavigation, int journalIndex)
    {
        if (isJournalNavigation)
        {
            _currentIndexInJournal = journalIndex;
        }
        else
        {
            NavigationJournal.Record(_journal, ref _currentIndexInJournal, viewItem.Id);
        }

        IsBackEnabled = CanGoBack;
    }

    /// <summary>Provides the GetNavigationItemInstance member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <returns>The result.</returns>
    private object GetNavigationItemInstance(INavigationViewItem viewItem)
    {
        if (viewItem.TargetPageType is null)
        {
            throw new ArgumentNullException(nameof(viewItem.TargetPageType));
        }

        if (_serviceProvider is not null)
        {
            return _serviceProvider.GetService(viewItem.TargetPageType)
                ?? throw new InvalidOperationException($"GetService returned null for {viewItem.TargetPageType}");
        }

        if (_pageService is not null)
        {
            return _pageService.GetPage(viewItem.TargetPageType)
                ?? throw new InvalidOperationException($"GetPage returned null for {viewItem.TargetPageType}");
        }

        if (viewItem.TargetPageFactory is null)
        {
            throw new InvalidOperationException($"No page service or AOT-safe page factory is configured for {viewItem.TargetPageType}.");
        }

        return _cache.Remember(viewItem.TargetPageType, viewItem.NavigationCacheMode, viewItem.TargetPageFactory)
            ?? throw new ArgumentNullException($"Unable to create instance of {viewItem.TargetPageType}");
    }

    /// <summary>Provides the UpdateContent member.</summary>
    /// <param name="content">The content value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    private void UpdateContent(object? content, object? dataContext = null)
    {
        if (dataContext is not null && content is Control control)
        {
            control.DataContext = dataContext;
        }

        if (_navigationViewContentPresenter is not null)
        {
            _navigationViewContentPresenter.Content = content;
        }

        Content = content;
    }

    /// <summary>Provides the OnMenuItemsCollectionChanged member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnMenuItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AddItemsToDictionaries();
    }
}
