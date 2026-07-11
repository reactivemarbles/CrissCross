// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>Converts an enum value to a boolean based on parameter comparison.</summary>
public class EnumToBoolConverter : IValueConverter
{
    /// <summary>Gets the default instance of this converter.</summary>
    public static EnumToBoolConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null || parameter is null ? false : value.Equals(parameter);
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true && parameter is not null ? parameter : BindingOperations.DoNothing;
    }
}
