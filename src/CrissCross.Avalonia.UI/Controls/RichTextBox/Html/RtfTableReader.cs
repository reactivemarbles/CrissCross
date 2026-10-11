// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Net;
using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Buffers native RTF table rows until their merge and nested row properties are known.</summary>
internal sealed class RtfTableReader
{
    /// <summary>The maximum supported table depth.</summary>
    private const int MaximumDepth = 64;

    /// <summary>The number of twips in one CSS pixel.</summary>
    private const double TwipsPerPixel = 15D;

    /// <summary>The CSS name of a cell's right border.</summary>
    private const string RightSide = "right";

    /// <summary>The CSS name of a cell's bottom border.</summary>
    private const string BottomSide = "bottom";

    /// <summary>The active tables ordered by nesting depth.</summary>
    private readonly List<Table> _tables = [];

    /// <summary>The pending row definition preceding its contents.</summary>
    private Row? _definition;

    /// <summary>The cell definition receiving property controls.</summary>
    private Cell _cellDefinition = new();

    /// <summary>The selected cell border.</summary>
    private Border? _border;

    /// <summary>The current paragraph's table depth.</summary>
    private int _depth = 1;

    /// <summary>Gets whether a row is currently receiving content.</summary>
    internal bool InRow => _tables.Count >= _depth && _tables[_depth - 1].Current is not null;

    /// <summary>Gets whether any table is buffered.</summary>
    internal bool HasTable => _tables.Count > 0;

    /// <summary>Gets the color table used by cell shading and borders.</summary>
    internal List<string> Colors { get; } = [];

    /// <summary>Returns the output buffer for the current cell or document.</summary>
    /// <param name="document">The document output.</param>
    /// <returns>The appropriate output buffer.</returns>
    internal StringBuilder Output(StringBuilder document) => InRow ? EnsureCell().Content : document;

    /// <summary>Begins a row definition, including Word's trailing nested table destination.</summary>
    internal void BeginDefinition()
    {
        _definition = InRow ? _tables[_depth - 1].Current : new();
        _definition?.Cells.Clear();
        _cellDefinition = new();
        _border = null;
    }

    /// <summary>Changes table nesting, completing nested tables before returning to their parent.</summary>
    /// <param name="depth">The requested nesting level.</param>
    /// <param name="document">The document output.</param>
    internal void SetDepth(int depth, StringBuilder document)
    {
        if (depth is < 0 or > MaximumDepth)
        {
            throw new FormatException("The RTF table nesting level is invalid.");
        }

        var target = Math.Max(1, depth);
        while (_tables.Count > target)
        {
            CloseLast(document);
        }

        _depth = target;
        if (depth <= 1)
        {
            return;
        }

        EnsureRow();
    }

    /// <summary>Starts receiving cells at the current table depth.</summary>
    internal void EnsureRow()
    {
        if (_tables.Count + 1 < _depth)
        {
            throw new FormatException("An RTF nested table has no parent cell.");
        }

        if (_tables.Count < _depth)
        {
            _tables.Add(new());
        }

        var table = _tables[_depth - 1];
        if (table.Current is not null)
        {
            return;
        }

        var row = _definition ?? InheritRow(table);
        _definition = null;
        table.Rows.Add(row);
        table.Current = row;
    }

    /// <summary>Ensures a content buffer exists for the active physical cell.</summary>
    internal void EnsureContent() => _ = EnsureCell();

    /// <summary>Finishes the current physical cell.</summary>
    internal void EndCell()
    {
        EnsureRow();
        _ = EnsureCell();
        _tables[_depth - 1].Current!.Index++;
    }

    /// <summary>Finishes a row after validating the cell definitions.</summary>
    internal void EndRow()
    {
        if (!InRow)
        {
            throw new FormatException("An RTF row terminator has no row.");
        }

        var table = _tables[_depth - 1];
        var row = table.Current!;
        if (row.Cells.Count > 0 && row.Cells.Count != row.Contents.Count)
        {
            throw new FormatException("RTF cell contents do not match the row's cell definitions.");
        }

        table.Current = null;
        _definition = null;
    }

    /// <summary>Completes every buffered table.</summary>
    /// <param name="document">The document output.</param>
    internal void Close(StringBuilder document)
    {
        while (_tables.Count > 0)
        {
            CloseLast(document);
        }

        _depth = 1;
    }

