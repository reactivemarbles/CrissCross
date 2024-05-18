// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Specifies how remaining space is distributed.
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
public enum SpacingMode
{
    /// <summary>
    /// Spacing is disabled and all items will be arranged as closely as possible.
    /// </summary>
    None,

    /// <summary>
    /// The remaining space is evenly distributed between the items on a layout row, as well as the start and end of each row.
    /// </summary>
    Uniform,

    /// <summary>
    /// The remaining space is evenly distributed between the items on a layout row, excluding the start and end of each row.
    /// </summary>
    BetweenItemsOnly,

    /// <summary>
    /// The remaining space is evenly distributed between start and end of each row.
    /// </summary>
    StartAndEndOnly
}
