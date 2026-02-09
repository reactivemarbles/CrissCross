// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an icon that uses a glyph from the specified font.
/// </summary>
public class FontIcon : IconElement
{
    /// <summary>
    /// Property for <see cref="FontFamily"/>.
    /// </summary>
    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        AvaloniaProperty.Register<FontIcon, FontFamily>(nameof(FontFamily), FontFamily.Default);

    /// <summary>
    /// Property for <see cref="FontSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<FontIcon, double>(nameof(FontSize), 20.0);

    /// <summary>
    /// Property for <see cref="FontStyle"/>.
    /// </summary>
    public static readonly StyledProperty<FontStyle> FontStyleProperty =
        AvaloniaProperty.Register<FontIcon, FontStyle>(nameof(FontStyle), FontStyle.Normal);

    /// <summary>
    /// Property for <see cref="FontWeight"/>.
    /// </summary>
    public static readonly StyledProperty<FontWeight> FontWeightProperty =
        AvaloniaProperty.Register<FontIcon, FontWeight>(nameof(FontWeight), FontWeight.Normal);

    /// <summary>
    /// Property for <see cref="Glyph"/>.
    /// </summary>
    public static readonly StyledProperty<string?> GlyphProperty =
        AvaloniaProperty.Register<FontIcon, string?>(nameof(Glyph), string.Empty);

    private TextBlock? _textBlock;

    static FontIcon()
    {
        AffectsRender<FontIcon>(FontFamilyProperty, FontSizeProperty, FontStyleProperty, FontWeightProperty, GlyphProperty);
    }

    /// <summary>
    /// Gets or sets the font family used to display text.
    /// </summary>
    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size used to display text.
    /// </summary>
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the font style to apply to the text content.
    /// </summary>
    public FontStyle FontStyle
    {
        get => GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets the weight of the font used to display the control's text.
    /// </summary>
    public FontWeight FontWeight
    {
        get => GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the character code that identifies the icon glyph.
    /// </summary>
    /// <returns>The hexadecimal character code for the icon glyph.</returns>
    public string? Glyph
    {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        base.Render(context);

        if (string.IsNullOrEmpty(Glyph))
        {
            return;
        }

        var typeface = new Typeface(FontFamily, FontStyle, FontWeight);
        var formattedText = new FormattedText(
            Glyph,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            Foreground);

        var origin = new global::Avalonia.Point(
            (Bounds.Width - formattedText.Width) / 2,
            (Bounds.Height - formattedText.Height) / 2);

        context.DrawText(formattedText, origin);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (string.IsNullOrEmpty(Glyph))
        {
            return default;
        }

        var typeface = new Typeface(FontFamily, FontStyle, FontWeight);
        var formattedText = new FormattedText(
            Glyph,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            Foreground);

        return new Size(
            Math.Max(formattedText.Width, FontSize),
            Math.Max(formattedText.Height, FontSize));
    }
}
