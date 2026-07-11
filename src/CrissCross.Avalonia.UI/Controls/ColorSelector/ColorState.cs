// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI;

/// <summary>Represents the state of a color in multiple color spaces (RGB, HSV, HSL).</summary>
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
public struct ColorState(double rgbR, double rgbG, double rgbB, double a, double hsvH, double hsvS, double hsvV, double hslH, double hslS, double hslL) : IEquatable<ColorState>
{
    /// <summary>Provides the _rgbR member.</summary>
    private double _rgbR = rgbR;

    /// <summary>Provides the _rgbG member.</summary>
    private double _rgbG = rgbG;

    /// <summary>Provides the _rgbB member.</summary>
    private double _rgbB = rgbB;

    /// <summary>Provides the _hsvH member.</summary>
    private double _hsvH = hsvH;

    /// <summary>Provides the _hsvS member.</summary>
    private double _hsvS = hsvS;

    /// <summary>Provides the _hsvV member.</summary>
    private double _hsvV = hsvV;

    /// <summary>Provides the _hslH member.</summary>
    private double _hslH = hslH;

    /// <summary>Provides the _hslS member.</summary>
    private double _hslS = hslS;

    /// <summary>Provides the _hslL member.</summary>
    private double _hslL = hslL;

    /// <summary>Gets or sets the alpha component.</summary>
    public double A { get; set; } = a;

    /// <summary>Gets or sets the RGB red component.</summary>
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

    /// <summary>Gets or sets the RGB green component.</summary>
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

    /// <summary>Gets or sets the RGB blue component.</summary>
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

    /// <summary>Gets or sets the HSV hue component.</summary>
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

    /// <summary>Gets or sets the HSV saturation component.</summary>
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

    /// <summary>Gets or sets the HSV value component.</summary>
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

    /// <summary>Gets or sets the HSL hue component.</summary>
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

    /// <summary>Gets or sets the HSL saturation component.</summary>
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

    /// <summary>Gets or sets the HSL lightness component.</summary>
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

    /// <summary>Determines whether two color states are equal.</summary>
    /// <param name="left">The left color state.</param>
    /// <param name="right">The right color state.</param>
    /// <returns><see langword="true"/> when the states are equal.</returns>
    public static bool operator ==(ColorState left, ColorState right) => left.Equals(right);

    /// <summary>Determines whether two color states are not equal.</summary>
    /// <param name="left">The left color state.</param>
    /// <param name="right">The right color state.</param>
    /// <returns><see langword="true"/> when the states are not equal.</returns>
    public static bool operator !=(ColorState left, ColorState right) => !left.Equals(right);

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

    /// <inheritdoc/>
    public readonly bool Equals(ColorState other) =>
        _rgbR.Equals(other._rgbR) &&
        _rgbG.Equals(other._rgbG) &&
        _rgbB.Equals(other._rgbB) &&
        A.Equals(other.A) &&
        _hsvH.Equals(other._hsvH) &&
        _hsvS.Equals(other._hsvS) &&
        _hsvV.Equals(other._hsvV) &&
        _hslH.Equals(other._hslH) &&
        _hslS.Equals(other._hslS) &&
        _hslL.Equals(other._hslL);

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is ColorState other && Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode() =>
        HashCode.Combine(
            _rgbR,
            _rgbG,
            _rgbB,
            A,
            _hsvH,
            _hsvS,
            _hsvV,
            HashCode.Combine(_hslH, _hslS, _hslL));

    /// <summary>Provides the RecalculateHSLFromRGB member.</summary>
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

    /// <summary>Provides the RecalculateHSLFromHSV member.</summary>
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

    /// <summary>Provides the RecalculateHSVFromRGB member.</summary>
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

    /// <summary>Provides the RecalculateHSVFromHSL member.</summary>
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
}
