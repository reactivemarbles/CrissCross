// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigatedEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
public class NavigatedEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    /// <summary>
    /// Gets the page.
    /// </summary>
    /// <value>
    /// The page.
    /// </value>
    public required object Page { get; init; }
}
