// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Dark Pastel palette.</summary>
public class DarkPastel : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#66c2a5", "#fc8d62", "#8da0cb", "#e78ac3", "#a6d854",
        "#ffd92f", "#e5c494", "#b3b3b3",
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Dark Pastel";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A qualitative 8-color palette generated using colorbrewer2.org";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
