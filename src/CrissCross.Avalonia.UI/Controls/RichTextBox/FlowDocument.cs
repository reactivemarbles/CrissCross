// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Lightweight representation of a flow document that mirrors the segments produced by <see cref="RichTextDocument"/>.
/// </summary>
public sealed class FlowDocument
{
    private static readonly char[] NewLineSeparators = ['\r', '\n'];

    private readonly ObservableCollection<Block> _blocks = new();
    private readonly ReadOnlyObservableCollection<Block> _readOnlyBlocks;
    private readonly RichTextDocument _coreDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowDocument"/> class with a fresh <see cref="RichTextDocument"/>.
    /// </summary>
    public FlowDocument()
        : this(new RichTextDocument())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowDocument"/> class wrapping an existing <see cref="RichTextDocument"/>.
    /// </summary>
    /// <param name="document">The document to wrap.</param>
    public FlowDocument(RichTextDocument document)
    {
        _coreDocument = document ?? throw new ArgumentNullException(nameof(document));
        _readOnlyBlocks = new ReadOnlyObservableCollection<Block>(_blocks);
        Blocks = _readOnlyBlocks;
        SyncBlocks();
    }

    /// <summary>
    /// Raised whenever the underlying content changes.
    /// </summary>
    public event EventHandler? TextChanged;

    /// <summary>
    /// Gets the block collection that represents the current layout.
    /// </summary>
    public IReadOnlyList<Block> Blocks { get; }

    /// <summary>
    /// Gets the parsed segments for the current document.
    /// </summary>
    public IReadOnlyList<TextSegment> Segments => _coreDocument.Segments;

    /// <summary>
    /// Gets the number of characters in the raw document.
    /// </summary>
    public int Length => _coreDocument.Length;

    /// <summary>
    /// Gets a pointer to the start of the document.
    /// </summary>
    public TextPointer ContentStart => new(this, 0);

    /// <summary>
    /// Gets a pointer to the end of the document.
    /// </summary>
    public TextPointer ContentEnd => new(this, Length);

    /// <summary>
    /// Replaces the document content.
    /// </summary>
    /// <param name="text">New markup or plain text.</param>
    public void SetText(string? text) => UpdateDocument(() => _coreDocument.SetText(text));

    /// <summary>
    /// Appends text to the end of the document.
    /// </summary>
    /// <param name="text">The content to append.</param>
    public void AppendText(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        UpdateDocument(() => _coreDocument.AppendText(text));
    }

    /// <summary>
    /// Inserts text at the specified offset.
    /// </summary>
    /// <param name="offset">The insertion offset.</param>
    /// <param name="text">The text to insert.</param>
    public void Insert(int offset, string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        ValidateOffset(offset, allowEnd: true);
        UpdateDocument(() => _coreDocument.Insert(offset, text));
    }

    /// <summary>
    /// Deletes a text range.
    /// </summary>
    /// <param name="offset">The start offset.</param>
    /// <param name="length">The number of characters to delete.</param>
    public void Delete(int offset, int length)
    {
        ValidateOffset(offset, allowEnd: false);
        ThrowIfNegative(length, nameof(length));
        UpdateDocument(() => _coreDocument.Delete(offset, length));
    }

    /// <summary>
    /// Replaces a text range with new text.
    /// </summary>
    /// <param name="offset">The start offset.</param>
    /// <param name="length">The number of characters to replace.</param>
    /// <param name="text">The replacement text.</param>
    public void Replace(int offset, int length, string? text)
    {
        ValidateOffset(offset, allowEnd: true);
        ThrowIfNegative(length, nameof(length));
        UpdateDocument(() => _coreDocument.Replace(offset, length, text));
    }

    /// <summary>
    /// Toggles formatting for a specific range.
    /// </summary>
    /// <param name="start">The start offset.</param>
    /// <param name="length">The length of the range.</param>
    /// <param name="formatType">The formatting to toggle.</param>
    /// <returns><see langword="true"/> when formatting ends applied across the entire range.</returns>
    public bool ToggleFormatting(int start, int length, TextFormatType formatType)
    {
        ValidateOffset(start, allowEnd: true);
        ThrowIfNegative(length, nameof(length));
        var applied = _coreDocument.ToggleFormatting(start, length, formatType);
        SyncBlocks();
        OnTextChanged();
        return applied;
    }

