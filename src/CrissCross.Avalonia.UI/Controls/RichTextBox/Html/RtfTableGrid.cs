// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Places HTML table cells into a shared physical RTF column grid.</summary>
internal sealed class RtfTableGrid
{
    /// <summary>The maximum HTML span or physical column count.</summary>
    private const int MaximumSpan = 1_000;

    /// <summary>Gets the rows indexed by physical column.</summary>
    internal List<Dictionary<int, RtfTableCell>> Rows { get; } = [];

    /// <summary>Gets the number of physical columns.</summary>
    internal int ColumnCount { get; private set; }

    /// <summary>Creates a grid without including rows from descendant tables.</summary>
    /// <param name="table">The source table.</param>
    /// <returns>The physical grid.</returns>
    internal static RtfTableGrid Create(IElement table)
    {
        var sourceRows = ReadRows(table);
        var grid = new RtfTableGrid();
        for (var row = 0; row < sourceRows.Count; row++)
        {
            grid.Rows.Add([]);
        }

        for (var row = 0; row < sourceRows.Count; row++)
        {
            grid.PlaceRow(sourceRows[row], row);
        }

        return grid;
    }

    /// <summary>Reads the rows belonging to this table.</summary>
    /// <param name="table">The source table.</param>
    /// <returns>The direct rows in document order.</returns>
    private static List<IElement> ReadRows(IElement table)
    {
        var rows = new List<IElement>();
        foreach (var child in table.Children)
        {
            if (child.LocalName == "tr")
            {
                rows.Add(child);
            }
            else if (child.LocalName is "thead" or "tbody" or "tfoot")
            {
                foreach (var row in child.Children)
                {
                    if (row.LocalName == "tr")
                    {
                        rows.Add(row);
                    }
                }
            }
        }

        return rows;
    }

    /// <summary>Reads an HTML cell span.</summary>
    /// <param name="element">The source cell.</param>
    /// <param name="attribute">The span attribute.</param>
    /// <param name="remainingRows">The rows remaining in this table.</param>
    /// <returns>The normalized span.</returns>
    private static int ReadSpan(IElement element, string attribute, int remainingRows)
    {
        var value = element.GetAttribute(attribute);
        if (value is null)
        {
            return 1;
        }

        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var span) || span < 0 || span > MaximumSpan)
        {
            throw new FormatException("A table span must be a nonnegative bounded integer.");
        }

        if (attribute == "rowspan")
        {
            return span == 0 ? remainingRows : Math.Min(span, remainingRows);
        }

        return Math.Max(1, span);
    }

    /// <summary>Counts rows remaining in the same HTML row group.</summary>
    /// <param name="row">The starting row.</param>
    /// <returns>The rows that an HTML rowspan can occupy.</returns>
    private static int ReadRemainingRows(IElement row)
    {
        var remaining = 1;
        var sibling = row.NextElementSibling;
        while (sibling is not null)
        {
            if (sibling.LocalName == "tr")
            {
                remaining++;
            }

            sibling = sibling.NextElementSibling;
        }

        return remaining;
    }

    /// <summary>Places the source cells of one row.</summary>
    /// <param name="element">The source row.</param>
    /// <param name="row">The physical row index.</param>
    private void PlaceRow(IElement element, int row)
    {
        var column = 0;
        foreach (var child in element.Children)
        {
            if (child.LocalName is not ("td" or "th"))
            {
                continue;
            }

            while (Rows[row].ContainsKey(column))
            {
                column++;
            }

            var rowSpan = ReadSpan(child, "rowspan", ReadRemainingRows(element));
            var columnSpan = ReadSpan(child, "colspan", Rows.Count - row);
            PlaceCell(new(child, row, column, rowSpan, columnSpan));
            column += columnSpan;
            ColumnCount = Math.Max(ColumnCount, column);
        }
    }

    /// <summary>Reserves every grid location occupied by a source cell.</summary>
    /// <param name="cell">The source cell rectangle.</param>
    private void PlaceCell(RtfTableCell cell)
    {
        if (cell.Column + cell.ColumnSpan > MaximumSpan)
        {
            throw new NotSupportedException("RTF export supports at most 1000 physical table columns.");
        }

        for (var row = cell.Row; row < cell.Row + cell.RowSpan; row++)
        {
            for (var column = cell.Column; column < cell.Column + cell.ColumnSpan; column++)
            {
                if (!Rows[row].TryAdd(column, cell))
                {
                    throw new FormatException("HTML table cell spans overlap.");
                }
            }
        }
    }
}
