// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Defines an edge crossing within a rectangular coordinate grid.</summary>
public interface IEdge
{
    /// <summary>Gets the first grid vertex associated with this edge.</summary>
    Vertex First { get; }

    /// <summary>Interpolates this edge at the specified Z coordinate.</summary>
    /// <param name="coordinateGrid">The rectangular grid of three-dimensional coordinates.</param>
    /// <param name="z">The Z coordinate at which to interpolate.</param>
    /// <returns>The interpolated two-dimensional coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The rectangular grid parameter is part of the established public interpolation API.")]
    Coordinates Interpolate(Coordinates3d[, ] coordinateGrid, double z);
}
