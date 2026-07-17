// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the Formatting members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>Determines whether the document should be serialized.</summary>
    /// <returns><see langword="true"/> when the current document has content.</returns>
    public bool ShouldSerializeDocument() => !string.IsNullOrWhiteSpace(Document.PlainText);

    /// <summary>Applies bold formatting to the selection.</summary>
    public void ToggleBold() => ToggleBoldCommand.Execute(null);

    /// <summary>Applies italic formatting to the selection.</summary>
    public void ToggleItalic() => ToggleItalicCommand.Execute(null);

    /// <summary>Applies underline formatting to the selection.</summary>
    public void ToggleUnderline() => ToggleUnderlineCommand.Execute(null);

    /// <summary>Applies strikethrough formatting to the selection.</summary>
    public void ToggleStrikethrough() => ToggleStrikethroughCommand.Execute(null);

    /// <summary>Applies a font family to the selected content.</summary>
    /// <param name="fontFamily">The font family name.</param>
    public void SetSelectionFontFamily(string fontFamily)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fontFamily);
        ApplySpanStyleToSelection($"font-family:{fontFamily};");
    }

    /// <summary>Applies a font size to the selected content.</summary>
    /// <param name="fontSize">The font size in pixels.</param>
    public void SetSelectionFontSize(double fontSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);

        ApplySpanStyleToSelection($"font-size:{fontSize}px;");
    }

    /// <summary>Applies a foreground color to the selected content.</summary>
    /// <param name="color">The text color.</param>
    public void SetSelectionForeground(in Color color) =>
        ApplySpanStyleToSelection($"color:{color};");

    /// <summary>Applies a highlight color to the selected content.</summary>
    /// <param name="color">The highlight color.</param>
    public void SetSelectionHighlight(in Color color) =>
        ApplySpanStyleToSelection($"background-color:{color};");

    /// <summary>Selects all text.</summary>
    public void SelectAll() => SelectAllCommand.Execute(null);

    /// <summary>Selects a range by offset and length.</summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    public void Select(int startIndex, int length)
    {
        var max = Document.Length;
        var start = Math.Clamp(startIndex, 0, max);
        var end = Math.Clamp(start + length, 0, max);
        SelectionStart = Math.Min(start, end);
        SelectionEnd = Math.Max(start, end);
        CaretIndex = SelectionEnd;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        NotifyCommandStateChanged();
    }

    /// <summary>Selects a range using flow document text pointers.</summary>
    /// <param name="start">The selection start.</param>
    /// <param name="end">The selection end.</param>
    public void Select(TextPointer start, TextPointer end)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentNullException.ThrowIfNull(end);
        if (!ReferenceEquals(start.Document, Document) || !ReferenceEquals(end.Document, Document))
        {
            throw new InvalidOperationException("Selection pointers must belong to this document.");
        }

        var min = Math.Min(start.Offset, end.Offset);
        var max = Math.Max(start.Offset, end.Offset);
        SelectionStart = min;
        SelectionEnd = max;
        CaretIndex = max;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        NotifyCommandStateChanged();
    }

    /// <summary>Clears all text.</summary>
    public void Clear()
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        var before = CaptureHistory();
        SetHtmlCore(string.Empty, resetUndo: false);
        CommitHistory(before);
    }

    /// <summary>Clears all formatting.</summary>
    public void ClearFormatting() => ClearFormattingCommand.Execute(null);

    /// <summary>Appends plain text to the end of the document.</summary>
    /// <param name="text">The text to append.</param>
    public void AppendText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var encoded = HtmlClipboardUtilities.EncodePlainText(text);
        Document.AppendText(encoded);
        SyncTextFromDocument();
    }

    /// <summary>Appends HTML content to the end of the document.</summary>
    /// <param name="html">The html fragment to append.</param>
    public void AppendHtml(string html)
    {
        ArgumentNullException.ThrowIfNull(html);
        Document.AppendText(html);
        SyncTextFromDocument();
    }

    /// <summary>Replaces the current content with HTML.</summary>
    /// <param name="html">The html content.</param>
    public void SetHtml(string? html) => SetHtmlCore(html, resetUndo: true);

    /// <summary>Replaces the current content with plain text.</summary>
    /// <param name="text">The plain text content.</param>
    public void SetPlainText(string? text) => SetHtml(HtmlClipboardUtilities.EncodePlainText(text));

    /// <summary>Replaces the current content with markdown converted to HTML.</summary>
    /// <param name="markdown">The markdown content.</param>
    public void SetMarkdown(string? markdown) => SetHtml(MarkdownUtilities.ToHtml(markdown));

    /// <summary>Replaces the current selection with plain text.</summary>
    /// <param name="text">The replacement text.</param>
    public void ReplaceSelection(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ReplaceSelectionWithHtml(HtmlClipboardUtilities.EncodePlainText(text));
    }

    /// <summary>Replaces the current selection with HTML.</summary>
    /// <param name="html">The replacement html content.</param>
    public void ReplaceSelectionWithHtml(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        if (IsReadOnlyInternal)
        {
            return;
        }

        var before = CaptureHistory();
        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        Document.Replace(start, length, html);
        SyncTextFromDocument();

        var insertedLength = HtmlTextProjection.Create(html).Length;
        var newCaret = Math.Clamp(start + insertedLength, 0, Document.Length);
        SelectionStart = newCaret;
        SelectionEnd = newCaret;
        CaretIndex = newCaret;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        CommitHistory(before);
    }
}
