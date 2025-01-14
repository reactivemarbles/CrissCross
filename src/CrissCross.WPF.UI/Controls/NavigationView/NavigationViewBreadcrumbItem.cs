// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

internal class NavigationViewBreadcrumbItem(INavigationViewItem item)
{
    public object Content { get; } = item.Content;

    public string PageId { get; } = item.Id;
}
