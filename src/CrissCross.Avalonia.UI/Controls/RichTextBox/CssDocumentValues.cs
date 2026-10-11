// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Converts the CSS values commonly exported by word processors.</summary>
internal static class CssDocumentValues
{
    /// <summary>Default font size used by relative CSS units.</summary>
    private const double DefaultFontSize = 14;

    /// <summary>Percentage divisor.</summary>
    private const double PercentDivisor = 100;

    /// <summary>Number of RGB channels.</summary>
    private const int RgbChannels = 3;

    /// <summary>Number of RGBA channels.</summary>
    private const int RgbaChannels = 4;

    /// <summary>Maximum color channel value.</summary>
    private const byte MaximumChannel = 255;

    /// <summary>CSS unit conversion factors to device independent pixels.</summary>
    private static readonly Dictionary<string, double> AbsoluteUnits = new(StringComparer.Ordinal)
    {
        ["px"] = 1,
        ["pt"] = 96D / 72,
        ["pc"] = 16,
        ["in"] = 96,
        ["cm"] = 96D / 2.54,
        ["mm"] = 96D / 25.4,
    };

    /// <summary>Converts a CSS length to device independent pixels.</summary>
    /// <param name="value">The CSS length.</param>
    /// <param name="inherited">The font size for relative units.</param>
    /// <returns>The supported positive length.</returns>
    internal static double? ParseLength(string value, double? inherited)
    {
        var text = value.Trim().ToLowerInvariant();
        var multiplier = GetUnitMultiplier(ref text, inherited);
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            && double.IsFinite(number) && number > 0 ? number * multiplier : null;
    }

    /// <summary>Parses named, hexadecimal and RGB CSS colors.</summary>
    /// <param name="value">The CSS color.</param>
    /// <param name="color">The parsed color.</param>
    /// <returns>Whether the color is supported.</returns>
    internal static bool TryParseColor(string value, out Color color)
    {
        if (Color.TryParse(value, out color))
        {
            return true;
        }

        var start = value.IndexOf('(');
        if (start < 0 || !value.EndsWith(')') || !value.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var parts = value[(start + 1)..^1].Split(',', StringSplitOptions.TrimEntries);
        return TryParseChannels(parts, out color);
    }

    /// <summary>Reads and removes a supported unit suffix.</summary>
    /// <param name="text">The CSS number and unit.</param>
    /// <param name="inherited">The inherited font size.</param>
    /// <returns>The unit multiplier.</returns>
    private static double GetUnitMultiplier(ref string text, double? inherited)
    {
        if (text.EndsWith('%'))
        {
            text = text[..^1];
            return (inherited ?? DefaultFontSize) / PercentDivisor;
        }

        foreach (var unit in AbsoluteUnits)
        {
            if (text.EndsWith(unit.Key, StringComparison.Ordinal))
            {
                text = text[..^unit.Key.Length];
                return unit.Value;
            }
        }

        return GetFontUnitMultiplier(ref text, inherited);
    }

    /// <summary>Reads font relative unit suffixes.</summary>
    /// <param name="text">The CSS number and unit.</param>
    /// <param name="inherited">The inherited font size.</param>
    /// <returns>The multiplier.</returns>
    private static double GetFontUnitMultiplier(ref string text, double? inherited)
    {
        const string rootFontUnit = "rem";
        const string fontUnit = "em";
        if (text.EndsWith(rootFontUnit, StringComparison.Ordinal))
        {
            text = text[..^rootFontUnit.Length];
            return DefaultFontSize;
        }

        if (text.EndsWith(fontUnit, StringComparison.Ordinal))
        {
            text = text[..^fontUnit.Length];
            return inherited ?? DefaultFontSize;
        }

        return 1;
    }

    /// <summary>Parses RGB or RGBA component lists.</summary>
    /// <param name="parts">The component strings.</param>
    /// <param name="color">The parsed color.</param>
    /// <returns>Whether all components are valid.</returns>
    private static bool TryParseChannels(string[] parts, out Color color)
    {
        color = default;
        if (parts.Length is not (RgbChannels or RgbaChannels))
        {
            return false;
        }

        Span<byte> channels = stackalloc byte[RgbaChannels];
        channels[RgbChannels] = MaximumChannel;
        for (var index = 0; index < parts.Length; index++)
        {
            if (!TryParseChannel(parts[index], index == RgbChannels, out channels[index]))
            {
                return false;
            }
        }

        color = Color.FromArgb(channels[RgbChannels], channels[0], channels[1], channels[2]);
        return true;
    }

    /// <summary>Parses one CSS channel, including percentages.</summary>
    /// <param name="text">The component value.</param>
    /// <param name="alpha">Whether the value is alpha.</param>
    /// <param name="channel">The converted channel.</param>
    /// <returns>Whether the component is finite and numeric.</returns>
    private static bool TryParseChannel(string text, bool alpha, out byte channel)
    {
        channel = 0;
        var percentage = text.EndsWith('%');
        var number = percentage ? text[..^1] : text;
        if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var component) || !double.IsFinite(component))
        {
            return false;
        }

        var multiplier = alpha ? MaximumChannel : 1D;
        if (percentage)
        {
            multiplier = MaximumChannel / PercentDivisor;
        }

        channel = (byte)Math.Clamp(Math.Round(component * multiplier), 0, MaximumChannel);
        return true;
    }
}
