// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents an icon source that uses a glyph from the specified font.</summary>
public class FontIconSource : IconSource
{
    /// <summary>Property for <see cref="FontFamily"/>.</summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
        nameof(FontFamily),
        typeof(FontFamily),
        typeof(FontIconSource),
        new PropertyMetadata(SystemFonts.MessageFontFamily));

    /// <summary>Property for <see cref="FontSize"/>.</summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        nameof(FontSize),
        typeof(double),
        typeof(FontIconSource),
        new PropertyMetadata(SystemFonts.MessageFontSize));

    /// <summary>Property for <see cref="FontStyle"/>.</summary>
    public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
        nameof(FontStyle),
        typeof(FontStyle),
        typeof(FontIconSource),
        new PropertyMetadata(FontStyles.Normal));

    /// <summary>Property for <see cref="FontWeight"/>.</summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        nameof(FontWeight),
        typeof(FontWeight),
        typeof(FontIconSource),
        new PropertyMetadata(FontWeights.Normal));

    /// <summary>Property for <see cref="Glyph"/>.</summary>
    public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
        nameof(Glyph),
        typeof(string),
        typeof(FontIconSource),
        new PropertyMetadata(string.Empty));

    /// <summary>Gets or sets the font family used to render the icon glyph.</summary>
    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>Gets or sets the font size used to render the icon glyph.</summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>Gets or sets the font weight used to render the icon glyph.</summary>
    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    /// <summary>Gets or sets the font style used to render the icon glyph.</summary>
    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    /// <summary>Gets or sets the character code that identifies the icon glyph.</summary>
    /// <returns>The hexadecimal character code for the icon glyph.</returns>
    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    /// <summary>Creates the icon element.</summary>
    /// <returns>A IconElement.</returns>
    public override IconElement CreateIconElement()
    {
        var glyph = Glyph;
        var fontIcon = new FontIcon() { Glyph = glyph };

        if (!Equals(FontFamily, SystemFonts.MessageFontFamily))
        {
            fontIcon.FontFamily = FontFamily;
        }

        if (!FontSize.Equals(SystemFonts.MessageFontSize))
        {
            fontIcon.FontSize = FontSize;
        }

        if (FontWeight != FontWeights.Normal)
        {
            fontIcon.FontWeight = FontWeight;
        }

        if (FontStyle != FontStyles.Normal)
        {
            fontIcon.FontStyle = FontStyle;
        }

        if (!Equals(Foreground, SystemColors.ControlTextBrush))
        {
            fontIcon.Foreground = Foreground;
        }

        return fontIcon;
    }
}
