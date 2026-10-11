// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies crossing annotations spanning multiple Word paragraphs.</summary>
public sealed class DocxCrossRangeTests
{
    /// <summary>The WordprocessingML namespace.</summary>
    private static readonly XNamespace Word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    /// <summary>Crossing native ranges retain semantic coverage and one native pair after export.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task CrossParagraphRanges_ImportExport_PreservesCoverageAndSchema()
    {
        var bytes = CreateCrossingFixture();
        var html = DocxDocumentConverter.ToHtml(bytes);
        var parsed = new HtmlParser().ParseDocument(html);
        var paragraphs = parsed.QuerySelectorAll("p");
        await Assert.That(paragraphs[0].QuerySelector("span[data-bookmark='target']")?.TextContent).IsEqualTo("First");
        await Assert.That(paragraphs[1].QuerySelector("span[data-bookmark='target']")?.TextContent).IsEqualTo("Second");
        await Assert.That(paragraphs[1].QuerySelector("mark[data-comment-id]")?.TextContent).IsEqualTo("Second");
        await Assert.That(paragraphs[1].QuerySelector("mark")?.GetAttribute("data-author")).IsEqualTo("Reviewer");
        await Assert.That(paragraphs[2].QuerySelector("mark[data-comment-id]")?.TextContent).IsEqualTo("Third");
        await Assert.That(paragraphs[2].QuerySelector("span[data-bookmark]") is null).IsTrue();
        await using var stream = new MemoryStream(DocxDocumentConverter.ToDocx(html));
        using var package = WordprocessingDocument.Open(stream, false);
        var native = XDocument.Parse(NativeXml(package));
        await Assert.That(CountMarkers(native, "bookmarkStart")).IsEqualTo(1);
        await Assert.That(CountMarkers(native, "bookmarkEnd")).IsEqualTo(1);
        await Assert.That(CountMarkers(native, "commentRangeStart")).IsEqualTo(1);
        await Assert.That(CountMarkers(native, "commentRangeEnd")).IsEqualTo(1);
        List<string> errors = [];
        foreach (var error in new OpenXmlValidator(FileFormatVersions.Microsoft365).Validate(package))
        {
            errors.Add(error.Description ?? "Open XML validation error.");
        }

        await Assert.That(errors).IsEmpty();
    }

    /// <summary>Reads the required native document story.</summary>
    /// <param name="package">The exported package.</param>
    /// <returns>The document XML.</returns>
    private static string NativeXml(WordprocessingDocument package) =>
        package.MainDocumentPart?.Document?.OuterXml ?? throw new InvalidDataException("Missing document story.");

    /// <summary>Moves native range boundaries into crossing paragraph positions.</summary>
    /// <returns>The modified Word package.</returns>
    private static byte[] CreateCrossingFixture()
    {
        var bytes = DocxDocumentConverter.ToDocx("<p><span data-bookmark='target'>First</span></p>"
            + "<p><mark data-comment-id='review' data-author='Reviewer' title='Check'>Second</mark></p><p>Third</p>");
        using var stream = new MemoryStream();
        stream.Write(bytes);
        stream.Position = 0;
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            var entry = zip.GetEntry("word/document.xml")!;
            XDocument xml;
            using (var input = entry.Open())
            {
                xml = XDocument.Load(input);
            }

            var paragraphs = new List<XElement>(xml.Descendants(Word + "p"));
            var bookmarkEnd = paragraphs[0].Element(Word + "bookmarkEnd")!;
            bookmarkEnd.Remove();
            paragraphs[1].Add(bookmarkEnd);
            var commentEnd = paragraphs[1].Element(Word + "commentRangeEnd")!;
            commentEnd.Remove();
            paragraphs[2].Add(commentEnd);
            entry.Delete();
            using var output = zip.CreateEntry("word/document.xml").Open();
            xml.Save(output);
        }

        return stream.ToArray();
    }

    /// <summary>Counts one native annotation boundary kind.</summary>
    /// <param name="document">The native document.</param>
    /// <param name="name">The boundary name.</param>
    /// <returns>The number of boundaries.</returns>
    private static int CountMarkers(XDocument document, string name)
    {
        var count = 0;
        foreach (var marker in document.Descendants(Word + name))
        {
            count++;
        }

        return count;
    }
}
