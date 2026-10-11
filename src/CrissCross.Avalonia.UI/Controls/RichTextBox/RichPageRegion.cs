// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Resolves page region variants and dynamic page fields.</summary>
internal sealed class RichPageRegion
{
    /// <summary>The variant content.</summary>
    private readonly Dictionary<string, string> _variants = [with(StringComparer.Ordinal)];

    /// <summary>Reads a section's page region variants with document fallbacks.</summary>
    /// <param name="document">The parsed source.</param>
    /// <param name="id">The section identifier.</param>
    /// <param name="kind">The header or footer region kind.</param>
    /// <returns>The resolved variants.</returns>
    internal static RichPageRegion Read(IDocument document, string id, string kind)
    {
        var region = new RichPageRegion();
        foreach (var element in document.QuerySelectorAll("template[data-page-region]"))
        {
            if (element.GetAttribute("data-page-region") == kind && string.IsNullOrEmpty(element.GetAttribute("data-section-id")))
            {
                region.Add(element);
            }
        }

        foreach (var element in document.QuerySelectorAll("template[data-page-region]"))
        {
            if (element.GetAttribute("data-page-region") == kind && element.GetAttribute("data-section-id") == id)
            {
                region.Add(element);
            }
        }

        return region;
    }

    /// <summary>Renders dynamic fields into a standalone page region.</summary>
    /// <param name="html">The region source.</param>
    /// <param name="page">The one-based physical sheet number.</param>
    /// <param name="total">The measured total sheet count.</param>
    /// <returns>The resolved HTML.</returns>
    internal static string RenderFields(string html, int page, int total)
    {
        var document = new HtmlParser().ParseDocument(html);
        foreach (var field in document.QuerySelectorAll("[data-page-field]"))
        {
            var number = field.GetAttribute("data-page-field") switch
            {
                "PAGE" => page,
                "NUMPAGES" => total,
                _ => 0,
            };
            if (number > 0)
            {
                field.TextContent = number.ToString(CultureInfo.InvariantCulture);
            }
        }

        return document.Body?.InnerHtml ?? string.Empty;
    }

    /// <summary>Selects the first-page or parity variant, falling back to default.</summary>
    /// <param name="page">The one-based physical sheet number.</param>
    /// <param name="first">Whether this is the section's first sheet.</param>
    /// <returns>The selected region HTML.</returns>
    internal string Select(int page, bool first)
    {
        if (first && _variants.TryGetValue("first", out var firstHtml))
        {
            return firstHtml;
        }

        var parity = (page & 1) == 0 ? "even" : "odd";
        return _variants.TryGetValue(parity, out var html) ? html : _variants.GetValueOrDefault("default") ?? string.Empty;
    }

    /// <summary>Adds one variant, treating unmarked templates as default.</summary>
    /// <param name="element">The region template.</param>
    private void Add(IElement element) => _variants[element.GetAttribute("data-page-variant") ?? "default"] =
        element is IHtmlTemplateElement template ? template.Content.ToHtml() : element.InnerHtml;
}
