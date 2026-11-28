// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisLimitManagers;

/// <summary>Expands the vertical axis when data extends beyond the current view.</summary>
public class FixedWidth : IAxisLimitManager
{
    /// <summary>Gets or sets the fraction of the data span used to expand the vertical axis.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public double ExpandFractionY = 0.5;

    /// <summary>Gets the horizontal range occupied by the data.</summary>
    /// <param name="viewRangeX">The current horizontal view range.</param>
    /// <param name="dataRangeX">The horizontal range occupied by the data.</param>
    /// <returns>The horizontal data range.</returns>
    public CoordinateRange GetRangeX(CoordinateRange viewRangeX, CoordinateRange dataRangeX) => dataRangeX;

    /// <summary>Gets the vertical range, expanding it when the data extends beyond the current view.</summary>
    /// <param name="viewRangeY">The current vertical view range.</param>
    /// <param name="dataRangeY">The vertical range occupied by the data.</param>
    /// <returns>The vertical range to display.</returns>
    public CoordinateRange GetRangeY(CoordinateRange viewRangeY, CoordinateRange dataRangeY)
    {
        double expandY = ExpandFractionY * dataRangeY.Span;

        double minimumY = (dataRangeY.Min < viewRangeY.Min) ? dataRangeY.Min - expandY : viewRangeY.Min;
        double maximumY = (dataRangeY.Max > viewRangeY.Max) ? dataRangeY.Max + expandY : viewRangeY.Max;

        return new(minimumY, maximumY);
    }
}
