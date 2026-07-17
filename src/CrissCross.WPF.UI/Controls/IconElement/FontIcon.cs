// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Documents;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using SystemFonts = System.Windows.SystemFonts;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents an icon that uses a glyph from the specified font.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(FontIcon), "FontIcon.bmp")]
public class FontIcon : IconElement
{
    /// <summary>Property for <see cref="FontFamily"/>.</summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
        nameof(FontFamily),
        typeof(FontFamily),
        typeof(FontIcon),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnFontFamilyChanged));

    /// <summary>Property for <see cref="FontSize"/>.</summary>
    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
        typeof(FontIcon),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontSize,
            FrameworkPropertyMetadataOptions.Inherits,
            OnFontSizeChanged));

    /// <summary>Property for <see cref="FontStyle"/>.</summary>
    public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
        nameof(FontStyle),
        typeof(FontStyle),
        typeof(FontIcon),
        new FrameworkPropertyMetadata(FontStyles.Normal, OnFontStyleChanged));

    /// <summary>Property for <see cref="FontWeight"/>.</summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        nameof(FontWeight),
        typeof(FontWeight),
        typeof(FontIcon),
        new FrameworkPropertyMetadata(FontWeights.Normal, OnFontWeightChanged));

    /// <summary>Property for <see cref="Glyph"/>.</summary>
    public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
        nameof(Glyph),
        typeof(string),
        typeof(FontIcon),
        new FrameworkPropertyMetadata(string.Empty, OnGlyphChanged));

    /// <summary>Gets or sets the font family used to render the icon glyph.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    [Localizability(LocalizationCategory.Font)]
    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>Gets or sets the font size used to render the icon glyph.</summary>
    [TypeConverter(typeof(FontSizeConverter))]
    [Bindable(true)]
    [Category("Appearance")]
    [Localizability(LocalizationCategory.None)]
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>Gets or sets the font style used to render the icon glyph.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    /// <summary>Gets or sets the font weight used to render the icon glyph.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    /// <summary>Gets or sets the character code that identifies the icon glyph.</summary>
    /// <returns>The hexadecimal character code for the icon glyph.</returns>
    public string? Glyph
    {
        get => (string?)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    /// <summary>Gets or sets the text block.</summary>
    protected TextBlock? TextBlock { get; set; }

    /// <summary>Initializes the children.</summary>
    /// <returns>A UIElement.</returns>
    protected override UIElement InitializeChildren()
    {
        if (FontSize.Equals(SystemFonts.MessageFontSize))
        {
            SetResourceReference(FontSizeProperty, "DefaultIconFontSize");

            // If the FontSize is the default, set it to the parent's FontSize.
            if (
                VisualParent is not null
                && !DoubleComparison.AreClose(TextElement.GetFontSize(VisualParent), SystemFonts.MessageFontSize))
            {
                SetCurrentValue(FontSizeProperty, TextElement.GetFontSize(VisualParent));
            }
        }

        var fontFamily = FontFamily;
        var fontSize = FontSize;
        var fontStyle = FontStyle;
        var fontWeight = FontWeight;

        TextBlock = new TextBlock
        {
            Style = null,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            FontFamily = fontFamily,
            FontSize = fontSize,
            FontStyle = fontStyle,
            FontWeight = fontWeight,
            Text = Glyph,
            Visibility = Visibility.Visible,
            Focusable = false,
        };

        Focusable = false;

        return TextBlock;
    }

    /// <summary>Provides the OnFontFamilyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (FontIcon)d;
        if (self.TextBlock is null)
        {
            return;
        }

        self.TextBlock.FontFamily = (FontFamily)e.NewValue;
    }

    /// <summary>Provides the OnFontSizeChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (FontIcon)d;
        if (self.TextBlock is null)
        {
            return;
        }

        self.TextBlock.FontSize = (double)e.NewValue;
    }

    /// <summary>Provides the OnFontStyleChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (FontIcon)d;
        if (self.TextBlock is null)
        {
            return;
        }

        self.TextBlock.FontStyle = (FontStyle)e.NewValue;
    }

    /// <summary>Provides the OnFontWeightChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (FontIcon)d;
        if (self.TextBlock is null)
        {
            return;
        }

        self.TextBlock.FontWeight = (FontWeight)e.NewValue;
    }

    /// <summary>Provides the OnGlyphChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (FontIcon)d;
        if (self.TextBlock is null)
        {
            return;
        }

        self.TextBlock.Text = (string)e.NewValue;
    }
}
