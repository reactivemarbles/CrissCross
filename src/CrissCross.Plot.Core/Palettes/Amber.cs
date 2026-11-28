// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Amber sequential color palette.</summary>
public class Amber : IPalette
{
    /// <summary>Contains the hex values used to create the palette colors.</summary>
    private static readonly string[] _hexColors =
    {
        "#FF6F00", "#FF8F00", "#FFA000", "#FFB300", "#FFC107"
    };

    /// <summary>Gets the name of the palette.</summary>
    public string Name { get; } = "Amber";

    /// <summary>Gets the description of the palette.</summary>
    public string Description { get; } = string.Empty;

    /// <summary>Gets the colors in the palette.</summary>
    public Color[] Colors { get; } = Color.FromHex(_hexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
