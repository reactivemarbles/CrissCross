// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

internal class ProgressThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // TODO: It's too hardcoded, we should define better formula.
        if (value is double height)
        {
            return height / 8;
        }

        return 12.0d;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
