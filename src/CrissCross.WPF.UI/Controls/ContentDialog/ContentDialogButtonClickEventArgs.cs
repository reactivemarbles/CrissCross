﻿// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// ContentDialogButtonClickEventArgs.
/// </summary>
/// <seealso cref="System.Windows.RoutedEventArgs" />
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
