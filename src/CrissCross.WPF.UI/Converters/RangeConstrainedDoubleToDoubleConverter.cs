// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>
/// RangeConstrainedDoubleToDoubleConverter.
/// </summary>
/// <seealso cref="DependencyObject" />
/// <seealso cref="IValueConverter" />
[ValueConversion(typeof(double), typeof(string))]
internal class RangeConstrainedDoubleToDoubleConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// The minimum property.
    /// </summary>
    private static DependencyProperty MinProperty =
        DependencyProperty.Register(
            nameof(Min),
            typeof(double),
            typeof(RangeConstrainedDoubleToDoubleConverter),
            new PropertyMetadata(0.0));

    /// <summary>
    /// The maximum property.
    /// </summary>
    private static DependencyProperty MaxProperty =
        DependencyProperty.Register(
            nameof(Max),
            typeof(double),
            typeof(RangeConstrainedDoubleToDoubleConverter),
            new PropertyMetadata(1.0));

    /// <summary>
    /// Gets or sets determines the minimum of the parameters.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public double Min
    {
        get => (double)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    /// <summary>
    /// Gets or sets determines the maximum of the parameters.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public double Max
    {
        get => (double)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!double.TryParse(((string)value).Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return DependencyProperty.UnsetValue;
        }

        return MathHelper.Clamp(result, Min, Max);
    }
}
