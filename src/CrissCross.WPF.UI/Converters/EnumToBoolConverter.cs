// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Provides the EnumToBoolConverter member.</summary>
/// <typeparam name="TEnum">The type.</typeparam>
internal class EnumToBoolConverter<TEnum> : IValueConverter
    where TEnum : Enum
{
    /// <summary>Provides the Convert member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TEnum valueEnum)
        {
            throw new ArgumentException($"{nameof(value)} is not type: {typeof(TEnum)}");
        }

        if (parameter is not TEnum parameterEnum)
        {
            throw new ArgumentException($"{nameof(parameter)} is not type: {typeof(TEnum)}");
        }

        return EqualityComparer<TEnum>.Default.Equals(valueEnum, parameterEnum);
    }

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
