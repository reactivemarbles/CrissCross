// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI;

/// <summary>
/// Represents the state of a color in multiple color spaces (RGB, HSV, HSL).
/// </summary>
public record struct ColorState
{
    private double _rgbR;
    private double _rgbG;
    private double _rgbB;

    private double _hsvH;
    private double _hsvS;
    private double _hsvV;

    private double _hslH;
    private double _hslS;
    private double _hslL;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorState"/> struct.
    /// </summary>
    /// <param name="rgbR">The RGB red component.</param>
    /// <param name="rgbG">The RGB green component.</param>
    /// <param name="rgbB">The RGB blue component.</param>
    /// <param name="a">The alpha component.</param>
    /// <param name="hsvH">The HSV hue component.</param>
    /// <param name="hsvS">The HSV saturation component.</param>
    /// <param name="hsvV">The HSV value component.</param>
    /// <param name="hslH">The HSL hue component.</param>
    /// <param name="hslS">The HSL saturation component.</param>
    /// <param name="hslL">The HSL lightness component.</param>
    public ColorState(double rgbR, double rgbG, double rgbB, double a, double hsvH, double hsvS, double hsvV, double hslH, double hslS, double hslL)
    {
        _rgbR = rgbR;
        _rgbG = rgbG;
        _rgbB = rgbB;
        A = a;
        _hsvH = hsvH;
        _hsvS = hsvS;
        _hsvV = hsvV;
        _hslH = hslH;
        _hslS = hslS;
        _hslL = hslL;
    }

    /// <summary>
    /// Gets or sets the alpha component.
    /// </summary>
    public double A { get; set; }

    /// <summary>
    /// Gets or sets the RGB red component.
    /// </summary>
    public double RGB_R
    {
        readonly get => _rgbR;
        set
        {
            _rgbR = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>
    /// Gets or sets the RGB green component.
    /// </summary>
    public double RGB_G
    {
        readonly get => _rgbG;
        set
        {
            _rgbG = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>
    /// Gets or sets the RGB blue component.
    /// </summary>
    public double RGB_B
    {
        readonly get => _rgbB;
        set
        {
            _rgbB = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>
    /// Gets or sets the HSV hue component.
    /// </summary>
    public double HSV_H
    {
        readonly get => _hsvH;
        set
        {
            _hsvH = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>
    /// Gets or sets the HSV saturation component.
    /// </summary>
    public double HSV_S
    {
        readonly get => _hsvS;
        set
        {
            _hsvS = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>
    /// Gets or sets the HSV value component.
    /// </summary>
    public double HSV_V
    {
        readonly get => _hsvV;
        set
        {
            _hsvV = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>
    /// Gets or sets the HSL hue component.
    /// </summary>
    public double HSL_H
    {
        readonly get => _hslH;
        set
        {
            _hslH = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>
    /// Gets or sets the HSL saturation component.
    /// </summary>
    public double HSL_S
    {
        readonly get => _hslS;
        set
        {
            _hslS = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>
    /// Gets or sets the HSL lightness component.
    /// </summary>
    public double HSL_L
    {
        readonly get => _hslL;
        set
        {
            _hslL = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>
    /// Sets the ARGB values.
    /// </summary>
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

    private void RecalculateHSLFromRGB()
    {
        var (h, s, l) = ColorSpaceHelper.RgbToHsl(_rgbR, _rgbG, _rgbB);
        if (h != -1)
        {
            _hslH = h;
        }

        if (s != -1)
        {
            _hslS = s;
        }

        _hslL = l;
    }

    private void RecalculateHSLFromHSV()
    {
        var (h, s, l) = ColorSpaceHelper.HsvToHsl(_hsvH, _hsvS, _hsvV);
        _hslH = h;
        if (s != -1)
        {
            _hslS = s;
        }

        _hslL = l;
    }

    private void RecalculateHSVFromRGB()
    {
        var (h, s, v) = ColorSpaceHelper.RgbToHsv(_rgbR, _rgbG, _rgbB);
        if (h != -1)
        {
            _hsvH = h;
        }

        if (s != -1)
        {
            _hsvS = s;
        }

        _hsvV = v;
    }

    private void RecalculateHSVFromHSL()
    {
        var (h, s, v) = ColorSpaceHelper.HslToHsv(_hslH, _hslS, _hslL);
        _hsvH = h;
        if (s != -1)
        {
            _hsvS = s;
        }

        _hsvV = v;
    }

    private void RecalculateRGBFromHSL()
    {
        var (r, g, b) = ColorSpaceHelper.HslToRgb(_hslH, _hslS, _hslL);
        _rgbR = r;
        _rgbG = g;
        _rgbB = b;
    }

    private void RecalculateRGBFromHSV()
    {
        var (r, g, b) = ColorSpaceHelper.HsvToRgb(_hsvH, _hsvS, _hsvV);
        _rgbR = r;
        _rgbG = g;
        _rgbB = b;
    }
}
