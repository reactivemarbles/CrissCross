// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>Wraps a colormap and inverts the colors it produces.</summary>
/// <param name="original">The colormap whose colors are inverted.</param>
internal sealed class Inverted(IColormap original) : IColormap
{
    /// <summary>The original colormap.</summary>
    private readonly IColormap _original = original;

    /// <summary>Gets the name of the inverted colormap.</summary>
    public string Name => $"{_original.Name} (inverted)";

    /// <summary>Gets the inverted color at a position in the colormap.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The inverted color.</returns>
    public Color GetColor(double position) => _original.GetColor(position).Inverted();
}
