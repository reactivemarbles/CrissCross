// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>A custom colormap created from a collection of colors.</summary>
/// <param name="colors">The colors used by the colormap.</param>
/// <param name="smooth">Whether to interpolate smoothly between colors.</param>
public class Custom(Color[] colors, bool smooth = true) : IColormap
{
    /// <summary>The colormap implementation selected by the constructor.</summary>
    private readonly IColormap _cmap = smooth ? new CustomInterpolated(colors) : new CustomPalette(colors);

    /// <summary>Gets the name of the colormap.</summary>
    public string Name => _cmap.Name;

    /// <summary>Gets the color at the specified normalized position.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The color at the requested position.</returns>
    public Color GetColor(double position) => _cmap.GetColor(position);
}
