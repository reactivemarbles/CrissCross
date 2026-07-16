// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Provides the ProgressThicknessConverter member.</summary>
internal sealed class ProgressThicknessConverter : IValueConverter
{
    /// <summary>Provides the fallback progress indicator thickness.</summary>
    private const double DefaultThickness = 12D;

    /// <summary>Provides the height divisor used to calculate progress indicator thickness.</summary>
    private const double HeightDivisor = 8D;

    /// <summary>Provides the Convert member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // TODO: It's too hardcoded, we should define better formula.
        return value is double height ? height / HeightDivisor : DefaultThickness;
    }

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