    /// <summary>Reads row geometry and physical cell properties.</summary>
    /// <param name="word">The control name.</param>
    /// <param name="number">The optional control value.</param>
    /// <returns>Whether the control belongs to a table definition.</returns>
    internal bool ApplyProperty(string? word, int? number)
    {
        if (_definition is null || word is null)
        {
            return false;
        }

        if (word == "cellx")
        {
            AddDefinition(number ?? 0);
            return true;
        }

        if (word == "trleft")
        {
            _definition.Left = number ?? 0;
            return true;
        }

        return ApplyCellProperty(word, number) || ApplyBorderProperty(word, number);
    }

    /// <summary>Copies inherited row definitions without sharing mutable merge state.</summary>
    /// <param name="table">The table containing preceding row definitions.</param>
    /// <returns>A new row with inherited physical geometry.</returns>
    private static Row InheritRow(Table table)
    {
        var row = new Row();
        if (table.Rows.Count == 0)
        {
            return row;
        }

        var previous = table.Rows[^1];
        row.Left = previous.Left;
        foreach (var definition in previous.Cells)
        {
            row.Cells.Add(CopyDefinition(definition));
        }

        return row;
    }

    /// <summary>Copies the immutable part of a physical cell definition.</summary>
    /// <param name="definition">The preceding row's cell definition.</param>
    /// <returns>An independent cell definition.</returns>
    private static Cell CopyDefinition(Cell definition)
    {
        var copy = new Cell
        {
            Left = definition.Left,
            Right = definition.Right,
            Width = definition.Width,
            WidthType = definition.WidthType,
            Background = definition.Background,
            Alignment = definition.Alignment,
            HorizontalStart = definition.HorizontalStart,
            HorizontalContinue = definition.HorizontalContinue,
            VerticalStart = definition.VerticalStart,
            VerticalContinue = definition.VerticalContinue,
        };
        foreach (var border in definition.Borders)
        {
            copy.Borders.Add(border.Key, border.Value);
        }

        return copy;
    }

    /// <summary>Resolves horizontal and vertical continuations without emitting duplicate cells.</summary>
    /// <param name="row">The physical row.</param>
    /// <param name="previous">The previous row's active vertical origins.</param>
    /// <returns>The origins available to the next row.</returns>
    private static Dictionary<(int Left, int Right), Cell> MergeRow(Row row, Dictionary<(int Left, int Right), Cell> previous)
    {
        var current = new Dictionary<(int Left, int Right), Cell>();
        Cell? horizontal = null;
        for (var index = 0; index < row.Contents.Count; index++)
        {
            var cell = index < row.Cells.Count ? row.Cells[index] : new Cell();
            _ = cell.Content.Append(row.Contents[index].Content);
            cell.PhysicalRight = cell.Right;
            row.Contents[index] = cell;
            horizontal = MergeHorizontal(cell, horizontal);
        }

        horizontal = null;
        foreach (var cell in row.Contents)
        {
            if (!cell.HorizontalContinue)
            {
                horizontal = cell.HorizontalStart ? cell : null;
            }

            MergeVertical(cell, horizontal, previous, current);
        }

        return current;
    }

    /// <summary>Combines a physical horizontal continuation with its origin.</summary>
    /// <param name="cell">The physical cell.</param>
    /// <param name="origin">The preceding merge origin.</param>
    /// <returns>The active horizontal origin.</returns>
    private static Cell? MergeHorizontal(Cell cell, Cell? origin)
    {
        if (!cell.HorizontalContinue)
        {
            return cell.HorizontalStart ? cell : null;
        }

        if (origin is null)
        {
            throw new FormatException("An RTF horizontal merge continuation has no origin.");
        }

        origin.Columns++;
        origin.Right = cell.Right;
        origin.Width += cell.Width;
        _ = origin.Content.Append(cell.Content);
        if (cell.Borders.TryGetValue(RightSide, out var rightBorder))
        {
            origin.Borders[RightSide] = rightBorder;
        }

        cell.Hidden = true;
        return origin;
    }

    /// <summary>Extends a vertical origin using physical cell boundaries.</summary>
    /// <param name="cell">The physical cell.</param>
    /// <param name="horizontal">The active horizontal origin.</param>
    /// <param name="previous">The previous row's origins.</param>
    /// <param name="current">The next row's origins.</param>
    private static void MergeVertical(Cell cell, Cell? horizontal, Dictionary<(int Left, int Right), Cell> previous, Dictionary<(int Left, int Right), Cell> current)
    {
        var key = (cell.Left, cell.PhysicalRight);
        if (cell.VerticalContinue)
        {
            if (!previous.TryGetValue(key, out var origin))
            {
                throw new FormatException("An RTF vertical merge continuation has no matching origin.");
            }

            if (!cell.HorizontalContinue)
            {
                origin.Rows++;
                _ = origin.Content.Append(cell.Content);
            }

            cell.Hidden = true;
            current[key] = origin;
            if (cell.Borders.TryGetValue(BottomSide, out var bottomBorder))
            {
                origin.Borders[BottomSide] = bottomBorder;
            }
        }
        else if (cell.VerticalStart)
        {
            current[key] = horizontal ?? cell;
        }
    }

