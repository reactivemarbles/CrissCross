// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.MultiplotLayouts;

/// <summary>Arranges subplots in a fixed number of rows and columns.</summary>
/// <param name="rows">The number of rows in the grid.</param>
/// <param name="columns">The number of columns in the grid.</param>
public class Grid(int rows, int columns) : IMultiplotLayout
{
    /// <summary>Gets the subplot rectangles arranged in row-major order.</summary>
    /// <param name="subplots">The subplots to arrange.</param>
    /// <param name="figureRect">The rectangle available to the multiplot.</param>
    /// <returns>The subplot rectangles.</returns>
    public PixelRect[] GetSubplotRectangles(SubplotCollection subplots, PixelRect figureRect)
    {
        PixelRect[] rectangles = new PixelRect[subplots.Count];

        double fractionPerRow = 1.0 / rows;
        double fractionPerColumn = 1.0 / columns;

        for (int i = 0; i < subplots.Count; i++)
        {
            int rowIndex = i / columns;
            int columnIndex = i % columns;
            FractionRect fr = new(fractionPerColumn * columnIndex, fractionPerRow * rowIndex, fractionPerColumn, fractionPerRow);
            rectangles[i] = fr.GetPixelRect(figureRect);
        }

        return rectangles;
    }
}
