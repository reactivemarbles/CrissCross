// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;
using Avalonia.VisualTree;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests pagination against actual Avalonia text measurement and document offsets.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class PagedDocumentPresenterTests
{
    /// <summary>The narrow fixture page width.</summary>
    private const double PageWidth = 200;

    /// <summary>The content width inside the fixture margins.</summary>
    private const double ContentWidth = 180;

    /// <summary>The fixture margin on each edge.</summary>
    private const double PageMargin = 10;

    /// <summary>The combined top and bottom fixture margins.</summary>
    private const double VerticalMargins = 20;

    /// <summary>The large fixture font size.</summary>
    private const double LargeFontSize = 20;

    /// <summary>The height sufficient for one line but insufficient for two.</summary>
    private const double SingleLineScale = 1.5;

    /// <summary>The expected three-sheet count.</summary>
    private const int ThreeSheets = 3;

    /// <summary>The expected two-sheet count.</summary>
    private const int TwoSheets = 2;

    /// <summary>The expected single-sheet count.</summary>
    private const int OneSheet = 1;

    /// <summary>The offset where the fixture second line starts.</summary>
    private const int SecondLineOffset = 6;

    /// <summary>The fraction locating a caret's vertical midpoint.</summary>
    private const double Half = 0.5;

    /// <summary>The default paper width.</summary>
    private const double DefaultPageWidth = 794;

    /// <summary>The invalid fixture paper dimensions.</summary>
    private const double SmallPageSize = 100;

    /// <summary>The margin that consumes the fixture paper.</summary>
    private const double InvalidMargin = 60;

    /// <summary>The table fixture paper width.</summary>
    private const double TablePageWidth = 240;

    /// <summary>The table fixture paper height.</summary>
    private const double TablePageHeight = 30;

    /// <summary>The page height providing ample room for the explicit break fixture.</summary>
    private const double ExplicitBreakPageHeight = 1000;

    /// <summary>Uses measured lines rather than character count to place sheets.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Measure_ThreeTextLines_PlacesEachOnItsOwnSheet()
    {
        var document = new FlowDocument();
        document.SetText("First\nSecond\nThird");
        var reference = new FormattedTextPresenter { Document = document, FontSize = LargeFontSize, DefaultFontSize = LargeFontSize, TextWrapping = TextWrapping.Wrap };
        _ = reference.MeasureDocument(ContentWidth);
        var lineHeight = reference.TextLayout.TextLines[0].Height;
        var surface = new PagedDocumentPresenter
        {
            Document = document,
            DefaultFontSize = LargeFontSize,
            PageSize = new(PageWidth, (lineHeight * SingleLineScale) + VerticalMargins),
            PagePadding = new(PageMargin),
        };
        surface.Measure(new(PageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        await Assert.That(surface.PageCount).IsEqualTo(ThreeSheets);
        await Assert.That(surface.HasOverflow).IsFalse();
        await Assert.That(surface.DesiredSize.Height).IsEqualTo(Math.Ceiling((surface.PageSize.Height * ThreeSheets) + (surface.PageSpacing * TwoSheets)));
        await Assert.That(surface.GetVerticalDocumentPosition(SecondLineOffset, true)).IsEqualTo(0);
        await Assert.That(surface.GetVerticalDocumentPosition(0, false)).IsEqualTo(SecondLineOffset);
    }

    /// <summary>Maps the second sheet caret back to the rendered document offset.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task HitTest_SecondSheetCaret_ReturnsDocumentOffset()
    {
        var document = new FlowDocument();
        document.SetText("First\nSecond");
        var reference = new FormattedTextPresenter { Document = document, TextWrapping = TextWrapping.Wrap };
        _ = reference.MeasureDocument(ContentWidth);
        var surface = new PagedDocumentPresenter
        {
            Document = document,
            PageSize = new(PageWidth, (reference.TextLayout.TextLines[0].Height * SingleLineScale) + VerticalMargins),
            PagePadding = new(PageMargin),
        };
        surface.Measure(new(PageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        var caret = surface.GetDocumentCaretRect(SecondLineOffset);
        var hit = surface.GetDocumentPosition(new(caret.X, caret.Y + (caret.Height * Half)));
        await Assert.That(caret.Y > surface.PageSize.Height).IsTrue();
        await Assert.That(hit).IsEqualTo(SecondLineOffset);
    }

    /// <summary>Repaginates after edits and explicit page breaks.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Measure_ExplicitPageBreak_StartsNewSheetAndRefreshesAfterEdit()
    {
        var document = new FlowDocument();
        document.SetText("<p>First</p><br style=\"break-before:page\"><p>Second</p>");
        var surface = new PagedDocumentPresenter { Document = document };
        surface.Measure(new(DefaultPageWidth, double.PositiveInfinity));
        await Assert.That(surface.PageCount).IsEqualTo(TwoSheets);
        document.SetText("Single page");
        surface.Measure(new(DefaultPageWidth, double.PositiveInfinity));
        await Assert.That(surface.PageCount).IsEqualTo(OneSheet);
    }

    /// <summary>Creates a new sheet for CSS page-break-before and maps the caret to its document offset.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Measure_CssPageBreakBefore_StartsNewSheetAndMapsCaret()
    {
        var document = new FlowDocument();
        document.SetText("<p>First</p><h2 style='break-before:page'>Second</h2>");
        var surface = new PagedDocumentPresenter { Document = document, PageSize = new(DefaultPageWidth, ExplicitBreakPageHeight) };
        surface.Measure(new(DefaultPageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));

        var secondTextOffset = document.PlainText.IndexOf("Second", StringComparison.Ordinal);
        var caret = surface.GetDocumentCaretRect(secondTextOffset);
        var hit = surface.GetDocumentPosition(new(caret.X, caret.Y + (caret.Height * Half)));

        await Assert.That(surface.PageCount).IsEqualTo(TwoSheets);
        await Assert.That(caret.Y > surface.PageSize.Height).IsTrue();
        await Assert.That(hit).IsEqualTo(secondTextOffset);
    }

    /// <summary>Verifies sheet presenters share image decodes and rebuild after the image is removed.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task MultipleSheetsShareDecodedImageAndReleaseRemovedContent()
    {
        const string image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAAXNSR0IArs4c6QAAA"
            + "ARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAQSURBVBhXY/jPwPCfARkAAB7zAf+x9MCaAAAAAElFTkSuQmCC";
        var document = new FlowDocument();
        document.SetText($"<p><img src='{image}' /></p><h2 style='break-before:page'>Next sheet</h2>");
        var surface = new PagedDocumentPresenter { Document = document, PageSize = new(DefaultPageWidth, ExplicitBreakPageHeight) };
        surface.Measure(new(DefaultPageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        HashSet<IImage> sources = [with(ReferenceEqualityComparer.Instance)];
        var imageCount = 0;
        foreach (var visual in surface.GetVisualDescendants())
        {
            if (visual is not global::Avalonia.Controls.Image { Source: { } source })
            {
                continue;
            }

            imageCount++;
            _ = sources.Add(source);
        }

        await Assert.That(imageCount >= TwoSheets).IsTrue();
        await Assert.That(sources.Count).IsEqualTo(1);
        document.SetText("<p>Text only</p>");
        surface.Measure(new(DefaultPageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        await Assert.That(surface.PageCount).IsEqualTo(1);
    }

    /// <summary>Rejects page margins that consume the entire content area.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Measure_PaddingConsumesPage_Throws()
    {
        var surface = new PagedDocumentPresenter { PageSize = new(SmallPageSize, SmallPageSize), PagePadding = new(InvalidMargin) };
        await Assert.That(() => surface.Measure(new(SmallPageSize, SmallPageSize))).Throws<InvalidOperationException>();
    }

    /// <summary>Rejects sheet sizes that cannot yield a finite layout.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task PageSize_InfiniteWidth_Throws()
    {
        var surface = new PagedDocumentPresenter();
        await Assert.That(() => surface.PageSize = new(double.PositiveInfinity, SmallPageSize)).Throws<ArgumentException>();
    }

    /// <summary>Measures actual table geometry and preserves it when a nominal sheet is too short.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Measure_TableTallerThanSheet_ReportsExpandedSheet()
    {
        var document = new FlowDocument();
        document.SetText("<table><tr><td>First cell</td><td>Second cell</td></tr><tr><td>Third cell</td><td>Fourth cell</td></tr></table>");
        var surface = new PagedDocumentPresenter { Document = document, PageSize = new(TablePageWidth, TablePageHeight), PagePadding = new(PageMargin) };
        surface.Measure(new(TablePageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        await Assert.That(surface.HasOverflow).IsTrue();
        await Assert.That(surface.DesiredSize.Height > surface.PageSize.Height).IsTrue();
        var cellOffset = document.PlainText.IndexOf("Third", StringComparison.Ordinal);
        var caret = surface.GetDocumentCaretRect(cellOffset);
        await Assert.That(caret.Height > 0).IsTrue();
    }
}
