// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Converters;
#else
namespace CrissCross.WPF.UI.Converters;
#endif

/// <summary>Provides the ProportialConverter member.</summary>
public sealed class ProportialConverter : IMultiValueConverter
{
    /// <summary>Provides the primary value index.</summary>
    private const int PrimaryValueIndex = 0;

    /// <summary>Provides the available-size value index.</summary>
    private const int AvailableSizeIndex = 1;

    /// <summary>Provides the scale-factor value index.</summary>
    private const int ScaleFactorIndex = 2;

    /// <summary>Provides the Convert member.</summary>
    /// <param name="values">The values value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values[PrimaryValueIndex] switch
        {
            double firstValue
                when values[AvailableSizeIndex] is double secondValue
                    && values[ScaleFactorIndex] is double scaleFactor => firstValue * (secondValue / scaleFactor),
            _ => 0,
        };

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetTypes">The targetTypes value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        [Binding.DoNothing];
}
