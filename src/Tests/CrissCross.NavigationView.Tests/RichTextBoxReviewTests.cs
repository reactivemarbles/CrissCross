// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Html.Parser;
using Avalonia;
using Avalonia.VisualTree;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies page region variants and automatic revision tracking.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxReviewTests
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

    /// <summary>The insertion position value used by these tests.</summary>
    private const int InsertionPosition = 2;

    /// <summary>The replacement length value used by these tests.</summary>
    private const int ReplacementLength = 3;

    /// <summary>The deletion start value used by these tests.</summary>
    private const int DeletionStart = 5;

    /// <summary>The deletion length value used by these tests.</summary>
    private const int DeletionLength = 6;

    /// <summary>The three pages value used by these tests.</summary>
    private const int ThreePages = 3;

    /// <summary>A collapsed bookmark preserves and restores an insertion position.</summary>
    /// <returns>A task that completes after the bookmark assertions pass.</returns>
    [Test]
    public async Task Bookmark_StoresAnInsertionPosition()
    {
        var editor = new RichTextBox();
        editor.SetHtml("Text");
        editor.Select(InsertionPosition, FirstPageIndex);
        editor.AddBookmark("position");
        editor.Select(0, 0);
        await Assert.That(editor.NavigateToBookmark("position")).IsTrue();
        await Assert.That(editor.SelectionStart).IsEqualTo(InsertionPosition);
        await Assert.That(editor.SelectedText).IsEqualTo(string.Empty);
        await Assert.That(editor.Document.PlainText).IsEqualTo("Text");
    }

    /// <summary>Automatic tracking retains both edit versions and supports rejection.</summary>
    /// <returns>A task that completes after the revision assertions pass.</returns>
    [Test]
    public async Task TrackChanges_ReplacementCanBeRejectedAndUndone()
    {
        var editor = new RichTextBox { TrackChanges = true, RevisionAuthor = "Reviewer" };
        editor.SetHtml("<strong>old</strong>");
        editor.Select(FirstPageIndex, ReplacementLength);
        editor.ReplaceSelection("new");
        await Assert.That(editor.Document.PlainText).IsEqualTo("oldnew");
        var revision = new HtmlParser().ParseDocument(editor.Document.Text).QuerySelector("ins[data-revision-id]");
        await Assert.That(revision?.GetAttribute("data-author")).IsEqualTo("Reviewer");
        var id = revision?.GetAttribute("data-revision-id") ?? string.Empty;
        editor.RejectRevision(id);
        await Assert.That(editor.Document.PlainText).IsEqualTo("old");
        await Assert.That(editor.Document.Segments[0].IsBold).IsTrue();
        editor.Undo();
        await Assert.That(editor.Document.PlainText).IsEqualTo("oldnew");
        editor.Undo();
        await Assert.That(editor.Document.PlainText).IsEqualTo("old");
    }

    /// <summary>Tracked deletion can be accepted without leaving deleted content.</summary>
    /// <returns>A task that completes after the deletion assertions pass.</returns>
    [Test]
    public async Task TrackChanges_DeletionCanBeAccepted()
    {
        var editor = new RichTextBox { TrackChanges = true };
        editor.SetHtml("Keep remove");
        editor.Select(DeletionStart, DeletionLength);
        editor.ReplaceSelection(string.Empty);
        await Assert.That(editor.Document.PlainText).IsEqualTo("Keep remove");
        var revision = new HtmlParser().ParseDocument(editor.Document.Text).QuerySelector("del[data-revision-id]");
        editor.AcceptRevision(revision?.GetAttribute("data-revision-id") ?? string.Empty);
        await Assert.That(editor.Document.PlainText).IsEqualTo("Keep ");
    }

    /// <summary>First and parity variants render actual physical page and total fields.</summary>
    /// <returns>A task that completes after the page region assertions pass.</returns>
    [Test]
    public async Task PageRegions_SelectVariantsAndResolvePageFields()
    {
        var editor = new RichTextBox();
        editor.SetHtml("One<br style=\"break-before:page\">Two<br style=\"break-before:page\">Three");
        var body = editor.Document.PlainText;
        const string fields = "<span data-page-field=\"PAGE\">?</span>/<span data-page-field=\"NUMPAGES\">?</span>";
        editor.SetPageRegion($"First{fields}", isFooter: false, sectionId: null, variant: "first");
        editor.SetPageRegion($"Even{fields}", isFooter: false, sectionId: null, variant: "even");
        editor.SetPageRegion($"Odd{fields}", isFooter: false, sectionId: null, variant: "odd");
        var presenter = new PagedDocumentPresenter { Document = editor.Document, PageSize = new(PageWidth, PageHeight), PagePadding = new(PagePadding) };
        presenter.Measure(new(AvailableWidth, double.PositiveInfinity));
        presenter.Arrange(new(0, 0, presenter.DesiredSize.Width, presenter.DesiredSize.Height));
        var regions = new List<string>();
        foreach (var visual in presenter.GetVisualDescendants())
        {
            if (visual is FormattedTextPresenter region && region.Document is { } document && !ReferenceEquals(document, editor.Document))
            {
                regions.Add(document.PlainText);
            }
        }

        await Assert.That(presenter.PageCount).IsEqualTo(ThreePages);
        await Assert.That(regions).Contains("First1/3");
        await Assert.That(regions).Contains("Even2/3");
        await Assert.That(regions).Contains("Odd3/3");
        await Assert.That(editor.Document.PlainText).IsEqualTo(body);
    }
}
