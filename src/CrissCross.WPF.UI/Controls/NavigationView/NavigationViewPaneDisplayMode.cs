// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on Windows UI Library
//// Copyright(c) Microsoft Corporation.All rights reserved.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Defines constants that specify how and where the NavigationView pane is shown.
/// </summary>
public enum NavigationViewPaneDisplayMode
{
    /// <summary>
    /// The pane is shown on the left side of the control.
    /// </summary>
    Left,

    /// <summary>
    /// The pane is shown on the left side of the control. Only the pane icons are shown.
    /// </summary>
    LeftMinimal,

    /// <summary>
    /// The pane is shown on the left side of the control. Large icons with titles underneath are the only display option. Does not support <see cref="INavigationViewItem.MenuItems"/>.
    /// <para>Similar to the Windows Store (2022) app.</para>
    /// </summary>
    LeftFluent,

    /// <summary>
    /// The pane is shown at the top of the control.
    /// </summary>
    Top,

    /// <summary>
    /// The pane is shown at the bottom of the control.
    /// </summary>
    Bottom
}
