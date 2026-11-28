// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>A custom palette which smoothly blends across a collection of colors using linear interpolation.</summary>
public class CustomInterpolated : IColormap
{
    /// <summary>The colors used to interpolate the palette.</summary>
    private readonly Color[] _colors;

    /// <summary>Initializes a new instance of the <see cref="CustomInterpolated"/> class.</summary>
    /// <param name="colors">The colors used by the palette.</param>
    public CustomInterpolated(Color[] colors)
    {
        if (colors.Length == 0)
        {
            // user supplied no colors so make it transparent
            _colors = [CrissCross.Plot.Colors.Transparent, CrissCross.Plot.Colors.Transparent];
        }
        else if (colors.Length == 1)
        {
            // user supplied a single color to place it twice so interpolation doesn't crash
            _colors = [colors[0], colors[0]];
        }
        else
        {
            _colors = colors;
        }
    }

    /// <summary>Gets the name of the colormap.</summary>
    public string Name => "Interpolated";

    /// <summary>Gets the interpolated color at the specified normalized position.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The color at the requested position.</returns>
    public Color GetColor(double position)
    {
        if (position <= 0)
        {
            return _colors[0];
        }

        if (position >= 1)
        {
            return _colors[^1];
        }

        int rangeCount = _colors.Length - 1;
        double rangeSize = 1.0 / rangeCount;

        int firstColorIndex = (int)(position / rangeSize);
        double positionInRange = (position - (rangeSize * firstColorIndex)) / rangeSize;

        Color color1 = _colors[firstColorIndex];
        Color color2 = _colors[firstColorIndex + 1];

        return color1.MixedWith(color2, positionInRange);
    }
}
