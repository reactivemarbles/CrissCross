// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SkiaSharp;

namespace CrissCross.Plot;

/// <summary>Represents a red, green, blue, and alpha color.</summary>
public readonly struct Color : IEquatable<Color>
{
    /// <summary>Gets the red component.</summary>
    [SuppressMessage("Style", "SST1401", Justification = "The public field is retained for binary and source compatibility with the existing API.")]
    public readonly byte Red;

    /// <summary>Gets the green component.</summary>
    [SuppressMessage("Style", "SST1401", Justification = "The public field is retained for binary and source compatibility with the existing API.")]
    public readonly byte Green;

    /// <summary>Gets the blue component.</summary>
    [SuppressMessage("Style", "SST1401", Justification = "The public field is retained for binary and source compatibility with the existing API.")]
    public readonly byte Blue;

    /// <summary>Gets the alpha component.</summary>
    [SuppressMessage("Style", "SST1401", Justification = "The public field is retained for binary and source compatibility with the existing API.")]
    public readonly byte Alpha;

    /// <summary>The maximum value representable by a color channel.</summary>
    private const int ByteMaximum = byte.MaxValue;

    /// <summary>The half fraction used in color conversions.</summary>
    private const float FractionHalf = 0.5F;

    /// <summary>The one-third fraction used in color conversions.</summary>
    private const float FractionThird = 1F / 3F;

    /// <summary>The two-thirds fraction used in color conversions.</summary>
    private const float FractionTwoThirds = 2F / 3F;

    /// <summary>The value two used in HSL conversions.</summary>
    private const float FractionTwo = 2F;

    /// <summary>The value four used in HSL conversions.</summary>
    private const float FractionFour = 4F;

    /// <summary>The value six used in HSL conversions.</summary>
    private const float FractionSix = 6F;

    /// <summary>Initializes a new instance of the <see cref="Color"/> struct from byte components.</summary>
    /// <param name="red">The red component.</param>
    /// <param name="green">The green component.</param>
    /// <param name="blue">The blue component.</param>
    /// <param name="alpha">The alpha component.</param>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional alpha parameter is retained for compatibility with the existing public API.")]
    public Color(byte red, byte green, byte blue, byte alpha = 255)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    /// <summary>Initializes a new instance of the <see cref="Color"/> struct from fractional components.</summary>
    /// <param name="red">The red component from 0 to 1.</param>
    /// <param name="green">The green component from 0 to 1.</param>
    /// <param name="blue">The blue component from 0 to 1.</param>
    /// <param name="alpha">The alpha component from 0 to 1.</param>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional alpha parameter is retained for compatibility with the existing public API.")]
    public Color(float red, float green, float blue, float alpha = 1)
    {
        Red = (byte)(red * ByteMaximum);
        Green = (byte)(green * ByteMaximum);
        Blue = (byte)(blue * ByteMaximum);
        Alpha = (byte)(alpha * ByteMaximum);
    }

    /// <summary>Initializes a new instance of the <see cref="Color"/> struct from hexadecimal text.</summary>
    /// <param name="hexCode">A hexadecimal color string.</param>
    public Color(string hexCode)
        : this(Hex2Argb(hexCode))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Color"/> struct from an ARGB value.</summary>
    /// <param name="argb">The unsigned ARGB value.</param>
    public Color(uint argb)
    {
        Alpha = (byte)(argb >> 24);
        Red = (byte)(argb >> 16);
        Green = (byte)(argb >> 8);
        Blue = (byte)argb;
    }

    /// <summary>Initializes a new instance of the <see cref="Color"/> struct from a SkiaSharp color.</summary>
    /// <param name="color">The SkiaSharp color.</param>
    public Color(SKColor color)
    {
        Alpha = color.Alpha;
        Red = color.Red;
        Green = color.Green;
        Blue = color.Blue;
    }

    /// <summary>Initializes a new instance of the <see cref="Color"/> struct from a System.Drawing color.</summary>
    /// <param name="color">The System.Drawing color.</param>
    public Color(System.Drawing.Color color)
    {
        Alpha = color.A;
        Red = color.R;
        Green = color.G;
        Blue = color.B;
    }

    /// <summary>Gets the red component.</summary>
    public byte R => Red;

    /// <summary>Gets the green component.</summary>
    public byte G => Green;

    /// <summary>Gets the blue component.</summary>
    public byte B => Blue;

    /// <summary>Gets the alpha component.</summary>
    public byte A => Alpha;

    /// <summary>Gets the ARGB value.</summary>
    public uint ARGB => ((uint)Alpha << 24)
        | ((uint)Red << 16)
        | ((uint)Green << 8)
        | (uint)Blue;

    /// <summary>Gets the opacity fraction from 0 to 1.</summary>
    public double Opacity => Alpha / (double)ByteMaximum;

    /// <summary>Gets the premultiplied ARGB value.</summary>
    public uint PremultipliedARGB
    {
        get
        {
            var premultipliedRed = (byte)((Red * Alpha) / ByteMaximum);
            var premultipliedGreen = (byte)((Green * Alpha) / ByteMaximum);
            var premultipliedBlue = (byte)((Blue * Alpha) / ByteMaximum);
            return ((uint)Alpha << 24)
                | ((uint)premultipliedRed << 16)
                | ((uint)premultipliedGreen << 8)
                | (uint)premultipliedBlue;
        }
    }

    /// <summary>Gets luminance as a fraction from 0 to 1.</summary>
    public float Luminance => ToHSL().l;

    /// <summary>Gets hue as a fraction from 0 to 1.</summary>
    public float Hue => ToHSL().h;

    /// <summary>Gets saturation as a fraction from 0 to 1.</summary>
    public float Saturation => ToHSL().s;

    /// <summary>Creates a grayscale color.</summary>
    /// <param name="value">The gray component value.</param>
    /// <returns>The grayscale color.</returns>
    public static Color Gray(byte value) => new(value, value, value);

    /// <summary>Creates a CrissCross.Plot color from a System.Drawing color.</summary>
    /// <param name="color">The System.Drawing color.</param>
    /// <returns>The corresponding CrissCross plot color.</returns>
    public static Color FromColor(System.Drawing.Color color) => FromSDColor(color);

    /// <summary>Gets a color from a signed ARGB value.</summary>
    /// <param name="argb">The signed ARGB value.</param>
    /// <returns>The corresponding color.</returns>
    public static Color FromARGB(int argb) => FromARGB((uint)argb);

    /// <summary>Gets a color from an unsigned ARGB value.</summary>
    /// <param name="argb">The unsigned ARGB value.</param>
    /// <returns>The corresponding color.</returns>
    public static Color FromARGB(uint argb) => new(argb);

    /// <summary>Creates a color from hexadecimal text.</summary>
    /// <param name="hex">The hexadecimal color text.</param>
    /// <returns>The parsed color.</returns>
    public static Color FromHex(string hex) => new(hex);

    /// <summary>Creates colors from hexadecimal text.</summary>
    /// <param name="hex">The hexadecimal color strings.</param>
    /// <returns>An array of parsed colors.</returns>
    public static Color[] FromHex(IEnumerable<string> hex)
    {
        List<Color> result = [];
        foreach (string hexCode in hex)
        {
            result.Add(FromHex(hexCode));
        }

        return result.ToArray();
    }

    /// <summary>Creates a color from HTML text.</summary>
    /// <param name="html">The HTML color text.</param>
    /// <returns>The parsed color.</returns>
    public static Color FromHtml(string html) => new(html);

    /// <summary>Converts a CrissCross plot color to a System.Drawing color.</summary>
    /// <param name="color">The CrissCross plot color.</param>
    /// <returns>The System.Drawing color.</returns>
    public static System.Drawing.Color ToColor(Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

    /// <summary>Creates a CrissCross.Plot color from a SkiaSharp color.</summary>
    /// <param name="skcolor">The SkiaSharp color.</param>
    /// <returns>The corresponding CrissCross plot color.</returns>
    public static Color FromSKColor(SKColor skcolor) => new(skcolor.Red, skcolor.Green, skcolor.Blue, skcolor.Alpha);

    /// <summary>Creates a CrissCross plot color from a System.Drawing color.</summary>
    /// <param name="sdColor">The System.Drawing color.</param>
    /// <returns>The corresponding CrissCross plot color.</returns>
    [SuppressMessage("Style", "SST1305", Justification = "Retained as the existing public parameter name for named-argument compatibility.")]
    public static Color FromSDColor(System.Drawing.Color sdColor) => new(sdColor.R, sdColor.G, sdColor.B, sdColor.A);

    /// <summary>Creates a color given hue, saturation, and luminance.</summary>
    /// <param name="hue">Hue from 0 to 1.</param>
    /// <param name="saturation">Saturation from 0 to 1.</param>
    /// <param name="luminosity">Luminance from 0 to 1.</param>
    /// <param name="alpha">Alpha from 0 to 1.</param>
    /// <returns>The converted color.</returns>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional alpha parameter is retained for compatibility with the existing public API.")]
    [SuppressMessage("Style", "SST1471", Justification = "Named constants would obscure this adapted HSL conversion algorithm without changing behavior.")]
    public static Color FromHSL(float hue, float saturation, float luminosity, float alpha = 1)
    {
        // adapted from Microsoft.Maui.Graphics/Color.cs
        if (luminosity == 0)
        {
            return new(0, 0, 0);
        }

        if (saturation == 0)
        {
            return new(luminosity, luminosity, luminosity);
        }

        var temp2 = luminosity <= FractionHalf ? luminosity * (1F + saturation) : luminosity + saturation - (luminosity * saturation);
        var temp1 = (2F * luminosity) - temp2;

        var t3 = new[] { hue + FractionThird, hue, hue - FractionThird };
        var clr = new float[] { 0, 0, 0 };
        for (var i = 0; i < 3; i++)
        {
            if (t3[i] < 0)
            {
                t3[i]++;
            }

            if (t3[i] > 1)
            {
                t3[i]--;
            }

            if (6F * t3[i] < 1F)
            {
                clr[i] = temp1 + ((temp2 - temp1) * t3[i] * 6F);
            }
            else if (2F * t3[i] < 1F)
            {
                clr[i] = temp2;
            }
            else if (3F * t3[i] < FractionTwoThirds)
            {
                clr[i] = temp1 + ((temp2 - temp1) * (FractionTwoThirds - t3[i]) * 6F);
            }
            else
            {
                clr[i] = temp1;
            }
        }

        return new(clr[0], clr[1], clr[2], alpha);
    }

    /// <summary>Creates a random fully saturated middle-luminance color.</summary>
    /// <returns>A random-hue color.</returns>
    public static Color RandomHue()
    {
        var hue = (float)Generate.RandomNumber();
        const float saturation = 1;
        const float luminosity = 0.5F;
        return FromHSL(hue, saturation, luminosity);
    }

    /// <summary>Interpolates between two colors.</summary>
    /// <param name="c1">The first color.</param>
    /// <param name="c2">The second color.</param>
    /// <param name="fraction">The interpolation fraction.</param>
    /// <returns>The interpolated color.</returns>
    public static Color InterpolateRgb(Color c1, Color c2, double fraction) => new(
        InterpolateRgb(c1.R, c2.R, fraction),
        InterpolateRgb(c1.G, c2.G, fraction),
        InterpolateRgb(c1.B, c2.B, fraction),
        InterpolateRgb(c1.A, c2.A, fraction));

    /// <summary>Creates an array of RGB interpolation steps.</summary>
    /// <param name="c1">The first color.</param>
    /// <param name="c2">The second color.</param>
    /// <param name="steps">The number of steps.</param>
    /// <returns>The interpolated colors.</returns>
    public static Color[] InterpolateRgbArray(Color c1, Color c2, int steps)
    {
        var stepFactor = 1.0 / (steps - 1);
        var array = new Color[steps];
        for (var i = 0; i < steps; ++i)
        {
            array[i] = InterpolateRgb(c1, c2, stepFactor * i);
        }

        return array;
    }

    /// <summary>Determines whether two colors have the same ARGB value.</summary>
    /// <param name="a">The first color.</param>
    /// <param name="b">The second color.</param>
    /// <returns><see langword="true"/> when the colors are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Color a, Color b) => a.ARGB == b.ARGB;

    /// <summary>Determines whether two colors differ.</summary>
    /// <param name="a">The first color.</param>
    /// <param name="b">The second color.</param>
    /// <returns><see langword="true"/> when the colors differ; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Color a, Color b) => a.ARGB != b.ARGB;

    /// <summary>Interpolates this color with another color.</summary>
    /// <param name="c1">The first color.</param>
    /// <param name="fraction">The interpolation fraction.</param>
    /// <returns>The interpolated color.</returns>
    public Color InterpolateRgb(Color c1, double fraction) => InterpolateRgb(this, c1, fraction);

    /// <summary>Creates an array of RGB interpolation steps between this color and another color.</summary>
    /// <param name="c1">The other color.</param>
    /// <param name="steps">The number of steps.</param>
    /// <returns>The interpolated colors.</returns>
    public Color[] InterpolateArrayRgb(Color c1, int steps) => InterpolateRgbArray(this, c1, steps);

    /// <inheritdoc/>
    public override string ToString() => $"Color {ToHex()} (R={R}, G={G}, B={B}, A={A})";

    /// <inheritdoc/>
    public override int GetHashCode() => (int)ARGB;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Color color && Equals(color);

    /// <summary>Determines whether this color equals another color.</summary>
    /// <param name="other">The color to compare.</param>
    /// <returns><see langword="true"/> when the colors are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Color other) => other.ARGB == ARGB;

    /// <summary>Creates a copy with a replacement red component.</summary>
    /// <param name="red">The red component.</param>
    /// <returns>The updated color.</returns>
    public Color WithRed(byte red) => new(red, Green, Blue, Alpha);

    /// <summary>Creates a copy with a replacement green component.</summary>
    /// <param name="green">The green component.</param>
    /// <returns>The updated color.</returns>
    public Color WithGreen(byte green) => new(Red, green, Blue, Alpha);

    /// <summary>Creates a copy with a replacement blue component.</summary>
    /// <param name="blue">The blue component.</param>
    /// <returns>The updated color.</returns>
    public Color WithBlue(byte blue) => new(Red, Green, blue, Alpha);

    /// <summary>Creates a copy with a replacement alpha component.</summary>
    /// <param name="alpha">The alpha component.</param>
    /// <returns>The updated color.</returns>
    public Color WithAlpha(byte alpha)
    {
        // If requesting a semitransparent black, make it slightly non-black
        // to prevent SVG export from rendering the color as opaque.
        return Red == 0 && Green == 0 && Blue == 0 && alpha < ByteMaximum ? new(1, 1, 1, alpha) : new(Red, Green, Blue, alpha);
    }

    /// <summary>Creates a copy with a replacement fractional alpha component.</summary>
    /// <param name="alpha">The alpha fraction from 0 to 1.</param>
    /// <returns>The updated color.</returns>
    public Color WithAlpha(double alpha) => WithAlpha((byte)(alpha * ByteMaximum));

    /// <summary>Creates a copy with a replacement opacity.</summary>
    /// <param name="opacity">The opacity fraction from 0 to 1.</param>
    /// <returns>The updated color.</returns>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional opacity parameter is retained for compatibility with the existing public API.")]
    public Color WithOpacity(double opacity = .5) => WithAlpha((byte)(opacity * ByteMaximum));

    /// <summary>Creates a copy with replacement luminance.</summary>
    /// <param name="lightness">The luminance fraction from 0 to 1.</param>
    /// <returns>The updated color.</returns>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional lightness parameter is retained for compatibility with the existing public API.")]
    public Color WithLightness(float lightness = .5F)
    {
        (var h, var s, _) = ToHSL();
        return FromHSL(h, s, Math.Min(Math.Max(lightness, 0F), 1F));
    }

    /// <summary>Lightens the color.</summary>
    /// <param name="fraction">The fraction to lighten.</param>
    /// <returns>The lightened color.</returns>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional fraction parameter is retained for compatibility with the existing public API.")]
    public Color Lighten(double fraction = .5F)
    {
        if (fraction < 0)
        {
            return Darken(-fraction);
        }

        fraction = Math.Min(1F, fraction);

        static byte LightenByte(byte color, double amount)
        {
            var result = color + ((ByteMaximum - color) * amount);
            return (byte)Math.Min(Math.Max(result, 0), ByteMaximum);
        }

        var r = LightenByte(R, fraction);
        var g = LightenByte(G, fraction);
        var b = LightenByte(B, fraction);
        return new(r, g, b, Alpha);
    }

    /// <summary>Darkens the color.</summary>
    /// <param name="fraction">The fraction to darken.</param>
    /// <returns>The darkened color.</returns>
    [SuppressMessage("Usage", "SST2309", Justification = "The optional fraction parameter is retained for compatibility with the existing public API.")]
    public Color Darken(double fraction = .5F)
    {
        if (fraction < 0)
        {
            return Lighten(-fraction);
        }

        fraction = Math.Max(0F, 1F - fraction);

        static byte DarkenByte(byte color, double amount)
        {
            var result = color * amount;
            return (byte)Math.Min(Math.Max(result, 0), ByteMaximum);
        }

        var r = DarkenByte(R, fraction);
        var g = DarkenByte(G, fraction);
        var b = DarkenByte(B, fraction);
        return new(r, g, b, Alpha);
    }

    /// <summary>Creates a copy with inverted RGB components.</summary>
    /// <returns>The inverted color.</returns>
    public Color Inverted() => new(
        (byte)(ByteMaximum - R),
        (byte)(ByteMaximum - G),
        (byte)(ByteMaximum - B));

    /// <summary>Creates a copy with the hue rotated by half a cycle.</summary>
    /// <returns>The hue-inverted color.</returns>
    public Color InvertedHue() => FromHSL(Hue + FractionHalf, Saturation, Luminance);

    /// <summary>Mixes this color with another color.</summary>
    /// <param name="otherColor">The color to mix.</param>
    /// <param name="fraction">The fraction of <paramref name="otherColor"/> to use.</param>
    /// <returns>The mixed color.</returns>
    public Color MixedWith(Color otherColor, double fraction) => InterpolateRgb(otherColor, fraction);

    /// <summary>Returns this color formatted as hexadecimal RGB or RGBA text.</summary>
    /// <returns>A string such as "#6699AA" or "#6699AA42".</returns>
    public string ToHex() => Alpha == ByteMaximum
        ? $"#{R:X2}{G:X2}{B:X2}"
        : $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    /// <summary>Returns this color formatted as hexadecimal RGB text.</summary>
    /// <returns>A string such as "#6699AA".</returns>
    public string ToStringRGB() => $"#{R:X2}{G:X2}{B:X2}";

    /// <summary>Returns this color formatted as hexadecimal RGBA text.</summary>
    /// <returns>A string such as "#6699AAFF".</returns>
    public string ToStringRGBA() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    /// <summary>Creates a SkiaSharp color.</summary>
    /// <returns>The SkiaSharp color.</returns>
    public SKColor ToSKColor() => new(Red, Green, Blue, Alpha);

    /// <summary>Creates a System.Drawing color.</summary>
    /// <returns>The System.Drawing color.</returns>
    [SuppressMessage("Style", "SST2275", Justification = "A block body documents that the conversion API constructs a System.Drawing value.")]
    public System.Drawing.Color ToSDColor()
    {
        return System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
    }

    /// <summary>Converts hue, saturation, and luminance to this color representation.</summary>
    /// <returns>The hue, saturation, and luminance fractions.</returns>
    public (float h, float s, float l) ToHSL()
    {
        // adapted from http://en.wikipedia.org/wiki/HSL_color_space
        var r = Red / (float)ByteMaximum;
        var g = Green / (float)ByteMaximum;
        var b = Blue / (float)ByteMaximum;

        var max = Math.Max(Math.Max(r, g), b);
        var min = Math.Min(Math.Min(r, g), b);

        float h;
        float s;
        var l = (min + max) / FractionTwo;
        if (l <= 0F)
        {
            return (0F, 0F, 0F);
        }

        var delta = max - min;
        s = delta;
        if (s <= 0F)
        {
            return (0F, 0F, l);
        }

        s = l > FractionHalf ? delta / (FractionTwo - delta) : delta / (max + min);

        h = (r > g && r > b, g > b) switch
        {
            (true, _) => ((g - b) / delta) + (g < b ? FractionSix : 0F),
            (false, true) => ((b - r) / delta) + FractionTwo,
            _ => ((r - g) / delta) + FractionFour,
        };

        h /= FractionSix;
        return (h, s, l);
    }

    /// <summary>Converts a hexadecimal color string to an ARGB value.</summary>
    /// <param name="hex">The hexadecimal color string.</param>
    /// <returns>The ARGB value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2204",
        Justification = "Substring is required to support the project's .NET Framework targets, which do not provide System.Range.")]
    private static uint Hex2Argb(string hex)
    {
        uint rgba = 0;

        if (hex[0] == '#')
        {
            hex = hex.Substring(1);
        }

        if (hex.Length == 6)
        {
            hex += "FF";
        }

        if (uint.TryParse(hex, NumberStyles.HexNumber, null, out var parsedValue))
        {
            rgba = parsedValue;
        }

        return ((rgba & 0xFF) << 24) | (rgba >> 8);
    }

    /// <summary>Interpolates one color channel between two byte values.</summary>
    /// <param name="b1">The first channel value.</param>
    /// <param name="b2">The second channel value.</param>
    /// <param name="fraction">The interpolation fraction.</param>
    /// <returns>The interpolated channel value.</returns>
    private static byte InterpolateRgb(byte b1, byte b2, double fraction) => b1 < b2
        ? Math.Min(Math.Max((byte)(b1 + ((b2 - b1) * fraction)), (byte)0), (byte)ByteMaximum)
        : Math.Min(Math.Max((byte)(b2 + ((b1 - b2) * (1D - fraction))), (byte)0), (byte)ByteMaximum);
}
