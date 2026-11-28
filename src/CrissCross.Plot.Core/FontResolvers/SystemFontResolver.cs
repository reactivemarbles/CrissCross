// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.FontResolvers;

/// <summary>Resolves typefaces from fonts installed on the current system.</summary>
public class SystemFontResolver : IFontResolver
{
    /// <summary>Gets the default system font family name.</summary>
    /// <returns>The installed default font family.</returns>
    public static string DefaultSystemFont() => SKTypeface.Default.FamilyName;

    /// <summary>Creates a typeface for the default system font.</summary>
    /// <returns>The default system typeface.</returns>
    public static SKTypeface CreateDefaultTypeface()
    {
        string fontName = DefaultSystemFont();
        SKTypeface? typeface = SKTypeface.FromFamilyName(fontName);
        return typeface is null
            ? throw new InvalidOperationException($"Unable to create typeface using the default system font ({fontName})")
            : typeface;
    }

    /// <summary>Creates a typeface using the specified style.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="weight">The requested font weight.</param>
    /// <param name="slant">The requested font slant.</param>
    /// <param name="spacing">The requested font width.</param>
    /// <returns>The matching typeface, or <see langword="null"/> when the font family is not installed.</returns>
    public SKTypeface? CreateTypeface(string fontName, FontWeight weight, FontSlant slant, FontSpacing spacing)
    {
        if (!SystemFontFamilyResolver.IsInstalled(fontName))
        {
            return null;
        }

        SKFontStyle style = new(weight.ToSKFontStyleWeight(), spacing.ToSKFontStyleWidth(), slant.ToSKFontStyleSlant());
        return SKTypeface.FromFamilyName(fontName, style);
    }

    /// <summary>Creates a typeface using legacy bold and italic flags.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="bold">Whether the font is bold.</param>
    /// <param name="italic">Whether the font is italic.</param>
    /// <returns>The matching typeface, or <see langword="null"/> when the font family is not installed.</returns>
    public SKTypeface? CreateTypeface(string fontName, bool bold, bool italic) => CreateTypeface(
            fontName,
            bold ? FontWeight.Bold : FontWeight.Normal,
            italic ? FontSlant.Italic : FontSlant.Upright,
            FontSpacing.Normal);

    /// <summary>Creates a typeface using legacy bold and italic flags with a specified width.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="bold">Whether the font is bold.</param>
    /// <param name="italic">Whether the font is italic.</param>
    /// <param name="width">The requested font width.</param>
    /// <returns>The matching typeface, or <see langword="null"/> when the font family is not installed.</returns>
    public SKTypeface? CreateTypeface(string fontName, bool bold, bool italic, FontSpacing width) => CreateTypeface(
            fontName,
            bold ? FontWeight.Bold : FontWeight.Normal,
            italic ? FontSlant.Italic : FontSlant.Upright,
            width);
}
