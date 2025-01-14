// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
