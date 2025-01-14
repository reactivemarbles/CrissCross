// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

internal class LeftSplitCornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CornerRadius cornerRadius)
        {
            return value;
        }

        return new CornerRadius(cornerRadius.TopLeft, 0, 0, cornerRadius.BottomLeft);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
