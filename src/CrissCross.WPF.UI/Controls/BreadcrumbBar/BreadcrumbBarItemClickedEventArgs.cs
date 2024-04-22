// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on Windows UI Library
//// Copyright(c) Microsoft Corporation.All rights reserved.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// BreadcrumbBarItemClickedEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="BreadcrumbBarItemClickedEventArgs"/> class.
/// </remarks>
/// <param name="routedEvent">The routed event.</param>
/// <param name="source">The source.</param>
/// <param name="item">The item.</param>
/// <param name="index">The index.</param>
public sealed class BreadcrumbBarItemClickedEventArgs(RoutedEvent routedEvent, object source, object item, int index) :
    RoutedEventArgs(routedEvent, source)
{
    /// <summary>
    /// Gets the Content property value of the BreadcrumbBarItem that is clicked.
    /// </summary>
    public object Item { get; } = item;

    /// <summary>
    /// Gets the index of the item that was clicked.
    /// </summary>
    public int Index { get; } = index;
}
