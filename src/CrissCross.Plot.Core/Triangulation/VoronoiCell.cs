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

/// <summary>Represents a Voronoi cell.</summary>
/// <param name="Index">The source point index.</param>
/// <param name="Points">The cell vertices in 3D coordinates.</param>
public readonly record struct VoronoiCell(int Index, Coordinates3d[] Points)
{
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

            return Array.ConvertAll(Points, static point => point.Coordinates2d());
        }
    }

    /// <summary>Gets the closed 2D coordinate path.</summary>
    public CoordinatePath Path => CoordinatePath.Closed(Points2d);
}
