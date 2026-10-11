// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Applies effective formatting to selected HTML leaves without crossing element boundaries.</summary>
internal static class HtmlFormattingHelper
{
    /// <summary>Element names whose contents are not rendered.</summary>
    private static readonly string[] OpaqueElementNames = ["script", "style", "head", "template"];

    /// <summary>Wraps selected HTML text leaves without crossing existing element boundaries.</summary>
    /// <param name="source">The source HTML.</param>
    /// <param name="start">The rendered start.</param>
    /// <param name="length">The rendered length.</param>
    /// <param name="opening">The opening annotation markup.</param>
    /// <param name="closing">The closing annotation markup.</param>
    /// <returns>The annotated source.</returns>
    internal static string WrapMarkup(string source, int start, int length, string opening, string closing)
    {
        var (sourceStart, sourceLength) = HtmlTextProjection.Create(source).GetSourceRange(start, length);
        var result = new StringBuilder(source.Length);
        var position = 0;
        while (position < source.Length)
        {
            var end = GetTokenEnd(source, position);
            var selectedStart = Math.Max(position, sourceStart);
            var selectedEnd = Math.Min(end, sourceStart + sourceLength);
            var image = source.AsSpan(position, end - position).StartsWith("<img", StringComparison.OrdinalIgnoreCase);
            if ((source[position] == '<' && !image) || selectedEnd <= selectedStart)
            {
                _ = result.Append(source, position, end - position);
            }
            else
            {
                _ = result.Append(source, position, selectedStart - position).Append(opening);
                _ = result.Append(source, selectedStart, selectedEnd - selectedStart).Append(closing);
                _ = result.Append(source, selectedEnd, end - selectedEnd);
            }

            position = end;
        }

        return result.ToString();
    }

    /// <summary>Toggles the effective formatting of a rendered selection.</summary>
    /// <param name="source">The source HTML.</param>
    /// <param name="start">The rendered selection start.</param>
    /// <param name="length">The rendered selection length.</param>
    /// <param name="formatType">The requested formatting.</param>
    /// <returns>The updated HTML and effective formatting state.</returns>
    internal static (string? Content, bool Applied) Toggle(string source, int start, int length, TextFormatType formatType)
    {
        var projection = HtmlTextProjection.Create(source);
        var tag = GetTag(formatType);
        if (length <= 0 || start < 0 || start >= projection.Length || tag.Length == 0)
        {
            return (source, false);
        }

        var (hasContent, applied) = GetSelectionState(source, start, length, formatType);
        if (!hasContent)
        {
            return (source, false);
        }

        var (sourceStart, sourceLength) = projection.GetSourceRange(start, length);
        return (ApplyToLeaves(source, (sourceStart, sourceStart + sourceLength), formatType, applied), applied);
    }

    /// <summary>Reads the effective format of selected text.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The rendered start.</param>
    /// <param name="length">The rendered length.</param>
    /// <param name="formatType">The requested format.</param>
    /// <returns>Whether text exists and whether formatting should be applied.</returns>
    private static (bool HasContent, bool Applied) GetSelectionState(string source, int start, int length, TextFormatType formatType)
    {
        var segments = HtmlContentParser.Parse(HtmlSelectionFragment.Create(source, start, length));
        var hasContent = false;
        var applied = false;
        foreach (var segment in GetTextLeaves(segments))
        {
            if (string.IsNullOrWhiteSpace(segment.Text))
            {
                continue;
            }

            hasContent = true;
            applied |= !HasFormatting(segment, formatType);
        }

        return (hasContent, applied);
    }

    /// <summary>Applies the requested state to selected source leaves.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="selection">The selected source boundaries.</param>
    /// <param name="formatType">The requested format.</param>
    /// <param name="applied">Whether the format is being applied.</param>
    /// <returns>The updated source markup.</returns>
    private static string ApplyToLeaves(string source, (int Start, int End) selection, TextFormatType formatType, bool applied)
    {
        var result = new StringBuilder(source.Length);
        var position = 0;
        while (position < source.Length)
        {
            var end = GetTokenEnd(source, position);
            AppendToken(result, source, position, end, selection, formatType, applied);
            position = end;
        }

        return result.ToString();
    }

    /// <summary>Finds the end of one source text or markup token.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The token start.</param>
    /// <returns>The token end.</returns>
    private static int GetTokenEnd(string source, int start)
    {
        if (source[start] != '<')
        {
            var next = source.IndexOf('<', start);
            return next < 0 ? source.Length : next;
        }

        var end = FindTagEnd(source, start) + 1;
        return end <= start ? source.Length : GetOpaqueEnd(source, start, end);
    }

    /// <summary>Copies a token and overrides selected leaf formatting.</summary>
    /// <param name="result">The output buffer.</param>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The token start.</param>
    /// <param name="end">The token end.</param>
    /// <param name="selection">The selected source boundaries.</param>
    /// <param name="formatType">The requested format.</param>
    /// <param name="applied">Whether the format is being applied.</param>
    private static void AppendToken(StringBuilder result, string source, int start, int end, (int Start, int End) selection, TextFormatType formatType, bool applied)
    {
        var image = source.AsSpan(start, end - start).StartsWith("<img", StringComparison.OrdinalIgnoreCase);
        var selectedStart = Math.Max(start, selection.Start);
        var selectedEnd = Math.Min(end, selection.End);
        if ((source[start] == '<' && !image) || selectedEnd <= selectedStart)
        {
            _ = result.Append(source, start, end - start);
            return;
        }

        var tag = GetTag(formatType);
        _ = result.Append(source, start, selectedStart - start);
        var opening = applied ? $"<{tag}>" : GetResetTag(source, selectedStart, selectedEnd - selectedStart, formatType);
        _ = result.Append(opening).Append(source, selectedStart, selectedEnd - selectedStart);
        _ = result.Append(applied ? $"</{tag}>" : "</span>");
        _ = result.Append(source, selectedEnd, end - selectedEnd);
    }

