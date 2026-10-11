// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Partitions tables at measured row boundaries for paper layout.</summary>
public partial class FormattedTextPresenter
{
    /// <summary>Flattens a table into indivisible groups of connected spanning rows.</summary>
    /// <param name="result">The measured document lines.</param>
    /// <param name="grid">The table control.</param>
    /// <param name="table">The table document positions.</param>
    /// <param name="start">The table's document start.</param>
    /// <param name="end">The formatter line's document end.</param>
    /// <param name="height">The formatter line height.</param>
    private static void AppendMeasuredTableRows(List<DocumentPageLayout.MeasuredLine> result, Grid grid, RichTextTable table, int start, int end, double height)
    {
        grid.Arrange(new(grid.DesiredSize));
        var remaining = height;
        var leading = Math.Max(0, height - grid.DesiredSize.Height);
        for (var row = 0; row < table.RowCount;)
        {
            var groupEnd = FindSpanningRowEnd(table, row);
            var groupHeight = row == 0 ? leading : 0;
            for (var measuredRow = row; measuredRow < groupEnd; measuredRow++)
            {
                groupHeight += grid.RowDefinitions[measuredRow].ActualHeight;
            }

            var last = groupEnd == table.RowCount;
            groupHeight = last ? Math.Max(0, remaining) : groupHeight;
            var rowStart = start + FindTableRowStart(table, row);
            var rowEnd = last ? end : start + FindTableRowStart(table, groupEnd);
            result.Add(new(rowStart, rowEnd, groupHeight));
            remaining -= groupHeight;
            row = groupEnd;
        }
    }

    /// <summary>Finds the end of a group that cannot split through vertically merged cells.</summary>
    /// <param name="table">The cell model.</param>
    /// <param name="firstRow">The first connected row.</param>
    /// <returns>The exclusive last connected row.</returns>
    private static int FindSpanningRowEnd(RichTextTable table, int firstRow)
    {
        var end = firstRow + 1;
        for (var row = firstRow; row < end; row++)
        {
            foreach (var cell in table.Cells)
            {
                if (cell.Row == row)
                {
                    end = Math.Max(end, Math.Min(table.RowCount, row + cell.RowSpan));
                }
            }
        }

        return end;
    }

    /// <summary>Finds the first body position of a table row.</summary>
    /// <param name="table">The table cell model.</param>
    /// <param name="row">The requested row.</param>
    /// <returns>The relative document position.</returns>
    private static int FindTableRowStart(RichTextTable table, int row)
    {
        foreach (var cell in table.Cells)
        {
            if (cell.Row == row)
            {
                return cell.StartIndex;
            }
        }

        return table.Cells.Count == 0 ? 0 : table.Cells[^1].StartIndex + table.Cells[^1].Length;
    }
}
