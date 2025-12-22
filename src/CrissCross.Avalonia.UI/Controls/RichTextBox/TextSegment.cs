// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia.Layout;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a segment of text with optional formatting.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TextSegment"/> class.
/// </remarks>
/// <param name="text">The text content.</param>
/// <param name="startIndex">The start index in the document.</param>
public class TextSegment(string text, int startIndex)
{
    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text { get; set; } = text ?? string.Empty;

    /// <summary>
    /// Gets or sets the start index in the document.
    /// </summary>
    public int StartIndex { get; set; } = startIndex;

    /// <summary>
    /// Gets the end index in the document.
    /// </summary>
    public int EndIndex => StartIndex + Text.Length;

    /// <summary>
    /// Gets or sets a value indicating whether this segment is bold.
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment is italic.
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment is underlined.
    /// </summary>
    public bool IsUnderline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment has strikethrough.
    /// </summary>
    public bool IsStrikethrough { get; set; }

    /// <summary>
    /// Gets or sets the foreground color for this segment.
    /// </summary>
    public IBrush? Foreground { get; set; }

    /// <summary>
    /// Gets or sets the background color for this segment.
    /// </summary>
    public IBrush? Background { get; set; }

    /// <summary>
    /// Gets or sets the font size for this segment.
    /// </summary>
    public double? FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font family for this segment.
    /// </summary>
    public FontFamily? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment represents a line break.
    /// </summary>
    public bool IsLineBreak { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment represents a paragraph break.
    /// </summary>
    public bool IsParagraphBreak { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment represents an image.
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    /// Gets or sets the source for the image when <see cref="IsImage"/> is true.
    /// </summary>
    public string? ImageSource { get; set; }

    /// <summary>
    /// Gets or sets the image width in device independent pixels.
    /// </summary>
    public double? ImageWidth { get; set; }

    /// <summary>
    /// Gets or sets the image height in device independent pixels.
    /// </summary>
    public double? ImageHeight { get; set; }

    /// <summary>
    /// Gets or sets the alignment used when rendering the image.
    /// </summary>
    public HorizontalAlignment ImageAlignment { get; set; } = HorizontalAlignment.Left;

    /// <summary>
    /// Gets or sets the text alignment applied to a paragraph break.
    /// </summary>
    public TextAlignment? ParagraphAlignment { get; set; }

    /// <summary>
    /// Gets a value indicating whether the segment contains visible text.
    /// </summary>
    public bool HasRenderableText => !string.IsNullOrEmpty(Text);

    /// <summary>
    /// Gets the font weight based on formatting.
    /// </summary>
    public FontWeight FontWeight => IsBold ? FontWeight.Bold : FontWeight.Normal;

    /// <summary>
    /// Gets the font style based on formatting.
    /// </summary>
    public FontStyle FontStyle => IsItalic ? FontStyle.Italic : FontStyle.Normal;

    /// <summary>
    /// Gets the text decorations based on formatting.
    /// </summary>
    public TextDecorationCollection? TextDecorations
    {
        get
        {
            if (!IsUnderline && !IsStrikethrough)
            {
                return null;
            }

            var decorations = new TextDecorationCollection();

            if (IsUnderline)
            {
                decorations.Add(new TextDecoration { Location = TextDecorationLocation.Underline });
            }

            if (IsStrikethrough)
            {
                decorations.Add(new TextDecoration { Location = TextDecorationLocation.Strikethrough });
            }

            return decorations;
        }
    }

    /// <summary>
    /// Creates a clone of this segment.
    /// </summary>
    /// <returns>A new TextSegment with the same properties.</returns>
    public TextSegment Clone() => new(Text, StartIndex)
    {
        IsBold = IsBold,
        IsItalic = IsItalic,
        IsUnderline = IsUnderline,
        IsStrikethrough = IsStrikethrough,
        Foreground = Foreground,
        Background = Background,
        FontSize = FontSize,
        FontFamily = FontFamily,
        IsLineBreak = IsLineBreak,
        IsParagraphBreak = IsParagraphBreak,
        IsImage = IsImage,
        ImageSource = ImageSource,
        ImageWidth = ImageWidth,
        ImageHeight = ImageHeight,
        ImageAlignment = ImageAlignment,
        ParagraphAlignment = ParagraphAlignment,
    };

    /// <summary>
    /// Checks if this segment has the same formatting as another segment.
    /// </summary>
    /// <param name="other">The other segment to compare.</param>
    /// <returns>True if formatting matches.</returns>
    public bool HasSameFormatting(TextSegment other)
    {
        if (other is null)
        {
            return false;
        }

        return IsBold == other.IsBold &&
               IsItalic == other.IsItalic &&
               IsUnderline == other.IsUnderline &&
               IsStrikethrough == other.IsStrikethrough &&
               Equals(Foreground, other.Foreground) &&
               Equals(Background, other.Background) &&
               FontSize == other.FontSize &&
               Equals(FontFamily, other.FontFamily) &&
               IsLineBreak == other.IsLineBreak &&
               IsParagraphBreak == other.IsParagraphBreak &&
               IsImage == other.IsImage &&
               ImageAlignment == other.ImageAlignment;
    }

#pragma warning disable SA1204 // Static members should appear before instance members
    /// <summary>
    /// Creates a line break segment used for block separation.
    /// </summary>
    /// <param name="offset">The document offset associated with the break.</param>
    /// <returns>A <see cref="TextSegment"/> representing a line break.</returns>
    public static TextSegment CreateLineBreak(int offset) => new(Environment.NewLine, offset)
    {
        IsLineBreak = true,
    };

    /// <summary>
    /// Creates a paragraph break marker.
    /// </summary>
    /// <param name="offset">The document offset associated with the break.</param>
    /// <param name="alignment">Optional alignment metadata for the paragraph.</param>
    /// <returns>A <see cref="TextSegment"/> describing the break.</returns>
    public static TextSegment CreateParagraphBreak(int offset, TextAlignment? alignment = null) => new(string.Empty, offset)
    {
        IsParagraphBreak = true,
        ParagraphAlignment = alignment,
    };

    /// <summary>
    /// Creates an inline image segment.
    /// </summary>
    /// <param name="offset">The document offset where the image is injected.</param>
    /// <param name="source">The URI or path to the image content.</param>
    /// <param name="alignment">The horizontal alignment applied to the rendered image.</param>
    /// <param name="width">Optional explicit width in device independent pixels.</param>
    /// <param name="height">Optional explicit height in device independent pixels.</param>
    /// <returns>A <see cref="TextSegment"/> describing the image.</returns>
    public static TextSegment CreateImage(int offset, string source, HorizontalAlignment alignment, double? width, double? height) => new(string.Empty, offset)
    {
        IsImage = true,
        ImageSource = source,
        ImageAlignment = alignment,
        ImageWidth = width,
        ImageHeight = height,
    };
#pragma warning restore SA1204
}
