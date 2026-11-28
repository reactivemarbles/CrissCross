// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.MultiplotLayouts;

/// <summary>Arranges plots in explicitly assigned grid cells.</summary>
public class CustomGrid : IMultiplotLayout
{
    /// <summary>The grid cell assigned to each plot.</summary>
    private readonly Dictionary<Plot, GridCell> _plotCells = [];

    /// <summary>Gets the subplot rectangles using their assigned grid cells.</summary>
    /// <param name="subplots">The subplots to arrange.</param>
    /// <param name="figureRect">The rectangle available to the multiplot.</param>
    /// <returns>The subplot rectangles.</returns>
    public PixelRect[] GetSubplotRectangles(SubplotCollection subplots, PixelRect figureRect)
    {
        PixelRect[] rectangles = new PixelRect[subplots.Count];

        Plot[] plots = subplots.GetPlots();

        for (int i = 0; i < plots.Length; i++)
        {
            if (_plotCells.TryGetValue(plots[i], out GridCell? cell))
            {
                rectangles[i] = cell.GetRect(figureRect);
            }
            else
            {
                rectangles[i] = PixelRect.NaN;
            }
        }

        return rectangles;
    }

    /// <summary>Assigns a plot to a grid cell.</summary>
    /// <param name="plot">The plot to arrange.</param>
    /// <param name="gridCell">The cell assigned to the plot.</param>
    public void Set(Plot plot, GridCell gridCell) => _plotCells[plot] = gridCell;
}
