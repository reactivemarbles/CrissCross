// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Tracks editor mutations as reviewable insertions and deletions.</summary>
public partial class RichTextBox
{
    /// <summary>Identifies automatic revision tracking.</summary>
    public static readonly StyledProperty<bool> TrackChangesProperty = AvaloniaProperty.Register<RichTextBox, bool>(nameof(TrackChanges));

    /// <summary>Identifies the author assigned to new revisions.</summary>
    public static readonly StyledProperty<string> RevisionAuthorProperty = AvaloniaProperty.Register<RichTextBox, string>(nameof(RevisionAuthor), string.Empty);

    /// <summary>The closing deletion annotation markup.</summary>
    private const string DeletionClosingMarkup = "</del>";

    /// <summary>Gets or sets whether editor replacements create tracked revisions.</summary>
    public bool TrackChanges { get => GetValue(TrackChangesProperty); set => SetValue(TrackChangesProperty, value); }

    /// <summary>Gets or sets the author assigned to tracked revisions.</summary>
    public string RevisionAuthor { get => GetValue(RevisionAuthorProperty); set => SetValue(RevisionAuthorProperty, value); }

    /// <summary>Applies an automatically tracked replacement when tracking is enabled.</summary>
    /// <param name="html">The replacement HTML.</param>
    /// <returns>Whether tracking handled the replacement.</returns>
    private bool TryReplaceTrackedSelection(string html)
    {
        if (!TrackChanges)
        {
            return false;
        }

        CommitRevision(Guid.NewGuid().ToString("N"), RevisionAuthor, html);
        return true;
    }

    /// <summary>Commits an explicit or automatically tracked replacement.</summary>
    /// <param name="id">The revision identifier.</param>
    /// <param name="author">The revision author.</param>
    /// <param name="replacementHtml">The replacement HTML.</param>
    private void CommitRevision(string id, string author, string replacementHtml)
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        var attributes = $"data-revision-id=\"{WebUtility.HtmlEncode(id)}\" data-author=\"{WebUtility.HtmlEncode(author)}\"";
        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        var deleted = HtmlFormattingHelper.WrapMarkup(Document.Text, start, length, $"<del {attributes}>", DeletionClosingMarkup);
        var insertion = string.IsNullOrEmpty(replacementHtml) ? string.Empty : $"<ins {attributes} style=\"text-decoration:underline;color:#008000\">{replacementHtml}</ins>";
        var insertionOffset = HtmlTextProjection.Create(deleted).GetSourceInsertionOffset(start + length);
        if (deleted.AsSpan(insertionOffset).StartsWith(DeletionClosingMarkup, StringComparison.OrdinalIgnoreCase))
        {
            insertionOffset += DeletionClosingMarkup.Length;
        }

        var before = CaptureHistory();
        SetHtmlCore(deleted.Insert(insertionOffset, insertion), resetUndo: false);
        var insertedLength = HtmlTextProjection.Create(replacementHtml).Length;
        Select(Math.Min(start + length + insertedLength, Document.Length), 0);
        CommitHistory(before);
    }
}
