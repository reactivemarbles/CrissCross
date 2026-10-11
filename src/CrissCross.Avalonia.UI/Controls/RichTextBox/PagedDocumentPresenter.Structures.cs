// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Measures section paper geometry and repeated page regions.</summary>
public sealed partial class PagedDocumentPresenter
{
    /// <summary>Gets the measured paper bounds for each section sheet.</summary>
    /// <returns>The arranged sheet rectangles.</returns>
    public IReadOnlyList<Rect> CapturePageBounds()
    {
        var bounds = new Rect[_pages.Count];
        for (var index = 0; index < bounds.Length; index++)
        {
            var page = _pages[index];
            bounds[index] = new(page.Left, page.Top, page.Size.Width, page.SheetHeight);
        }

        return bounds;
    }

    /// <summary>Finds the nearest insertion point on an adjacent page's first or last line.</summary>
    /// <param name="index">The current page index.</param>
    /// <param name="previous">Whether to move to the preceding sheet.</param>
    /// <param name="visualX">The horizontal position on the paged surface.</param>
    /// <returns>The adjacent insertion position.</returns>
    private int GetAdjacentPagePosition(int index, bool previous, double visualX)
    {
        var nextIndex = index + (previous ? -1 : 1);
        if (nextIndex < 0 || nextIndex >= _pages.Count)
        {
            return previous ? 0 : Document?.Length ?? 0;
        }

        var page = _pages[nextIndex];
        var position = previous ? Math.Max(page.Slice.Start, page.Slice.End - 1) : page.Slice.Start;
        var caret = page.Viewport.Presenter.GetDocumentCaretRect(position);
        var point = new Point(visualX - page.Left - page.Padding.Left, caret.Y + (caret.Height * Half));
        return Math.Clamp(page.Viewport.Presenter.GetDocumentPosition(point), page.Slice.Start, page.Slice.End);
    }

    /// <summary>Measures one section at its printable width and creates page slices.</summary>
    /// <param name="section">The section settings and body range.</param>
    private void AddSectionPages(RichDocumentStructure.Section section)
    {
        var width = section.Size.Width - section.Padding.Left - section.Padding.Right;
        var height = section.Size.Height - section.Padding.Top - section.Padding.Bottom;
        var measurement = CreatePresenter();
        var documentHeight = measurement.MeasureDocument(width).Height;
        var (lines, baseTop) = MeasureSectionLines(section, measurement);
        var first = true;
        foreach (var notePage in CreateNotePages(lines, height, width, section))
        {
            var slice = notePage.Slice with { DocumentTop = notePage.Slice.DocumentTop + baseTop };
            if (lines.Count == 0)
            {
                slice = slice with { Start = section.Start, End = section.End };
            }

            AddSectionPage(section, notePage with { Slice = slice }, documentHeight, width, first);
            first = false;
        }
    }

    /// <summary>Selects measured lines belonging to one section.</summary>
    /// <param name="section">The section range.</param>
    /// <param name="measurement">The measured body.</param>
    /// <returns>The section lines and their document top.</returns>
    private (List<DocumentPageLayout.MeasuredLine> Lines, double BaseTop) MeasureSectionLines(RichDocumentStructure.Section section, FormattedTextPresenter measurement)
    {
        var lines = new List<DocumentPageLayout.MeasuredLine>();
        var baseTop = 0.0;
        var text = Document?.PlainText ?? string.Empty;
        foreach (var line in measurement.GetMeasuredDocumentLines())
        {
            if (line.End <= section.Start && line.Start < section.Start)
            {
                baseTop += line.Height;
                continue;
            }

            if (line.Start >= section.End && section.End > section.Start)
            {
                break;
            }

            var start = Math.Max(section.Start, line.Start);
            var end = Math.Min(section.End, line.End);
            if (end < start)
            {
                continue;
            }

            var breakAfter = text.AsSpan(Math.Clamp(start, 0, text.Length), Math.Max(0, Math.Min(end, text.Length) - Math.Clamp(start, 0, text.Length))).Contains('\f');
            lines.Add(new(start, end, line.Height, BreakBefore: HasPageBreakBefore(start, end), BreakAfter: breakAfter));
        }

        return (lines, baseTop);
    }

    /// <summary>Creates one section sheet and its repeated regions.</summary>
    /// <param name="section">The section geometry.</param>
    /// <param name="notePage">The body slice and its notes.</param>
    /// <param name="documentHeight">The measured document height.</param>
    /// <param name="width">The printable width.</param>
    /// <param name="first">Whether this is the first section sheet.</param>
    private void AddSectionPage(RichDocumentStructure.Section section, NotePage notePage, double documentHeight, double width, bool first)
    {
        var slice = notePage.Slice;
        var presenter = CreatePresenter();
        var viewport = new PageViewport(this, presenter, slice.DocumentTop, documentHeight, slice.Height == 0)
        {
            Height = slice.Height > 0 ? slice.Height : DefaultFontSize * EmptyLineHeightScale,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Top,
        };
        var sheetHeight = Math.Max(section.Size.Height, slice.Height + notePage.Height + section.Padding.Top + section.Padding.Bottom);
        var pageContent = new Canvas();
        Canvas.SetLeft(viewport, section.Padding.Left);
        Canvas.SetTop(viewport, section.Padding.Top);
        viewport.Width = width;
        pageContent.Children.Add(viewport);
        AddPageRegion(pageContent, section.Header.Select(_pages.Count + 1, first), width, section.Padding.Left, 0, section.Padding.Top, "Header");
        AddPageRegion(pageContent, section.Footer.Select(_pages.Count + 1, first), width, section.Padding.Left, sheetHeight - section.Padding.Bottom, section.Padding.Bottom, "Footer");
        AddPageRegion(pageContent, notePage.Html, width, section.Padding.Left, sheetHeight - section.Padding.Bottom - notePage.Height, notePage.Height, "Footnotes");
        var sheet = new Border { Background = Brushes.White, Child = pageContent };
        _pages.Add(new(sheet, viewport, slice, sheetHeight, section.Size, section.Padding));
        Children.Add(sheet);
        HasOverflow |= slice.HasOverflow;
    }

    /// <summary>Adds a repeated rich region clipped to the reserved paper margin.</summary>
    /// <param name="canvas">The page content surface.</param>
    /// <param name="html">The region HTML.</param>
    /// <param name="width">The printable width.</param>
    /// <param name="left">The horizontal margin.</param>
    /// <param name="top">The region top.</param>
    /// <param name="height">The reserved margin height.</param>
    /// <param name="kind">The accessible story kind.</param>
    private void AddPageRegion(Canvas canvas, string html, double width, double left, double top, double height, string kind)
    {
        if (string.IsNullOrEmpty(html) || height <= 0)
        {
            return;
        }

        var document = new FlowDocument();
        document.SetText(html);
        if (html.Contains("data-page-field", StringComparison.Ordinal))
        {
            _pageFieldRegions.Add(new(document, html, _pages.Count + 1));
        }

        var presenter = new FormattedTextPresenter
        {
            Document = document,
            DefaultFontSize = DefaultFontSize,
            FontFamily = DefaultFontFamily,
            DefaultForeground = DefaultForeground,
            TextWrapping = TextWrapping.Wrap,
            ImageSourceLoader = LoadCachedImage,
            IsHitTestVisible = false,
        };
        var region = new Border { Width = width, Height = height, Child = presenter, ClipToBounds = true, IsHitTestVisible = false };
        Canvas.SetLeft(region, left);
        Canvas.SetTop(region, top);
        canvas.Children.Add(region);
        _automationStories.Add(new(region, presenter, kind, _pages.Count + 1));
    }
}
