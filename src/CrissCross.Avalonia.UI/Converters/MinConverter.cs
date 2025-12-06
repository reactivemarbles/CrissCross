// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>
/// Returns the minimum of two values.
/// </summary>
public class MinConverter : IValueConverter
{
    /// <summary>
    /// Gets the default instance of this converter.
    /// </summary>
    public static MinConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue && parameter is double paramDouble)
        {
            return Math.Min(doubleValue, paramDouble);
        }

        if (value is double val && double.TryParse(parameter?.ToString(), out var paramValue))
        {
            return Math.Min(val, paramValue);
        }

        return value;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        BindingOperations.DoNothing;
}