    /// <summary>Writes an HTML span only when more than one cell is covered.</summary>
    /// <param name="html">The markup buffer.</param>
    /// <param name="attribute">The span attribute.</param>
    /// <param name="count">The physical cell count.</param>
    private static void AppendSpan(StringBuilder html, string attribute, int count)
    {
        if (count <= 1)
        {
            return;
        }

        _ = html.Append(' ').Append(attribute).Append("=\"").Append(count.ToString(CultureInfo.InvariantCulture)).Append('"');
    }

    /// <summary>Returns or creates the cell receiving content.</summary>
    /// <returns>The current physical cell.</returns>
    private Cell EnsureCell()
    {
        var row = _tables[_depth - 1].Current ?? throw new FormatException("An RTF cell has no row.");
        if (row.Contents.Count <= row.Index)
        {
            row.Contents.Add(new());
        }

        return row.Contents[row.Index];
    }

    /// <summary>Reads merge flags, widths, shading, and cell border selectors.</summary>
    /// <param name="word">The control name.</param>
    /// <param name="number">The optional control value.</param>
    /// <returns>Whether a cell property was recognized.</returns>
    private bool ApplyCellProperty(string word, int? number) =>
        ApplyMergeProperty(word) || ApplyCellDimensions(word, number) || ApplyCellDecoration(word, number);

    /// <summary>Reads the four native cell merge flags.</summary>
    /// <param name="word">The control name.</param>
    /// <returns>Whether a merge flag was recognized.</returns>
    private bool ApplyMergeProperty(string word) => word switch
    {
        "clmgf" => _cellDefinition.HorizontalStart = true,
        "clmrg" => _cellDefinition.HorizontalContinue = true,
        "clvmgf" => _cellDefinition.VerticalStart = true,
        "clvmrg" => _cellDefinition.VerticalContinue = true,
        _ => false,
    };

    /// <summary>Reads preferred cell widths and vertical alignment.</summary>
    /// <param name="word">The control name.</param>
    /// <param name="number">The optional control value.</param>
    /// <returns>Whether a geometry property was recognized.</returns>
    private bool ApplyCellDimensions(string word, int? number)
    {
        if (word is "clwWidth" or "clftsWidth")
        {
            SetCellWidth(word, number ?? 0);
            return true;
        }

        var alignment = word switch
        {
            "clvertalt" => "top",
            "clvertalc" => "middle",
            "clvertalb" => BottomSide,
            _ => string.Empty,
        };
        if (alignment.Length == 0)
        {
            return false;
        }

        _cellDefinition.Alignment = alignment;
        return true;
    }

    /// <summary>Updates a cell width value or unit selector.</summary>
    /// <param name="word">The width control name.</param>
    /// <param name="value">The width control value.</param>
    private void SetCellWidth(string word, int value)
    {
        if (word == "clwWidth")
        {
            _cellDefinition.Width = value;
            return;
        }

        _cellDefinition.WidthType = value;
    }

    /// <summary>Reads cell shading and border selectors.</summary>
    /// <param name="word">The control name.</param>
    /// <param name="number">The optional control value.</param>
    /// <returns>Whether a decoration property was recognized.</returns>
    private bool ApplyCellDecoration(string word, int? number)
    {
        if (word == "clcbpat")
        {
            _cellDefinition.Background = number ?? 0;
            return true;
        }

        return word switch
        {
            "clbrdrt" => SelectBorder("top"),
            "clbrdrl" => SelectBorder("left"),
            "clbrdrb" => SelectBorder(BottomSide),
            "clbrdrr" => SelectBorder(RightSide),
            _ => false,
        };
    }

    /// <summary>Selects a side for subsequent border controls.</summary>
    /// <param name="side">The CSS border side.</param>
    /// <returns>A recognized control result.</returns>
    private bool SelectBorder(string side)
    {
        _border = new();
        _cellDefinition.Borders[side] = _border;
        return true;
    }

