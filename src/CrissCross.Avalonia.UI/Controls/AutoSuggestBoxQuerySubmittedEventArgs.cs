// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the event that is raised when a user submits a query in an AutoSuggestBox control.
/// </summary>
public sealed class AutoSuggestBoxQuerySubmittedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuggestBoxQuerySubmittedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="sender">The sender.</param>
    public AutoSuggestBoxQuerySubmittedEventArgs(RoutedEvent routedEvent, object sender)
        : base(routedEvent, sender)
    {
    }

    /// <summary>
    /// Gets the query text.
    /// </summary>
    public string QueryText { get; init; } = string.Empty;
}
