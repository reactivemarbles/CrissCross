// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>Wraps a colormap and inverts the hue of its colors.</summary>
/// <param name="original">The colormap whose color hues are inverted.</param>
internal sealed class InvertedHue(IColormap original) : IColormap
{
    /// <summary>The original colormap.</summary>
    private readonly IColormap _original = original;

    /// <summary>Gets the name of the hue-inverted colormap.</summary>
    public string Name => $"{_original.Name} (inverted hue)";

    /// <summary>Gets the hue-inverted color at a position in the colormap.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The hue-inverted color.</returns>
    public Color GetColor(double position) => _original.GetColor(position).InvertedHue();
}
