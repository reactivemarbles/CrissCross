// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents ContentDialogClosedEventArgs.</summary>
/// <seealso cref="RoutedEventArgs" />
/// <param name="routedEvent">The routedEvent value.</param>
/// <param name="source">The source value.</param>
public class ContentDialogClosedEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    /// <summary>Gets the result.</summary>
    /// <value>
    /// The result.
    /// </value>
    public required ContentDialogResult Result { get; init; }
}
