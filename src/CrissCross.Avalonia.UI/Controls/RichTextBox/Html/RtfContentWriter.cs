// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Globalization;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Writes scoped RTF from the rich editor's HTML document representation.</summary>
internal sealed class RtfContentWriter
{
    /// <summary>The number of points in one CSS pixel.</summary>
    private const double PointsPerPixel = 0.75;

    /// <summary>The largest accepted CSS size.</summary>
    private const double MaximumSize = 10_000;

    /// <summary>The number of characters in a CSS size suffix.</summary>
    private const int SizeSuffixLength = 2;

    /// <summary>The number of half points in one point.</summary>
    private const int HalfPointsPerPoint = 2;

    /// <summary>The number of twips in one point.</summary>
    private const int TwipsPerPoint = 20;

    /// <summary>The minimum numeric font weight rendered as bold.</summary>
    private const int BoldWeight = 600;

    /// <summary>The width of a table cell in twips.</summary>
    private const int CellWidth = 2_000;

    /// <summary>The alpha channel value for opaque colors.</summary>
    private const byte OpaqueAlpha = 255;

    /// <summary>The legacy CSS forced page break value.</summary>
    private const string AlwaysBreak = "always";

    /// <summary>The RTF forced page break control.</summary>
    private const string PageBreak = "\\page ";

    /// <summary>The document font table.</summary>
    private readonly List<string> _fonts = ["Arial"];

    /// <summary>The document color table.</summary>
    private readonly List<Color> _colors = [];

    /// <summary>The generated document body.</summary>
    private readonly StringBuilder _body = new();

    /// <summary>Converts an HTML document to RTF.</summary>
    /// <param name = "html">The source document.</param>
    /// <returns>The generated RTF document.</returns>
    internal string Write(string html)
    {
        var document = new HtmlParser().ParseDocument(HtmlDocumentNormalizer.Normalize(html));
        var container = document.Body ?? document.DocumentElement;
        if (container is not null)
        {
            WriteChildren(container, false);
        }

        var rtf = new StringBuilder("{\\rtf1\\ansi\\ansicpg1252\\deff0\\uc1{\\fonttbl");
        WriteFontTable(rtf);
        _ = rtf.Append("}{\\colortbl;");
        foreach (var color in _colors)
        {
            _ = rtf.Append("\\red").Append(color.R).Append("\\green").Append(color.G).Append("\\blue").Append(color.B).Append(';');
        }

        return rtf.Append("}\\viewkind4\\pard\\plain\\f0\\fs24 ").Append(_body).Append('}').ToString();
    }

    /// <summary>Escapes text for an RTF destination.</summary>
    /// <param name = "output">The destination buffer.</param>
    /// <param name = "value">The text to escape.</param>
    private static void Escape(StringBuilder output, string value)
    {
        foreach (var character in value)
        {
            var escaped = character switch
            {
                '\\' or '{' or '}' => $"\\{character}",
                '\r' => string.Empty,
                '\n' => "\\line ",
                '\t' => "\\tab ",
                < ' ' or > '~' => $"\\u{unchecked((short)character)}?",
                _ => character.ToString(),
            };
            _ = output.Append(escaped);
        }
    }

    /// <summary>Reads inline CSS declarations.</summary>
    /// <param name = "element">The source element.</param>
    /// <returns>The declarations indexed by property name.</returns>
    private static Dictionary<string, string> ReadStyles(IElement element)
    {
        var styles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var declaration in (element.GetAttribute("style") ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var colon = declaration.IndexOf(':');
            if (colon > 0)
            {
                styles[declaration[..colon].Trim()] = declaration[(colon + 1)..].Trim();
            }
        }

        return styles;
    }

