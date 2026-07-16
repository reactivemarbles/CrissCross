// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia.Layout;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a segment of text with optional formatting.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TextSegment"/> class.
/// </remarks>
/// <param name="text">The text content.</param>
/// <param name="startIndex">The start index in the document.</param>
public class TextSegment(string text, int startIndex)
{
    /// <summary>Tolerance used when comparing optional font sizes.</summary>
    private const double FontSizeComparisonTolerance = 1E-10D;

    /// <summary>Gets or sets the text content.</summary>
    public string Text { get; set; } = text ?? string.Empty;

    /// <summary>Gets or sets the start index in the document.</summary>
    public int StartIndex { get; set; } = startIndex;

    /// <summary>Gets the end index in the document.</summary>
    public int EndIndex => StartIndex + Length;

    /// <summary>Gets the number of rendered document positions consumed by this segment.</summary>
    public int Length => IsImage ? 1 : Text.Length;

    /// <summary>Gets or sets a value indicating whether this segment is bold.</summary>
    public bool IsBold { get; set; }

    /// <summary>Gets or sets a value indicating whether this segment is italic.</summary>
    public bool IsItalic { get; set; }

    /// <summary>Gets or sets a value indicating whether this segment is underlined.</summary>
    public bool IsUnderline { get; set; }

    /// <summary>Gets or sets a value indicating whether this segment has strikethrough.</summary>
    public bool IsStrikethrough { get; set; }

    /// <summary>Gets or sets the foreground color for this segment.</summary>
    public IBrush? Foreground { get; set; }

    /// <summary>Gets or sets the background color for this segment.</summary>
    public IBrush? Background { get; set; }

    /// <summary>Gets or sets the font size for this segment.</summary>
    public double? FontSize { get; set; }

    /// <summary>Gets or sets the font family for this segment.</summary>
    public FontFamily? FontFamily { get; set; }

    /// <summary>Gets or sets a value indicating whether this segment represents a line break.</summary>
    public bool IsLineBreak { get; set; }

    /// <summary>Gets or sets a value indicating whether this segment represents a paragraph break.</summary>
    public bool IsParagraphBreak { get; set; }

    /// <summary>Gets or sets a value indicating whether this segment represents an image.</summary>
    public bool IsImage { get; set; }

    /// <summary>Gets or sets the source for the image when <see cref="IsImage"/> is true.</summary>
    public string? ImageSource { get; set; }

    /// <summary>Gets or sets the image width in device independent pixels.</summary>
    public double? ImageWidth { get; set; }

    /// <summary>Gets or sets the image height in device independent pixels.</summary>
    public double? ImageHeight { get; set; }

    /// <summary>Gets or sets the alignment used when rendering the image.</summary>
    public HorizontalAlignment ImageAlignment { get; set; } = HorizontalAlignment.Left;

    /// <summary>Gets or sets the text alignment applied to a paragraph break.</summary>
    public TextAlignment? ParagraphAlignment { get; set; }

    /// <summary>Gets a value indicating whether the segment contains visible text.</summary>
    public bool HasRenderableText => !string.IsNullOrEmpty(Text);

    /// <summary>Gets the font weight based on formatting.</summary>
    public FontWeight FontWeight => IsBold ? FontWeight.Bold : FontWeight.Normal;

    /// <summary>Gets the font style based on formatting.</summary>
    public FontStyle FontStyle => IsItalic ? FontStyle.Italic : FontStyle.Normal;

    /// <summary>Gets the text decorations based on formatting.</summary>
    public TextDecorationCollection? TextDecorations => CreateTextDecorations(IsUnderline, IsStrikethrough);

    /// <summary>Creates a line break segment used for block separation.</summary>
    /// <param name="offset">The document offset associated with the break.</param>
    /// <returns>A <see cref="TextSegment"/> representing a line break.</returns>
    public static TextSegment CreateLineBreak(int offset) => new(Environment.NewLine, offset) { IsLineBreak = true };

    /// <summary>Creates a paragraph break marker.</summary>
    /// <param name="offset">The document offset associated with the break.</param>
    /// <returns>A <see cref="TextSegment"/> describing the break.</returns>
    public static TextSegment CreateParagraphBreak(int offset) => CreateParagraphBreak(offset, null);

    /// <summary>Creates a paragraph break marker.</summary>
    /// <param name="offset">The document offset associated with the break.</param>
    /// <param name="alignment">Alignment metadata for the paragraph.</param>
    /// <returns>A <see cref="TextSegment"/> describing the break.</returns>
    public static TextSegment CreateParagraphBreak(int offset, TextAlignment? alignment) =>
        new(string.Empty, offset) { IsParagraphBreak = true, ParagraphAlignment = alignment };

