// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Defines how a chip group tracks selection.
/// </summary>
public enum ChipGroupSelectionMode
{
    /// <summary>
    /// Chips are displayed as labels only and cannot be selected.
    /// </summary>
    None,

    /// <summary>
    /// At most one chip can be selected.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple chips can be selected.
    /// </summary>
    Multiple
}
