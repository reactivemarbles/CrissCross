// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a slider for selecting hue values from 0-360.</summary>
public class HueSlider : RangeBase
{
    /// <summary>Property for <see cref="SelectedHue"/>.</summary>
    public static readonly StyledProperty<double> SelectedHueProperty =
        AvaloniaProperty.Register<HueSlider, double>(nameof(SelectedHue), 0.0);

    /// <summary>Property for <see cref="SelectedColor"/>.</summary>
    public static readonly StyledProperty<Color> SelectedColorProperty =
        AvaloniaProperty.Register<HueSlider, Color>(nameof(SelectedColor), Colors.Red);

    /// <summary>Provides the HueSlider member.</summary>
    static HueSlider()
    {
        const double hueDegrees = 360.0;

        MinimumProperty.OverrideDefaultValue<HueSlider>(0.0);
        MaximumProperty.OverrideDefaultValue<HueSlider>(hueDegrees);
        _ = ValueProperty.Changed.AddClassHandler<HueSlider>((x, _) => x.OnValueChanged());
        _ = SelectedHueProperty.Changed.AddClassHandler<HueSlider>((x, e) => x.OnSelectedHueChanged(e));
    }

    /// <summary>Gets or sets the selected hue (0-360).</summary>
    public double SelectedHue
    {
        get => GetValue(SelectedHueProperty);
        set => SetValue(SelectedHueProperty, value);
    }

    /// <summary>Gets or sets the selected color based on hue.</summary>
    public Color SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>Provides the HslToRgb member.</summary>
    /// <param name="h">The h value.</param>
    /// <param name="s">The s value.</param>
    /// <param name="l">The l value.</param>
    /// <returns>The result.</returns>
    private static Color HslToRgb(double h, double s, double l)
    {
        const double halfScale = 0.5;
        const double doubleScale = 2.0;
        const double complementaryHueOffset = 120.0;
        const double byteChannelScale = byte.MaxValue;

        double r;
        double g;
        double b;

        if (s == 0)
        {
            r = l;
            g = l;
            b = l;
        }
        else
        {
            var q = l < halfScale ? l * (1 + s) : l + s - (l * s);
            var p = (doubleScale * l) - q;
            r = HueToRgb(p, q, h + complementaryHueOffset);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - complementaryHueOffset);
        }

        return Color.FromRgb((byte)(r * byteChannelScale), (byte)(g * byteChannelScale), (byte)(b * byteChannelScale));
    }

    /// <summary>Provides the HueToRgb member.</summary>
    /// <param name="p">The p value.</param>
    /// <param name="q">The q value.</param>
    /// <param name="t">The t value.</param>
    /// <returns>The result.</returns>
    private static double HueToRgb(double p, double q, double t)
    {
        const double hueDegrees = 360.0;
        const double hueDegreesPerSector = 60.0;
        const double greenHueUpperBound = 180.0;
        const double blueHueUpperBound = 240.0;

        if (t < 0)
        {
            t += hueDegrees;
        }

        if (t > hueDegrees)
        {
            t -= hueDegrees;
        }

        if (t < hueDegreesPerSector)
        {
            return p + ((q - p) * t / hueDegreesPerSector);
        }

        if (t < greenHueUpperBound)
        {
            return q;
        }

        return t < blueHueUpperBound ? p + ((q - p) * (blueHueUpperBound - t) / hueDegreesPerSector) : p;
    }

    /// <summary>Provides the OnValueChanged member.</summary>
    private void OnValueChanged()
    {
        SelectedHue = Value;
        UpdateSelectedColor();
    }

    /// <summary>Provides the OnSelectedHueChanged member.</summary>
    /// <param name="e">The e value.</param>
    private void OnSelectedHueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        Value = (double)e.NewValue!;
        UpdateSelectedColor();
    }

    /// <summary>Provides the UpdateSelectedColor member.</summary>
    private void UpdateSelectedColor()
    {
        const double halfScale = 0.5;

        SelectedColor = HslToRgb(SelectedHue, 1.0, halfScale);
    }
}
