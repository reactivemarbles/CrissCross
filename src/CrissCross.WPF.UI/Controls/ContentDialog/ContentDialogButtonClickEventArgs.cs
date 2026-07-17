// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents ContentDialogButtonClickEventArgs.</summary>
/// <seealso cref="RoutedEventArgs" />
/// <param name="routedEvent">The routedEvent value.</param>
/// <param name="source">The source value.</param>
public class ContentDialogButtonClickEventArgs(RoutedEvent routedEvent, object source)
    : RoutedEventArgs(routedEvent, source)
{
    /// <summary>Gets the button.</summary>
    /// <value>
    /// The button.
    /// </value>
    public required ContentDialogButton Button { get; init; }
}
