// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Defines constants that specify whether the back button is visible in NavigationView.
/// </summary>
public enum NavigationViewBackButtonVisible
{
    /// <summary>
    /// Do not display the back button in NavigationView, and do not reserve space for it in layout.
    /// </summary>
    Collapsed,

    /// <summary>
    /// Display the back button in NavigationView.
    /// </summary>
    Visible,

    /// <summary>
    /// The system chooses whether or not to display the back button, depending on the device/form factor. On phones, tablets, desktops, and hubs, the back button is visible. On Xbox/TV, the back button is collapsed.
    /// </summary>
    Auto
}