    /// <summary>Gets whether a parsed leaf has the requested effective formatting.</summary>
    /// <param name="segment">The leaf formatting.</param>
    /// <param name="formatType">The requested format.</param>
    /// <returns>Whether the format is active.</returns>
    private static bool HasFormatting(TextSegment segment, TextFormatType formatType) => formatType switch
    {
        TextFormatType.Bold => segment.IsBold,
        TextFormatType.Italic => segment.IsItalic,
        TextFormatType.Underline => segment.IsUnderline,
        TextFormatType.Strikethrough => segment.IsStrikethrough,
        _ => false,
    };

    /// <summary>Gets the canonical HTML formatting element.</summary>
    /// <param name="formatType">The requested format.</param>
    /// <returns>The element name.</returns>
    private static string GetTag(TextFormatType formatType) => formatType switch
    {
        TextFormatType.Bold => "strong",
        TextFormatType.Italic => "em",
        TextFormatType.Underline => "u",
        TextFormatType.Strikethrough => "s",
        _ => string.Empty,
    };

    /// <summary>Overrides inherited formatting while retaining the other decoration.</summary>
    /// <param name="source">The complete source HTML.</param>
    /// <param name="start">The source leaf start.</param>
    /// <param name="length">The source leaf length.</param>
    /// <param name="formatType">The format to remove.</param>
    /// <returns>The opening reset span.</returns>
    private static string GetResetTag(string source, int start, int length, TextFormatType formatType)
    {
        if (formatType == TextFormatType.Bold)
        {
            return "<span style=\"font-weight:normal\">";
        }

        if (formatType == TextFormatType.Italic)
        {
            return "<span style=\"font-style:normal\">";
        }

        var renderedStart = HtmlTextProjection.Create(source[..start]).Length;
        var renderedLength = HtmlTextProjection.Create(source[start..(start + length)]).Length;
        var segments = HtmlContentParser.Parse(HtmlSelectionFragment.Create(source, renderedStart, renderedLength));
        var otherDecoration = false;
        foreach (var segment in GetTextLeaves(segments))
        {
            otherDecoration |= formatType == TextFormatType.Underline ? segment.IsStrikethrough : segment.IsUnderline;
        }

        var retainedDecoration = formatType == TextFormatType.Underline ? "line-through" : "underline";
        var decoration = otherDecoration ? retainedDecoration : "none";
        return $"<span style=\"text-decoration:{decoration}\">";
    }

    /// <summary>Enumerates text leaves inside structural table segments.</summary>
    /// <param name="segments">The parsed document segments.</param>
    /// <returns>The leaves that carry effective character formatting.</returns>
    private static IEnumerable<TextSegment> GetTextLeaves(IReadOnlyList<TextSegment> segments)
    {
        foreach (var segment in segments)
        {
            if (segment.Table is { } table)
            {
                foreach (var cell in table.Cells)
                {
                    foreach (var leaf in GetTextLeaves(cell.Segments))
                    {
                        yield return leaf;
                    }
                }
            }
            else if (!segment.IsImage && segment.ListMarker is null)
            {
                yield return segment;
            }
        }
    }

    /// <summary>Skips comments and non-rendered content as a single unchanged token.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The token start.</param>
    /// <param name="end">The opening tag end.</param>
    /// <returns>The complete opaque token end.</returns>
    private static int GetOpaqueEnd(string source, int start, int end)
    {
        var remaining = source.AsSpan(start);
        if (remaining.StartsWith("<!--", StringComparison.Ordinal))
        {
            var commentEnd = source.IndexOf("-->", start + "<!--".Length, StringComparison.Ordinal);
            return commentEnd < 0 ? source.Length : commentEnd + "-->".Length;
        }

        var name = GetOpaqueName(remaining);
        if (name.Length == 0)
        {
            return end;
        }

        var closing = source.IndexOf($"</{name}", end, StringComparison.OrdinalIgnoreCase);
        var closeEnd = closing < 0 ? -1 : FindTagEnd(source, closing);
        return closeEnd < 0 ? source.Length : closeEnd + 1;
    }

    /// <summary>Identifies an opening tag for hidden content.</summary>
    /// <param name="markup">The source from the opening tag.</param>
    /// <returns>The hidden element name, or an empty string.</returns>
    private static string GetOpaqueName(ReadOnlySpan<char> markup)
    {
        foreach (var name in OpaqueElementNames)
        {
            if (markup.StartsWith($"<{name}", StringComparison.OrdinalIgnoreCase))
            {
                return name;
            }
        }

        return string.Empty;
    }

    /// <summary>Finds a tag boundary outside quoted attributes.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The tag start.</param>
    /// <returns>The closing bracket offset, or minus one.</returns>
    private static int FindTagEnd(string source, int start)
    {
        var quote = '\0';
        for (var index = start + 1; index < source.Length; index++)
        {
            var character = source[index];
            if (quote != '\0')
            {
                if (character == quote)
                {
                    quote = '\0';
                }
            }
            else if (character is '\'' or '"')
            {
                quote = character;
            }
            else if (character == '>')
            {
                return index;
            }
        }

        return -1;
    }
}
