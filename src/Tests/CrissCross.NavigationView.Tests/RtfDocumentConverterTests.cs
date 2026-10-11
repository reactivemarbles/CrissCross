// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies RTF document interoperability and loss prevention.</summary>
public sealed class RtfDocumentConverterTests
{
    /// <summary>The CSS declaration for a bold character run.</summary>
    private const string BoldStyle = "font-weight:bold";

    /// <summary>The HTML attribute containing inline formatting.</summary>
    private const string StyleAttributeName = "style";

    /// <summary>The number of images and rows in the structured fixture.</summary>
    private const int ImageAndRowCount = 2;

    /// <summary>The number of paragraphs in the empty-paragraph fixture.</summary>
    private const int ParagraphFixtureCount = 3;

    /// <summary>Writes one forced break when modern and legacy CSS request the same sheet.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_DuplicatePageBreakDeclarations_EmitsOneBreak()
    {
        var rtf = RtfDocumentConverter.ToRtf("<p style='page-break-before:always;break-before:page'>Page</p>");
        await Assert.That(rtf.Contains("\\page ", StringComparison.Ordinal)).IsTrue();
        await Assert.That(rtf.Contains("\\page \\page ", StringComparison.Ordinal)).IsFalse();
    }

    /// <summary>Reads Word-shaped font/color tables, scoped formatting, Unicode, and hyperlinks.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_WordDocument_PreservesFormattedUnicodeAndLinks()
    {
        const string Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0"
            + "{\\fonttbl{\\f0\\fnil Calibri;}{\\f1\\fnil Times New Roman;}}"
            + "{\\colortbl;\\red255\\green0\\blue0;}\\viewkind4\\uc1\\pard\\f1\\fs32\\cf1 "
            + "{\\b Bold} normal \\u937? "
            + "{\\field{\\*\\fldinst HYPERLINK \"https://example.com\"}{\\fldrslt Click}}\\par}";
        var html = RtfDocumentConverter.ToHtml(Rtf);
        var document = new HtmlParser().ParseDocument(html);
        await Assert.That(document.Body!.TextContent).IsEqualTo("Bold normal Ω Click");
        await Assert.That(html).Contains("Times New Roman");
        await Assert.That(html).Contains("font-size:16pt");
        await Assert.That(html).Contains("color:#FF0000");
        await Assert.That(document.QuerySelector("a")!.GetAttribute("href")).IsEqualTo("https://example.com");
        var spans = document.QuerySelectorAll("span");
        await Assert.That(spans[0].GetAttribute(StyleAttributeName)!).Contains(BoldStyle);
        await Assert.That(spans[1].GetAttribute(StyleAttributeName)!.Contains(BoldStyle, StringComparison.Ordinal)).IsFalse();
    }

    /// <summary>Round trips Unicode surrogate pairs and RTF escape characters.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_UnicodeAndEscapes_PreservesText()
    {
        const string Text = "Greek Ω, Chinese 中文, emoji 😀, {braces} and \\slash";
        var rtf = RtfDocumentConverter.ToRtf($"<p>{Text}</p>");
        var html = RtfDocumentConverter.ToHtml(rtf);
        await Assert.That(new HtmlParser().ParseDocument(html).Body!.TextContent).IsEqualTo(Text);
        await Assert.That(rtf).Contains("\\u");
    }

