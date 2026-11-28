// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides vector-field data and the limits used to render it.</summary>
public interface IVectorFieldSource
{
    /// <summary>Gets or sets the first vector index to render.</summary>
    int MinRenderIndex { get; set; }

    /// <summary>Gets or sets the final vector index to render.</summary>
    int MaxRenderIndex { get; set; }

    /// <summary>Gets the rooted coordinate vectors.</summary>
    /// <returns>The vectors in source order.</returns>
    IReadOnlyList<RootedCoordinateVector> GetRootedVectors();

    /// <summary>Gets the vector nearest a coordinate using pixel scaling from a prior render.</summary>
    /// <param name="location">The coordinate to search around.</param>
    /// <param name="renderInfo">The render details used to calculate pixel distances.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest vector point, or <see cref="DataPoint.None"/> when it exceeds the maximum distance.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The existing optional parameter is retained for compatibility with callers.")]
    DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15);

    /// <summary>Gets the horizontal coordinate limits of the vectors.</summary>
    /// <returns>The horizontal limits.</returns>
    CoordinateRange GetLimitsX();

    /// <summary>Gets the vertical coordinate limits of the vectors.</summary>
    /// <returns>The vertical limits.</returns>
    CoordinateRange GetLimitsY();

    /// <summary>Gets the combined coordinate limits of the vectors.</summary>
    /// <returns>The combined axis limits.</returns>
    AxisLimits GetLimits();
}
