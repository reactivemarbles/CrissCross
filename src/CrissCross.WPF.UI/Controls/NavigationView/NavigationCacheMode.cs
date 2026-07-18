// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
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
