// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>
/// Converts a boolean value to its inverted value.
/// </summary>
public class BoolToInvertedBoolConverter : IValueConverter
{
    /// <summary>
    /// Gets the default instance of this converter.
    /// </summary>
    public static BoolToInvertedBoolConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return false;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return false;
    }
}
