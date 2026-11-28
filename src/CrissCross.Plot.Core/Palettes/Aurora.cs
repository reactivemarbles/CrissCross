// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Aurora palette.</summary>
public class Aurora : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#BF616A", "#D08770", "#EBCB8B", "#A3BE8C", "#B48EAD",
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Aurora";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "From the Nord "
        + "collection of palettes: https://github.com/arcticicestudio/nord";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
