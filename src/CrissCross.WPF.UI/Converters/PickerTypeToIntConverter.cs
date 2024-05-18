﻿// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

[ValueConversion(typeof(PickerType), typeof(int))]
internal class PickerTypeToIntConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (PickerType)value;
}
