// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// BreadcrumbBarItemClickedEventArgs.
/// </summary>
public sealed class BreadcrumbBarItemClickedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbBarItemClickedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="source">The source.</param>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    public BreadcrumbBarItemClickedEventArgs(RoutedEvent routedEvent, object source, object item, int index)
        : base(routedEvent, source)
    {
        Item = item;
        Index = index;
    }

    /// <summary>
    /// Gets the Content property value of the BreadcrumbBarItem that is clicked.
    /// </summary>
    public object Item { get; }

    /// <summary>
    /// Gets the index of the item that was clicked.
    /// </summary>
    public int Index { get; }
}
