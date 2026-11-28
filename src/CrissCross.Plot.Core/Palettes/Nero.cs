// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Nero color palette.</summary>
public class Nero : IPalette
{
    /// <summary>The hexadecimal color values in the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#013A20", "#478C5C", "#94C973", "#BACC81", "#CDD193"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Nero";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = string.Empty;

    /// <summary>Gets the colors in the palette.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index.</summary>
    /// <param name="index">The index of the color to retrieve.</param>
    /// <returns>The color at the specified index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