    /// <summary>
    /// Removes all formatting, keeping only the textual content.
    /// </summary>
    public void ClearFormatting() => UpdateDocument(_coreDocument.ClearFormatting);

    /// <summary>
    /// Gets the underlying markup/plain text.
    /// </summary>
    /// <returns>The raw text representation.</returns>
    public string GetText() => _coreDocument.PlainText;

    /// <summary>
    /// Creates a pointer at the specified offset.
    /// </summary>
    /// <param name="offset">The offset relative to <see cref="ContentStart"/>.</param>
    /// <returns>A new <see cref="TextPointer"/> bound to this document.</returns>
    public TextPointer GetTextPointer(int offset)
    {
        ValidateOffset(offset, allowEnd: true);
        return new TextPointer(this, offset);
    }

    /// <summary>
    /// Gets the text within a pointer range.
    /// </summary>
    /// <param name="start">The start pointer.</param>
    /// <param name="end">The end pointer.</param>
    /// <returns>The requested substring, or <see cref="string.Empty"/> when the range is empty.</returns>
    public string GetTextRange(TextPointer start, TextPointer end)
    {
        var text = GetText();
        if (text.Length == 0)
        {
            return string.Empty;
        }

        var (rangeStart, rangeLength) = GetRange(start, end);
        if (rangeStart >= text.Length || rangeLength <= 0)
        {
            return string.Empty;
        }

        var safeLength = Math.Min(rangeLength, text.Length - rangeStart);
        return safeLength <= 0 ? string.Empty : text.Substring(rangeStart, safeLength);
    }

    /// <summary>
    /// Gets numeric range information represented by two pointers.
    /// </summary>
    /// <param name="start">The start pointer.</param>
    /// <param name="end">The end pointer.</param>
    /// <returns>The zero-based start index and length.</returns>
    public (int start, int length) GetRange(TextPointer start, TextPointer end)
    {
        ValidatePointer(start);
        ValidatePointer(end);
        var rangeStart = Math.Min(start.Offset, end.Offset);
        var rangeEnd = Math.Max(start.Offset, end.Offset);
        return (rangeStart, rangeEnd - rangeStart);
    }

    /// <summary>
    /// Rebuilds the block representation from the underlying document.
    /// </summary>
    public void Refresh()
    {
        SyncBlocks();
        OnTextChanged();
    }

