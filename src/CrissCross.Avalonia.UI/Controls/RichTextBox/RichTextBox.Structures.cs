// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Edits named document structures while retaining HTML and undo history.</summary>
public partial class RichTextBox
{
    /// <summary>The section identifier attribute.</summary>
    private const string SectionIdentifierAttribute = "data-section-id";

    /// <summary>The note marker punctuation length.</summary>
    private const int NoteMarkerPunctuationLength = 2;

    /// <summary>Marks the selected content as a named bookmark.</summary>
    /// <param name="name">The unique bookmark name.</param>
    public void AddBookmark(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        EnsureUniqueStructure("data-bookmark", name);
        if (!HasSelection && IsFormattingEnabled && !IsReadOnlyInternal)
        {
            var before = CaptureHistory();
            var offset = Math.Min(SelectionStart, SelectionEnd);
            var sourceOffset = HtmlTextProjection.Create(Document.Text).GetSourceInsertionOffset(offset);
            SetHtmlCore(Document.Text.Insert(sourceOffset, $"<span data-bookmark=\"{WebUtility.HtmlEncode(name)}\"></span>"), resetUndo: false);
            Select(offset, 0);
            CommitHistory(before);
            return;
        }

        WrapStructureSelection($"<span data-bookmark=\"{WebUtility.HtmlEncode(name)}\">", "</span>");
    }

    /// <summary>Selects the content of a named bookmark.</summary>
    /// <param name="name">The bookmark name.</param>
    /// <returns>Whether the bookmark exists.</returns>
    public bool NavigateToBookmark(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var parsed = ParseStructures();
        var first = int.MaxValue;
        var last = 0;
        foreach (var bookmark in parsed.QuerySelectorAll("[data-bookmark]"))
        {
            if (bookmark.GetAttribute("data-bookmark") != name || bookmark.SourceReference is not { } reference)
            {
                continue;
            }

            var sourceStart = Document.Text.IndexOf('>', reference.Position.Index) + 1;
            var offset = HtmlTextProjection.Create(Document.Text[..sourceStart]).Length;
            first = Math.Min(first, offset);
            last = Math.Max(last, offset + HtmlTextProjection.Create(bookmark.InnerHtml).Length);
        }

        if (first == int.MaxValue)
        {
            return false;
        }

        Select(first, last - first);
        return true;
    }

    /// <summary>Attaches a highlighted comment to selected content.</summary>
    /// <param name="id">The unique comment identifier.</param>
    /// <param name="author">The comment author.</param>
    /// <param name="text">The comment text shown in the annotation title.</param>
    public void AddComment(string id, string author, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(author);
        ArgumentNullException.ThrowIfNull(text);
        EnsureUniqueStructure("data-comment-id", id);
        var attributes = $"data-comment-id=\"{WebUtility.HtmlEncode(id)}\" data-author=\"{WebUtility.HtmlEncode(author)}\" title=\"{WebUtility.HtmlEncode(text)}\"";
        WrapStructureSelection($"<mark {attributes} style=\"background-color:#fff2a8\">", "</mark>");
    }

    /// <summary>Removes a comment and its highlight, retaining the annotated text.</summary>
    /// <param name="id">The comment identifier.</param>
    public void RemoveComment(string id) => ResolveStructure("data-comment-id", id, removeContent: false);

    /// <summary>Replaces the selection with an explicitly tracked revision.</summary>
    /// <param name="id">The unique revision identifier.</param>
    /// <param name="author">The revision author.</param>
    /// <param name="replacement">The replacement plain text.</param>
    public void ReplaceSelectionWithRevision(string id, string author, string replacement)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(author);
        ArgumentNullException.ThrowIfNull(replacement);
        EnsureUniqueStructure("data-revision-id", id);
        CommitRevision(id, author, WebUtility.HtmlEncode(replacement));
    }

    /// <summary>Accepts a tracked revision, removing deleted text and retaining inserted text.</summary>
    /// <param name="id">The revision identifier.</param>
    public void AcceptRevision(string id) => ResolveRevision(id, accept: true);

    /// <summary>Rejects a tracked revision, restoring deleted text and removing inserted text.</summary>
    /// <param name="id">The revision identifier.</param>
    public void RejectRevision(string id) => ResolveRevision(id, accept: false);

    /// <summary>Inserts a reference and an editable note at the document end.</summary>
    /// <param name="id">The unique note identifier.</param>
    /// <param name="text">The note plain text.</param>
    public void InsertNote(string id, string text) => InsertNote(id, text, isEndnote: false);

    /// <summary>Inserts a reference and an editable note at the document end.</summary>
    /// <param name="id">The unique note identifier.</param>
    /// <param name="text">The note plain text.</param>
    /// <param name="isEndnote">Whether the note is an endnote rather than a footnote.</param>
    public void InsertNote(string id, string text, bool isEndnote)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(text);
        EnsureUniqueStructure("id", $"note-{id}");
        if (IsReadOnlyInternal)
        {
            return;
        }

        var before = CaptureHistory();
        var safeId = WebUtility.HtmlEncode(id);
        var start = Math.Min(SelectionStart, SelectionEnd);
        Document.Replace(start, Math.Abs(SelectionEnd - SelectionStart), $"<sup><a href=\"#note-{safeId}\">[{safeId}]</a></sup>");
        var tag = isEndnote ? "aside" : "template";
        var kind = isEndnote ? "endnote" : "footnote";
        Document.AppendText($"<{tag} id=\"note-{safeId}\" data-note-kind=\"{kind}\"><p>[{safeId}] {WebUtility.HtmlEncode(text)}</p></{tag}>");
        SyncTextFromDocument();
        Select(start + id.Length + NoteMarkerPunctuationLength, 0);
        CommitHistory(before);
    }

    /// <summary>Replaces the content of an existing footnote or endnote.</summary>
    /// <param name="id">The note identifier.</param>
    /// <param name="text">The replacement plain text.</param>
    public void SetNoteText(string id, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(text);
        MutateStructures(document =>
        {
            var note = FindStructure(document, "id", $"note-{id}") ?? throw new ArgumentException("The note does not exist.", nameof(id));
            if (note is IHtmlTemplateElement template)
            {
                template.Content.TextContent = string.Empty;
            }

            note.InnerHtml = $"<p>[{WebUtility.HtmlEncode(id)}] {WebUtility.HtmlEncode(text)}</p>";
        });
    }
}
