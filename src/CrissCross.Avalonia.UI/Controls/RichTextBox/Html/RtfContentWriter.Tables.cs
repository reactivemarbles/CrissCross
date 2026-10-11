// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Dom;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Writes standard RTF table definitions and nested row destinations.</summary>
internal sealed partial class RtfContentWriter
{
    /// <summary>The RTF width type representing twips.</summary>
    private const int TwipWidthType = 3;

    /// <summary>The depth of the cell currently receiving content.</summary>
    private int _tableDepth;

    /// <summary>Reads absolute source column widths.</summary>
    /// <param name="table">The source table.</param>
    /// <param name="grid">The physical grid.</param>
    /// <returns>The physical column widths in twips.</returns>
    private static int[] ReadTableWidths(IElement table, RtfTableGrid grid)
    {
        var widths = new int[grid.ColumnCount];
        Array.Fill(widths, CellWidth);
        ReadColumnWidths(table, widths);
        for (var row = 0; row < grid.Rows.Count; row++)
        {
            foreach (var pair in grid.Rows[row])
            {
                var cell = pair.Value;
                if (cell.Row == row && cell.Column == pair.Key)
                {
                    ApplyCellWidth(cell, widths);
                }
            }
        }

        return widths;
    }

    /// <summary>Reads explicit HTML col widths.</summary>
    /// <param name="table">The source table.</param>
    /// <param name="widths">The physical column widths.</param>
    private static void ReadColumnWidths(IElement table, int[] widths)
    {
        var column = 0;
        foreach (var group in table.Children)
        {
            if (group.LocalName != "colgroup")
            {
                continue;
            }

            foreach (var col in group.Children)
            {
                if (col.LocalName == "col" && column < widths.Length)
                {
                    var width = ReadWidth(col);
                    widths[column] = width > 0 ? width : CellWidth;
                    column++;
                }
            }
        }
    }

    /// <summary>Distributes a cell width across its physical columns.</summary>
    /// <param name="cell">The source cell.</param>
    /// <param name="widths">The physical column widths.</param>
    private static void ApplyCellWidth(RtfTableCell cell, int[] widths)
    {
        var width = ReadWidth(cell.Element);
        if (width <= 0)
        {
            return;
        }

        var each = Math.Max(1, width / cell.ColumnSpan);
        for (var column = cell.Column; column < cell.Column + cell.ColumnSpan; column++)
        {
            widths[column] = each;
        }
    }

    /// <summary>Reads an absolute cell or column width.</summary>
    /// <param name="element">The source element.</param>
    /// <returns>The width in twips, or zero when unspecified.</returns>
    private static int ReadWidth(IElement element)
    {
        var value = ReadStyles(element).GetValueOrDefault("width") ?? element.GetAttribute("width");
        if (value is null)
        {
            return 0;
        }

        var hasUnit = value.EndsWith("px", StringComparison.OrdinalIgnoreCase) || value.EndsWith("pt", StringComparison.OrdinalIgnoreCase);
        return TryPoints(hasUnit ? value : $"{value}px", out var points) ? (int)Math.Round(points * TwipsPerPoint) : 0;
    }

    /// <summary>Writes a table at its current nesting level.</summary>
    /// <param name="table">The source table.</param>
    /// <param name="inTable">Whether the parent is a table cell.</param>
    /// <returns>Whether the element was handled.</returns>
    private bool WriteTable(IElement table, bool inTable)
    {
        var grid = RtfTableGrid.Create(table);
        var widths = ReadTableWidths(table, grid);
        _ = _body.Append("\\par ");
        _tableDepth++;
        for (var row = 0; row < grid.Rows.Count; row++)
        {
            WriteTableRow(grid, row, widths, ReadStyles(table));
        }

        _tableDepth--;
        _ = _body.Append("\\pard");
        if (inTable)
        {
            _ = _body.Append("\\intbl\\itap").Append(_tableDepth);
        }

        _ = _body.Append(' ');
        return true;
    }

    /// <summary>Writes a row, delaying nested row properties until after its content.</summary>
    /// <param name="grid">The physical table grid.</param>
    /// <param name="row">The physical row index.</param>
    /// <param name="widths">The physical column widths.</param>
    /// <param name="tableStyles">The table declarations.</param>
    private void WriteTableRow(RtfTableGrid grid, int row, int[] widths, Dictionary<string, string> tableStyles)
    {
        if (_tableDepth == 1)
        {
            WriteRowDefinition(grid, row, widths, tableStyles);
        }
        else
        {
            _ = _body.Append("\\pard\\intbl\\itap").Append(_tableDepth).Append(' ');
        }

        WriteRowCells(grid, row);
        if (_tableDepth > 1)
        {
            _ = _body.Append("{\\*\\nesttableprops");
            WriteRowDefinition(grid, row, widths, tableStyles);
            _ = _body.Append("\\nestrow}");
        }
        else
        {
            WriteRowDefinition(grid, row, widths, tableStyles);
            _ = _body.Append("\\row ");
        }
    }

