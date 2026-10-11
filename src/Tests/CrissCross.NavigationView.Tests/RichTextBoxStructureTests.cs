// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.VisualTree;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies editable annotations, section sheets, and repeated page regions.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxStructureTests
{
    /// <summary>The page width value used by these tests.</summary>
    private const double PageWidth = 400;

    /// <summary>The page height value used by these tests.</summary>
    private const double PageHeight = 500;

    /// <summary>The page padding value used by these tests.</summary>
    private const double PagePadding = 30;

    /// <summary>The available width value used by these tests.</summary>
    private const double AvailableWidth = 800;

    /// <summary>The first page index value used by these tests.</summary>
    private const int FirstPageIndex = 0;

    /// <summary>The first page count value used by these tests.</summary>
    private const int FirstPageCount = 1;

    /// <summary>The two pages value used by these tests.</summary>
    private const int TwoPages = 2;

    /// <summary>The header position value used by these tests.</summary>
    private const int HeaderPosition = 100;

    /// <summary>The world start value used by these tests.</summary>
    private const int WorldStart = 6;

    /// <summary>The world length value used by these tests.</summary>
    private const int WorldLength = 5;

    /// <summary>The old text length value used by these tests.</summary>
    private const int OldTextLength = 3;

    /// <summary>The review text length value used by these tests.</summary>
    private const int ReviewTextLength = 6;

    /// <summary>The body text length value used by these tests.</summary>
    private const int BodyTextLength = 4;

    /// <summary>The section page width value used by these tests.</summary>
    private const double SectionPageWidth = 600;

    /// <summary>The section page height value used by these tests.</summary>
    private const double SectionPageHeight = 300;

    /// <summary>The footnote page threshold value used by these tests.</summary>
    private const double FootnotePageThreshold = 400;

    /// <summary>The edited footnote text value used by these tests.</summary>
    private const string EditedFootnoteText = "Edited footnote";

    /// <summary>A bookmark navigates to its preserved formatted text.</summary>
    /// <returns>A task that completes after the bookmark assertions pass.</returns>
    [Test]
    public async Task Bookmark_NavigatesToFormattedRange()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<p>Hello <strong>world</strong></p>");
        editor.Select(WorldStart, WorldLength);
        editor.AddBookmark("chapter one");
        editor.Select(0, 0);
        await Assert.That(editor.NavigateToBookmark("chapter one")).IsTrue();
        await Assert.That(editor.SelectedText).IsEqualTo("world");
        await Assert.That(editor.Document.Text).Contains("<strong>");
        await Assert.That(editor.NavigateToBookmark("missing")).IsFalse();
    }

    /// <summary>Accept and reject preserve the intended side of a tracked edit.</summary>
    /// <param name="accept">Whether to accept the tracked edit.</param>
    /// <param name="expected">The expected text after resolving the revision.</param>
    /// <returns>A task that completes after the revision assertions pass.</returns>
    [Test]
    [Arguments(true, "new")]
    [Arguments(false, "old")]
    public async Task Revision_ResolvesInsertionAndDeletion(bool accept, string expected)
    {
        var editor = new RichTextBox();
        editor.SetHtml("old");
        editor.Select(FirstPageIndex, OldTextLength);
        editor.ReplaceSelectionWithRevision("r1", "Editor", "new");
        await Assert.That(editor.Document.PlainText).IsEqualTo("oldnew");
        if (accept)
        {
            editor.AcceptRevision("r1");
        }
        else
        {
            editor.RejectRevision("r1");
        }

        await Assert.That(editor.Document.PlainText).IsEqualTo(expected);
        editor.Undo();
        await Assert.That(editor.Document.PlainText).IsEqualTo("oldnew");
    }

    /// <summary>Comment removal retains all selected content.</summary>
    /// <returns>A task that completes after the comment assertions pass.</returns>
    [Test]
    public async Task Comment_RemovalPreservesTextAndHistory()
    {
        var editor = new RichTextBox();
        editor.SetHtml("Review me");
        editor.Select(FirstPageIndex, ReviewTextLength);
        editor.AddComment("c1", "Reviewer", "Check & confirm");
        await Assert.That(editor.Document.Text).Contains("title=\"Check &amp; confirm\"");
        editor.RemoveComment("c1");
        await Assert.That(editor.Document.PlainText).IsEqualTo("Review me");
        await Assert.That(editor.Document.Text.Contains("data-comment-id", StringComparison.Ordinal)).IsFalse();
        editor.Undo();
        await Assert.That(editor.Document.Text).Contains("data-comment-id");
    }

    /// <summary>Repeated region content does not enter editable body coordinates.</summary>
    /// <returns>A task that completes after the repeated region assertions pass.</returns>
    [Test]
    public async Task HeaderFooter_RepeatWithoutChangingBodyPositions()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<p>Body</p><br style=\"break-before:page\"><p>Next</p>");
        var body = editor.Document.PlainText;
        editor.SetPageRegion("<strong>Report header</strong>");
        editor.SetPageRegion("Footer", isFooter: true);
        await Assert.That(editor.Document.PlainText).IsEqualTo(body);
        var presenter = CreatePresenter(editor);
        presenter.Measure(new(AvailableWidth, double.PositiveInfinity));
        presenter.Arrange(new(0, 0, presenter.DesiredSize.Width, presenter.DesiredSize.Height));
        await Assert.That(presenter.PageCount).IsEqualTo(TwoPages);
        await Assert.That(presenter.GetDocumentPosition(new(HeaderPosition, FirstPageIndex))).IsEqualTo(FirstPageIndex);
        var headers = 0;
        var footers = 0;
        foreach (var visual in presenter.GetVisualDescendants())
        {
            if (visual is FormattedTextPresenter region)
            {
                headers += region.Document?.PlainText.Contains("Report header", StringComparison.Ordinal) == true ? 1 : 0;
                footers += region.Document?.PlainText.Contains("Footer", StringComparison.Ordinal) == true ? 1 : 0;
            }
        }

        await Assert.That(headers).IsEqualTo(TwoPages);
        await Assert.That(footers).IsEqualTo(TwoPages);
    }

    /// <summary>Sections produce distinct sheet widths and heights.</summary>
    /// <returns>A task that completes after the section geometry assertions pass.</returns>
    [Test]
    public async Task Sections_UseTheirPaperGeometry()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<section data-section-id=\"a\" data-page-width=\"400\" data-page-height=\"500\" data-page-padding=\"20,20,20,20\"><p>First</p></section>"
            + "<section data-section-id=\"b\" data-page-width=\"600\" data-page-height=\"300\" data-page-padding=\"30,30,30,30\"><p>Second</p></section>");
        var presenter = CreatePresenter(editor);
        presenter.Measure(new(AvailableWidth, double.PositiveInfinity));
        presenter.Arrange(new(0, 0, presenter.DesiredSize.Width, presenter.DesiredSize.Height));
        var bounds = presenter.CapturePageBounds();
        await Assert.That(bounds.Count).IsEqualTo(TwoPages);
        await Assert.That(bounds[FirstPageIndex].Width).IsEqualTo(PageWidth);
        await Assert.That(bounds[FirstPageIndex].Height).IsEqualTo(PageHeight);
        await Assert.That(bounds[1].Width).IsEqualTo(SectionPageWidth);
        await Assert.That(bounds[1].Height).IsEqualTo(SectionPageHeight);
        var second = editor.Document.PlainText.IndexOf("Second", StringComparison.Ordinal);
        await Assert.That(presenter.GetDocumentCaretRect(second).Y).IsGreaterThan(bounds[1].Y);
    }

    /// <summary>Footnotes occupy their reference sheet without becoming body text.</summary>
    /// <returns>A task that completes after the footnote assertions pass.</returns>
    [Test]
    public async Task Footnote_RendersAtPageBottomAndCanBeEdited()
    {
        var editor = new RichTextBox();
        editor.SetHtml("Body");
        editor.Select(BodyTextLength, FirstPageIndex);
        editor.InsertNote("1", "Original footnote");
        await Assert.That(editor.Document.PlainText).IsEqualTo("Body[1]");
        editor.SetNoteText("1", EditedFootnoteText);
        await Assert.That(editor.Document.Text).Contains(EditedFootnoteText);
        await Assert.That(editor.Document.Text).DoesNotContain("Original footnote");
        await Assert.That(editor.Document.PlainText).IsEqualTo("Body[1]");
        var presenter = CreatePresenter(editor);
        presenter.Measure(new(AvailableWidth, double.PositiveInfinity));
        presenter.Arrange(new(0, 0, presenter.DesiredSize.Width, presenter.DesiredSize.Height));
        await Assert.That(presenter.PageCount).IsEqualTo(FirstPageCount);
        await Assert.That(presenter.HasOverflow).IsFalse();
        var found = false;
        foreach (var visual in presenter.GetVisualDescendants())
        {
            if (visual is FormattedTextPresenter region && region.Document?.PlainText.Contains(EditedFootnoteText, StringComparison.Ordinal) == true)
            {
                found = true;
                await Assert.That(region.TranslatePoint(default, presenter)?.Y ?? FirstPageIndex).IsGreaterThan(FootnotePageThreshold);
            }
        }

        await Assert.That(found).IsTrue();
    }

    /// <summary>Read-only documents ignore structure mutations.</summary>
    /// <returns>A task that completes after the read-only assertions pass.</returns>
    [Test]
    public async Task Structures_RespectReadOnly()
    {
        var editor = new RichTextBox();
        editor.SetHtml("Text");
        editor.IsReadOnly = true;
        editor.Select(FirstPageIndex, BodyTextLength);
        editor.AddComment("c1", "Author", "Text");
        editor.InsertNote("1", "Note");
        editor.SetPageRegion("Header");
        await Assert.That(editor.Document.Text).IsEqualTo("Text");
    }

    /// <summary>Creates a sheet presenter using the edited document.</summary>
    /// <param name="editor">The document owner.</param>
    /// <returns>The presenter.</returns>
    private static PagedDocumentPresenter CreatePresenter(RichTextBox editor) => new() { Document = editor.Document, PageSize = new(PageWidth, PageHeight), PagePadding = new(PagePadding) };
}
