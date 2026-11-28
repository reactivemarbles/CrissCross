// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Palettes;

/// <summary>Provides a palette constructed from caller-supplied colors.</summary>
public class Custom : IPalette
{
    /// <summary>Initializes a new instance of the <see cref="Custom"/> class from color values.</summary>
    /// <param name="colors">The colors in the palette.</param>
    /// <param name="name">The optional palette name.</param>
    /// <param name="description">The optional palette description.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional description parameter is retained for source and binary compatibility.")]
    public Custom(IEnumerable<Color> colors, string? name, string? description = null)
    {
        Colors = [.. colors];
        Name = name ?? "unnamed";
        Description = description ?? "no description";
    }

    /// <summary>Initializes a new instance of the <see cref="Custom"/> class from hex color values.</summary>
    /// <param name="hex">The hex color values.</param>
    /// <param name="name">The optional palette name.</param>
    /// <param name="description">The optional palette description.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional parameters are retained for source and binary compatibility.")]
    public Custom(string[] hex, string? name = null, string? description = null)
        : this(Color.FromHex(hex), name, description)
    {
    }

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; }

    /// <summary>Gets the palette name.</summary>
    public string Name { get; }

    /// <summary>Gets the palette description.</summary>
    public string Description { get; }

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
