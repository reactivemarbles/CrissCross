// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Use <see cref="ToggleSwitch"/> to present users with two mutally exclusive options (like on/off).
/// </summary>
// [ToolboxItem(true)]
// [ToolboxBitmap(typeof(ToggleSwitch), "ToggleSwitch.bmp")]
public class ToggleSwitch : System.Windows.Controls.Primitives.ToggleButton
{
    /// <summary>
    /// Property for <see cref="OffContent"/>.
    /// </summary>
    public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register(
        "OffContent",
        typeof(object),
        typeof(ToggleSwitch),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="OnContent"/>.
    /// </summary>
    public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register(
        "OnContent",
        typeof(object),
        typeof(ToggleSwitch),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets provides the object content that should be displayed when this
    /// <see cref="ToggleSwitch" /> has state of "Off".
    /// </summary>
    [Bindable(true)]
    public object OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }

    /// <summary>
    /// Gets or sets provides the object content that should be displayed when this
    /// <see cref="ToggleSwitch" /> has state of "On".
    /// </summary>
    [Bindable(true)]
    public object OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }
}
