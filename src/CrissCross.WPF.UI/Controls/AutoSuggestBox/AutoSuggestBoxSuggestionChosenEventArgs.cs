// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

// ReSharper disable once CheckNamespace
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
