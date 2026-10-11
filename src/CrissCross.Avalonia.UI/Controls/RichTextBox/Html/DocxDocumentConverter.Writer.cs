// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Composes editable HTML into WordprocessingML package stories.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The highest Word list level.</summary>
    private const int WriterMaximumListLevel = 8;

    /// <summary>The width of a default table column in twips.</summary>
    private const int WriterColumnWidth = 2_400;

    /// <summary>The list indentation in twips.</summary>
    private const int WriterListIndent = 720;

    /// <summary>The list hanging indent in twips.</summary>
    private const int WriterHangingIndent = 360;

    /// <summary>The number of EMUs per twip.</summary>
    private const int WriterEmusPerTwip = 635;

    /// <summary>The length of a data URI prefix.</summary>
    private const int WriterDataPrefixLength = 5;

    /// <summary>The section properties element name.</summary>
    private const string WriterSectionProperties = "sectPr";

    /// <summary>The vertical merge element name.</summary>
    private const string WriterVerticalMerge = "vMerge";

    /// <summary>The numbering identifier attribute name.</summary>
    private const string WriterNumberId = "numId";

    /// <summary>The abstract numbering identifier attribute name.</summary>
    private const string WriterAbstractNumberId = "abstractNumId";

    /// <summary>The image relationship attribute name.</summary>
    private const string WriterImageEmbed = "embed";

    /// <summary>The content type attribute name.</summary>
    private const string WriterContentType = "ContentType";

    /// <summary>The header page region kind.</summary>
    private const string WriterHeader = "header";

    /// <summary>Composes WordprocessingML with package relationships.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>The package entries.</summary>
        private readonly Dictionary<string, byte[]> _parts;

        /// <summary>The main story path.</summary>
        private readonly string _path;

        /// <summary>The main story relationships.</summary>
        private readonly Dictionary<string, PackageRelationship> _rels;

        /// <summary>The content type definitions.</summary>
        private readonly XElement _types;

        /// <summary>The exported sections and their identifiers.</summary>
        private readonly List<(string Id, XElement Properties)> _sections = [];

        /// <summary>The deferred page region templates.</summary>
        private readonly List<IElement> _regions = [];

        /// <summary>The numbering definitions.</summary>
        private XElement? _numbering;

        /// <summary>The numbering part path.</summary>
        private string? _numberingPath;

        /// <summary>The next unique identifier.</summary>
        private int _identifier = 1;

        /// <summary>Initializes a new instance of the <see cref="PackageWriter"/> class.</summary>
        /// <param name="parts">The package parts.</param>
        /// <param name="path">The main part.</param>
        internal PackageWriter(Dictionary<string, byte[]> parts, string path)
        {
            _parts = parts;
            _path = path;
            _rels = ReadRelationships(parts, path);
            _types = parts.TryGetValue(
                "[Content_Types].xml",
                out var types)
                ? ReadXml(types).Root ?? throw new InvalidDataException("Empty content types.")
                : CreateContentTypes();
        }

        /// <summary>Writes block content into a Word container.</summary>
        /// <param name="source">The HTML container.</param>
        /// <param name="target">The Word container.</param>
        internal void WriteBlocks(INode source, XElement target)
        {
            XElement? paragraph = null;
            foreach (var node in source.ChildNodes)
            {
                if (node is IElement element && WriteBlockElement(element, target))
                {
                    paragraph = null;
                    continue;
                }

                if (node is IText text && string.IsNullOrWhiteSpace(text.Data) && paragraph is null)
                {
                    continue;
                }

                if (paragraph is null)
                {
                    paragraph = new(Word + "p");
                    target.Add(paragraph);
                }

                WriteInline(node, paragraph, InheritedRunProperties(source));
            }
        }

        /// <summary>Finishes package relationship and content type entries.</summary>
        internal void Complete()
        {
            WriteAnnotationStories();
            WriteRegions();
            SaveRevisionIdentifiers();
            AddContentType(_path, "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml");
            var rootRelationships = new XElement(
                Relationships + "Relationships",
                new XElement(
                    Relationships + "Relationship",
                    new XAttribute("Id", "rId1"),
                    new XAttribute(nameof(PackageRelationship.Type), $"{Office}/officeDocument"),
                    new XAttribute("Target", _path)));
            _ = _parts.TryAdd("_rels/.rels", XmlBytes(rootRelationships));

            SaveRelationships();
            if (_numbering is not null && _numberingPath is not null)
            {
                _parts[_numberingPath] = XmlBytes(_numbering);
            }

            _parts["[Content_Types].xml"] = XmlBytes(_types);
        }

        /// <summary>Replaces the reference to one page region story.</summary>
        /// <param name="properties">The section properties.</param>
        /// <param name="kind">The region kind.</param>
        /// <param name="regionType">The first, even, or default page region.</param>
        /// <param name="relationship">The story relationship.</param>
        private static void UpdateRegionReference(XElement properties, string kind, string regionType, string relationship)
        {
            var removals = new List<XElement>();
            foreach (var reference in properties.Elements(Word + $"{kind}Reference"))
            {
                if (Value(reference, "type") == regionType)
                {
                    removals.Add(reference);
                }
            }

            foreach (var reference in removals)
            {
                reference.Remove();
            }

            properties.AddFirst(new XElement(Word + $"{kind}Reference", new XAttribute(Word + "type", regionType), new XAttribute(Office + "id", relationship)));
        }

        /// <summary>Applies directly specified paragraph layout.</summary>
        /// <param name="element">The source paragraph.</param>
        /// <param name="properties">The Word paragraph properties.</param>
        private static void WriteParagraphLayout(IElement element, XElement properties)
        {
            var styles = Styles(element);
            if (styles.TryGetValue("text-align", out var alignment))
            {
                Set(properties, "jc", alignment == "justify" ? "both" : alignment);
            }

            var modernBreak = styles.TryGetValue("break-before", out var before) && before == "page";
            var legacyBreak = styles.TryGetValue("page-break-before", out var legacy) && legacy == "always";
            if (!modernBreak && !legacyBreak)
            {
                return;
            }

            Set(properties, "pageBreakBefore", "1");
        }

        /// <summary>Writes a text run with scoped inherited properties.</summary>
        /// <param name="text">The text.</param>
        /// <param name="target">The Word container.</param>
        /// <param name="inherited">The run properties.</param>
        private static void WriteTextRun(string text, XElement target, XElement inherited)
        {
            if (text.Length == 0)
            {
                return;
            }

            target.Add(new XElement(
                Word + "r",
                inherited.HasElements ? new XElement(inherited) : null,
                new XElement(Word + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), text)));
        }

        /// <summary>Combines inherited and preserved properties with edited formatting.</summary>
        /// <param name="element">The source.</param>
        /// <param name="inherited">The inherited properties.</param>
        /// <returns>The combined properties.</returns>
        private static XElement InlineProperties(IElement element, XElement inherited)
        {
            var properties = new XElement(inherited);
            if (element.GetAttribute("data-docx-properties") is { } encoded)
            {
                var original = DecodeXml(encoded);
                if (original.Name != Word + "rPr")
                {
                    throw new InvalidDataException("Inline DOCX metadata is not a run property element.");
                }

                foreach (var property in original.Elements())
                {
                    Replace(properties, new(property));
                }
            }

            ApplyRunStyles(element, properties);
            return properties;
        }

        /// <summary>Gets rows belonging to a table, excluding nested tables.</summary>
        /// <param name="table">The HTML table.</param>
        /// <returns>The table rows.</returns>
        private static List<IElement> TableRows(IElement table)
        {
            var rows = new List<IElement>();
            foreach (var row in table.QuerySelectorAll("tr"))
            {
                if (ReferenceEquals(row.Closest("table"), table))
                {
                    rows.Add(row);
                }
            }

            return rows;
        }

        /// <summary>Checks whether a row still contains merge continuations.</summary>
        /// <param name="active">The active merges.</param>
        /// <param name="column">The current column.</param>
        /// <returns>Whether a continuation remains.</returns>
        private static bool HasRemainingMerge(Dictionary<int, (int Remaining, int Span)> active, int column)
        {
            foreach (var key in active.Keys)
            {
                if (key >= column)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Writes a cell continuing a vertical merge.</summary>
        /// <param name="row">The Word row.</param>
        /// <param name="active">The merge state.</param>
        /// <param name="column">The current column.</param>
        /// <param name="merge">The merge dimensions.</param>
        private static void WriteMergeContinuation(XElement row, Dictionary<int, (int Remaining, int Span)> active, int column, (int Remaining, int Span) merge)
        {
            row.Add(new XElement(
                Word + "tc",
                new XElement(
                    Word + "tcPr",
                    new XElement(Word + "gridSpan", new XAttribute(Word + "val", merge.Span)),
                    new XElement(Word + WriterVerticalMerge)),
                new XElement(Word + "p")));
            if (merge.Remaining == 1)
            {
                _ = active.Remove(column);
            }
            else
            {
                active[column] = (merge.Remaining - 1, merge.Span);
            }
        }

        /// <summary>Creates a default fixed-width table grid.</summary>
        /// <param name="columns">The number of columns.</param>
        /// <returns>The grid.</returns>
        private static XElement TableGrid(int columns)
        {
            var grid = new XElement(Word + "tblGrid");
            for (var index = 0; index < columns; index++)
            {
                grid.Add(new XElement(Word + "gridCol", new XAttribute(Word + "w", WriterColumnWidth)));
            }

            return grid;
        }

        /// <summary>Finds an imported list item's paragraph.</summary>
        /// <param name="item">The list item.</param>
        /// <returns>The first paragraph, when present.</returns>
        private static IElement? FirstParagraph(IElement item)
        {
            foreach (var child in item.Children)
            {
                if (child.LocalName == "p")
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>Creates one numbering level with hanging indentation.</summary>
        /// <param name="level">The level index.</param>
        /// <param name="bullet">Whether bullets are requested.</param>
        /// <param name="start">The optional starting number.</param>
        /// <returns>The level definition.</returns>
        private static XElement NumberingLevel(int level, bool bullet, string? start)
        {
            var marker = bullet ? "•" : $"%{(level + 1).ToString(CultureInfo.InvariantCulture)}.";
            return new(
                Word + "lvl",
                new XAttribute(Word + "ilvl", level),
                new XElement(Word + "start", new XAttribute(Word + "val", PositiveInteger(start, 1))),
                new XElement(Word + "numFmt", new XAttribute(Word + "val", bullet ? "bullet" : "decimal")),
                new XElement(Word + "lvlText", new XAttribute(Word + "val", marker)),
                new XElement(Word + "lvlJc", new XAttribute(Word + "val", "left")),
                new XElement(
                    Word + "pPr",
                    new XElement(
                        Word + "ind",
                        new XAttribute(Word + "left", WriterListIndent * (level + 1)),
                        new XAttribute(Word + "hanging", WriterHangingIndent))));
        }

        /// <summary>Updates page margins while retaining header/footer distances.</summary>
        /// <param name="section">The source section.</param>
        /// <param name="properties">The Word section properties.</param>
        private static void WriteSectionMargins(IElement section, XElement properties)
        {
            if (section.GetAttribute("data-page-padding") is not { } padding)
            {
                return;
            }

            var values = padding.Split(',');
            if (values.Length != 4)
            {
                throw new InvalidDataException("Section page padding requires left, top, right, and bottom values.");
            }

            var margins = properties.Element(Word + "pgMar") ?? new XElement(
                Word + "pgMar",
                new XAttribute(Word + WriterHeader, 0),
                new XAttribute(Word + "footer", 0),
                new XAttribute(Word + "gutter", 0));
            foreach (var pair in new[] { ("left", values[0]), ("top", values[1]), ("right", values[2]), ("bottom", values[3]) })
            {
                margins.SetAttributeValue(Word + pair.Item1, PixelsTwips(pair.Item2));
            }

            if (margins.Parent is not null)
            {
                return;
            }

            properties.Add(margins);
        }

        /// <summary>Decodes an embedded image without downloading external resources.</summary>
        /// <param name="image">The image element.</param>
        /// <returns>The content type, extension, and bytes.</returns>
        private static (string Mime, string Extension, byte[] Bytes) ImagePayload(IElement image)
        {
            var source = image.GetAttribute("src") ?? string.Empty;
            var comma = source.IndexOf(',');
            if (!source.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) || comma < 0
                || !source[..comma].EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("DOCX export requires embedded base64 image data; external images are not downloaded.");
            }

            var mime = source[WriterDataPrefixLength..source.IndexOf(';')].ToLowerInvariant();
            var extension = mime switch
            {
                "image/png" => "png",
                "image/jpeg" => "jpg",
                "image/gif" => "gif",
                "image/bmp" => "bmp",
                "image/tiff" => "tif",
                _ => throw new NotSupportedException($"DOCX does not support the embedded image type '{mime}'."),
            };
            var bytes = Convert.FromBase64String(source[(comma + 1)..]);
            if (bytes.LongLength > MaximumPackageBytes)
            {
                throw new InvalidDataException("The embedded image exceeds the DOCX size limit.");
            }

            return (mime, extension, bytes);
        }

        /// <summary>Updates an imported drawing without discarding its layout.</summary>
        /// <param name="preserved">The original XML.</param>
        /// <param name="relationship">The image relationship.</param>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <returns>The updated drawing.</returns>
        private static XElement UpdateDrawing(string preserved, string relationship, long width, long height)
        {
            var original = DecodeXml(preserved);
            if (original.Name != Word + "drawing")
            {
                throw new InvalidDataException("Image metadata does not contain a Word drawing.");
            }

            foreach (var blip in original.Descendants(Drawing + "blip"))
            {
                blip.SetAttributeValue(Office + WriterImageEmbed, relationship);
            }

            foreach (var extent in original.Descendants(WordDrawing + "extent"))
            {
                extent.SetAttributeValue("cx", width);
                extent.SetAttributeValue("cy", height);
            }

            return original;
        }

        /// <summary>Identifies block media without a DOCX representation.</summary>
        /// <param name="name">The HTML tag.</param>
        /// <returns>Whether the tag is unsupported.</returns>
        private static bool IsUnsupportedBlock(string name) =>
            name is "iframe" or "object" or "embed" or "video" or "audio" or "canvas" or "svg";

        /// <summary>Identifies ordinary paragraphs and headings.</summary>
        /// <param name="name">The HTML tag.</param>
        /// <returns>Whether the tag contains paragraph content.</returns>
        private static bool IsParagraphElement(string name) =>
            name is "p" or "div" or "blockquote" or "pre" || IsHeadingElement(name);

        /// <summary>Identifies the six HTML heading levels.</summary>
        /// <param name="name">The HTML tag.</param>
        /// <returns>Whether the tag is a heading.</returns>
        private static bool IsHeadingElement(string name) =>
            name.Length == 2 && name[0] == 'h' && name[1] is >= '1' and <= '6';

        /// <summary>Normalizes the page variant for a Word region reference.</summary>
        /// <param name="region">The region template.</param>
        /// <returns>The Word region type.</returns>
        private static string RegionType(IElement region)
        {
            var value = region.GetAttribute("data-page-variant") ?? region.GetAttribute("data-docx-region-type") ?? "default";
            return value == "odd" ? "default" : value;
        }

        /// <summary>Preserves unsupported passive inline XML.</summary>
        /// <param name="element">The template.</param>
        /// <param name="target">The parent.</param>
        /// <param name="inherited">The scoped run properties.</param>
        private void WriteOpaqueInline(IElement element, XElement target, XElement inherited)
        {
            if (element.GetAttribute("data-docx-xml") is not { } opaque)
            {
                return;
            }

            var original = DecodeXml(opaque);
            if (HasGeneratedCommentReference(original))
            {
                return;
            }

            if (original.Name == Word + WriterSectionProperties)
            {
                throw new InvalidDataException("Section properties cannot appear inside a paragraph.");
            }

            target.Add(element.GetAttribute("data-docx-run") == "true"
                ? new XElement(
                    Word + "r",
                    inherited.HasElements ? new XElement(inherited) : null,
                    original)
                : original);
        }

        /// <summary>Dispatches block elements and returns whether they were handled.</summary>
        /// <param name="element">The HTML element.</param>
        /// <param name="target">The Word container.</param>
        /// <returns>Whether the element was a block.</returns>
        private bool WriteBlockElement(IElement element, XElement target) =>
            element.LocalName switch
            {
                "template" => WriteTemplateBlock(element, target),
                "script" or "style" => true,
                "table" => WriteTableBlock(element, target),
                "ol" or "ul" => WriteListBlock(element, target),
                "section" => WriteSectionBlock(element, target),
                _ => WriteParagraphBlock(element, target),
            };

        /// <summary>Writes one metadata template block.</summary>
        /// <param name="element">The template.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>True because templates are block metadata.</returns>
        private bool WriteTemplateBlock(IElement element, XElement target)
        {
            WriteTemplate(element, target);
            return true;
        }

        /// <summary>Writes one table block.</summary>
        /// <param name="element">The table.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>True because tables are blocks.</returns>
        private bool WriteTableBlock(IElement element, XElement target)
        {
            target.Add(WriteTable(element));
            return true;
        }

        /// <summary>Writes one list block.</summary>
        /// <param name="element">The list.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>True because lists are blocks.</returns>
        private bool WriteListBlock(IElement element, XElement target)
        {
            WriteList(element, target, 0);
            return true;
        }

        /// <summary>Writes a section's body and page properties.</summary>
        /// <param name="element">The section.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>True because sections are blocks.</returns>
        private bool WriteSectionBlock(IElement element, XElement target)
        {
            WriteBlocks(element, target);
            WriteSection(element, target);
            return true;
        }

        /// <summary>Handles ordinary paragraph blocks and rejects unsupported media.</summary>
        /// <param name="element">The HTML element.</param>
        /// <param name="target">The Word container.</param>
        /// <returns>Whether a paragraph was written.</returns>
        private bool WriteParagraphBlock(IElement element, XElement target)
        {
            if (element.GetAttribute("data-note-kind") is not null)
            {
                return true;
            }

            if (IsUnsupportedBlock(element.LocalName))
            {
                throw new NotSupportedException($"The HTML element '{element.LocalName}' cannot be exported to DOCX.");
            }

            var isParagraph = IsParagraphElement(element.LocalName);
            if (!isParagraph)
            {
                return false;
            }

            target.Add(WriteParagraph(element));
            return true;
        }

        /// <summary>Collects page chrome and retains inert original XML.</summary>
        /// <param name="element">The template.</param>
        /// <param name="target">The Word parent.</param>
        private void WriteTemplate(IElement element, XElement target)
        {
            if (element.GetAttribute("data-note-kind") is not null)
            {
                return;
            }

            if (element.GetAttribute("data-page-region") is not null)
            {
                _regions.Add(element);
            }

            if (element.GetAttribute("data-docx-xml") is not { } opaque)
            {
                return;
            }

            target.Add(DecodeXml(opaque));
        }

        /// <summary>Writes this story's relationships.</summary>
        private void SaveRelationships()
        {
            var root = new XElement(Relationships + "Relationships");
            foreach (var pair in _rels)
            {
                root.Add(new XElement(
                    Relationships + "Relationship",
                    new XAttribute("Id", pair.Key),
                    new XAttribute(nameof(PackageRelationship.Type), pair.Value.Type),
                    new XAttribute("Target", pair.Value.Target),
                    pair.Value.External ? new XAttribute("TargetMode", "External") : null));
            }

            _parts[RelationshipPath(_path)] = XmlBytes(root);
        }

        /// <summary>Writes editable page region templates into independent Word stories.</summary>
        private void WriteRegions()
        {
            foreach (var region in _regions)
            {
                WriteRegion(region);
            }
        }

        /// <summary>Writes one region and updates its section references.</summary>
        /// <param name="region">The page region template.</param>
        private void WriteRegion(IElement region)
        {
            var kind = region.GetAttribute("data-page-region");
            if (kind is not (WriterHeader or "footer"))
            {
                throw new InvalidDataException("DOCX page regions must be headers or footers.");
            }

            var sectionId = region.GetAttribute("data-section-id") ?? string.Empty;
            var sections = FindRegionSections(sectionId);
            if (sections.Count == 0)
            {
                throw new NotSupportedException("DOCX page regions require an associated document section.");
            }

            var regionType = RegionType(region);
            var target = $"{kind}-crisscross-{NextIdentifier()}.xml";
            var path = ResolvePart(_path, target);
            var root = new XElement(Word + (kind == WriterHeader ? "hdr" : "ftr"));
            var writer = new PackageWriter(_parts, path);
            writer.WriteBlocks(region is IHtmlTemplateElement template ? template.Content : region, root);
            if (!root.HasElements)
            {
                root.Add(new XElement(Word + "p"));
            }

            writer.SaveRelationships();
            _parts[path] = XmlBytes(root);
            AddContentType(path, $"application/vnd.openxmlformats-officedocument.wordprocessingml.{kind}+xml");
            var relationship = AddRelationship(kind, target, false);
            foreach (var section in sections)
            {
                EnablePageVariant(section.Properties, regionType);
                UpdateRegionReference(section.Properties, kind, regionType, relationship);
            }
        }

        /// <summary>Finds sections associated with one page region.</summary>
        /// <param name="identifier">The section identifier, or empty for all sections.</param>
        /// <returns>The matching sections.</returns>
        private List<(string Id, XElement Properties)> FindRegionSections(string identifier)
        {
            var result = new List<(string Id, XElement Properties)>();
            foreach (var section in _sections)
            {
                if (identifier.Length == 0 || section.Id == identifier)
                {
                    result.Add(section);
                }
            }

            return result;
        }

        /// <summary>Allocates a unique ordinal without expression side effects.</summary>
        /// <returns>The identifier.</returns>
        private string NextIdentifier()
        {
            var value = _identifier;
            _identifier++;
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Builds a paragraph from HTML and preserved properties.</summary>
        /// <param name="element">The source paragraph.</param>
        /// <returns>The paragraph.</returns>
        private XElement WriteParagraph(IElement element)
        {
            var properties = Properties(element, "pPr");
            if (element.GetAttribute("data-docx-style") is { } style)
            {
                Set(properties, "pStyle", style);
            }

            properties.Elements(Word + WriterSectionProperties).Remove();
            WriteParagraphLayout(element, properties);

            ApplyParagraphNumbering(element, properties);

            var paragraph = new XElement(Word + "p", properties.HasElements ? properties : null);
            var runProperties = InheritedRunProperties(element);
            ApplyRunStyles(element, runProperties);
            foreach (var child in element.ChildNodes)
            {
                WriteInline(child, paragraph, runProperties);
            }

            return paragraph;
        }

        /// <summary>Writes one inline while keeping inherited formatting scoped.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="target">The Word parent.</param>
        /// <param name="inherited">The inherited run properties.</param>
        private void WriteInline(INode node, XElement target, XElement inherited)
        {
            if (node is IText text)
            {
                WriteTextRun(text.Data, target, inherited);
                return;
            }

            if (node is not IElement element || element.LocalName is "ol" or "ul")
            {
                return;
            }

            if (element.LocalName == "template")
            {
                WriteOpaqueInline(element, target, inherited);
                return;
            }

            var properties = InlineProperties(element, inherited);
            if (WriteSpecialInline(element, target))
            {
                return;
            }

            var parent = InlineParent(element, target);
            WriteInlineChildren(element, parent, properties);
        }

        /// <summary>Writes images and explicit breaks.</summary>
        /// <param name="element">The source.</param>
        /// <param name="target">The parent.</param>
        /// <returns>Whether the inline was handled.</returns>
        private bool WriteSpecialInline(IElement element, XElement target)
        {
            if (WriteAnnotationInline(element, target))
            {
                return true;
            }

            if (element.LocalName == "br")
            {
                var styles = Styles(element);
                var page = element.GetAttribute("data-docx-break") == "page" || (styles.TryGetValue("break-before", out var before) && before == "page");
                target.Add(new XElement(Word + "r", new XElement(Word + "br", page ? new XAttribute(Word + "type", "page") : null)));
                return true;
            }

            if (element.LocalName != "img")
            {
                return false;
            }

            target.Add(new XElement(Word + "r", WriteImage(element)));
            return true;
        }

        /// <summary>Creates scoped hyperlinks and tracked revision containers.</summary>
        /// <param name="element">The source.</param>
        /// <param name="target">The parent.</param>
        /// <returns>The inline container.</returns>
        private XElement InlineParent(IElement element, XElement target)
        {
            var parent = target;
            if (element.LocalName == "a" && element.GetAttribute("href") is { } href)
            {
                parent = new(
                    Word + "hyperlink",
                    href.StartsWith('#')
                        ? new XAttribute(
                            Word + "anchor",
                            href[1..])
                        : new XAttribute(Office + "id", AddRelationship("hyperlink", href, true)));
                target.Add(parent);
            }
            else if (element.LocalName is "ins" or "del" && element.GetAttribute("data-revision-id") is { } revision)
            {
                parent = new(
                    Word + element.LocalName,
                    new XAttribute(Word + "id", NativeRevisionIdentifier(element, revision)),
                    new XAttribute(Word + "author", element.GetAttribute("data-author") ?? string.Empty));
                if (element.GetAttribute("data-docx-date") is { } date)
                {
                    parent.Add(new XAttribute(Word + "date", date));
                }

                target.Add(parent);
            }

            return parent;
        }

        /// <summary>Writes child runs and bookmark ranges.</summary>
        /// <param name="element">The source.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="properties">The scoped run properties.</param>
        private void WriteInlineChildren(IElement element, XElement parent, XElement properties)
        {
            var comment = BeginComment(element, parent);
            var bookmark = element.GetAttribute("data-bookmark");
            var bookmarkId = BeginBookmark(bookmark, parent);

            foreach (var child in element.ChildNodes)
            {
                WriteInline(child, parent, properties);
            }

            if (parent.Name == Word + "del")
            {
                foreach (var runText in parent.Descendants(Word + "t"))
                {
                    runText.Name = Word + "delText";
                }
            }

            EndBookmark(bookmark, bookmarkId, parent);
            EndComment(comment, parent);
        }

        /// <summary>Builds a table and materializes row-span continuation cells.</summary>
        /// <param name="element">The HTML table.</param>
        /// <returns>The Word table.</returns>
        private XElement WriteTable(IElement element)
        {
            var tableProperties = Properties(element, "tblPr");
            ApplyTableStyles(element, tableProperties, false);
            var table = new XElement(Word + "tbl", tableProperties);
            var rows = TableRows(element);
            var active = new Dictionary<int, (int Remaining, int Span)>();
            var maximumColumns = 0;
            foreach (var sourceRow in rows)
            {
                var row = WriteTableRow(sourceRow, active, out var column);
                maximumColumns = Math.Max(maximumColumns, column);
                table.Add(row);
            }

            if (active.Count != 0)
            {
                throw new InvalidDataException("The HTML table contains a row span extending beyond its rows.");
            }

            var grid = element.GetAttribute("data-docx-grid") is { } encoded ? DecodeXml(encoded) : TableGrid(maximumColumns);
            table.Element(Word + "tblPr")!.AddAfterSelf(grid);
            return table;
        }

        /// <summary>Writes one row and its continuation cells.</summary>
        /// <param name="source">The HTML row.</param>
        /// <param name="active">The active vertical merges.</param>
        /// <param name="columns">The row width in grid columns.</param>
        /// <returns>The Word row.</returns>
        private XElement WriteTableRow(IElement source, Dictionary<int, (int Remaining, int Span)> active, out int columns)
        {
            var row = new XElement(Word + "tr", Properties(source, "trPr"));
            var cells = new List<IElement>();
            foreach (var child in source.Children)
            {
                if (child.LocalName is "td" or "th")
                {
                    cells.Add(child);
                }
            }

            var index = 0;
            columns = 0;
            while (index < cells.Count || HasRemainingMerge(active, columns))
            {
                if (active.TryGetValue(columns, out var merge))
                {
                    WriteMergeContinuation(row, active, columns, merge);
                    columns += merge.Span;
                }
                else if (index >= cells.Count)
                {
                    row.Add(new XElement(Word + "tc", new XElement(Word + "p")));
                    columns++;
                }
                else
                {
                    var cell = cells[index];
                    index++;
                    columns += WriteTableCell(row, cell, active, columns);
                }
            }

            return row;
        }

        /// <summary>Writes a cell and starts its vertical merge when needed.</summary>
        /// <param name="row">The Word row.</param>
        /// <param name="source">The HTML cell.</param>
        /// <param name="active">The merge state.</param>
        /// <param name="column">The current column.</param>
        /// <returns>The consumed grid columns.</returns>
        private int WriteTableCell(XElement row, IElement source, Dictionary<int, (int Remaining, int Span)> active, int column)
        {
            var span = PositiveInteger(source.GetAttribute("colspan"), 1);
            var rowSpan = PositiveInteger(source.GetAttribute("rowspan"), 1);
            for (var offset = 0; offset < span; offset++)
            {
                if (active.ContainsKey(column + offset))
                {
                    throw new InvalidDataException("The HTML table has overlapping merged cells.");
                }
            }

            var properties = Properties(source, "tcPr");
            ApplyTableStyles(source, properties, true);
            Set(properties, "gridSpan", span.ToString(CultureInfo.InvariantCulture));
            properties.Elements(Word + WriterVerticalMerge).Remove();
            if (rowSpan > 1)
            {
                Set(properties, WriterVerticalMerge, "restart");
                active[column] = (rowSpan - 1, span);
            }

            var cell = new XElement(Word + "tc", properties);
            WriteBlocks(source, cell);
            XElement? last = null;
            foreach (var child in cell.Elements())
            {
                last = child;
            }

            if (last?.Name != Word + "p")
            {
                cell.Add(new XElement(Word + "p"));
            }

            row.Add(cell);
            return span;
        }

        /// <summary>Writes an HTML list as numbered Word paragraphs.</summary>
        /// <param name="list">The source list.</param>
        /// <param name="target">The Word parent.</param>
        /// <param name="level">The nesting level.</param>
        private void WriteList(IElement list, XElement target, int level)
        {
            if (level > WriterMaximumListLevel)
            {
                throw new NotSupportedException("DOCX lists support at most nine nesting levels.");
            }

            var number = ListNumbering(list);
            foreach (var item in list.Children)
            {
                if (item.LocalName != "li")
                {
                    continue;
                }

                WriteListItem(item, target, level, number);
            }
        }

        /// <summary>Allocates a valid nine-level numbering definition.</summary>
        /// <param name="bullet">Whether the list uses bullets.</param>
        /// <param name="start">The optional first number.</param>
        /// <returns>The numbering identifier.</returns>
        private string CreateNumbering(bool bullet, string? start)
        {
            if (_numbering is null)
            {
                var relation = NumberingRelationship();
                _numberingPath = relation is not null ? ResolvePart(_path, relation.Target) : ResolvePart(_path, "numbering.xml");
                _numbering = _parts.TryGetValue(
                    _numberingPath,
                    out var bytes)
                    ? ReadXml(bytes).Root ?? throw new InvalidDataException("Empty numbering part.")
                    : new XElement(Word + "numbering");
                if (relation is null)
                {
                    _ = AddRelationship("numbering", "numbering.xml", false);
                }

                AddContentType(_numberingPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml");
            }

            var abstractId = NextNumericId(_numbering, "abstractNum", WriterAbstractNumberId);
            var numberId = NextNumericId(_numbering, "num", WriterNumberId);
            var definition = new XElement(
                Word + "abstractNum",
                new XAttribute(Word + WriterAbstractNumberId, abstractId),
                new XElement(Word + "multiLevelType", new XAttribute(Word + "val", "multilevel")));
            for (var level = 0; level <= WriterMaximumListLevel; level++)
            {
                definition.Add(NumberingLevel(level, bullet, start));
            }

            var firstNumber = _numbering.Element(Word + "num");
            if (firstNumber is null)
            {
                _numbering.Add(definition);
            }
            else
            {
                firstNumber.AddBeforeSelf(definition);
            }

            _numbering.Add(new XElement(
                Word + "num",
                new XAttribute(Word + WriterNumberId, numberId),
                new XElement(Word + WriterAbstractNumberId, new XAttribute(Word + "val", abstractId))));
            return numberId;
        }

        /// <summary>Finds the existing numbering relationship.</summary>
        /// <returns>The numbering relationship, when present.</returns>
        private PackageRelationship? NumberingRelationship()
        {
            foreach (var relation in _rels.Values)
            {
                if (relation.Type.EndsWith("/numbering", StringComparison.Ordinal))
                {
                    return relation;
                }
            }

            return null;
        }

        /// <summary>Writes section page geometry after its content.</summary>
        /// <param name="section">The HTML section.</param>
        /// <param name="target">The Word body.</param>
        private void WriteSection(IElement section, XElement target)
        {
            var properties = section.GetAttribute("data-docx-section-properties") is { } encoded
                ? new XElement(DecodeXml(encoded))
                : new XElement(Word + WriterSectionProperties);
            if (properties.Name != Word + WriterSectionProperties)
            {
                throw new InvalidDataException("Section metadata must contain section properties.");
            }

            if (section.GetAttribute("data-page-width") is { } width && section.GetAttribute("data-page-height") is { } height)
            {
                Replace(properties, new(Word + "pgSz", new XAttribute(Word + "w", PixelsTwips(width)), new XAttribute(Word + "h", PixelsTwips(height))));
            }

            WriteSectionMargins(section, properties);
            if (!properties.HasElements)
            {
                return;
            }

            target.Add(properties);
            _sections.Add((section.GetAttribute("data-section-id") ?? string.Empty, properties));
        }

        /// <summary>Creates an embedded DrawingML picture.</summary>
        /// <param name="image">The HTML image.</param>
        /// <returns>The drawing element.</returns>
        private XElement WriteImage(IElement image)
        {
            var (mime, extension, bytes) = ImagePayload(image);
            if (!FindExistingImage(bytes, out var target, out var relationship))
            {
                target = $"media/crisscross-{NextIdentifier()}.{extension}";
                while (_parts.ContainsKey(ResolvePart(_path, target)))
                {
                    target = $"media/crisscross-{NextIdentifier()}.{extension}";
                }

                var imagePath = ResolvePart(_path, target);
                _parts.Add(imagePath, bytes);
                AddContentType(imagePath, mime);
                relationship = AddRelationship("image", target, false);
            }

            var width = PixelsTwips(image.GetAttribute("width") ?? "100") * WriterEmusPerTwip;
            var height = PixelsTwips(image.GetAttribute("height") ?? "100") * WriterEmusPerTwip;
            return image.GetAttribute("data-docx-drawing") is { } preserved
                ? UpdateDrawing(preserved, relationship, width, height)
                : CreateDrawing(image, target, relationship, width, height);
        }

        /// <summary>Reuses embedded media so repeated document round trips do not grow the package.</summary>
        /// <param name="bytes">The image payload.</param>
        /// <param name="target">The reused target part.</param>
        /// <param name="identifier">The reused relationship identifier.</param>
        /// <returns>Whether the image already exists in this story.</returns>
        private bool FindExistingImage(byte[] bytes, out string target, out string identifier)
        {
            foreach (var relationship in _rels)
            {
                var relation = relationship.Value;
                if (relation.External || !relation.Type.EndsWith("/image", StringComparison.Ordinal))
                {
                    continue;
                }

                if (_parts.TryGetValue(ResolvePart(_path, relation.Target), out var existing) && bytes.AsSpan().SequenceEqual(existing))
                {
                    target = relation.Target;
                    identifier = relationship.Key;
                    return true;
                }
            }

            target = string.Empty;
            identifier = string.Empty;
            return false;
        }

        /// <summary>Creates a standards-based inline picture drawing.</summary>
        /// <param name="image">The image HTML.</param>
        /// <param name="target">The image part.</param>
        /// <param name="relationship">The relationship identifier.</param>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <returns>The drawing.</returns>
        private XElement CreateDrawing(IElement image, string target, string relationship, long width, long height)
        {
            var drawingId = NextIdentifier();
            var transform = new XElement(
                Drawing + "xfrm",
                new XElement(Drawing + "off", new XAttribute("x", 0), new XAttribute("y", 0)),
                new XElement(Drawing + "ext", new XAttribute("cx", width), new XAttribute("cy", height)));
            var picture = new XElement(
                Picture + "pic",
                new XElement(
                    Picture + "nvPicPr",
                    new XElement(Picture + "cNvPr", new XAttribute("id", 0), new XAttribute("name", target)),
                    new XElement(Picture + "cNvPicPr")),
                new XElement(
                    Picture + "blipFill",
                    new XElement(Drawing + "blip", new XAttribute(Office + WriterImageEmbed, relationship)),
                    new XElement(Drawing + "stretch", new XElement(Drawing + "fillRect"))),
                new XElement(
                    Picture + "spPr",
                    transform,
                    new XElement(Drawing + "prstGeom", new XAttribute("prst", "rect"), new XElement(Drawing + "avLst"))));
            return new(
                Word + "drawing",
                new XElement(
                    WordDrawing + "inline",
                    new XElement(WordDrawing + "extent", new XAttribute("cx", width), new XAttribute("cy", height)),
                    new XElement(
                        WordDrawing + "docPr",
                        new XAttribute("id", drawingId),
                        new XAttribute("name", $"Image {drawingId}"),
                        new XAttribute("descr", image.GetAttribute("alt") ?? string.Empty)),
                    new XElement(Drawing + "graphic", new XElement(Drawing + "graphicData", new XAttribute("uri", Picture.NamespaceName), picture))));
        }

        /// <summary>Adds a relationship with a unique identifier.</summary>
        /// <param name="type">The Office relationship suffix.</param>
        /// <param name="target">The target.</param>
        /// <param name="external">Whether the target is external.</param>
        /// <returns>The relationship identifier.</returns>
        private string AddRelationship(string type, string target, bool external)
        {
            string id;
            do
            {
                id = $"rIdCrissCross{NextIdentifier()}";
            }
            while (_rels.ContainsKey(id));

            _rels.Add(id, new($"{Office}/{type}", target, external));
            return id;
        }

        /// <summary>Registers an explicit content type.</summary>
        /// <param name="path">The part path.</param>
        /// <param name="mime">The content type.</param>
        private void AddContentType(string path, string mime)
        {
            XElement? existing = null;
            foreach (var item in _types.Elements(ContentTypes + "Override"))
            {
                if ((string?)item.Attribute("PartName") == $"/{path}")
                {
                    existing = item;
                    break;
                }
            }

            if (existing is null)
            {
                _types.Add(new XElement(ContentTypes + "Override", new XAttribute("PartName", $"/{path}"), new XAttribute(WriterContentType, mime)));
            }
            else
            {
                existing.SetAttributeValue(WriterContentType, mime);
            }
        }
    }
}
