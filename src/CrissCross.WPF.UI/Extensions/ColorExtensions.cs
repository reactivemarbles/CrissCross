// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>Adds an extension for <see cref="Color"/> that allows manipulation with HSL and HSV color spaces.</summary>
public static class ColorExtensions
{
    /// <summary>Maximum <see cref="byte"/> size with the current <see cref="float"/> precision.</summary>
    private const float ByteMax = (float)byte.MaxValue;

    /// <summary>Provides the maximum percentage value.</summary>
    private const float MaximumPercentage = 100F;

    /// <summary>Provides the minimum percentage adjustment.</summary>
    private const float MinimumAdjustmentPercentage = -MaximumPercentage;

    /// <summary>Provides the angle of a complete hue rotation in degrees.</summary>
    private const float FullHueAngle = 360F;

    /// <summary>Provides the angle of one hue sector in degrees.</summary>
    private const float HueSectorAngle = 60F;

    /// <summary>Provides the number of hue sectors.</summary>
    private const float HueSectorCount = 6F;

    /// <summary>Provides the green hue-sector offset.</summary>
    private const float GreenHueOffset = 2F;

    /// <summary>Provides the blue hue-sector offset.</summary>
    private const float BlueHueOffset = 4F;

    /// <summary>Provides one third of a normalized hue rotation.</summary>
    private const float OneThirdHue = 1F / 3F;

    /// <summary>Provides two thirds of a normalized hue rotation.</summary>
    private const float TwoThirdsHue = 2F / 3F;

    /// <summary>Provides the divisor used to average minimum and maximum channels.</summary>
    private const float LightnessAverageDivisor = 2F;

    /// <summary>Provides the midpoint of a normalized channel.</summary>
    private const float NormalizedMidpoint = 0.5F;

    /// <summary>Provides the tolerance used to compare color-channel values.</summary>
    private const float ColorComparisonTolerance = 0.01F;

    /// <summary>Provides the offset used to round normalized channels to byte values.</summary>
    private const float ChannelRoundingOffset = 0.5F;

    /// <summary>Provides the scale used to test the upper HSL segment.</summary>
    private const float HslUpperSegmentScale = 3F;

    /// <summary>Provides the boundary used to test the upper HSL segment.</summary>
    private const float HslUpperSegmentBoundary = 2F;

    /// <summary>Provides the blue HSV sector index.</summary>
    private const int BlueHueSector = 2;

    /// <summary>Provides the magenta HSV sector index.</summary>
    private const int MagentaHueSector = 3;

    /// <summary>Provides the red HSV sector index.</summary>
    private const int RedHueSector = 4;

    /// <summary>Provides the yellow HSV sector index.</summary>
    private const int YellowHueSector = 5;

    /// <summary>Provides extension members.</summary>
    /// <param name="color">The color value.</param>
    extension(Color color)
    {
        /// <summary>Creates a <see cref="SolidColorBrush"/> from a <see cref="Color"/>.</summary>
        /// <returns>Brush converted to color.</returns>
        public SolidColorBrush ToBrush() => new(color);

        /// <summary>Creates a SolidColorBrush from a Color with defined brush opacity.</summary>
        /// <param name="opacity">Degree of opacity.</param>
        /// <returns>Brush converted to color with modified opacity.</returns>
        public SolidColorBrush ToBrush(double opacity) => new() { Color = color, Opacity = opacity };

        /// <summary>Gets <see cref="Color"/> luminance based on HSL space.</summary>
        /// <returns>A double.</returns>
        public double GetLuminance()
        {
            (var _, var _, var luminance) = color.ToHsl();

            return (double)luminance;
        }

        /// <summary>Gets <see cref="Color"/> brightness based on HSV space.</summary>
        /// <returns>A double.</returns>
        public double GetBrightness()
        {
            (var _, var _, var brightness) = color.ToHsv();

            return (double)brightness;
        }

        /// <summary>Gets <see cref="Color"/> hue based on HSV space.</summary>
        /// <returns>A double.</returns>
        public double GetHue()
        {
            (var hue, var _, var _) = color.ToHsv();

            return (double)hue;
        }

        /// <summary>Gets <see cref="Color"/> saturation based on HSV space.</summary>
        /// <returns>A double.</returns>
        public double GetSaturation()
        {
            (var _, var saturation, var _) = color.ToHsv();

            return (double)saturation;
        }

        /// <summary>Allows to change the luminance by a factor based on the HSL color space.</summary>
        /// <param name="factor">The value of the luminance change factor from <see langword="100"/> to <see
        /// langword="-100"/>.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color UpdateLuminance(float factor)
        {
            if (factor > MaximumPercentage || factor < MinimumAdjustmentPercentage)
            {
                throw new ArgumentOutOfRangeException(nameof(factor));
            }

            var (hue, saturation, rawLuminance) = color.ToHsl();

            var (red, green, blue) = FromHslToRgb(hue, saturation, ToPercentage(rawLuminance + factor));

            return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
        }

