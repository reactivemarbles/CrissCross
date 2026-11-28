// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>
/// A rainbow colormap inspired by Jet and Turbo but adapted by Scott Harden to have
/// less dark edges (red or purple) and a more mellow center (darker yellow/green)
/// for more even brightness perception when displayed using thin lines on a white background.
/// </summary>
public class MellowRainbow : IColormap
{
    /// <summary>The interpolated palette used to map positions to colors.</summary>
    private readonly CustomInterpolated _colormap;

    /// <summary>Initializes a new instance of the <see cref="MellowRainbow"/> class.</summary>
    public MellowRainbow()
    {
        Color[] colors =
        [
            new("#466be3"),
            new("#29bbec"),
            new("#30f199"),
            new("#edd03a"),
            new("#fb8023"),
            new("#d23104"),
        ];

        _colormap = new(colors);
    }

    /// <summary>Gets the colormap name.</summary>
    public string Name => "Mellow Rainbow";

    /// <summary>Gets the color at the specified position.</summary>
    /// <param name="position">The position in the palette.</param>
    /// <returns>The color at the specified position.</returns>
    public Color GetColor(double position) => _colormap.GetColor(position);
}
