// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reads section geometry and page chrome without adding body text positions.</summary>
internal static class RichDocumentStructure
{
    /// <summary>The section identifier attribute.</summary>
    private const string SectionIdentifierAttribute = "data-section-id";

    /// <summary>The number of margin values.</summary>
    private const int MarginValueCount = 4;

    /// <summary>Validates a page's printable area.</summary>
    /// <param name="size">The page size.</param>
    /// <param name="padding">The margins.</param>
    internal static void ValidateGeometry(Size size, in Thickness padding)
    {
        if (!double.IsFinite(size.Width) || !double.IsFinite(size.Height) || size.Width <= 0 || size.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        if (IsValidPadding(padding, size))
        {
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(padding));
    }

    /// <summary>Reads contiguous section ranges and defaults for unsectioned paragraphs.</summary>
    /// <param name="html">The document HTML.</param>
    /// <param name="size">The default sheet size.</param>
    /// <param name="padding">The default margins.</param>
    /// <returns>The section ranges.</returns>
    internal static List<Section> ReadSections(string html, Size size, in Thickness padding)
    {
        ValidateGeometry(size, padding);
        var document = new HtmlParser(new HtmlParserOptions { IsKeepingSourceReferences = true }).ParseDocument(html);
        var sections = new List<Section>();
        var rangeEnd = 0;
        foreach (var element in document.QuerySelectorAll("section[data-section-id]"))
        {
            if (element.SourceReference is not { } source || element.ParentElement?.Closest("section[data-section-id]") is not null)
            {
                continue;
            }

            var start = HtmlTextProjection.Create(html[..source.Position.Index]).Length;
            if (start > rangeEnd)
            {
                sections.Add(CreateSection(document, string.Empty, rangeEnd, start, size, padding));
            }

            var sectionSize = new Size(ReadNumber(element, "data-page-width", size.Width), ReadNumber(element, "data-page-height", size.Height));
            var sectionPadding = ReadPadding(element.GetAttribute("data-page-padding"), padding);
            ValidateGeometry(sectionSize, sectionPadding);
            rangeEnd = start + HtmlTextProjection.Create(element.InnerHtml).Length;
            sections.Add(CreateSection(document, element.GetAttribute(SectionIdentifierAttribute) ?? string.Empty, start, rangeEnd, sectionSize, sectionPadding));
        }

        var length = HtmlTextProjection.Create(html).Length;
        if (rangeEnd < length || sections.Count == 0)
        {
            sections.Add(CreateSection(document, string.Empty, rangeEnd, length, size, padding));
        }

        return sections;
    }

    /// <summary>Checks finite nonnegative margins and positive content dimensions.</summary>
    /// <param name="padding">The margins.</param>
    /// <param name="size">The paper size.</param>
    /// <returns>Whether the margins are valid.</returns>
    private static bool IsValidPadding(in Thickness padding, Size size) =>
        IsNonnegative(padding.Left) && IsNonnegative(padding.Top) && IsNonnegative(padding.Right) && IsNonnegative(padding.Bottom)
        && padding.Left + padding.Right < size.Width && padding.Top + padding.Bottom < size.Height;

    /// <summary>Checks a margin.</summary>
    /// <param name="value">The margin.</param>
    /// <returns>Whether the margin is finite and nonnegative.</returns>
    private static bool IsNonnegative(double value) => double.IsFinite(value) && value >= 0;

    /// <summary>Creates a section and resolves its repeated regions.</summary>
    /// <param name="document">The parsed source.</param>
    /// <param name="id">The section identifier.</param>
    /// <param name="start">The body start.</param>
    /// <param name="end">The body end.</param>
    /// <param name="size">The sheet size.</param>
    /// <param name="padding">The margins.</param>
    /// <returns>The section.</returns>
    private static Section CreateSection(IDocument document, string id, int start, int end, Size size, in Thickness padding) =>
        new(start, end, size, padding, RichPageRegion.Read(document, id, "header"), RichPageRegion.Read(document, id, "footer"));

    /// <summary>Reads an invariant finite positive geometry value.</summary>
    /// <param name="element">The section.</param>
    /// <param name="attribute">The attribute name.</param>
    /// <param name="fallback">The default value.</param>
    /// <returns>The geometry value.</returns>
    private static double ReadNumber(IElement element, string attribute, double fallback)
    {
        var value = element.GetAttribute(attribute);
        if (value is null)
        {
            return fallback;
        }

        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number) && number > 0
            ? number
            : throw new FormatException($"Invalid section geometry: {attribute}.");
    }

    /// <summary>Reads four invariant comma separated margins.</summary>
    /// <param name="value">The margin attribute.</param>
    /// <param name="fallback">The default margins.</param>
    /// <returns>The margins.</returns>
    private static Thickness ReadPadding(string? value, in Thickness fallback)
    {
        if (value is null)
        {
            return fallback;
        }

        var values = value.Split(',');
        if (values.Length != MarginValueCount)
        {
            throw new FormatException("Section padding requires left, top, right, and bottom values.");
        }

        return new(
            double.Parse(values[0], CultureInfo.InvariantCulture),
            double.Parse(values[1], CultureInfo.InvariantCulture),
            double.Parse(values[2], CultureInfo.InvariantCulture),
            double.Parse(values[3], CultureInfo.InvariantCulture));
    }

    /// <summary>A printable section and its body coordinates.</summary>
    /// <param name="Start">The first body offset.</param>
    /// <param name="End">The exclusive last body offset.</param>
    /// <param name="Size">The paper size.</param>
    /// <param name="Padding">The printable margins.</param>
    /// <param name="Header">The repeated header HTML.</param>
    /// <param name="Footer">The repeated footer HTML.</param>
    internal sealed record Section(int Start, int End, Size Size, Thickness Padding, RichPageRegion Header, RichPageRegion Footer);
}
