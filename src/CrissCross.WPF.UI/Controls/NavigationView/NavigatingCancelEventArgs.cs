// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents NavigatingCancelEventArgs.</summary>
/// <seealso cref="RoutedEventArgs" />
/// <param name="routedEvent">The routedEvent value.</param>
/// <param name="source">The source value.</param>
public class NavigatingCancelEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    /// <summary>Gets the page.</summary>
    /// <value>
    /// The page.
    /// </value>
    public required object Page { get; init; }

    /// <summary>Gets or sets a value indicating whether this <see cref="NavigatingCancelEventArgs"/> is cancel.</summary>
    /// <value>
    ///   <c>true</c> if cancel; otherwise, <c>false</c>.
    /// </value>
    public bool Cancel { get; set; }
}
