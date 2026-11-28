// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.PathStrategies;

/// <summary>
/// Connect points with curved lines which ease into and out of the midpoint between each pair.
/// This strategy does not "overshoot" points in the Y direction.
/// </summary>
public class QuadHalfPoint : IPathStrategy
{
    /// <summary>The divisor used to calculate the midpoint between two coordinates.</summary>
    private const float MidpointDivisor = 2;

    /// <summary>Creates a path of quadratic curves that pass through each segment midpoint.</summary>
    /// <param name="pixels">The points connected by the path; NaN points separate segments.</param>
    /// <returns>The generated path.</returns>
    public SKPath GetPath(IEnumerable<Pixel> pixels)
    {
        using SKPathBuilder path = new();

        bool moveToNextPoint = true;
        SKPoint lastPoint = default;

        foreach (var pixel in pixels)
        {
            if (float.IsNaN(pixel.X) || float.IsNaN(pixel.Y))
            {
                moveToNextPoint = true;
                continue;
            }

            SKPoint thisPoint = pixel.ToSKPoint();

            if (moveToNextPoint)
            {
                path.MoveTo(thisPoint);
                moveToNextPoint = false;
            }
            else
            {
                float halfX = (lastPoint.X + thisPoint.X) / MidpointDivisor;
                float halfY = (lastPoint.Y + thisPoint.Y) / MidpointDivisor;
                SKPoint halfPoint = new(halfX, halfY);

                SKPoint controlPoint1 = new(halfPoint.X, lastPoint.Y);
                SKPoint controlPoint2 = new(halfPoint.X, thisPoint.Y);

                path.QuadTo(controlPoint1, halfPoint);
                path.QuadTo(controlPoint2, thisPoint);
            }

            lastPoint = thisPoint;
        }

        return path.Detach();
    }
}
