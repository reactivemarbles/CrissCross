// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Navigation controls (back/forward buttons, etc.).
/// </summary>
public class NavigationControls : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="CanGoBack"/>.
    /// </summary>
    public static readonly StyledProperty<bool> CanGoBackProperty =
        AvaloniaProperty.Register<NavigationControls, bool>(nameof(CanGoBack), false);

    /// <summary>
    /// Property for <see cref="CanGoForward"/>.
    /// </summary>
    public static readonly StyledProperty<bool> CanGoForwardProperty =
        AvaloniaProperty.Register<NavigationControls, bool>(nameof(CanGoForward), false);

    /// <summary>
    /// Gets or sets a value indicating whether navigation can go back.
    /// </summary>
    public bool CanGoBack
    {
        get => GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether navigation can go forward.
    /// </summary>
    public bool CanGoForward
    {
        get => GetValue(CanGoForwardProperty);
        set => SetValue(CanGoForwardProperty, value);
    }
}
