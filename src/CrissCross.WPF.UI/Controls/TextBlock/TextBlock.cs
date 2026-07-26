// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Extensions;
#else
using CrissCross.WPF.UI.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extended TextBlock with additional parameters like FontTypography.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class TextBlock : System.Windows.Controls.TextBlock
{
    /// <summary>Property for <see cref="FontTypography"/>.</summary>
    public static readonly DependencyProperty FontTypographyProperty = DependencyProperty.Register(
        nameof(FontTypography),
        typeof(FontTypography),
        typeof(TextBlock),
        new(
            FontTypography.Body,
            static (o, args) => ((TextBlock)o).OnFontTypographyChanged((FontTypography)args.NewValue)));

    /// <summary>Property for <see cref="Appearance"/>.</summary>
    public static readonly DependencyProperty AppearanceProperty = DependencyProperty.Register(
        nameof(Appearance),
        typeof(TextColor),
        typeof(TextBlock),
        new(
            TextColor.Primary,
            static (o, args) => ((TextBlock)o).OnAppearanceChanged((TextColor)args.NewValue)));

    /// <summary>Gets or sets the font typography.</summary>
    /// <value>
    /// The font typography.
    /// </value>
    public FontTypography FontTypography
    {
        get => (FontTypography)GetValue(FontTypographyProperty);
        set => SetValue(FontTypographyProperty, value);
    }

    /// <summary>Gets or sets the appearance.</summary>
    /// <value>
    /// The appearance.
    /// </value>
    public TextColor Appearance
    {
        get => (TextColor)GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Provides the OnFontTypographyChanged member.</summary>
    /// <param name="newTypography">The newTypography value.</param>
    private void OnFontTypographyChanged(FontTypography newTypography) =>
        SetResourceReference(StyleProperty, newTypography.ToResourceValue());

    /// <summary>Provides the OnAppearanceChanged member.</summary>
    /// <param name="textColor">The textColor value.</param>
    private void OnAppearanceChanged(TextColor textColor) =>
        SetResourceReference(ForegroundProperty, textColor.ToResourceValue());
}
