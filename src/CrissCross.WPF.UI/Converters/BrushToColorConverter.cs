// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

internal class BrushToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color;
        }

        if (value is Color)
        {
            return value;
        }

        // We draw red to visibly see an invalid bind in the UI.
        return new Color
        {
            A = 255,
            R = 255,
            G = 0,
            B = 0
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
