// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//// This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// WindowBackdropType.
/// </summary>
public enum WindowBackdropType
{
    /// <summary>
    /// No backdrop effect.
    /// </summary>
    None,

    /// <summary>
    /// Sets <c>DWMWA_SYSTEMBACKDROP_TYPE</c> to <see langword="0"></see>.
    /// </summary>
    Auto,

    /// <summary>
    /// Windows 11 Mica effect.
    /// </summary>
    Mica,

    /// <summary>
    /// Windows Acrylic effect.
    /// </summary>
    Acrylic,

    /// <summary>
    /// Windows 11 wallpaper blur effect.
    /// </summary>
    Tabbed
}
