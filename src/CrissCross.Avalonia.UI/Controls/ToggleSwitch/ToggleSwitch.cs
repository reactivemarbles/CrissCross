// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Use <see cref="ToggleSwitch"/> to present users with two mutally exclusive options (like on/off).
/// </summary>
public class ToggleSwitch : global::Avalonia.Controls.Primitives.ToggleButton
{
    /// <summary>
    /// Property for <see cref="OffContent"/>.
    /// </summary>
    public static readonly StyledProperty<object> OffContentProperty = AvaloniaProperty.Register<ToggleSwitch, object>(
        nameof(OffContent), null);

    /// <summary>
    /// Property for <see cref="OnContent"/>.
    /// </summary>
    public static readonly StyledProperty<object> OnContentProperty = AvaloniaProperty.Register<ToggleSwitch, object>(
        nameof(OnContent), null);

    /// <summary>
    /// Gets or sets the object content that should be displayed when this
    /// <see cref="ToggleSwitch" /> has state of "Off".
    /// </summary>
    public object? OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the object content that should be displayed when this
    /// <see cref="ToggleSwitch" /> has state of "On".
    /// </summary>
    public object? OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }
}
