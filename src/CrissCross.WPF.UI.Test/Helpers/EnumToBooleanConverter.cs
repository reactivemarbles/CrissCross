// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Test.Helpers;

/// <summary>EnumToBooleanConverter member.</summary>
/// <seealso cref="IValueConverter" />
public class EnumToBooleanConverter : IValueConverter
{
    /// <summary>Converts a value.</summary>
    /// <exception cref="ArgumentException">
    /// ExceptionEnumToBooleanConverterParameterMustBeAnEnumName
    /// or
    /// ExceptionEnumToBooleanConverterValueMustBeAnEnum.
    /// </exception>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        if (!Enum.IsDefined(typeof(CrissCross.WPF.UI.Appearance.ApplicationTheme), value))
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
        }

        var enumValue = Enum.Parse<CrissCross.WPF.UI.Appearance.ApplicationTheme>(enumString);

        return enumValue.Equals(value);
    }

    /// <summary>Converts a value.</summary>
    /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterParameterMustBeAnEnumName.</exception>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        return Enum.Parse<CrissCross.WPF.UI.Appearance.ApplicationTheme>(enumString);
    }
}
