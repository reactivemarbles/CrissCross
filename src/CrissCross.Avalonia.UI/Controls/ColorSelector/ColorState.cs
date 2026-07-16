// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI;

/// <summary>Represents the state of a color in multiple color spaces (RGB, HSV, HSL).</summary>
public sealed class ColorState
{
    /// <summary>The sentinel used for an undefined color channel.</summary>
    private const double UndefinedChannel = -1D;

    /// <summary>Tolerance used when testing the undefined-channel sentinel.</summary>
    private const double ComparisonTolerance = 1E-10D;

    /// <summary>Provides the _rgbR member.</summary>
    private double _rgbR;

    /// <summary>Provides the _rgbG member.</summary>
    private double _rgbG;

    /// <summary>Provides the _rgbB member.</summary>
    private double _rgbB;

    /// <summary>Provides the _hsvH member.</summary>
    private double _hsvH;

    /// <summary>Provides the _hsvS member.</summary>
    private double _hsvS;

    /// <summary>Provides the _hsvV member.</summary>
    private double _hsvV;

    /// <summary>Provides the _hslH member.</summary>
    private double _hslH;

    /// <summary>Provides the _hslS member.</summary>
    private double _hslS;

    /// <summary>Provides the _hslL member.</summary>
    private double _hslL;

    /// <summary>Initializes a new instance of the <see cref="ColorState"/> class.</summary>
    /// <param name="rgb">The RGB components.</param>
    /// <param name="a">The alpha component.</param>
    /// <param name="hsv">The HSV components.</param>
    /// <param name="hsl">The HSL components.</param>
    public ColorState(RgbColorComponents rgb, double a, HsvColorComponents hsv, HslColorComponents hsl)
    {
        _rgbR = rgb.Red;
        _rgbG = rgb.Green;
        _rgbB = rgb.Blue;
        A = a;
        _hsvH = hsv.Hue;
        _hsvS = hsv.Saturation;
        _hsvV = hsv.Value;
        _hslH = hsl.Hue;
        _hslS = hsl.Saturation;
        _hslL = hsl.Lightness;
    }

    /// <summary>Gets or sets the alpha component.</summary>
    public double A { get; set; }

    /// <summary>Gets or sets the RGB red component.</summary>
    public double RGB_R
    {
        get => _rgbR;
        set
        {
            _rgbR = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>Gets or sets the RGB green component.</summary>
    public double RGB_G
    {
        get => _rgbG;
        set
        {
            _rgbG = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>Gets or sets the RGB blue component.</summary>
    public double RGB_B
    {
        get => _rgbB;
        set
        {
            _rgbB = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>Gets or sets the HSV hue component.</summary>
    public double HSV_H
    {
        get => _hsvH;
        set
        {
            _hsvH = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>Gets or sets the HSV saturation component.</summary>
    public double HSV_S
    {
        get => _hsvS;
        set
        {
            _hsvS = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>Gets or sets the HSV value component.</summary>
    public double HSV_V
    {
        get => _hsvV;
        set
        {
            _hsvV = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>Gets or sets the HSL hue component.</summary>
    public double HSL_H
    {
        get => _hslH;
        set
        {
            _hslH = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>Gets or sets the HSL saturation component.</summary>
    public double HSL_S
    {
        get => _hslS;
        set
        {
            _hslS = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>Gets or sets the HSL lightness component.</summary>
    public double HSL_L
    {
        get => _hslL;
        set
        {
            _hslL = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>Sets the ARGB values.</summary>
    /// <param name="a">The alpha component.</param>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    public void SetARGB(double a, double r, double g, double b)
    {
        A = a;
        _rgbR = r;
        _rgbG = g;
        _rgbB = b;
        RecalculateHSVFromRGB();
        RecalculateHSLFromRGB();
    }

    /// <summary>Provides the RecalculateHSLFromRGB member.</summary>
    private void RecalculateHSLFromRGB()
    {
        var (h, s, l) = ColorSpaceHelper.RgbToHsl(_rgbR, _rgbG, _rgbB);
        if (Math.Abs(h - UndefinedChannel) > ComparisonTolerance)
        {
            _hslH = h;
        }

        if (Math.Abs(s - UndefinedChannel) > ComparisonTolerance)
        {
            _hslS = s;
        }

        _hslL = l;
    }

    /// <summary>Provides the RecalculateHSLFromHSV member.</summary>
    private void RecalculateHSLFromHSV()
    {
        var (h, s, l) = ColorSpaceHelper.HsvToHsl(_hsvH, _hsvS, _hsvV);
        _hslH = h;
        if (Math.Abs(s - UndefinedChannel) > ComparisonTolerance)
        {
            _hslS = s;
        }

        _hslL = l;
    }

    /// <summary>Provides the RecalculateHSVFromRGB member.</summary>
    private void RecalculateHSVFromRGB()
    {
        var (h, s, v) = ColorSpaceHelper.RgbToHsv(_rgbR, _rgbG, _rgbB);
        if (Math.Abs(h - UndefinedChannel) > ComparisonTolerance)
        {
            _hsvH = h;
        }

        if (Math.Abs(s - UndefinedChannel) > ComparisonTolerance)
        {
            _hsvS = s;
        }

        _hsvV = v;
    }

    /// <summary>Provides the RecalculateHSVFromHSL member.</summary>
    private void RecalculateHSVFromHSL()
    {
        var (h, s, v) = ColorSpaceHelper.HslToHsv(_hslH, _hslS, _hslL);
        _hsvH = h;
        if (Math.Abs(s - UndefinedChannel) > ComparisonTolerance)
        {
            _hsvS = s;
        }

        _hsvV = v;
    }

    /// <summary>Provides the RecalculateRGBFromHSL member.</summary>
    private void RecalculateRGBFromHSL()
    {
        var (r, g, b) = ColorSpaceHelper.HslToRgb(_hslH, _hslS, _hslL);
        _rgbR = r;
        _rgbG = g;
        _rgbB = b;
    }

    /// <summary>Provides the RecalculateRGBFromHSV member.</summary>
    private void RecalculateRGBFromHSV()
    {
        var (r, g, b) = ColorSpaceHelper.HsvToRgb(_hsvH, _hsvS, _hsvV);
        _rgbR = r;
        _rgbG = g;
        _rgbB = b;
    }

    /// <summary>Represents normalized RGB color components.</summary>
    /// <param name="Red">The red component.</param>
    /// <param name="Green">The green component.</param>
    /// <param name="Blue">The blue component.</param>
    public readonly record struct RgbColorComponents(double Red, double Green, double Blue);

    /// <summary>Represents normalized HSV color components.</summary>
    /// <param name="Hue">The hue component.</param>
    /// <param name="Saturation">The saturation component.</param>
    /// <param name="Value">The value component.</param>
    public readonly record struct HsvColorComponents(double Hue, double Saturation, double Value);

    /// <summary>Represents normalized HSL color components.</summary>
    /// <param name="Hue">The hue component.</param>
    /// <param name="Saturation">The saturation component.</param>
    /// <param name="Lightness">The lightness component.</param>
    public readonly record struct HslColorComponents(double Hue, double Saturation, double Lightness);
}
