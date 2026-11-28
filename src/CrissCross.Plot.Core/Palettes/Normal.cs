// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Xgfs Normal 6 palette.</summary>
public class Normal : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#4053d3", "#ddb310", "#b51d14",
        "#00beff", "#fb49b0", "#00b25d", "#cacaca",
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Xgfs Normal 6";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A color palette adapted from "
        + "Tsitsulin's 6-color normal xgfs palette: http://tsitsul.in/blog/coloropt";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
