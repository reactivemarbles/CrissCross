// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that allows users to select a color using HSV model.
/// </summary>
public class ColorSelector : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="SelectedColor"/>.
    /// </summary>
    public static readonly StyledProperty<Color> SelectedColorProperty =
        AvaloniaProperty.Register<ColorSelector, Color>(nameof(SelectedColor), Colors.Red);

    /// <summary>
    /// Property for <see cref="Hue"/>.
    /// </summary>
    public static readonly StyledProperty<double> HueProperty =
        AvaloniaProperty.Register<ColorSelector, double>(nameof(Hue), 0.0);

    /// <summary>
    /// Property for <see cref="Saturation"/>.
    /// </summary>
    public static readonly StyledProperty<double> SaturationProperty =
        AvaloniaProperty.Register<ColorSelector, double>(nameof(Saturation), 1.0);

    /// <summary>
    /// Property for <see cref="Brightness"/>.
    /// </summary>
    public static readonly StyledProperty<double> BrightnessProperty =
        AvaloniaProperty.Register<ColorSelector, double>(nameof(Brightness), 1.0);

    /// <summary>
    /// Property for <see cref="Alpha"/>.
    /// </summary>
    public static readonly StyledProperty<double> AlphaProperty =
        AvaloniaProperty.Register<ColorSelector, double>(nameof(Alpha), 1.0);

    /// <summary>
    /// Property for <see cref="HueColor"/>.
    /// </summary>
    public static readonly StyledProperty<Color> HueColorProperty =
        AvaloniaProperty.Register<ColorSelector, Color>(nameof(HueColor), Colors.Red);

    static ColorSelector()
    {
        HueProperty.Changed.AddClassHandler<ColorSelector>((x, _) => x.UpdateFromHsv());
        SaturationProperty.Changed.AddClassHandler<ColorSelector>((x, _) => x.UpdateFromHsv());
        BrightnessProperty.Changed.AddClassHandler<ColorSelector>((x, _) => x.UpdateFromHsv());
        AlphaProperty.Changed.AddClassHandler<ColorSelector>((x, _) => x.UpdateFromHsv());
    }

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the hue (0-360).
    /// </summary>
    public double Hue
    {
        get => GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    /// <summary>
    /// Gets or sets the saturation (0-1).
    /// </summary>
    public double Saturation
    {
        get => GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    /// <summary>
    /// Gets or sets the brightness/value (0-1).
    /// </summary>
    public double Brightness
    {
        get => GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    /// <summary>
    /// Gets or sets the alpha (0-1).
    /// </summary>
    public double Alpha
    {
        get => GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    /// <summary>
    /// Gets or sets the pure hue color (full saturation and brightness).
    /// </summary>
    public Color HueColor
    {
        get => GetValue(HueColorProperty);
        set => SetValue(HueColorProperty, value);
    }

    private static Color HsvToRgb(double h, double s, double v)
    {
        var hi = (int)(h / 60) % 6;
        var f = (h / 60) - Math.Floor(h / 60);
        var p = v * (1 - s);
        var q = v * (1 - (f * s));
        var t = v * (1 - ((1 - f) * s));

        double r, g, b;
        switch (hi)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            default: r = v; g = p; b = q; break;
        }

        return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    private static Color HsvToRgba(double h, double s, double v, double a)
    {
        var rgb = HsvToRgb(h, s, v);
        return Color.FromArgb((byte)(a * 255), rgb.R, rgb.G, rgb.B);
    }

    private void UpdateFromHsv()
    {
        HueColor = HsvToRgb(Hue, 1.0, 1.0);
        SelectedColor = HsvToRgba(Hue, Saturation, Brightness, Alpha);
    }
}
