// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides a palette designed to be distinguishable by people with color vision deficiencies.</summary>
public class ColorblindFriendly : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#000000", "#E69F00", "#56B4E9", "#009E73", "#F0E442",
        "#0072B2", "#D55E00", "#CC79A7",
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Colorblind Friendly";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A set of 8 colorblind-friendly colors "
        + "from Bang Wong's Nature Methods paper https://www.nature.com/articles/nmeth.1618.pdf";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