    /// <summary>Reads the selected border's line style, width, and color.</summary>
    /// <param name="word">The control name.</param>
    /// <param name="number">The optional control value.</param>
    /// <returns>Whether a border control was recognized.</returns>
    private bool ApplyBorderProperty(string word, int? number)
    {
        if (_border is null)
        {
            return false;
        }

        if (word is "brdrw" or "brdrcf")
        {
            SetBorderNumber(word, number ?? 0);
            return true;
        }

        var style = word switch
        {
            "brdrs" => "solid",
            "brdrdb" => "double",
            "brdrdash" => "dashed",
            "brdrdot" => "dotted",
            "brdrnil" or "brdrnone" => "none",
            _ => string.Empty,
        };
        if (style.Length == 0)
        {
            return false;
        }

        _border.Style = style;
        return true;
    }

    /// <summary>Updates the selected border's width or color.</summary>
    /// <param name="word">The border control name.</param>
    /// <param name="value">The border control value.</param>
    private void SetBorderNumber(string word, int value)
    {
        if (word == "brdrw")
        {
            _border!.Width = value;
            return;
        }

        _border!.Color = value;
    }

    /// <summary>Adds a cell definition with an increasing right boundary.</summary>
    /// <param name="right">The right boundary in twips.</param>
    private void AddDefinition(int right)
    {
        var row = _definition!;
        var left = row.Cells.Count == 0 ? row.Left : row.Cells[^1].Right;
        if (right <= left)
        {
            throw new FormatException("RTF table cell boundaries must increase across a row.");
        }

        _cellDefinition.Left = left;
        _cellDefinition.Right = right;
        row.Cells.Add(_cellDefinition);
        _cellDefinition = new();
        _border = null;
    }

    /// <summary>Renders a complete table and attaches it to its parent cell or document.</summary>
    /// <param name="document">The document output.</param>
    private void CloseLast(StringBuilder document)
    {
        var table = _tables[^1];
        if (table.Current is not null)
        {
            throw new FormatException("An RTF table row is not terminated.");
        }

        var html = RenderTable(table);
        _tables.RemoveAt(_tables.Count - 1);
        if (_tables.Count == 0)
        {
            _ = document.Append(html);
            return;
        }

        var parent = _tables[^1].Current ?? throw new FormatException("A nested RTF table has no parent row.");
        if (parent.Contents.Count <= parent.Index)
        {
            parent.Contents.Add(new());
        }

        _ = parent.Contents[parent.Index].Content.Append(html);
    }

    /// <summary>Normalizes merged cells and writes the HTML table.</summary>
    /// <param name="table">The complete buffered table.</param>
    /// <returns>The table markup.</returns>
    private string RenderTable(Table table)
    {
        var previous = new Dictionary<(int Left, int Right), Cell>();
        foreach (var row in table.Rows)
        {
            previous = MergeRow(row, previous);
        }

        var html = new StringBuilder("<table style=\"border-collapse:collapse\">");
        foreach (var row in table.Rows)
        {
            AppendRow(html, row);
        }

        return html.Append("</table>").ToString();
    }

    /// <summary>Writes the visible cells of one row.</summary>
    /// <param name="html">The markup buffer.</param>
    /// <param name="row">The normalized row.</param>
    private void AppendRow(StringBuilder html, Row row)
    {
        _ = html.Append("<tr>");
        foreach (var cell in row.Contents)
        {
            if (!cell.Hidden)
            {
                AppendCell(html, cell);
            }
        }

        _ = html.Append("</tr>");
    }

    /// <summary>Writes one merged cell with its dimensions and decoration.</summary>
    /// <param name="html">The markup buffer.</param>
    /// <param name="cell">The normalized cell.</param>
    private void AppendCell(StringBuilder html, Cell cell)
    {
        _ = html.Append("<td");
        AppendSpan(html, "colspan", cell.Columns);
        AppendSpan(html, "rowspan", cell.Rows);
        var style = GetStyle(cell);
        if (style.Length > 0)
        {
            _ = html.Append(" style=\"").Append(WebUtility.HtmlEncode(style)).Append('"');
        }

        _ = html.Append('>').Append(cell.Content).Append("</td>");
    }

    /// <summary>Produces CSS widths, alignment, shading, and per-side borders.</summary>
    /// <param name="cell">The cell definition.</param>
    /// <returns>The inline cell style.</returns>
    private string GetStyle(Cell cell)
    {
        const int TwipsWidthType = 3;
        var style = new StringBuilder();
        var width = cell.WidthType == TwipsWidthType && cell.Width > 0 ? cell.Width : cell.Right - cell.Left;
        if (width > 0)
        {
            _ = style.Append("width:").Append((width / TwipsPerPixel).ToString(CultureInfo.InvariantCulture)).Append("px;");
        }

        if (cell.Alignment.Length > 0)
        {
            _ = style.Append("vertical-align:").Append(cell.Alignment).Append(';');
        }

        AppendColor(style, "background-color", cell.Background);
        foreach (var border in cell.Borders)
        {
            AppendBorder(style, border.Key, border.Value);
        }

        return style.ToString();
    }

