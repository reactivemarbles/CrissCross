// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the NavigationViewBreadcrumbItem member.</summary>
/// <param name="item">The item value.</param>
internal sealed class NavigationViewBreadcrumbItem(INavigationViewItem item)
{
    /// <summary>Gets the Content value.</summary>
    internal object Content { get; } = item.Content;

    /// <summary>Gets the PageId value.</summary>
    internal string PageId { get; } = item.Id;
}
