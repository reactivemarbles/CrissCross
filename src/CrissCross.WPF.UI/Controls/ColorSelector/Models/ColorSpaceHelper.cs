// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Provides the ColorSpaceHelper member.</summary>
internal static class ColorSpaceHelper
{
    /// <summary>The hue-sector offset used when blue is the largest RGB component.</summary>
    private const double BlueHueSectorOffset = 4D;

    /// <summary>The number of degrees in a complete hue circle.</summary>
    private const double DegreesInFullCircle = 360D;

    /// <summary>The number of degrees in each HSV hue sector.</summary>
    private const double DegreesPerHueSector = 60D;

    /// <summary>The halfway point in normalized color calculations.</summary>
    private const double Half = 0.5D;

    /// <summary>The number of hue sectors used by the HSL conversion formula.</summary>
    private const double HslHueSectorCount = 6D;

    /// <summary>The scale used when calculating normalized lightness.</summary>
    private const double LightnessScale = 2D;

    /// <summary>The normalized HSL hue offset for green.</summary>
    private const double OneThird = 1D / 3D;

    /// <summary>The normalized HSL hue offset for blue.</summary>
    private const double TwoThirds = 2D / 3D;

    /// <summary>The hue-sector offset used when green is the largest RGB component.</summary>
    private const double GreenHueSectorOffset = 2D;

    /// <summary>The integer hue sector for cyan in RGB conversion switch expressions.</summary>
    private const int CyanHueSector = 3;

    /// <summary>The integer hue sector for green in RGB conversion switch expressions.</summary>
    private const int GreenHueSector = 2;

    /// <summary>The integer hue sector for purple in RGB conversion switch expressions.</summary>
    private const int PurpleHueSector = 4;

    /// <summary>Converts RGB to HSV, returns -1 for undefined channels.</summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Value (0-1).</returns>
    public static Tuple<double, double, double> RgbToHsv(double r, double g, double b)
    {
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
        if (max != 0)
        {
            s = delta / max;
        }
        else
        {
            // pure black
            s = -1;
            h = -1;
            return new Tuple<double, double, double>(h, s, v);
        }

        h = max switch
        {
            _ when DoubleComparison.AreClose(r, max) => (g - b) / delta,
            _ when DoubleComparison.AreClose(g, max) => GreenHueSectorOffset + ((b - r) / delta),
            _ => BlueHueSectorOffset + ((r - g) / delta),
        };

        h *= DegreesPerHueSector;
        if (h < 0)
        {
            h += DegreesInFullCircle;
        }

        if (double.IsNaN(h))
        {
            // delta == 0, case of pure gray
            h = -1;
        }

        return new Tuple<double, double, double>(h, s, v);
    }

    /// <summary>Converts RGB to HSL, returns -1 for undefined channels.</summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Lightness (0-1).</returns>
    public static Tuple<double, double, double> RgbToHsl(double r, double g, double b)
    {
        double h;
        double s;
        double l;

        var min = Math.Min(Math.Min(r, g), b);
        var max = Math.Max(Math.Max(r, g), b);
        var delta = max - min;
        l = (max + min) / LightnessScale;

        if (max == 0)
        {
            // pure black
            return new Tuple<double, double, double>(-1, -1, 0);
        }

        if (delta == 0)
        {
            // gray
            return new Tuple<double, double, double>(-1, 0, l);
        }

        // magic
        s = l <= Half ? delta / (max + min) : delta / (LightnessScale - max - min);

        h = max switch
        {
            _ when DoubleComparison.AreClose(r, max) => (g - b) / HslHueSectorCount / delta,
            _ when DoubleComparison.AreClose(g, max) => OneThird + ((b - r) / HslHueSectorCount / delta),
            _ => TwoThirds + ((r - g) / HslHueSectorCount / delta),
        };

        if (h < 0)
        {
            h++;
        }

        if (h > 1)
        {
            h--;
        }

        h *= DegreesInFullCircle;

        return new Tuple<double, double, double>(h, s, l);
    }

