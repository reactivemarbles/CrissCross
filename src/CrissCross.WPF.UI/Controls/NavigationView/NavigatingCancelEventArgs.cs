// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigatingCancelEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
public class NavigatingCancelEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    /// <summary>
    /// Gets the page.
    /// </summary>
    /// <value>
    /// The page.
    /// </value>
    public required object Page { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="NavigatingCancelEventArgs"/> is cancel.
    /// </summary>
    /// <value>
    ///   <c>true</c> if cancel; otherwise, <c>false</c>.
    /// </value>
    public bool Cancel { get; set; }
}
