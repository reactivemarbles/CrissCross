// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extended <see cref="System.Windows.Controls.TextBlock"/> with additional parameters like <see cref="FontTypography"/>.
/// </summary>
public class TextBlock : System.Windows.Controls.TextBlock
{
    /// <summary>
    /// Property for <see cref="FontTypography"/>.
    /// </summary>
    public static readonly DependencyProperty FontTypographyProperty = DependencyProperty.Register(
        nameof(FontTypography),
        typeof(FontTypography),
        typeof(TextBlock),
        new PropertyMetadata(
            FontTypography.Body,
            static (o, args) => ((TextBlock)o).OnFontTypographyChanged((FontTypography)args.NewValue)));

    /// <summary>
    /// Property for <see cref="Appearance"/>.
    /// </summary>
    public static readonly DependencyProperty AppearanceProperty = DependencyProperty.Register(
        nameof(Appearance),
        typeof(TextColor),
        typeof(TextBlock),
        new PropertyMetadata(
            TextColor.Primary,
            static (o, args) => ((TextBlock)o).OnAppearanceChanged((TextColor)args.NewValue)));

    /// <summary>
    /// Gets or sets the font typography.
    /// </summary>
    /// <value>
    /// The font typography.
    /// </value>
    public FontTypography FontTypography
    {
        get => (FontTypography)GetValue(FontTypographyProperty);
        set => SetValue(FontTypographyProperty, value);
    }

    /// <summary>
    /// Gets or sets the appearance.
    /// </summary>
    /// <value>
    /// The appearance.
    /// </value>
    public TextColor Appearance
    {
        get => (TextColor)GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    private void OnFontTypographyChanged(FontTypography newTypography) =>
        SetResourceReference(StyleProperty, newTypography.ToResourceValue());

    private void OnAppearanceChanged(TextColor textColor) =>
        SetResourceReference(ForegroundProperty, textColor.ToResourceValue());
}
