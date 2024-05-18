// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
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
        IIconControl
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
    /// The chevron grid.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected Grid? ChevronGrid;
#pragma warning restore SA1401 // Fields should be private

    static NavigationViewItem() => DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationViewItem),
            new FrameworkPropertyMetadata(typeof(NavigationViewItem)));

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    public NavigationViewItem()
    {
        Id = Guid.NewGuid().ToString("n");

        Unloaded += static (sender, _) => ((NavigationViewItem)sender).NavigationViewItemParent = null;

        Loaded += (_, _) => InitializeNavigationViewEvents();

        // Initialize the `Items` collection
        var menuItems = new ObservableCollection<object>();
        menuItems.CollectionChanged += OnMenuItems_CollectionChanged;
        SetValue(MenuItemsPropertyKey, menuItems);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    /// <param name="targetPageType">Type of the target page.</param>
    public NavigationViewItem(Type targetPageType)
        : this() => SetValue(TargetPageTypeProperty, targetPageType);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="targetPageType">Type of the target page.</param>
    public NavigationViewItem(string name, Type targetPageType)
        : this(targetPageType) => SetValue(ContentProperty, name);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="targetPageType">Type of the target page.</param>
    public NavigationViewItem(string name, SymbolRegular icon, Type targetPageType)
        : this(targetPageType)
    {
        SetValue(ContentProperty, name);
        SetValue(IconProperty, new SymbolIcon { Symbol = icon });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewItem"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="targetPageType">Type of the target page.</param>
    /// <param name="menuItems">The menu items.</param>
    public NavigationViewItem(string name, SymbolRegular icon, Type targetPageType, IList menuItems)
        : this(name, icon, targetPageType) => SetValue(MenuItemsProperty, menuItems);

    /// <inheritdoc/>
    public IList MenuItems => (ObservableCollection<object>)GetValue(MenuItemsProperty);

    /// <inheritdoc/>
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
    /// Gets a value indicating whether the <see cref="NavigationViewItem"/> has <see cref="MenuItems"/>.
    /// </summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool HasMenuItems
    {
        get => (bool)GetValue(HasMenuItemsProperty);
        private set => SetValue(HasMenuItemsProperty, value);
    }

    /// <inheritdoc />
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <inheritdoc />
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is pane open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is pane open; otherwise, <c>false</c>.
    /// </value>
    [Browsable(false)]
    [ReadOnly(true)]
    public bool IsPaneOpen
    {
        get => (bool)GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <inheritdoc />
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <inheritdoc />
    public string TargetPageTag
    {
        get => (string)GetValue(TargetPageTagProperty);
        set => SetValue(TargetPageTagProperty, value);
    }

    /// <inheritdoc />
    public Type? TargetPageType
    {
        get => (Type)GetValue(TargetPageTypeProperty);
        set => SetValue(TargetPageTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the information badge.
    /// </summary>
    /// <value>
    /// The information badge.
    /// </value>
    public InfoBadge? InfoBadge
    {
        get => (InfoBadge)GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    /// <inheritdoc/>
    public NavigationCacheMode NavigationCacheMode
    {
        get => (NavigationCacheMode)GetValue(NavigationCacheModeProperty);
        set => SetValue(NavigationCacheModeProperty, value);
    }

    /// <inheritdoc />
    public INavigationViewItem? NavigationViewItemParent { get; set; }

    /// <inheritdoc />
    public bool IsMenuElement { get; set; }

    /// <inheritdoc />
    public string Id { get; }

    /// <summary>
    /// Correctly activates.
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
    /// Correctly deactivates.
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

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        if (string.IsNullOrWhiteSpace(TargetPageTag) && Content is not null)
        {
            TargetPageTag = Content as string ?? Content.ToString()?.ToLower().Trim() ?? string.Empty;
        }
    }

    /// <inheritdoc />
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
    /// Is called when mouse is clicked down.
    /// </summary>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
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
        if (NavigationView.GetNavigationParent(this) is { } navigationView)
        {
            IsPaneOpen = navigationView.IsPaneOpen;

            navigationView.PaneOpened += (_, _) => IsPaneOpen = true;
            navigationView.PaneClosed += (_, _) => IsPaneOpen = false;
        }
    }
}
