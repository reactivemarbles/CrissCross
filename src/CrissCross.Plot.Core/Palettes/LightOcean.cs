// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Light Ocean palette.</summary>
public class LightOcean : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#dfedd9", "#dbecdc", "#dbede4",
        "#daeeec", "#daeef3", "#dae6f2",
        "#dadef1", "#dedaee", "#e5daed"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Light ocean";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A 9-color palette by Arthurits created by a mixture of light greens, blues, and purples";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
