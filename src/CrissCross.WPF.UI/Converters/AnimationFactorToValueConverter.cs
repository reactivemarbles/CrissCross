// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Provides the AnimationFactorToValueConverter member.</summary>
internal sealed class AnimationFactorToValueConverter : IMultiValueConverter
{
    /// <summary>Provides the Convert member.</summary>
    /// <param name="values">The values value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is not double completeValue)
        {
            return 0.0;
        }

        if (values[1] is not double factor)
        {
            return 0.0;
        }

        if (parameter is "negative")
        {
            factor = -factor;
        }

        return factor * completeValue;
    }

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetTypes">The targetTypes value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing];
}
