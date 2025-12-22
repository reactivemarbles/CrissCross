// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a selection within a <see cref="FlowDocument"/>.
/// </summary>
public sealed class TextSelection
{
    private FlowDocument _document;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextSelection"/> class.
    /// </summary>
    /// <param name="document">The owning document.</param>
    internal TextSelection(FlowDocument document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
        Start = document.ContentStart;
        End = document.ContentStart;
    }

    /// <summary>
    /// Gets the owning document.
    /// </summary>
    public FlowDocument Document => _document;

    /// <summary>
    /// Gets the start pointer.
    /// </summary>
    public TextPointer Start { get; private set; }

    /// <summary>
    /// Gets the end pointer.
    /// </summary>
    public TextPointer End { get; private set; }

    /// <summary>
    /// Gets the selection length.
    /// </summary>
    public int Length => Math.Abs(End.Offset - Start.Offset);

    /// <summary>
    /// Gets a value indicating whether the selection is empty.
    /// </summary>
    public bool IsEmpty => Length == 0;

    /// <summary>
    /// Gets the text contained within the selection.
    /// </summary>
    public string Text => _document.GetTextRange(Start, End);

    /// <summary>
    /// Replaces the selection with the specified range.
    /// </summary>
    /// <param name="start">The start pointer.</param>
    /// <param name="end">The end pointer.</param>
    public void Select(TextPointer start, TextPointer end)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentNullException.ThrowIfNull(end);
        ValidatePointer(start);
        ValidatePointer(end);
        Start = start;
        End = end;
    }

    /// <summary>
    /// Selects the entire document.
    /// </summary>
    public void SelectAll() => Select(_document.ContentStart, _document.ContentEnd);

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    public void Clear() => Select(_document.ContentStart, _document.ContentStart);

    /// <summary>
    /// Applies formatting to the current selection.
    /// </summary>
    /// <param name="formatType">The formatting to toggle.</param>
    public void ApplyFormatting(TextFormatType formatType)
    {
        if (IsEmpty)
        {
            return;
        }

        var (start, length) = _document.GetRange(Start, End);
        _document.ToggleFormatting(start, length, formatType);
    }

    /// <summary>
    /// Gets the range represented by the selection.
    /// </summary>
    /// <returns>A tuple describing the range.</returns>
    public (int start, int length) GetRange() => _document.GetRange(Start, End);

    /// <summary>
    /// Updates the selection using absolute offsets.
    /// </summary>
    /// <param name="startOffset">Start offset.</param>
    /// <param name="endOffset">End offset.</param>
    internal void Select(int startOffset, int endOffset)
    {
        Start = _document.GetTextPointer(startOffset);
        End = _document.GetTextPointer(endOffset);
    }

    /// <summary>
    /// Rebinds the selection to a new document instance.
    /// </summary>
    /// <param name="document">The document to attach.</param>
    internal void Attach(FlowDocument document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
        Start = document.ContentStart;
        End = document.ContentStart;
    }

    private void ValidatePointer(TextPointer pointer)
    {
        ArgumentNullException.ThrowIfNull(pointer);

        if (pointer.Document != _document)
        {
            throw new InvalidOperationException("Pointer does not belong to this document.");
        }
    }
}
