// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Converts font settings to their SkiaSharp equivalents.</summary>
public static class FontExtensions
{
    /// <summary>Extension members for <see cref="FontSlant"/>.</summary>
    /// <param name="slant">The font slant to convert.</param>
    extension(FontSlant slant)
    {
        /// <summary>Converts this slant to a SkiaSharp font-style slant.</summary>
        /// <returns>The SkiaSharp equivalent of this slant.</returns>
        public SKFontStyleSlant ToSKFontStyleSlant() => slant switch
        {
            FontSlant.Upright => SKFontStyleSlant.Upright,
            FontSlant.Italic => SKFontStyleSlant.Italic,
            FontSlant.Oblique => SKFontStyleSlant.Oblique,
            _ => throw new ArgumentOutOfRangeException(nameof(slant))
        };
    }

    /// <summary>Extension members for <see cref="FontSpacing"/>.</summary>
    /// <param name="width">The font spacing to convert.</param>
    extension(FontSpacing width)
    {
        /// <summary>Converts this spacing to a SkiaSharp font-style width.</summary>
        /// <returns>The SkiaSharp equivalent of this spacing.</returns>
        public SKFontStyleWidth ToSKFontStyleWidth() => width switch
        {
            FontSpacing.UltraCondensed => SKFontStyleWidth.UltraCondensed,
            FontSpacing.ExtraCondensed => SKFontStyleWidth.ExtraCondensed,
            FontSpacing.Condensed => SKFontStyleWidth.Condensed,
            FontSpacing.SemiCondensed => SKFontStyleWidth.SemiCondensed,
            FontSpacing.Normal => SKFontStyleWidth.Normal,
            FontSpacing.SemiExpanded => SKFontStyleWidth.SemiExpanded,
            FontSpacing.Expanded => SKFontStyleWidth.Expanded,
            FontSpacing.ExtraExpanded => SKFontStyleWidth.ExtraExpanded,
            FontSpacing.UltraExpanded => SKFontStyleWidth.UltraExpanded,
            _ => throw new ArgumentOutOfRangeException(nameof(width))
        };
    }

    /// <summary>Extension members for <see cref="FontWeight"/>.</summary>
    /// <param name="weight">The font weight to convert.</param>
    extension(FontWeight weight)
    {
        /// <summary>Converts this weight to a SkiaSharp font-style weight.</summary>
        /// <returns>The SkiaSharp equivalent of this weight.</returns>
        public SKFontStyleWeight ToSKFontStyleWeight() => weight switch
        {
            FontWeight.Invisible => SKFontStyleWeight.Invisible,
            FontWeight.Thin => SKFontStyleWeight.Thin,
            FontWeight.ExtraLight => SKFontStyleWeight.ExtraLight,
            FontWeight.Light => SKFontStyleWeight.Light,
            FontWeight.Normal => SKFontStyleWeight.Normal,
            FontWeight.Medium => SKFontStyleWeight.Medium,
            FontWeight.SemiBold => SKFontStyleWeight.SemiBold,
            FontWeight.Bold => SKFontStyleWeight.Bold,
            FontWeight.ExtraBold => SKFontStyleWeight.ExtraBold,
            FontWeight.Black => SKFontStyleWeight.Black,
            FontWeight.ExtraBlack => SKFontStyleWeight.ExtraBlack,
            _ => throw new ArgumentOutOfRangeException(nameof(weight))
        };
    }
}
