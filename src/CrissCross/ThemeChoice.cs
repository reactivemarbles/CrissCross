// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Defines platform-neutral theme choices exposed by theme selection controls.
/// </summary>
public enum ThemeChoice
{
    /// <summary>
    /// Follow the current operating-system or host application theme.
    /// </summary>
    System,

    /// <summary>
    /// Use the light application theme.
    /// </summary>
    Light,

    /// <summary>
    /// Use the dark application theme.
    /// </summary>
    Dark,

    /// <summary>
    /// Use a high-contrast theme when the current platform supports it.
    /// </summary>
    HighContrast
}
