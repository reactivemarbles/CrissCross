// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//// This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Types of the available color accents of the controls.
/// </summary>
public enum ControlAppearance
{
    /// <summary>
    /// Control color according to the current theme accent.
    /// </summary>
    Primary,

    /// <summary>
    /// Control color according to the current theme element.
    /// </summary>
    Secondary,

    /// <summary>
    /// Blue color theme.
    /// </summary>
    Info,

    /// <summary>
    /// Dark color theme.
    /// </summary>
    Dark,

    /// <summary>
    /// Light color theme.
    /// </summary>
    Light,

    /// <summary>
    /// Red color theme.
    /// </summary>
    Danger,

    /// <summary>
    /// Green color theme.
    /// </summary>
    Success,

    /// <summary>
    /// Orange color theme.
    /// </summary>
    Caution,

    /// <summary>
    /// Transparent color theme.
    /// </summary>
    Transparent
}
