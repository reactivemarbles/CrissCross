// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Nord palette.</summary>
public class Nord : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#bf616a", "#a3be8c", "#ebcb8b", "#81a1c1", "#b48ead", "#88c0d0", "#e5e9f0"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Nord";

    /// <summary>Gets the palette description.</summary>
    public string Description => "From the Nord "
        + "ConEmu color scheme: https://github.com/arcticicestudio/nord-conemu";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
