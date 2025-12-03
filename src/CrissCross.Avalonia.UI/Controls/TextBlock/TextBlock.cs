// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Extended <see cref="Avalonia.Controls.TextBlock"/> with additional parameters like <see cref="FontTypography"/>.
/// </summary>
public class TextBlock : global::Avalonia.Controls.TextBlock
{
    /// <summary>
    /// Property for <see cref="FontTypography"/>.
    /// </summary>
    public static readonly StyledProperty<FontTypography> FontTypographyProperty = AvaloniaProperty.Register<TextBlock, FontTypography>(
        nameof(FontTypography), FontTypography.Body);

    /// <summary>
    /// Property for <see cref="Appearance"/>.
    /// </summary>
    public static readonly StyledProperty<TextColor> AppearanceProperty = AvaloniaProperty.Register<TextBlock, TextColor>(
        nameof(Appearance), TextColor.Primary);

    /// <summary>
    /// Gets or sets the font typography.
    /// </summary>
    public FontTypography FontTypography
    {
        get => GetValue(FontTypographyProperty);
        set => SetValue(FontTypographyProperty, value);
    }

    /// <summary>
    /// Gets or sets the appearance.
    /// </summary>
    public TextColor Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }
}
