// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Converters;
#else
namespace CrissCross.WPF.UI.Converters;
#endif

/// <summary>Represents RangeConstrainedDoubleToDoubleConverter.</summary>
/// <seealso cref="DependencyObject" />
/// <seealso cref="IValueConverter" />
[ValueConversion(typeof(double), typeof(string))]
public sealed class RangeConstrainedDoubleToDoubleConverter : DependencyObject, IValueConverter
{
    /// <summary>The minimum property.</summary>
    private static readonly DependencyProperty _minProperty = DependencyProperty.Register(
        nameof(Min),
        typeof(double),
        typeof(RangeConstrainedDoubleToDoubleConverter),
        new PropertyMetadata(0.0));

    /// <summary>The maximum property.</summary>
    private static readonly DependencyProperty _maxProperty = DependencyProperty.Register(
        nameof(Max),
        typeof(double),
        typeof(RangeConstrainedDoubleToDoubleConverter),
        new PropertyMetadata(1.0));

    /// <summary>Gets or sets determines the minimum of the parameters.</summary>
    /// <value>
    /// The minimum.
    /// </value>
    public double Min
    {
        get => (double)GetValue(_minProperty);
        set => SetValue(_minProperty, value);
    }

    /// <summary>Gets or sets determines the maximum of the parameters.</summary>
    /// <value>
    /// The maximum.
    /// </value>
    public double Max
    {
        get => (double)GetValue(_maxProperty);
        set => SetValue(_maxProperty, value);
    }

    /// <summary>Converts a value.</summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

    /// <summary>Converts a value.</summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return !double.TryParse(
            ((string)value).Replace(',', '.'),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var result)
            ? DependencyProperty.UnsetValue
            : MathExtensions.Clamp(result, Min, Max);
    }
}
