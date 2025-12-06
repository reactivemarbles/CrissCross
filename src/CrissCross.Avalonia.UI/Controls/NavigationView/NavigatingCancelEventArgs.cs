// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// NavigatingCancelEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
public class NavigatingCancelEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatingCancelEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="source">The source.</param>
    public NavigatingCancelEventArgs(RoutedEvent routedEvent, object? source)
        : base(routedEvent, source)
    {
    }

    /// <summary>
    /// Gets the page being navigated to.
    /// </summary>
    /// <value>
    /// The page.
    /// </value>
    public required object Page { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="NavigatingCancelEventArgs"/> should cancel navigation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if cancel; otherwise, <c>false</c>.
    /// </value>
    public bool Cancel { get; set; }
}
