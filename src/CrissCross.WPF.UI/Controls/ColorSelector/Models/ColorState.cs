// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// ColorState.
/// </summary>
public record struct ColorState
{
    private double _RGB_R;
    private double _RGB_G;
    private double _RGB_B;

    private double _HSV_H;
    private double _HSV_S;
    private double _HSV_V;

    private double _HSL_H;
    private double _HSL_S;
    private double _HSL_L;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorState"/> struct.
    /// </summary>
    /// <param name="rGB_R">The r gb r.</param>
    /// <param name="rGB_G">The r gb g.</param>
    /// <param name="rGB_B">The r gb b.</param>
    /// <param name="a">a.</param>
    /// <param name="hSV_H">The h sv h.</param>
    /// <param name="hSV_S">The h sv s.</param>
    /// <param name="hSV_V">The h sv v.</param>
    /// <param name="hSL_h">The h sl h.</param>
    /// <param name="hSL_s">The h sl s.</param>
    /// <param name="hSL_l">The h sl l.</param>
    public ColorState(double rGB_R, double rGB_G, double rGB_B, double a, double hSV_H, double hSV_S, double hSV_V, double hSL_h, double hSL_s, double hSL_l)
    {
        _RGB_R = rGB_R;
        _RGB_G = rGB_G;
        _RGB_B = rGB_B;
        A = a;
        _HSV_H = hSV_H;
        _HSV_S = hSV_S;
        _HSV_V = hSV_V;
        _HSL_H = hSL_h;
        _HSL_S = hSL_s;
        _HSL_L = hSL_l;
    }

    /// <summary>
    /// Gets or sets a.
    /// </summary>
    /// <value>
    /// a.
    /// </value>
    public double A { get; set; }

    /// <summary>
    /// Gets or sets the RGB r.
    /// </summary>
    /// <value>
    /// The RGB r.
    /// </value>
    public double RGB_R
    {
        readonly get => _RGB_R;
        set
        {
            _RGB_R = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>
    /// Gets or sets the RGB g.
    /// </summary>
    /// <value>
    /// The RGB g.
    /// </value>
    public double RGB_G
    {
        readonly get => _RGB_G;
        set
        {
            _RGB_G = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>
    /// Gets or sets the RGB b.
    /// </summary>
    /// <value>
    /// The RGB b.
    /// </value>
    public double RGB_B
    {
        readonly get => _RGB_B;
        set
        {
            _RGB_B = value;
            RecalculateHSVFromRGB();
            RecalculateHSLFromRGB();
        }
    }

    /// <summary>
    /// Gets or sets the HSV h.
    /// </summary>
    /// <value>
    /// The HSV h.
    /// </value>
    public double HSV_H
    {
        readonly get => _HSV_H;
        set
        {
            _HSV_H = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>
    /// Gets or sets the HSV s.
    /// </summary>
    /// <value>
    /// The HSV s.
    /// </value>
    public double HSV_S
    {
        readonly get => _HSV_S;
        set
        {
            _HSV_S = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>
    /// Gets or sets the HSV v.
    /// </summary>
    /// <value>
    /// The HSV v.
    /// </value>
    public double HSV_V
    {
        readonly get => _HSV_V;
        set
        {
            _HSV_V = value;
            RecalculateRGBFromHSV();
            RecalculateHSLFromHSV();
        }
    }

    /// <summary>
    /// Gets or sets the HSL h.
    /// </summary>
    /// <value>
    /// The HSL h.
    /// </value>
    public double HSL_H
    {
        readonly get => _HSL_H;
        set
        {
            _HSL_H = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>
    /// Gets or sets the HSL s.
    /// </summary>
    /// <value>
    /// The HSL s.
    /// </value>
    public double HSL_S
    {
        readonly get => _HSL_S;
        set
        {
            _HSL_S = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>
    /// Gets or sets the HSL l.
    /// </summary>
    /// <value>
    /// The HSL l.
    /// </value>
    public double HSL_L
    {
        readonly get => _HSL_L;
        set
        {
            _HSL_L = value;
            RecalculateRGBFromHSL();
            RecalculateHSVFromHSL();
        }
    }

    /// <summary>
    /// Sets the ARGB.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    public void SetARGB(double a, double r, double g, double b)
    {
        A = a;
        _RGB_R = r;
        _RGB_G = g;
        _RGB_B = b;
        RecalculateHSVFromRGB();
        RecalculateHSLFromRGB();
    }

    private void RecalculateHSLFromRGB()
    {
        var hsltuple = ColorSpaceHelper.RgbToHsl(_RGB_R, _RGB_G, _RGB_B);
        double h = hsltuple.Item1, s = hsltuple.Item2, l = hsltuple.Item3;
        if (h != -1)
        {
            _HSL_H = h;
        }

        if (s != -1)
        {
            _HSL_S = s;
        }

        _HSL_L = l;
    }

    private void RecalculateHSLFromHSV()
    {
        var hsltuple = ColorSpaceHelper.HsvToHsl(_HSV_H, _HSV_S, _HSV_V);
        double h = hsltuple.Item1, s = hsltuple.Item2, l = hsltuple.Item3;
        _HSL_H = h;
        if (s != -1)
        {
            _HSL_S = s;
        }

        _HSL_L = l;
    }

    private void RecalculateHSVFromRGB()
    {
        var hsvtuple = ColorSpaceHelper.RgbToHsv(_RGB_R, _RGB_G, _RGB_B);
        double h = hsvtuple.Item1, s = hsvtuple.Item2, v = hsvtuple.Item3;
        if (h != -1)
        {
            _HSV_H = h;
        }

        if (s != -1)
        {
            _HSV_S = s;
        }

        _HSV_V = v;
    }

    private void RecalculateHSVFromHSL()
    {
        var hsvtuple = ColorSpaceHelper.HslToHsv(_HSL_H, _HSL_S, _HSL_L);
        double h = hsvtuple.Item1, s = hsvtuple.Item2, v = hsvtuple.Item3;
        _HSV_H = h;
        if (s != -1)
        {
            _HSV_S = s;
        }

        _HSV_V = v;
    }

    private void RecalculateRGBFromHSL()
    {
        var rgbtuple = ColorSpaceHelper.HslToRgb(_HSL_H, _HSL_S, _HSL_L);
        _RGB_R = rgbtuple.Item1;
        _RGB_G = rgbtuple.Item2;
        _RGB_B = rgbtuple.Item3;
    }

    private void RecalculateRGBFromHSV()
    {
        var rgbtuple = ColorSpaceHelper.HsvToRgb(_HSV_H, _HSV_S, _HSV_V);
        _RGB_R = rgbtuple.Item1;
        _RGB_G = rgbtuple.Item2;
        _RGB_B = rgbtuple.Item3;
    }
}
