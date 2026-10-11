// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies real Word ZIP/XML interoperability and import validation.</summary>
public sealed class DocxDocumentConverterTests
{
    /// <summary>The maximum accepted compressed DOCX size.</summary>
    private const int ImportLimitBytes = 64 * 1_024 * 1_024;

    /// <summary>A one-pixel PNG image fixture.</summary>
    private const string ImageData = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jkz8AAAAASUVORK5CYII=";

    /// <summary>The bookmark identifier used by the retention fixture.</summary>
    private const int RetainedBookmarkId = 7;

    /// <summary>The expected count for both the merged rows and nested table.</summary>
    private const int TwoElements = 2;

    /// <summary>The expected count for the three nested list items.</summary>
    private const int ThreeElements = 3;

    /// <summary>The main Word document package part.</summary>
    private const string DocumentPart = "word/document.xml";

    /// <summary>The WordprocessingML namespace.</summary>
    private static readonly XNamespace Word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    /// <summary>Imports the fixture saved by Microsoft Word with all visible rich elements intact.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToHtml_NativeWordFixture_PreservesFormattingAndDocumentElements()
    {
        var bytes = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Fixtures", "WordNativeSample.docx"));
        var html = DocxDocumentConverter.ToHtml(bytes);
        var document = new HtmlParser().ParseDocument(html);
        await Assert.That(document.Body!.TextContent).Contains("日本語");
        await Assert.That(html).Contains("Times New Roman");
        await Assert.That(html).Contains("Consolas");
        await Assert.That(html).Contains("font-weight:bold");
        await Assert.That(html).Contains("color:#FF0000");
        await Assert.That(html).Contains("background-color:yellow");
        await Assert.That(document.QuerySelector("img")?.GetAttribute("src")).StartsWith("data:image/png;base64,");
        await Assert.That(document.QuerySelector("a")?.GetAttribute("href")).Contains("avalonia");
        await Assert.That(document.QuerySelector("table")?.TextContent).Contains("Milestone");
        await Assert.That(document.QuerySelector("br[data-docx-break='page']") is not null).IsTrue();
    }

    /// <summary>Writes real package parts and round trips Unicode, fonts, colors, links, and images.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToDocx_FormattedDocument_ProducesWordPackageAndPreservesContent()
    {
        const string htmlPrefix = "<p><strong>日本語 😀 café</strong><span style='font-family:Times New Roman;font-size:18pt;color:#123456;background-color:yellow'> styled</span>";
        const string htmlLinkPrefix = "<a href='https://example.com'>link</a>\n<img src='";
        const string htmlSuffix = "' width='24' height='12'></p><p style='break-before:page'>next</p>";
        const string html = $"{htmlPrefix}{htmlLinkPrefix}{ImageData}{htmlSuffix}";
        var bytes = DocxDocumentConverter.ToDocx(html);
        var xml = ReadPart(bytes, DocumentPart);
        await Assert.That(xml.Root?.Name == Word + "document").IsTrue();
        await Assert.That(CountElements(xml, Word + "drawing")).IsEqualTo(1);
        await Assert.That(CountElements(xml, Word + "pageBreakBefore")).IsEqualTo(1);
        await Assert.That(ReadPart(bytes, "word/_rels/document.xml.rels").ToString()).Contains("https://example.com");
        var imported = new HtmlParser().ParseDocument(DocxDocumentConverter.ToHtml(bytes));
        await Assert.That(imported.Body!.TextContent).Contains("日本語 😀 café styledlink");
        await Assert.That(imported.QuerySelector("img")?.GetAttribute("src")).IsEqualTo(ImageData);
        await Assert.That(imported.QuerySelector("a")?.GetAttribute("href")).IsEqualTo("https://example.com");
        await Assert.That(imported.Body.InnerHtml).Contains("Times New Roman");
        await Assert.That(imported.Body.InnerHtml).Contains("font-size:18pt");
        await Assert.That(imported.Body.InnerHtml).Contains("color:#123456");
    }

    /// <summary>Round trips vertical and horizontal merges and nested tables.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToDocx_NestedMergedTables_PreservesStructure()
    {
        const string Html = "<table><tr><td rowspan='2' colspan='2'><p>merged</p><table><tr><td>nested</td></tr></table></td><td>A</td></tr><tr><td>B</td></tr></table>";
        var bytes = DocxDocumentConverter.ToDocx(Html);
        var xml = ReadPart(bytes, DocumentPart);
        await Assert.That(CountElements(xml, Word + "tbl")).IsEqualTo(TwoElements);
        await Assert.That(CountElements(xml, Word + "vMerge")).IsEqualTo(TwoElements);
        var imported = new HtmlParser().ParseDocument(DocxDocumentConverter.ToHtml(bytes));
        await Assert.That(imported.QuerySelectorAll("table").Length).IsEqualTo(TwoElements);
        await Assert.That(imported.QuerySelector("td[rowspan]")?.GetAttribute("rowspan")).IsEqualTo("2");
        await Assert.That(imported.QuerySelector("td[colspan]")?.GetAttribute("colspan")).IsEqualTo("2");
        var again = ReadPart(DocxDocumentConverter.ToDocx(imported.Body!.InnerHtml), DocumentPart);
        await Assert.That(CountElements(again, Word + "vMerge")).IsEqualTo(TwoElements);
        await Assert.That(HasText(again, Word + "t", "nested")).IsTrue();
    }

