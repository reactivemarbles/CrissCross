// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Edits named document structures while retaining HTML and undo history.</summary>
public partial class RichTextBox
{
    /// <summary>The default page region variant.</summary>
    private const string DefaultPageVariant = "default";

    /// <summary>Applies a named paragraph style with inline CSS that renders immediately.</summary>
    /// <param name="name">The style identifier.</param>
    /// <param name="css">The paragraph CSS declarations.</param>
    public void ApplyParagraphStyle(string name, string css)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(css);
        TransformSelectedParagraphs((document, paragraphs) =>
        {
            foreach (var paragraph in paragraphs)
            {
                paragraph.SetAttribute("data-docx-style", name);
                paragraph.SetAttribute("style", $"{paragraph.GetAttribute("style")};{css}");
            }
        });
    }

    /// <summary>Sets the numbering start for selected numbered list items.</summary>
    /// <param name="start">The first displayed number.</param>
    public void RestartNumbering(int start)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(start, 1);
        TransformSelectedParagraphs((document, paragraphs) =>
        {
            foreach (var paragraph in paragraphs)
            {
                if (paragraph.LocalName == "li" && paragraph.ParentElement?.LocalName == "ol")
                {
                    paragraph.SetAttribute("value", start.ToString(CultureInfo.InvariantCulture));
                    start++;
                }
            }
        });
    }

    /// <summary>Creates a page section from complete selected paragraphs.</summary>
    /// <param name="id">The unique section identifier.</param>
    /// <param name="size">The sheet dimensions in pixels.</param>
    /// <param name="padding">The content margins in pixels.</param>
    public void CreateSection(string id, Size size, Thickness padding)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        RichDocumentStructure.ValidateGeometry(size, padding);
        EnsureUniqueStructure(SectionIdentifierAttribute, id);
        TransformSelectedParagraphs((document, paragraphs) =>
        {
            if (paragraphs.Count == 0 || paragraphs[0].ParentElement is not { } parent || paragraphs.Exists(paragraph => !ReferenceEquals(paragraph.ParentElement, parent)))
            {
                throw new InvalidOperationException("A section requires paragraphs with a common parent.");
            }

            var section = document.CreateElement("section");
            section.SetAttribute(SectionIdentifierAttribute, id);
            section.SetAttribute("data-page-width", size.Width.ToString(CultureInfo.InvariantCulture));
            section.SetAttribute("data-page-height", size.Height.ToString(CultureInfo.InvariantCulture));
            section.SetAttribute("data-page-padding", FormattableString.Invariant($"{padding.Left},{padding.Top},{padding.Right},{padding.Bottom}"));
            _ = parent.InsertBefore(section, paragraphs[0]);
            foreach (var paragraph in paragraphs)
            {
                _ = section.AppendChild(paragraph);
            }
        });
    }

    /// <summary>Sets repeated page header or footer HTML outside editable body coordinates.</summary>
    /// <param name="html">The region HTML.</param>
    public void SetPageRegion(string html) => SetPageRegion(html, isFooter: false, sectionId: null);

    /// <summary>Sets a repeated document header or footer.</summary>
    /// <param name="html">The region HTML.</param>
    /// <param name="isFooter">Whether to set the footer.</param>
    public void SetPageRegion(string html, bool isFooter) => SetPageRegion(html, isFooter, sectionId: null);

    /// <summary>Sets repeated page header or footer HTML outside editable body coordinates.</summary>
    /// <param name="html">The region HTML.</param>
    /// <param name="isFooter">Whether to set the footer.</param>
    /// <param name="sectionId">The section identifier, or null for the whole document.</param>
    public void SetPageRegion(string html, bool isFooter, string? sectionId) => SetPageRegion(html, isFooter, sectionId, DefaultPageVariant);

    /// <summary>Sets a repeated region for default, first, even, or odd section pages.</summary>
    /// <param name="html">The region HTML.</param>
    /// <param name="isFooter">Whether to set the footer.</param>
    /// <param name="sectionId">The section identifier, or null for the document.</param>
    /// <param name="variant">The default, first, even, or odd variant.</param>
    public void SetPageRegion(string html, bool isFooter, string? sectionId, string variant)
    {
        ArgumentNullException.ThrowIfNull(html);
        if (variant is not (DefaultPageVariant or "first" or "even" or "odd"))
        {
            throw new ArgumentException("The page variant must be default, first, even, or odd.", nameof(variant));
        }

        MutateStructures(document =>
        {
            var region = isFooter ? "footer" : "header";
            foreach (var previous in document.QuerySelectorAll("template[data-page-region]"))
            {
                if (previous.GetAttribute("data-page-region") == region
                    && (previous.GetAttribute(SectionIdentifierAttribute) ?? string.Empty) == (sectionId ?? string.Empty)
                    && (previous.GetAttribute("data-page-variant") ?? DefaultPageVariant) == variant)
                {
                    previous.Remove();
                }
            }

            var template = document.CreateElement("template");
            template.SetAttribute("data-page-region", region);
            template.SetAttribute(SectionIdentifierAttribute, sectionId ?? string.Empty);
            template.SetAttribute("data-page-variant", variant);
            template.InnerHtml = html;
            _ = document.Body?.AppendChild(template);
        });
    }
}
