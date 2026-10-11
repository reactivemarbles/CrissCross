// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies independent paragraph alignment against actual measured text geometry.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextParagraphLayoutTests
{
    /// <summary>The available fixture paragraph width.</summary>
    private const double ContentWidth = 240;

    /// <summary>The fixture page height.</summary>
    private const double PageHeight = 90;

    /// <summary>The fixture page margin.</summary>
    private const double PageMargin = 10;

    /// <summary>The fraction locating the middle of a caret.</summary>
    private const double Half = 0.5;

    /// <summary>The repetition count producing a paragraph longer than one page.</summary>
    private const int WordRepetitions = 20;

    /// <summary>Only the centered paragraph moves when neighboring paragraphs remain left aligned.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Layout_MixedParagraphAlignment_MovesOnlySelectedParagraph()
    {
        var left = CreatePresenter("<p>First</p><p>Middle</p><p>Last</p>");
        var mixed = CreatePresenter("<p>First</p><p style=\"text-align:center\">Middle</p><p>Last</p>");
        var middle = mixed.Document!.PlainText.IndexOf("Middle", StringComparison.Ordinal);
        var last = mixed.Document.PlainText.IndexOf("Last", StringComparison.Ordinal);
        await Assert.That(mixed.GetDocumentCaretRect(0).X).IsEqualTo(left.GetDocumentCaretRect(0).X);
        await Assert.That(mixed.GetDocumentCaretRect(middle).X > left.GetDocumentCaretRect(middle).X).IsTrue();
        await Assert.That(mixed.GetDocumentCaretRect(last).X).IsEqualTo(left.GetDocumentCaretRect(last).X);
        await Assert.That(mixed.TextAlignment).IsEqualTo(TextAlignment.Left);
    }

    /// <summary>Center and right alignment preserve exact document positions during pointer hit testing.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task HitTest_IndependentParagraphCarets_ReturnsOriginalOffsets()
    {
        var presenter = CreatePresenter("<p>Left</p><p style=\"text-align:center\">Center</p><p style=\"text-align:right\">Right</p>");
        var text = presenter.Document!.PlainText;
        foreach (var word in new[] { "Left", "Center", "Right" })
        {
            var offset = text.IndexOf(word, StringComparison.Ordinal);
            var caret = presenter.GetDocumentCaretRect(offset);
            await Assert.That(presenter.GetDocumentPosition(new(caret.X, caret.Y + (caret.Height * Half)))).IsEqualTo(offset);
        }

        var center = presenter.GetDocumentCaretRect(text.IndexOf("Center", StringComparison.Ordinal));
        var right = presenter.GetDocumentCaretRect(text.IndexOf("Right", StringComparison.Ordinal));
        await Assert.That(right.X > center.X).IsTrue();
    }

    /// <summary>Wrapped independently centered text partitions into regular sheets at measured lines.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Pagination_WrappedCenteredParagraph_SplitsAtMeasuredLines()
    {
        var words = string.Join(" ", Enumerable.Repeat("Centered paragraph wraps across paper", WordRepetitions));
        var document = new FlowDocument();
        document.SetText($"<p>Neighbor</p><p style=\"text-align:center\">{words}</p><p>Last</p>");
        var surface = new PagedDocumentPresenter { Document = document, PageSize = new(ContentWidth + PageMargin + PageMargin, PageHeight), PagePadding = new(PageMargin) };
        surface.Measure(new(surface.PageSize.Width, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        await Assert.That(surface.PageCount > 1).IsTrue();
        await Assert.That(surface.HasOverflow).IsFalse();
        var offset = document.PlainText.LastIndexOf("Centered", StringComparison.Ordinal);
        var caret = surface.GetDocumentCaretRect(offset);
        await Assert.That(caret.Y > surface.PageSize.Height).IsTrue();
        await Assert.That(surface.GetDocumentPosition(new(caret.X, caret.Y + (caret.Height * Half)))).IsEqualTo(offset);
    }

    /// <summary>Replacing an aligned document resets the root alignment and removes composed children.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task UpdateInlines_ReplacesAlignedDocument_ResetsLayoutState()
    {
        var presenter = CreatePresenter("<p style=\"text-align:right\">Right</p>");
        await Assert.That(presenter.TextAlignment).IsEqualTo(TextAlignment.Right);
        presenter.Document = new();
        presenter.Document.SetText("<p>Left</p><p style=\"text-align:center\">Center</p>");
        presenter.UpdateInlines();
        _ = presenter.MeasureDocument(ContentWidth);
        presenter.Arrange(new(presenter.DesiredSize));
        await Assert.That(presenter.TextAlignment).IsEqualTo(TextAlignment.Left);
        presenter.Document.SetText(string.Empty);
        presenter.UpdateInlines();
        await Assert.That(presenter.TextAlignment).IsEqualTo(TextAlignment.Left);
        await Assert.That(presenter.Inlines!.Count).IsEqualTo(0);
    }

    /// <summary>Independent paragraph children retain rich table cell caret geometry.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Layout_MixedAlignmentWithTable_PreservesCellOffsets()
    {
        var presenter = CreatePresenter("<p style=\"text-align:center\">Heading</p><table><tr><td>Cell</td></tr></table><p>After</p>");
        var offset = presenter.Document!.PlainText.IndexOf("Cell", StringComparison.Ordinal);
        var caret = presenter.GetDocumentCaretRect(offset);
        await Assert.That(presenter.GetDocumentPosition(new(caret.X, caret.Y + (caret.Height * Half)))).IsEqualTo(offset);
    }

    /// <summary>Creates and arranges a rich presenter at the fixture width.</summary>
    /// <param name="html">The fixture document.</param>
    /// <returns>The measured presenter.</returns>
    private static FormattedTextPresenter CreatePresenter(string html)
    {
        var document = new FlowDocument();
        document.SetText(html);
        var presenter = new FormattedTextPresenter { Document = document, TextWrapping = TextWrapping.Wrap };
        _ = presenter.MeasureDocument(ContentWidth);
        presenter.Arrange(new(presenter.DesiredSize));
        return presenter;
    }
}
