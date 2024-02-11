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
/// Provides event data for the <see cref="AutoSuggestBox.QuerySubmitted"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AutoSuggestBoxQuerySubmittedEventArgs"/> class.
/// </remarks>
/// <param name="eventArgs">The event arguments.</param>
/// <param name="sender">The sender.</param>
public sealed class AutoSuggestBoxQuerySubmittedEventArgs(RoutedEvent eventArgs, object sender)
    : RoutedEventArgs(eventArgs, sender)
{
    /// <summary>
    /// Gets the query text.
    /// </summary>
    /// <value>
    /// The query text.
    /// </value>
    public required string QueryText { get; init; }
}