        /// <summary>Allows to change the saturation by a factor based on the HSL color space.</summary>
        /// <param name="factor">The value of the saturation change factor from <see langword="100"/> to <see
        /// langword="-100"/>.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color UpdateSaturation(float factor)
        {
            if (factor > MaximumPercentage || factor < MinimumAdjustmentPercentage)
            {
                throw new ArgumentOutOfRangeException(nameof(factor));
            }

            var (hue, rawSaturation, brightness) = color.ToHsl();

            var (red, green, blue) = FromHslToRgb(hue, ToPercentage(rawSaturation + factor), brightness);

            return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
        }

        /// <summary>Allows to change the brightness by a factor based on the HSV color space.</summary>
        /// <param name="factor">The value of the brightness change factor from <see langword="100"/> to <see
        /// langword="-100"/>.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color UpdateBrightness(float factor)
        {
            if (factor > MaximumPercentage || factor < MinimumAdjustmentPercentage)
            {
                throw new ArgumentOutOfRangeException(nameof(factor));
            }

            var (hue, saturation, rawBrightness) = color.ToHsv();

            var (red, green, blue) = FromHsvToRgb(hue, saturation, ToPercentage(rawBrightness + factor));

            return Color.FromArgb(color.A, ToColorByte(red), ToColorByte(green), ToColorByte(blue));
        }

        /// <summary>Provides the Update member.</summary>
        /// <param name="brightnessFactor">The value of the brightness change factor from <see langword="100"/> to <see
        /// langword="-100"/>.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color Update(float brightnessFactor) => color.Update(brightnessFactor, 0, 0);

        /// <summary>Updates the color brightness and saturation.</summary>
        /// <param name="brightnessFactor">The brightness change factor.</param>
        /// <param name="saturationFactor">The saturation change factor.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color Update(float brightnessFactor, float saturationFactor) =>
            color.Update(brightnessFactor, saturationFactor, 0);

        /// <summary>Updates the color brightness, saturation, and luminance.</summary>
        /// <param name="brightnessFactor">The brightness change factor.</param>
        /// <param name="saturationFactor">The saturation change factor.</param>
        /// <param name="luminanceFactor">The luminance change factor.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color Update(float brightnessFactor, float saturationFactor, float luminanceFactor)
        {
            if (brightnessFactor > MaximumPercentage || brightnessFactor < MinimumAdjustmentPercentage)
            {
                throw new ArgumentOutOfRangeException(nameof(brightnessFactor));
            }

            if (saturationFactor > MaximumPercentage || saturationFactor < MinimumAdjustmentPercentage)
            {
                throw new ArgumentOutOfRangeException(nameof(saturationFactor));
            }

            if (luminanceFactor > MaximumPercentage || luminanceFactor < MinimumAdjustmentPercentage)
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
        /// <returns>
        ///   <see langword="float" /> hue, <see langword="float" /> saturation, <see langword="float" /> lightness.
        /// </returns>
        public (float Hue, float Saturation, float Lightness) ToHsl()
        {
            int red = color.R;
            int green = color.G;
            int blue = color.B;

            var max = Math.Max(red, Math.Max(green, blue));
            var min = Math.Min(red, Math.Min(green, blue));

            var delta = (max - min) / ByteMax;

            float hue;
            float saturation;
            float lightness;

            if (max <= 0)
            {
                return (0F, 0F, 0F);
            }

            saturation = 0.0F;
            lightness = (max + min) / ByteMax / LightnessAverageDivisor;

            if (delta <= 0.0)
            {
                return (0F, saturation * MaximumPercentage, lightness * MaximumPercentage);
            }

            saturation = delta / (max / ByteMax);

            hue = max switch
            {
                _ when max == red => (green - blue) / ByteMax / delta,
                _ when max == green => GreenHueOffset + ((blue - red) / ByteMax / delta),
                _ => BlueHueOffset + ((red - green) / ByteMax / delta),
            };

            if (hue < 0)
            {
                hue += FullHueAngle;
            }

            return (hue * HueSectorAngle, saturation * MaximumPercentage, lightness * MaximumPercentage);
        }

        /// <summary>HSV representation models how colors appear under light.</summary>
        /// <returns>
        ///   <see langword="float" /> hue, <see langword="float" /> saturation, <see langword="float" /> brightness.
        /// </returns>
        public (float Hue, float Saturation, float Value) ToHsv()
        {
            int red = color.R;
            int green = color.G;
            int blue = color.B;

            var max = Math.Max(red, Math.Max(green, blue));
            var min = Math.Min(red, Math.Min(green, blue));

            var delta = (max - min) / ByteMax;

            float hue;
            float saturation;
            float value;

            if (max <= 0)
            {
                return (0F, 0F, 0F);
            }

            saturation = delta / (max / ByteMax);
            value = max / ByteMax;

            if (delta <= 0.0)
            {
                return (0F, saturation * MaximumPercentage, value * MaximumPercentage);
            }

            hue = max switch
            {
                _ when max == red => (green - blue) / ByteMax / delta,
                _ when max == green => GreenHueOffset + ((blue - red) / ByteMax / delta),
                _ => BlueHueOffset + ((red - green) / ByteMax / delta),
            };

            if (hue < 0)
            {
                hue += FullHueAngle;
            }

            return (hue * HueSectorAngle, saturation * MaximumPercentage, value * MaximumPercentage);
        }
    }

