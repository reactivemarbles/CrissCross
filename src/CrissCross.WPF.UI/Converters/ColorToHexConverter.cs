// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

[ValueConversion(typeof(Color), typeof(string))]
internal class ColorToHexConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(ColorToHexConverter),
            new PropertyMetadata(true, ShowAlphaChangedCallback));

    public event EventHandler? OnShowAlphaChange;

    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>
    /// Converts the no alpha.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An object.</returns>
    public static object ConvertNoAlpha(object value) => "#" + ((Color)value).ToString().Substring(3, 6);

    public static object ConvertBackNoAlpha(object value)
    {
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

    public void RaiseShowAlphaChange() => OnShowAlphaChange?.Invoke(this, EventArgs.Empty);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!ShowAlpha)
        {
            return ColorToHexConverter.ConvertNoAlpha(value);
        }

        return ((Color)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!ShowAlpha)
        {
            return ColorToHexConverter.ConvertBackNoAlpha(value);
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
