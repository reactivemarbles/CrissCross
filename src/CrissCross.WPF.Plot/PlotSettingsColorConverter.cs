// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Converts plot color strings and picker colors without changing channel order.</summary>
internal sealed class PlotSettingsColorConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string colorName || string.IsNullOrWhiteSpace(colorName))
        {
            return DependencyProperty.UnsetValue;
        }

        var color = colorName.StartsWith('#')
            ? CrissCross.Plot.Color.FromHex(colorName)
            : CrissCross.Plot.Color.FromColor(System.Drawing.Color.FromName(colorName));
        return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is System.Windows.Media.Color color ? new CrissCross.Plot.Color(color.R, color.G, color.B, color.A).ToHex() : Binding.DoNothing;
}
