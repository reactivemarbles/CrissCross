// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A slider control for adjusting the alpha channel of a color.
/// </summary>
public class AlphaSlider : RangeBase
{
    /// <summary>
    /// Property for <see cref="Color"/>.
    /// </summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<AlphaSlider, Color>(nameof(Color), Colors.Red);

    /// <summary>
    /// Property for <see cref="Alpha"/>.
    /// </summary>
    public static readonly StyledProperty<double> AlphaProperty =
        AvaloniaProperty.Register<AlphaSlider, double>(nameof(Alpha), 1.0);

    static AlphaSlider()
    {
        MinimumProperty.OverrideDefaultValue<AlphaSlider>(0.0);
        MaximumProperty.OverrideDefaultValue<AlphaSlider>(1.0);
        ValueProperty.Changed.AddClassHandler<AlphaSlider>((x, _) => x.OnValueChanged());
        AlphaProperty.Changed.AddClassHandler<AlphaSlider>((x, e) => x.OnAlphaChanged(e));
    }

    /// <summary>
    /// Gets or sets the base color (without alpha).
    /// </summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the alpha value (0.0 to 1.0).
    /// </summary>
    public double Alpha
    {
        get => GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    private void OnValueChanged()
    {
        Alpha = Value;
    }

    private void OnAlphaChanged(AvaloniaPropertyChangedEventArgs e)
    {
        Value = (double)e.NewValue!;
    }
}
