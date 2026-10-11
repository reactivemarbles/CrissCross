// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Writes editable notes, comments, and page fields into Word stories.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>Composes annotation parts alongside the main story.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>The comment definition element name.</summary>
        private const string AnnotationComment = "comment";

        /// <summary>The paragraph element name.</summary>
        private const string AnnotationParagraph = "p";

        /// <summary>The text run element name.</summary>
        private const string AnnotationRun = "r";

        /// <summary>The Word annotation type attribute.</summary>
        private const string AnnotationTypeAttribute = "type";

        /// <summary>The editable note templates indexed by HTML identifier.</summary>
        private readonly Dictionary<string, (string Kind, string Id, IElement Element)> _notes = [with(StringComparer.Ordinal)];

        /// <summary>The comment identifiers assigned to HTML ranges.</summary>
        private readonly Dictionary<string, string> _commentIdentifiers = [with(StringComparer.Ordinal)];

        /// <summary>The assigned numeric note identifiers.</summary>
        private readonly HashSet<string> _noteIdentifiers = [with(StringComparer.Ordinal)];

        /// <summary>The comment definitions updated by the editor.</summary>
        private XElement? _comments;

        /// <summary>The comments package path.</summary>
        private string? _commentsPath;

        /// <summary>Writes one supported dynamic page field.</summary>
        /// <param name="element">The HTML inline.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>Whether a page field was written.</returns>
        private static bool WriteAnnotationPageField(IElement element, XElement target)
        {
            if (element.GetAttribute("data-page-field") is not { } field)
            {
                return false;
            }

            if (field is not ("PAGE" or "NUMPAGES"))
            {
                throw new NotSupportedException("DOCX page fields support PAGE and NUMPAGES.");
            }

            target.Add(new XElement(
                Word + "fldSimple",
                new XAttribute(Word + "instr", field),
                new XElement(Word + AnnotationRun, new XElement(Word + "t", element.TextContent))));
            return true;
        }

        /// <summary>Creates an Office note separator definition.</summary>
        /// <param name="kind">The note kind.</param>
        /// <param name="identifier">The reserved identifier.</param>
        /// <param name="type">The separator type.</param>
        /// <returns>The separator note.</returns>
        private static XElement NoteSeparator(string kind, string identifier, string type) =>
            new(
                Word + kind,
                new XAttribute(Word + AnnotationIdentifier, identifier),
                new XAttribute(Word + AnnotationTypeAttribute, type),
                new XElement(Word + AnnotationParagraph, new XElement(Word + AnnotationRun, new XElement(Word + type))));

        /// <summary>Assigns a note identifier without collisions with imported notes.</summary>
        /// <param name="kind">The note kind.</param>
        /// <param name="element">The note definition.</param>
        /// <returns>The numeric identifier.</returns>
        private string AssignNoteIdentifier(string kind, IElement element)
        {
            var numeric = element.GetAttribute("data-docx-note-id");
            if (numeric is not null)
            {
                if (!int.TryParse(numeric, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value <= 0
                    || !_noteIdentifiers.Add($"{kind}:{numeric}"))
                {
                    throw new InvalidDataException("DOCX note identifiers must be unique positive integers.");
                }

                return numeric;
            }

            do
            {
                numeric = NextIdentifier();
            }
            while (!_noteIdentifiers.Add($"{kind}:{numeric}"));

            return numeric;
        }

        /// <summary>Writes note references and dynamic page number fields.</summary>
        /// <param name="element">The HTML inline.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>Whether an annotation was written.</returns>
        private bool WriteAnnotationInline(IElement element, XElement target)
        {
            if (element.LocalName == "a" && element.GetAttribute("href") is { } href && href.StartsWith('#')
                && _notes.TryGetValue(href[1..], out var note))
            {
                target.Add(new XElement(Word + AnnotationRun, new XElement(Word + $"{note.Kind}Reference", new XAttribute(Word + AnnotationIdentifier, note.Id))));
                return true;
            }

            return WriteAnnotationPageField(element, target);
        }

        /// <summary>Writes changed note and comment parts.</summary>
        private void WriteAnnotationStories()
        {
            foreach (var kind in AnnotationNoteKinds)
            {
                WriteNotes(kind);
            }

            if (_comments is null || _commentsPath is null)
            {
                return;
            }

            _parts[_commentsPath] = XmlBytes(_comments);
        }

        /// <summary>Rebuilds one note story from its editable definitions.</summary>
        /// <param name="kind">The footnote or endnote kind.</param>
        private void WriteNotes(string kind)
        {
            var definitions = new List<(string Kind, string Id, IElement Element)>();
            foreach (var note in _notes.Values)
            {
                if (note.Kind != kind)
                {
                    continue;
                }

                definitions.Add(note);
            }

            if (definitions.Count == 0)
            {
                return;
            }

            var target = $"{kind}s.xml";
            var path = EnsureAnnotationRelationship($"{kind}s", target);
            var root = new XElement(Word + $"{kind}s", NoteSeparator(kind, "-1", "separator"), NoteSeparator(kind, "0", "continuationSeparator"));
            var writer = new PackageWriter(_parts, path);
            foreach (var definition in definitions)
            {
                var note = new XElement(Word + kind, new XAttribute(Word + AnnotationIdentifier, definition.Id));
                writer.WriteBlocks(definition.Element is IHtmlTemplateElement template ? template.Content : definition.Element, note);
                if (!note.HasElements)
                {
                    note.Add(new XElement(Word + AnnotationParagraph));
                }

                root.Add(note);
            }

            writer.SaveRelationships();
            _parts[path] = XmlBytes(root);
        }

        /// <summary>Ensures an annotation story exists and has a relationship.</summary>
        /// <param name="kind">The story kind.</param>
        /// <param name="target">The default part name.</param>
        /// <returns>The resolved part path.</returns>
        private string EnsureAnnotationRelationship(string kind, string target)
        {
            foreach (var relationship in _rels.Values)
            {
                if (relationship.Type.EndsWith($"/{kind}", StringComparison.Ordinal))
                {
                    return ResolvePart(_path, relationship.Target);
                }
            }

            _ = AddRelationship(kind, target, false);
            var path = ResolvePart(_path, target);
            AddContentType(path, $"application/vnd.openxmlformats-officedocument.wordprocessingml.{kind}+xml");
            return path;
        }

        /// <summary>Begins an editable comment range and updates its story text.</summary>
        /// <param name="element">The HTML comment mark.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>The assigned comment identifier.</returns>
        private string? BeginComment(IElement element, XElement target)
        {
            if (element.GetAttribute("data-comment-id") is not { } identifier)
            {
                return null;
            }

            EnsureComments();
            ref var numeric = ref CollectionsMarshal.GetValueRefOrAddDefault(_commentIdentifiers, identifier, out var exists);
            if (!exists)
            {
                numeric = int.TryParse(identifier, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value >= 0
                    ? identifier
                    : NextNumericId(_comments!, AnnotationComment, AnnotationIdentifier);
            }

            var assignedNumeric = numeric!;
            var comment = FindReaderElement(_comments, Word + AnnotationComment, AnnotationIdentifier, assignedNumeric);
            if (comment is null)
            {
                comment = new(Word + AnnotationComment, new XAttribute(Word + AnnotationIdentifier, assignedNumeric));
                _comments!.Add(comment);
            }

            comment.SetAttributeValue(Word + "author", element.GetAttribute("data-author") ?? string.Empty);
            comment.SetAttributeValue(Word + "date", element.GetAttribute("data-docx-date"));
            comment.ReplaceNodes(new XElement(Word + AnnotationParagraph, new XElement(Word + AnnotationRun, new XElement(Word + "t", element.GetAttribute("title") ?? string.Empty))));
            StartCommentRange(assignedNumeric, target);
            return assignedNumeric;
        }

        /// <summary>Closes a comment range and emits its reference marker.</summary>
        /// <param name="identifier">The comment identifier.</param>
        /// <param name="target">The Word parent.</param>
        private void EndComment(string? identifier, XElement target)
        {
            if (identifier is null)
            {
                return;
            }

            FinishCommentRange(identifier, target);
        }

        /// <summary>Loads or creates the original comments story.</summary>
        private void EnsureComments()
        {
            if (_comments is not null)
            {
                return;
            }

            _commentsPath = EnsureAnnotationRelationship("comments", "comments.xml");
            _comments = _parts.TryGetValue(_commentsPath, out var bytes)
                ? ReadXml(bytes).Root ?? throw new InvalidDataException("Empty comments story.")
                : new XElement(Word + "comments");
        }

        /// <summary>Checks whether an imported reference was regenerated from an editable comment range.</summary>
        /// <param name="original">The original XML.</param>
        /// <returns>Whether the original reference should be omitted.</returns>
        private bool HasGeneratedCommentReference(XElement original)
        {
            if (original.Name != Word + "commentReference")
            {
                return false;
            }

            var identifier = Value(original, AnnotationIdentifier);
            foreach (var numeric in _commentIdentifiers.Values)
            {
                if (numeric == identifier)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
