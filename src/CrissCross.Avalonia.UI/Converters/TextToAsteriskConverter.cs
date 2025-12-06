// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CrissCross.Avalonia.UI.Converters;

/// <summary>
/// Converts a string to asterisks for password display.
/// </summary>
public class TextToAsteriskConverter : IValueConverter
{
    /// <summary>
    /// Gets the default instance of this converter.
    /// </summary>
    public static TextToAsteriskConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            return new string('•', text.Length);
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        BindingOperations.DoNothing;
}
