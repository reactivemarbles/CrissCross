// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI;

/// <summary>
/// Helper class for color space conversions.
/// </summary>
internal static class ColorSpaceHelper
{
    /// <summary>
    /// Converts RGB to HSV, returns -1 for undefined channels.
    /// </summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Value (0-1).</returns>
    public static (double H, double S, double V) RgbToHsv(double r, double g, double b)
    {
        double min, max, delta;
        double h, s, v;

        min = Math.Min(r, Math.Min(g, b));
        max = Math.Max(r, Math.Max(g, b));
        v = max;
        delta = max - min;
        if (max != 0)
        {
            s = delta / max;
        }
        else
        {
            // pure black
            s = -1;
            h = -1;
            return (h, s, v);
        }

        if (r == max)
        {
            // between yellow & magenta
            h = (g - b) / delta;
        }
        else if (g == max)
        {
            // between cyan & yellow
            h = 2 + ((b - r) / delta);
        }
        else
        {
            // between magenta & cyan
            h = 4 + ((r - g) / delta);
        }

        h *= 60;
        if (h < 0)
        {
            h += 360;
        }

        if (double.IsNaN(h))
        {
            // delta == 0, case of pure gray
            h = -1;
        }

        return (h, s, v);
    }

    /// <summary>
    /// Converts RGB to HSL, returns -1 for undefined channels.
    /// </summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Lightness (0-1).</returns>
    public static (double H, double S, double L) RgbToHsl(double r, double g, double b)
    {
        double h, s, l;

        var min = Math.Min(Math.Min(r, g), b);
        var max = Math.Max(Math.Max(r, g), b);
        var delta = max - min;
        l = (max + min) / 2;

        if (max == 0)
        {
            // pure black
            return (-1, -1, 0);
        }

        if (delta == 0)
        {
            // gray
            return (-1, 0, l);
        }

        // magic
        s = l <= 0.5 ? delta / (max + min) : delta / (2 - max - min);

        if (r == max)
        {
            h = (g - b) / 6 / delta;
        }
        else if (g == max)
        {
            h = (1.0f / 3) + ((b - r) / 6 / delta);
        }
        else
        {
            h = (2.0f / 3) + ((r - g) / 6 / delta);
        }

        if (h < 0)
        {
            h++;
        }

        if (h > 1)
        {
            h--;
        }

        h *= 360;

        return (h, s, l);
    }

    /// <summary>
    /// Converts HSV to RGB.
    /// </summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="v">Value, 0-1.</param>
    /// <returns>Values (0-1) in order: R, G, B.</returns>
    public static (double R, double G, double B) HsvToRgb(double h, double s, double v)
    {
        if (s == 0)
        {
            // achromatic (grey)
            return (v, v, v);
        }

        if (h >= 360.0)
        {
            h = 0;
        }

        h /= 60;
        var i = (int)h;
        var f = h - i;
        var p = v * (1 - s);
        var q = v * (1 - (s * f));
        var t = v * (1 - (s * (1 - f)));

        return i switch
        {
            0 => (v, t, p),
            1 => (q, v, p),
            2 => (p, v, t),
            3 => (p, q, v),
            4 => (t, p, v),
            _ => (v, p, q),
        };
    }

    /// <summary>
    /// Converts HSV to HSL.
    /// </summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="v">Value, 0-1.</param>
    /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Lightness (0-1).</returns>
    public static (double H, double S, double L) HsvToHsl(double h, double s, double v)
    {
        var hsl_l = v * (1 - (s / 2));
        double hsl_s;
        if (hsl_l == 0 || hsl_l == 1)
        {
            hsl_s = -1;
        }
        else
        {
            hsl_s = (v - hsl_l) / Math.Min(hsl_l, 1 - hsl_l);
        }

        return (h, hsl_s, hsl_l);
    }

    /// <summary>
    /// Converts HSL to RGB.
    /// </summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="l">Lightness, 0-1.</param>
    /// <returns>Values (0-1) in order: R, G, B.</returns>
    public static (double R, double G, double B) HslToRgb(double h, double s, double l)
    {
        var hueCircleSegment = (int)(h / 60);
        var circleSegmentFraction = (h - (60 * hueCircleSegment)) / 60;

        var maxRGB = l < 0.5 ? l * (1 + s) : l + s - (l * s);
        var minRGB = (2 * l) - maxRGB;
        var delta = maxRGB - minRGB;

        return hueCircleSegment switch
        {
            0 => (maxRGB, (delta * circleSegmentFraction) + minRGB, minRGB), // red-yellow
            1 => ((delta * (1 - circleSegmentFraction)) + minRGB, maxRGB, minRGB), // yellow-green
            2 => (minRGB, maxRGB, (delta * circleSegmentFraction) + minRGB), // green-cyan
            3 => (minRGB, (delta * (1 - circleSegmentFraction)) + minRGB, maxRGB), // cyan-blue
            4 => ((delta * circleSegmentFraction) + minRGB, minRGB, maxRGB), // blue-purple
            _ => (maxRGB, minRGB, (delta * (1 - circleSegmentFraction)) + minRGB), // purple-red and invalid values
        };
    }

    /// <summary>
    /// Converts HSL to HSV.
    /// </summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="l">Lightness, 0-1.</param>
    /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Value (0-1).</returns>
    public static (double H, double S, double V) HslToHsv(double h, double s, double l)
    {
        var hsv_v = l + (s * Math.Min(l, 1 - l));
        double hsv_s;
        if (hsv_v == 0)
        {
            hsv_s = -1;
        }
        else
        {
            hsv_s = 2 * (1 - (l / hsv_v));
        }

        return (h, hsv_s, hsv_v);
    }
}
