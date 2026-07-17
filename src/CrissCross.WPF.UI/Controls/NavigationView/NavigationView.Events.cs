// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents NavigationView.</summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    /// <summary>Property for <see cref="PaneOpened"/>.</summary>
    public static readonly RoutedEvent PaneOpenedEvent = EventManager.RegisterRoutedEvent(
        nameof(PaneOpened),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(NavigationView));

    /// <summary>Property for <see cref="PaneClosed"/>.</summary>
    public static readonly RoutedEvent PaneClosedEvent = EventManager.RegisterRoutedEvent(
        nameof(PaneClosed),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(NavigationView));

    /// <summary>Property for <see cref="SelectionChanged"/>.</summary>
    public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(SelectionChanged),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(NavigationView));

    /// <summary>Property for <see cref="ItemInvoked"/>.</summary>
    public static readonly RoutedEvent ItemInvokedEvent = EventManager.RegisterRoutedEvent(
        nameof(ItemInvoked),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(NavigationView));

    /// <summary>Property for <see cref="BackRequested"/>.</summary>
    public static readonly RoutedEvent BackRequestedEvent = EventManager.RegisterRoutedEvent(
        nameof(BackRequested),
        RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(NavigationView));

    /// <summary>Property for <see cref="Navigating"/>.</summary>
    public static readonly RoutedEvent NavigatingEvent = EventManager.RegisterRoutedEvent(
        nameof(Navigating),
        RoutingStrategy.Bubble,
        typeof(EventHandler<NavigatingCancelEventArgs>),
        typeof(NavigationView));

    /// <summary>Property for <see cref="NavigatedEvent"/>.</summary>
    public static readonly RoutedEvent NavigatedEvent = EventManager.RegisterRoutedEvent(
        nameof(Navigated),
        RoutingStrategy.Bubble,
        typeof(EventHandler<NavigatedEventArgs>),
        typeof(NavigationView));

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs> PaneOpened
    {
        add => AddHandler(PaneOpenedEvent, value);
        remove => RemoveHandler(PaneOpenedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs> PaneClosed
    {
        add => AddHandler(PaneClosedEvent, value);
        remove => RemoveHandler(PaneClosedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs> SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs> ItemInvoked
    {
        add => AddHandler(ItemInvokedEvent, value);
        remove => RemoveHandler(ItemInvokedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<RoutedEventArgs> BackRequested
    {
        add => AddHandler(BackRequestedEvent, value);
        remove => RemoveHandler(BackRequestedEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<NavigatingCancelEventArgs> Navigating
    {
        add => AddHandler(NavigatingEvent, value);
        remove => RemoveHandler(NavigatingEvent, value);
    }

    /// <inheritdoc/>
    public event EventHandler<NavigatedEventArgs> Navigated
    {
        add => AddHandler(NavigatedEvent, value);
        remove => RemoveHandler(NavigatedEvent, value);
    }

    /// <summary>Raises the pane opened event.</summary>
    protected virtual void OnPaneOpened() => RaiseEvent(new RoutedEventArgs(PaneOpenedEvent, this));

    /// <summary>Raises the pane closed event.</summary>
    protected virtual void OnPaneClosed() => RaiseEvent(new RoutedEventArgs(PaneClosedEvent, this));

    /// <summary>Raises the selection changed event.</summary>
    protected virtual void OnSelectionChanged() => RaiseEvent(new RoutedEventArgs(SelectionChangedEvent, this));

    /// <summary>Raises the item invoked event.</summary>
    protected virtual void OnItemInvoked() => RaiseEvent(new RoutedEventArgs(ItemInvokedEvent, this));

    /// <summary>Raises the back requested event.</summary>
    protected virtual void OnBackRequested() => RaiseEvent(new RoutedEventArgs(BackRequestedEvent));

    /// <summary>Raises the navigating requested event.</summary>
    /// <param name="sourcePage">The source page.</param>
    /// <returns>A bool.</returns>
    protected virtual bool OnNavigating(object sourcePage)
    {
        var eventArgs = new NavigatingCancelEventArgs(NavigatingEvent, this) { Page = sourcePage };

        RaiseEvent(eventArgs);

        return eventArgs.Cancel;
    }

    /// <summary>Raises the navigated requested event.</summary>
    /// <param name="page">The page.</param>
    protected virtual void OnNavigated(object page)
    {
        var eventArgs = new NavigatedEventArgs(NavigatedEvent, this) { Page = page };

        RaiseEvent(eventArgs);
    }
}
