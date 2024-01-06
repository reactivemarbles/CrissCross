// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Test.Helpers;

/// <summary>
/// EnumToBooleanConverter.
/// </summary>
/// <seealso cref="IValueConverter" />
public class EnumToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// ExceptionEnumToBooleanConverterParameterMustBeAnEnumName
    /// or
    /// ExceptionEnumToBooleanConverterValueMustBeAnEnum.
    /// </exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        if (!Enum.IsDefined(typeof(Wpf.Ui.Appearance.ApplicationTheme), value))
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
        }

        var enumValue = Enum.Parse(typeof(Wpf.Ui.Appearance.ApplicationTheme), enumString);

        return enumValue.Equals(value);
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
    /// </returns>
    /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterParameterMustBeAnEnumName.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        return Enum.Parse(typeof(Wpf.Ui.Appearance.ApplicationTheme), enumString);
    }
}
