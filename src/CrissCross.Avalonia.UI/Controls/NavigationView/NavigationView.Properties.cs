// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using CrissCross;
using CrissCross.Avalonia.UI.Animations;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Defines the events and styled properties exposed by <see cref="NavigationView"/>.</summary>
public partial class NavigationView
{
    /// <summary>Property for <see cref="MenuItems"/>.</summary>
    public static readonly DirectProperty<NavigationView, IList> MenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, IList>(nameof(MenuItems), o => o.MenuItems);

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
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<
        NavigationView,
        object?
    >(nameof(Header));

    /// <summary>Property for <see cref="HeaderVisibility"/>.</summary>
    public static readonly StyledProperty<bool> HeaderVisibilityProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(HeaderVisibility), true);

    /// <summary>Property for <see cref="AlwaysShowHeader"/>.</summary>
    public static readonly StyledProperty<bool> AlwaysShowHeaderProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(AlwaysShowHeader), false);

    /// <summary>Property for <see cref="Content"/>.</summary>
    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<
        NavigationView,
        object?
    >(nameof(Content));

    /// <summary>Property for <see cref="ContentOverlay"/>.</summary>
    public static readonly StyledProperty<object?> ContentOverlayProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(ContentOverlay));

    /// <summary>Property for <see cref="IsPaneOpen"/>.</summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty = AvaloniaProperty.Register<
        NavigationView,
        bool
    >(nameof(IsPaneOpen), true);

    /// <summary>Property for <see cref="IsPaneVisible"/>.</summary>
    public static readonly StyledProperty<bool> IsPaneVisibleProperty = AvaloniaProperty.Register<
        NavigationView,
        bool
    >(nameof(IsPaneVisible), true);

    /// <summary>Property for <see cref="IsPaneToggleVisible"/>.</summary>
    public static readonly StyledProperty<bool> IsPaneToggleVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneToggleVisible), true);

    /// <summary>Property for <see cref="IsBackEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsBackEnabledProperty = AvaloniaProperty.Register<
        NavigationView,
        bool
    >(nameof(IsBackEnabled), false);

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
    public static readonly StyledProperty<object?> PaneHeaderProperty = AvaloniaProperty.Register<
        NavigationView,
        object?
    >(nameof(PaneHeader));

    /// <summary>Property for <see cref="PaneTitle"/>.</summary>
    public static readonly StyledProperty<string?> PaneTitleProperty = AvaloniaProperty.Register<
        NavigationView,
        string?
    >(nameof(PaneTitle));

    /// <summary>Property for <see cref="PaneFooter"/>.</summary>
    public static readonly StyledProperty<object?> PaneFooterProperty = AvaloniaProperty.Register<
        NavigationView,
        object?
    >(nameof(PaneFooter));

    /// <summary>Property for <see cref="PaneDisplayMode"/>.</summary>
    public static readonly StyledProperty<NavigationViewPaneDisplayMode> PaneDisplayModeProperty =
        AvaloniaProperty.Register<NavigationView, NavigationViewPaneDisplayMode>(
            nameof(PaneDisplayMode),
            NavigationViewPaneDisplayMode.Left);

    /// <summary>Property for <see cref="TitleBar"/>.</summary>
    public static readonly StyledProperty<TitleBar?> TitleBarProperty = AvaloniaProperty.Register<
        NavigationView,
        TitleBar?
    >(nameof(TitleBar));

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
        AvaloniaProperty.Register<NavigationView, Transition>(
            nameof(Transition),
            Transition.FadeInWithSlide);

    /// <summary>Property for <see cref="FrameMargin"/>.</summary>
    public static readonly StyledProperty<Thickness> FrameMarginProperty =
        AvaloniaProperty.Register<NavigationView, Thickness>(nameof(FrameMargin), default);

    /// <summary>Routed event for <see cref="PaneOpened"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> PaneOpenedEvent = RoutedEvent.Register<
        NavigationView,
        RoutedEventArgs
    >(nameof(PaneOpened), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="PaneClosed"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> PaneClosedEvent = RoutedEvent.Register<
        NavigationView,
        RoutedEventArgs
    >(nameof(PaneClosed), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="SelectionChanged"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(
            nameof(SelectionChanged),
            RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="ItemInvoked"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> ItemInvokedEvent = RoutedEvent.Register<
        NavigationView,
        RoutedEventArgs
    >(nameof(ItemInvoked), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="BackRequested"/>.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> BackRequestedEvent = RoutedEvent.Register<
        NavigationView,
        RoutedEventArgs
    >(nameof(BackRequested), RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="Navigating"/>.</summary>
    public static readonly RoutedEvent<NavigatingCancelEventArgs> NavigatingEvent =
        RoutedEvent.Register<NavigationView, NavigatingCancelEventArgs>(
            nameof(Navigating),
            RoutingStrategies.Bubble);

    /// <summary>Routed event for <see cref="Navigated"/>.</summary>
    public static readonly RoutedEvent<NavigatedEventArgs> NavigatedEvent = RoutedEvent.Register<
        NavigationView,
        NavigatedEventArgs
    >(nameof(Navigated), RoutingStrategies.Bubble);

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
}
