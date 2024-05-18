// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
internal class BoolToVisibilityConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty InvertProperty =
        DependencyProperty.Register(
            nameof(Invert),
            typeof(bool),
            typeof(BoolToVisibilityConverter),
            new PropertyMetadata(false));

    public bool Invert
    {
        get => (bool)GetValue(InvertProperty);
        set => SetValue(InvertProperty, value);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actualValue = (bool)value ^ Invert;
        return actualValue ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
