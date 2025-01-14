// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Provides data for the <see cref="AutoSuggestBox.TextChanged"/> event.
/// </summary>
public sealed class AutoSuggestBoxTextChangedEventArgs(RoutedEvent eventArgs, object sender)
    : RoutedEventArgs(eventArgs, sender)
{
    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public required string Text { get; init; }

    /// <summary>
    /// Gets the reason.
    /// </summary>
    /// <value>
    /// The reason.
    /// </value>
    public required AutoSuggestionBoxTextChangeReason Reason { get; init; }
}
