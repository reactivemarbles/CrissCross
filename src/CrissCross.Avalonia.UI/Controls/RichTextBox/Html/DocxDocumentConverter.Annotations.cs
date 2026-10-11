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

/// <summary>Projects Word annotations into the editor's editable HTML contracts.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The annotation identifier attribute.</summary>
    private const string AnnotationIdentifier = "id";

    /// <summary>The footnote story kind.</summary>
    private const string AnnotationFootnote = "footnote";

    /// <summary>The bookmark start element name.</summary>
    private const string AnnotationBookmarkStart = "bookmarkStart";

    /// <summary>The supported note story kinds.</summary>
    private static readonly string[] AnnotationNoteKinds = [AnnotationFootnote, "endnote"];

    /// <summary>Reads note stories as editable metadata templates or visible endnotes.</summary>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The main story path.</param>
    /// <param name="rels">The main story relationships.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadDocumentNotes(
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        foreach (var kind in AnnotationNoteKinds)
        {
            var relation = FindReaderRelationship(rels, $"/{kind}s");
            if (relation?.External != false)
            {
                continue;
            }

            var notePath = ResolvePart(path, relation.Target);
            if (parts.TryGetValue(notePath, out var bytes))
            {
                ReadNoteStory(ReadXml(bytes), kind, output, parts, notePath, styles);
            }
        }
    }

    /// <summary>Reads the ordinary notes from one note story.</summary>
    /// <param name="document">The note XML.</param>
    /// <param name="kind">The note kind.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The note story path.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadNoteStory(
        XDocument document,
        string kind,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, XElement> styles)
    {
        foreach (var note in document.Root?.Elements(Word + kind) ?? [])
        {
            var identifier = Value(note, AnnotationIdentifier);
            if (!int.TryParse(identifier, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric) || numeric <= 0)
            {
                continue;
            }

            var tag = kind == AnnotationFootnote ? "template" : "aside";
            _ = output.Append('<').Append(tag);
            Attribute(output, AnnotationIdentifier, $"note-{kind}-{identifier}");
            Attribute(output, "data-note-kind", kind);
            Attribute(output, "data-docx-note-id", identifier);
            _ = output.Append('>');
            ReadBlocks(note.Elements(), output, parts, path, ReadRelationships(parts, path), styles);
            _ = output.Append("</").Append(tag).Append('>');
        }
    }

    /// <summary>Reads note references and generated page fields from run children.</summary>
    /// <param name="element">The Word run child.</param>
    /// <param name="output">The HTML destination.</param>
    /// <returns>Whether the child was handled.</returns>
    private static bool ReadAnnotationRunChild(XElement element, StringBuilder output)
    {
        if (element.Name == Word + "footnoteReference" || element.Name == Word + "endnoteReference")
        {
            var kind = element.Name == Word + "footnoteReference" ? AnnotationFootnote : "endnote";
            var identifier = Value(element, AnnotationIdentifier) ?? string.Empty;
            _ = output.Append("<sup><a");
            Attribute(output, "href", $"#note-{kind}-{identifier}");
            _ = output.Append(">[").Append(WebUtility.HtmlEncode(identifier)).Append("]</a></sup>");
            return true;
        }

        return element.Name == Word + "footnoteRef" || element.Name == Word + "endnoteRef";
    }

    /// <summary>Reads crossing and cross-paragraph ranges as balanced editable spans.</summary>
    /// <param name="paragraph">The Word paragraph.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The story path.</param>
    /// <param name="rels">The story relationships.</param>
    /// <param name="styles">The named styles.</param>
    private static void ReadAnnotatedParagraph(
        XElement paragraph,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        Dictionary<string, XElement> styles)
    {
        var complete = FindCompleteAnnotationRanges(FindAnnotationStory(paragraph));
        var active = FindActiveAnnotationRanges(paragraph, complete);
        HashSet<XElement> consumed = [];
        foreach (var child in paragraph.Elements())
        {
            if (child.Name == Word + "pPr" || consumed.Contains(child) || UpdateAnnotationRange(child, complete, active))
            {
                continue;
            }

            var closing = OpenAnnotationRanges(active, output, parts, path, rels);
            if (!ReadComplexPageField(child, output, consumed))
            {
                ReadInline(child, output, parts, path, rels, styles);
            }

            while (closing.TryPop(out var tag))
            {
                _ = output.Append(tag);
            }
        }
    }

    /// <summary>Reads editable comment text and author metadata.</summary>
    /// <param name="marker">The range start.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The story path.</param>
    /// <param name="rels">The relationships.</param>
    private static void ReadCommentStart(
        XElement marker,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels)
    {
        var identifier = Value(marker, AnnotationIdentifier);
        _ = output.Append("<mark");
        Attribute(output, "data-comment-id", identifier);
        var relation = FindReaderRelationship(rels, "/comments");
        if (relation is null || !parts.TryGetValue(ResolvePart(path, relation.Target), out var bytes))
        {
            return;
        }

        var comment = FindReaderElement(ReadXml(bytes).Root, Word + "comment", AnnotationIdentifier, identifier);
        Attribute(output, "data-author", Value(comment, "author"));
        Attribute(output, "data-docx-date", Value(comment, "date"));
        var text = new StringBuilder();
        foreach (var run in comment?.Descendants(Word + "t") ?? [])
        {
            _ = text.Append(run.Value);
        }

        Attribute(output, "title", text.ToString());
    }
}
