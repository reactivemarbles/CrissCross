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
/// NavigationView.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="CrissCross.WPF.UI.Controls.INavigationView" />
public partial class NavigationView
{
    /// <summary>
    /// Attached property for <see cref="NavigationView"/>'s to get its parent.
    /// </summary>
    internal static readonly DependencyProperty NavigationParentProperty =
        DependencyProperty.RegisterAttached(
            nameof(NavigationParent),
            typeof(INavigationView),
            typeof(INavigationView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    internal INavigationView NavigationParent
    {
        get => (INavigationView)GetValue(NavigationParentProperty);
        private set => SetValue(NavigationParentProperty, value);
    }

    /// <summary>
    /// Gets the <see cref="NavigationView" /> parent view for its <see cref="INavigationViewItem" /> children.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="navigationItem">The navigation item.</param>
    /// <returns>
    /// Instance of the <see cref="NavigationView" /> or <see langword="null" />.
    /// </returns>
    internal static NavigationView? GetNavigationParent<T>(T navigationItem)
        where T : DependencyObject, INavigationViewItem =>
        navigationItem.GetValue(NavigationParentProperty) is NavigationView navigationView ? navigationView : null;
}
