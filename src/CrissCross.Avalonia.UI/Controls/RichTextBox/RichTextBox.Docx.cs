// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Net;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Preserves editor paper settings in native Word exports.</summary>
public partial class RichTextBox
{
    /// <summary>Supplies default section geometry when the body has no explicit sections.</summary>
    /// <returns>The body and its Word paper settings.</returns>
    private string GetDocxExportHtml()
    {
        var document = new HtmlParser().ParseDocument(HtmlDocumentNormalizer.Normalize(Html));
        if (document.Head is { } head && document.Body is { } body)
        {
            foreach (var definition in head.QuerySelectorAll("template[data-note-kind],template[data-page-region],template[data-docx-package]"))
            {
                _ = body.AppendChild(definition);
            }
        }

        if (document.QuerySelector("section[data-section-id]") is not null)
        {
            return document.DocumentElement.OuterHtml;
        }

        RichDocumentStructure.ValidateGeometry(PageSize, PagePadding);
        var padding = FormattableString.Invariant($"{PagePadding.Left},{PagePadding.Top},{PagePadding.Right},{PagePadding.Bottom}");
        var width = PageSize.Width.ToString(CultureInfo.InvariantCulture);
        var height = PageSize.Height.ToString(CultureInfo.InvariantCulture);
        var style = document.Body?.GetAttribute("style");
        var styleAttribute = style is null ? string.Empty : $" style=\"{WebUtility.HtmlEncode(style)}\"";
        return $"<section data-section-id=\"document\" data-page-width=\"{width}\" data-page-height=\"{height}\" data-page-padding=\"{padding}\"{styleAttribute}>"
            + $"{document.Body?.InnerHtml ?? string.Empty}</section>";
    }
}
