// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

namespace CrissCross.Avalonia.UI.Controls;

internal static class HtmlFormattingHelper
{
    public static (string? Content, bool Applied) Toggle(string source, int start, int length, TextFormatType formatType)
    {
        if (string.IsNullOrEmpty(source) || length <= 0 || start < 0 || start >= source.Length)
        {
            return (source, false);
        }

        var boundedLength = Math.Min(length, source.Length - start);
        var selection = source.Substring(start, boundedLength);
        var (openTag, closeTag) = GetTags(formatType);
        if (string.IsNullOrEmpty(openTag) || string.IsNullOrEmpty(closeTag))
        {
            return (source, false);
        }

        if (selection.StartsWith(openTag, StringComparison.OrdinalIgnoreCase) &&
            selection.EndsWith(closeTag, StringComparison.OrdinalIgnoreCase))
        {
            var inner = selection[openTag.Length..(selection.Length - closeTag.Length)];
            var builder = new StringBuilder(source.Length - openTag.Length - closeTag.Length);
            _ = builder.Append(source, 0, start);
            _ = builder.Append(inner);
            _ = builder.Append(source, start + boundedLength, source.Length - start - boundedLength);
            return (builder.ToString(), false);
        }

        var wrappedBuilder = new StringBuilder(source.Length + openTag.Length + closeTag.Length);
        _ = wrappedBuilder.Append(source, 0, start);
        _ = wrappedBuilder.Append(openTag);
        _ = wrappedBuilder.Append(selection);
        _ = wrappedBuilder.Append(closeTag);
        _ = wrappedBuilder.Append(source, start + boundedLength, source.Length - start - boundedLength);
        return (wrappedBuilder.ToString(), true);
    }

    private static (string OpenTag, string CloseTag) GetTags(TextFormatType formatType) => formatType switch
    {
        TextFormatType.Bold => ("<strong>", "</strong>"),
        TextFormatType.Italic => ("<em>", "</em>"),
        TextFormatType.Underline => ("<u>", "</u>"),
        TextFormatType.Strikethrough => ("<s>", "</s>"),
        _ => (string.Empty, string.Empty),
    };
}
