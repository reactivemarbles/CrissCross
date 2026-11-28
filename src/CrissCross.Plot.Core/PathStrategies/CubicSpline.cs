// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.PathStrategies;

/// <summary>Connects points with Catmull-Rom cubic splines and skips NaN values to create path gaps.</summary>
public class CubicSpline : IPathStrategy
{
    /// <summary>The number of segments used to calculate each cubic control point.</summary>
    private const float ControlPointDivisor = 6.0F;

    /// <summary>The minimum number of padded points needed to form a spline segment.</summary>
    private const int MinimumPaddedPointCount = 4;

    /// <summary>The index offset from the end for the third-to-last point.</summary>
    private const int ThirdFromEndOffset = 3;

    /// <summary>The index offset from the end for the second-to-last point.</summary>
    private const int SecondFromEndOffset = 2;

    /// <summary>Gets or sets the spline tension.</summary>
    public double Tension { get; set; } = 1.0F;

    /// <summary>Creates a path through the supplied pixels using cubic splines.</summary>
    /// <param name="pixels">The points to connect; NaN points separate segments.</param>
    /// <returns>The path connecting the valid point segments.</returns>
    public SKPath GetPath(IEnumerable<Pixel> pixels)
    {
        using SKPathBuilder path = new();

        // Organize into segments of connected points
        foreach (var segment in GetSegments(pixels))
        {
            if (segment.Length < MinimumPaddedPointCount)
            {
                continue;
            }

            Pixel first = segment[1];
            Pixel second = segment[2];
            Pixel nextLast = segment[^ThirdFromEndOffset];
            Pixel last = segment[^SecondFromEndOffset];

            bool closed = first == last;

            // Fill in the empty padded points
            segment[0] = closed ? nextLast : first;
            segment[^1] = closed ? second : last;

            path.MoveTo(first.ToSKPoint());

            for (int i = 2; i < segment.Length - 1; ++i)
            {
                Pixel p0 = segment[i - SecondFromEndOffset];
                Pixel p1 = segment[i - 1];
                Pixel p2 = segment[i];
                Pixel p3 = segment[i + 1];
                Pixel c1 = p1 + ((p2 - p0) / (ControlPointDivisor * (float)Tension));
                Pixel c2 = p2 - ((p3 - p1) / (ControlPointDivisor * (float)Tension));

                path.CubicTo(c1.ToSKPoint(), c2.ToSKPoint(), p2.ToSKPoint());
            }
        }

        return path.Detach();
    }

    /// <summary>Organizes connected points into segments padded at the front and back.</summary>
    /// <param name="pixels">The points to organize.</param>
    /// <returns>Arrays representing the connected point segments.</returns>
    private static IEnumerable<Pixel[]> GetSegments(IEnumerable<Pixel> pixels)
    {
        // Reserve an empty element at the front
        List<Pixel> segment = [Pixel.NaN];

        foreach (var pixel in pixels)
        {
            if (float.IsNaN(pixel.X) || float.IsNaN(pixel.Y))
            {
                if (segment.Count > 1)
                {
                    // And one at the back
                    segment.Add(Pixel.NaN);
                    yield return segment.ToArray();

                    segment.Clear();
                    segment.Add(Pixel.NaN);
                }
            }
            else
            {
                segment.Add(pixel);
            }
        }

        if (segment.Count <= 1)
        {
            yield break;
        }

        segment.Add(Pixel.NaN);
        yield return segment.ToArray();
    }
}
