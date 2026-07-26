// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContentDialogButtonClickEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    /// <summary>Gets the button.</summary>
    /// <value>
    /// The button.
    /// </value>
    public required ContentDialogButton Button { get; init; }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
