﻿// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// ContentDialogButtonClickEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
public class ContentDialogButtonClickEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    /// <summary>
    /// Gets the button.
    /// </summary>
    /// <value>
    /// The button.
    /// </value>
    public required ContentDialogButton Button { get; init; }
}
