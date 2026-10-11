// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Resolves footnotes against their body reference positions.</summary>
internal static class RichDocumentFootnotes
{
    /// <summary>Reads rendered footnotes in a body section.</summary>
    /// <param name="html">The source document.</param>
    /// <param name="section">The section range.</param>
    /// <returns>The referenced note bodies.</returns>
    internal static List<Note> Read(string html, RichDocumentStructure.Section section)
    {
        var document = new HtmlParser(new HtmlParserOptions { IsKeepingSourceReferences = true }).ParseDocument(html);
        var bodies = ReadBodies(document);
        var notes = new List<Note>();
        foreach (var element in document.QuerySelectorAll("a[href]"))
        {
            var href = element.GetAttribute("href");
            if (href is not { Length: > 1 } || href[0] != '#' || element.SourceReference is not { } source || !bodies.TryGetValue(href[1..], out var body))
            {
                continue;
            }

            var offset = HtmlTextProjection.Create(html[..source.Position.Index]).Length;
            if (offset >= section.Start && offset < section.End)
            {
                notes.Add(new(offset, body));
            }
        }

        return notes;
    }

    /// <summary>Reads note bodies stored outside body coordinates.</summary>
    /// <param name="document">The parsed source.</param>
    /// <returns>The note bodies by identifier.</returns>
    private static Dictionary<string, string> ReadBodies(AngleSharp.Dom.IDocument document)
    {
        var bodies = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var element in document.QuerySelectorAll("template[data-note-kind]"))
        {
            if (element.GetAttribute("data-note-kind") == "footnote" && element.Id is { Length: > 0 } id)
            {
                bodies[id] = element is IHtmlTemplateElement template ? template.Content.ToHtml() : element.InnerHtml;
            }
        }

        return bodies;
    }

    /// <summary>A footnote and the insertion position of its reference.</summary>
    /// <param name="Offset">The reference body offset.</param>
    /// <param name="Html">The note body.</param>
    internal sealed record Note(int Offset, string Html);
}
