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
    public static readonly DependencyProperty ShowAlphaProperty = DependencyProperty.Register(
        nameof(ShowAlpha),
        typeof(bool),
        typeof(ColorToHexConverter),
        new PropertyMetadata(
            true,
            static (dependencyObject, _) => ((ColorToHexConverter)dependencyObject).RaiseShowAlphaChange()));

    /// <summary>Provides the red character index in a short RGB color.</summary>
    private const int ShortRgbRedIndex = 0;

    /// <summary>Provides the green character index in a short RGB color.</summary>
    private const int ShortRgbGreenIndex = 1;

    /// <summary>Provides the blue character index in a short RGB color.</summary>
    private const int ShortRgbBlueIndex = 2;

    /// <summary>Provides the alpha character index in a short ARGB color.</summary>
    private const int ShortArgbAlphaIndex = 0;

    /// <summary>Provides the red character index in a short ARGB color.</summary>
    private const int ShortArgbRedIndex = 1;

    /// <summary>Provides the green character index in a short ARGB color.</summary>
    private const int ShortArgbGreenIndex = 2;

    /// <summary>Provides the blue character index in a short ARGB color.</summary>
    private const int ShortArgbBlueIndex = 3;

    /// <summary>Provides the first RGB character index in a WPF color string.</summary>
    private const int RgbStartIndex = 3;

    /// <summary>Provides the character count of a short RGB color.</summary>
    private const int ShortRgbLength = 3;

    /// <summary>Provides the character count of a short ARGB color.</summary>
    private const int ShortArgbLength = 4;

    /// <summary>Provides the character count of a full RGB color.</summary>
    private const int RgbLength = 6;

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
        return string.Concat("#".AsSpan(), colorText.AsSpan(RgbStartIndex, RgbLength));
#else
        return "#" + colorText.Substring(RgbStartIndex, RgbLength);
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
        if (text.Length == ShortRgbLength)
        {
            _ = final
                .Append("#FF")
                .Append(text[ShortRgbRedIndex])
                .Append(text[ShortRgbRedIndex])
                .Append(text[ShortRgbGreenIndex])
                .Append(text[ShortRgbGreenIndex])
                .Append(text[ShortRgbBlueIndex])
                .Append(text[ShortRgbBlueIndex]);
        }

        if (text.Length is ShortArgbLength or > RgbLength)
        {
            return DependencyProperty.UnsetValue;
        }

        if (text.Length != ShortRgbLength)
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
        if (text.Length == ShortRgbLength)
        {
            // short hex with no alpha
            _ = final
                .Append("#FF")
                .Append(text[ShortRgbRedIndex])
                .Append(text[ShortRgbRedIndex])
                .Append(text[ShortRgbGreenIndex])
                .Append(text[ShortRgbGreenIndex])
                .Append(text[ShortRgbBlueIndex])
                .Append(text[ShortRgbBlueIndex]);
        }
        else if (text.Length == ShortArgbLength)
        {
            // short hex with alpha
            _ = final
                .Append('#')
                .Append(text[ShortArgbAlphaIndex])
                .Append(text[ShortArgbAlphaIndex])
                .Append(text[ShortArgbRedIndex])
                .Append(text[ShortArgbRedIndex])
                .Append(text[ShortArgbGreenIndex])
                .Append(text[ShortArgbGreenIndex])
                .Append(text[ShortArgbBlueIndex])
                .Append(text[ShortArgbBlueIndex]);
        }
        else if (text.Length == RgbLength)
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
