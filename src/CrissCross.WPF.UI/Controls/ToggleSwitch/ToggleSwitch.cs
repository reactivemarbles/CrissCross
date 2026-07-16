// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Use <see cref="ToggleSwitch"/> to present users with two mutally exclusive options (like on/off).</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(ToggleSwitch), "ToggleSwitch.bmp")]
public class ToggleSwitch : System.Windows.Controls.Primitives.ToggleButton
{
    /// <summary>Property for <see cref="OffContent"/>.</summary>
    public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register(
        nameof(OffContent),
        typeof(object),
        typeof(ToggleSwitch),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="OnContent"/>.</summary>
    public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register(
        nameof(OnContent),
        typeof(object),
        typeof(ToggleSwitch),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the GetValue value.</summary>
    [Bindable(true)]
    public object OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }

    /// <summary>Gets or sets the GetValue value.</summary>
    [Bindable(true)]
    public object OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }
}
