// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Identifies the caller-facing side of a bidirectional navigation request.
/// </summary>
public enum NavigationSourceKind
{
    /// <summary>
    /// The request is keyed by a view model type or instance.
    /// </summary>
    ViewModel,

    /// <summary>
    /// The request is keyed by a view type or instance.
    /// </summary>
    View,
}
