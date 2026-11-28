// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>An axis manager contains logic to suggest axis limits given the current view and size of the data.</summary>
public interface IAxisLimitManager
{
    /// <summary>Returns the recommended X axis range given the current view and size of the data.</summary>
    /// <param name="viewRangeX">The current X axis view range.</param>
    /// <param name="dataRangeX">The X axis range occupied by the data.</param>
    /// <returns>The recommended X axis range.</returns>
    CoordinateRange GetRangeX(CoordinateRange viewRangeX, CoordinateRange dataRangeX);

    /// <summary>Returns the recommended Y axis range given the current view and size of the data.</summary>
    /// <param name="viewRangeY">The current Y axis view range.</param>
    /// <param name="dataRangeY">The Y axis range occupied by the data.</param>
    /// <returns>The recommended Y axis range.</returns>
    CoordinateRange GetRangeY(CoordinateRange viewRangeY, CoordinateRange dataRangeY);
}
