// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.PathStrategies;

/// <summary>Connect points with straight lines. NaN values will be skipped, producing a gap in the path.</summary>
public class Straight : IPathStrategy
{
    /// <summary>Creates a path by connecting consecutive non-NaN pixels with straight lines.</summary>
    /// <param name="pixels">The pixels to connect.</param>
    /// <returns>A path containing the connected line segments.</returns>
    public SKPath GetPath(IEnumerable<Pixel> pixels)
    {
        using SKPathBuilder path = new();

        bool move = true;

        foreach (var pixel in pixels)
        {
            if (float.IsNaN(pixel.X) || float.IsNaN(pixel.Y))
            {
                move = true;
            }
            else if (move)
            {
                path.MoveTo(pixel.ToSKPoint());
                move = false;
            }
            else
            {
                path.LineTo(pixel.ToSKPoint());
            }
        }

        return path.Detach();
    }
}
