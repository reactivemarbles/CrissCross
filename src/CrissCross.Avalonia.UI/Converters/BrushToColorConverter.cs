// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>Converts a SolidColorBrush to a Color.</summary>
public class BrushToColorConverter : IValueConverter
{
    /// <summary>Gets the default instance of this converter.</summary>
    public static BrushToColorConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is SolidColorBrush brush ? brush.Color : Colors.Transparent;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Color color ? new SolidColorBrush(color) : null;
    }
}
