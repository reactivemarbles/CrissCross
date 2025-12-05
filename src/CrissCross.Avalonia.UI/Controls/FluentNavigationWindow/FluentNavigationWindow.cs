// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A Fluent Window with integrated navigation support.
/// </summary>
public class FluentNavigationWindow : FluentWindow
{
    /// <summary>
    /// Property for <see cref="NavigationView"/>.
    /// </summary>
    public static readonly StyledProperty<NavigationView?> NavigationViewProperty =
        AvaloniaProperty.Register<FluentNavigationWindow, NavigationView?>(
            nameof(NavigationView), null);

    /// <summary>
    /// Gets or sets the navigation view for this window.
    /// </summary>
    public NavigationView? NavigationView
    {
        get => GetValue(NavigationViewProperty);
        set => SetValue(NavigationViewProperty, value);
    }
}
