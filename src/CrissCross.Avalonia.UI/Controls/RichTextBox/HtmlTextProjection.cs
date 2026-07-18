// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the HtmlTextProjection member.</summary>
internal sealed class HtmlTextProjection
{
    /// <summary>Provides the EmbeddedObjectText member.</summary>
    private const string EmbeddedObjectText = "\uFFFC";

    /// <summary>Provides the MaximumEntityLength member.</summary>
    private const int MaximumEntityLength = 16;

    /// <summary>HTML block elements whose closing tag contributes a paragraph break.</summary>
    private static readonly HashSet<string> BlockEndTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "P",
        "DIV",
        "LI",
        "TR",
        "H1",
        "H2",
        "H3",
        "H4",
        "H5",
        "H6",
    };

    /// <summary>Provides the documented member.</summary>
    private readonly List<RenderedCharacter> _characters;

    /// <summary>Initializes a new instance of the <see cref="HtmlTextProjection"/> class.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="text">The text value.</param>
    /// <param name="characters">The characters value.</param>
    private HtmlTextProjection(string source, string text, List<RenderedCharacter> characters)
    {
        Source = source;
        Text = text;
        _characters = characters;
    }

    /// <summary>Gets the value.</summary>
    public string Source { get; }

    /// <summary>Gets the value.</summary>
    public string Text { get; }

    /// <summary>Gets the Length value.</summary>
    public int Length => Text.Length;

    /// <summary>Provides the Create member.</summary>
    /// <param name="source">The source value.</param>
    /// <returns>The result.</returns>
    public static HtmlTextProjection Create(string? source)
    {
        var html = source ?? string.Empty;
        if (html.Length == 0)
        {
            return new HtmlTextProjection(string.Empty, string.Empty, []);
        }

        var text = new StringBuilder(html.Length);
        var characters = new List<RenderedCharacter>(html.Length);
        var index = 0;

        while (index < html.Length)
        {
            if (TryAppendTag(html, ref index, text, characters))
            {
                continue;
            }

            if (TryAppendEntity(html, ref index, text, characters))
            {
                continue;
            }

            AppendLiteral(html, index, text, characters);
            index++;
        }

        return new HtmlTextProjection(html, text.ToString(), characters);
    }

    /// <summary>Provides the GetRangeText member.</summary>
    /// <param name="start">The start value.</param>
    /// <param name="length">The length value.</param>
    /// <returns>The result.</returns>
    public string GetRangeText(int start, int length)
    {
        if (length <= 0 || Length == 0)
        {
            return string.Empty;
        }

        var boundedStart = Math.Clamp(start, 0, Length);
        var boundedLength = Math.Min(length, Length - boundedStart);
        return boundedLength <= 0 ? string.Empty : Text.Substring(boundedStart, boundedLength);
    }

    /// <summary>Provides the GetSourceRange member.</summary>
    /// <param name="start">The start value.</param>
    /// <param name="length">The length value.</param>
    /// <returns>The result.</returns>
    public (int Start, int Length) GetSourceRange(int start, int length)
    {
        if (length <= 0 || _characters.Count == 0)
        {
            var insertionOffset = GetSourceInsertionOffset(start);
            return (insertionOffset, 0);
        }

        var boundedStart = Math.Clamp(start, 0, Length);
        var boundedEnd = Math.Clamp(boundedStart + length, boundedStart, Length);
        if (boundedStart >= boundedEnd)
        {
            var insertionOffset = GetSourceInsertionOffset(boundedStart);
            return (insertionOffset, 0);
        }

        var rawStart = _characters[boundedStart].SourceStart;
        var rawEnd = _characters[boundedEnd - 1].SourceEnd;
        return (rawStart, rawEnd - rawStart);
    }

    /// <summary>Provides the GetSourceInsertionOffset member.</summary>
    /// <param name="offset">The offset value.</param>
    /// <returns>The result.</returns>
    public int GetSourceInsertionOffset(int offset)
    {
        if (_characters.Count == 0)
        {
            return Math.Clamp(offset, 0, Source.Length);
        }

        var boundedOffset = Math.Clamp(offset, 0, Length);
        if (boundedOffset == 0)
        {
            return _characters[0].SourceStart;
        }

        return boundedOffset >= Length ? _characters[^1].SourceEnd : _characters[boundedOffset].SourceStart;
    }

    /// <summary>Provides the AppendLiteral member.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="sourceIndex">The sourceIndex value.</param>
    /// <param name="text">The text value.</param>
    /// <param name="characters">The characters value.</param>
    private static void AppendLiteral(
        string source,
        int sourceIndex,
        StringBuilder text,
        List<RenderedCharacter> characters)
    {
        _ = text.Append(source[sourceIndex]);
        characters.Add(new RenderedCharacter(sourceIndex, sourceIndex + 1));
    }

    /// <summary>Provides the AppendMapped member.</summary>
    /// <param name="rendered">The rendered value.</param>
    /// <param name="sourceStart">The sourceStart value.</param>
    /// <param name="sourceEnd">The sourceEnd value.</param>
    /// <param name="text">The text value.</param>
    /// <param name="characters">The characters value.</param>
    private static void AppendMapped(
        string rendered,
        int sourceStart,
        int sourceEnd,
        StringBuilder text,
        List<RenderedCharacter> characters)
    {
        foreach (var character in rendered)
        {
            _ = text.Append(character);
            characters.Add(new RenderedCharacter(sourceStart, sourceEnd));
        }
    }

    /// <summary>Tries to append a mapped tag.</summary>
    /// <param name="source">The source HTML.</param>
    /// <param name="index">The current source index.</param>
    /// <param name="text">The rendered text builder.</param>
    /// <param name="characters">The source map.</param>
    /// <returns><see langword="true"/> when a tag was consumed.</returns>
    private static bool TryAppendTag(
        string source,
        ref int index,
        StringBuilder text,
        List<RenderedCharacter> characters)
    {
        if (source[index] != '<')
        {
            return false;
        }

        var tagEnd = source.IndexOf('>', index);
        if (tagEnd < 0)
        {
            AppendLiteral(source, index, text, characters);
            index++;
            return true;
        }

        var tagContent = source.Substring(index + 1, tagEnd - index - 1).Trim();
        AppendKnownTag(tagContent, index, tagEnd + 1, text, characters);
        index = tagEnd + 1;
        return true;
    }

    /// <summary>Appends the rendered representation for supported tags.</summary>
    /// <param name="tagContent">The tag content.</param>
    /// <param name="sourceStart">The source start offset.</param>
    /// <param name="sourceEnd">The source end offset.</param>
    /// <param name="text">The rendered text builder.</param>
    /// <param name="characters">The source map.</param>
    private static void AppendKnownTag(
        string tagContent,
        int sourceStart,
        int sourceEnd,
        StringBuilder text,
        List<RenderedCharacter> characters)
    {
        if (IsBreakTag(tagContent))
        {
            AppendMapped(Environment.NewLine, sourceStart, sourceEnd, text, characters);
            return;
        }

        if (IsBlockEndTag(tagContent))
        {
            AppendMapped(Environment.NewLine, sourceStart, sourceEnd, text, characters);
            return;
        }

        if (!IsImageTag(tagContent))
        {
            return;
        }

        AppendMapped(EmbeddedObjectText, sourceStart, sourceEnd, text, characters);
    }

    /// <summary>Tries to append a decoded HTML entity.</summary>
    /// <param name="source">The source HTML.</param>
    /// <param name="index">The current source index.</param>
    /// <param name="text">The rendered text builder.</param>
    /// <param name="characters">The source map.</param>
    /// <returns><see langword="true"/> when an entity was consumed.</returns>
    private static bool TryAppendEntity(
        string source,
        ref int index,
        StringBuilder text,
        List<RenderedCharacter> characters)
    {
        if (source[index] != '&')
        {
            return false;
        }

        var entityEnd = source.IndexOf(';', index);
        if (entityEnd <= index || entityEnd - index > MaximumEntityLength)
        {
            return false;
        }

        var entity = source.Substring(index, entityEnd - index + 1);
        var decoded = WebUtility.HtmlDecode(entity);
        if (string.IsNullOrEmpty(decoded) || decoded == entity)
        {
            return false;
        }

        AppendMapped(decoded, index, entityEnd + 1, text, characters);
        index = entityEnd + 1;
        return true;
    }

    /// <summary>Provides the IsBreakTag member.</summary>
    /// <param name="tagContent">The tagContent value.</param>
    /// <returns>The result.</returns>
    private static bool IsBreakTag(string tagContent)
    {
        if (string.IsNullOrWhiteSpace(tagContent))
        {
            return false;
        }

        var tagName = tagContent
            .TrimStart('/')
            .Split([' ', '/', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
        return string.Equals(tagName, "br", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Determines whether a tag closes a block that contributes a paragraph break.</summary>
    /// <param name="tagContent">The tag content.</param>
    /// <returns><see langword="true"/> when the tag contributes a rendered line break.</returns>
    private static bool IsBlockEndTag(string tagContent)
    {
        if (!tagContent.StartsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        var tagName = tagContent[1..]
            .Split([' ', '/', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
        return tagName is not null && BlockEndTags.Contains(tagName);
    }

    /// <summary>Provides the IsImageTag member.</summary>
    /// <param name="tagContent">The tagContent value.</param>
    /// <returns>The result.</returns>
    private static bool IsImageTag(string tagContent)
    {
        if (string.IsNullOrWhiteSpace(tagContent) || tagContent.StartsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        var tagName = tagContent
            .TrimStart('/')
            .Split([' ', '/', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
        return string.Equals(tagName, "img", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Provides the RenderedCharacter member.</summary>
    /// <param name="SourceStart">The SourceStart value.</param>
    /// <param name="SourceEnd">The SourceEnd value.</param>
    private readonly record struct RenderedCharacter(int SourceStart, int SourceEnd);
}