    /// <summary>Creates valid numbering parts and preserves nested bullet and ordered lists.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToDocx_NestedLists_PreservesNumberingAndText()
    {
        var bytes = DocxDocumentConverter.ToDocx("<ol><li>first<ul><li>nested</li></ul></li><li>second</li></ol>");
        var xml = ReadPart(bytes, DocumentPart);
        await Assert.That(CountElements(xml, Word + "numPr")).IsEqualTo(ThreeElements);
        var numbering = ReadPart(bytes, "word/numbering.xml");
        await Assert.That(HasAttributeValue(numbering, Word + "numFmt", Word + "val", "bullet")).IsTrue();
        var imported = new HtmlParser().ParseDocument(DocxDocumentConverter.ToHtml(bytes));
        await Assert.That(imported.QuerySelector("ol ul li")?.TextContent).IsEqualTo("nested");
        await Assert.That(imported.Body!.TextContent).IsEqualTo("firstnestedsecond");
    }

    /// <summary>Retains untouched auxiliary parts and opaque passive structures after a text edit.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToHtml_AuxiliaryPartsAndOpaqueXml_SurviveTextEdit()
    {
        var original = DocxDocumentConverter.ToDocx("<p>original</p>");
        var xml = ReadPart(original, DocumentPart);
        FirstElement(xml, Word + "p").AddFirst(
            new XElement(Word + "bookmarkStart", new XAttribute(Word + "id", RetainedBookmarkId), new XAttribute(Word + "name", "retained")));
        var modified = ReplacePart(original, DocumentPart, Encoding.UTF8.GetBytes(xml.ToString()));
        modified = ReplacePart(modified, "customXml/item1.xml", "<passive>retained auxiliary data</passive>"u8.ToArray());
        var html = DocxDocumentConverter.ToHtml(modified).Replace("original", "edited", StringComparison.Ordinal);
        var exported = DocxDocumentConverter.ToDocx(html);
        var exportedXml = ReadPart(exported, DocumentPart);
        await Assert.That(FirstElement(exportedXml, Word + "t").Value).IsEqualTo("edited");
        await Assert.That(CountElements(exportedXml, Word + "bookmarkStart")).IsEqualTo(1);
        await Assert.That(ReadPart(exported, "customXml/item1.xml").Root!.Value).IsEqualTo("retained auxiliary data");
    }

    /// <summary>Preserves page geometry and exports edited repeated page regions as Word stories.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToDocx_SectionAndPageRegions_PreservesGeometryAndHeaderText()
    {
        const string Html = """
            <section data-section-id='one' data-page-width='800' data-page-height='1100' data-page-padding='40,50,60,70'><p>Body</p></section>
            <template data-page-region='header' data-section-id='one'><p>Header</p></template>
            """;
        var bytes = DocxDocumentConverter.ToDocx(Html);
        var xml = ReadPart(bytes, DocumentPart);
        await Assert.That((string?)FirstElement(xml, Word + "pgSz").Attribute(Word + "w")).IsEqualTo("12000");
        await Assert.That(CountElements(xml, Word + "headerReference")).IsEqualTo(1);
        var html = DocxDocumentConverter.ToHtml(bytes);
        await Assert.That(html).Contains("data-page-width=\"800\"");
        await Assert.That(html).Contains("data-page-region=\"header\"");
        await Assert.That(html).Contains("Header");
        var edited = DocxDocumentConverter.ToDocx(html.Replace(">Header<", ">Edited header<", StringComparison.Ordinal));
        await Assert.That(DocxDocumentConverter.ToHtml(edited)).Contains("Edited header");
    }

    /// <summary>Rejects malformed ZIP packages instead of accepting text as DOCX.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToHtml_InvalidPackage_Throws() => await Assert.That(static () => DocxDocumentConverter.ToHtml("not a zip"u8.ToArray())).Throws<InvalidDataException>();

