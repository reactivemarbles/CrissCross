// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Markdig;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides the MarkdownUtilities member.</summary>
internal static class MarkdownUtilities
{
    /// <summary>Provides the ToHtml member.</summary>
    /// <param name="markdown">The markdown value.</param>
    /// <returns>The result.</returns>
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
