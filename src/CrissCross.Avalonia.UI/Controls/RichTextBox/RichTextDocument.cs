// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a document model for rich text content with formatting information.
/// </summary>
public class RichTextDocument
{
    private readonly List<TextSegment> _segments = [];
    private string _rawText = string.Empty;
    private HtmlTextProjection _projection = HtmlTextProjection.Create(string.Empty);

    /// <summary>
    /// Gets the segments in the document.
    /// </summary>
    public IReadOnlyList<TextSegment> Segments => _segments;

    /// <summary>
    /// Gets the raw HTML content of the document.
    /// </summary>
    public string PlainText => _rawText;

    /// <summary>
    /// Gets the rendered plain-text projection of the document.
    /// </summary>
    public string RenderedText => _projection.Text;

    /// <summary>
    /// Gets the total rendered text length, excluding markup tags.
    /// </summary>
    public int Length => _projection.Length;

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

        var index = _projection.GetSourceInsertionOffset(offset);
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
        if (length <= 0 || offset < 0 || offset >= _projection.Length)
        {
            return;
        }

        var (sourceStart, sourceLength) = _projection.GetSourceRange(offset, length);
        if (sourceLength <= 0)
        {
            return;
        }

        _rawText = RemoveEmptyFormattingElements(_rawText.Remove(sourceStart, sourceLength));
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
            var sourceOffset = _projection.GetSourceInsertionOffset(Math.Clamp(offset, 0, _projection.Length));
            var boundedOffset = Math.Clamp(sourceOffset, 0, _rawText.Length);
            _rawText = _rawText.Insert(boundedOffset, text);
            RebuildSegments();
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
        var plain = _projection.Text;
        _rawText = HtmlClipboardUtilities.EncodePlainText(plain);
        RebuildSegments();
    }

    /// <summary>
    /// Gets rendered text for a document range.
    /// </summary>
    /// <param name="offset">The rendered start offset.</param>
    /// <param name="length">The rendered range length.</param>
    /// <returns>The rendered text in the requested range.</returns>
    public string GetTextRange(int offset, int length) => _projection.GetRangeText(offset, length);

    /// <summary>
    /// Gets the source HTML fragment for a rendered document range.
    /// </summary>
    /// <param name="offset">The rendered start offset.</param>
    /// <param name="length">The rendered range length.</param>
    /// <returns>The source HTML fragment for the requested range.</returns>
    public string GetHtmlRange(int offset, int length)
    {
        if (length <= 0 || string.IsNullOrEmpty(_rawText))
        {
            return string.Empty;
        }

        var (sourceStart, sourceLength) = _projection.GetSourceRange(offset, length);
        if (sourceLength <= 0 || sourceStart < 0 || sourceStart >= _rawText.Length)
        {
            return string.Empty;
        }

        var safeLength = Math.Min(sourceLength, _rawText.Length - sourceStart);
        return safeLength <= 0 ? string.Empty : _rawText.Substring(sourceStart, safeLength);
    }

    private static string RemoveEmptyFormattingElements(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var cleaned = text;
        string previous;
        do
        {
            previous = cleaned;
            cleaned = cleaned
                .Replace("<strong></strong>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<b></b>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<em></em>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<i></i>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<u></u>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<s></s>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<strike></strike>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("<del></del>", string.Empty, StringComparison.OrdinalIgnoreCase);
        }
        while (!string.Equals(previous, cleaned, StringComparison.Ordinal));

        return cleaned;
    }

    private void RebuildSegments()
    {
        _segments.Clear();
        _projection = HtmlTextProjection.Create(_rawText);
        var parsed = HtmlContentParser.Parse(_rawText);
        if (parsed.Count == 0)
        {
            return;
        }

        _segments.AddRange(parsed);
    }
}