    /// <summary>Converts HSV to RGB.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="v">Value, 0-1.</param>
    /// <returns>Values (0-1) in order: R, G, B.</returns>
    public static Tuple<double, double, double> HsvToRgb(double h, double s, double v)
    {
        if (s == 0)
        {
            // achromatic (grey)
            return new Tuple<double, double, double>(v, v, v);
        }

        if (h >= DegreesInFullCircle)
        {
            h = 0;
        }

        h /= DegreesPerHueSector;
        var i = (int)h;
        var f = h - i;
        var p = v * (1 - s);
        var q = v * (1 - (s * f));
        var t = v * (1 - (s * (1 - f)));

        return i switch
        {
            0 => new Tuple<double, double, double>(v, t, p),
            1 => new Tuple<double, double, double>(q, v, p),
            GreenHueSector => new Tuple<double, double, double>(p, v, t),
            CyanHueSector => new Tuple<double, double, double>(p, q, v),
            PurpleHueSector => new Tuple<double, double, double>(t, p, v),
            _ => new Tuple<double, double, double>(v, p, q),
        };
    }

    /// <summary>Converts HSV to HSL.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="v">Value, 0-1.</param>
    /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Lightness (0-1).</returns>
    public static Tuple<double, double, double> HsvToHsl(double h, double s, double v)
    {
        var hsl_l = v * (1 - (s / LightnessScale));
        var hsl_s =
            DoubleComparison.AreClose(hsl_l, 0D) || DoubleComparison.AreClose(hsl_l, 1D)
                ? -1
                : (v - hsl_l) / Math.Min(hsl_l, 1 - hsl_l);

        return new Tuple<double, double, double>(h, hsl_s, hsl_l);
    }

    /// <summary>Converts HSL to RGB.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="l">Lightness, 0-1.</param>
    /// <returns>Values (0-1) in order: R, G, B.</returns>
    public static Tuple<double, double, double> HslToRgb(double h, double s, double l)
    {
        var hueCircleSegment = (int)(h / DegreesPerHueSector);
        var circleSegmentFraction = (h - (DegreesPerHueSector * hueCircleSegment)) / DegreesPerHueSector;

        var maxRGB = l < Half ? l * (1 + s) : l + s - (l * s);
        var minRGB = (LightnessScale * l) - maxRGB;
        var delta = maxRGB - minRGB;

        return hueCircleSegment switch
        {
            0 => new Tuple<double, double, double>(
                maxRGB,
                (delta * circleSegmentFraction) + minRGB,
                minRGB),
            1 => new Tuple<double, double, double>(
                (delta * (1 - circleSegmentFraction)) + minRGB,
                maxRGB,
                minRGB),
            GreenHueSector => new Tuple<double, double, double>(
                minRGB,
                maxRGB,
                (delta * circleSegmentFraction) + minRGB), // green-cyan
            CyanHueSector => new Tuple<double, double, double>(
                minRGB,
                (delta * (1 - circleSegmentFraction)) + minRGB,
                maxRGB), // cyan-blue
            PurpleHueSector => new Tuple<double, double, double>(
                (delta * circleSegmentFraction) + minRGB,
                minRGB,
                maxRGB), // blue-purple
            _ => new Tuple<double, double, double>(
                maxRGB,
                minRGB,
                (delta * (1 - circleSegmentFraction)) + minRGB),
        };
    }

    /// <summary>Converts HSL to HSV.</summary>
    /// <param name="h">Hue, 0-360.</param>
    /// <param name="s">Saturation, 0-1.</param>
    /// <param name="l">Lightness, 0-1.</param>
    /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Value (0-1).</returns>
    public static Tuple<double, double, double> HslToHsv(double h, double s, double l)
    {
        var hsv_v = l + (s * Math.Min(l, 1 - l));
        var hsv_s = hsv_v == 0 ? -1 : LightnessScale * (1 - (l / hsv_v));

        return new Tuple<double, double, double>(h, hsv_s, hsv_v);
    }
}
