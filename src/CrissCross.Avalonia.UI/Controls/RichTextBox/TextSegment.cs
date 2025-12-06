// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a segment of text with optional formatting.
/// </summary>
public class TextSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextSegment"/> class.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <param name="startIndex">The start index in the document.</param>
    public TextSegment(string text, int startIndex)
    {
        Text = text ?? string.Empty;
        StartIndex = startIndex;
    }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the start index in the document.
    /// </summary>
    public int StartIndex { get; set; }

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
        FontFamily = FontFamily
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
               Equals(FontFamily, other.FontFamily);
    }
}
