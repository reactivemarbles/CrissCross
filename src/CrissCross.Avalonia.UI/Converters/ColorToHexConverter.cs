// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>
/// Converts a Color to a hex string.
/// </summary>
public class ColorToHexConverter : IValueConverter
{
    /// <summary>
    /// Gets the default instance of this converter.
    /// </summary>
    public static ColorToHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return color.ToString();
        }

        return "#00000000";
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hexString && Color.TryParse(hexString, out var color))
        {
            return color;
        }

        return Colors.Transparent;
    }
}
