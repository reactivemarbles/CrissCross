// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#nullable enable
using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

internal class FallbackBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush;
        }

        if (value is Color color)
        {
            return new SolidColorBrush(color);
        }

        // We draw red to visibly see an invalid bind in the UI.
        return new SolidColorBrush(
            new Color
            {
                A = 255,
                R = 255,
                G = 0,
                B = 0
            });
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => Binding.DoNothing;
}
