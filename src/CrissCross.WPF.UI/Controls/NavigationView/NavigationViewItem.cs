// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using CrissCross.WPF.UI.Converters;

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
    internal static readonly DependencyPropertyKey HasMenuItemsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(HasMenuItems),
            typeof(bool),
            typeof(NavigationViewItem),
            new PropertyMetadata(false));

    private static readonly DependencyPropertyKey MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(MenuItems),
        typeof(ObservableCollection<object>),
        typeof(NavigationViewItem),
        new PropertyMetadata(null));

    /// <summary>Identifies the <see cref="MenuItems"/> dependency property.</summary>
#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;
#pragma warning restore SA1202 // Elements should be ordered by access

    /// <summary>
    /// Property for <see cref="MenuItemsSource"/>.
    /// </summary>
    public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register(
        nameof(MenuItemsSource),
        typeof(object),
        typeof(NavigationViewItem),
        new PropertyMetadata(null, OnMenuItemsSourceChanged));

    /// <summary>Identifies the <see cref="HasMenuItems"/> dependency property.</summary>
    public static readonly DependencyProperty HasMenuItemsProperty =
        HasMenuItemsPropertyKey.DependencyProperty;

    /// <summary>
    /// Property for <see cref="IsActive"/>.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(NavigationViewItem),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="IsPaneOpen"/>.
    /// </summary>
    public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(
        nameof(IsPaneOpen),
        typeof(bool),
        typeof(NavigationViewItem),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="IsExpanded"/>.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(NavigationViewItem),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(NavigationViewItem),
        new PropertyMetadata(null, null, IconSourceElementConverter.ConvertToIconElement));

    /// <summary>
    /// Property for <see cref="TargetPageTag"/>.
    /// </summary>
    public static readonly DependencyProperty TargetPageTagProperty = DependencyProperty.Register(
        nameof(TargetPageTag),
        typeof(string),
        typeof(NavigationViewItem),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="TargetPageType"/>.
    /// </summary>
    public static readonly DependencyProperty TargetPageTypeProperty = DependencyProperty.Register(
        nameof(TargetPageType),
        typeof(Type),
        typeof(NavigationViewItem),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="InfoBadge"/>.
    /// </summary>
    public static readonly DependencyProperty InfoBadgeProperty = DependencyProperty.Register(
        nameof(InfoBadge),
        typeof(InfoBadge),
        typeof(NavigationViewItem),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="NavigationCacheMode"/>.
    /// </summary>
    public static readonly DependencyProperty NavigationCacheModeProperty = DependencyProperty.Register(
        nameof(NavigationCacheMode),
        typeof(NavigationCacheMode),
        typeof(NavigationViewItem),
        new FrameworkPropertyMetadata(NavigationCacheMode.Disabled));

    /// <summary>
    /// The template element chevron grid.
    /// </summary>
    protected const string TemplateElementChevronGrid = "PART_ChevronGrid";

    /// <summary>
    /// The chevron grid element from template.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected Grid? ChevronGrid;
#pragma warning restore SA1401 // Fields should be private

    private CompositeDisposable? _subscriptions; // reactive subscriptions per instance
    private NavigationView? _navigationViewHost;
    private bool _disposed;

    static NavigationViewItem() => DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationViewItem),
            new FrameworkPropertyMetadata(typeof(NavigationViewItem)));

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    public NavigationViewItem()
    {
        Id = Guid.NewGuid().ToString("n");

        // Reactive lifecycle handling (avoid .Events() to keep compatibility)
        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Loaded += h, h => Loaded -= h)
            .Take(1)
            .Subscribe(_ => InitializeNavigationViewEvents());
        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Unloaded += h, h => Unloaded -= h)
            .Subscribe(_ => OnReactiveUnloaded());

        // Initialize the `Items` collection
        var menuItems = new ObservableCollection<object>();
        menuItems.CollectionChanged += OnMenuItems_CollectionChanged;
        SetValue(MenuItemsPropertyKey, menuItems);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class with target page type.
    /// </summary>
    /// <param name="targetPageType">Target page type.</param>
    public NavigationViewItem(Type targetPageType)
        : this() => SetValue(TargetPageTypeProperty, targetPageType);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class with a name and target page type.
    /// </summary>
    /// <param name="name">Display name.</param>
    /// <param name="targetPageType">Target page type.</param>
    public NavigationViewItem(string name, Type targetPageType)
        : this(targetPageType) => SetValue(ContentProperty, name);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class with name, icon and target page type.
    /// </summary>
    /// <param name="name">Display name.</param>
    /// <param name="icon">Symbol icon.</param>
    /// <param name="targetPageType">Target page type.</param>
    public NavigationViewItem(string name, SymbolRegular icon, Type targetPageType)
        : this(targetPageType)
    {
        SetValue(ContentProperty, name);
        SetValue(IconProperty, new SymbolIcon { Symbol = icon });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class with name, icon, target page type and menu items.
    /// </summary>
    /// <param name="name">Display name.</param>
    /// <param name="icon">Symbol icon.</param>
    /// <param name="targetPageType">Target page type.</param>
    /// <param name="menuItems">Child menu items.</param>
    public NavigationViewItem(string name, SymbolRegular icon, Type targetPageType, IList menuItems)
        : this(name, icon, targetPageType) => SetValue(MenuItemsProperty, menuItems);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    internal NavigationViewItem(string name, string targetPageTag, Type targetPageType)
        : this(name, targetPageType) => TargetPageTag = targetPageTag;

    /// <summary>
    /// Gets the menu items.
    /// </summary>
    public IList MenuItems => (ObservableCollection<object>)GetValue(MenuItemsProperty);

    /// <summary>
    /// Gets or sets a source for menu items.
    /// </summary>
    [Bindable(true)]
    public object? MenuItemsSource
    {
        get => GetValue(MenuItemsSourceProperty);
        set
        {
            if (value == null)
            {
                ClearValue(MenuItemsSourceProperty);
            }
            else
            {
                SetValue(MenuItemsSourceProperty, value);
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this item has menu items.
    /// </summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool HasMenuItems
    {
        get => (bool)GetValue(HasMenuItemsProperty);
        private set => SetValue(HasMenuItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item is active.
    /// </summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item is expanded.
    /// </summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the parent pane is open.
    /// </summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsPaneOpen
    {
        get => (bool)GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the target page tag.
    /// </summary>
    public string TargetPageTag
    {
        get => (string)GetValue(TargetPageTagProperty);
        set => SetValue(TargetPageTagProperty, value);
    }

    /// <summary>
    /// Gets or sets the target page type.
    /// </summary>
    public Type? TargetPageType
    {
        get => (Type)GetValue(TargetPageTypeProperty);
        set => SetValue(TargetPageTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets an info badge.
    /// </summary>
    public InfoBadge? InfoBadge
    {
        get => (InfoBadge)GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    /// <summary>
    /// Gets or sets the navigation cache mode.
    /// </summary>
    public NavigationCacheMode NavigationCacheMode
    {
        get => (NavigationCacheMode)GetValue(NavigationCacheModeProperty);
        set => SetValue(NavigationCacheModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the parent navigation view item.
    /// </summary>
    public INavigationViewItem? NavigationViewItemParent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is a menu element.
    /// </summary>
    public bool IsMenuElement { get; set; }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Activates this item.
    /// </summary>
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

        if (NavigationViewItemParent is not null)
        {
            NavigationViewItemParent.IsExpanded = navigationView.IsPaneOpen && navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top;
        }

        if (Icon is SymbolIcon symbolIcon && navigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftFluent)
        {
            symbolIcon.Filled = true;
        }
    }

    /// <summary>
    /// Deactivates this item.
    /// </summary>
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

        if (Icon is SymbolIcon symbolIcon && navigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftFluent)
        {
            symbolIcon.Filled = false;
        }
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(TemplateElementChevronGrid) is Grid chevronGrid)
        {
            ChevronGrid = chevronGrid;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Core dispose.
    /// </summary>
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

    /// <summary>
    /// Called when the control is initialized.
    /// </summary>
    /// <param name="e">Event args.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        if (string.IsNullOrWhiteSpace(TargetPageTag) && Content is not null)
        {
            TargetPageTag = Content as string ?? Content.ToString()?.ToLower().Trim() ?? string.Empty;
        }
    }

    /// <summary>
    /// Click handler.
    /// </summary>
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

        if (TargetPageType is not null)
        {
            navigationView.OnNavigationViewItemClick(this);
        }

        base.OnClick();
    }

    /// <summary>
    /// Mouse down logic to manage expansion region.
    /// </summary>
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

        for (var i = 0; i < MenuItems.Count; i++)
        {
            var menuItem = MenuItems[i];

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

            break;
        }

        e.Handled = true;
    }

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
                navigationViewItem.MenuItems.Add(item);
            }
        }
        else if (e.NewValue != null)
        {
            navigationViewItem.MenuItems.Add(e.NewValue);
        }
    }

    private void OnMenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SetValue(HasMenuItemsPropertyKey, MenuItems.Count > 0);

        foreach (var item in MenuItems.OfType<INavigationViewItem>())
        {
            item.NavigationViewItemParent = this;
        }
    }

    private void InitializeNavigationViewEvents()
    {
        _subscriptions?.Dispose();
        _subscriptions = new CompositeDisposable();

        _navigationViewHost = NavigationView.GetNavigationParent(this);
        if (_navigationViewHost is null)
        {
            return;
        }

        IsPaneOpen = _navigationViewHost.IsPaneOpen;

        Observable.FromEventPattern<TypedEventHandler<NavigationView, RoutedEventArgs>, RoutedEventArgs>(
                h => _navigationViewHost.PaneOpened += h,
                h => _navigationViewHost.PaneOpened -= h)
            .Subscribe(_ => IsPaneOpen = true)
            .DisposeWith(_subscriptions);

        Observable.FromEventPattern<TypedEventHandler<NavigationView, RoutedEventArgs>, RoutedEventArgs>(
                h => _navigationViewHost.PaneClosed += h,
                h => _navigationViewHost.PaneClosed -= h)
            .Subscribe(_ => IsPaneOpen = false)
            .DisposeWith(_subscriptions);
    }

    private void OnReactiveUnloaded()
    {
        NavigationViewItemParent = null;
        _subscriptions?.Dispose();
        _subscriptions = null;
        _navigationViewHost = null;
    }
}
