// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisLimitManagers;

/// <summary>Shows the entire data range, changing the axis limits when data extends beyond the current view.</summary>
public class Full : IAxisLimitManager
{
    /// <summary>
    /// Gets or sets the amount of whitespace added around data when it extends beyond the current view.
    /// A value of 0 adds no whitespace; a value of 1 adds whitespace equal to the data span.
    /// </summary>
    public double ExpansionRatio { get; set; } = .25;

    /// <summary>Gets the horizontal range, including the configured expansion when required.</summary>
    /// <param name="viewRangeX">The current horizontal view range.</param>
    /// <param name="dataRangeX">The horizontal range occupied by the data.</param>
    /// <returns>The horizontal range to display.</returns>
    public CoordinateRange GetRangeX(CoordinateRange viewRangeX, CoordinateRange dataRangeX)
    {
        bool rightEdgeIsTooClose = viewRangeX.Max < dataRangeX.Max;
        bool rightEdgeIsTooFar = viewRangeX.Max > dataRangeX.Max + dataRangeX.Span;
        bool replaceRight = rightEdgeIsTooClose || rightEdgeIsTooFar;
        double horizontalMaximum = replaceRight
            ? dataRangeX.Max + (dataRangeX.Span * ExpansionRatio)
            : viewRangeX.Max;

        return new(dataRangeX.Min, horizontalMaximum);
    }

    /// <summary>Gets the vertical range, including the configured expansion when required.</summary>
    /// <param name="viewRangeY">The current vertical view range.</param>
    /// <param name="dataRangeY">The vertical range occupied by the data.</param>
    /// <returns>The vertical range to display.</returns>
    public CoordinateRange GetRangeY(CoordinateRange viewRangeY, CoordinateRange dataRangeY)
    {
        bool topEdgeIsTooClose = viewRangeY.Max < dataRangeY.Max;
        bool topEdgeIsTooFar = viewRangeY.Max < dataRangeY.Max + dataRangeY.Span;
        bool replaceMax = topEdgeIsTooClose || topEdgeIsTooFar;
        double verticalMaximum = replaceMax
            ? dataRangeY.Max + (dataRangeY.Span * ExpansionRatio)
            : viewRangeY.Max;

        bool bottomEdgeIsTooClose = viewRangeY.Min > dataRangeY.Min;
        bool bottomEdgeIsTooFar = viewRangeY.Min > dataRangeY.Min - dataRangeY.Span;
        bool replaceMin = bottomEdgeIsTooClose || bottomEdgeIsTooFar;
        double verticalMinimum = replaceMin
            ? dataRangeY.Min - (dataRangeY.Span * ExpansionRatio)
            : viewRangeY.Min;

        return new(verticalMinimum, verticalMaximum);
    }
}
