// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides a function whose y-range can be calculated for an x-range.</summary>
public interface IFunctionSource
{
    /// <summary>Gets or sets the x-coordinate range used by the source.</summary>
    CoordinateRange RangeX { get; set; }

    /// <summary>Gets the y-coordinate range corresponding to an x-coordinate range.</summary>
    /// <param name="rangeX">The x-coordinate range to evaluate.</param>
    /// <returns>The y-coordinate range produced over the specified x-range.</returns>
    CoordinateRange GetRangeY(CoordinateRange rangeX);

    /// <summary>Evaluates the function at the specified x-coordinate.</summary>
    /// <param name="x">The x-coordinate to evaluate.</param>
    /// <returns>The function value at <paramref name="x"/>.</returns>
    double Get(double x);
}