    private static void ThrowIfNegative(int value, string name)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }

    private static RunInline CreateRun(TextSegment segment, string text)
    {
        var run = new RunInline
        {
            Text = text,
            FontWeight = segment.FontWeight,
            FontStyle = segment.FontStyle,
            FontFamily = segment.FontFamily,
            FontSize = segment.FontSize,
            Foreground = segment.Foreground,
            Background = segment.Background,
        };

        if (segment.TextDecorations is not null)
        {
            run.TextDecorations = segment.TextDecorations;
        }

        return run;
    }

    private static ImageInline? CreateImageInline(TextSegment segment)
    {
        if (string.IsNullOrWhiteSpace(segment.ImageSource))
        {
            return null;
        }

        return new ImageInline(segment.ImageSource)
        {
            Alignment = segment.ImageAlignment,
            Width = segment.ImageWidth,
            Height = segment.ImageHeight,
        };
    }

    private static void AppendRuns(Paragraph paragraph, TextSegment segment)
    {
        if (string.IsNullOrEmpty(segment.Text))
        {
            return;
        }

        var text = segment.Text.AsSpan();
        var index = 0;
        while (index < text.Length)
        {
            var newlineIndex = text[index..].IndexOfAny(NewLineSeparators);
            if (newlineIndex < 0)
            {
                var remaining = text[index..].ToString();
                if (remaining.Length > 0)
                {
                    paragraph.Inlines.Add(CreateRun(segment, remaining));
                }

                break;
            }

            if (newlineIndex > 0)
            {
                var runText = text.Slice(index, newlineIndex).ToString();
                if (runText.Length > 0)
                {
                    paragraph.Inlines.Add(CreateRun(segment, runText));
                }
            }

            paragraph.Inlines.Add(new LineBreakInline());
            index += newlineIndex + 1;

            if (index < text.Length && text[index - 1] == '\r' && text[index] == '\n')
            {
                index++;
            }
        }
    }

    private void ValidateOffset(int offset, bool allowEnd)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (allowEnd)
        {
            if (offset > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            return;
        }

        if (Length == 0 || offset >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
    }

    private Paragraph EnsureParagraph(TextAlignment? alignment = null)
    {
        Paragraph paragraph;
        if (_blocks.Count == 0 || _blocks[^1] is not Paragraph existing)
        {
            paragraph = new Paragraph();
            if (alignment.HasValue)
            {
                paragraph.TextAlignment = alignment.Value;
            }

            _blocks.Add(paragraph);
        }
        else
        {
            paragraph = existing;
            if (alignment.HasValue)
            {
                paragraph.TextAlignment = alignment.Value;
            }
        }

        return paragraph;
    }

    private void UpdateDocument(Action mutation)
    {
        ArgumentNullException.ThrowIfNull(mutation);
        mutation();
        SyncBlocks();
        OnTextChanged();
    }

    private void SyncBlocks()
    {
        _blocks.Clear();
        if (Segments.Count == 0)
        {
            return;
        }

        Paragraph? paragraph = null;
        TextAlignment? pendingAlignment = null;

        foreach (var segment in Segments)
        {
            if (segment.IsParagraphBreak)
            {
                if (paragraph is not null && segment.ParagraphAlignment.HasValue)
                {
                    paragraph.TextAlignment = segment.ParagraphAlignment.Value;
                }

                paragraph = null;
                pendingAlignment = segment.ParagraphAlignment;
                continue;
            }

            paragraph ??= EnsureParagraph(pendingAlignment);
            pendingAlignment = null;

            if (segment.IsLineBreak)
            {
                paragraph.Inlines.Add(new LineBreakInline());
                continue;
            }

            if (segment.IsImage)
            {
                var imageInline = CreateImageInline(segment);
                if (imageInline is not null)
                {
                    paragraph.Inlines.Add(imageInline);
                }

                continue;
            }

            if (!segment.HasRenderableText)
            {
                continue;
            }

            AppendRuns(paragraph, segment);
        }
    }

    private void ValidatePointer(TextPointer pointer)
    {
        ArgumentNullException.ThrowIfNull(pointer);
        if (!ReferenceEquals(pointer.Document, this))
        {
            throw new InvalidOperationException("Pointer belongs to a different document.");
        }
    }

    private void OnTextChanged() => TextChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Base block element.
    /// </summary>
    public abstract class Block
    {
        /// <summary>
        /// Gets or sets the block margin.
        /// </summary>
        public Thickness Margin { get; set; } = new(0);

        /// <summary>
        /// Gets or sets the text alignment for the block.
        /// </summary>
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;
    }

    /// <summary>
    /// Paragraph block containing inline content.
    /// </summary>
    public sealed class Paragraph : Block
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        public Paragraph() => Inlines = [];

        /// <summary>
        /// Gets the inline collection associated with this paragraph.
        /// </summary>
        public InlineCollection Inlines { get; }
    }

    /// <summary>
    /// Base inline element.
    /// </summary>
    public abstract class Inline
    {
        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        public FontWeight FontWeight { get; set; } = FontWeight.Normal;

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;

        /// <summary>
        /// Gets or sets the text decorations.
        /// </summary>
        public TextDecorationCollection? TextDecorations { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush.
        /// </summary>
        public IBrush? Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        public IBrush? Background { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        public FontFamily? FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public double? FontSize { get; set; }
    }

    /// <summary>
    /// Represents a run of text.
    /// </summary>
    public sealed class RunInline : Inline
    {
        /// <summary>
        /// Gets or sets the run text.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a line break.
    /// </summary>
    public sealed class LineBreakInline : Inline;

    /// <summary>
    /// Represents an inline image.
    /// </summary>
    public sealed class ImageInline : Inline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInline"/> class.
        /// </summary>
        /// <param name="source">The image source.</param>
        public ImageInline(string source)
        {
            Source = source;
        }

        /// <summary>
        /// Gets the image source string.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets or sets the desired width.
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Gets or sets the desired height.
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Gets or sets the image alignment.
        /// </summary>
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Center;
    }

    /// <summary>
    /// Represents a mutable collection of <see cref="Inline"/> instances.
    /// </summary>
    public sealed class InlineCollection : Collection<Inline>;
}
