// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the One Half light palette.</summary>
public class OneHalf : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#383a42", "#e4564a", "#50a14f", "#c18402", "#0084bc", "#a626a4", "#0897b3"
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "One Half";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A Sublime color scheme "
        + "by Son A. Pham: https://github.com/sonph/onehalf";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
