// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Dock To Text Alignment Converter.</summary>
public class DockToTextAlignmentConverter : IValueConverter
{
    /// <summary>Converts the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>TextAlignment value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Dock dock
            ? dock switch
            {
                Dock.Left => TextAlignment.Left,
                Dock.Right => TextAlignment.Right,
                Dock.Top or Dock.Bottom => TextAlignment.Center,
                _ => TextAlignment.Left,
            }
            : TextAlignment.Left;
    }

    /// <summary>Converts the back.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Dock value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TextAlignment textAlignment
            ? textAlignment switch
            {
                TextAlignment.Left => Dock.Left,
                TextAlignment.Right => Dock.Right,
                _ => Dock.Left,
            }
            : Dock.Left;
    }
}
