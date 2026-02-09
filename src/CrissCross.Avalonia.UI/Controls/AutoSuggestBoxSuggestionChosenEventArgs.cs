// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the event that is raised when a user selects a suggestion in an AutoSuggestBox control.
/// </summary>
/// <remarks>Use this class to access the item that was chosen by the user from the suggestion list. This event
/// data is typically used in an event handler for the AutoSuggestBox.SuggestionChosen event to retrieve the selected
/// item and perform additional actions, such as updating the UI or processing the selection.</remarks>
public sealed class AutoSuggestBoxSuggestionChosenEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuggestBoxSuggestionChosenEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="sender">The sender.</param>
    public AutoSuggestBoxSuggestionChosenEventArgs(RoutedEvent routedEvent, object sender)
        : base(routedEvent, sender)
    {
    }

    /// <summary>
    /// Gets the selected item.
    /// </summary>
    public object SelectedItem { get; init; } = new();
}
