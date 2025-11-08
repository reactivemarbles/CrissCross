// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Controls.Gauges.Converters;

/// <summary>
/// Calculates the range indicator light position.
/// </summary>
public class RangeIndicatorLightPositionConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var tg = new TransformGroup();
        tg.Children.Add(new RotateTransform());
        tg.Children.Add(new TranslateTransform { Y = (double)value });
        return tg;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
