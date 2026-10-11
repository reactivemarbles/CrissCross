// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Converts plot color strings and picker colors without changing channel order.</summary>
internal sealed class PlotSettingsColorConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string colorName || string.IsNullOrWhiteSpace(colorName))
        {
            return AvaloniaProperty.UnsetValue;
        }

        var color = colorName.StartsWith('#')
            ? ScottPlot.Color.FromHex(colorName)
            : ScottPlot.Color.FromColor(System.Drawing.Color.FromName(colorName));
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Color color ? new ScottPlot.Color(color.R, color.G, color.B, color.A).ToHex() : BindingOperations.DoNothing;
}
