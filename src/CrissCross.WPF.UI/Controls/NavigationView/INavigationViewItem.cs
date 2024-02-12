// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on Windows UI Library
//// Copyright(c) Microsoft Corporation.All rights reserved.

using System.Collections;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents the container for an item in a NavigationView control.
/// </summary>
public interface INavigationViewItem
{
    /// <summary>
    /// Add / Remove ClickEvent handler.
    /// </summary>
    [Category("Behavior")]
    event RoutedEventHandler Click;

    /// <summary>
    /// Gets unique identifier that allows the item to be located in the navigation.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets or sets get or sets content.
    /// </summary>
    object Content { get; set; }

    /// <summary>
    /// Gets or sets the icon displayed in the MenuItem object.
    /// </summary>
    IconElement? Icon { get; set; }

    /// <summary>
    /// Gets or sets the collection of menu items displayed in the NavigationView.
    /// </summary>
    IList MenuItems { get; set; }

    /// <summary>
    /// Gets or sets an object source used to generate the content of the NavigationView menu.
    /// </summary>
    object? MenuItemsSource { get; set; }

    /// <summary>
    /// Gets a value indicating whether gets information whether the current element is active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets or sets a value indicating whether gets information whether the sub-<see cref="MenuItems"/> are expanded.
    /// </summary>
    bool IsExpanded { get; internal set; }

    /// <summary>
    /// Gets or sets a unique tag used by the parent navigation system for the purpose of searching and navigating.
    /// </summary>
    string TargetPageTag { get; set; }

    /// <summary>
    /// Gets or sets the type of the page to be navigated. (Should be derived from <see cref="FrameworkElement"/>).
    /// </summary>
    Type? TargetPageType { get; set; }

    /// <summary>
    /// Gets or sets the information badge.
    /// </summary>
    /// <value>
    /// The information badge.
    /// </value>
    InfoBadge? InfoBadge { get; set; }

    /// <summary>
    /// Gets or sets specifies caching characteristics for a page involved in a navigation.
    /// </summary>
    NavigationCacheMode NavigationCacheMode { get; set; }

    /// <summary>
    /// Gets or sets template Property.
    /// </summary>
    ControlTemplate? Template { get; set; }

    /// <summary>
    /// Gets or sets parent if in <see cref="MenuItems"/> collection.
    /// </summary>
    INavigationViewItem? NavigationViewItemParent { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is menu element.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is menu element; otherwise, <c>false</c>.
    /// </value>
    internal bool IsMenuElement { get; set; }

    /// <summary>
    /// Correctly activates.
    /// </summary>
    /// <param name="navigationView">The navigation view.</param>
    void Activate(INavigationView navigationView);

    /// <summary>
    /// Correctly deactivates.
    /// </summary>
    /// <param name="navigationView">The navigation view.</param>
    void Deactivate(INavigationView navigationView);
}
