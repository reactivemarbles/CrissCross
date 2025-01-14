// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Dock To Text Alignment Converter.
/// </summary>
public class DockToTextAlignmentConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>TextAlignment value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Dock dock)
        {
            switch (dock)
            {
                case Dock.Left:
                    return TextAlignment.Left;

                case Dock.Right:
                    return TextAlignment.Right;

                case Dock.Top:
                case Dock.Bottom:
                    return TextAlignment.Center;
            }
        }

        return TextAlignment.Left;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Dock value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case TextAlignment.Left:
                    return Dock.Left;

                case TextAlignment.Right:
                    return Dock.Right;
            }
        }

        return Dock.Left;
    }
}
