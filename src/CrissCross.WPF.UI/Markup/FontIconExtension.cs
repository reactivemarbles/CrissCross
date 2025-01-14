// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

namespace CrissCross.WPF.UI.Markup;

/// <summary>
/// Custom <see cref="MarkupExtension"/> which can provide <see cref="FontIcon"/>.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:Button
///     Appearance="Primary"
///     Content="WPF button with font icon"
///     Icon="{ui:FontIcon '&#x1F308;'}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:Button Icon="{ui:FontIcon '&amp;#x1F308;'}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:HyperlinkButton Icon="{ui:FontIcon '&amp;#x1F308;'}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:TitleBar Icon="{ui:FontIcon '&amp;#x1F308;'}" /&gt;
/// </code>
/// </example>
/// <remarks>
/// Initializes a new instance of the <see cref="FontIconExtension"/> class.
/// </remarks>
[ContentProperty(nameof(Glyph))]
[MarkupExtensionReturnType(typeof(FontIcon))]
public class FontIconExtension : MarkupExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontIconExtension"/> class.
    /// </summary>
    public FontIconExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontIconExtension"/> class.
    /// </summary>
    /// <param name="glyph">The glyph.</param>
    public FontIconExtension(string glyph) => Glyph = glyph;

    /// <summary>
    /// Gets or sets the glyph.
    /// </summary>
    /// <value>
    /// The glyph.
    /// </value>
    [ConstructorArgument("glyph")]
    public string? Glyph { get; set; }

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    /// <value>
    /// The font family.
    /// </value>
    [ConstructorArgument("fontFamily")]
    public FontFamily FontFamily { get; set; } = new("FluentSystemIcons");

    /// <summary>
    /// Gets or sets the size of the font.
    /// </summary>
    /// <value>
    /// The size of the font.
    /// </value>
    public double FontSize { get; set; }

    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        FontIcon fontIcon = new() { Glyph = Glyph, FontFamily = FontFamily };

        if (FontSize > 0)
        {
            fontIcon.FontSize = FontSize;
        }

        return fontIcon;
    }
}
