// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>Maps positions to shades of gray.</summary>
public class Grayscale : IColormap
{
    /// <summary>The maximum channel value for an 8-bit grayscale color.</summary>
    private const byte MaximumChannelValue = 255;

    /// <summary>Gets the name of this colormap.</summary>
    public string Name => "Grayscale";

    /// <summary>Gets the grayscale color at a position in the colormap.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The grayscale color at the specified position.</returns>
    public Color GetColor(double position)
    {
        position = NumericConversion.Clamp(position, 0, 1);
        byte value = (byte)(MaximumChannelValue * position);
        return Color.Gray(value);
    }
}
