// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Light Spectrum color palette.</summary>
public class LightSpectrum : IPalette
{
    /// <summary>The hexadecimal color values in the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#fce5e6", "#fff8e7", "#fffce8",
        "#eff5e4", "#e7f2e6", "#ddf0f5",
        "#e6f2fc", "#e6eaf7", "#eee0f0"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Light spectrum";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A 9-color palette by Arthurits created by lightening the colors in the visible spectrum";

    /// <summary>Gets the colors in the palette.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index.</summary>
    /// <param name="index">The index of the color to retrieve.</param>
    /// <returns>The color at the specified index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
