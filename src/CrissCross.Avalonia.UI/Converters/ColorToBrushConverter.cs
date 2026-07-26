// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Converters;
#else
namespace CrissCross.Avalonia.UI.Converters;
#endif

/// <summary>Converts a Color to a SolidColorBrush.</summary>
public class ColorToBrushConverter : IValueConverter
{
    /// <summary>Gets the default instance of this converter.</summary>
    public static ColorToBrushConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is Color color ? new SolidColorBrush(color) : null;

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is SolidColorBrush brush ? brush.Color : Colors.Transparent;
}
