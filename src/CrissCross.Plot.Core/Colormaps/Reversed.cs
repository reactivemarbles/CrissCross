// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>Reverses the color order of another colormap.</summary>
/// <param name="cmap">The colormap whose colors are reversed.</param>
public class Reversed(IColormap cmap) : IColormap
{
    /// <summary>The colormap whose colors are reversed.</summary>
    private readonly IColormap _cmap = cmap;

    /// <summary>Gets the name of the reversed colormap.</summary>
    public string Name => $"{_cmap.Name} (Reversed)";

    /// <summary>Gets the color at the reversed normalized position.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The color at the corresponding reversed position.</returns>
    public Color GetColor(double position) => _cmap.GetColor(1.0 - position);
}
