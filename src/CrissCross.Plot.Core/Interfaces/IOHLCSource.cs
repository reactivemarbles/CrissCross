// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides OHLC values and their coordinate limits.</summary>
public interface IOHLCSource
{
    /// <summary>Gets the number of OHLC values.</summary>
    int Count { get; }

    /// <summary>Gets all OHLC values.</summary>
    /// <returns>The OHLC values in source order.</returns>
    IReadOnlyList<OHLC> GetOHLCs();

    /// <summary>Gets the horizontal coordinate limits.</summary>
    /// <returns>The horizontal limits.</returns>
    CoordinateRange GetLimitsX();

    /// <summary>Gets the vertical coordinate limits.</summary>
    /// <returns>The vertical limits.</returns>
    CoordinateRange GetLimitsY();

    /// <summary>Gets the combined coordinate limits.</summary>
    /// <returns>The combined axis limits.</returns>
    AxisLimits GetLimits();

    /// <summary>Gets the price range for an inclusive index range.</summary>
    /// <param name="index1">The first OHLC index.</param>
    /// <param name="index2">The final OHLC index.</param>
    /// <returns>The minimum and maximum prices in the selected range.</returns>
    CoordinateRange GetPriceRange(int index1, int index2);
}