    /// <summary>Creates an inline image segment.</summary>
    /// <param name="offset">The document offset where the image is injected.</param>
    /// <param name="source">The URI or path to the image content.</param>
    /// <param name="alignment">The horizontal alignment applied to the rendered image.</param>
    /// <param name="width">Optional explicit width in device independent pixels.</param>
    /// <param name="height">Optional explicit height in device independent pixels.</param>
    /// <returns>A <see cref="TextSegment"/> describing the image.</returns>
    public static TextSegment CreateImage(
        int offset,
        string source,
        HorizontalAlignment alignment,
        double? width,
        double? height) =>
        new(string.Empty, offset)
        {
            IsImage = true,
            ImageSource = source,
            ImageAlignment = alignment,
            ImageWidth = width,
            ImageHeight = height,
        };

    /// <summary>Creates a clone of this segment.</summary>
    /// <returns>A new TextSegment with the same properties.</returns>
    public TextSegment Clone()
    {
        var source = this;
        return new TextSegment(source.Text, source.StartIndex)
        {
            IsBold = source.IsBold,
            IsItalic = source.IsItalic,
            IsUnderline = source.IsUnderline,
            IsStrikethrough = source.IsStrikethrough,
            Foreground = source.Foreground,
            Background = source.Background,
            FontSize = source.FontSize,
            FontFamily = source.FontFamily,
            IsLineBreak = source.IsLineBreak,
            IsParagraphBreak = source.IsParagraphBreak,
            IsImage = source.IsImage,
            ImageSource = source.ImageSource,
            ImageWidth = source.ImageWidth,
            ImageHeight = source.ImageHeight,
            ImageAlignment = source.ImageAlignment,
            ParagraphAlignment = source.ParagraphAlignment,
        };
    }

    /// <summary>Checks if this segment has the same formatting as another segment.</summary>
    /// <param name="other">The other segment to compare.</param>
    /// <returns>True if formatting matches.</returns>
    public bool HasSameFormatting(TextSegment other)
    {
        return other is not null
            && HasSameTextFormatting(other)
            && HasSameBlockFormatting(other)
            && HasSameImageFormatting(other);
    }

    /// <summary>Creates text decorations for underline and strikethrough state.</summary>
    /// <param name="isUnderline">Whether underline decoration is enabled.</param>
    /// <param name="isStrikethrough">Whether strikethrough decoration is enabled.</param>
    /// <returns>The decoration collection, or <see langword="null"/> when no decoration applies.</returns>
    private static TextDecorationCollection? CreateTextDecorations(bool isUnderline, bool isStrikethrough)
    {
        if (!isUnderline && !isStrikethrough)
        {
            return null;
        }

        var decorations = new TextDecorationCollection();
        AddDecoration(decorations, isUnderline, TextDecorationLocation.Underline);
        AddDecoration(decorations, isStrikethrough, TextDecorationLocation.Strikethrough);
        return decorations;
    }

    /// <summary>Adds one decoration when requested.</summary>
    /// <param name="decorations">The target collection.</param>
    /// <param name="enabled">Whether the decoration should be added.</param>
    /// <param name="location">The decoration location.</param>
    private static void AddDecoration(
        TextDecorationCollection decorations,
        bool enabled,
        TextDecorationLocation location)
    {
        if (!enabled)
        {
            return;
        }

        decorations.Add(new TextDecoration { Location = location });
    }

    /// <summary>Compares optional font sizes using a floating-point tolerance.</summary>
    /// <param name="left">The first font size.</param>
    /// <param name="right">The second font size.</param>
    /// <returns><see langword="true"/> when both values are absent or effectively equal.</returns>
    private static bool HaveEqualFontSize(double? left, double? right) =>
        left.HasValue == right.HasValue
        && (!left.HasValue || Math.Abs(left.Value - right!.Value) <= FontSizeComparisonTolerance);

    /// <summary>Compares character formatting options.</summary>
    /// <param name="other">The other segment.</param>
    /// <returns><see langword="true"/> when character formatting matches.</returns>
    private bool HasSameTextFormatting(TextSegment other) =>
        IsBold == other.IsBold
        && IsItalic == other.IsItalic
        && IsUnderline == other.IsUnderline
        && IsStrikethrough == other.IsStrikethrough
        && Equals(Foreground, other.Foreground)
        && Equals(Background, other.Background)
        && HaveEqualFontSize(FontSize, other.FontSize)
        && Equals(FontFamily, other.FontFamily);

    /// <summary>Compares line and paragraph formatting options.</summary>
    /// <param name="other">The other segment.</param>
    /// <returns><see langword="true"/> when block formatting matches.</returns>
    private bool HasSameBlockFormatting(TextSegment other) =>
        IsLineBreak == other.IsLineBreak && IsParagraphBreak == other.IsParagraphBreak;

    /// <summary>Compares image formatting options.</summary>
    /// <param name="other">The other segment.</param>
    /// <returns><see langword="true"/> when image formatting matches.</returns>
    private bool HasSameImageFormatting(TextSegment other) =>
        IsImage == other.IsImage && ImageAlignment == other.ImageAlignment;
}