    /// <summary>Writes one explicit cell border.</summary>
    /// <param name="style">The CSS buffer.</param>
    /// <param name="side">The cell side.</param>
    /// <param name="border">The border properties.</param>
    private void AppendBorder(StringBuilder style, string side, Border border)
    {
        _ = style.Append("border-").Append(side).Append(':')
            .Append((Math.Max(0, border.Width) / TwipsPerPixel).ToString(CultureInfo.InvariantCulture))
            .Append("px ").Append(border.Style).Append(' ');
        _ = style.Append(border.Color > 0 && border.Color < Colors.Count ? Colors[border.Color] : "currentColor").Append(';');
    }

    /// <summary>Writes an explicit color-table entry.</summary>
    /// <param name="style">The CSS buffer.</param>
    /// <param name="property">The CSS property.</param>
    /// <param name="index">The color-table index.</param>
    private void AppendColor(StringBuilder style, string property, int index)
    {
        if (index <= 0 || index >= Colors.Count || Colors[index].Length == 0)
        {
            return;
        }

        _ = style.Append(property).Append(':').Append(Colors[index]).Append(';');
    }

    /// <summary>Stores rows of one nested table.</summary>
    private sealed class Table
    {
        /// <summary>Gets the buffered rows.</summary>
        internal List<Row> Rows { get; } = [];

        /// <summary>Gets or sets the row receiving content.</summary>
        internal Row? Current { get; set; }
    }

    /// <summary>Stores a row's physical definitions and contents.</summary>
    private sealed class Row
    {
        /// <summary>Gets the physical cell definitions.</summary>
        internal List<Cell> Cells { get; } = [];

        /// <summary>Gets the physical cell contents.</summary>
        internal List<Cell> Contents { get; } = [];

        /// <summary>Gets or sets the current cell index.</summary>
        internal int Index { get; set; }

        /// <summary>Gets or sets the left row boundary.</summary>
        internal int Left { get; set; }
    }

    /// <summary>Stores one physical cell's content and properties.</summary>
    private sealed class Cell
    {
        /// <summary>Gets the cell content markup.</summary>
        internal StringBuilder Content { get; } = new();

        /// <summary>Gets the per-side borders.</summary>
        internal Dictionary<string, Border> Borders { get; } = [with(StringComparer.Ordinal)];

        /// <summary>Gets or sets the left boundary.</summary>
        internal int Left { get; set; }

        /// <summary>Gets or sets the right boundary.</summary>
        internal int Right { get; set; }

        /// <summary>Gets or sets the original physical right boundary before horizontal merging.</summary>
        internal int PhysicalRight { get; set; }

        /// <summary>Gets or sets the requested width.</summary>
        internal int Width { get; set; }

        /// <summary>Gets or sets the width unit selector.</summary>
        internal int WidthType { get; set; }

        /// <summary>Gets or sets the shading color index.</summary>
        internal int Background { get; set; }

        /// <summary>Gets or sets the vertical alignment.</summary>
        internal string Alignment { get; set; } = string.Empty;

        /// <summary>Gets or sets whether this starts a horizontal merge.</summary>
        internal bool HorizontalStart { get; set; }

        /// <summary>Gets or sets whether this continues a horizontal merge.</summary>
        internal bool HorizontalContinue { get; set; }

        /// <summary>Gets or sets whether this starts a vertical merge.</summary>
        internal bool VerticalStart { get; set; }

        /// <summary>Gets or sets whether this continues a vertical merge.</summary>
        internal bool VerticalContinue { get; set; }

        /// <summary>Gets or sets whether this physical cell is represented by an origin.</summary>
        internal bool Hidden { get; set; }

        /// <summary>Gets or sets the horizontal cell count.</summary>
        internal int Columns { get; set; } = 1;

        /// <summary>Gets or sets the vertical row count.</summary>
        internal int Rows { get; set; } = 1;
    }

    /// <summary>Stores one cell border.</summary>
    private sealed class Border
    {
        /// <summary>The default border width in twips.</summary>
        private const int DefaultWidth = 15;

        /// <summary>Gets or sets the CSS line style.</summary>
        internal string Style { get; set; } = "solid";

        /// <summary>Gets or sets the width in twips.</summary>
        internal int Width { get; set; } = DefaultWidth;

        /// <summary>Gets or sets the color-table index.</summary>
        internal int Color { get; set; }
    }
}
