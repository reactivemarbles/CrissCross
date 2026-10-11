// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reads editable HTML from Word document packages.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The disabled Word property value.</summary>
    private const string ReaderDisabledValue = "0";

    /// <summary>The id token used during document reading.</summary>
    private const string ReaderId = "id";

    /// <summary>The sectPr token used during document reading.</summary>
    private const string ReaderSectionProperties = "sectPr";

    /// <summary>The numId token used during document reading.</summary>
    private const string ReaderNumberId = "numId";

    /// <summary>The abstractNumId token used during document reading.</summary>
    private const string ReaderAbstractNumberId = "abstractNumId";

    /// <summary>The ilvl token used during document reading.</summary>
    private const string ReaderListLevel = "ilvl";

    /// <summary>The pPr token used during document reading.</summary>
    private const string ReaderParagraphProperties = "pPr";

    /// <summary>The style token used during document reading.</summary>
    private const string ReaderStyle = "style";

    /// <summary>The tcPr token used during document reading.</summary>
    private const string ReaderCellProperties = "tcPr";

    /// <summary>The gridSpan token used during document reading.</summary>
    private const string ReaderGridSpan = "gridSpan";

    /// <summary>The vMerge token used during document reading.</summary>
    private const string ReaderVerticalMerge = "vMerge";

    /// <summary>The restart token used during document reading.</summary>
    private const string ReaderMergeRestart = "restart";

    /// <summary>The rPr token used during document reading.</summary>
    private const string ReaderRunProperties = "rPr";

    /// <summary>The 0.### token used during document reading.</summary>
    private const string ReaderPixelFormat = "0.###";

    /// <summary>The auto token used during document reading.</summary>
    private const string ReaderAutomaticColor = "auto";

    /// <summary>The none token used during document reading.</summary>
    private const string ReaderNoFormatting = "none";

    /// <summary>The application/octet-stream token used during document reading.</summary>
    private const string ReaderBinaryMime = "application/octet-stream";

    /// <summary>The document unit or bound used by the reader.</summary>
    private const int ReaderTwipsPerPixel = 15;

    /// <summary>The document unit or bound used by the reader.</summary>
    private const int ReaderEmusPerPixel = 9525;

    /// <summary>The document unit or bound used by the reader.</summary>
    private const int ReaderMaximumListLevel = 8;

    /// <summary>The document unit or bound used by the reader.</summary>
    private const int ReaderMaximumTableSpan = 1000;

    /// <summary>The document unit or bound used by the reader.</summary>
    private const int ReaderHalfPointsPerPoint = 2;

    /// <summary>Projects section boundaries, geometry, and page chrome into HTML metadata.</summary>
    /// <param name="body">The main story.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    /// <param name="styles">The styles.</param>
    private static void ReadSections(
        XElement body,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        var blocks = new List<XElement>();
        var sectionIndex = 0;
        foreach (var element in body.Elements())
        {
            var section = element.Name == Word + ReaderSectionProperties ? element : element.Element(Word + ReaderParagraphProperties)?.Element(Word + ReaderSectionProperties);
            if (element.Name != Word + ReaderSectionProperties)
            {
                blocks.Add(element);
            }

            if (section is null)
            {
                continue;
            }

            sectionIndex++;
            var id = $"docx-section-{sectionIndex.ToString(CultureInfo.InvariantCulture)}";
            ReadSectionMetadata(section, output, id);
            ReadBlocks(blocks, output, parts, path, rels, styles);
            _ = output.Append("</section>");
            ReadSectionRegions(section, output, parts, path, rels, styles, id);
            blocks.Clear();
        }

        ReadBlocks(blocks, output, parts, path, rels, styles);
    }

    /// <summary>Converts twips to invariant CSS pixels.</summary>
    /// <param name="value">The Word dimension.</param>
    /// <returns>The CSS dimension.</returns>
    private static string? TwipPixels(string? value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var size)
            ? (size / ReaderTwipsPerPixel).ToString(ReaderPixelFormat, CultureInfo.InvariantCulture)
            : null;

    /// <summary>Groups consecutive numbered paragraphs into nested semantic lists.</summary>
    /// <param name="elements">The source blocks.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadBlocks(
        IEnumerable<XElement> elements,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        var lists = new List<(string Id, string Tag)>();
        foreach (var element in elements)
        {
            var number = element.Element(Word + ReaderParagraphProperties)?.Element(Word + "numPr");
            var id = element.Name == Word + "p" ? Value(number?.Element(Word + ReaderNumberId)) : null;
            if (id is null || id == ReaderDisabledValue)
            {
                CloseLists(output, lists, 0);
                ReadBlock(element, output, parts, path, rels, styles);
                continue;
            }

            var level = ReaderNumberLevel(number);
            level = Math.Min(level, lists.Count);
            CloseLists(output, lists, level + 1);
            if (lists.Count > level && lists[level].Id != id)
            {
                CloseLists(output, lists, level);
            }

            if (lists.Count <= level)
            {
                var tag = ListTag(parts, path, rels, id, level);
                AppendListOpening(output, tag, ReadListLevel(parts, path, rels, id, level));
                lists.Add((id, tag));
            }
            else
            {
                _ = output.Append("</li>");
            }

            _ = output.Append("<li>");
            ReadBlock(element, output, parts, path, rels, styles);
        }

        CloseLists(output, lists, 0);
    }

    /// <summary>Closes nested HTML lists to the requested depth.</summary>
    /// <param name="output">The HTML destination.</param>
    /// <param name="lists">The list stack.</param>
    /// <param name="depth">The retained depth.</param>
    private static void CloseLists(StringBuilder output, List<(string Id, string Tag)> lists, int depth)
    {
        while (lists.Count > depth)
        {
            _ = output.Append("</li></").Append(lists[^1].Tag).Append('>');
            lists.RemoveAt(lists.Count - 1);
        }
    }

    /// <summary>Reads the bullet or ordered numbering format for a list level.</summary>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    /// <param name="id">The numbering identifier.</param>
    /// <param name="level">The level.</param>
    /// <returns>The HTML list tag.</returns>
    private static string ListTag(Dictionary<string, byte[]> parts, string path, Dictionary<string, PackageRelationship> rels, string id, int level)
    {
        var numbering = ReadListLevel(parts, path, rels, id, level);
        return numbering.Format == "bullet" ? "ul" : "ol";
    }

    /// <summary>Reads named styles for rendering inherited formatting.</summary>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The main relationships.</param>
    /// <returns>The style definitions.</returns>
    private static Dictionary<string, XElement> ReadStyleDefinitions(Dictionary<string, byte[]> parts, string path, Dictionary<string, PackageRelationship> rels)
    {
        var styles = new Dictionary<string, XElement>(StringComparer.Ordinal);
        var relationship = FindReaderRelationship(rels, "/styles");
        if (relationship?.External == false && parts.TryGetValue(ResolvePart(path, relationship.Target), out var bytes))
        {
            var document = ReadXml(bytes);
            foreach (var style in document.Descendants(Word + ReaderStyle))
            {
                if (Value(style, "styleId") is { } id)
                {
                    styles[id] = style;
                }
            }

            AddStyleDefaults(document.Root, styles);
        }

        return styles;
    }

    /// <summary>Reads one block, preserving XML for structures outside the editable subset.</summary>
    /// <param name="element">The Word element.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadBlock(
        XElement element,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        if (element.Name == Word + "p")
        {
            var properties = element.Element(Word + ReaderParagraphProperties);
            var styleId = Value(properties?.Element(Word + "pStyle"));
            var css = new StringBuilder("white-space:pre-wrap;");
            ReadParagraphStyles(properties, css);
            var inherited = new StringBuilder();
            ReadParagraphNamedStyles(styleId, styles, inherited);
            var number = properties?.Element(Word + "numPr");
            _ = output.Append("<p");
            PropertyAttribute(output, properties);
            Attribute(output, ReaderStyle, inherited.ToString() + css);
            Attribute(output, "data-docx-style", styleId);
            Attribute(output, "data-docx-num-id", Value(number?.Element(Word + ReaderNumberId)));
            Attribute(output, "data-docx-level", Value(number?.Element(Word + ReaderListLevel)));
            _ = output.Append('>');
            ReadAnnotatedParagraph(element, output, parts, path, rels, styles);

            _ = output.Append("</p>");
        }
        else if (element.Name == Word + "tbl")
        {
            ReadTable(element, output, parts, path, rels, styles);
        }
        else
        {
            ReadOpaque(element, output);
        }
    }

    /// <summary>Reads one table, projecting vertical merge continuations into row spans.</summary>
    /// <param name="table">The table.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadTable(
        XElement table,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        var rows = new List<XElement>(table.Elements(Word + "tr"));
        _ = output.Append("<table");
        PropertyAttribute(output, table.Element(Word + "tblPr"));
        ReadTableStyleAttributes(table.Element(Word + "tblPr"), false, output);
        Attribute(output, "data-docx-grid", table.Element(Word + "tblGrid") is { } grid ? EncodeXml(grid) : null);
        _ = output.Append('>');
        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            ReadTableRow(rows, rowIndex, output, parts, path, rels, styles);
        }

        _ = output.Append("</table>");
    }

    /// <summary>Reads runs, hyperlinks, and opaque inline structures.</summary>
    /// <param name="element">The source element.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadInline(
        XElement element,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        if (ReadSimplePageField(element, output))
        {
            return;
        }

        if (element.Name == Word + "r")
        {
            ReadInlineRun(element, output, parts, path, rels, styles);
        }
        else if (element.Name == Word + "hyperlink")
        {
            var id = (string?)element.Attribute(Office + ReaderId);
            var target = id is not null && rels.TryGetValue(id, out var relationship) ? relationship.Target : $"#{Value(element, "anchor")}";
            _ = output.Append("<a");
            Attribute(output, "href", target);
            _ = output.Append('>');
            foreach (var child in element.Elements())
            {
                ReadInline(child, output, parts, path, rels, styles);
            }

            _ = output.Append("</a>");
        }
        else if (element.Name == Word + "ins" || element.Name == Word + "del")
        {
            var tag = element.Name.LocalName;
            _ = output.Append('<').Append(tag);
            Attribute(output, "data-revision-id", LogicalRevisionIdentifier(parts, Value(element, ReaderId)));
            Attribute(output, "data-docx-native-revision-id", Value(element, ReaderId));
            Attribute(output, "data-author", Value(element, "author"));
            Attribute(output, "data-docx-date", Value(element, "date"));
            _ = output.Append('>');
            foreach (var child in element.Elements())
            {
                ReadInline(child, output, parts, path, rels, styles);
            }

            _ = output.Append("</").Append(tag).Append('>');
        }
        else
        {
            ReadOpaque(element, output);
        }
    }

    /// <summary>Reads an embedded image without accessing external resources.</summary>
    /// <param name="element">The drawing.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main part.</param>
    /// <param name="rels">The relationships.</param>
    private static void ReadImage(XElement element, StringBuilder output, Dictionary<string, byte[]> parts, string path, Dictionary<string, PackageRelationship> rels)
    {
        var blip = FirstReaderDescendant(element, Drawing + "blip");
        var id = (string?)blip?.Attribute(Office + "embed");
        if (id is null || !rels.TryGetValue(id, out var relation) || relation.External || !parts.TryGetValue(ResolvePart(path, relation.Target), out var bytes))
        {
            ReadOpaque(element, output);
            return;
        }

        var imagePath = ResolvePart(path, relation.Target);
        var types = ReadXml(parts["[Content_Types].xml"]);
        var mime = ReaderImageMime(types.Root, imagePath);
        _ = output.Append("<img");
        Attribute(output, "src", $"data:{mime};base64,{Convert.ToBase64String(bytes)}");
        Attribute(output, "data-docx-drawing", EncodeXml(element));
        var extent = FirstReaderDescendant(element, WordDrawing + "extent");
        Attribute(output, "width", EmuPixels((string?)extent?.Attribute("cx")));
        Attribute(output, "height", EmuPixels((string?)extent?.Attribute("cy")));
        Attribute(output, "alt", (string?)FirstReaderDescendant(element, WordDrawing + "docPr")?.Attribute("descr"));
        _ = output.Append('>');
    }

    /// <summary>Converts drawing units to CSS pixels.</summary>
    /// <param name="value">The EMU size.</param>
    /// <returns>The pixel size.</returns>
    private static string? EmuPixels(string? value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var size)
            ? (size / ReaderEmusPerPixel).ToString(ReaderPixelFormat, CultureInfo.InvariantCulture)
            : null;

    /// <summary>Reads paragraph style properties.</summary>
    /// <param name="properties">The properties.</param>
    /// <param name="css">The CSS destination.</param>
    private static void ReadParagraphStyles(XElement? properties, StringBuilder css)
    {
        if (Value(properties?.Element(Word + "jc")) is { } alignment)
        {
            _ = css.Append("text-align:").Append(alignment == "both" ? "justify" : alignment).Append(';');
        }

        if (!Enabled(properties?.Element(Word + "pageBreakBefore")))
        {
            return;
        }

        _ = css.Append("break-before:page;");
    }

    /// <summary>Reads directly specified run formatting.</summary>
    /// <param name="properties">The properties.</param>
    /// <param name="css">The CSS destination.</param>
    private static void ReadRunStyles(XElement? properties, StringBuilder css)
    {
        if (properties is null)
        {
            return;
        }

        if (properties.Element(Word + "b") is { } bold)
        {
            _ = css.Append(Enabled(bold) ? "font-weight:bold;" : "font-weight:normal;");
        }

        if (properties.Element(Word + "i") is { } italic)
        {
            _ = css.Append(Enabled(italic) ? "font-style:italic;" : "font-style:normal;");
        }

        ReadRunDecorations(properties, css);

        if (Value(properties.Element(Word + "rFonts"), "ascii") is { } font)
        {
            _ = css.Append("font-family:'").Append(font.Replace("'", string.Empty, StringComparison.Ordinal)).Append("';");
        }

        if (double.TryParse(Value(properties.Element(Word + "sz")), NumberStyles.Float, CultureInfo.InvariantCulture, out var size))
        {
            _ = css.Append("font-size:").Append((size / ReaderHalfPointsPerPoint).ToString(CultureInfo.InvariantCulture)).Append("pt;");
        }

        ReadRunColors(properties, css);
        ReadRunVerticalAlignment(properties, css);
    }

    /// <summary>Resolves style inheritance with cycle protection.</summary>
    /// <param name="id">The style identifier.</param>
    /// <param name="styles">The definitions.</param>
    /// <param name="css">The CSS destination.</param>
    /// <param name="visited">The recursion guard.</param>
    private static void ReadNamedStyle(string? id, Dictionary<string, XElement> styles, StringBuilder css, HashSet<string> visited)
    {
        if (id is null || !visited.Add(id) || !styles.TryGetValue(id, out var style))
        {
            return;
        }

        ReadNamedStyle(Value(style.Element(Word + "basedOn")), styles, css, visited);
        ReadParagraphStyles(style.Element(Word + ReaderParagraphProperties), css);
        ReadRunStyles(style.Element(Word + ReaderRunProperties), css);
    }

    /// <summary>Checks a Word on/off property.</summary>
    /// <param name="element">The property.</param>
    /// <returns>Whether it is enabled.</returns>
    private static bool Enabled(XElement? element) => element is not null && Value(element) is not (ReaderDisabledValue or "false" or "off");

    /// <summary>Parses a bounded positive table span.</summary>
    /// <param name="value">The span text.</param>
    /// <param name="fallback">The missing-value default.</param>
    /// <returns>The validated span.</returns>
    private static int PositiveInteger(string? value, int fallback)
    {
        if (value is null)
        {
            return fallback;
        }

        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) || result is < 1 or > ReaderMaximumTableSpan)
        {
            throw new InvalidDataException("DOCX table spans must be between 1 and 1000.");
        }

        return result;
    }

    /// <summary>Writes escaped HTML attributes.</summary>
    /// <param name="output">The destination.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The optional value.</param>
    private static void Attribute(StringBuilder output, string name, string? value)
    {
        if (value is null)
        {
            return;
        }

        _ = output.Append(' ').Append(name).Append("=\"").Append(WebUtility.HtmlEncode(value)).Append('"');
    }

    /// <summary>Preserves nonvisual Word property elements.</summary>
    /// <param name="output">The destination.</param>
    /// <param name="properties">The property element.</param>
    private static void PropertyAttribute(StringBuilder output, XElement? properties) => Attribute(output, "data-docx-properties", properties is null ? null : EncodeXml(properties));

    /// <summary>Preserves unsupported XML as an inert, noneditable template.</summary>
    /// <param name="element">The original XML.</param>
    /// <param name="output">The destination.</param>
    /// <param name="inRun">Whether the XML was a run child.</param>
    private static void ReadOpaque(XElement element, StringBuilder output, bool inRun = false)
    {
        _ = output.Append("<template data-docx-xml=\"").Append(EncodeXml(element)).Append('"');
        if (inRun)
        {
            _ = output.Append(" data-docx-run=\"true\"");
        }

        _ = output.Append("></template>");
    }

    /// <summary>Projects section geometry into HTML metadata.</summary>
    /// <param name="section">The section.</param>
    /// <param name="output">The output.</param>
    /// <param name="id">The id.</param>
    private static void ReadSectionMetadata(XElement section, StringBuilder output, string id)
    {
        _ = output.Append("<section");
        Attribute(output, "data-section-id", id);
        Attribute(output, "data-docx-section-properties", EncodeXml(section));
        Attribute(output, "data-page-width", TwipPixels(Value(section.Element(Word + "pgSz"), "w")));
        Attribute(output, "data-page-height", TwipPixels(Value(section.Element(Word + "pgSz"), "h")));
        var margins = section.Element(Word + "pgMar");
        if (margins is not null)
        {
            var padding = new List<string>();
            foreach (var name in new[] { "left", "top", "right", "bottom" })
            {
                padding.Add(TwipPixels(Value(margins, name)) ?? ReaderDisabledValue);
            }

            Attribute(output, "data-page-padding", string.Join(',', padding));
        }

        _ = output.Append('>');
    }

    /// <summary>Preserves section headers and footers in inert templates.</summary>
    /// <param name="section">The section.</param>
    /// <param name="output">The output.</param>
    /// <param name="parts">The parts.</param>
    /// <param name="path">The path.</param>
    /// <param name="rels">The rels.</param>
    /// <param name="styles">The styles.</param>
    /// <param name="id">The id.</param>
    private static void ReadSectionRegions(
        XElement section,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles,
        string id)
    {
        foreach (var reference in section.Elements())
        {
            if (reference.Name != Word + "headerReference" && reference.Name != Word + "footerReference")
            {
                continue;
            }

            var relationshipId = (string?)reference.Attribute(Office + ReaderId);
            if (relationshipId is null || !rels.TryGetValue(relationshipId, out var relationship) || relationship.External)
            {
                continue;
            }

            var regionPath = ResolvePart(path, relationship.Target);
            if (!parts.TryGetValue(regionPath, out var bytes))
            {
                continue;
            }

            ReadSectionRegion(reference, output, parts, regionPath, styles, id, bytes);
        }
    }

    /// <summary>Projects a resolved section header or footer into a template.</summary>
    /// <param name="reference">The region reference.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package parts.</param>
    /// <param name="regionPath">The region part path.</param>
    /// <param name="styles">The named styles.</param>
    /// <param name="id">The section identifier.</param>
    /// <param name="bytes">The region XML.</param>
    private static void ReadSectionRegion(
        XElement reference,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string regionPath,
        Dictionary<string, XElement> styles,
        string id,
        byte[] bytes)
    {
        var region = reference.Name == Word + "headerReference" ? "header" : "footer";
        var variant = Value(reference, "type") ?? "default";
        _ = output.Append("<template");
        Attribute(output, "data-page-region", region);
        Attribute(output, "data-section-id", id);
        Attribute(output, "data-docx-region-type", variant);
        Attribute(output, "data-page-variant", variant);
        _ = output.Append('>');
        ReadBlocks(ReadXml(bytes).Root?.Elements() ?? [], output, parts, regionPath, ReadRelationships(parts, regionPath), styles);
        _ = output.Append("</template>");
    }

    /// <summary>Finds the first relationship with the requested type suffix.</summary>
    /// <param name="rels">The rels.</param>
    /// <param name="suffix">The suffix.</param>
    /// <returns>The resolved value.</returns>
    private static PackageRelationship? FindReaderRelationship(Dictionary<string, PackageRelationship> rels, string suffix)
    {
        foreach (var relationship in rels.Values)
        {
            if (relationship.Type.EndsWith(suffix, StringComparison.Ordinal))
            {
                return relationship;
            }
        }

        return null;
    }

    /// <summary>Finds the first matching Word attribute.</summary>
    /// <param name="parent">The parent.</param>
    /// <param name="name">The name.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="value">The value.</param>
    /// <returns>The resolved value.</returns>
    private static XElement? FindReaderElement(XElement? parent, XName name, string attribute, string? value)
    {
        foreach (var element in parent?.Elements(name) ?? [])
        {
            if (Value(element, attribute) == value)
            {
                return element;
            }
        }

        return null;
    }

    /// <summary>Finds the first descendant with the requested name.</summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <returns>The resolved value.</returns>
    private static XElement? FirstReaderDescendant(XElement element, XName name)
    {
        using var descendants = element.Descendants(name).GetEnumerator();
        return descendants.MoveNext() ? descendants.Current : null;
    }

    /// <summary>Enumerates content without its property element.</summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <returns>The resolved value.</returns>
    private static IEnumerable<XElement> ReaderChildrenExcept(XElement element, XName name)
    {
        foreach (var child in element.Elements())
        {
            if (child.Name != name)
            {
                yield return child;
            }
        }
    }

    /// <summary>Projects one table row and its merge origins.</summary>
    /// <param name="rows">The rows.</param>
    /// <param name="rowIndex">The rowIndex.</param>
    /// <param name="output">The output.</param>
    /// <param name="parts">The parts.</param>
    /// <param name="path">The path.</param>
    /// <param name="rels">The rels.</param>
    /// <param name="styles">The styles.</param>
    private static void ReadTableRow(
        List<XElement> rows,
        int rowIndex,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        _ = output.Append("<tr");
        PropertyAttribute(output, rows[rowIndex].Element(Word + "trPr"));
        _ = output.Append('>');
        var column = 0;
        foreach (var cell in rows[rowIndex].Elements(Word + "tc"))
        {
            var properties = cell.Element(Word + ReaderCellProperties);
            var span = PositiveInteger(Value(properties?.Element(Word + ReaderGridSpan)), 1);
            var merge = properties?.Element(Word + ReaderVerticalMerge);
            if (merge is not null && Value(merge) != ReaderMergeRestart)
            {
                column += span;
                continue;
            }

            var rowSpan = merge is null ? 1 : ReaderRowSpan(rows, rowIndex, column, span);

            _ = output.Append("<td");
            PropertyAttribute(output, properties);
            ReadTableStyleAttributes(properties, true, output);
            Attribute(output, "colspan", span > 1 ? span.ToString(CultureInfo.InvariantCulture) : null);
            Attribute(output, "rowspan", rowSpan > 1 ? rowSpan.ToString(CultureInfo.InvariantCulture) : null);
            _ = output.Append('>');
            ReadBlocks(ReaderChildrenExcept(cell, Word + ReaderCellProperties), output, parts, path, rels, styles);

            _ = output.Append("</td>");
            column += span;
        }

        _ = output.Append("</tr>");
    }

    /// <summary>Counts matching vertical merge continuations.</summary>
    /// <param name="rows">The rows.</param>
    /// <param name="rowIndex">The rowIndex.</param>
    /// <param name="column">The column.</param>
    /// <param name="span">The span.</param>
    /// <returns>The resolved value.</returns>
    private static int ReaderRowSpan(List<XElement> rows, int rowIndex, int column, int span)
    {
        var rowSpan = 1;
        for (var next = rowIndex + 1; next < rows.Count; next++)
        {
            if (!ReaderMergeContinues(rows[next], column, span))
            {
                break;
            }

            rowSpan++;
        }

        return rowSpan;
    }

    /// <summary>Tests whether the same column continues a vertical merge.</summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="span">The span.</param>
    /// <returns>The resolved value.</returns>
    private static bool ReaderMergeContinues(XElement row, int column, int span)
    {
        var nextColumn = 0;
        foreach (var nextCell in row.Elements(Word + "tc"))
        {
            var properties = nextCell.Element(Word + ReaderCellProperties);
            var nextSpan = PositiveInteger(Value(properties?.Element(Word + ReaderGridSpan)), 1);
            if (nextColumn == column)
            {
                return nextSpan == span && properties?.Element(Word + ReaderVerticalMerge) is { } merge && Value(merge) != ReaderMergeRestart;
            }

            nextColumn += nextSpan;
        }

        return false;
    }

    /// <summary>Projects a formatted Word run.</summary>
    /// <param name="element">The element.</param>
    /// <param name="output">The output.</param>
    /// <param name="parts">The parts.</param>
    /// <param name="path">The path.</param>
    /// <param name="rels">The rels.</param>
    /// <param name="styles">The styles.</param>
    private static void ReadInlineRun(
        XElement element,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        var properties = element.Element(Word + ReaderRunProperties);
        var css = new StringBuilder();
        ReadNamedStyle(Value(properties?.Element(Word + "rStyle")), styles, css, [with(StringComparer.Ordinal)]);
        ReadRunStyles(properties, css);
        _ = output.Append("<span");
        PropertyAttribute(output, properties);
        Attribute(output, ReaderStyle, css.Length == 0 ? null : css.ToString());
        _ = output.Append('>');
        foreach (var child in element.Elements())
        {
            ReadRunChild(child, output, parts, path, rels);
        }

        _ = output.Append("</span>");
    }

    /// <summary>Projects one run child while preserving unsupported XML.</summary>
    /// <param name="child">The child.</param>
    /// <param name="output">The output.</param>
    /// <param name="parts">The parts.</param>
    /// <param name="path">The path.</param>
    /// <param name="rels">The rels.</param>
    private static void ReadRunChild(
        XElement child,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels)
    {
        if (ReadAnnotationRunChild(child, output))
        {
            return;
        }

        if (child.Name == Word + "t" || child.Name == Word + "delText")
        {
            _ = output.Append(WebUtility.HtmlEncode(child.Value));
        }
        else if (child.Name == Word + "tab")
        {
            _ = output.Append("&#9;");
        }
        else if (child.Name == Word + "br" || child.Name == Word + "cr")
        {
            _ = output.Append(Value(child, "type") == "page" ? "<br style=\"break-before:page\" data-docx-break=\"page\">" : "<br>");
        }
        else if (child.Name == Word + "drawing")
        {
            ReadImage(child, output, parts, path, rels);
        }
        else if (child.Name != Word + ReaderRunProperties)
        {
            ReadOpaque(child, output, true);
        }
    }

    /// <summary>Resolves image content types using override precedence.</summary>
    /// <param name="root">The root.</param>
    /// <param name="imagePath">The imagePath.</param>
    /// <returns>The resolved value.</returns>
    private static string ReaderImageMime(XElement? root, string imagePath)
    {
        if (ReaderImageOverrideMime(root, imagePath) is { } mime)
        {
            return mime;
        }

        var extension = System.IO.Path.GetExtension(imagePath).TrimStart('.');
        foreach (var item in root?.Elements(ContentTypes + "Default") ?? [])
        {
            if (string.Equals((string?)item.Attribute("Extension"), extension, StringComparison.OrdinalIgnoreCase))
            {
                return (string?)item.Attribute("ContentType") ?? ReaderBinaryMime;
            }
        }

        return ReaderBinaryMime;
    }

    /// <summary>Finds a content type specified for an individual image part.</summary>
    /// <param name="root">The content type declarations.</param>
    /// <param name="imagePath">The image part path.</param>
    /// <returns>The explicitly declared content type.</returns>
    private static string? ReaderImageOverrideMime(XElement? root, string imagePath)
    {
        foreach (var item in root?.Elements(ContentTypes + "Override") ?? [])
        {
            if ((string?)item.Attribute("PartName") == $"/{imagePath}" && (string?)item.Attribute("ContentType") is { } mime)
            {
                return mime;
            }
        }

        return null;
    }

    /// <summary>Projects underline and strike formatting.</summary>
    /// <param name="properties">The properties.</param>
    /// <param name="css">The css.</param>
    private static void ReadRunDecorations(XElement properties, StringBuilder css)
    {
        var decorations = new List<string>();
        if (properties.Element(Word + "u") is { } underline && Value(underline) != ReaderNoFormatting)
        {
            decorations.Add("underline");
        }

        if (Enabled(properties.Element(Word + "strike")) || Enabled(properties.Element(Word + "dStrike")))
        {
            decorations.Add("line-through");
        }

        if (decorations.Count == 0)
        {
            return;
        }

        _ = css.Append("text-decoration:").AppendJoin(' ', decorations).Append(';');
    }

    /// <summary>Projects foreground, shading, and highlight colors.</summary>
    /// <param name="properties">The properties.</param>
    /// <param name="css">The css.</param>
    private static void ReadRunColors(XElement properties, StringBuilder css)
    {
        if (Value(properties.Element(Word + "color")) is { } color && color != ReaderAutomaticColor)
        {
            _ = css.Append("color:#").Append(color).Append(';');
        }

        if (Value(properties.Element(Word + "shd"), "fill") is { } fill && fill != ReaderAutomaticColor)
        {
            _ = css.Append("background-color:#").Append(fill).Append(';');
        }
        else if (Value(properties.Element(Word + "highlight")) is { } highlight && highlight != ReaderNoFormatting)
        {
            var htmlColor = ReaderHighlightColor(highlight);
            _ = css.Append("background-color:").Append(htmlColor).Append(';');
        }
    }

    /// <summary>Projects superscript and subscript alignment.</summary>
    /// <param name="properties">The properties.</param>
    /// <param name="css">The css.</param>
    private static void ReadRunVerticalAlignment(XElement properties, StringBuilder css)
    {
        if (Value(properties.Element(Word + "vertAlign")) is not { } vertical)
        {
            return;
        }

        var alignment = vertical switch
        {
            "superscript" => "super",
            "subscript" => "sub",
            _ => "baseline",
        };
        _ = css.Append("vertical-align:").Append(alignment).Append(';');
    }

    /// <summary>Parses and bounds a Word list level.</summary>
    /// <param name="number">The numbering properties.</param>
    /// <returns>The supported nesting level.</returns>
    private static int ReaderNumberLevel(XElement? number) =>
        int.TryParse(Value(number?.Element(Word + ReaderListLevel)), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? Math.Clamp(parsed, 0, ReaderMaximumListLevel)
            : 0;

    /// <summary>Maps Word highlight names to CSS colors.</summary>
    /// <param name="highlight">The Word color name.</param>
    /// <returns>The equivalent CSS color.</returns>
    private static string ReaderHighlightColor(string highlight) =>
        highlight switch
        {
            "darkBlue" => "#000080",
            "darkCyan" => "#008080",
            "darkGreen" => "#008000",
            "darkMagenta" => "#800080",
            "darkRed" => "#800000",
            "darkYellow" => "#808000",
            "darkGray" => "#808080",
            "lightGray" => "#C0C0C0",
            _ => highlight,
        };
}
