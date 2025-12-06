// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a document model for rich text content with formatting information.
/// </summary>
public class RichTextDocument
{
    private readonly List<TextSegment> _segments = [];
    private string _plainText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextDocument"/> class.
    /// </summary>
    public RichTextDocument()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextDocument"/> class with initial text.
    /// </summary>
    /// <param name="text">The initial plain text.</param>
    public RichTextDocument(string text) => SetText(text);

    /// <summary>
    /// Gets the segments in the document.
    /// </summary>
    public IReadOnlyList<TextSegment> Segments => _segments;

    /// <summary>
    /// Gets the plain text content of the document.
    /// </summary>
    public string PlainText => _plainText;

    /// <summary>
    /// Gets the total length of the document.
    /// </summary>
    public int Length => _plainText.Length;

    /// <summary>
    /// Sets the document text, clearing all formatting.
    /// </summary>
    /// <param name="text">The plain text to set.</param>
    public void SetText(string? text)
    {
        _plainText = text ?? string.Empty;
        _segments.Clear();

        if (!string.IsNullOrEmpty(_plainText))
        {
            _segments.Add(new TextSegment(_plainText, 0));
        }
    }

    /// <summary>
    /// Inserts text at the specified position.
    /// </summary>
    /// <param name="index">The position to insert at.</param>
    /// <param name="text">The text to insert.</param>
    public void Insert(int index, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        index = Math.Clamp(index, 0, _plainText.Length);

        // Update plain text
        _plainText = _plainText.Insert(index, text);

        // Find and split the segment at the insertion point
        var segmentIndex = FindSegmentIndex(index);

        if (segmentIndex < 0 || _segments.Count == 0)
        {
            // No segments or insertion at the very end
            if (_segments.Count == 0)
            {
                _segments.Add(new TextSegment(text, 0));
            }
            else
            {
                var lastSegment = _segments[^1];
                lastSegment.Text += text;
            }
        }
        else
        {
            var segment = _segments[segmentIndex];
            var localIndex = index - segment.StartIndex;

            // Insert into the existing segment, inheriting its formatting
            segment.Text = segment.Text.Insert(localIndex, text);

            // Update start indices for subsequent segments
            for (var i = segmentIndex + 1; i < _segments.Count; i++)
            {
                _segments[i].StartIndex += text.Length;
            }
        }
    }

    /// <summary>
    /// Deletes text from the specified range.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length to delete.</param>
    public void Delete(int startIndex, int length)
    {
        if (length <= 0 || startIndex < 0 || startIndex >= _plainText.Length)
        {
            return;
        }

        length = Math.Min(length, _plainText.Length - startIndex);

        // Update plain text
        _plainText = _plainText.Remove(startIndex, length);

        // Update segments
        var endIndex = startIndex + length;
        var segmentsToRemove = new List<int>();

        for (var i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];

            if (segment.EndIndex <= startIndex)
            {
                // Segment is before the deletion range, no change
                continue;
            }

            if (segment.StartIndex >= endIndex)
            {
                // Segment is after the deletion range, just update index
                segment.StartIndex -= length;
            }
            else if (segment.StartIndex >= startIndex && segment.EndIndex <= endIndex)
            {
                // Segment is completely within the deletion range
                segmentsToRemove.Add(i);
            }
            else if (segment.StartIndex < startIndex && segment.EndIndex > endIndex)
            {
                // Deletion is completely within this segment
                var localStart = startIndex - segment.StartIndex;
                segment.Text = segment.Text.Remove(localStart, length);
            }
            else if (segment.StartIndex < startIndex)
            {
                // Segment overlaps the start of the deletion
                var localStart = startIndex - segment.StartIndex;
                segment.Text = segment.Text[..localStart];
            }
            else
            {
                // Segment overlaps the end of the deletion
                var localEnd = endIndex - segment.StartIndex;
                segment.Text = segment.Text[localEnd..];
                segment.StartIndex = startIndex;
            }
        }

        // Remove empty segments in reverse order
        for (var i = segmentsToRemove.Count - 1; i >= 0; i--)
        {
            _segments.RemoveAt(segmentsToRemove[i]);
        }

        // Remove segments with empty text
        _segments.RemoveAll(s => string.IsNullOrEmpty(s.Text));

