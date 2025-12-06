// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a slider for selecting hue values from 0-360.
/// </summary>
public class HueSlider : RangeBase
{
    /// <summary>
    /// Property for <see cref="SelectedHue"/>.
    /// </summary>
    public static readonly StyledProperty<double> SelectedHueProperty =
        AvaloniaProperty.Register<HueSlider, double>(nameof(SelectedHue), 0.0);

    /// <summary>
    /// Property for <see cref="SelectedColor"/>.
    /// </summary>
    public static readonly StyledProperty<Color> SelectedColorProperty =
        AvaloniaProperty.Register<HueSlider, Color>(nameof(SelectedColor), Colors.Red);

    static HueSlider()
    {
        MinimumProperty.OverrideDefaultValue<HueSlider>(0.0);
        MaximumProperty.OverrideDefaultValue<HueSlider>(360.0);
        ValueProperty.Changed.AddClassHandler<HueSlider>((x, _) => x.OnValueChanged());
        SelectedHueProperty.Changed.AddClassHandler<HueSlider>((x, e) => x.OnSelectedHueChanged(e));
    }

    /// <summary>
    /// Gets or sets the selected hue (0-360).
    /// </summary>
    public double SelectedHue
    {
        get => GetValue(SelectedHueProperty);
        set => SetValue(SelectedHueProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected color based on hue.
    /// </summary>
    public Color SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    private static Color HslToRgb(double h, double s, double l)
    {
        double r, g, b;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            var q = l < 0.5 ? l * (1 + s) : l + s - (l * s);
            var p = (2 * l) - q;
            r = HueToRgb(p, q, h + 120);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 120);
        }

        return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0)
        {
            t += 360;
        }

        if (t > 360)
        {
            t -= 360;
        }

        if (t < 60)
        {
            return p + ((q - p) * t / 60);
        }

        if (t < 180)
        {
            return q;
        }

        if (t < 240)
        {
            return p + ((q - p) * (240 - t) / 60);
        }

        return p;
    }

    private void OnValueChanged()
    {
        SelectedHue = Value;
        UpdateSelectedColor();
    }

    private void OnSelectedHueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        Value = (double)e.NewValue!;
        UpdateSelectedColor();
    }

    private void UpdateSelectedColor()
    {
        SelectedColor = HslToRgb(SelectedHue, 1.0, 0.5);
    }
}
