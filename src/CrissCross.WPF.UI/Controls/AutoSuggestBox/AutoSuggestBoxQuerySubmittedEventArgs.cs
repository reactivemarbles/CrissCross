// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides event data for the <see cref="AutoSuggestBox.QuerySubmitted"/> event.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AutoSuggestBoxQuerySubmittedEventArgs"/> class.
/// </remarks>
/// <param name="eventArgs">The event arguments.</param>
/// <param name="sender">The sender.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class AutoSuggestBoxQuerySubmittedEventArgs(RoutedEvent eventArgs, object sender) : RoutedEventArgs(eventArgs, sender)
{
    /// <summary>Gets the query text.</summary>
    /// <value>
    /// The query text.
    /// </value>
    public required string QueryText { get; init; }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
