// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Represents color channel state.</summary>
/// <param name="red">The RGB red channel.</param>
/// <param name="green">The RGB green channel.</param>
/// <param name="blue">The RGB blue channel.</param>
/// <param name="a">The alpha channel.</param>
/// <param name="hsvHue">The HSV hue channel.</param>
/// <param name="hsvSaturation">The HSV saturation channel.</param>
/// <param name="hsvValue">The HSV value channel.</param>
/// <param name="hslHue">The HSL hue channel.</param>
/// <param name="hslSaturation">The HSL saturation channel.</param>
/// <param name="hslLightness">The HSL lightness channel.</param>
public struct ColorState(double red, double green, double blue, double a, double hsvHue, double hsvSaturation, double hsvValue, double hslHue, double hslSaturation, double hslLightness) : IEquatable<ColorState>
{
    /// <summary>Stores the red RGB channel.</summary>
    private double _rgbR = red;

    /// <summary>Stores the green RGB channel.</summary>
    private double _rgbG = green;

    /// <summary>Stores the blue RGB channel.</summary>
    private double _rgbB = blue;

    /// <summary>Stores the HSV hue channel.</summary>
    private double _hsvH = hsvHue;

    /// <summary>Stores the HSV saturation channel.</summary>
    private double _hsvS = hsvSaturation;

    /// <summary>Stores the HSV value channel.</summary>
    private double _hsvV = hsvValue;

    /// <summary>Stores the HSL hue channel.</summary>
    private double _hslH = hslHue;

    /// <summary>Stores the HSL saturation channel.</summary>
    private double _hslS = hslSaturation;

    /// <summary>Stores the HSL lightness channel.</summary>
    private double _hslL = hslLightness;

    /// <summary>Gets or sets a.</summary>
    /// <value>
    /// a.
    /// </value>
    public double A { get; set; } = a;

    /// <summary>Gets or sets the RGB r.</summary>
    /// <value>
    /// The RGB r.
    /// </value>
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

    /// <summary>Gets or sets the RGB g.</summary>
    /// <value>
    /// The RGB g.
    /// </value>
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

    /// <summary>Gets or sets the RGB b.</summary>
    /// <value>
    /// The RGB b.
    /// </value>
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

    /// <summary>Gets or sets the HSV h.</summary>
    /// <value>
    /// The HSV h.
    /// </value>
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

    /// <summary>Gets or sets the HSV s.</summary>
    /// <value>
    /// The HSV s.
    /// </value>
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

    /// <summary>Gets or sets the HSV v.</summary>
    /// <value>
    /// The HSV v.
    /// </value>
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

    /// <summary>Gets or sets the HSL h.</summary>
    /// <value>
    /// The HSL h.
    /// </value>
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

    /// <summary>Gets or sets the HSL s.</summary>
    /// <value>
    /// The HSL s.
    /// </value>
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

    /// <summary>Gets or sets the HSL l.</summary>
    /// <value>
    /// The HSL l.
    /// </value>
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

    /// <summary>Determines whether two instances are equal.</summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns><see langword="true"/> when the values are equal.</returns>
    public static bool operator ==(ColorState left, ColorState right) => left.Equals(right);

    /// <summary>Determines whether two instances are different.</summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns><see langword="true"/> when the values are different.</returns>
    public static bool operator !=(ColorState left, ColorState right) => !left.Equals(right);

    /// <inheritdoc/>
    public readonly bool Equals(ColorState other) =>
        _rgbR.Equals(other._rgbR)
        && _rgbG.Equals(other._rgbG)
        && _rgbB.Equals(other._rgbB)
        && A.Equals(other.A)
        && _hsvH.Equals(other._hsvH)
        && _hsvS.Equals(other._hsvS)
        && _hsvV.Equals(other._hsvV)
        && _hslH.Equals(other._hslH)
        && _hslS.Equals(other._hslS)
        && _hslL.Equals(other._hslL);

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is ColorState other && Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            var hashCode = _rgbR.GetHashCode();
            hashCode = (hashCode * 397) ^ _rgbG.GetHashCode();
            hashCode = (hashCode * 397) ^ _rgbB.GetHashCode();
            hashCode = (hashCode * 397) ^ A.GetHashCode();
            hashCode = (hashCode * 397) ^ _hsvH.GetHashCode();
            hashCode = (hashCode * 397) ^ _hsvS.GetHashCode();
            hashCode = (hashCode * 397) ^ _hsvV.GetHashCode();
            hashCode = (hashCode * 397) ^ _hslH.GetHashCode();
            hashCode = (hashCode * 397) ^ _hslS.GetHashCode();
            return (hashCode * 397) ^ _hslL.GetHashCode();
        }
    }

    /// <summary>Sets the ARGB.</summary>
    /// <param name="a">a.</param>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
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
        var hsltuple = ColorSpaceHelper.RgbToHsl(_rgbR, _rgbG, _rgbB);
        double h = hsltuple.Item1;
        double s = hsltuple.Item2;
        double l = hsltuple.Item3;
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
        var hsltuple = ColorSpaceHelper.HsvToHsl(_hsvH, _hsvS, _hsvV);
        double h = hsltuple.Item1;
        double s = hsltuple.Item2;
        double l = hsltuple.Item3;
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
        var hsvtuple = ColorSpaceHelper.RgbToHsv(_rgbR, _rgbG, _rgbB);
        double h = hsvtuple.Item1;
        double s = hsvtuple.Item2;
        double v = hsvtuple.Item3;
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
        var hsvtuple = ColorSpaceHelper.HslToHsv(_hslH, _hslS, _hslL);
        double h = hsvtuple.Item1;
        double s = hsvtuple.Item2;
        double v = hsvtuple.Item3;
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
        var rgbtuple = ColorSpaceHelper.HslToRgb(_hslH, _hslS, _hslL);
        _rgbR = rgbtuple.Item1;
        _rgbG = rgbtuple.Item2;
        _rgbB = rgbtuple.Item3;
    }

    /// <summary>Provides the RecalculateRGBFromHSV member.</summary>
    private void RecalculateRGBFromHSV()
    {
        var rgbtuple = ColorSpaceHelper.HsvToRgb(_hsvH, _hsvS, _hsvV);
        _rgbR = rgbtuple.Item1;
        _rgbG = rgbtuple.Item2;
        _rgbB = rgbtuple.Item3;
    }
}
