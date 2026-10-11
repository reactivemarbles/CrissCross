// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using CrissCross.Avalonia.UI.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace CrissCross.NavigationView.Tests;

/// <summary>Validates Word packages independently of the editor's own reader.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class DocxSchemaTests
{
    /// <summary>The portable image used in the schema fixture.</summary>
    private const string ImageSource = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jYuoAAAAASUVORK5CYII=";

    /// <summary>The native package fixture copied from Microsoft Word.</summary>
    private const string NativeFixtureName = "WordNativeSample.docx";

    /// <summary>The custom paper width in device independent pixels.</summary>
    private const int PaperWidth = 640;

    /// <summary>The custom paper height in device independent pixels.</summary>
    private const int PaperHeight = 880;

    /// <summary>The uniform paper margin in device independent pixels.</summary>
    private const int PaperMargin = 32;

    /// <summary>The number of Word twips in a device independent pixel.</summary>
    private const int TwipsPerPixel = 15;

    /// <summary>Editor paper settings become native Word section settings.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Export_PreservesEditorPaperGeometry()
    {
        var editor = new RichTextBox { PageSize = new(PaperWidth, PaperHeight), PagePadding = new(PaperMargin) };
        editor.SetHtml("<html><body style='font-family:Consolas'><p>Custom paper</p></body></html>");
        await using var stream = new MemoryStream();
        editor.Save(stream, RichTextDataFormat.Docx);
        stream.Position = 0;
        using var document = WordprocessingDocument.Open(stream, false);
        var body = Required(document.MainDocumentPart?.Document?.Body);
        var section = Required(body.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.SectionProperties>());
        var size = Required(section.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.PageSize>());
        var margins = Required(section.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.PageMargin>());
        var font = FindFont(body);
        await Assert.That(Required(size.Width).Value).IsEqualTo((uint)(PaperWidth * TwipsPerPixel));
        await Assert.That(Required(size.Height).Value).IsEqualTo((uint)(PaperHeight * TwipsPerPixel));
        await Assert.That(Required(margins.Left).Value).IsEqualTo((uint)(PaperMargin * TwipsPerPixel));
        await Assert.That(Required(font.Ascii).Value).IsEqualTo("Consolas");
        await Assert.That(ValidationErrors(document)).IsEmpty();
    }

    /// <summary>Fresh packages containing rich elements satisfy Office schema constraints.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Export_RichElementsProduceValidWordprocessingPackage()
    {
        var editor = new RichTextBox();
        editor.SetHtml($"<p><strong>日本語</strong> <a href='https://avaloniaui.net'>Link</a><img src='{ImageSource}' width='16' height='16'></p>"
            + "<ol start='3'><li>One</li><li>Two</li></ol><table><tr><td colspan='2'>Merged</td></tr><tr><td>A</td><td>B</td></tr></table>"
            + "<p style='break-before:page;font-family:Consolas;color:#FF0000'>Page two</p>");
        await using var stream = new MemoryStream();
        editor.Save(stream, RichTextDataFormat.Docx);
        stream.Position = 0;
        using var document = WordprocessingDocument.Open(stream, false);
        await Assert.That(ValidationErrors(document)).IsEmpty();
        await Assert.That(document.MainDocumentPart?.Document?.Body?.InnerText).Contains("日本語");
        await Assert.That(stream.CanRead).IsTrue();
    }

    /// <summary>A real Word package remains valid after importing and editing its text.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task NativeWordFixture_EditAndExportRemainsSchemaValid()
    {
        var editor = new RichTextBox();
        await using (var input = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Fixtures", NativeFixtureName)))
        {
            editor.Load(input, RichTextDataFormat.Docx);
        }

        editor.Select(0, "CrissCross".Length);
        editor.ReplaceSelection("CrissCross edited");
        await using var output = new MemoryStream();
        editor.Save(output, RichTextDataFormat.Docx);
        output.Position = 0;
        using var package = WordprocessingDocument.Open(output, false);
        await Assert.That(ValidationErrors(package)).IsEmpty();
        await Assert.That(package.MainDocumentPart?.Document?.Body?.InnerText).Contains("CrissCross edited");
        await Assert.That(package.MainDocumentPart?.Document?.Body?.InnerText).Contains("日本語");
        await File.WriteAllBytesAsync(Path.Combine(Path.GetTempPath(), "CrissCrossEditedWordFixture.docx"), output.ToArray());
    }

    /// <summary>Requires an element or attribute in the exported fixture.</summary>
    /// <typeparam name="T">The required object type.</typeparam>
    /// <param name="value">The exported object.</param>
    /// <returns>The required object.</returns>
    private static T Required<T>(T? value)
        where T : class => value ?? throw new InvalidDataException("The expected Word package element is absent.");

    /// <summary>Finds the first authored run font in the native story.</summary>
    /// <param name="body">The native body.</param>
    /// <returns>The exported run font.</returns>
    private static DocumentFormat.OpenXml.Wordprocessing.RunFonts FindFont(DocumentFormat.OpenXml.Wordprocessing.Body body) => Required(
        body.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Paragraph>()?.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Run>()
            ?.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.RunProperties>()?.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.RunFonts>());

    /// <summary>Collects schema failures with their document locations.</summary>
    /// <param name="document">The package to validate.</param>
    /// <returns>The complete validation errors.</returns>
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
