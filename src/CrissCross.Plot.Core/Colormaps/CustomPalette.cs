// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>A palette of colors with "hard edges" (no interpolation between colors).</summary>
public class CustomPalette : IColormap
{
    /// <summary>The colors in this palette.</summary>
    private readonly Color[] _colors;

    /// <summary>Initializes a new instance of the <see cref="CustomPalette"/> class.</summary>
    /// <param name="colors">The colors in the palette.</param>
    /// <param name="name">The name of the palette.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional palette name is part of the existing public API.")]
    public CustomPalette(Color[] colors, string name = "custom")
    {
        _colors = colors;
        Name = name;
    }

    /// <summary>Gets the name of the palette.</summary>
    public string Name { get; }

    /// <summary>Gets the palette color at the specified normalized position.</summary>
    /// <param name="position">The position within the palette.</param>
    /// <returns>The color at the requested position.</returns>
    public Color GetColor(double position)
    {
        position = NumericConversion.Clamp(position, 0, 1);
        int index = (int)((_colors.Length - 1) * position);
        return _colors[index];
    }
}
