// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Adds an extension for <see cref="Color"/> that allows manipulation with HSL and HSV color spaces.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Maximum <see cref="byte"/> size with the current <see cref="float"/> precision.
    /// </summary>
    private const float _byteMax = (float)byte.MaxValue;

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
    public static SolidColorBrush ToBrush(this Color color, double opacity) => new() { Color = color, Opacity = opacity };

    /// <summary>
    /// Gets <see cref="Color"/> luminance based on HSL space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>A double.</returns>
    public static double GetLuminance(this Color color)
    {
        (var _, var _, var luminance) = color.ToHsl();

        return (double)luminance;
    }

    /// <summary>
    /// Gets <see cref="Color"/> brightness based on HSV space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>A double.</returns>
    public static double GetBrightness(this Color color)
    {
        (var _, var _, var brightness) = color.ToHsv();

        return (double)brightness;
    }

    /// <summary>
    /// Gets <see cref="Color"/> hue based on HSV space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>A double.</returns>
    public static double GetHue(this Color color)
    {
        (var hue, var _, var _) = color.ToHsv();

        return (double)hue;
    }

    /// <summary>
    /// Gets <see cref="Color"/> saturation based on HSV space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <returns>A double.</returns>
    public static double GetSaturation(this Color color)
    {
        (var _, var saturation, var _) = color.ToHsv();

        return (double)saturation;
    }

    /// <summary>
    /// Allows to change the luminance by a factor based on the HSL color space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="factor">The value of the luminance change factor from <see langword="100"/> to <see langword="-100"/>.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color UpdateLuminance(this Color color, float factor)
    {
        if (factor > 100f || factor < -100f)
        {
            throw new ArgumentOutOfRangeException(nameof(factor));
        }

        var (hue, saturation, rawLuminance) = color.ToHsl();

        var (red, green, blue) = FromHslToRgb(hue, saturation, ToPercentage(rawLuminance + factor));

        return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
    }

    /// <summary>
    /// Allows to change the saturation by a factor based on the HSL color space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="factor">The value of the saturation change factor from <see langword="100"/> to <see langword="-100"/>.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color UpdateSaturation(this Color color, float factor)
    {
        if (factor > 100f || factor < -100f)
        {
            throw new ArgumentOutOfRangeException(nameof(factor));
        }

        var (hue, rawSaturation, brightness) = color.ToHsl();

        var (red, green, blue) = FromHslToRgb(hue, ToPercentage(rawSaturation + factor), brightness);

        return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
    }

    /// <summary>
    /// Allows to change the brightness by a factor based on the HSV color space.
    /// </summary>
    /// <param name="color">Input color.</param>
    /// <param name="factor">The value of the brightness change factor from <see langword="100"/> to <see langword="-100"/>.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color UpdateBrightness(this Color color, float factor)
    {
        if (factor > 100f || factor < -100f)
        {
            throw new ArgumentOutOfRangeException(nameof(factor));
        }

        var (hue, saturation, rawBrightness) = color.ToHsv();

        var (red, green, blue) = FromHsvToRgb(hue, saturation, ToPercentage(rawBrightness + factor));

        return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
    }

    /// <summary>
    /// Allows to change the brightness, saturation and luminance by a factors based on the HSL and HSV color space.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    /// <param name="brightnessFactor">The value of the brightness change factor from <see langword="100"/> to <see langword="-100"/>.</param>
    /// <param name="saturationFactor">The value of the saturation change factor from <see langword="100"/> to <see langword="-100"/>.</param>
    /// <param name="luminanceFactor">The value of the luminance change factor from <see langword="100"/> to <see langword="-100"/>.</param>
    /// <returns>Updated <see cref="Color"/>.</returns>
    public static Color Update(
        this Color color,
        float brightnessFactor,
        float saturationFactor = 0,
        float luminanceFactor = 0)
    {
        if (brightnessFactor > 100f || brightnessFactor < -100f)
        {
            throw new ArgumentOutOfRangeException(nameof(brightnessFactor));
        }

        if (saturationFactor > 100f || saturationFactor < -100f)
        {
            throw new ArgumentOutOfRangeException(nameof(saturationFactor));
        }

        if (luminanceFactor > 100f || luminanceFactor < -100f)
        {
            throw new ArgumentOutOfRangeException(nameof(luminanceFactor));
        }

        var (hue, rawSaturation, rawBrightness) = color.ToHsv();

        var (red, green, blue) = FromHsvToRgb(
            hue,
            ToPercentage(rawSaturation + saturationFactor),
            ToPercentage(rawBrightness + brightnessFactor));

        if (luminanceFactor == 0)
        {
            return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
        }

        (hue, var saturation, var rawLuminance) = Color
            .FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue))
            .ToHsl();

        (red, green, blue) = FromHslToRgb(hue, saturation, ToPercentage(rawLuminance + luminanceFactor));

        return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
    }

    /// <summary>
    /// HSL representation models the way different paints mix together to create colour in the real world,
    /// with the lightness dimension resembling the varying amounts of black or white paint in the mixture.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>
    ///   <see langword="float" /> hue, <see langword="float" /> saturation, <see langword="float" /> lightness.
    /// </returns>
    public static (float Hue, float Saturation, float Lightness) ToHsl(this Color color)
    {
        int red = color.R;
        int green = color.G;
        int blue = color.B;

        var max = Math.Max(red, Math.Max(green, blue));
        var min = Math.Min(red, Math.Min(green, blue));

        var fDelta = (max - min) / _byteMax;

        float hue;
        float saturation;
        float lightness;

        if (max <= 0)
        {
            return (0f, 0f, 0f);
        }

        saturation = 0.0f;
        lightness = (max + min) / _byteMax / 2.0f;

        if (fDelta <= 0.0)
        {
            return (0f, saturation * 100f, lightness * 100f);
        }

        saturation = fDelta / (max / _byteMax);

        if (max == red)
        {
            hue = (green - blue) / _byteMax / fDelta;
        }
        else if (max == green)
        {
            hue = 2f + ((blue - red) / _byteMax / fDelta);
        }
        else
        {
            hue = 4f + ((red - green) / _byteMax / fDelta);
        }

        if (hue < 0)
        {
            hue += 360;
        }

        return (hue * 60f, saturation * 100f, lightness * 100f);
    }

    /// <summary>
    /// HSV representation models how colors appear under light.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>
    ///   <see langword="float" /> hue, <see langword="float" /> saturation, <see langword="float" /> brightness.
    /// </returns>
    public static (float Hue, float Saturation, float Value) ToHsv(this Color color)
    {
        int red = color.R;
        int green = color.G;
        int blue = color.B;

        var max = Math.Max(red, Math.Max(green, blue));
        var min = Math.Min(red, Math.Min(green, blue));

        var fDelta = (max - min) / _byteMax;

        float hue;
        float saturation;
        float value;

        if (max <= 0)
        {
            return (0f, 0f, 0f);
        }

        saturation = fDelta / (max / _byteMax);
        value = max / _byteMax;

        if (fDelta <= 0.0)
        {
            return (0f, saturation * 100f, value * 100f);
        }

        if (max == red)
        {
            hue = (green - blue) / _byteMax / fDelta;
        }
        else if (max == green)
        {
            hue = 2f + ((blue - red) / _byteMax / fDelta);
        }
        else
        {
            hue = 4f + ((red - green) / _byteMax / fDelta);
        }

        if (hue < 0)
        {
            hue += 360;
        }

        return (hue * 60f, saturation * 100f, value * 100f);
    }

    /// <summary>
    /// Converts the color values stored as HSL to RGB.
    /// </summary>
    /// <param name="hue">The hue.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="lightness">The lightness.</param>
    /// <returns>
    /// RGB value.
    /// </returns>
    public static (int R, int G, int B) FromHslToRgb(float hue, float saturation, float lightness)
    {
        if (AlmostEquals(saturation, 0, 0.01f))
        {
            var color = (int)(lightness * _byteMax);

            return (color, color, color);
        }

        lightness /= 100f;
        saturation /= 100f;

        var hueAngle = hue / 360f;

        return (
            CalcHslChannel(hueAngle + 0.333333333f, saturation, lightness),
            CalcHslChannel(hueAngle, saturation, lightness),
            CalcHslChannel(hueAngle - 0.333333333f, saturation, lightness));
    }

    /// <summary>
    /// Converts the color values stored as HSV (HSB) to RGB.
    /// </summary>
    /// <param name="hue">The hue.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="brightness">The brightness.</param>
    /// <returns>
    /// A RGB Value.
    /// </returns>
    public static (int R, int G, int B) FromHsvToRgb(float hue, float saturation, float brightness)
    {
        var red = 0;
        var green = 0;
        var blue = 0;

        if (AlmostEquals(saturation, 0, 0.01f))
        {
            red = green = blue = (int)(((brightness / 100f) * _byteMax) + 0.5f);

            return (red, green, blue);
        }

        hue /= 360f;
        brightness /= 100f;
        saturation /= 100f;

        var hueAngle = (hue - (float)Math.Floor(hue)) * 6.0f;
        var f = hueAngle - (float)Math.Floor(hueAngle);

        var p = brightness * (1.0f - saturation);
        var q = brightness * (1.0f - (saturation * f));
        var t = brightness * (1.0f - (saturation * (1.0f - f)));

        switch ((int)hueAngle)
        {
            case 0:
                red = (int)((brightness * 255.0f) + 0.5f);
                green = (int)((t * 255.0f) + 0.5f);
                blue = (int)((p * 255.0f) + 0.5f);

                break;
            case 1:
                red = (int)((q * 255.0f) + 0.5f);
                green = (int)((brightness * 255.0f) + 0.5f);
                blue = (int)((p * 255.0f) + 0.5f);

                break;
            case 2:
                red = (int)((p * 255.0f) + 0.5f);
                green = (int)((brightness * 255.0f) + 0.5f);
                blue = (int)((t * 255.0f) + 0.5f);

                break;
            case 3:
                red = (int)((p * 255.0f) + 0.5f);
                green = (int)((q * 255.0f) + 0.5f);
                blue = (int)((brightness * 255.0f) + 0.5f);

                break;
            case 4:
                red = (int)((t * 255.0f) + 0.5f);
                green = (int)((p * 255.0f) + 0.5f);
                blue = (int)((brightness * 255.0f) + 0.5f);

                break;
            case 5:
                red = (int)((brightness * 255.0f) + 0.5f);
                green = (int)((p * 255.0f) + 0.5f);
                blue = (int)((q * 255.0f) + 0.5f);

                break;
        }

        return (red, green, blue);
    }

    /// <summary>
    /// Calculates the color component for HSL.
    /// </summary>
    private static int CalcHslChannel(float color, float saturation, float lightness)
    {
        float num1,
            num2;

        if (color > 1)
        {
            color--;
        }

        if (color < 0)
        {
            color++;
        }

        if (lightness < 0.5f)
        {
            num1 = lightness * (1f + saturation);
        }
        else
        {
            num1 = lightness + saturation - (lightness * saturation);
        }

        num2 = (2f * lightness) - num1;

        if (color * 6f < 1)
        {
            return (int)((num2 + ((num1 - num2) * 6f * color)) * _byteMax);
        }

        if (color * 2f < 1)
        {
            return (int)(num1 * _byteMax);
        }

        if (color * 3f < 2)
        {
            return (int)((num2 + ((num1 - num2) * (0.666666666f - color) * 6f)) * _byteMax);
        }

        return (int)(num2 * _byteMax);
    }

    /// <summary>
    /// Whether the floating point number is about the same.
    /// </summary>
    private static bool AlmostEquals(float numberOne, float numberTwo, float precision = 0)
    {
        if (precision <= 0)
        {
            precision = float.Epsilon;
        }

        return numberOne >= (numberTwo - precision) && numberOne <= (numberTwo + precision);
    }

    /// <summary>
    /// Absolute percentage.
    /// </summary>
    private static float ToPercentage(float value) => value switch
    {
        > 100f => 100f,
        < 0f => 0f,
        _ => value
    };

    /// <summary>
    /// Absolute byte.
    /// </summary>
    private static byte ToColorByte(int value)
    {
        if (value > byte.MaxValue)
        {
            value = byte.MaxValue;
        }
        else if (value < byte.MinValue)
        {
            value = byte.MinValue;
        }

        return Convert.ToByte(value);
    }
}
