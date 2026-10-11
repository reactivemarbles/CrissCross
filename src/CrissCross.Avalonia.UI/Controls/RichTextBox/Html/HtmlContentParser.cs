// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaHorizontalAlignment = Avalonia.Layout.HorizontalAlignment;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the HtmlContentParser member.</summary>
internal static class HtmlContentParser
{
    /// <summary>HTML width attribute name.</summary>
    private const string WidthAttributeName = "width";

    /// <summary>Maximum supported cell span.</summary>
    private const int MaximumCellSpan = 1000;

    /// <summary>Provides the StyleDeclarationTokenCount member.</summary>
    private const int StyleDeclarationTokenCount = 2;

    /// <summary>Name of the HTML style attribute.</summary>
    private const string StyleAttributeName = "style";

    /// <summary>Provides the parser used for synchronous document parsing.</summary>
    private static readonly HtmlParser Parser = new();

    /// <summary>Provides the Parse member.</summary>
    /// <param name="html">The html value.</param>
    /// <returns>The result.</returns>
    internal static IReadOnlyList<TextSegment> Parse(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return [];
        }

        var document = Parser.ParseDocument(HtmlDocumentNormalizer.Normalize(html));
        var container = document.Body ?? document.DocumentElement;
        if (container is null)
        {
            return [];
        }

        var writer = new SegmentWriter();
        var rootContext = FormattingContext.Default.WithElement(container);
        foreach (var node in container.ChildNodes)
        {
            ProcessNode(node, rootContext, writer);
        }

