// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>
/// Adds an extension for <see cref="Color"/> that allows manipulation with HSL and HSV color spaces.
/// </summary>
public static class ColorExtensions
{
    private const float ByteMax = (float)byte.MaxValue;

    /// <summary>
    /// Creates a <see cref="SolidColorBrush"/> from a <see cref="Color"/>.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>Brush converted to color.</returns>
    public static SolidColorBrush ToBrush(this Color color) => new(color);

    /// <summary>
    /// Creates a <see cref="SolidColorBrush"/> from a <see cref="Color"/> with defined brush opacity.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="opacity">Degree of opacity.</param>
    /// <returns>Brush converted to color with modified opacity.</returns>
    public static SolidColorBrush ToBrush(this Color color, double opacity) => new(color, opacity);

    /// <summary>
    /// Gets <see cref="Color"/> luminance based on HSL space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>Luminance value.</returns>
    public static double GetLuminance(this Color color)
    {
        var hsl = color.ToHsl();
        return hsl.L;
    }

    /// <summary>
    /// Gets <see cref="Color"/> brightness based on HSV space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>Brightness value.</returns>
    public static double GetBrightness(this Color color)
    {
        var hsv = color.ToHsv();
        return hsv.V;
    }

    /// <summary>
    /// Gets <see cref="Color"/> hue based on HSV space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>Hue value.</returns>
    public static double GetHue(this Color color)
    {
        var hsv = color.ToHsv();
        return hsv.H;
    }

    /// <summary>
    /// Gets <see cref="Color"/> saturation based on HSV space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>Saturation value.</returns>
    public static double GetSaturation(this Color color)
    {
        var hsv = color.ToHsv();
        return hsv.S;
    }

    /// <summary>
    /// Allows to change the luminance by a factor based on the HSL color space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="factor">The value of the luminance change factor from 100 to -100.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color UpdateLuminance(this Color color, float factor)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, 100f);
        ArgumentOutOfRangeException.ThrowIfLessThan(factor, -100f);

        var hsl = color.ToHsl();
        var newL = Math.Clamp(hsl.L + (factor / 100.0), 0.0, 1.0);
        return HslColor.ToRgb(hsl.H, hsl.S, newL, hsl.A);
    }

    /// <summary>
    /// Allows to change the saturation by a factor based on the HSL color space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="factor">The value of the saturation change factor from 100 to -100.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color UpdateSaturation(this Color color, float factor)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, 100f);
        ArgumentOutOfRangeException.ThrowIfLessThan(factor, -100f);

        var hsl = color.ToHsl();
        var newS = Math.Clamp(hsl.S + (factor / 100.0), 0.0, 1.0);
        return HslColor.ToRgb(hsl.H, newS, hsl.L, hsl.A);
    }

    /// <summary>
    /// Allows to change the brightness by a factor based on the HSV color space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="factor">The value of the brightness change factor from 100 to -100.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color UpdateBrightness(this Color color, float factor)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, 100f);
        ArgumentOutOfRangeException.ThrowIfLessThan(factor, -100f);

        var hsv = color.ToHsv();
        var newV = Math.Clamp(hsv.V + (factor / 100.0), 0.0, 1.0);
        return HsvColor.ToRgb(hsv.H, hsv.S, newV, hsv.A);
    }

    /// <summary>
    /// Converts a hex string to a Color.
    /// </summary>
    /// <param name="hex">The hex string.</param>
    /// <returns>The color, or null if parsing fails.</returns>
    public static Color? FromHex(string hex)
    {
        if (Color.TryParse(hex, out var color))
        {
            return color;
        }

        return null;
    }

    /// <summary>
    /// Converts a Color to a hex string.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The hex string representation.</returns>
    public static string ToHex(this Color color) => color.ToString();

    /// <summary>
    /// Converts a Color to a hex string without alpha.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The hex string representation without alpha.</returns>
    public static string ToHexWithoutAlpha(this Color color) =>
        $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
