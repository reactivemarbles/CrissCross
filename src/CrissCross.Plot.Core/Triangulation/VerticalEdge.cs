// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Represents a vertical edge in a rectangular coordinate grid.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The rectangular grid parameter is part of the established public interpolation API.")]
public class VerticalEdge : IEdge
{
    /// <summary>Initializes a new instance of the <see cref="VerticalEdge"/> class.</summary>
    /// <param name="first">The grid vertex at the start of this edge.</param>
    public VerticalEdge(Vertex first)
    {
        First = first;
    }

    /// <summary>Gets the first grid vertex associated with this edge.</summary>
    public Vertex First { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not VerticalEdge other)
        {
            return false;
        }

        return First.i == other.First.i && First.j == other.First.j;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 23) + First.i.GetHashCode();
            hash = (hash * 23) + First.j.GetHashCode();
            return hash;
        }
    }

    /// <inheritdoc/>
    public Coordinates Interpolate(Coordinates3d[, ] coordinateGrid, double z)
    {
        Coordinates3d first = coordinateGrid[First.j, First.i];
        Coordinates3d second = coordinateGrid[First.j + 1, First.i];
        var y = first.Y + ((second.Y - first.Y) / (second.Z - first.Z) * (z - first.Z));
        return new(first.X, y);
    }
}
