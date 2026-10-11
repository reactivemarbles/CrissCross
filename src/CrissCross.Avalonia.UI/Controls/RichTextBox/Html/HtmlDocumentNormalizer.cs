// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Preserves common word processor stylesheet formatting in standalone fragments.</summary>
internal static class HtmlDocumentNormalizer
{
    /// <summary>Name of the HTML style attribute.</summary>
    private const string StyleAttribute = "style";

    /// <summary>Class selector specificity.</summary>
    private const int ClassSpecificity = 10;

    /// <summary>Identifier selector specificity.</summary>
    private const int IdSpecificity = 100;

    /// <summary>Creates a standalone clipboard fragment with stylesheet formatting inlined.</summary>
    /// <param name="html">The source document or fragment.</param>
    /// <returns>The body content without a nested HTML document.</returns>
    internal static string ToStandaloneFragment(string html)
    {
        var normalized = Normalize(html, normalizeWhitespace: true);
        if (!normalized.Contains("<html", StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        var document = new HtmlParser().ParseDocument(normalized);
        return document.Body?.InnerHtml ?? normalized;
    }

    /// <summary>Inlines stylesheet rules and converts Word image elements.</summary>
    /// <param name="html">The original HTML document.</param>
    /// <param name="normalizeWhitespace">Whether imported source wrapping should be collapsed.</param>
    /// <returns>A document with supported formatting inlined.</returns>
    internal static string Normalize(string html, bool normalizeWhitespace = false)
    {
        var needsWhitespace = normalizeWhitespace && (html.Contains('\n') || html.Contains('\r'));
        if (!needsWhitespace && !html.Contains("<style", StringComparison.OrdinalIgnoreCase)
            && !html.Contains("v:imagedata", StringComparison.OrdinalIgnoreCase))
        {
            return html;
        }

        var document = new HtmlParser().ParseDocument(html);
        var rules = ReadStylesheets(document);
        rules.Sort(static (left, right) => left.Specificity != right.Specificity
            ? left.Specificity.CompareTo(right.Specificity)
            : left.Order.CompareTo(right.Order));
        foreach (var element in document.QuerySelectorAll("*"))
        {
            InlineStyles(element, rules);
            ConvertWordImage(document, element);
        }

        if (needsWhitespace)
        {
            HtmlSourceWhitespace.Normalize(document.Body ?? document.DocumentElement);
        }

        return document.DocumentElement.OuterHtml;
    }

    /// <summary>Reads supported rules from the document stylesheets.</summary>
    /// <param name="document">The HTML document.</param>
    /// <returns>The stylesheet rules in source order.</returns>
    private static List<(string Selector, string Declarations, int Specificity, int Order)> ReadStylesheets(IDocument document)
    {
        var rules = new List<(string Selector, string Declarations, int Specificity, int Order)>();
        foreach (var sheet in document.QuerySelectorAll(StyleAttribute))
        {
            ReadRules(sheet.TextContent, rules);
        }

        return rules;
    }

    /// <summary>Reads simple CSS rules from a stylesheet.</summary>
    /// <param name="text">The stylesheet text.</param>
    /// <param name="rules">The destination rules.</param>
    private static void ReadRules(string text, List<(string Selector, string Declarations, int Specificity, int Order)> rules)
    {
        var position = 0;
        while (position < text.Length)
        {
            var open = text.IndexOf('{', position);
            var close = open < 0 ? -1 : text.IndexOf('}', open);
            if (close < 0)
            {
                return;
            }

            var selectors = text[position..open].Replace("<!--", string.Empty, StringComparison.Ordinal).Trim();
            AddSelectors(selectors, text[(open + 1)..close], rules);
            position = close + 1;
        }
    }

    /// <summary>Adds valid simple selectors for one declaration block.</summary>
    /// <param name="selectors">The comma separated selectors.</param>
    /// <param name="declarations">The declarations.</param>
    /// <param name="rules">The destination rules.</param>
    private static void AddSelectors(string selectors, string declarations, List<(string Selector, string Declarations, int Specificity, int Order)> rules)
    {
        foreach (var selector in selectors.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            if (!IsSimpleSelector(selector))
            {
                continue;
            }

            var specificity = selector.Contains('.', StringComparison.Ordinal) ? ClassSpecificity : 1;
            if (selector.Contains('#', StringComparison.Ordinal))
            {
                specificity = IdSpecificity;
            }

            rules.Add((selector, declarations, specificity, rules.Count));
        }
    }

    /// <summary>Inlines stylesheet declarations before the element's inline declarations.</summary>
    /// <param name="element">The destination element.</param>
    /// <param name="rules">The sorted rules.</param>
    private static void InlineStyles(IElement element, List<(string Selector, string Declarations, int Specificity, int Order)> rules)
    {
        var declarations = new StringBuilder();
        foreach (var rule in rules)
        {
            if (MatchesSelector(element, rule.Selector))
            {
                _ = declarations.Append(rule.Declarations).Append(';');
            }
        }

        if (declarations.Length == 0)
        {
            return;
        }

        _ = declarations.Append(element.GetAttribute(StyleAttribute));
        element.SetAttribute(StyleAttribute, declarations.ToString());
    }

    /// <summary>Converts Word VML image references into standard inline images.</summary>
    /// <param name="document">The owner document.</param>
    /// <param name="element">The candidate Word image.</param>
    private static void ConvertWordImage(IDocument document, IElement element)
    {
        if (!element.TagName.Equals("V:IMAGEDATA", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var image = document.CreateElement("img");
        image.SetAttribute("src", element.GetAttribute("src") ?? string.Empty);
        image.SetAttribute(StyleAttribute, element.ParentElement?.GetAttribute(StyleAttribute) ?? string.Empty);
        _ = element.Parent?.ReplaceChild(image, element);
    }

    /// <summary>Tests whether a selector belongs to the supported subset.</summary>
    /// <param name="selector">The selector.</param>
    /// <returns>Whether the selector is supported.</returns>
    private static bool IsSimpleSelector(string selector)
    {
        foreach (var character in selector)
        {
            if (!char.IsLetterOrDigit(character) && character is not ('.' or '#' or '-' or '_' or '*'))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Matches a supported tag, class or identifier selector.</summary>
    /// <param name="element">The candidate element.</param>
    /// <param name="selector">The supported selector.</param>
    /// <returns>Whether the element matches.</returns>
    private static bool MatchesSelector(IElement element, string selector) => element.Matches(selector);
}
