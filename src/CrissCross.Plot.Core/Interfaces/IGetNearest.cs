// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot;

/// <summary>This interface is used by plottables that can locate a <see cref="DataPoint"/> within a specified range of a coordinate on the plot.</summary>
public interface IGetNearest
{
    /// <summary>
    /// Returns the point nearest a specific location given pixel scaling from a previous render.
    /// Returns <see cref="DataPoint.None"/> if the nearest point is farther away than <paramref name="maxDistance"/> pixels.
    /// </summary>
    /// <param name="mouseLocation">The location to search around.</param>
    /// <param name="renderInfo">The pixel scaling information from a previous render.</param>
    /// <param name="maxDistance">The maximum search distance in pixels.</param>
    /// <returns>The nearest point, or <see cref="DataPoint.None"/> if no point is within the maximum distance.</returns>
    DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance);

    /// <summary>
    /// Returns the point nearest the specified X location using pixel scaling from a previous render.
    /// Returns <see cref="DataPoint.None"/> if the nearest point is farther away than <paramref name="maxDistance"/> pixels.
    /// </summary>
    /// <param name="mouseLocation">The location whose X coordinate is searched.</param>
    /// <param name="renderInfo">The pixel scaling information from a previous render.</param>
    /// <param name="maxDistance">The maximum search distance in pixels.</param>
    /// <returns>The nearest point, or <see cref="DataPoint.None"/> if no point is within the maximum distance.</returns>
    DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance);
}
