// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Provides the MinConverter member.</summary>
internal sealed class MinConverter : IMultiValueConverter
{
    /// <summary>Provides the Convert member.</summary>
    /// <param name="values">The values value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values.Min()!;

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetTypes">The targetTypes value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing];
}