    /// <summary>Parses a CSS size as points.</summary>
    /// <param name = "value">The CSS size.</param>
    /// <param name = "points">The size in points.</param>
    /// <returns>Whether the size is finite and supported.</returns>
    private static bool TryPoints(string value, out double points)
    {
        var pixels = value.EndsWith("px", StringComparison.OrdinalIgnoreCase);
        var hasSuffix = pixels || value.EndsWith("pt", StringComparison.OrdinalIgnoreCase);
        var number = hasSuffix ? value[..^SizeSuffixLength] : value;
        var valid = double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out points) && double.IsFinite(points) && points > 0 && points < MaximumSize;
        points *= pixels ? PointsPerPixel : 1;
        return valid;
    }

    /// <summary>Checks whether an element starts a paragraph.</summary>
    /// <param name = "tag">The HTML tag.</param>
    /// <returns>Whether the element is a block.</returns>
    private static bool IsBlock(string tag) => tag is "p" or "div" or "section" or "blockquote" or "li" or "pre" || IsHeading(tag);

    /// <summary>Checks whether an element is a heading.</summary>
    /// <param name = "tag">The HTML tag.</param>
    /// <returns>Whether the tag represents a heading.</returns>
    private static bool IsHeading(string tag) => tag is "h1" or "h2" or "h3" or "h4" or "h5" or "h6";

    /// <summary>Checks whether an element cannot be represented in RTF.</summary>
    /// <param name = "tag">The HTML tag.</param>
    /// <returns>Whether the element requires unsupported content.</returns>
    private static bool IsUnsupported(string tag) => tag is "iframe" or "object" or "embed" or "video" or "audio" or "svg" or "canvas";

    /// <summary>Validates table cell spans.</summary>
    /// <param name = "cell">The source cell.</param>
    private static void ValidateCell(IElement cell)
    {
        var colspan = cell.GetAttribute("colspan");
        var rowspan = cell.GetAttribute("rowspan");
        if (!((colspan is not null && colspan != "1") || (rowspan is not null && rowspan != "1")))
        {
            return;
        }

        throw new NotSupportedException("Merged table cells cannot be exported to RTF.");
    }

    /// <summary>Writes the registered fonts.</summary>
    /// <param name = "output">The document buffer.</param>
    private void WriteFontTable(StringBuilder output)
    {
        for (var index = 0; index < _fonts.Count; index++)
        {
            _ = output.Append("{\\f").Append(index).Append("\\fnil ");
            Escape(output, _fonts[index]);
            _ = output.Append(";}");
        }
    }

    /// <summary>Writes an HTML node.</summary>
    /// <param name = "node">The source node.</param>
    /// <param name = "inTable">Whether the node is inside a table cell.</param>
    private void WriteNode(INode node, bool inTable)
    {
        if (node is IText text)
        {
            Escape(_body, text.Data);
            return;
        }

        if (node is not IElement element || element.LocalName is "script" or "style" or "head")
        {
            return;
        }

        var styles = ReadStyles(element);
        var pageBefore = WritePageBreak(styles, "page-break-before", "break-before");
        if (WriteSpecialElement(element, inTable, pageBefore))
        {
            return;
        }

        WriteElement(element, styles, inTable);
        _ = WritePageBreak(styles, "page-break-after", "break-after");
    }

    /// <summary>Writes forced CSS page breaks.</summary>
    /// <param name = "styles">The inline declarations.</param>
    /// <param name = "legacyProperty">The legacy CSS property.</param>
    /// <param name = "property">The modern CSS property.</param>
    /// <returns>Whether either declaration requested a break.</returns>
    private bool WritePageBreak(Dictionary<string, string> styles, string legacyProperty, string property)
    {
        var legacyBreak = styles.GetValueOrDefault(legacyProperty) == AlwaysBreak;
        var modernBreak = styles.GetValueOrDefault(property) == "page";
        var requested = legacyBreak || modernBreak;
        if (requested)
        {
            _ = _body.Append(PageBreak);
        }

        return requested;
    }

    /// <summary>Writes elements that do not use a scoped text group.</summary>
    /// <param name = "element">The source element.</param>
    /// <param name = "inTable">Whether the element is inside a cell.</param>
    /// <param name = "pageBefore">Whether a page break was written.</param>
    /// <returns>Whether the element was handled.</returns>
    private bool WriteSpecialElement(IElement element, bool inTable, bool pageBefore)
    {
        if (IsUnsupported(element.LocalName))
        {
            throw new NotSupportedException($"HTML element '{element.LocalName}' cannot be represented in RTF by this editor.");
        }

        return element.LocalName switch
        {
            "img" => WriteImage(element),
            "br" => WriteLineBreak(pageBefore),
            "table" => WriteTable(element, inTable),
            _ => false,
        };
    }

    /// <summary>Writes a line break unless a page break already starts the block.</summary>
    /// <param name="pageBefore">Whether an explicit page break was emitted.</param>
    /// <returns>Whether the element was handled.</returns>
    private bool WriteLineBreak(bool pageBefore)
    {
        if (pageBefore)
        {
            return true;
        }

        _ = _body.Append("\\line ");

        return true;
    }

    /// <summary>Writes a scoped HTML text element.</summary>
    /// <param name = "element">The source element.</param>
    /// <param name = "styles">The inline declarations.</param>
    /// <param name = "inTable">Whether the element is inside a cell.</param>
    private void WriteElement(IElement element, Dictionary<string, string> styles, bool inTable)
    {
        var block = IsBlock(element.LocalName);
        _ = _body.Append('{');
        if (block)
        {
            WriteParagraphStart(styles, inTable);
        }

        WriteFormatting(element, styles);
        var link = element.LocalName == "a" ? element.GetAttribute("href") : null;
        if (link is not null)
        {
            _ = _body.Append("{\\field{\\*\\fldinst HYPERLINK \"");
            Escape(_body, link.Replace("\"", "%22", StringComparison.Ordinal));
            _ = _body.Append("\"}{\\fldrslt ");
        }

        if (element.LocalName == "li")
        {
            _ = _body.Append("\\bullet\\tab ");
        }

        WriteChildren(element, inTable);
        if (link is not null)
        {
            _ = _body.Append("}}");
        }

        if (block && (element.LocalName == "p" || element.ChildNodes.Length > 0))
        {
            _ = _body.Append("\\par ");
        }

        _ = _body.Append('}');
    }

    /// <summary>Writes paragraph controls.</summary>
    /// <param name = "styles">The inline declarations.</param>
    /// <param name = "inTable">Whether the paragraph is inside a cell.</param>
    private void WriteParagraphStart(Dictionary<string, string> styles, bool inTable)
    {
        _ = _body.Append("\\pard");
        if (inTable)
        {
            _ = _body.Append("\\intbl");
        }

        if (styles.TryGetValue("text-align", out var alignment))
        {
            _ = _body.Append(alignment switch
            {
                "center" => "\\qc",
                "right" => "\\qr",
                "justify" => "\\qj",
                _ => "\\ql",
            });
        }

        _ = _body.Append(' ');
    }

    /// <summary>Writes the children of a node.</summary>
    /// <param name = "node">The parent node.</param>
    /// <param name = "inTable">Whether the children are inside a cell.</param>
    private void WriteChildren(INode node, bool inTable)
    {
        foreach (var child in node.ChildNodes)
        {
            WriteNode(child, inTable);
        }
    }

    /// <summary>Writes inline character formatting.</summary>
    /// <param name = "element">The source element.</param>
    /// <param name = "styles">The inline declarations.</param>
    private void WriteFormatting(IElement element, Dictionary<string, string> styles)
    {
        WriteTagFormatting(element.LocalName);
        WriteStyleFormatting(styles);
        WriteFont(styles.GetValueOrDefault("font-family") ?? element.GetAttribute("face"));
        if (styles.TryGetValue("font-size", out var size) && TryPoints(size, out var points))
        {
            _ = _body.Append("\\fs").Append((int)Math.Round(points * HalfPointsPerPoint)).Append(' ');
        }

        WriteColor(styles.GetValueOrDefault("color") ?? element.GetAttribute("color"), "cf");
        WriteColor(styles.GetValueOrDefault("background-color") ?? styles.GetValueOrDefault("background"), "highlight");
    }

    /// <summary>Writes formatting implied by an HTML tag.</summary>
    /// <param name = "tag">The source tag.</param>
    private void WriteTagFormatting(string tag)
    {
        _ = _body.Append(tag switch
        {
            "b" or "strong" => "\\b ",
            "i" or "em" => "\\i ",
            "u" => "\\ul ",
            _ => string.Empty,
        });
        WriteAdditionalTagFormatting(tag);
    }

    /// <summary>Writes strikeout and baseline formatting implied by a tag.</summary>
    /// <param name = "tag">The source tag.</param>
    private void WriteAdditionalTagFormatting(string tag) => _ = _body.Append(tag switch
    {
        "s" or "strike" or "del" => "\\strike ",
        "sup" => "\\super ",
        "sub" => "\\sub ",
        _ => string.Empty,
    });

    /// <summary>Writes CSS character formatting.</summary>
    /// <param name = "styles">The inline declarations.</param>
    private void WriteStyleFormatting(Dictionary<string, string> styles)
    {
        if (styles.TryGetValue("font-weight", out var weight))
        {
            var bold = weight.Equals("bold", StringComparison.OrdinalIgnoreCase) || (int.TryParse(weight, out var numericWeight) && numericWeight >= BoldWeight);
            _ = _body.Append(bold ? "\\b " : "\\b0 ");
        }

        if (styles.TryGetValue("font-style", out var italic))
        {
            _ = _body.Append(italic == "italic" ? "\\i " : "\\i0 ");
        }

        WriteDecoration(styles);
    }

    /// <summary>Writes CSS decoration and baseline controls.</summary>
    /// <param name = "styles">The inline declarations.</param>
    private void WriteDecoration(Dictionary<string, string> styles)
    {
        if (styles.TryGetValue("text-decoration", out var decoration))
        {
            _ = _body.Append(decoration.Contains("underline", StringComparison.OrdinalIgnoreCase) ? "\\ul " : "\\ulnone ")
                .Append(decoration.Contains("line-through", StringComparison.OrdinalIgnoreCase) ? "\\strike " : "\\strike0 ");
        }

        if (!styles.TryGetValue("vertical-align", out var baseline))
        {
            return;
        }

        _ = _body.Append(baseline switch
        {
            "super" => "\\super ",
            "sub" => "\\sub ",
            _ => "\\nosupersub ",
        });
    }

    /// <summary>Registers and writes a font family.</summary>
    /// <param name = "family">The CSS font list or HTML face attribute.</param>
    private void WriteFont(string? family)
    {
        if (string.IsNullOrWhiteSpace(family))
        {
            return;
        }

        family = family.Split(',')[0].Trim().Trim('\'', '"');
        if (family.Contains(';'))
        {
            throw new FormatException("An RTF font name cannot contain a semicolon.");
        }

        var index = _fonts.IndexOf(family);
        if (index < 0)
        {
            index = _fonts.Count;
            _fonts.Add(family);
        }

        _ = _body.Append("\\f").Append(index).Append(' ');
    }

    /// <summary>Registers and writes an opaque color.</summary>
    /// <param name = "value">The CSS color.</param>
    /// <param name = "control">The RTF color control.</param>
    private void WriteColor(string? value, string control)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!Color.TryParse(value, out var color))
        {
            throw new NotSupportedException($"The HTML color '{value}' cannot be represented in RTF.");
        }

        if (color.A != OpaqueAlpha)
        {
            throw new NotSupportedException("RTF cannot preserve translucent HTML colors.");
        }

        var index = _colors.IndexOf(color);
        if (index < 0)
        {
            index = _colors.Count;
            _colors.Add(color);
        }

        _ = _body.Append('\\').Append(control).Append(index + 1).Append(' ');
    }

    /// <summary>Writes an embedded PNG or JPEG image.</summary>
    /// <param name = "element">The source image.</param>
    /// <returns>Whether the element was handled.</returns>
    private bool WriteImage(IElement element)
    {
        var source = element.GetAttribute("src") ?? string.Empty;
        var comma = source.IndexOf(',');
        var png = source.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase);
        var jpeg = source.StartsWith("data:image/jpeg;base64,", StringComparison.OrdinalIgnoreCase) || source.StartsWith("data:image/jpg;base64,", StringComparison.OrdinalIgnoreCase);
        if ((!png && !jpeg) || comma < 0)
        {
            throw new NotSupportedException("RTF export requires embedded PNG or JPEG images as base64 data URIs.");
        }

        var bytes = Convert.FromBase64String(source[(comma + 1)..]);
        _ = _body.Append("{\\pict").Append(png ? "\\pngblip" : "\\jpegblip");
        var styles = ReadStyles(element);
        WriteImageSize(element.GetAttribute("width") ?? styles.GetValueOrDefault("width"), "picwgoal");
        WriteImageSize(element.GetAttribute("height") ?? styles.GetValueOrDefault("height"), "pichgoal");
        _ = _body.Append(' ').Append(Convert.ToHexString(bytes)).Append('}');
        return true;
    }

    /// <summary>Writes an image dimension in twips.</summary>
    /// <param name = "value">The dimension in CSS units or pixels.</param>
    /// <param name = "word">The RTF dimension control.</param>
    private void WriteImageSize(string? value, string word)
    {
        if (value is null)
        {
            return;
        }

        var hasUnit = value.EndsWith("pt", StringComparison.OrdinalIgnoreCase) || value.EndsWith("px", StringComparison.OrdinalIgnoreCase);
        if (!TryPoints(hasUnit ? value : $"{value}px", out var points))
        {
            return;
        }

        _ = _body.Append('\\').Append(word).Append((int)Math.Round(points * TwipsPerPoint));
    }

    /// <summary>Writes a table with unmerged cells.</summary>
    /// <param name = "table">The source table.</param>
    /// <param name = "inTable">Whether this table is nested.</param>
    /// <returns>Whether the element was handled.</returns>
    private bool WriteTable(IElement table, bool inTable)
    {
        if (inTable)
        {
            throw new NotSupportedException("Nested HTML tables cannot be exported to RTF.");
        }

        _ = _body.Append("\\par ");
        foreach (var row in table.QuerySelectorAll("tr"))
        {
            WriteTableRow(row);
        }

        _ = _body.Append("\\pard ");
        return true;
    }

    /// <summary>Writes a table row and its cell boundaries.</summary>
    /// <param name = "row">The source row.</param>
    private void WriteTableRow(IElement row)
    {
        var cells = new List<IElement>();
        foreach (var child in row.Children)
        {
            if (child.LocalName is "td" or "th")
            {
                cells.Add(child);
            }
        }

        _ = _body.Append("\\trowd");
        for (var index = 0; index < cells.Count; index++)
        {
            ValidateCell(cells[index]);
            _ = _body.Append("\\cellx").Append((index + 1) * CellWidth);
        }

        _ = _body.Append(' ');
        foreach (var cell in cells)
        {
            _ = _body.Append("{\\pard\\intbl ");
            WriteFormatting(cell, ReadStyles(cell));
            WriteChildren(cell, true);
            _ = _body.Append("\\cell}");
        }

        _ = _body.Append("\\row ");
    }
}
