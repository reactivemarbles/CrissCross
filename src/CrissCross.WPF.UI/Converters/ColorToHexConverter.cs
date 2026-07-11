// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Converts between a Color and its hexadecimal string representation, with optional alpha support.</summary>
[ValueConversion(typeof(Color), typeof(string))]
#if NET7_0_OR_GREATER
public partial class ColorToHexConverter : DependencyObject, IValueConverter
#else
public class ColorToHexConverter : DependencyObject, IValueConverter
#endif
{
    /// <summary>DependencyProperty to control whether alpha should be included.</summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(ColorToHexConverter),
            new PropertyMetadata(true, ShowAlphaChangedCallback));

    /// <summary>Raised when ShowAlpha changes.</summary>
    public event EventHandler? OnShowAlphaChange;

    /// <summary>Gets or sets a value indicating whether the alpha channel should be included.</summary>
    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>Convert a Color to a hex string without alpha.</summary>
    /// <exception cref="System.ArgumentNullException">value.</exception>
    /// <param name="value">The value.</param>
    /// <returns>The converted value.</returns>
    public static object ConvertNoAlpha(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var colorText = ((Color)value).ToString();
#if NET8_0_OR_GREATER
        return string.Concat("#".AsSpan(), colorText.AsSpan(3, 6));
#else
        return "#" + colorText[3..9];
#endif
    }

    /// <summary>Convert a hex string (without alpha) to a Color.</summary>
    /// <exception cref="System.ArgumentNullException">value.</exception>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The converted value.
    /// </returns>
    public static object ConvertBackNoAlpha(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var text = (string)value;
        text = HexCharacterRegex().Replace(text.ToUpperInvariant(), string.Empty);
        var final = new StringBuilder();

        // short hex
        if (text.Length == 3)
        {
            _ = final.Append("#FF").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]);
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
            _ = final.Append('#').Append(text);
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

    /// <summary>Raise the ShowAlpha changed event.</summary>
    public void RaiseShowAlphaChange() => OnShowAlphaChange?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return DependencyProperty.UnsetValue;
        }

        return !ShowAlpha ? ConvertNoAlpha(value) : ((Color)value).ToString();
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
        text = HexCharacterRegex().Replace(text.ToUpperInvariant(), string.Empty);
        var final = new StringBuilder();
        if (text.Length == 3)
        {
            // short hex with no alpha
            _ = final.Append("#FF").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]);
        }
        else if (text.Length == 4)
        {
            // short hex with alpha
            _ = final.Append('#').Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]).Append(text[3]).Append(text[3]);
        }
        else if (text.Length == 6)
        {
            // hex with no alpha
            _ = final.Append("#FF").Append(text);
        }
        else
        {
            _ = final.Append('#').Append(text);
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

    /// <summary>Provides the ShowAlphaChangedCallback member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void ShowAlphaChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ColorToHexConverter)d).RaiseShowAlphaChange();

#if NET7_0_OR_GREATER
    /// <summary>Gets a regex that matches non-hexadecimal characters.</summary>
    /// <returns>The hexadecimal character regex.</returns>
    [GeneratedRegex("[^0-9A-F]")]
    private static partial Regex HexCharacterRegex();
#else
    /// <summary>Gets a regex that matches non-hexadecimal characters.</summary>
    /// <returns>The hexadecimal character regex.</returns>
    private static Regex HexCharacterRegex() => new("[^0-9A-F]", RegexOptions.Compiled);
#endif
}
