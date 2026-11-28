// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Pastel Wheel palette.</summary>
public class PastelWheel : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#f8c5c7", "#fadec3", "#fbf6c4",
        "#e1ecc8", "#d7e8cb", "#daebd7",
        "#d9eef3", "#cadbed", "#c7d2e6",
        "#d4d1e5", "#e8d3e6", "#f8c7de"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Pastel wheel";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A 12-color palette by Arthurits created by lightening the color wheel";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
