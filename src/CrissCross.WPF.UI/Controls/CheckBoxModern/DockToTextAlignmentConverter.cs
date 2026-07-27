// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Dock To Text Alignment Converter.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DockToTextAlignmentConverter : IValueConverter
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Converts the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>TextAlignment value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is Dock dock
        ? dock switch
            {
                Dock.Left => TextAlignment.Left,
                Dock.Right => TextAlignment.Right,
                Dock.Top or Dock.Bottom => TextAlignment.Center,
                _ => TextAlignment.Left,
            }
        : TextAlignment.Left;

    /// <summary>Converts the back.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Dock value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is TextAlignment textAlignment
        ? textAlignment switch
            {
                TextAlignment.Left => Dock.Left,
                TextAlignment.Right => Dock.Right,
                _ => Dock.Left,
            }
        : Dock.Left;
}
