// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// BreadcrumbBarItemClickedEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="BreadcrumbBarItemClickedEventArgs"/> class.
/// </remarks>
/// <param name="routedEvent">The routed event.</param>
/// <param name="source">The source.</param>
/// <param name="item">The item.</param>
/// <param name="index">The index.</param>
public sealed class BreadcrumbBarItemClickedEventArgs(RoutedEvent routedEvent, object source, object item, int index) :
    RoutedEventArgs(routedEvent, source)
{
    /// <summary>
    /// Gets the Content property value of the BreadcrumbBarItem that is clicked.
    /// </summary>
    public object Item { get; } = item;

    /// <summary>
    /// Gets the index of the item that was clicked.
    /// </summary>
    public int Index { get; } = index;
}
