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
/// NavigationView.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    /// <summary>
    /// The header content property.
    /// </summary>
    public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.RegisterAttached(
        "HeaderContent",
        typeof(object),
        typeof(FrameworkElement),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets the content of the header.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>An object.</returns>
    public static object? GetHeaderContent(FrameworkElement target) => target?.GetValue(HeaderContentProperty);

    /// <summary>
    /// Sets the content of the header.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="headerContent">Content of the header.</param>
    public static void SetHeaderContent(FrameworkElement target, object headerContent) =>
        target?.SetValue(HeaderContentProperty, headerContent);
}
