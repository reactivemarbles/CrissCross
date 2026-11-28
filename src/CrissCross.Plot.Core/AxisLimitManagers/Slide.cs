// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisLimitManagers;

/// <summary>Slides the view to the right to keep the newest data points in view.</summary>
public class Slide : IAxisLimitManager
{
    /// <summary>
    /// Gets or sets the fraction of the display width added as whitespace to the right of overflowing data.
    /// A value of 0 slides whenever new data is added; a value of 1 slides only when data moves off-screen.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public double PaddingFractionX;

    /// <summary>
    /// Gets or sets the fraction of the data span added as whitespace above or below overflowing data.
    /// A value of 0 tightly fits the data height; a value of 1 doubles the span in the direction of overflow.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public double PaddingFractionY = .5;

    /// <summary>Gets or sets the amount of horizontal area to display, in axis units.</summary>
    public double Width { get; set; } = 1000;

    /// <summary>Gets the horizontal range, sliding the view when data extends beyond it.</summary>
    /// <param name="viewRangeX">The current horizontal view range.</param>
    /// <param name="dataRangeX">The horizontal range occupied by the data.</param>
    /// <returns>The horizontal range to display.</returns>
    public CoordinateRange GetRangeX(CoordinateRange viewRangeX, CoordinateRange dataRangeX)
    {
        double padHorizontal = Width * PaddingFractionX;

        bool horizontalOverflow = dataRangeX.Max > viewRangeX.Max || dataRangeX.Max < viewRangeX.Min;
        double maximumX = horizontalOverflow ? dataRangeX.Max + padHorizontal : viewRangeX.Max;
        double minimumX = horizontalOverflow ? maximumX - Width : viewRangeX.Min;

        return new(minimumX, maximumX);
    }

    /// <summary>Gets the vertical range, adding whitespace when data extends beyond it.</summary>
    /// <param name="viewRangeY">The current vertical view range.</param>
    /// <param name="dataRangeY">The vertical range occupied by the data.</param>
    /// <returns>The vertical range to display.</returns>
    public CoordinateRange GetRangeY(CoordinateRange viewRangeY, CoordinateRange dataRangeY)
    {
        double padVertical = viewRangeY.Span * PaddingFractionY;

        bool verticalOverflow = dataRangeY.Min < viewRangeY.Min || dataRangeY.Max > viewRangeY.Max;
        double minimumY = verticalOverflow ? dataRangeY.Min - padVertical : viewRangeY.Min;
        double maximumY = verticalOverflow ? dataRangeY.Max + padVertical : viewRangeY.Max;

        return new(minimumY, maximumY);
    }
}
