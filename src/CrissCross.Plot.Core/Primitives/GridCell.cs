// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a single cell in a rectangular grid.</summary>
public class GridCell
{
    /// <summary>The cell bounds expressed as fractions of the grid dimensions.</summary>
    private readonly FractionRect _fractionRect;

    /// <summary>Initializes a new instance of the <see cref="GridCell"/> class.</summary>
    /// <param name="rowIndex">The zero-based index of the cell's row.</param>
    /// <param name="colIndex">The zero-based index of the cell's column.</param>
    /// <param name="rowCount">The number of rows in the grid.</param>
    /// <param name="colCount">The number of columns in the grid.</param>
    /// <param name="rowSpan">The number of rows occupied by the cell.</param>
    /// <param name="colSpan">The number of columns occupied by the cell.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional span defaults are part of the existing public constructor API.")]
    public GridCell(int rowIndex, int colIndex, int rowCount, int colCount, int rowSpan = 1, int colSpan = 1)
    {
        double unitWidth = 1.0 / colCount;
        double unitHeight = 1.0 / rowCount;
        double horizontalOffset = unitWidth * colIndex;
        double verticalOffset = unitHeight * rowIndex;
        _fractionRect = new(horizontalOffset, verticalOffset, unitWidth * colSpan, unitHeight * rowSpan);
    }

    /// <summary>Gets the rectangle for this cell within the specified figure rectangle.</summary>
    /// <param name="figureRect">The rectangle containing the entire grid.</param>
    /// <returns>The pixel rectangle occupied by this cell.</returns>
    public PixelRect GetRect(PixelRect figureRect) => _fractionRect.GetPixelRect(figureRect.Width, figureRect.Height);
}