    /// <summary>Writes a row definition with one descriptor per physical slot.</summary>
    /// <param name="grid">The physical table grid.</param>
    /// <param name="row">The physical row index.</param>
    /// <param name="widths">The physical column widths.</param>
    /// <param name="tableStyles">The table declarations.</param>
    private void WriteRowDefinition(RtfTableGrid grid, int row, int[] widths, Dictionary<string, string> tableStyles)
    {
        _ = _body.Append("\\trowd\\trgaph0\\trleft0");
        var right = 0;
        for (var column = 0; column < grid.ColumnCount; column++)
        {
            if (grid.Rows[row].TryGetValue(column, out var cell))
            {
                WriteCellMerges(cell, row, column);
                WriteCellStyles(cell.Element, tableStyles);
            }

            right += widths[column];
            _ = _body.Append("\\clftsWidth").Append(TwipWidthType).Append("\\clwWidth").Append(widths[column]).Append("\\cellx").Append(right);
        }

        _ = _body.Append(' ');
    }

    /// <summary>Writes horizontal and vertical merge state for one physical slot.</summary>
    /// <param name="cell">The owning source cell.</param>
    /// <param name="row">The physical row index.</param>
    /// <param name="column">The physical column index.</param>
    private void WriteCellMerges(RtfTableCell cell, int row, int column)
    {
        if (cell.ColumnSpan > 1)
        {
            _ = _body.Append(column == cell.Column ? "\\clmgf" : "\\clmrg");
        }

        if (cell.RowSpan <= 1)
        {
            return;
        }

        _ = _body.Append(row == cell.Row ? "\\clvmgf" : "\\clvmrg");
    }

    /// <summary>Writes source content only at each rectangle's origin.</summary>
    /// <param name="grid">The physical table grid.</param>
    /// <param name="row">The physical row index.</param>
    private void WriteRowCells(RtfTableGrid grid, int row)
    {
        for (var column = 0; column < grid.ColumnCount; column++)
        {
            _ = _body.Append('{');
            var styles = grid.Rows[row].TryGetValue(column, out var source) ? ReadStyles(source.Element) : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            WriteParagraphStart(styles, true);
            if (grid.Rows[row].TryGetValue(column, out var cell) && cell.Row == row && cell.Column == column)
            {
                WriteFormatting(cell.Element, styles);
                WriteChildren(cell.Element, true);
            }

            _ = _body.Append(_tableDepth > 1 ? "\\nestcell}" : "\\cell}");
        }
    }

    /// <summary>Writes cell shading, vertical alignment, and borders.</summary>
    /// <param name="element">The source cell.</param>
    /// <param name="tableStyles">The table declarations.</param>
    private void WriteCellStyles(IElement element, Dictionary<string, string> tableStyles)
    {
        var styles = ReadStyles(element);
        _ = _body.Append("\\clshdng0");
        WriteColor(styles.GetValueOrDefault("background-color") ?? styles.GetValueOrDefault("background") ?? element.GetAttribute("bgcolor"), "clcbpat");
        _ = _body.Append(styles.GetValueOrDefault("vertical-align") switch
        {
            "middle" => "\\clvertalc",
            "bottom" => "\\clvertalb",
            _ => "\\clvertalt",
        });
        WriteCellBorder(styles, tableStyles, "top", "clbrdrt");
        WriteCellBorder(styles, tableStyles, "bottom", "clbrdrb");
        WriteCellBorder(styles, tableStyles, "left", "clbrdrl");
        WriteCellBorder(styles, tableStyles, "right", "clbrdrr");
    }

    /// <summary>Writes a CSS border shorthand as a standard RTF cell border.</summary>
    /// <param name="styles">The cell declarations.</param>
    /// <param name="tableStyles">The table declarations.</param>
    /// <param name="side">The CSS border side.</param>
    /// <param name="control">The RTF cell border control.</param>
    private void WriteCellBorder(Dictionary<string, string> styles, Dictionary<string, string> tableStyles, string side, string control)
    {
        var border = styles.GetValueOrDefault($"border-{side}") ?? styles.GetValueOrDefault("border") ?? tableStyles.GetValueOrDefault("border");
        if (string.IsNullOrWhiteSpace(border))
        {
            return;
        }

        _ = _body.Append('\\').Append(control);
        foreach (var part in border.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            WriteBorderPart(part);
        }
    }

    /// <summary>Writes a width, style, or color from a border shorthand.</summary>
    /// <param name="part">The CSS border component.</param>
    private void WriteBorderPart(string part)
    {
        if (TryPoints(part, out var points))
        {
            _ = _body.Append("\\brdrw").Append((int)Math.Round(points * TwipsPerPoint));
        }
        else if (Color.TryParse(part, out _))
        {
            WriteColor(part, "brdrcf");
        }
        else
        {
            _ = _body.Append(part switch
            {
                "none" or "hidden" or "0" => "\\brdrnil",
                "double" => "\\brdrdb",
                "dashed" => "\\brdrdash",
                "dotted" => "\\brdrdot",
                "solid" => "\\brdrs",
                _ => string.Empty,
            });
        }
    }
}
