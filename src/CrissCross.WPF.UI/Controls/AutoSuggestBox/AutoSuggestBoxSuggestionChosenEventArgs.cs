// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Provides data for the <see cref="AutoSuggestBox.SuggestionChosen"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AutoSuggestBoxSuggestionChosenEventArgs"/> class.
/// </remarks>
/// <param name="eventArgs">The event arguments.</param>
/// <param name="sender">The sender.</param>
public sealed class AutoSuggestBoxSuggestionChosenEventArgs(RoutedEvent eventArgs, object sender) : RoutedEventArgs(eventArgs, sender)
{
    /// <summary>
    /// Gets the selected item.
    /// </summary>
    /// <value>
    /// The selected item.
    /// </value>
    public required object SelectedItem { get; init; }
}
