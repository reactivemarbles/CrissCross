// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reads and writes WordprocessingML packages without Office or reflection.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The maximum expanded package size.</summary>
    private const long MaximumPackageBytes = 64 * 1024 * 1024;

    /// <summary>The bounded ZIP expansion buffer size.</summary>
    private const int PackageCopyBufferBytes = 81_920;

    /// <summary>The default page width in twips.</summary>
    private const int DefaultPageWidthTwips = 12_240;

    /// <summary>The default page height in twips.</summary>
    private const int DefaultPageHeightTwips = 15_840;

    /// <summary>The default page margin in twips.</summary>
    private const int DefaultPageMarginTwips = 1_440;

    /// <summary>The relationship type attribute in the package protocol.</summary>
    private const string RelationshipTypeAttribute = "Type";

    /// <summary>The WordprocessingML namespace.</summary>
    private static readonly XNamespace Word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    /// <summary>The Office relationship namespace.</summary>
    private static readonly XNamespace Office = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    /// <summary>The package relationship namespace.</summary>
    private static readonly XNamespace Relationships = "http://schemas.openxmlformats.org/package/2006/relationships";

    /// <summary>The content types namespace.</summary>
    private static readonly XNamespace ContentTypes = "http://schemas.openxmlformats.org/package/2006/content-types";

    /// <summary>The DrawingML namespace.</summary>
    private static readonly XNamespace Drawing = "http://schemas.openxmlformats.org/drawingml/2006/main";

    /// <summary>The Word drawing namespace.</summary>
    private static readonly XNamespace WordDrawing = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";

    /// <summary>The picture namespace.</summary>
    private static readonly XNamespace Picture = "http://schemas.openxmlformats.org/drawingml/2006/picture";

    /// <summary>Reads a DOCX stream with a bound before allocating its complete package.</summary>
    /// <param name="source">The caller-owned DOCX stream.</param>
    /// <returns>The editable HTML document.</returns>
    internal static string ToHtml(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);
        using var package = new MemoryStream();
        var buffer = new byte[PackageCopyBufferBytes];
        var read = source.Read(buffer);
        while (read != 0)
        {
            if (package.Length + read > MaximumPackageBytes)
            {
                throw new InvalidDataException("The DOCX package exceeds the 64 MiB import limit.");
            }

            package.Write(buffer.AsSpan(0, read));
            read = source.Read(buffer);
        }

        return ToHtml(package.ToArray());
    }

    /// <summary>Imports the main story and preserves untouched auxiliary parts.</summary>
    /// <param name="data">The DOCX ZIP package.</param>
    /// <returns>Editable HTML containing package metadata.</returns>
    internal static string ToHtml(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var parts = ReadPackage(data);
        var path = MainPart(parts);
        var document = ReadXml(parts[path]);
        var body = document.Root?.Element(Word + "body") ?? throw new InvalidDataException("The DOCX document has no WordprocessingML body.");
        var rels = ReadRelationships(parts, path);
        var styles = ReadStyleDefinitions(parts, path, rels);
        var output = new StringBuilder();
        ReadSections(body, output, parts, path, rels, styles);
        ReadDocumentNotes(output, parts, path, rels, styles);

        _ = output.Append("<template data-docx-package=\"").Append(Convert.ToBase64String(data)).Append("\"></template>");
        return output.ToString();
    }

    /// <summary>Exports editable HTML as an Office Open XML Word document.</summary>
    /// <param name="html">The HTML fragment.</param>
    /// <returns>The DOCX ZIP package.</returns>
    internal static byte[] ToDocx(string html)
    {
        ArgumentNullException.ThrowIfNull(html);
        var document = new HtmlParser().ParseDocument(HtmlDocumentNormalizer.Normalize(html));
        var metadata = document.QuerySelector("template[data-docx-package]")?.GetAttribute("data-docx-package");
        var parts = metadata is null ? new Dictionary<string, byte[]>(StringComparer.Ordinal) : ReadPackage(Convert.FromBase64String(metadata));
        var path = parts.Count == 0 ? "word/document.xml" : MainPart(parts);
        var writer = new PackageWriter(parts, path);
        writer.PrepareAnnotations(document.Body);
        var body = new XElement(Word + "body");
        if (document.Body is not null)
        {
            writer.WriteBlocks(document.Body, body);
        }

        if (body.FirstNode is null)
        {
            body.Add(new XElement(Word + "p"));
        }

        writer.Complete();

        FinalizeBodySections(body);
        var root = parts.TryGetValue(path, out var original)
            ? ReadXml(original).Root ?? throw new InvalidDataException("Empty DOCX document.")
            : new XElement(Word + "document", new XAttribute(XNamespace.Xmlns + "w", Word), new XAttribute(XNamespace.Xmlns + "r", Office));
        root.ReplaceNodes(body);
        parts[path] = XmlBytes(root);
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            foreach (var part in parts)
            {
                using var destination = zip.CreateEntry(part.Key, CompressionLevel.Optimal).Open();
                destination.Write(part.Value);
            }
        }

        return stream.ToArray();
    }

    /// <summary>Places the final section properties at the end of the story.</summary>
    /// <param name="body">The exported story.</param>
    private static void FinalizeBodySections(XElement body)
    {
        XElement? last = null;
        foreach (var section in body.Elements(Word + "sectPr"))
        {
            last?.ReplaceWith(new XElement(Word + "p", new XElement(Word + "pPr", new XElement(last))));

            last = section;
        }

        last?.Remove();
        body.Add(last ?? CreateDefaultSection());
    }

    /// <summary>Creates default letter-size page geometry.</summary>
    /// <returns>The section properties.</returns>
    private static XElement CreateDefaultSection() => new(
        Word + "sectPr",
        new XElement(Word + "pgSz", new XAttribute(Word + "w", DefaultPageWidthTwips), new XAttribute(Word + "h", DefaultPageHeightTwips)),
        new XElement(
            Word + "pgMar",
            new XAttribute(Word + "top", DefaultPageMarginTwips),
            new XAttribute(Word + "bottom", DefaultPageMarginTwips),
            new XAttribute(Word + "left", DefaultPageMarginTwips),
            new XAttribute(Word + "right", DefaultPageMarginTwips)));

    /// <summary>Reads a bounded package, rejecting duplicate and unsafe part names.</summary>
    /// <param name="data">The package bytes.</param>
    /// <returns>The uncompressed parts.</returns>
    private static Dictionary<string, byte[]> ReadPackage(byte[] data)
    {
        if (data.LongLength > MaximumPackageBytes)
        {
            throw new InvalidDataException("The DOCX package exceeds the 64 MiB import limit.");
        }

        using var stream = new MemoryStream(data, false);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        var parts = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        long total = 0;
        foreach (var entry in zip.Entries)
        {
            if (entry.FullName.EndsWith('/'))
            {
                continue;
            }

            total += entry.Length;
            if (total > MaximumPackageBytes || IsUnsafePartName(entry.FullName) || parts.ContainsKey(entry.FullName))
            {
                throw new InvalidDataException("The DOCX package contains duplicate, unsafe, or oversized parts.");
            }

            using var source = entry.Open();
            using var destination = new MemoryStream();
            CopyPackagePart(source, destination, entry.Length);
            parts.Add(entry.FullName, destination.ToArray());
        }

        if (!parts.ContainsKey("[Content_Types].xml"))
        {
            throw new InvalidDataException("The DOCX package is missing its content types.");
        }

        ValidatePackage(parts);

        return parts;
    }

    /// <summary>Enforces actual ZIP expansion limits rather than trusting declared entry sizes.</summary>
    /// <param name="source">The compressed entry stream.</param>
    /// <param name="destination">The expanded entry destination.</param>
    /// <param name="expectedLength">The declared uncompressed size.</param>
    private static void CopyPackagePart(Stream source, MemoryStream destination, long expectedLength)
    {
        var buffer = new byte[PackageCopyBufferBytes];
        var read = source.Read(buffer);
        while (read != 0)
        {
            if (destination.Length + read > expectedLength || destination.Length + read > MaximumPackageBytes)
            {
                throw new InvalidDataException("The DOCX entry expands beyond its declared or allowed size.");
            }

            destination.Write(buffer.AsSpan(0, read));
            read = source.Read(buffer);
        }

        if (destination.Length == expectedLength)
        {
            return;
        }

        throw new InvalidDataException("The DOCX entry length does not match its package metadata.");
    }

    /// <summary>Checks for absolute paths and traversal segments.</summary>
    /// <param name="name">The ZIP entry name.</param>
    /// <returns>Whether the entry name is unsafe.</returns>
    private static bool IsUnsafePartName(string name)
    {
        if (name.StartsWith('/') || name.Contains('\\'))
        {
            return true;
        }

        foreach (var segment in name.Split('/'))
        {
            if (segment is "." or "..")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Rejects active Office content and external resources without accessing them.</summary>
    /// <param name="parts">The package entries.</param>
    private static void ValidatePackage(Dictionary<string, byte[]> parts)
    {
        var types = ReadXml(parts["[Content_Types].xml"]);
        if (types.Root?.Name != ContentTypes + "Types")
        {
            throw new InvalidDataException("The package content types namespace is invalid.");
        }

        foreach (var type in types.Root.Elements())
        {
            var mime = ((string?)type.Attribute("ContentType") ?? string.Empty).ToLowerInvariant();
            if (IsActiveContentType(mime))
            {
                throw new NotSupportedException("Macro, ActiveX, and embedded OLE content are not supported by the DOCX editor.");
            }
        }

        foreach (var part in parts)
        {
            if (!part.Key.EndsWith(".rels", StringComparison.Ordinal))
            {
                continue;
            }

            ValidateRelationships(part.Value);
        }
    }

    /// <summary>Checks content types for active Office payloads.</summary>
    /// <param name="mime">The lowercase content type.</param>
    /// <returns>Whether the payload is active.</returns>
    private static bool IsActiveContentType(string mime) =>
        mime.Contains("macroenabled", StringComparison.Ordinal) || mime.Contains("vbaproject", StringComparison.Ordinal)
        || mime.Contains("activex", StringComparison.Ordinal) || mime.Contains("oleobject", StringComparison.Ordinal);

    /// <summary>Validates all relationships in one package part.</summary>
    /// <param name="bytes">The relationships XML.</param>
    private static void ValidateRelationships(byte[] bytes)
    {
        var relationships = ReadXml(bytes);
        if (relationships.Root?.Name != Relationships + "Relationships")
        {
            throw new InvalidDataException("The package relationships namespace is invalid.");
        }

        foreach (var relationship in relationships.Root.Elements())
        {
            var type = (string?)relationship.Attribute(RelationshipTypeAttribute) ?? string.Empty;
            if (IsActiveRelationship(type))
            {
                throw new NotSupportedException("Active embedded content and attached templates are not supported by the DOCX editor.");
            }

            if ((string?)relationship.Attribute("TargetMode") == "External" && !type.EndsWith("/hyperlink", StringComparison.Ordinal))
            {
                throw new NotSupportedException("External DOCX resources are not imported; embed images and document resources first.");
            }
        }
    }

    /// <summary>Checks relationship types for active embedded content.</summary>
    /// <param name="type">The relationship type.</param>
    /// <returns>Whether the relationship imports active content.</returns>
    private static bool IsActiveRelationship(string type) =>
        type.EndsWith("/oleObject", StringComparison.Ordinal) || type.EndsWith("/package", StringComparison.Ordinal)
        || type.EndsWith("/control", StringComparison.Ordinal) || type.EndsWith("/attachedTemplate", StringComparison.Ordinal)
        || type.EndsWith("/aFChunk", StringComparison.Ordinal);

    /// <summary>Loads XML with DTD processing disabled and an explicit size limit.</summary>
    /// <param name="data">The XML bytes.</param>
    /// <returns>The parsed XML.</returns>
    private static XDocument ReadXml(byte[] data)
    {
        using var stream = new MemoryStream(data, false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = MaximumPackageBytes });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    /// <summary>Encodes XML as UTF-8.</summary>
    /// <param name="element">The XML root.</param>
    /// <returns>The XML bytes.</returns>
    private static byte[] XmlBytes(XElement element)
    {
        OrderWordProperties(element);
        return Encoding.UTF8.GetBytes(new XDocument(new XDeclaration("1.0", "utf-8", "yes"), element).ToString(SaveOptions.DisableFormatting));
    }

    /// <summary>Locates the main story through the package root relationships.</summary>
    /// <param name="parts">The package parts.</param>
    /// <returns>The main document path.</returns>
    private static string MainPart(Dictionary<string, byte[]> parts)
    {
        var rels = ReadRelationships(parts, string.Empty);
        PackageRelationship? main = null;
        foreach (var relation in rels.Values)
        {
            if (relation.Type.EndsWith("/officeDocument", StringComparison.Ordinal))
            {
                main = relation;
                break;
            }
        }

        if (main?.External != false)
        {
            throw new InvalidDataException("The package has no internal Office document relationship.");
        }

        var path = ResolvePart(string.Empty, main.Target);
        if (!parts.ContainsKey(path))
        {
            throw new InvalidDataException("The main DOCX document part is missing.");
        }

        return path;
    }

    /// <summary>Resolves a package-relative relationship without file system access.</summary>
    /// <param name="source">The owning part path.</param>
    /// <param name="target">The relationship target.</param>
    /// <returns>The normalized package part path.</returns>
    private static string ResolvePart(string source, string target)
    {
        var baseUri = new Uri($"https://package.invalid/{source}", UriKind.Absolute);
        var resolved = new Uri(baseUri, target);
        if (resolved.Host != "package.invalid" || resolved.Query.Length != 0 || resolved.Fragment.Length != 0)
        {
            throw new InvalidDataException("An internal DOCX relationship points outside the package.");
        }

        return Uri.UnescapeDataString(resolved.AbsolutePath.TrimStart('/'));
    }

    /// <summary>Gets a part's relationships path.</summary>
    /// <param name="path">The owning part.</param>
    /// <returns>The relationships part.</returns>
    private static string RelationshipPath(string path)
    {
        var slash = path.LastIndexOf('/');
        return path.Length == 0 ? "_rels/.rels" : $"{path[..(slash + 1)]}_rels/{path[(slash + 1)..]}.rels";
    }

    /// <summary>Reads relationships keyed by identifier.</summary>
    /// <param name="parts">The package.</param>
    /// <param name="path">The owning part.</param>
    /// <returns>The relationship lookup.</returns>
    private static Dictionary<string, PackageRelationship> ReadRelationships(Dictionary<string, byte[]> parts, string path)
    {
        var result = new Dictionary<string, PackageRelationship>(StringComparer.Ordinal);
        if (parts.TryGetValue(RelationshipPath(path), out var bytes))
        {
            foreach (var element in ReadXml(bytes).Root?.Elements(Relationships + "Relationship") ?? [])
            {
                var id = (string?)element.Attribute("Id") ?? throw new InvalidDataException("A DOCX relationship has no identifier.");
                result.Add(id, new(
                    (string?)element.Attribute(RelationshipTypeAttribute) ?? string.Empty,
                    (string?)element.Attribute("Target") ?? string.Empty,
                    (string?)element.Attribute("TargetMode") == "External"));
            }
        }

        return result;
    }

    /// <summary>Gets a Word attribute value.</summary>
    /// <param name="element">The source element.</param>
    /// <param name="name">The attribute name.</param>
    /// <returns>The attribute value.</returns>
    private static string? Value(XElement? element, string name = "val") => (string?)element?.Attribute(Word + name);

    /// <summary>Serializes original XML into an inert HTML attribute.</summary>
    /// <param name="element">The original XML.</param>
    /// <returns>The encoded XML.</returns>
    private static string EncodeXml(XElement element) => Convert.ToBase64String(Encoding.UTF8.GetBytes(element.ToString(SaveOptions.DisableFormatting)));

    /// <summary>Decodes preserved XML with the same import limits.</summary>
    /// <param name="encoded">The encoded XML.</param>
    /// <returns>The preserved element.</returns>
    private static XElement DecodeXml(string encoded) => ReadXml(Convert.FromBase64String(encoded)).Root ?? throw new InvalidDataException("Empty DOCX metadata.");

    /// <summary>Represents one package relationship.</summary>
    /// <param name="Type">The relationship type.</param>
    /// <param name="Target">The target part or URI.</param>
    /// <param name="External">Whether the target is external.</param>
    private sealed record PackageRelationship(string Type, string Target, bool External);
}