    /// <summary>Rejects macros and DTD expansion before document import.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToHtml_ActiveContentAndDtd_RejectsImport()
    {
        var original = DocxDocumentConverter.ToDocx("<p>safe</p>");
        var types = ReadPart(original, "[Content_Types].xml");
        types.Root!.Add(new XElement(
            types.Root.Name.Namespace + "Override",
            new XAttribute("PartName", "/word/vbaProject.bin"),
            new XAttribute("ContentType", "application/vnd.ms-office.vbaProject")));
        var macro = ReplacePart(original, "[Content_Types].xml", Encoding.UTF8.GetBytes(types.ToString()));
        await Assert.That(() => DocxDocumentConverter.ToHtml(macro)).Throws<NotSupportedException>();
        var dtd = ReplacePart(original, DocumentPart, "<!DOCTYPE document [<!ENTITY payload 'expanded'>]><document>&payload;</document>"u8.ToArray());
        await Assert.That(() => DocxDocumentConverter.ToHtml(dtd)).Throws<System.Xml.XmlException>();
    }

    /// <summary>Rejects unsupported external resources and invalid table merges.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToDocx_UnsupportedContent_ThrowsWithoutSilentlyLosingIt()
    {
        await Assert.That(static () => DocxDocumentConverter.ToDocx("<img src='https://example.com/image.png'>")).Throws<NotSupportedException>();
        await Assert.That(static () => DocxDocumentConverter.ToDocx("<table><tr><td rowspan='2'>invalid</td></tr></table>")).Throws<InvalidDataException>();
        await Assert.That(static () => DocxDocumentConverter.ToDocx("<iframe src='https://example.com'></iframe>")).Throws<NotSupportedException>();
    }

    /// <summary>Rejects oversized caller-owned streams before attempting ZIP import and leaves them open.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ToHtml_OversizedStream_RejectsInputAndLeavesCallerStreamOpen()
    {
        await using var source = new MemoryStream();
        source.SetLength(ImportLimitBytes + 1);
        await Assert.That(() => DocxDocumentConverter.ToHtml(source)).Throws<InvalidDataException>();
        await Assert.That(source.CanRead).IsTrue();
    }

    /// <summary>Reads a package XML part independently from the converter.</summary>
    /// <param name="bytes">The ZIP package.</param>
    /// <param name="name">The entry name.</param>
    /// <returns>The parsed XML.</returns>
    private static XDocument ReadPart(byte[] bytes, string name)
    {
        using var stream = new MemoryStream(bytes);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        using var part = zip.GetEntry(name)!.Open();
        return XDocument.Load(part);
    }

    /// <summary>Counts descendants with the requested XML name.</summary>
    /// <param name="document">The XML container to inspect.</param>
    /// <param name="name">The element name to count.</param>
    /// <returns>The number of matching descendants.</returns>
    private static int CountElements(XContainer document, XName name)
    {
        var count = 0;
        foreach (var element in document.Descendants(name))
        {
            count++;
        }

        return count;
    }

    /// <summary>Returns the first descendant with the requested XML name.</summary>
    /// <param name="document">The XML container to inspect.</param>
    /// <param name="name">The element name to find.</param>
    /// <returns>The first matching element.</returns>
    private static XElement FirstElement(XContainer document, XName name)
    {
        using var enumerator = document.Descendants(name).GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException($"Expected XML element '{name}'.");
    }

    /// <summary>Checks whether an XML descendant has the expected text.</summary>
    /// <param name="document">The XML container to inspect.</param>
    /// <param name="name">The element name to inspect.</param>
    /// <param name="expected">The expected element text.</param>
    /// <returns><see langword="true"/> when a matching element exists.</returns>
    private static bool HasText(XContainer document, XName name, string expected)
    {
        foreach (var element in document.Descendants(name))
        {
            if (element.Value == expected)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Checks whether an XML descendant has the expected attribute value.</summary>
    /// <param name="document">The XML container to inspect.</param>
    /// <param name="name">The element name to inspect.</param>
    /// <param name="attribute">The attribute name to inspect.</param>
    /// <param name="expected">The expected attribute value.</param>
    /// <returns><see langword="true"/> when a matching element exists.</returns>
    private static bool HasAttributeValue(XContainer document, XName name, XName attribute, string expected)
    {
        foreach (var element in document.Descendants(name))
        {
            if ((string?)element.Attribute(attribute) == expected)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Replaces or adds a ZIP entry for independent import fixtures.</summary>
    /// <param name="bytes">The original ZIP.</param>
    /// <param name="name">The entry name.</param>
    /// <param name="replacement">The entry payload.</param>
    /// <returns>The changed package.</returns>
    private static byte[] ReplacePart(byte[] bytes, string name, byte[] replacement)
    {
        using var stream = new MemoryStream();
        stream.Write(bytes);
        stream.Position = 0;
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            zip.GetEntry(name)?.Delete();
            using var part = zip.CreateEntry(name).Open();
            part.Write(replacement);
        }

        return stream.ToArray();
    }
}
