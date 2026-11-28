// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Palettes;

/// <summary>Inverted version of <see cref="ColorblindFriendly"/> which is useful for plots with dark backgrounds.</summary>
public class ColorblindFriendlyDark : IPalette
{
    /// <summary>The inverted colorblind-friendly palette.</summary>
    private readonly IPalette _palette = new ColorblindFriendly().Inverted();

    /// <summary>Gets the palette name.</summary>
    public string Name => _palette.Name;

    /// <summary>Gets the palette description.</summary>
    public string Description => _palette.Description;

    /// <summary>Gets the colors in the palette.</summary>
    public Color[] Colors => _palette.Colors;

    /// <summary>Gets a color by its palette index.</summary>
    /// <param name="index">The index of the requested color.</param>
    /// <returns>The color at the specified index.</returns>
    public Color GetColor(int index) => _palette.GetColor(index);
}
