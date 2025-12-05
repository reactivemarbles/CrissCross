// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// NavigationView provides a collapsible navigation menu for top-level navigation.
/// </summary>
public class NavigationView : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="MenuItems"/>.
    /// </summary>
    public static readonly StyledProperty<object?> MenuItemsProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(MenuItems), null);

    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Header), null);

    /// <summary>
    /// Property for <see cref="Content"/>.
    /// </summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Content), null);

    /// <summary>
    /// Property for <see cref="IsPaneOpen"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneOpen), true);

    /// <summary>
    /// Gets or sets the menu items for navigation.
    /// </summary>
    public object? MenuItems
    {
        get => GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the main content.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the navigation pane is open.
    /// </summary>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }
}
