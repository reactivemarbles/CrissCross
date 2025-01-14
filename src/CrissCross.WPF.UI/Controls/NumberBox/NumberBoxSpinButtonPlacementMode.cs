// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Defines values that specify how the spin buttons used to increment or decrement the <see cref="NumberBox.Value"/> are displayed.
/// </summary>
public enum NumberBoxSpinButtonPlacementMode
{
    /// <summary>
    /// The spin buttons are not displayed.
    /// </summary>
    Hidden,

    /// <summary>
    /// The spin buttons have two visual states, depending on focus. By default, the spin buttons are displayed in a compact, vertical orientation. When the Numberbox gets focus, the spin buttons expand.
    /// </summary>
    Compact,

    /// <summary>
    /// The spin buttons are displayed in an expanded, horizontal orientation.
    /// </summary>
    Inline
}
