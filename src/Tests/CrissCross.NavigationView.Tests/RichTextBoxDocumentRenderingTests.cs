// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;
using Grid = CrissCross.Avalonia.UI.Controls.Grid;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests rich HTML metadata, table layout and measured document positions.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxDocumentRenderingTests
{
    /// <summary>Verifies loading unaligned content resets the previous paragraph alignment.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task LoadingUnalignedContentResetsPreviousAlignment()
    {
        var document = new FlowDocument();
        document.SetText("<p style='text-align:right'>Right</p>");
        var presenter = new FormattedTextPresenter { Document = document };
        presenter.UpdateInlines();
        await Assert.That(presenter.TextAlignment).IsEqualTo(TextAlignment.Right);
        document.SetText("<p>Left</p>");
        presenter.UpdateInlines();
        await Assert.That(presenter.TextAlignment).IsEqualTo(TextAlignment.Left);
    }

    /// <summary>Point units and relative fonts retain inherited formatting.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task WordStyles_ConvertPointUnitsAndInheritFormatting()
    {
        const double parentFontSize = 16D;
        const double childFontSize = 24D;
        const int redChannel = 12;
        const int greenChannel = 34;
        const int blueChannel = 56;

        var document = new FlowDocument();
        document.SetText(
            "<span style=\"font-family:'Cambria';font-size:12pt;color:rgb(12,34,56);font-weight:700\">outer"
            + "<span style=\"font-size:150%;font-weight:normal;font-style:italic\">inner</span></span>");
        var outer = FindSegment(document, static segment => segment.Text == "outer");
        var inner = FindSegment(document, static segment => segment.Text == "inner");
        await Assert.That(outer.FontSize).IsEqualTo(parentFontSize);
        await Assert.That(inner.FontSize).IsEqualTo(childFontSize);
        await Assert.That(outer.IsBold).IsTrue();
        await Assert.That(inner.IsBold).IsFalse();
        await Assert.That(inner.IsItalic).IsTrue();
        await Assert.That(inner.FontFamily?.Name).IsEqualTo("Cambria");
        await Assert.That((inner.Foreground as SolidColorBrush)?.Color).IsEqualTo(Color.FromRgb(redChannel, greenChannel, blueChannel));
    }

    /// <summary>Word class styles are applied before inline declarations.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task WordStyles_ApplyDocumentClassesAndInlineOverrides()
    {
        const double inlineFontSize = 20D;

        var document = new FlowDocument();
        document.SetText(
            "<html><head><style>p.WordText {font-family:Calibri;font-size:11pt;color:#123456} .emphasis {font-weight:bold}</style></head>"
            + "<body><p class=\"WordText emphasis\" style=\"font-size:15pt\">Word</p></body></html>");
        var segment = FindSegment(document, static segment => segment.Text == "Word");
        await Assert.That(segment.FontFamily?.Name).IsEqualTo("Calibri");
        await Assert.That(segment.FontSize).IsEqualTo(inlineFontSize);
        await Assert.That(segment.IsBold).IsTrue();
        await Assert.That((segment.Foreground as SolidColorBrush)?.Color).IsEqualTo(Color.Parse("#123456"));
    }

    /// <summary>Nested hyperlinks preserve their destination and target in cloned runs.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task Hyperlink_PreservesAddressAndTargetAcrossNestedRuns()
    {
        var document = new FlowDocument();
        document.SetText("<a href=\"https://example.com/path?q=1&amp;x=2\" target=\"_blank\"><b>linked</b></a>");
        var segment = document.Segments[0];
        var clone = segment.Clone();
        await Assert.That(clone.HyperlinkNavigationUri).IsEqualTo("https://example.com/path?q=1&x=2");
        await Assert.That(clone.HyperlinkTarget).IsEqualTo("_blank");
        await Assert.That(clone.IsUnderline).IsTrue();
        await Assert.That(clone.IsBold).IsTrue();
    }

    /// <summary>Spanning cells reserve occupied columns in following rows.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task Table_RendersSpanningCellsAsGridAndPreservesTextOffsets()
    {
        const int spanSize = 2;
        const int columnCount = 3;
        const int cellCount = 4;
        const int documentWidth = 300;
        const int minimumTableHeight = 30;

        var document = new FlowDocument();
        document.SetText("<table><tr><td rowspan=\"2\">A</td><td colspan=\"2\"><b>B</b></td></tr><tr><td>C</td><td>D</td></tr></table>tail");
        var segment = FindSegment(document, static segment => segment.Table is not null);
        var table = segment.Table!;
        await Assert.That(table.RowCount).IsEqualTo(spanSize);
        await Assert.That(table.ColumnCount).IsEqualTo(columnCount);
        await Assert.That(table.Cells[1].ColumnSpan).IsEqualTo(spanSize);
        await Assert.That(table.Cells[spanSize].Column).IsEqualTo(1);
        await Assert.That(table.Cells[spanSize].Row).IsEqualTo(1);
        await Assert.That(table.Cells[1].Segments[0].IsBold).IsTrue();
        var presenter = new FormattedTextPresenter { Document = document, TextWrapping = TextWrapping.Wrap };
        _ = presenter.MeasureDocument(documentWidth);
        presenter.Arrange(new(presenter.DesiredSize));
        var inline = FindInlineContainer(presenter);
        var grid = await Assert.That(inline.Child).IsTypeOf<Grid>() ?? throw new InvalidOperationException("The table grid was not created.");
        await Assert.That(grid.Children.Count).IsEqualTo(cellCount);
        await Assert.That(Grid.GetRowSpan(grid.Children[0])).IsEqualTo(spanSize);
        await Assert.That(Grid.GetColumnSpan(grid.Children[1])).IsEqualTo(spanSize);
        await Assert.That(presenter.GetDocumentOffset(1)).IsEqualTo(segment.Length);
        await Assert.That(presenter.GetLayoutOffset(segment.Length)).IsEqualTo(1);
        await Assert.That(presenter.DesiredSize.Height > minimumTableHeight).IsTrue();
    }

    /// <summary>Inline objects contribute their measured height without inserting an extra line.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task InlineImage_ReservesActualSizeAndStableDocumentPosition()
    {
        const int documentWidth = 400;
        const int imageHeight = 120;
        const int afterImageOffset = 7;
        const int imageOffset = 6;

        var document = new FlowDocument();
        document.SetText("before<img src=\"missing-image.png\" width=\"96\" height=\"120\" />after");
        var presenter = new FormattedTextPresenter { Document = document, TextWrapping = TextWrapping.Wrap };
        _ = presenter.MeasureDocument(documentWidth);
        presenter.Arrange(new(presenter.DesiredSize));
        await Assert.That(CountInlineContainers(presenter)).IsEqualTo(1);
        await Assert.That(presenter.DesiredSize.Height >= imageHeight).IsTrue();
        await Assert.That(presenter.GetDocumentOffset(afterImageOffset)).IsEqualTo(afterImageOffset);
        await Assert.That(presenter.GetLayoutOffset(afterImageOffset)).IsEqualTo(afterImageOffset);
        await Assert.That(FindSegment(document, static segment => segment.IsImage).StartIndex).IsEqualTo(imageOffset);
    }

    /// <summary>Paragraph and page break positions agree with the document projection.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task ParagraphAndPageBreaks_KeepFollowingTextOffsets()
    {
        const int firstWordLength = 5;
        const int documentWidth = 300;

        var document = new FlowDocument();
        document.SetText("<p>first</p><br style=\"break-before:page\" />last");
        var last = FindSegment(document, static segment => segment.Text == "last");
        await Assert.That(last.StartIndex).IsEqualTo(firstWordLength + Environment.NewLine.Length + 1);
        await Assert.That(FindSegment(document, static segment => segment.IsPageBreak).Text).IsEqualTo("\f");
        var presenter = new FormattedTextPresenter { Document = document };
        _ = presenter.MeasureDocument(documentWidth);
        await Assert.That(presenter.GetDocumentOffset(presenter.GetLayoutOffset(last.StartIndex))).IsEqualTo(last.StartIndex);
    }

    /// <summary>Generated list markers never consume editable document positions.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task OrderedList_MapsGeneratedMarkersOutsideDocumentPositions()
    {
        const int documentWidth = 300;
        const int markerLength = 3;
        const int insideMarkerOffset = 2;

        var document = new FlowDocument();
        document.SetText("<ol start=\"3\"><li>alpha</li><li>beta</li></ol>");
        var markers = FindListMarkers(document);
        await Assert.That(markers[0].ListMarker).IsEqualTo("3. ");
        await Assert.That(markers[1].ListMarker).IsEqualTo("4. ");
        await Assert.That(markers[0].Length).IsEqualTo(0);
        await Assert.That(document.PlainText).IsEqualTo($"alpha{Environment.NewLine}beta{Environment.NewLine}");
        var presenter = new FormattedTextPresenter { Document = document };
        _ = presenter.MeasureDocument(documentWidth);
        await Assert.That(presenter.GetLayoutOffset(0)).IsEqualTo(markerLength);
        await Assert.That(presenter.GetDocumentOffset(insideMarkerOffset)).IsEqualTo(0);
        await Assert.That(presenter.GetDocumentOffset(markerLength)).IsEqualTo(0);
    }

    /// <summary>Table hit testing and selection use the rich cell's measured text geometry.</summary>
    /// <returns>The asynchronous assertion result.</returns>
    [Test]
    public async Task TableCellGeometry_MapsCaretHitAndSelectionToDocumentPositions()
    {
        const int documentWidth = 300;
        const double insideGlyph = 0.1;
        const double verticalCenterDivisor = 2;
        var document = new FlowDocument();
        document.SetText("<table><tr><td>alpha</td><td>beta</td></tr></table>");
        var presenter = new FormattedTextPresenter { Document = document, TextWrapping = TextWrapping.Wrap };
        _ = presenter.MeasureDocument(documentWidth);
        presenter.Arrange(new(presenter.DesiredSize));
        var inline = FindInlineContainer(presenter);
        var grid = await Assert.That(inline.Child).IsTypeOf<Grid>() ?? throw new InvalidOperationException("The table grid was not created.");
        var border = grid.Children[1] as global::Avalonia.Controls.Border ?? throw new InvalidOperationException("The cell border was not created.");
        var cellPresenter = border.Child as FormattedTextPresenter ?? throw new InvalidOperationException("The rich cell was not created.");
        var cell = FindSegment(document, static segment => segment.Table is not null).Table!.Cells[1];
        var caret = cellPresenter.GetDocumentCaretRect(1);
        var point = cellPresenter.TranslatePoint(new(caret.X + insideGlyph, caret.Y + (caret.Height / verticalCenterDivisor)), presenter)
            ?? throw new InvalidOperationException("The table geometry was not arranged.");
        var documentOffset = cell.StartIndex + 1;
        await Assert.That(presenter.GetDocumentPosition(point)).IsEqualTo(documentOffset);
        var documentCaret = presenter.GetDocumentCaretRect(documentOffset);
        var translatedCaret = cellPresenter.TranslatePoint(caret.Position, presenter)
            ?? throw new InvalidOperationException("The table caret was not arranged.");
        await Assert.That(documentCaret.X).IsEqualTo(translatedCaret.X);
        var rectangles = presenter.GetDocumentRangeRectangles(documentOffset, 1);
        await Assert.That(rectangles.Count > 0).IsTrue();
        await Assert.That(rectangles[0].X).IsEqualTo(documentCaret.X);
    }

    /// <summary>Finds the fixture segment without allocating an enumeration pipeline.</summary>
    /// <param name="document">The document fixture.</param>
    /// <param name="predicate">The matching condition.</param>
    /// <returns>The matching segment.</returns>
    private static TextSegment FindSegment(FlowDocument document, Func<TextSegment, bool> predicate)
    {
        foreach (var segment in document.Segments)
        {
            if (predicate(segment))
            {
                return segment;
            }
        }

        throw new InvalidOperationException("The fixture segment was not parsed.");
    }

    /// <summary>Finds the inline object used by a fixture.</summary>
    /// <param name="presenter">The fixture presenter.</param>
    /// <returns>The inline container.</returns>
    private static InlineUIContainer FindInlineContainer(FormattedTextPresenter presenter)
    {
        foreach (var inline in presenter.Inlines!)
        {
            if (inline is InlineUIContainer container)
            {
                return container;
            }
        }

        throw new InvalidOperationException("The inline object was not rendered.");
    }

    /// <summary>Counts inline controls in a fixture.</summary>
    /// <param name="presenter">The fixture presenter.</param>
    /// <returns>The number of inline controls.</returns>
    private static int CountInlineContainers(FormattedTextPresenter presenter)
    {
        var count = 0;
        foreach (var inline in presenter.Inlines!)
        {
            if (inline is InlineUIContainer)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Collects generated markers from a list fixture.</summary>
    /// <param name="document">The document fixture.</param>
    /// <returns>The generated marker segments.</returns>
    private static List<TextSegment> FindListMarkers(FlowDocument document)
    {
        var markers = new List<TextSegment>();
        foreach (var segment in document.Segments)
        {
            if (segment.ListMarker is not null)
            {
                markers.Add(segment);
            }
        }

        return markers;
    }
}
