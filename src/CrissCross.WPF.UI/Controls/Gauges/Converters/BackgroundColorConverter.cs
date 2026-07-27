// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Gauges.Converters;
#else
namespace CrissCross.WPF.UI.Controls.Gauges.Converters;
#endif

/// <summary>Converts background color to Gradient effect.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class BackgroundColorConverter : IValueConverter
{
    /// <summary>Provides the foreground gradient stop offset.</summary>
    private const double ForegroundGradientStopOffset = 0.982;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Converts a value.</summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var radBrush = new RadialGradientBrush();
        radBrush.GradientStops.Add(
            new GradientStop { Offset = ForegroundGradientStopOffset, Color = value is null ? Colors.Transparent : ((SolidColorBrush)value).Color, });
        radBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0xFF, 0xAF, 0xB2, 0xB0) });
        return radBrush;
    }

    /// <summary>Converts a value.</summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
