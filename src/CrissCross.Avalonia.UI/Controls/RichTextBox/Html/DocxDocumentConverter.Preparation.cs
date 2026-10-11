// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Prepares document-wide annotation identities before story export.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>Discovers editable annotation definitions.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>Discovers note definitions before their references are exported.</summary>
        /// <param name="body">The HTML body.</param>
        internal void PrepareAnnotations(IElement? body)
        {
            if (body is null)
            {
                return;
            }

            PrepareRanges(body);
            foreach (var element in body.QuerySelectorAll("[data-note-kind]"))
            {
                var kind = element.GetAttribute("data-note-kind");
                var identifier = element.GetAttribute(AnnotationIdentifier);
                if (kind is not ("footnote" or "endnote") || string.IsNullOrEmpty(identifier))
                {
                    throw new InvalidDataException("DOCX notes require a footnote or endnote kind and a nonempty identifier.");
                }

                var numeric = AssignNoteIdentifier(kind, element);
                _notes.Add(identifier, (kind, numeric, element));
            }
        }
    }
}
