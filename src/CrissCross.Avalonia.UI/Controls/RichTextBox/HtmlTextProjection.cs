// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Text;

namespace CrissCross.Avalonia.UI.Controls;

internal sealed class HtmlTextProjection
{
    private const string EmbeddedObjectText = "\uFFFC";

    private readonly List<RenderedCharacter> _characters;

    private HtmlTextProjection(string source, string text, List<RenderedCharacter> characters)
    {
        Source = source;
        Text = text;
        _characters = characters;
    }

    public string Source { get; }

    public string Text { get; }

    public int Length => Text.Length;

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
            var current = html[index];
            if (current == '<')
            {
                var tagEnd = html.IndexOf('>', index);
                if (tagEnd < 0)
                {
                    AppendLiteral(html, index, text, characters);
                    index++;
                    continue;
                }

                var tagContent = html.Substring(index + 1, tagEnd - index - 1).Trim();
                if (IsBreakTag(tagContent))
                {
                    AppendMapped(Environment.NewLine, index, tagEnd + 1, text, characters);
                }
                else if (IsImageTag(tagContent))
                {
                    AppendMapped(EmbeddedObjectText, index, tagEnd + 1, text, characters);
                }

                index = tagEnd + 1;
                continue;
            }

            if (current == '&')
            {
                var entityEnd = html.IndexOf(';', index);
                if (entityEnd > index && entityEnd - index <= 16)
                {
                    var entity = html.Substring(index, entityEnd - index + 1);
                    var decoded = WebUtility.HtmlDecode(entity);
                    if (!string.IsNullOrEmpty(decoded) && decoded != entity)
                    {
                        AppendMapped(decoded, index, entityEnd + 1, text, characters);
                        index = entityEnd + 1;
                        continue;
                    }
                }
            }

            AppendLiteral(html, index, text, characters);
            index++;
        }

        return new HtmlTextProjection(html, text.ToString(), characters);
    }

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

        if (boundedOffset >= Length)
        {
            return _characters[^1].SourceEnd;
        }

        return _characters[boundedOffset].SourceStart;
    }

    private static void AppendLiteral(string source, int sourceIndex, StringBuilder text, List<RenderedCharacter> characters)
    {
        text.Append(source[sourceIndex]);
        characters.Add(new RenderedCharacter(sourceIndex, sourceIndex + 1));
    }

    private static void AppendMapped(string rendered, int sourceStart, int sourceEnd, StringBuilder text, List<RenderedCharacter> characters)
    {
        foreach (var character in rendered)
        {
            text.Append(character);
            characters.Add(new RenderedCharacter(sourceStart, sourceEnd));
        }
    }

    private static bool IsBreakTag(string tagContent)
    {
        if (string.IsNullOrWhiteSpace(tagContent))
        {
            return false;
        }

        var tagName = tagContent.TrimStart('/').Split([' ', '/', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.Equals(tagName, "br", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsImageTag(string tagContent)
    {
        if (string.IsNullOrWhiteSpace(tagContent) || tagContent.StartsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        var tagName = tagContent.TrimStart('/').Split([' ', '/', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.Equals(tagName, "img", StringComparison.OrdinalIgnoreCase);
    }

    private readonly record struct RenderedCharacter(int SourceStart, int SourceEnd);
}
