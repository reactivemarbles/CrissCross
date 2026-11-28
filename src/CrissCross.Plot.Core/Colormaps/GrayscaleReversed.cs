// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Colormaps;

/// <summary>Provides a grayscale colormap that transitions from white to black.</summary>
public class GrayscaleReversed : IColormap
{
    /// <summary>The greatest value representable by a byte.</summary>
    private const byte MaximumByteValue = byte.MaxValue;

    /// <summary>Gets the name of the colormap.</summary>
    public string Name => "Grayscale Reversed";

    /// <summary>Gets the grayscale color at the specified normalized position.</summary>
    /// <param name="position">The position within the colormap.</param>
    /// <returns>The grayscale color at the requested position.</returns>
    public Color GetColor(double position)
    {
        position = NumericConversion.Clamp(position, 0, 1);
        byte value = (byte)(MaximumByteValue - (byte)(MaximumByteValue * position));
        return Color.Gray(value);
    }
}
