// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Penumbra palette.</summary>
public class Penumbra : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#CB7459", "#A38F2D", "#46A473", "#00A0BE", "#7E87D6", "#BD72A8"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Penumbra";

    /// <summary>Gets the palette description.</summary>
    public string Description => "A perceptually uniform color palette "
        + "by Neal McKee: https://github.com/nealmckee/penumbra";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
