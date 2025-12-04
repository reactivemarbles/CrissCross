// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a slider for selecting hue values.
/// </summary>
public class HueSlider : Slider
{
    /// <summary>
    /// Property for <see cref="SelectedHue"/>.
    /// </summary>
    public static readonly StyledProperty<double> SelectedHueProperty =
        AvaloniaProperty.Register<HueSlider, double>(nameof(SelectedHue), 0.0);

    /// <summary>
    /// Gets or sets the selected hue (0-360).
    /// </summary>
    public double SelectedHue
    {
        get => GetValue(SelectedHueProperty);
        set => SetValue(SelectedHueProperty, value);
    }
}
