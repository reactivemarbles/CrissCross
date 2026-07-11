// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the NavigationViewBreadcrumbItem member.</summary>
/// <param name="item">The item value.</param>
internal sealed class NavigationViewBreadcrumbItem(INavigationViewItem item)
{
    /// <summary>Gets the Content value.</summary>
    public object Content { get; } = item.Content;

    /// <summary>Gets the PageId value.</summary>
    public string PageId { get; } = item.Id;
}
