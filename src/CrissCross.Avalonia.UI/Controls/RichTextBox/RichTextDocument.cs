// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a document model for rich text content with formatting information.
/// </summary>
public class RichTextDocument
{
    private readonly List<TextSegment> _segments = [];
    private string _rawText = string.Empty;

    /// <summary>
    /// Gets the segments in the document.
    /// </summary>
    public IReadOnlyList<TextSegment> Segments => _segments;

    /// <summary>
    /// Gets the raw HTML content of the document.
    /// </summary>
    public string PlainText => _rawText;

    /// <summary>
    /// Gets the total length of the underlying HTML string.
    /// </summary>
    public int Length => _rawText.Length;

    /// <summary>
    /// Sets the document text, replacing existing content.
    /// </summary>
    /// <param name="text">The HTML or markdown text to set.</param>
    public void SetText(string? text)
    {
        _rawText = text ?? string.Empty;
        RebuildSegments();
    }

    /// <summary>
    /// Appends text to the end of the document.
    /// </summary>
    /// <param name="text">The HTML fragment to append.</param>
    public void AppendText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        _rawText += text;
        RebuildSegments();
    }

    /// <summary>
    /// Inserts text at the specified offset.
    /// </summary>
    /// <param name="offset">The insertion offset.</param>
    /// <param name="text">The HTML fragment to insert.</param>
    public void Insert(int offset, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var index = Math.Clamp(offset, 0, _rawText.Length);
        _rawText = _rawText.Insert(index, text);
        RebuildSegments();
    }

    /// <summary>
    /// Deletes text in the provided range.
    /// </summary>
    /// <param name="offset">The start offset.</param>
    /// <param name="length">The number of characters to delete.</param>
    public void Delete(int offset, int length)
    {
        if (length <= 0 || offset < 0 || offset >= _rawText.Length)
        {
            return;
        }

        var boundedLength = Math.Min(length, _rawText.Length - offset);
        _rawText = _rawText.Remove(offset, boundedLength);
        RebuildSegments();
    }

    /// <summary>
    /// Replaces a range with new text.
    /// </summary>
    /// <param name="offset">The start offset.</param>
    /// <param name="length">The length of the range to replace.</param>
    /// <param name="text">The replacement HTML.</param>
    public void Replace(int offset, int length, string? text)
    {
        Delete(offset, length);
        if (!string.IsNullOrEmpty(text))
        {
            Insert(offset, text);
        }
    }

    /// <summary>
    /// Toggles formatting for a range of text by wrapping the HTML selection with tags.
    /// </summary>
    /// <param name="start">The start offset.</param>
    /// <param name="length">The length of the range.</param>
    /// <param name="formatType">The formatting to toggle.</param>
    /// <returns>True when formatting ends up applied for the whole range.</returns>
    public bool ToggleFormatting(int start, int length, TextFormatType formatType)
    {
        var (content, applied) = HtmlFormattingHelper.Toggle(_rawText, start, length, formatType);
        if (content is null)
        {
            return false;
        }

        _rawText = content;
        RebuildSegments();
        return applied;
    }

    /// <summary>
    /// Clears all formatting information, keeping the textual content.
    /// </summary>
    public void ClearFormatting()
    {
        var plain = HtmlContentParser.ToPlainText(_rawText);
        _rawText = HtmlClipboardUtilities.EncodePlainText(plain);
        RebuildSegments();
    }

    private void RebuildSegments()
    {
        _segments.Clear();
        var parsed = HtmlContentParser.Parse(_rawText);
        if (parsed.Count == 0)
        {
            return;
        }

        _segments.AddRange(parsed);
    }
}
