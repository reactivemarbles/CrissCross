// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI;
#else
namespace CrissCross.Avalonia.UI;
#endif

/// <summary>Helper class for color space conversions.</summary>
internal static class ColorSpaceHelper
{
    /// <summary>Tolerance used when comparing normalized color components.</summary>
    private const double ComparisonTolerance = 1E-10D;

    /// <summary>Converts RGB to HSV, returns -1 for undefined channels.</summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Value (0-1).</returns>
    public static (double H, double S, double V) RgbToHsv(double r, double g, double b)
    {
        const double undefinedChannel = -1.0;
        const double rgbToHsvGreenSector = 2.0;
        const double rgbToHsvBlueSector = 4.0;
        const double hueDegreesPerSector = 60.0;
        const double hueDegrees = 360.0;

        double min;
        double max;
        double delta;
        double h;
        double s;
        double v;

        min = Math.Min(r, Math.Min(g, b));
        max = Math.Max(r, Math.Max(g, b));
        v = max;
        delta = max - min;
        if (!AreClose(max, 0D))
        {
            s = delta / max;
        }
        else
        {
            // pure black
            s = undefinedChannel;
            h = undefinedChannel;
            return (h, s, v);
        }

        h = max switch
        {
            _ when AreClose(r, max) => (g - b) / delta,
            _ when AreClose(g, max) => rgbToHsvGreenSector + ((b - r) / delta),
            _ => rgbToHsvBlueSector + ((r - g) / delta),
        };

        h *= hueDegreesPerSector;
        if (h < 0)
        {
            h += hueDegrees;
        }

        if (double.IsNaN(h))
        {
            // delta == 0, case of pure gray
            h = undefinedChannel;
        }

        return (h, s, v);
    }

    /// <summary>Converts RGB to HSL, returns -1 for undefined channels.</summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Lightness (0-1).</returns>
    public static (double H, double S, double L) RgbToHsl(double r, double g, double b)
    {
        const double undefinedChannel = -1.0;
        const double halfScale = 0.5;
        const double doubleScale = 2.0;
        const double hslGreenOffset = 1.0 / 3.0;
        const double hslBlueOffset = 2.0 / 3.0;
        const double hueSectorCount = 6.0;
        const double hueDegrees = 360.0;
        const double half = 2.0;

        double h;
        double s;
        double l;

        var min = Math.Min(Math.Min(r, g), b);
        var max = Math.Max(Math.Max(r, g), b);
        var delta = max - min;
        l = (max + min) / half;

        if (AreClose(max, 0D))
        {
            // pure black
            return (undefinedChannel, undefinedChannel, 0);
        }

        if (AreClose(delta, 0D))
        {
            // gray
            return (undefinedChannel, 0, l);
        }

        // magic
        s = l <= halfScale ? delta / (max + min) : delta / (doubleScale - max - min);

        h = max switch
        {
            _ when AreClose(r, max) => (g - b) / hueSectorCount / delta,
            _ when AreClose(g, max) => hslGreenOffset + ((b - r) / hueSectorCount / delta),
            _ => hslBlueOffset + ((r - g) / hueSectorCount / delta),
        };

        if (h < 0)
        {
            h++;
        }

        if (h > 1)
        {
            h--;
        }

        h *= hueDegrees;

        return (h, s, l);
    }

