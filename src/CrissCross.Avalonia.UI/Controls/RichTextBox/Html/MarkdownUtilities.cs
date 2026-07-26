// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Markdig;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the MarkdownUtilities member.</summary>
internal static class MarkdownUtilities
{
    /// <summary>Provides the ToHtml member.</summary>
    /// <param name="markdown">The markdown value.</param>
    /// <returns>The result.</returns>
    internal static string ToHtml(string? markdown)
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
