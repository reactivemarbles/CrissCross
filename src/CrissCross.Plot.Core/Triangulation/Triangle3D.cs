// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

/*
--- NOTICE BEGIN ---

MIT License

Copyright (c) 2019 Patryk Grech

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

--- NOTICE END ---
*/

namespace CrissCross.Plot.Triangulation;

/// <summary>Represents a triangulated 3D triangle.</summary>
/// <param name="Index">The triangle index.</param>
/// <param name="Points">The three triangle vertices.</param>
public readonly record struct Triangle3D(int Index, IEnumerable<Coordinates3d> Points)
{
    /// <summary>Gets the minimum Z coordinate.</summary>
    public double MinZ => GetZRange().Min;

    /// <summary>Gets the maximum Z coordinate.</summary>
    public double MaxZ => GetZRange().Max;

    /// <summary>Gets the vertices projected onto the XY plane.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1017",
        Justification = "The existing public property returns a new projected array on every access; changing that contract would break callers.")]
    public Coordinates[] Points2d
    {
        get
        {
            if (Points is null)
            {
                throw new ArgumentNullException("source");
            }

            var points = new List<Coordinates>();
            foreach (var point in Points)
            {
                points.Add(point.Coordinates2d());
            }

            return points.ToArray();
        }
    }

    /// <summary>Gets the closed 2D coordinate path.</summary>
    public CoordinatePath Path => CoordinatePath.Closed(Points2d);

    /// <summary>Calculates the minimum and maximum Z values for the triangle points.</summary>
    /// <returns>The minimum and maximum Z coordinates.</returns>
    private (double Min, double Max) GetZRange()
    {
        using IEnumerator<Coordinates3d> points = Points.GetEnumerator();
        if (!points.MoveNext())
        {
            throw new InvalidOperationException("Sequence contains no elements.");
        }

        double min = points.Current.Z;
        double max = points.Current.Z;
        while (points.MoveNext())
        {
            min = Math.Min(min, points.Current.Z);
            max = Math.Max(max, points.Current.Z);
        }

        return (min, max);
    }
}