    /// <summary>Converts HSV to RGB.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="v">Value, 0-1.</param>
    /// <returns>Values (0-1) in order: R, G, B.</returns>
    public static (double R, double G, double B) HsvToRgb(double h, double s, double v)
    {
        const int redYellowSector = 0;
        const int yellowGreenSector = 1;
        const int greenCyanSector = 2;
        const int cyanBlueSector = 3;
        const int bluePurpleSector = 4;
        const double hueDegreesPerSector = 60.0;
        const double hueDegrees = 360.0;

        if (AreClose(s, 0D))
        {
            // achromatic (grey)
            return (v, v, v);
        }

        if (h >= hueDegrees)
        {
            h = 0;
        }

        h /= hueDegreesPerSector;
        var i = (int)h;
        var f = h - i;
        var p = v * (1 - s);
        var q = v * (1 - (s * f));
        var t = v * (1 - (s * (1 - f)));

        return i switch
        {
            redYellowSector => (v, t, p),
            yellowGreenSector => (q, v, p),
            greenCyanSector => (p, v, t),
            cyanBlueSector => (p, q, v),
            bluePurpleSector => (t, p, v),
            _ => (v, p, q),
        };
    }

    /// <summary>Converts HSV to HSL.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="v">Value, 0-1.</param>
    /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Lightness (0-1).</returns>
    public static (double H, double S, double L) HsvToHsl(double h, double s, double v)
    {
        const double undefinedChannel = -1.0;
        const double doubleScale = 2.0;

        var hsl_l = v * (1 - (s / doubleScale));
        double hsl_s = AreClose(hsl_l, 0D) || AreClose(hsl_l, 1D)
            ? undefinedChannel
            : (v - hsl_l) / Math.Min(hsl_l, 1 - hsl_l);

        return (h, hsl_s, hsl_l);
    }

    /// <summary>Converts HSL to RGB.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="l">Lightness, 0-1.</param>
    /// <returns>Values (0-1) in order: R, G, B.</returns>
    public static (double R, double G, double B) HslToRgb(double h, double s, double l)
    {
        const double halfScale = 0.5;
        const double doubleScale = 2.0;
        const double hueDegreesPerSector = 60.0;
        const int redYellowSector = 0;
        const int yellowGreenSector = 1;
        const int greenCyanSector = 2;
        const int cyanBlueSector = 3;
        const int bluePurpleSector = 4;

        var hueCircleSegment = (int)(h / hueDegreesPerSector);
        var circleSegmentFraction = (h - (hueDegreesPerSector * hueCircleSegment)) / hueDegreesPerSector;

        var maxRGB = l < halfScale ? l * (1 + s) : l + s - (l * s);
        var minRGB = (doubleScale * l) - maxRGB;
        var delta = maxRGB - minRGB;

        return hueCircleSegment switch
        {
            redYellowSector => (maxRGB, (delta * circleSegmentFraction) + minRGB, minRGB), // red-yellow
            yellowGreenSector => ((delta * (1 - circleSegmentFraction)) + minRGB, maxRGB, minRGB), // yellow-green
            greenCyanSector => (minRGB, maxRGB, (delta * circleSegmentFraction) + minRGB), // green-cyan
            cyanBlueSector => (minRGB, (delta * (1 - circleSegmentFraction)) + minRGB, maxRGB), // cyan-blue
            bluePurpleSector => ((delta * circleSegmentFraction) + minRGB, minRGB, maxRGB), // blue-purple
            _ => (maxRGB, minRGB, (delta * (1 - circleSegmentFraction)) + minRGB), // purple-red and invalid values
        };
    }

    /// <summary>Converts HSL to HSV.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="l">Lightness, 0-1.</param>
    /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Value (0-1).</returns>
    public static (double H, double S, double V) HslToHsv(double h, double s, double l)
    {
        const double undefinedChannel = -1.0;
        const double doubleScale = 2.0;

        var hsv_v = l + (s * Math.Min(l, 1 - l));
        double hsv_s = AreClose(hsv_v, 0D) ? undefinedChannel : doubleScale * (1 - (l / hsv_v));

        return (h, hsv_s, hsv_v);
    }

    /// <summary>Determines whether two color components are effectively equal.</summary>
    /// <param name="left">The first component.</param>
    /// <param name="right">The second component.</param>
    /// <returns><see langword="true"/> when the components are within the comparison tolerance.</returns>
    private static bool AreClose(double left, double right) => Math.Abs(left - right) <= ComparisonTolerance;
}