        // Recalculate start indices
        RecalculateIndices();
    }

    /// <summary>
    /// Applies formatting to a range of text.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length of the range.</param>
    /// <param name="formatType">The type of formatting to apply.</param>
    /// <param name="value">True to apply, false to remove the formatting.</param>
    public void ApplyFormatting(int startIndex, int length, TextFormatType formatType, bool value)
    {
        if (length <= 0 || startIndex < 0 || startIndex >= _plainText.Length)
        {
            return;
        }

        length = Math.Min(length, _plainText.Length - startIndex);
        var endIndex = startIndex + length;

        // Split segments at the boundaries
        SplitSegmentAt(startIndex);
        SplitSegmentAt(endIndex);

        // Apply formatting to affected segments
        foreach (var segment in _segments)
        {
            if (segment.StartIndex >= startIndex && segment.EndIndex <= endIndex)
            {
                switch (formatType)
                {
                    case TextFormatType.Bold:
                        segment.IsBold = value;
                        break;
                    case TextFormatType.Italic:
                        segment.IsItalic = value;
                        break;
                    case TextFormatType.Underline:
                        segment.IsUnderline = value;
                        break;
                    case TextFormatType.Strikethrough:
                        segment.IsStrikethrough = value;
                        break;
                }
            }
        }

        // Merge adjacent segments with the same formatting
        MergeAdjacentSegments();
    }

    /// <summary>
    /// Toggles formatting for a range of text.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length of the range.</param>
    /// <param name="formatType">The type of formatting to toggle.</param>
    /// <returns>True if formatting was applied, false if removed.</returns>
    public bool ToggleFormatting(int startIndex, int length, TextFormatType formatType)
    {
        if (length <= 0 || startIndex < 0 || startIndex >= _plainText.Length)
        {
            return false;
        }

        // Check if the entire range already has the formatting
        var hasFormatting = HasFormatting(startIndex, length, formatType);

        // Toggle - apply if not present, remove if present
        ApplyFormatting(startIndex, length, formatType, !hasFormatting);

        return !hasFormatting;
    }

    /// <summary>
    /// Checks if a range has a specific formatting.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length of the range.</param>
    /// <param name="formatType">The type of formatting to check.</param>
    /// <returns>True if the entire range has the formatting.</returns>
    public bool HasFormatting(int startIndex, int length, TextFormatType formatType)
    {
        if (length <= 0 || startIndex < 0 || startIndex >= _plainText.Length)
        {
            return false;
        }

        length = Math.Min(length, _plainText.Length - startIndex);
        var endIndex = startIndex + length;

        foreach (var segment in _segments)
        {
            // Check if segment overlaps with the range
            if (segment.StartIndex < endIndex && segment.EndIndex > startIndex)
            {
                var hasFormat = formatType switch
                {
                    TextFormatType.Bold => segment.IsBold,
                    TextFormatType.Italic => segment.IsItalic,
                    TextFormatType.Underline => segment.IsUnderline,
                    TextFormatType.Strikethrough => segment.IsStrikethrough,
                    _ => false
                };

                if (!hasFormat)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the formatting at a specific index.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <returns>The text segment at that position, or null if not found.</returns>
    public TextSegment? GetFormattingAt(int index)
    {
        var segmentIndex = FindSegmentIndex(index);
        return segmentIndex >= 0 ? _segments[segmentIndex] : null;
    }

    /// <summary>
    /// Clears all formatting, keeping the plain text.
    /// </summary>
    public void ClearFormatting()
    {
        var text = _plainText;
        SetText(text);
    }

    private int FindSegmentIndex(int charIndex)
    {
        for (var i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
            if (charIndex >= segment.StartIndex && charIndex < segment.EndIndex)
            {
                return i;
            }
        }

        // If at the very end, return last segment
        if (charIndex == _plainText.Length && _segments.Count > 0)
        {
            return _segments.Count - 1;
        }

        return -1;
    }

    private void SplitSegmentAt(int index)
    {
        if (index <= 0 || index >= _plainText.Length)
        {
            return;
        }

        var segmentIndex = FindSegmentIndex(index);
        if (segmentIndex < 0)
        {
            return;
        }

        var segment = _segments[segmentIndex];

        // Only split if the index is in the middle of the segment
        if (index <= segment.StartIndex || index >= segment.EndIndex)
        {
            return;
        }

        var localIndex = index - segment.StartIndex;
        var firstPart = segment.Text[..localIndex];
        var secondPart = segment.Text[localIndex..];

        // Update the existing segment
        segment.Text = firstPart;

        // Create a new segment for the second part
        var newSegment = segment.Clone();
        newSegment.Text = secondPart;
        newSegment.StartIndex = index;

        _segments.Insert(segmentIndex + 1, newSegment);
    }

    private void MergeAdjacentSegments()
    {
        for (var i = _segments.Count - 1; i > 0; i--)
        {
            var current = _segments[i];
            var previous = _segments[i - 1];

            if (previous.HasSameFormatting(current))
            {
                previous.Text += current.Text;
                _segments.RemoveAt(i);
            }
        }
    }

    private void RecalculateIndices()
    {
        var index = 0;
        foreach (var segment in _segments)
        {
            segment.StartIndex = index;
            index += segment.Text.Length;
        }
    }
}
