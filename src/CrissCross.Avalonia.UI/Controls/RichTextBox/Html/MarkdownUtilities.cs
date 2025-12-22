// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Markdig;

namespace CrissCross.Avalonia.UI.Controls;

internal static class MarkdownUtilities
{
    public static string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        try
        {
            return Markdown.ToHtml(markdown);
        }
        catch
        {
            return markdown;
        }
    }
}
