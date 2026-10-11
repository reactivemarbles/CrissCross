// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Collapses source formatting whitespace in imported HTML documents.</summary>
internal static class HtmlSourceWhitespace
{
    /// <summary>Block elements whose surrounding source whitespace is not document content.</summary>
    private static readonly HashSet<string> BlockElements =
    [
        "html", "body", "p", "div", "section", "blockquote", "li", "ul", "ol", "pre",
        "h1", "h2", "h3", "h4", "h5", "h6", "table", "thead", "tbody", "tfoot", "tr", "td", "th",
    ];

    /// <summary>Normalizes text nodes while retaining explicit preformatted content.</summary>
    /// <param name="node">The document subtree.</param>
    internal static void Normalize(INode node)
    {
        if (node is IElement element && PreservesWhitespace(element))
        {
            return;
        }

        if (node is IText text)
        {
            text.Data = IsFormattingWhitespace(text) ? string.Empty : Collapse(text.Data);
            return;
        }

        foreach (var child in node.ChildNodes)
        {
            Normalize(child);
        }
    }

    /// <summary>Identifies elements that intentionally preserve whitespace.</summary>
    /// <param name="element">The candidate element.</param>
    /// <returns>Whether its text must remain unchanged.</returns>
    private static bool PreservesWhitespace(IElement element) =>
        element.TagName.Equals("PRE", StringComparison.OrdinalIgnoreCase)
        || (element.GetAttribute("style")?.Contains("white-space:pre", StringComparison.OrdinalIgnoreCase) ?? false)
        || (element.GetAttribute("style")?.Contains("white-space: pre", StringComparison.OrdinalIgnoreCase) ?? false);

    /// <summary>Collapses HTML source spaces without collapsing nonbreaking spaces.</summary>
    /// <param name="source">The source text.</param>
    /// <returns>The normalized text.</returns>
    private static string Collapse(string source)
    {
        var output = new StringBuilder(source.Length);
        var previousSpace = false;
        foreach (var character in source)
        {
            var isSpace = IsCollapsibleSpace(character);
            if (!isSpace || !previousSpace)
            {
                _ = output.Append(isSpace ? ' ' : character);
            }

            previousSpace = isSpace;
        }

        return output.ToString();
    }

    /// <summary>Identifies collapsible HTML whitespace characters.</summary>
    /// <param name="character">The source character.</param>
    /// <returns>Whether it belongs to HTML source whitespace.</returns>
    private static bool IsCollapsibleSpace(char character) => character is ' ' or '\t' or '\r' or '\n' or '\f';

    /// <summary>Identifies source indentation surrounding blocks while preserving spaces between inline runs.</summary>
    /// <param name="text">The text node.</param>
    /// <returns>Whether the node contains only ignorable source indentation.</returns>
    private static bool IsFormattingWhitespace(IText text)
    {
        if (!string.IsNullOrWhiteSpace(text.Data) || (!text.Data.Contains('\n') && !text.Data.Contains('\r')))
        {
            return false;
        }

        var atBoundary = text.PreviousSibling is null || text.NextSibling is null;
        return IsBlock(text.PreviousSibling) || IsBlock(text.NextSibling) || (atBoundary && IsBlock(text.Parent));
    }

    /// <summary>Identifies a block node for whitespace normalization.</summary>
    /// <param name="node">The candidate node.</param>
    /// <returns>Whether it represents a block element.</returns>
    private static bool IsBlock(INode? node) => node is IElement element && BlockElements.Contains(element.LocalName);
}