    /// <summary>Retains image bytes, table cells, and explicit page breaks.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_ImagesTablesAndPageBreaks_PreservesStructure()
    {
        const string Html = "<p><img src='data:image/png;base64,iVBORw0KGgo=' width='80' height='40'>"
            + "<img src='data:image/jpeg;base64,/9j/2Q=='></p>"
            + "<table><tr><td>first</td><td><strong>second</strong></td></tr><tr><td>third</td><td>fourth</td></tr></table>"
            + "<div style='page-break-before:always'></div><p>next page</p>";
        var html = RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf(Html));
        var document = new HtmlParser().ParseDocument(html);
        var images = document.QuerySelectorAll("img");
        await Assert.That(images.Length).IsEqualTo(ImageAndRowCount);
        await Assert.That(images[0].GetAttribute("src")).IsEqualTo("data:image/png;base64,iVBORw0KGgo=");
        await Assert.That(images[0].GetAttribute("width")).IsEqualTo("80");
        await Assert.That(images[1].GetAttribute("src")).IsEqualTo("data:image/jpeg;base64,/9j/2Q==");
        await Assert.That(document.QuerySelectorAll("tr").Length).IsEqualTo(ImageAndRowCount);
        await Assert.That(ReadCellTexts(document.QuerySelectorAll("td"))).IsEqualTo("first|second|third|fourth");
        await Assert.That(html).Contains("break-before:page");
        await Assert.That(document.Body!.TextContent).Contains("next page");
    }

    /// <summary>Uses Windows-1252 escapes and group-specific Unicode fallback counts.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_AnsiAndUnicodeFallback_ReadsCharactersOnce()
    {
        var html = RtfDocumentConverter.ToHtml("{\\rtf1\\ansi\\ansicpg1252 \\'80 {\\uc2\\u937??} \\u20013?\\u25991?}");
        await Assert.That(new HtmlParser().ParseDocument(html).Body!.TextContent).IsEqualTo("€ Ω 中文");
    }

    /// <summary>Reads binary pictures containing RTF delimiters.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_BinaryPicture_DoesNotParsePayloadAsMarkup()
    {
        var html = RtfDocumentConverter.ToHtml("{\\rtf1{\\pict\\pngblip\\bin4 {\\}x}}");
        await Assert.That(new HtmlParser().ParseDocument(html).QuerySelector("img")!.GetAttribute("src")).IsEqualTo("data:image/png;base64,e1x9eA==");
    }

    /// <summary>Rejects truncated RTF without returning a partial document.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_UnclosedGroup_ThrowsFormatException() =>
        await Assert.That(static () => RtfDocumentConverter.ToHtml("{\\rtf1 missing end")).Throws<FormatException>();

    /// <summary>Rejects embedded OLE instead of discarding it.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_EmbeddedObject_ThrowsNotSupportedException() =>
        await Assert.That(static () => RtfDocumentConverter.ToHtml("{\\rtf1 before {\\object\\objemb data} after}")).Throws<NotSupportedException>();

    /// <summary>Requires an embedded byte payload for image export.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_RemoteImage_ThrowsNotSupportedException() =>
        await Assert.That(static () => RtfDocumentConverter.ToRtf("<img src='https://example.com/a.png'>")).Throws<NotSupportedException>();

    /// <summary>Includes native merged-cell controls in optional RTF clipboard output.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task TryToRtf_MergedTable_PreservesMergeControls() =>
        await Assert.That(RtfDocumentConverter.TryToRtf("<table><tr><td colspan='2'>merged</td></tr></table>")).Contains("\\clmgf");

    /// <summary>Reports malformed image data during optional RTF export.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task TryToRtf_MalformedEmbeddedImage_ThrowsFormatException() =>
        await Assert.That(static () => RtfDocumentConverter.TryToRtf("<img src='data:image/png;base64,invalid!'>")).Throws<FormatException>();

    /// <summary>Retains character styles, alignment, and encoded hyperlink targets through RTF.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_HtmlStylesAndLinks_PreservesFormatting()
    {
        const string Html = "<p style='text-align:right'><span style=\"font-family:'Times New Roman';font-size:18px;"
            + "color:#010203;background-color:#AABBCC;font-weight:700;font-style:italic;"
            + "text-decoration:underline line-through\">Ω</span>"
            + "<a href='https://example.com/?a=1&amp;b=2'>Visit</a></p>";
        var html = RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf(Html));
        var document = new HtmlParser().ParseDocument(html);
        var style = document.QuerySelector("span")!.GetAttribute(StyleAttributeName)!;
        await Assert.That(style).Contains("Times New Roman");
        await Assert.That(style).Contains("font-size:13.5pt");
        await Assert.That(style).Contains("color:#010203");
        await Assert.That(style).Contains("background-color:#AABBCC");
        await Assert.That(style).Contains(BoldStyle);
        await Assert.That(style).Contains("font-style:italic");
        await Assert.That(style).Contains("text-decoration:underline line-through");
        await Assert.That(document.QuerySelector("p")!.GetAttribute(StyleAttributeName)!).Contains("text-align:right");
        await Assert.That(document.QuerySelector("a")!.GetAttribute("href")).IsEqualTo("https://example.com/?a=1&b=2");
    }

    /// <summary>Preserves intentionally empty paragraphs through RTF.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_EmptyParagraphs_PreservesParagraphCount()
    {
        var html = RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf("<p></p><p>text</p><p></p>"));
        var paragraphs = new HtmlParser().ParseDocument(html).QuerySelectorAll("p");
        await Assert.That(paragraphs.Length).IsEqualTo(ParagraphFixtureCount);
        await Assert.That(paragraphs[0].TextContent).IsEqualTo(string.Empty);
        await Assert.That(paragraphs[1].TextContent).IsEqualTo("text");
        await Assert.That(paragraphs[ParagraphFixtureCount - 1].TextContent).IsEqualTo(string.Empty);
    }

    /// <summary>Reads hyperlink instructions nested in the groups produced by Word.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_NestedWordField_PreservesHyperlink()
    {
        const string Rtf = "{\\rtf1{\\field{\\*\\fldinst{HYPERLINK \"https://example.com/nested\"}}"
            + "{\\fldrslt Nested link}}}";
        var link = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf)).QuerySelector("a")!;
        await Assert.That(link.GetAttribute("href")).IsEqualTo("https://example.com/nested");
        await Assert.That(link.TextContent).IsEqualTo("Nested link");
    }

    /// <summary>Returns table cell text in document order.</summary>
    /// <param name="cells">The parsed table cells.</param>
    /// <returns>The cell contents separated by pipes.</returns>
    private static string ReadCellTexts(IHtmlCollection<IElement> cells)
    {
        var content = new List<string>();
        foreach (var cell in cells)
        {
            content.Add(cell.TextContent);
        }

        return string.Join("|", content);
    }
}
