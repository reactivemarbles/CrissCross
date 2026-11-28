// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Tsitsulin 25-color palette.</summary>
public class Tsitsulin : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#ebac23", "#b80058", "#008cf9", "#006e00", "#00bbad",
        "#d163e6", "#b24502", "#ff9287", "#5954d6", "#00c6f8",
        "#878500", "#00a76c",
        "#f6da9c", "#ff5caa", "#8accff", "#4bff4b", "#6efff4",
        "#edc1f5", "#feae7c", "#ffc8c3", "#bdbbef", "#bdf2ff",
        "#fffc43", "#65ffc8",
        "#aaaaaa",
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Xgfs 25";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "A 25-color palette by Arthurits "
        + "adapted from Tsitsulin's 12-color xgfs palette: http://tsitsul.in/blog/coloropt";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
