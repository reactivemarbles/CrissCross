// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents data points with distinct horizontal and vertical coordinates.</summary>
public interface IScatterSource
{
    /// <summary>Gets or sets the first point index to render.</summary>
    int MinRenderIndex { get; set; }

    /// <summary>Gets or sets the final point index to render.</summary>
    int MaxRenderIndex { get; set; }

    /// <summary>Gets a copy of the data as coordinates.</summary>
    /// <returns>The scatter points in source order.</returns>
    IReadOnlyList<Coordinates> GetScatterPoints();

    /// <summary>Gets the point nearest a coordinate using pixel scaling from a prior render.</summary>
    /// <param name="location">The coordinate to search around.</param>
    /// <param name="renderInfo">The render details used to calculate pixel distances.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest point, or <see cref="DataPoint.None"/> when it exceeds the maximum distance.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The existing optional parameter is retained for compatibility with callers.")]
    DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15);

    /// <summary>Gets the point nearest a horizontal coordinate using pixel scaling from a prior render.</summary>
    /// <param name="location">The coordinate whose horizontal component is searched.</param>
    /// <param name="renderInfo">The render details used to calculate pixel distances.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest point, or <see cref="DataPoint.None"/> when it exceeds the maximum distance.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The existing optional parameter is retained for compatibility with callers.")]
    DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15);

    /// <summary>Gets the horizontal coordinate limits of the data.</summary>
    /// <returns>The horizontal limits.</returns>
    CoordinateRange GetLimitsX();

    /// <summary>Gets the vertical coordinate limits of the data.</summary>
    /// <returns>The vertical limits.</returns>
    CoordinateRange GetLimitsY();

    /// <summary>Gets the combined coordinate limits of the data.</summary>
    /// <returns>The combined axis limits.</returns>
    AxisLimits GetLimits();
}
