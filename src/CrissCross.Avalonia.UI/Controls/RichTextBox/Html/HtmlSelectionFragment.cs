// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Copies source markup while preserving the ancestors of a text selection.</summary>
internal static class HtmlSelectionFragment
{
    /// <summary>HTML elements that have no closing tag.</summary>
    private static readonly HashSet<string> VoidElements = [with(StringComparer.OrdinalIgnoreCase),
        "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr",
    ];

    /// <summary>Creates a balanced fragment for rendered document positions.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The selected rendered offset.</param>
    /// <param name="length">The selected rendered length.</param>
    /// <returns>The selected markup with its inherited formatting.</returns>
    internal static string Create(string source, int start, int length)
    {
        var projection = HtmlTextProjection.Create(source);
        if (length <= 0 || start >= projection.Length)
        {
            return string.Empty;
        }

        if (start <= 0 && length >= projection.Length)
        {
            return source;
        }

        var (sourceStart, sourceLength) = projection.GetSourceRange(start, length);
        var ancestors = new List<(string Name, string Markup)>();
        TrackElements(source, 0, sourceStart, ancestors);
        var result = new StringBuilder();
        foreach (var ancestor in ancestors)
        {
            _ = result.Append(ancestor.Markup);
        }

        _ = result.Append(source, sourceStart, sourceLength);
        TrackElements(source, sourceStart, sourceStart + sourceLength, ancestors);
        for (var index = ancestors.Count - 1; index >= 0; index--)
        {
            _ = result.Append("</").Append(ancestors[index].Name).Append('>');
        }

        return result.ToString();
    }

    /// <summary>Replaces a rendered range while balancing the retained formatting elements.</summary>
    /// <param name="source">The document markup.</param>
    /// <param name="start">The rendered start offset.</param>
    /// <param name="length">The rendered selection length.</param>
    /// <param name="replacement">The inserted markup.</param>
    /// <returns>The balanced document markup.</returns>
    internal static string Replace(string source, int start, int length, string replacement)
    {
        var projection = HtmlTextProjection.Create(source);
        if (start <= 0 && length >= projection.Length && length > 0)
        {
            return replacement;
        }

        var (sourceStart, sourceLength) = projection.GetSourceRange(start, length);
        var before = new List<(string Name, string Markup)>();
        TrackElements(source, 0, sourceStart, before);
        var after = new List<(string Name, string Markup)>(before);
        TrackElements(source, sourceStart, sourceStart + sourceLength, after);
        var shared = 0;
        while (shared < before.Count && shared < after.Count && before[shared] == after[shared])
        {
            shared++;
        }

        var result = new StringBuilder(source.Length + replacement.Length);
        _ = result.Append(source, 0, sourceStart).Append(replacement);
        for (var index = before.Count - 1; index >= shared; index--)
        {
            _ = result.Append("</").Append(before[index].Name).Append('>');
        }

        for (var index = shared; index < after.Count; index++)
        {
            _ = result.Append(after[index].Markup);
        }

        _ = result.Append(source, sourceStart + sourceLength, source.Length - sourceStart - sourceLength);
        return result.ToString();
    }

    /// <summary>Tracks open elements in a source interval.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The interval start.</param>
    /// <param name="end">The interval end.</param>
    /// <param name="elements">The open element stack.</param>
    private static void TrackElements(string source, int start, int end, List<(string Name, string Markup)> elements)
    {
        var index = start;
        while (index < end)
        {
            if (source[index] != '<')
            {
                index++;
                continue;
            }

            var close = FindTagEnd(source, index + 1);
            if (close < 0 || close >= end)
            {
                break;
            }

            var tag = source.AsSpan(index + 1, close - index - 1).Trim();
            if (!tag.IsEmpty && tag[0] is not '!' and not '?')
            {
                TrackTag(tag, source[index..(close + 1)], elements);
            }

            index = close + 1;
        }
    }

    /// <summary>Updates the open element stack for one tag.</summary>
    /// <param name="tag">The tag content.</param>
    /// <param name="markup">The original markup.</param>
    /// <param name="elements">The open element stack.</param>
    private static void TrackTag(ReadOnlySpan<char> tag, string markup, List<(string Name, string Markup)> elements)
    {
        var closing = tag[0] == '/';
        var nameStart = closing ? 1 : 0;
        var nameEnd = nameStart;
        while (nameEnd < tag.Length && !char.IsWhiteSpace(tag[nameEnd]) && tag[nameEnd] != '/')
        {
            nameEnd++;
        }

        var name = tag[nameStart..nameEnd].ToString();
        if (closing)
        {
            var match = elements.FindLastIndex(element => string.Equals(element.Name, name, StringComparison.OrdinalIgnoreCase));
            if (match >= 0)
            {
                elements.RemoveRange(match, elements.Count - match);
            }
        }
        else if (!VoidElements.Contains(name) && tag[^1] != '/')
        {
            elements.Add((name, markup));
        }
    }

    /// <summary>Finds a tag boundary outside quoted attribute values.</summary>
    /// <param name="source">The source markup.</param>
    /// <param name="start">The search start.</param>
    /// <returns>The boundary offset or minus one.</returns>
    private static int FindTagEnd(string source, int start)
    {
        var quote = '\0';
        for (var index = start; index < source.Length; index++)
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
