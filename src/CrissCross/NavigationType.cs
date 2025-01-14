// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Identifies the types of navigation that are supported.
/// </summary>
public enum NavigationType
{
    /// <summary>
    /// Navigating to new content.
    /// </summary>
    New = 0,

    /// <summary>
    /// Navigating back in the back navigation history.
    /// </summary>
    Back = 1,

    /// <summary>
    /// Reloading the current content.
    /// </summary>
    Refresh = 2
}