    /// <summary>Converts the color values stored as HSL to RGB.</summary>
    /// <param name="hue">The hue.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="lightness">The lightness.</param>
    /// <returns>
    /// RGB value.
    /// </returns>
    public static (int R, int G, int B) FromHslToRgb(float hue, float saturation, float lightness)
    {
        if (AlmostEquals(saturation, 0, ColorComparisonTolerance))
        {
            var color = (int)(lightness * ByteMax);

            return (color, color, color);
        }

        lightness /= MaximumPercentage;
        saturation /= MaximumPercentage;

        var hueAngle = hue / FullHueAngle;

        return (
            CalcHslChannel(hueAngle + OneThirdHue, saturation, lightness),
            CalcHslChannel(hueAngle, saturation, lightness),
            CalcHslChannel(hueAngle - OneThirdHue, saturation, lightness));
    }

    /// <summary>Converts the color values stored as HSV (HSB) to RGB.</summary>
    /// <param name="hue">The hue.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="brightness">The brightness.</param>
    /// <returns>
    /// A RGB Value.
    /// </returns>
    public static (int R, int G, int B) FromHsvToRgb(float hue, float saturation, float brightness)
    {
        if (AlmostEquals(saturation, 0, ColorComparisonTolerance))
        {
            var channel = ToHsvChannel(brightness / MaximumPercentage);

            return (channel, channel, channel);
        }

        hue /= FullHueAngle;
        brightness /= MaximumPercentage;
        saturation /= MaximumPercentage;

        var hueAngle = (hue - (float)Math.Floor(hue)) * HueSectorCount;
        var f = hueAngle - (float)Math.Floor(hueAngle);

        var p = brightness * (1.0F - saturation);
        var q = brightness * (1.0F - (saturation * f));
        var t = brightness * (1.0F - (saturation * (1.0F - f)));

        return (int)hueAngle switch
        {
            0 => ToHsvRgb(brightness, t, p),
            1 => ToHsvRgb(q, brightness, p),
            BlueHueSector => ToHsvRgb(p, brightness, t),
            MagentaHueSector => ToHsvRgb(p, q, brightness),
            RedHueSector => ToHsvRgb(t, p, brightness),
            YellowHueSector => ToHsvRgb(brightness, p, q),
            _ => (0, 0, 0),
        };
    }

    /// <summary>Converts normalized HSV channels to RGB bytes.</summary>
    /// <param name="red">The red channel.</param>
    /// <param name="green">The green channel.</param>
    /// <param name="blue">The blue channel.</param>
    /// <returns>The RGB channels.</returns>
    private static (int R, int G, int B) ToHsvRgb(float red, float green, float blue) =>
        (ToHsvChannel(red), ToHsvChannel(green), ToHsvChannel(blue));

    /// <summary>Converts a normalized HSV channel to a byte-range integer.</summary>
    /// <param name="channel">The normalized channel.</param>
    /// <returns>The byte-range channel.</returns>
    private static int ToHsvChannel(float channel) => (int)((channel * ByteMax) + ChannelRoundingOffset);

    /// <summary>Calculates the color component for HSL.</summary>
    /// <param name="color">The color value.</param>
    /// <param name="saturation">The saturation value.</param>
    /// <param name="lightness">The lightness value.</param>
    /// <returns>The result.</returns>
    private static int CalcHslChannel(float color, float saturation, float lightness)
    {
        float num1;
        float num2;

        if (color > 1)
        {
            color--;
        }

        if (color < 0)
        {
            color++;
        }

        num1 =
            lightness < NormalizedMidpoint
                ? lightness * (1F + saturation)
                : lightness + saturation - (lightness * saturation);

        num2 = lightness + lightness - num1;

        if (color * HueSectorCount < 1)
        {
            return (int)((num2 + ((num1 - num2) * HueSectorCount * color)) * ByteMax);
        }

        if (color + color < 1)
        {
            return (int)(num1 * ByteMax);
        }

        return color * HslUpperSegmentScale < HslUpperSegmentBoundary
            ? (int)((num2 + ((num1 - num2) * (TwoThirdsHue - color) * HueSectorCount)) * ByteMax)
            : (int)(num2 * ByteMax);
    }

    /// <summary>Whether the floating point number is about the same.</summary>
    /// <param name="numberOne">The numberOne value.</param>
    /// <param name="numberTwo">The numberTwo value.</param>
    /// <param name="precision">The precision value.</param>
    /// <returns>The result.</returns>
    private static bool AlmostEquals(float numberOne, float numberTwo, float precision = 0)
    {
        if (precision <= 0)
        {
            precision = float.Epsilon;
        }

        return numberOne >= (numberTwo - precision) && numberOne <= (numberTwo + precision);
    }

    /// <summary>Absolute percentage.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private static float ToPercentage(float value) =>
        value switch
        {
            > MaximumPercentage => MaximumPercentage,
            < 0F => 0F,
            _ => value,
        };

    /// <summary>Absolute byte.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
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
