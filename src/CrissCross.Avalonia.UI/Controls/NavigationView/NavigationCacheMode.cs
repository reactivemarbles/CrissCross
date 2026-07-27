// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Specifies caching characteristics for a page involved in a navigation.</summary>
public enum NavigationCacheMode
{
    /// <summary>The page is never cached and a new instance of the page is created on each visit.</summary>
    Disabled,

    /// <summary>Provides the Enabled member.</summary>
    Enabled,

    /// <summary>Provides the Required member.</summary>
    Required,
}
