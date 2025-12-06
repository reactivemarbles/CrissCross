// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents the container for an item in a NavigationView control.
/// When needed, it can be used as a normal button with a Click action.
/// </summary>
public class NavigationViewItem : global::Avalonia.Controls.Button, INavigationViewItem
{
    /// <summary>
    /// Property for <see cref="MenuItems"/>.
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, IList> MenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationViewItem, IList>(
            nameof(MenuItems),
            o => o.MenuItems);

    /// <summary>
    /// Property for <see cref="MenuItemsSource"/>.
    /// </summary>
    public static readonly StyledProperty<object?> MenuItemsSourceProperty =
        AvaloniaProperty.Register<NavigationViewItem, object?>(nameof(MenuItemsSource));

    /// <summary>
    /// Property for <see cref="HasMenuItems"/>.
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, bool> HasMenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationViewItem, bool>(
            nameof(HasMenuItems),
            o => o.HasMenuItems);

    /// <summary>
    /// Property for <see cref="IsActive"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<NavigationViewItem, bool>(nameof(IsActive), false);

    /// <summary>
    /// Property for <see cref="IsPaneOpen"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<NavigationViewItem, bool>(nameof(IsPaneOpen), false);

    /// <summary>
    /// Property for <see cref="IsExpanded"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<NavigationViewItem, bool>(nameof(IsExpanded), false);

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<IconElement?> IconProperty =
        AvaloniaProperty.Register<NavigationViewItem, IconElement?>(nameof(Icon));

    /// <summary>
    /// Property for <see cref="TargetPageTag"/>.
    /// </summary>
    public static readonly StyledProperty<string> TargetPageTagProperty =
        AvaloniaProperty.Register<NavigationViewItem, string>(nameof(TargetPageTag), string.Empty);

    /// <summary>
    /// Property for <see cref="TargetPageType"/>.
    /// </summary>
    public static readonly StyledProperty<Type?> TargetPageTypeProperty =
        AvaloniaProperty.Register<NavigationViewItem, Type?>(nameof(TargetPageType));

    /// <summary>
    /// Property for <see cref="TargetViewModelType"/>.
    /// </summary>
    public static readonly StyledProperty<Type?> TargetViewModelTypeProperty =
        AvaloniaProperty.Register<NavigationViewItem, Type?>(nameof(TargetViewModelType));

    /// <summary>
    /// Property for <see cref="TargetHostName"/>.
    /// </summary>
    public static readonly StyledProperty<string?> TargetHostNameProperty =
        AvaloniaProperty.Register<NavigationViewItem, string?>(nameof(TargetHostName));

    /// <summary>
    /// Property for <see cref="InfoBadge"/>.
    /// </summary>
    public static readonly StyledProperty<InfoBadge?> InfoBadgeProperty =
        AvaloniaProperty.Register<NavigationViewItem, InfoBadge?>(nameof(InfoBadge));

    /// <summary>
    /// Property for <see cref="NavigationCacheMode"/>.
    /// </summary>
    public static readonly StyledProperty<NavigationCacheMode> NavigationCacheModeProperty =
        AvaloniaProperty.Register<NavigationViewItem, NavigationCacheMode>(nameof(NavigationCacheMode), NavigationCacheMode.Disabled);

    /// <summary>
    /// The template element chevron grid.
    /// </summary>
    protected const string TemplateElementChevronGrid = "PART_ChevronGrid";

    private readonly ObservableCollection<object> _menuItems = [];
    private bool _hasMenuItems;
    private Grid? _chevronGrid;

    static NavigationViewItem()
    {
        MenuItemsSourceProperty.Changed.AddClassHandler<NavigationViewItem>((x, e) => x.OnMenuItemsSourceChanged(e));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    public NavigationViewItem()
    {
        Id = Guid.NewGuid().ToString("n");
        _menuItems.CollectionChanged += OnMenuItemsCollectionChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    /// <param name="targetPageType">Type of the target page.</param>
    public NavigationViewItem(Type targetPageType)
        : this() => TargetPageType = targetPageType;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="targetPageType">Type of the target page.</param>
    public NavigationViewItem(string name, Type targetPageType)
        : this(targetPageType) => Content = name;

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs>? Click;

    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public IList MenuItems => _menuItems;

    /// <inheritdoc/>
    public object? MenuItemsSource
    {
        get => GetValue(MenuItemsSourceProperty);
        set => SetValue(MenuItemsSourceProperty, value);
    }

    /// <inheritdoc/>
    public bool HasMenuItems
    {
        get => _hasMenuItems;
        private set => SetAndRaise(HasMenuItemsProperty, ref _hasMenuItems, value);
    }

    /// <inheritdoc/>
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the parent pane is open.
    /// </summary>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <inheritdoc/>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <inheritdoc/>
    public IconElement? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <inheritdoc/>
    public string TargetPageTag
    {
        get => GetValue(TargetPageTagProperty);
        set => SetValue(TargetPageTagProperty, value);
    }

    /// <inheritdoc/>
    public Type? TargetPageType
    {
        get => GetValue(TargetPageTypeProperty);
        set => SetValue(TargetPageTypeProperty, value);
    }

    /// <inheritdoc/>
    public Type? TargetViewModelType
    {
        get => GetValue(TargetViewModelTypeProperty);
        set => SetValue(TargetViewModelTypeProperty, value);
    }

    /// <inheritdoc/>
    public string? TargetHostName
    {
        get => GetValue(TargetHostNameProperty);
        set => SetValue(TargetHostNameProperty, value);
    }

    /// <inheritdoc/>
    public InfoBadge? InfoBadge
    {
        get => GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    /// <inheritdoc/>
    public NavigationCacheMode NavigationCacheMode
    {
        get => GetValue(NavigationCacheModeProperty);
        set => SetValue(NavigationCacheModeProperty, value);
    }

    /// <inheritdoc/>
    public IControlTemplate? Template
    {
        get => base.Template;
        set => base.Template = value;
    }

    /// <inheritdoc/>
    public INavigationViewItem? NavigationViewItemParent { get; set; }

    /// <inheritdoc/>
    public bool IsMenuElement { get; set; }

    /// <inheritdoc/>
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
            NavigationViewItemParent.IsExpanded = navigationView.IsPaneOpen &&
                navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top;
        }
    }

    /// <inheritdoc/>
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
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ArgumentNullException.ThrowIfNull(e);

        _chevronGrid = e.NameScope.Find<Grid>(TemplateElementChevronGrid);
    }

    /// <inheritdoc/>
    protected override void OnClick()
    {
        base.OnClick();
        Click?.Invoke(this, new RoutedEventArgs());
    }

    private void OnMenuItemsSourceChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _menuItems.Clear();

        if (e.NewValue is IEnumerable newItemsSource and not string)
        {
            foreach (var item in newItemsSource)
            {
                _menuItems.Add(item);
            }
        }
        else if (e.NewValue != null)
        {
            _menuItems.Add(e.NewValue);
        }
    }

    private void OnMenuItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        HasMenuItems = _menuItems.Count > 0;

        foreach (var item in _menuItems.OfType<INavigationViewItem>())
        {
            item.NavigationViewItemParent = this;
        }
    }
}
