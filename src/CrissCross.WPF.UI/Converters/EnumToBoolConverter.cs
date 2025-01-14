// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

internal class EnumToBoolConverter<TEnum> : IValueConverter
    where TEnum : Enum
{
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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
