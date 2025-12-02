// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the <see cref="AutoSuggestBox.TextChanged"/> event.
/// </summary>
public sealed class AutoSuggestBoxTextChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuggestBoxTextChangedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="sender">The sender.</param>
    public AutoSuggestBoxTextChangedEventArgs(RoutedEvent routedEvent, object sender)
        : base(routedEvent, sender)
    {
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the reason.
    /// </summary>
    public AutoSuggestionBoxTextChangeReason Reason { get; init; }
}
