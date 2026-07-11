// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using CrissCross.WPF.UI.Converters;
using Splat; // for Locator

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents the container for an item in a NavigationView control.
/// When needed, it can be used as a normal button with a <see cref="System.Windows.Controls.Primitives.ButtonBase.Click"/> action.
/// </summary>
[ToolboxItem(true)]
[System.Drawing.ToolboxBitmap(typeof(NavigationViewItem), "NavigationViewItem.bmp")]
[TemplatePart(Name = TemplateElementChevronGrid, Type = typeof(Grid))]
public class NavigationViewItem
    : System.Windows.Controls.Primitives.ButtonBase,
        INavigationViewItem,
        IIconControl,
        IDisposable
{
    /// <summary>Identifies the <see cref="MenuItems"/> dependency property.</summary>
    public static readonly DependencyProperty MenuItemsProperty;

    /// <summary>Property for <see cref="MenuItemsSource"/>.</summary>
    public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register(
        nameof(MenuItemsSource),
        typeof(object),
        typeof(NavigationViewItem),
        new PropertyMetadata(null, OnMenuItemsSourceChanged));

    /// <summary>Identifies the <see cref="HasMenuItems"/> dependency property.</summary>
    public static readonly DependencyProperty HasMenuItemsProperty;

    /// <summary>Property for <see cref="IsActive"/>.</summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(NavigationViewItem),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="IsPaneOpen"/>.</summary>
    public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(
        nameof(IsPaneOpen),
        typeof(bool),
        typeof(NavigationViewItem),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="IsExpanded"/>.</summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(NavigationViewItem),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(NavigationViewItem),
        new PropertyMetadata(null, null, IconSourceElementConverter.ConvertToIconElement));

    /// <summary>Property for <see cref="TargetPageTag"/>.</summary>
    public static readonly DependencyProperty TargetPageTagProperty = DependencyProperty.Register(
        nameof(TargetPageTag),
        typeof(string),
        typeof(NavigationViewItem),
        new PropertyMetadata(string.Empty));

    /// <summary>Property for <see cref="TargetPageType"/>.</summary>
    public static readonly DependencyProperty TargetPageTypeProperty = DependencyProperty.Register(
        nameof(TargetPageType),
        typeof(Type),
        typeof(NavigationViewItem),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="InfoBadge"/>.</summary>
    public static readonly DependencyProperty InfoBadgeProperty = DependencyProperty.Register(
        nameof(InfoBadge),
        typeof(InfoBadge),
        typeof(NavigationViewItem),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="NavigationCacheMode"/>.</summary>
    public static readonly DependencyProperty NavigationCacheModeProperty = DependencyProperty.Register(
        nameof(NavigationCacheMode),
        typeof(NavigationCacheMode),
        typeof(NavigationViewItem),
        new FrameworkPropertyMetadata(NavigationCacheMode.Disabled));

    /// <summary>Target ViewModel type (ViewModel-first navigation).</summary>
    public static readonly DependencyProperty TargetViewModelTypeProperty = DependencyProperty.Register(
        nameof(TargetViewModelType),
        typeof(Type),
        typeof(NavigationViewItem),
        new PropertyMetadata(null));

    /// <summary>Target host name for ViewModel-first navigation.</summary>
    public static readonly DependencyProperty TargetHostNameProperty = DependencyProperty.Register(
        nameof(TargetHostName),
        typeof(string),
        typeof(NavigationViewItem),
        new PropertyMetadata(string.Empty));

    /// <summary>The template element chevron grid.</summary>
    protected const string TemplateElementChevronGrid = "PART_ChevronGrid";

    /// <summary>Provides the HasMenuItemsPropertyKey member.</summary>
    private static readonly DependencyPropertyKey HasMenuItemsPropertyKey;

    /// <summary>Provides the MenuItemsPropertyKey member.</summary>
    private static readonly DependencyPropertyKey MenuItemsPropertyKey;

    /// <summary>Reactive subscriptions per instance.</summary>
    private CompositeDisposable? _subscriptions;

    /// <summary>Stores the _disposed value.</summary>
    private bool _disposed;

    /// <summary>Provides the NavigationViewItem member.</summary>
    static NavigationViewItem()
    {
        HasMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(HasMenuItems),
            typeof(bool),
            typeof(NavigationViewItem),
            new PropertyMetadata(false));
        HasMenuItemsProperty = HasMenuItemsPropertyKey.DependencyProperty;

        MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(MenuItems),
            typeof(ObservableCollection<object>),
            typeof(NavigationViewItem),
            new PropertyMetadata(null));
        MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationViewItem),
            new FrameworkPropertyMetadata(typeof(NavigationViewItem)));
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationViewItem"/> class.</summary>
    public NavigationViewItem()
    {
        Id = Guid.NewGuid().ToString("n");

        // Reactive lifecycle handling (avoid .Events() to keep compatibility)
        _ = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Loaded += h, h => Loaded -= h)
            .Take(1)
            .Subscribe(_ => InitializeNavigationViewEvents());
        _ = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Unloaded += h, h => Unloaded -= h)
            .Subscribe(_ => OnReactiveUnloaded());

        // Initialize the `Items` collection
        var menuItems = new ObservableCollection<object>();
        menuItems.CollectionChanged += OnMenuItems_CollectionChanged;
        SetValue(MenuItemsPropertyKey, menuItems);
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationViewItem"/> class with target page type.</summary>
    /// <param name="targetPageType">Target page type.</param>
    public NavigationViewItem(Type targetPageType)
        : this() => SetValue(TargetPageTypeProperty, targetPageType);

    /// <summary>Initializes a new instance of the <see cref="NavigationViewItem"/> class with a name and target page type.</summary>
    /// <param name="name">Display name.</param>
    /// <param name="targetPageType">Target page type.</param>
    public NavigationViewItem(string name, Type targetPageType)
        : this(targetPageType) => SetValue(ContentProperty, name);

    /// <summary>Initializes a new instance of the <see cref="NavigationViewItem"/> class with name, icon and target page type.</summary>
    /// <param name="name">Display name.</param>
    /// <param name="icon">Symbol icon.</param>
    /// <param name="targetPageType">Target page type.</param>
    public NavigationViewItem(string name, SymbolRegular icon, Type targetPageType)
        : this(targetPageType)
    {
        SetValue(ContentProperty, name);
        SetValue(IconProperty, new SymbolIcon { Symbol = icon });
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationViewItem"/> class with name, icon, target page type and menu items.</summary>
    /// <param name="name">Display name.</param>
    /// <param name="icon">Symbol icon.</param>
    /// <param name="targetPageType">Target page type.</param>
    /// <param name="menuItems">Child menu items.</param>
    public NavigationViewItem(string name, SymbolRegular icon, Type targetPageType, IList menuItems)
        : this(name, icon, targetPageType) => SetValue(MenuItemsProperty, menuItems);

    /// <summary>Initializes a new instance of the <see cref="NavigationViewItem"/> class.</summary>
    /// <param name="name">The name value.</param>
    /// <param name="targetPageTag">The targetPageTag value.</param>
    /// <param name="targetPageType">The targetPageType value.</param>
    internal NavigationViewItem(string name, string targetPageTag, Type targetPageType)
        : this(name, targetPageType) => TargetPageTag = targetPageTag;

    /// <summary>Gets the menu items.</summary>
    public IList MenuItems => (ObservableCollection<object>)GetValue(MenuItemsProperty);

    /// <summary>Gets or sets a source for menu items.</summary>
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

    /// <summary>Gets a value indicating whether this item has menu items.</summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool HasMenuItems
    {
        get => (bool)GetValue(HasMenuItemsProperty);
        private set => SetValue(HasMenuItemsProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether this item is active.</summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether this item is expanded.</summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the parent pane is open.</summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsPaneOpen
    {
        get => (bool)GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <summary>Gets or sets the icon.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets the target page tag.</summary>
    public string TargetPageTag
    {
        get => (string)GetValue(TargetPageTagProperty);
        set => SetValue(TargetPageTagProperty, value);
    }

    /// <summary>Gets or sets the target page type.</summary>
    public Type? TargetPageType
    {
        get => (Type)GetValue(TargetPageTypeProperty);
        set => SetValue(TargetPageTypeProperty, value);
    }

    /// <summary>Gets or sets the target ViewModel type (ViewModel-first navigation). When set it overrides page navigation.</summary>
    public Type? TargetViewModelType
    {
        get => (Type?)GetValue(TargetViewModelTypeProperty);
        set => SetValue(TargetViewModelTypeProperty, value);
    }

    /// <summary>Gets or sets the target host name for ViewModel-first navigation. Empty uses default host.</summary>
    public string? TargetHostName
    {
        get => (string?)GetValue(TargetHostNameProperty);
        set => SetValue(TargetHostNameProperty, value);
    }

    /// <summary>Gets or sets the parent navigation view item.</summary>
    public INavigationViewItem? NavigationViewItemParent { get; set; }

    /// <summary>Gets or sets a value indicating whether this item is a menu element.</summary>
    public bool IsMenuElement { get; set; }

    /// <summary>Gets the identifier.</summary>
    public string Id { get; }

    /// <summary>Gets or sets an info badge.</summary>
    public InfoBadge? InfoBadge
    {
        get => (InfoBadge?)GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    /// <summary>Gets or sets the navigation cache mode.</summary>
    public NavigationCacheMode NavigationCacheMode
    {
        get => (NavigationCacheMode)GetValue(NavigationCacheModeProperty);
        set => SetValue(NavigationCacheModeProperty, value);
    }

    /// <summary>Gets or sets the chevron grid element from template.</summary>
    protected Grid? ChevronGrid { get; set; }

    /// <summary>Activates this item.</summary>
    /// <param name="navigationView">The navigation view.</param>
    public virtual void Activate(INavigationView navigationView)
    {
        if (navigationView is null)
        {
            return;
        }

        IsActive = true;

        if (!navigationView.IsPaneOpen && NavigationViewItemParent is not null)
        {
            NavigationViewItemParent.Activate(navigationView);
        }

        NavigationViewItemParent?.IsExpanded = navigationView.IsPaneOpen && navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top;

        if (Icon is not SymbolIcon symbolIcon || navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.LeftFluent)
        {
            return;
        }

        symbolIcon.Filled = true;
    }

    /// <summary>Deactivates this item.</summary>
    /// <param name="navigationView">The navigation view.</param>
    public virtual void Deactivate(INavigationView navigationView)
    {
        if (navigationView is null)
        {
            return;
        }

        IsActive = false;
        NavigationViewItemParent?.Deactivate(navigationView);

        if (!navigationView.IsPaneOpen && HasMenuItems)
        {
            IsExpanded = false;
        }

        if (Icon is not SymbolIcon symbolIcon || navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.LeftFluent)
        {
            return;
        }

        symbolIcon.Filled = false;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(TemplateElementChevronGrid) is not Grid chevronGrid)
        {
            return;
        }

        ChevronGrid = chevronGrid;
    }

    /// <summary>Performs application-defined tasks associated with freeing resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Core dispose.</summary>
    /// <param name="disposing">Disposing flag.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _subscriptions?.Dispose();
            _subscriptions = null;
        }

        _disposed = true;
    }

    /// <summary>Called when the control is initialized.</summary>
    /// <param name="e">Event args.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        if (!string.IsNullOrWhiteSpace(TargetPageTag) || Content is null)
        {
            return;
        }

        var contentText = Content as string;
        TargetPageTag = contentText ?? Content.ToString()?.ToLower().Trim() ?? string.Empty;
    }

    /// <summary>Click handler.</summary>
    protected override void OnClick()
    {
        if (NavigationView.GetNavigationParent(this) is not { } navigationView)
        {
            return;
        }

        if (HasMenuItems && navigationView.IsPaneOpen)
        {
            IsExpanded = !IsExpanded;
        }

        var handledVmFirst = false;
        if (TargetViewModelType is not null)
        {
            try
            {
                if (AppLocator.Current.GetService(TargetViewModelType) is IRxObject)
                {
                    // Use mixin NavigateToView via lightweight adapter implementing IUseNavigation
                    var adapter = new NavAdapter(TargetHostName);
                    adapter.NavigateToView(TargetViewModelType);
                    handledVmFirst = true;
                    IsActive = true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        if (!handledVmFirst && TargetPageType is not null)
        {
            navigationView.OnNavigationViewItemClick(this);
        }

        base.OnClick();
    }

    /// <summary>Mouse down logic to manage expansion region.</summary>
    /// <param name="e">Mouse args.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (!HasMenuItems || e?.LeftButton != MouseButtonState.Pressed)
        {
            base.OnMouseDown(e);
            return;
        }

        if (NavigationView.GetNavigationParent(this) is not { } navigationView)
        {
            return;
        }

        if (
            !navigationView.IsPaneOpen
            || navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Left
            || ChevronGrid is null)
        {
            base.OnMouseDown(e);
            return;
        }

        var mouseOverChevron = ActualWidth < e.GetPosition(this).X + ChevronGrid.ActualWidth;
        if (!mouseOverChevron)
        {
            base.OnMouseDown(e);
            return;
        }

        IsExpanded = !IsExpanded;

        UpdateActiveChildExpansion(navigationView);

        e.Handled = true;
    }

    /// <summary>Provides the OnMenuItemsSourceChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnMenuItemsSourceChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationViewItem navigationViewItem)
        {
            return;
        }

        navigationViewItem.MenuItems.Clear();

        if (e.NewValue is IEnumerable newItemsSource and not string)
        {
            foreach (var item in newItemsSource)
            {
                _ = navigationViewItem.MenuItems.Add(item);
            }
        }
        else if (e.NewValue is not null)
        {
            _ = navigationViewItem.MenuItems.Add(e.NewValue);
        }
    }

    /// <summary>Updates active child expansion after the chevron is clicked.</summary>
    /// <param name="navigationView">The navigation view.</param>
    private void UpdateActiveChildExpansion(INavigationView navigationView)
    {
        foreach (var menuItem in MenuItems)
        {
            if (menuItem is not INavigationViewItem { IsActive: true })
            {
                continue;
            }

            if (IsExpanded)
            {
                Deactivate(navigationView);
            }
            else
            {
                Activate(navigationView);
            }

            return;
        }
    }

    /// <summary>Provides the OnMenuItems_CollectionChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SetValue(HasMenuItemsPropertyKey, MenuItems.Count > 0);

        foreach (var item in MenuItems.OfType<INavigationViewItem>())
        {
            item.NavigationViewItemParent = this;
        }
    }

    /// <summary>Provides the InitializeNavigationViewEvents member.</summary>
    private void InitializeNavigationViewEvents()
    {
        _subscriptions?.Dispose();
        _subscriptions = [];

        var nav = NavigationView.GetNavigationParent(this);
        if (nav is null)
        {
            return;
        }

        IsPaneOpen = nav.IsPaneOpen;

        _ = Observable.FromEventPattern<EventHandler<RoutedEventArgs>, RoutedEventArgs>(
                h => nav.PaneOpened += h,
                h => nav.PaneOpened -= h)
            .Subscribe(_ => IsPaneOpen = true)
            .DisposeWith(_subscriptions);

        _ = Observable.FromEventPattern<EventHandler<RoutedEventArgs>, RoutedEventArgs>(
                h => nav.PaneClosed += h,
                h => nav.PaneClosed -= h)
            .Subscribe(_ => IsPaneOpen = false)
            .DisposeWith(_subscriptions);
    }

    /// <summary>Provides the OnReactiveUnloaded member.</summary>
    private void OnReactiveUnloaded()
    {
        NavigationViewItemParent = null;
        _subscriptions?.Dispose();
        _subscriptions = null;
    }

    /// <summary>Provides the NavAdapter member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class NavAdapter(string? name) : IUseNavigation
    {
        /// <summary>Gets the Name value.</summary>
        public string? Name { get; } = name ?? string.Empty;
    }
}
