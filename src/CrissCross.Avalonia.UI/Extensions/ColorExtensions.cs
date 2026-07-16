// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>Adds an extension for <see cref="Color"/> that allows manipulation with HSL and HSV color spaces.</summary>
public static class ColorExtensions
{
    /// <summary>Provides the PercentFactorMaximum member.</summary>
    private const float PercentFactorMaximum = 100F;

    /// <summary>Provides the PercentFactorMinimum member.</summary>
    private const float PercentFactorMinimum = -100F;

    /// <summary>Provides the PercentDivisor member.</summary>
    private const double PercentDivisor = 100.0;

    /// <summary>Provides extension members for <see cref="Color"/>.</summary>
    /// <param name="color">The input color.</param>
    extension(Color color)
    {
        /// <summary>Creates a <see cref="SolidColorBrush"/> from a <see cref="Color"/>.</summary>
        /// <returns>Brush converted to color.</returns>
        public SolidColorBrush ToBrush() => new(color);

        /// <summary>Creates a SolidColorBrush from a Color with defined brush opacity.</summary>
        /// <param name="opacity">Degree of opacity.</param>
        /// <returns>Brush converted to color with modified opacity.</returns>
        public SolidColorBrush ToBrush(double opacity) => new(color, opacity);

        /// <summary>Gets <see cref="Color"/> luminance based on HSL space.</summary>
        /// <returns>Luminance value.</returns>
        public double GetLuminance()
        {
            var hsl = color.ToHsl();
            return hsl.L;
        }

        /// <summary>Gets <see cref="Color"/> brightness based on HSV space.</summary>
        /// <returns>Brightness value.</returns>
        public double GetBrightness()
        {
            var hsv = color.ToHsv();
            return hsv.V;
        }

        /// <summary>Gets <see cref="Color"/> hue based on HSV space.</summary>
        /// <returns>Hue value.</returns>
        public double GetHue()
        {
            var hsv = color.ToHsv();
            return hsv.H;
        }

        /// <summary>Gets <see cref="Color"/> saturation based on HSV space.</summary>
        /// <returns>Saturation value.</returns>
        public double GetSaturation()
        {
            var hsv = color.ToHsv();
            return hsv.S;
        }

        /// <summary>Allows to change the luminance by a factor based on the HSL color space.</summary>
        /// <param name="factor">The value of the luminance change factor from 100 to -100.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color UpdateLuminance(float factor)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, PercentFactorMaximum);
            ArgumentOutOfRangeException.ThrowIfLessThan(factor, PercentFactorMinimum);

            var hsl = color.ToHsl();
            var newL = Math.Clamp(hsl.L + (factor / PercentDivisor), 0.0, 1.0);
            return HslColor.ToRgb(hsl.H, hsl.S, newL, hsl.A);
        }

        /// <summary>Allows to change the saturation by a factor based on the HSL color space.</summary>
        /// <param name="factor">The value of the saturation change factor from 100 to -100.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color UpdateSaturation(float factor)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, PercentFactorMaximum);
            ArgumentOutOfRangeException.ThrowIfLessThan(factor, PercentFactorMinimum);

            var hsl = color.ToHsl();
            var newS = Math.Clamp(hsl.S + (factor / PercentDivisor), 0.0, 1.0);
            return HslColor.ToRgb(hsl.H, newS, hsl.L, hsl.A);
        }

        /// <summary>Allows to change the brightness by a factor based on the HSV color space.</summary>
        /// <param name="factor">The value of the brightness change factor from 100 to -100.</param>
        /// <returns>Updated <see cref="Color"/>.</returns>
        public Color UpdateBrightness(float factor)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, PercentFactorMaximum);
            ArgumentOutOfRangeException.ThrowIfLessThan(factor, PercentFactorMinimum);

            var hsv = color.ToHsv();
            var newV = Math.Clamp(hsv.V + (factor / PercentDivisor), 0.0, 1.0);
            return HsvColor.ToRgb(hsv.H, hsv.S, newV, hsv.A);
        }

        /// <summary>Converts a Color to a hex string.</summary>
        /// <returns>The hex string representation.</returns>
        public string ToHex() => color.ToString();

        /// <summary>Converts a Color to a hex string without alpha.</summary>
        /// <returns>The hex string representation without alpha.</returns>
        public string ToHexWithoutAlpha() => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    /// <summary>Converts a hex string to a Color.</summary>
    /// <param name="hex">The hex string.</param>
    /// <returns>The color, or null if parsing fails.</returns>
    public static Color? FromHex(string hex)
    {
        return Color.TryParse(hex, out var color) ? color : null;
    }
}
