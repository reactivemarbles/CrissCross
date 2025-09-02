// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>
/// Converts between a Color and its hexadecimal string representation, with optional alpha support.
/// </summary>
[ValueConversion(typeof(Color), typeof(string))]
public class ColorToHexConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// DependencyProperty to control whether alpha should be included.
    /// </summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(ColorToHexConverter),
            new PropertyMetadata(true, ShowAlphaChangedCallback));

    /// <summary>
    /// Raised when ShowAlpha changes.
    /// </summary>
    public event EventHandler? OnShowAlphaChange;

    /// <summary>
    /// Gets or sets a value indicating whether the alpha channel should be included.
    /// </summary>
    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>
    /// Convert a Color to a hex string without alpha.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The converted value.</returns>
    /// <exception cref="System.ArgumentNullException">value.</exception>
    public static object ConvertNoAlpha(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return "#" + ((Color)value).ToString().Substring(3, 6);
    }

    /// <summary>
    /// Convert a hex string (without alpha) to a Color.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The converted value.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">value.</exception>
    public static object ConvertBackNoAlpha(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var text = (string)value;
        text = Regex.Replace(text.ToUpperInvariant(), "[^0-9A-F]", string.Empty);
        var final = new StringBuilder();

        // short hex
        if (text.Length == 3)
        {
            final.Append("#FF").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]);
        }
        else if (text.Length == 4)
        {
            return DependencyProperty.UnsetValue;
        }
        else if (text.Length > 6)
        {
            return DependencyProperty.UnsetValue;
        }
        else
        {
            // regular hex
            final.Append('#').Append(text);
        }

        try
        {
            return ColorConverter.ConvertFromString(final.ToString());
        }
        catch (Exception)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Raise the ShowAlpha changed event.
    /// </summary>
    public void RaiseShowAlphaChange() => OnShowAlphaChange?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return DependencyProperty.UnsetValue;
        }

        if (!ShowAlpha)
        {
            return ConvertNoAlpha(value);
        }

        return ((Color)value).ToString();
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return DependencyProperty.UnsetValue;
        }

        if (!ShowAlpha)
        {
            return ConvertBackNoAlpha(value);
        }

        var text = (string)value;
        text = Regex.Replace(text.ToUpperInvariant(), "[^0-9A-F]", string.Empty);
        var final = new StringBuilder();
        if (text.Length == 3)
        {
            // short hex with no alpha
            final.Append("#FF").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]);
        }
        else if (text.Length == 4)
        {
            // short hex with alpha
            final.Append('#').Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]).Append(text[3]).Append(text[3]);
        }
        else if (text.Length == 6)
        {
            // hex with no alpha
            final.Append("#FF").Append(text);
        }
        else
        {
            final.Append('#').Append(text);
        }

        try
        {
            return ColorConverter.ConvertFromString(final.ToString());
        }
        catch (Exception)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    private static void ShowAlphaChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ColorToHexConverter)d).RaiseShowAlphaChange();
}
