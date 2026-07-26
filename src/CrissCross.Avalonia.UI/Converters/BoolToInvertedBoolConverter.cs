// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data.Converters;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Converters;
#else
namespace CrissCross.Avalonia.UI.Converters;
#endif

/// <summary>Converts a boolean value to its inverted value.</summary>
public class BoolToInvertedBoolConverter : IValueConverter
{
    /// <summary>Gets the default instance of this converter.</summary>
    public static BoolToInvertedBoolConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool boolValue ? !boolValue : false;

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Convert(value, targetType, parameter, culture);
}
