// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

internal class BackButtonVisibilityToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not NavigationViewBackButtonVisible backButtonVisibility)
        {
            return Visibility.Collapsed;
        }

        return backButtonVisibility switch
        {
            NavigationViewBackButtonVisible.Collapsed => Visibility.Collapsed,
            _ => (object)Visibility.Visible,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
