// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Ways you can round windows.
/// </summary>
public enum WindowCornerPreference
{
    /// <summary>
    /// Determined by system or application preference.
    /// </summary>
    Default,

    /// <summary>
    /// Do not round the corners.
    /// </summary>
    DoNotRound,

    /// <summary>
    /// Round the corners.
    /// </summary>
    Round,

    /// <summary>
    /// Round the corners slightly.
    /// </summary>
    RoundSmall
}
