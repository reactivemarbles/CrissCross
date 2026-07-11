// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>Converts a Color to a hex string.</summary>
public class ColorToHexConverter : IValueConverter
{
    /// <summary>Gets the default instance of this converter.</summary>
    public static ColorToHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Color color ? color.ToString() : "#00000000";
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string hexString && Color.TryParse(hexString, out var color) ? color : Colors.Transparent;
    }
}