        return writer.Build();
    }

    /// <summary>Provides the ToPlainText member.</summary>
    /// <param name="html">The html value.</param>
    /// <returns>The result.</returns>
    internal static string ToPlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var document = Parser.ParseDocument(html);
        return (document.Body ?? document.DocumentElement)?.TextContent?.Trim() ?? string.Empty;
    }

    /// <summary>Provides the ProcessNode member.</summary>
    /// <param name="node">The node value.</param>
    /// <param name="context">The context value.</param>
    /// <param name="writer">The writer value.</param>
    private static void ProcessNode(INode node, in FormattingContext context, SegmentWriter writer)
    {
        if (node is IComment)
        {
            return;
        }

        if (node is IText textNode)
        {
            writer.AppendText(textNode.Data, context);
            return;
        }

        if (node is IHtmlBreakRowElement)
        {
            AppendBreak((IElement)node, writer);

            return;
        }

        if (node is IHtmlImageElement imageElement)
        {
            var imageAlignmentContext = context.WithImageAlignment(ParseAlignment(imageElement));
            var imageSource = imageElement.GetAttribute("src") ?? imageElement.Source ?? string.Empty;
            writer.AppendImage(
                imageSource,
                imageAlignmentContext.ImageAlignment,
                ParseLengthAttribute(imageElement, WidthAttributeName),
                ParseLengthAttribute(imageElement, "height"),
                imageElement.GetAttribute("alt"));
            return;
        }

        if (node is not IElement element)
        {
            foreach (var child in node.ChildNodes)
            {
                ProcessNode(child, context, writer);
            }

            return;
        }

        ProcessElement(element, context, writer);
    }

    /// <summary>Writes a line or page break.</summary>
    /// <param name="element">The break element.</param>
    /// <param name="writer">The destination.</param>
    private static void AppendBreak(IElement element, SegmentWriter writer)
    {
        if (IsPageBreakElement(element))
        {
            writer.AppendPageBreak();
        }
        else
        {
            writer.AppendLineBreak();
        }
    }

    /// <summary>Writes an element and its children with inherited formatting.</summary>
    /// <param name="element">The element to render.</param>
    /// <param name="context">The inherited formatting.</param>
    /// <param name="writer">The segment destination.</param>
    private static void ProcessElement(IElement element, in FormattingContext context, SegmentWriter writer)
    {
        if (IsNonRenderedElement(element))
        {
            return;
        }

        var scopedContext = context.WithElement(element);
        if (IsPageBreakElement(element))
        {
            writer.AppendPageBoundary();
        }

        if (element.TagName == "TABLE")
        {
            AppendTable(element, scopedContext, writer);
            AppendFollowingPageBoundary(element, writer);
            return;
        }

        if (element is IHtmlListItemElement)
        {
            writer.AppendListMarker(GetListPrefix(element));
        }

        foreach (var child in element.ChildNodes)
        {
            ProcessNode(child, scopedContext, writer);
        }

        if (!IsParagraphElement(element))
        {
            return;
        }

        writer.AppendParagraphBreak(scopedContext.ParagraphAlignment);
        AppendFollowingPageBoundary(element, writer);
    }

    /// <summary>Writes an explicit page boundary following a block.</summary>
    /// <param name="element">The block element.</param>
    /// <param name="writer">The destination.</param>
    private static void AppendFollowingPageBoundary(IElement element, SegmentWriter writer)
    {
        if (!HasPageBreakAfter(element))
        {
            return;
        }

        writer.AppendPageBoundary();
    }

    /// <summary>Identifies elements outside the rendered body.</summary>
    /// <param name="element">The source element.</param>
    /// <returns>Whether the element should be skipped.</returns>
    private static bool IsNonRenderedElement(IElement element) => element.TagName is "SCRIPT" or "STYLE" or "HEAD" or "TITLE" or "TEMPLATE";

    /// <summary>Gets the visible marker for a list item.</summary>
    /// <param name="element">The list item.</param>
    /// <returns>The ordered number or bullet prefix.</returns>
    private static string GetListPrefix(IElement element)
    {
        var parent = element.ParentElement;
        if (parent?.TagName != "OL")
        {
            return "• ";
        }

        var number = int.TryParse(parent.GetAttribute("start"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var start) ? start : 1;
        foreach (var sibling in parent.Children)
        {
            if (sibling.TagName == "LI" && int.TryParse(sibling.GetAttribute("value"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var explicitNumber))
            {
                number = explicitNumber;
            }

            if (sibling == element)
            {
                break;
            }

            if (sibling.TagName == "LI")
            {
                number++;
            }
        }

        return $"{number.ToString(CultureInfo.InvariantCulture)}. ";
    }

    /// <summary>Parses table cells into a positioned grid and a document projection.</summary>
    /// <param name="element">The table element.</param>
    /// <param name="context">The inherited table formatting.</param>
    /// <param name="writer">The document destination.</param>
    private static void AppendTable(IElement element, in FormattingContext context, SegmentWriter writer)
    {
        var state = new TableWriterState();
        foreach (var row in element.QuerySelectorAll("tr"))
        {
            if (row.Closest("table") == element)
            {
                AppendTableRow(row, context, state);
            }
        }

        var rowCount = state.RowCount;
        foreach (var cell in state.Cells)
        {
            rowCount = Math.Max(rowCount, cell.Row + cell.RowSpan);
        }

        var table = new RichTextTable { Cells = state.Cells, RowCount = rowCount, ColumnCount = state.ColumnCount, Width = ParseLengthAttribute(element, WidthAttributeName) };
        writer.AppendTable(table, state.Text.ToString());
    }

    /// <summary>Parses one table row while accounting for earlier row spans.</summary>
    /// <param name="row">The row element.</param>
    /// <param name="context">The inherited formatting.</param>
    /// <param name="state">The table writer state.</param>
    private static void AppendTableRow(IElement row, in FormattingContext context, TableWriterState state)
    {
        var columnIndex = 0;
        var firstCell = true;
        foreach (var cell in row.Children)
        {
            if (cell.TagName is not ("TD" or "TH"))
            {
                continue;
            }

            while (state.Occupied.Contains((state.RowCount, columnIndex)))
            {
                columnIndex++;
            }

            if (!firstCell)
            {
                _ = state.Text.Append('\t');
            }

            firstCell = false;
            var model = CreateTableCell(cell, context.WithElement(row), state, columnIndex);
            state.Cells.Add(model);
            ReserveCell(model, state.Occupied);
            columnIndex += model.ColumnSpan;
            state.ColumnCount = Math.Max(state.ColumnCount, columnIndex);
        }

        _ = state.Text.Append(Environment.NewLine);
        state.RowCount++;
    }

    /// <summary>Creates one positioned rich table cell.</summary>
    /// <param name="cell">The cell element.</param>
    /// <param name="context">The inherited formatting.</param>
    /// <param name="state">The table state.</param>
    /// <param name="column">The grid column.</param>
    /// <returns>The positioned cell.</returns>
    private static RichTextTableCell CreateTableCell(IElement cell, in FormattingContext context, TableWriterState state, int column)
    {
        var cellContext = context.WithElement(cell);
        if (cell.TagName == "TH")
        {
            cellContext = cellContext with { Bold = true };
        }

        var cellWriter = new SegmentWriter();
        foreach (var child in cell.ChildNodes)
        {
            ProcessNode(child, cellContext, cellWriter);
        }

        var segments = cellWriter.Build();
        var cellStyle = RichTableCellStyle.Read(cell);
        var start = state.Text.Length;
        AppendCellText(segments, state.Text);
        return new()
        {
            Row = state.RowCount,
            Column = column,
            RowSpan = ParseSpan(cell, "rowspan"),
            ColumnSpan = ParseSpan(cell, "colspan"),
            IsHeader = cell.TagName == "TH",
            StartIndex = start,
            Length = state.Text.Length - start,
            Segments = segments,
            Background = cellContext.Background,
            Width = ParseLengthAttribute(cell, WidthAttributeName),
            BorderThickness = cellStyle.BorderThickness,
            BorderBrush = cellStyle.BorderBrush,
            Padding = cellStyle.Padding,
            ContentAlignment = cellStyle.Alignment,
        };
    }

    /// <summary>Writes the text projection of a cell.</summary>
    /// <param name="segments">The cell segments.</param>
    /// <param name="text">The text destination.</param>
    private static void AppendCellText(List<TextSegment> segments, StringBuilder text)
    {
        foreach (var segment in segments)
        {
            _ = text.Append(segment.IsImage ? "\uFFFC" : segment.Text);
        }
    }

    /// <summary>Reserves all grid positions occupied by a spanning cell.</summary>
    /// <param name="cell">The positioned cell.</param>
    /// <param name="occupied">The occupied grid positions.</param>
    private static void ReserveCell(RichTextTableCell cell, HashSet<(int Row, int Column)> occupied)
    {
        for (var row = 0; row < cell.RowSpan; row++)
        {
            for (var column = 0; column < cell.ColumnSpan; column++)
            {
                _ = occupied.Add((cell.Row + row, cell.Column + column));
            }
        }
    }

    /// <summary>Reads a bounded positive table cell span.</summary>
    /// <param name="element">The cell element.</param>
    /// <param name="attribute">The span attribute.</param>
    /// <returns>The supported span.</returns>
    private static int ParseSpan(IElement element, string attribute) =>
        int.TryParse(element.GetAttribute(attribute), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? Math.Clamp(value, 1, MaximumCellSpan)
            : 1;

    /// <summary>Detects explicit page breaks in exported HTML.</summary>
    /// <param name="element">The break element.</param>
    /// <returns>Whether the element represents a page break.</returns>
    private static bool IsPageBreakElement(IElement element)
    {
        foreach (var style in ParseStyles(element.GetAttribute(StyleAttributeName)))
        {
            if ((style.Name.Equals("break-before", StringComparison.OrdinalIgnoreCase) && style.Value.Equals("page", StringComparison.OrdinalIgnoreCase))
                || (style.Name.Equals("page-break-before", StringComparison.OrdinalIgnoreCase) && style.Value.Equals("always", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Detects an explicit page boundary following a block.</summary>
    /// <param name="element">The block element.</param>
    /// <returns>Whether a page should follow the block.</returns>
    private static bool HasPageBreakAfter(IElement element)
    {
        foreach (var style in ParseStyles(element.GetAttribute(StyleAttributeName)))
        {
            if ((style.Name.Equals("break-after", StringComparison.OrdinalIgnoreCase) && style.Value.Equals("page", StringComparison.OrdinalIgnoreCase))
                || (style.Name.Equals("page-break-after", StringComparison.OrdinalIgnoreCase) && style.Value.Equals("always", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Provides the ParseAlignment member.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The result.</returns>
    private static AvaloniaHorizontalAlignment ParseAlignment(IElement element)
    {
        var alignAttribute = element.GetAttribute("align");
        if (
            !string.IsNullOrWhiteSpace(alignAttribute)
            && Enum.TryParse(alignAttribute, true, out AvaloniaHorizontalAlignment alignment))
        {
            return alignment;
        }

        foreach (var declaration in ParseStyles(element.GetAttribute(StyleAttributeName)))
        {
            if (
                declaration.Name.Equals("text-align", StringComparison.OrdinalIgnoreCase)
                && Enum.TryParse(declaration.Value, true, out AvaloniaHorizontalAlignment styleAlignment))
            {
                return styleAlignment;
            }
        }

        return AvaloniaHorizontalAlignment.Left;
    }

    /// <summary>Provides the ParseLengthAttribute member.</summary>
    /// <param name="element">The element value.</param>
    /// <param name="attributeName">The attributeName value.</param>
    /// <returns>The result.</returns>
    private static double? ParseLengthAttribute(IElement element, string attributeName)
    {
        var attributeValue = element.GetAttribute(attributeName);
        foreach (var declaration in ParseStyles(element.GetAttribute(StyleAttributeName)))
        {
            if (declaration.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase))
            {
                attributeValue = declaration.Value;
            }
        }

        return string.IsNullOrWhiteSpace(attributeValue) || attributeValue.Trim().EndsWith('%')
            ? null
            : CssDocumentValues.ParseLength(attributeValue, null);
    }

    /// <summary>Provides the IsParagraphElement member.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The result.</returns>
    private static bool IsParagraphElement(IElement element)
    {
        var tag = element.TagName?.ToUpperInvariant();
        return tag is "P" or "DIV" or "LI" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6";
    }

    /// <summary>Provides the ParseStyles member.</summary>
    /// <param name="declaration">The declaration value.</param>
    /// <returns>The result.</returns>
    private static IEnumerable<(string Name, string Value)> ParseStyles(string? declaration)
    {
        if (string.IsNullOrWhiteSpace(declaration))
        {
            yield break;
        }

        foreach (
            var part in declaration.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = part.Split(':', StyleDeclarationTokenCount, StringSplitOptions.TrimEntries);
            if (tokens.Length == StyleDeclarationTokenCount)
            {
                yield return (tokens[0], tokens[1]);
            }
        }
    }

    /// <summary>Gets the value.</summary>
    /// <param name="Bold">The Bold value.</param>
    /// <param name="Italic">The Italic value.</param>
    /// <param name="Underline">The Underline value.</param>
    /// <param name="Strikethrough">The Strikethrough value.</param>
    /// <param name="Foreground">The foreground brush.</param>
    /// <param name="Background">The background brush.</param>
    /// <param name="FontSize">The font size.</param>
    /// <param name="FontFamily">The font family.</param>
    /// <param name="ImageAlignment">The image alignment.</param>
    /// <param name="ParagraphAlignment">The paragraph alignment.</param>
    /// <param name="HyperlinkNavigationUri">The hyperlink navigation address.</param>
    /// <param name="HyperlinkTarget">The hyperlink target.</param>
    private readonly record struct FormattingContext(
        bool Bold,
        bool Italic,
        bool Underline,
        bool Strikethrough,
        IBrush? Foreground,
        IBrush? Background,
        double? FontSize,
        FontFamily? FontFamily,
        AvaloniaHorizontalAlignment ImageAlignment,
        TextAlignment? ParagraphAlignment,
        string? HyperlinkNavigationUri,
        string? HyperlinkTarget)
    {
        /// <summary>Numeric CSS font weight considered bold.</summary>
        private const int BoldFontWeight = 600;

        /// <summary>Number of characters in a heading tag name.</summary>
        private const int HeadingTagLength = 2;

        /// <summary>Default font size used for percentage calculations.</summary>
        private const double DefaultFontSize = 14D;

        /// <summary>Font size for a level-one heading.</summary>
        private const double HeadingOneFontSize = 32D;

        /// <summary>Font size for a level-two heading.</summary>
        private const double HeadingTwoFontSize = 28D;

        /// <summary>Font size for a level-three heading.</summary>
        private const double HeadingThreeFontSize = 24D;

        /// <summary>Font size for a level-four heading.</summary>
        private const double HeadingFourFontSize = 20D;

        /// <summary>Font size for a level-five heading.</summary>
        private const double HeadingFiveFontSize = 18D;

        /// <summary>Font size for a level-six heading.</summary>
        private const double HeadingSixFontSize = 16D;

        /// <summary>Gets the default formatting context.</summary>
        internal static FormattingContext Default =>
            new(false, false, false, false, null, null, null, null, AvaloniaHorizontalAlignment.Left, null, null, null);

        /// <summary>Provides the WithElement member.</summary>
        /// <param name="element">The element value.</param>
        /// <returns>The result.</returns>
        internal FormattingContext WithElement(IElement element)
        {
            var tagName = element.TagName?.ToUpperInvariant();
            var result = ApplyTag(in this, element, tagName);
            result = ApplyHeading(in result, tagName);

            foreach (var style in ParseStyles(element.GetAttribute(StyleAttributeName)))
            {
                result = ApplyStyle(in result, style);
            }

            return result;
        }

        /// <summary>Provides the WithImageAlignment member.</summary>
        /// <param name="alignment">The alignment value.</param>
        /// <returns>The result.</returns>
        internal FormattingContext WithImageAlignment(AvaloniaHorizontalAlignment alignment) =>
            this with
            {
                ImageAlignment = alignment,
            };

        /// <summary>Applies formatting implied by an element tag.</summary>
        /// <param name="context">The current context.</param>
        /// <param name="element">The element.</param>
        /// <param name="tagName">The normalized tag name.</param>
        /// <returns>The updated context.</returns>
        private static FormattingContext ApplyTag(in FormattingContext context, IElement element, string? tagName) =>
            tagName switch
            {
                "B" or "STRONG" => context with { Bold = true },
                "I" or "EM" => context with { Italic = true },
                "U" => context with { Underline = true },
                "A" => context with
                {
                    Underline = true,
                    Foreground = context.Foreground ?? Brushes.RoyalBlue,
                    HyperlinkNavigationUri = element.GetAttribute("href"),
                    HyperlinkTarget = element.GetAttribute("target"),
                },
                "FONT" => ApplyFontTag(context, element),
                "S" or "DEL" or "STRIKE" => context with { Strikethrough = true },
                "P" or "DIV" => context with { ParagraphAlignment = ParseParagraphAlignment(element) ?? context.ParagraphAlignment },
                _ => context,
            };

        /// <summary>Applies legacy HTML font attributes.</summary>
        /// <param name="context">The inherited formatting.</param>
        /// <param name="element">The font element.</param>
        /// <returns>The updated formatting.</returns>
        private static FormattingContext ApplyFontTag(in FormattingContext context, IElement element) => context with
        {
            FontFamily = element.GetAttribute("face") is { } face ? new FontFamily(face.Trim('\'', '"')) : context.FontFamily,
            Foreground = TryParseColor(element.GetAttribute("color"), out var color) ? color : context.Foreground,
        };

        /// <summary>Applies heading formatting.</summary>
        /// <param name="context">The current context.</param>
        /// <param name="tagName">The normalized tag name.</param>
        /// <returns>The updated context.</returns>
        private static FormattingContext ApplyHeading(in FormattingContext context, string? tagName) =>
            IsHeadingTag(tagName) ? context with { Bold = true, FontSize = GetHeadingSize(tagName!) } : context;

        /// <summary>Determines whether a tag is a heading tag.</summary>
        /// <param name="tagName">The normalized tag name.</param>
        /// <returns><see langword="true"/> when the tag is a heading.</returns>
        private static bool IsHeadingTag(string? tagName) =>
            tagName?.StartsWith("H", StringComparison.Ordinal) == true
            && tagName.Length == HeadingTagLength
            && char.IsDigit(tagName[1]);

        /// <summary>Applies one style declaration.</summary>
        /// <param name="context">The current context.</param>
        /// <param name="style">The style declaration.</param>
        /// <returns>The updated context.</returns>
        private static FormattingContext ApplyStyle(in FormattingContext context, (string Name, string Value) style) =>
            style.Name.ToLowerInvariant() switch
            {
                "font-weight" => context with
                {
                    Bold = style.Value.Equals("bold", StringComparison.OrdinalIgnoreCase)
                        || (int.TryParse(style.Value, out var weight) && weight >= BoldFontWeight),
                },
                "font-style" => context with
                {
                    Italic = style.Value.Equals("italic", StringComparison.OrdinalIgnoreCase) || style.Value.Equals("oblique", StringComparison.OrdinalIgnoreCase),
                },
                "text-decoration" or "text-decoration-line" =>
                    context with
                    {
                        Underline = style.Value.Contains("underline", StringComparison.OrdinalIgnoreCase),
                        Strikethrough = style.Value.Contains("line-through", StringComparison.OrdinalIgnoreCase),
                    },
                "color" when TryParseColor(style.Value, out var foreground) => context with { Foreground = foreground },
                "background-color" or "background" or "mso-highlight" when TryParseColor(style.Value, out var background) => context with
                {
                    Background = background,
                },
                "font-size" => context with { FontSize = ParseFontSize(style.Value, context.FontSize) },
                "font-family" => context with { FontFamily = new(style.Value.Replace("\"", string.Empty, StringComparison.Ordinal).Replace("'", string.Empty, StringComparison.Ordinal)) },
                "text-align" when Enum.TryParse<TextAlignment>(style.Value, true, out var alignment) => context with
                {
                    ParagraphAlignment = alignment,
                },
                _ => context,
            };

        /// <summary>Provides the ParseParagraphAlignment member.</summary>
        /// <param name="element">The element value.</param>
        /// <returns>The result.</returns>
        private static TextAlignment? ParseParagraphAlignment(IElement element)
        {
            var alignAttr = element.GetAttribute("align");
            if (
                !string.IsNullOrWhiteSpace(alignAttr)
                && Enum.TryParse<TextAlignment>(alignAttr, true, out var fromAttribute))
            {
                return fromAttribute;
            }

            foreach (var declaration in ParseStyles(element.GetAttribute(StyleAttributeName)))
            {
                if (
                    declaration.Name.Equals("text-align", StringComparison.OrdinalIgnoreCase)
                    && Enum.TryParse<TextAlignment>(declaration.Value, true, out var fromStyle))
                {
                    return fromStyle;
                }
            }

            return null;
        }

        /// <summary>Provides the ParseFontSize member.</summary>
        /// <param name="value">The value.</param>
        /// <param name="current">The current value.</param>
        /// <returns>The result.</returns>
        private static double? ParseFontSize(string value, double? current) =>
            CssDocumentValues.ParseLength(value, current ?? DefaultFontSize) ?? current;

        /// <summary>Provides the GetHeadingSize member.</summary>
        /// <param name="tag">The tag value.</param>
        /// <returns>The result.</returns>
        private static double? GetHeadingSize(string tag) =>
            tag switch
            {
                "H1" => HeadingOneFontSize,
                "H2" => HeadingTwoFontSize,
                "H3" => HeadingThreeFontSize,
                "H4" => HeadingFourFontSize,
                "H5" => HeadingFiveFontSize,
                "H6" => HeadingSixFontSize,
                _ => null,
            };

        /// <summary>Provides the TryParseColor member.</summary>
        /// <param name="input">The input value.</param>
        /// <param name="brush">The brush value.</param>
        /// <returns>The result.</returns>
        private static bool TryParseColor(string? input, out IBrush? brush)
        {
            brush = null;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var value = input.Trim();
            if (!CssDocumentValues.TryParseColor(value, out var color))
            {
                return false;
            }

            brush = new SolidColorBrush(color);
            return true;
        }
    }

    /// <summary>Collects a table's positioned cells and text projection.</summary>
    private sealed class TableWriterState
    {
        /// <summary>Gets the source ordered cells.</summary>
        internal List<RichTextTableCell> Cells { get; } = [];

        /// <summary>Gets the occupied grid positions.</summary>
        internal HashSet<(int Row, int Column)> Occupied { get; } = [];

        /// <summary>Gets the document text destination.</summary>
        internal StringBuilder Text { get; } = new();

        /// <summary>Gets or sets the row count.</summary>
        internal int RowCount { get; set; }

        /// <summary>Gets or sets the column count.</summary>
        internal int ColumnCount { get; set; }
    }

    /// <summary>Provides the SegmentWriter member.</summary>
    private sealed class SegmentWriter
    {
        /// <summary>Provides the _segments member.</summary>
        private readonly List<TextSegment> _segments = new();

        /// <summary>Provides the _offset member.</summary>
        private int _offset;

        /// <summary>Provides the AppendText member.</summary>
        /// <param name="text">The text value.</param>
        /// <param name="context">The context value.</param>
        internal void AppendText(string? text, in FormattingContext context)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var segment = new TextSegment(text, _offset)
            {
                IsBold = context.Bold,
                IsItalic = context.Italic,
                IsUnderline = context.Underline,
                IsStrikethrough = context.Strikethrough,
                Foreground = context.Foreground,
                Background = context.Background,
                FontSize = context.FontSize,
                FontFamily = context.FontFamily,
                ParagraphAlignment = context.ParagraphAlignment,
                HyperlinkNavigationUri = context.HyperlinkNavigationUri,
                HyperlinkTarget = context.HyperlinkTarget,
            };

            _segments.Add(segment);
            _offset += text.Length;
        }

        /// <summary>Provides the AppendLineBreak member.</summary>
        internal void AppendLineBreak()
        {
            _segments.Add(TextSegment.CreateLineBreak(_offset));
            _offset += Environment.NewLine.Length;
        }

        /// <summary>Appends a document page break.</summary>
        internal void AppendPageBreak()
        {
            _segments.Add(new("\f", _offset) { IsPageBreak = true });
            _offset++;
        }

        /// <summary>Appends a page boundary that occupies no editable document positions.</summary>
        internal void AppendPageBoundary() => _segments.Add(new(string.Empty, _offset) { BreakPageBefore = true });

        /// <summary>Appends a generated list marker without consuming document positions.</summary>
        /// <param name="marker">The marker text.</param>
        internal void AppendListMarker(string marker) => _segments.Add(new(string.Empty, _offset) { ListMarker = marker });

        /// <summary>Provides the AppendParagraphBreak member.</summary>
        /// <param name="alignment">The alignment value.</param>
        internal void AppendParagraphBreak(TextAlignment? alignment)
        {
            _segments.Add(TextSegment.CreateParagraphBreak(_offset, alignment));
            _offset += Environment.NewLine.Length;
        }

        /// <summary>Provides the AppendImage member.</summary>
        /// <param name="source">The source value.</param>
        /// <param name="alignment">The alignment value.</param>
        /// <param name="width">The width value.</param>
        /// <param name="height">The height value.</param>
        /// <param name="alternativeText">The accessible image description.</param>
        internal void AppendImage(string source, AvaloniaHorizontalAlignment alignment, double? width, double? height, string? alternativeText)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return;
            }

            var image = TextSegment.CreateImage(_offset, source, alignment, width, height);
            image.ImageAlternativeText = alternativeText;
            _segments.Add(image);
            _offset++;
        }

        /// <summary>Provides the Build member.</summary>
        /// <returns>The result.</returns>
        internal List<TextSegment> Build() => _segments;

        /// <summary>Appends a grid table with a plain text document projection.</summary>
        /// <param name="table">The table model.</param>
        /// <param name="text">The flattened cell text.</param>
        internal void AppendTable(RichTextTable table, string text)
        {
            _segments.Add(new(text, _offset) { Table = table });
            _offset += text.Length;
        }
    }
}
