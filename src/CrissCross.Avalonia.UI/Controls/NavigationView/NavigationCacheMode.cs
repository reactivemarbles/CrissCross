// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Specifies caching characteristics for a page involved in a navigation.
/// </summary>
public enum NavigationCacheMode
{
    /// <summary>
    /// The page is never cached and a new instance of the page is created on each visit.
    /// </summary>
    Disabled,

    /// <summary>
    /// The page is cached, but the cached instance is discarded when the size of the cache for the frame is exceeded.
    /// </summary>
    Enabled,

    /// <summary>
    /// The page is cached and the cached instance is reused for every visit regardless of the cache size for the frame.
    /// </summary>
    Required
}
