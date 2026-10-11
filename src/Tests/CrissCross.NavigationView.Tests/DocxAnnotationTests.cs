// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies editable Word annotation stories against the independent Office schema.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class DocxAnnotationTests
{
    /// <summary>Editor paper wrapping preserves note definitions stored in the document head.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Export_DefaultPaper_PreservesHeadNoteDefinition()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<html><head><template id='note-head' data-note-kind='footnote'><p>Head note</p></template></head>"
            + "<body><p>Body<sup><a href='#note-head'>[head]</a></sup></p></body></html>");
        await using var exported = new MemoryStream();
        editor.Save(exported, RichTextDataFormat.Docx);
        exported.Position = 0;
        using var package = WordprocessingDocument.Open(exported, false);
        await Assert.That(package.MainDocumentPart?.FootnotesPart?.Footnotes?.InnerText).Contains("Head note");
        await Assert.That(ValidationErrors(package)).IsEmpty();
    }

    /// <summary>Notes retain editable definitions and export their changed text into native Word note parts.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Notes_ImportEditExport_UpdatesNativeStories()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<p>Body<sup><a href='#note-foot'>[foot]</a></sup><sup><a href='#note-end'>[end]</a></sup></p>"
            + "<template id='note-foot' data-note-kind='footnote'><p>Foot original</p></template>"
            + "<aside id='note-end' data-note-kind='endnote'><p>End original</p></aside>");
        await using var exported = new MemoryStream();
        editor.Save(exported, RichTextDataFormat.Docx);
        exported.Position = 0;
        var imported = new RichTextBox();
        imported.Load(exported, RichTextDataFormat.Docx);
        imported.SetNoteText("footnote-1", "Edited native footnote");
        imported.SetNoteText("endnote-2", "Edited native endnote");
        await using var edited = new MemoryStream();
        imported.Save(edited, RichTextDataFormat.Docx);
        edited.Position = 0;
        using var package = WordprocessingDocument.Open(edited, false);
        await Assert.That(package.MainDocumentPart?.FootnotesPart?.Footnotes?.InnerText).Contains("Edited native footnote");
        await Assert.That(package.MainDocumentPart?.EndnotesPart?.Endnotes?.InnerText).Contains("Edited native endnote");
        await Assert.That(ValidationErrors(package)).IsEmpty();
    }

    /// <summary>Bookmarks, comments, and revisions become native Word structures after editing.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Annotations_RoundTrip_PreservesNativeRangesAndRevisionMetadata()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<p><span data-bookmark='target'>Bookmarked</span> "
            + "<mark data-comment-id='review' data-author='Reviewer' title='Check wording'>Commented</mark> "
            + "<del data-revision-id='1' data-author='Editor'>before</del><ins data-revision-id='2' data-author='Editor'>after</ins></p>");
        await using var exported = new MemoryStream();
        editor.Save(exported, RichTextDataFormat.Docx);
        exported.Position = 0;
        var imported = new RichTextBox();
        imported.Load(exported, RichTextDataFormat.Docx);
        await Assert.That(imported.Html).Contains("data-bookmark=\"target\"");
        await Assert.That(imported.Html).Contains("Check wording");
        imported.AcceptRevision("2");
        await using var edited = new MemoryStream();
        imported.Save(edited, RichTextDataFormat.Docx);
        edited.Position = 0;
        using var package = WordprocessingDocument.Open(edited, false);
        await Assert.That(package.MainDocumentPart?.WordprocessingCommentsPart?.Comments?.InnerText).Contains("Check wording");
        await Assert.That(package.MainDocumentPart?.Document?.OuterXml).Contains("bookmarkStart");
        await Assert.That(package.MainDocumentPart?.Document?.OuterXml).Contains("delText");
        await Assert.That(ValidationErrors(package)).IsEmpty();
    }

    /// <summary>Native Word settings enable first and even page stories containing dynamic page fields.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PageRegions_FirstEvenAndDynamicFields_EnableWordSettings()
    {
        var fixture = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Fixtures", "WordNativeSample.docx"));
        var html = DocxDocumentConverter.ToHtml(fixture)
            + "<template data-page-region='header' data-section-id='docx-section-1' data-page-variant='first'><p>First page</p></template>"
            + "<template data-page-region='footer' data-section-id='docx-section-1' data-page-variant='even'>"
            + "<p>Page <span data-page-field='PAGE'>1</span> of <span data-page-field='NUMPAGES'>3</span></p></template>";
        var bytes = DocxDocumentConverter.ToDocx(html);
        await using var stream = new MemoryStream(bytes);
        using var document = WordprocessingDocument.Open(stream, false);
        await Assert.That(document.MainDocumentPart?.Document?.OuterXml).Contains("titlePg");
        await Assert.That(document.MainDocumentPart?.DocumentSettingsPart?.Settings?.OuterXml).Contains("evenAndOddHeaders");
        await Assert.That(DocxDocumentConverter.ToHtml(bytes)).Contains("data-page-field=\"PAGE\"");
        await Assert.That(DocxDocumentConverter.ToHtml(bytes)).Contains("data-page-variant=\"first\"");
        await Assert.That(ValidationErrors(document)).IsEmpty();
    }

    /// <summary>Friendly revision names receive numeric Word IDs and remain resolvable after file import.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Revisions_FriendlyIdentifiers_RoundTripAndAcceptWithValidWordIds()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<p><del data-revision-id='change-α' data-author='Editor'>before</del>"
            + "<ins data-revision-id='change-α' data-author='Editor'>after</ins>"
            + "<ins data-revision-id='change-α' data-author='Editor'><strong>formatted</strong></ins></p>");
        await using var exported = new MemoryStream();
        editor.Save(exported, RichTextDataFormat.Docx);
        exported.Position = 0;
        using (var package = WordprocessingDocument.Open(exported, false))
        {
            await Assert.That(ValidationErrors(package)).IsEmpty();
        }

        exported.Position = 0;
        var imported = new RichTextBox();
        imported.Load(exported, RichTextDataFormat.Docx);
        var markup = new HtmlParser().ParseDocument(imported.Html);
        await Assert.That(markup.QuerySelector("ins")?.GetAttribute("data-revision-id")).IsEqualTo("change-α");
        imported.AcceptRevision("change-α");
        await Assert.That(imported.PlainText).IsEqualTo($"afterformatted{Environment.NewLine}");
        var acceptedMarkup = new HtmlParser().ParseDocument(imported.Html);
        await Assert.That(acceptedMarkup.QuerySelectorAll("p").Length).IsEqualTo(1);
        await Assert.That(acceptedMarkup.QuerySelectorAll("ins,del")).IsEmpty();
        await using var accepted = new MemoryStream();
        imported.Save(accepted, RichTextDataFormat.Docx);
        accepted.Position = 0;
        using var acceptedPackage = WordprocessingDocument.Open(accepted, false);
        var body = acceptedPackage.MainDocumentPart?.Document?.Body ?? throw new InvalidDataException("The accepted document has no body.");
        var paragraphCount = 0;
        foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
        {
            paragraphCount++;
        }

        var hasFormattedBoldRun = false;
        foreach (var run in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>())
        {
            if (run.InnerText == "formatted" && run.RunProperties?.Bold is not null)
            {
                hasFormattedBoldRun = true;
                break;
            }
        }

        await Assert.That(paragraphCount).IsEqualTo(1);
        await Assert.That(body.InnerText).IsEqualTo("afterformatted");
        await Assert.That(hasFormattedBoldRun).IsTrue();
        await Assert.That(ValidationErrors(acceptedPackage)).IsEmpty();
    }

    /// <summary>Collects schema failures from the Office validator.</summary>
    /// <param name="document">The package.</param>
    /// <returns>The validation failures.</returns>
    private static List<string> ValidationErrors(WordprocessingDocument document)
    {
        var errors = new List<string>();
        foreach (var error in new OpenXmlValidator(FileFormatVersions.Microsoft365).Validate(document))
        {
            errors.Add($"{error.Part?.Uri} {error.Path?.XPath}: {error.Description}");
        }

        return errors;
    }
}
