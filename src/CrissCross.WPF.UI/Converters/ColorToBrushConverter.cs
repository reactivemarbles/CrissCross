// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
internal class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var col = (Color)value;
        var c = Color.FromArgb(col.A, col.R, col.G, col.B);
        return new SolidColorBrush(c);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var c = (SolidColorBrush)value;
        var col = Color.FromArgb(c.Color.A, c.Color.R, c.Color.G, c.Color.B);
        return col;
    }
}
