// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#nullable enable
using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Provides the FallbackBrushConverter member.</summary>
internal sealed class FallbackBrushConverter : IValueConverter
{
    /// <summary>Provides the Convert member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush;
        }

        return value is Color color ? new SolidColorBrush(color) : new SolidColorBrush(
            new Color
            {
                A = 255,
                R = 255,
                G = 0,
                B = 0
            });
    }

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => Binding.DoNothing;
}
