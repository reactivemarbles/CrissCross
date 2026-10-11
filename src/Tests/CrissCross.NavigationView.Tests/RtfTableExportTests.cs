// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies standard RTF table export and semantic round trips.</summary>
public sealed class RtfTableExportTests
{
    /// <summary>The rectangular merge fixture.</summary>
    private const string MergedText = "merged";

    /// <summary>The HTML row span attribute.</summary>
    private const string RowSpanAttribute = "rowspan";

    /// <summary>The rectangular merge fixture.</summary>
    private const string RectangularTable = $"<table><tr><td colspan='2' {RowSpanAttribute}='2'>{MergedText}</td><td>A</td></tr><tr><td>B</td></tr></table>";

    /// <summary>The nested table fixture.</summary>
    private const string NestedTable = "<table><tr><td>before<table><tr><td>inner</td><td>second</td></tr></table>after</td><td>outer</td></tr></table>";

    /// <summary>The number of physical cells in a two by three grid.</summary>
    private const int RectangularPhysicalCells = 6;

    /// <summary>The number of source cells after merging.</summary>
    private const int RectangularSourceCells = 3;

    /// <summary>The number of table nesting levels in the fixture.</summary>
    private const int NestedLevels = 2;

    /// <summary>The count of nested fixture cells.</summary>
    private const int NestedCellCount = 4;

    /// <summary>Emits independent standard controls for a rectangular merge.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_RectangularMerge_EmitsPhysicalMergeDescriptors()
    {
        var rtf = RtfDocumentConverter.ToRtf(RectangularTable);
        await Assert.That(rtf).Contains("\\clmgf\\clvmgf");
        await Assert.That(rtf).Contains("\\clmrg\\clvmgf");
        await Assert.That(rtf).Contains("\\clmgf\\clvmrg");
        await Assert.That(rtf).Contains("\\clmrg\\clvmrg");
        await Assert.That(CountControl(rtf, "\\cell}")).IsEqualTo(RectangularPhysicalCells);
        await Assert.That(CountControl(rtf, MergedText)).IsEqualTo(1);
    }

    /// <summary>Retains combined row and column spans without exporting continuation text.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_RectangularMerge_PreservesOriginAndSpans()
    {
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf(RectangularTable)));
        var cells = document.QuerySelectorAll("td");
        await Assert.That(cells.Length).IsEqualTo(RectangularSourceCells);
        await Assert.That(cells[0].GetAttribute("colspan")).IsEqualTo("2");
        await Assert.That(cells[0].GetAttribute(RowSpanAttribute)).IsEqualTo("2");
        await Assert.That(cells[0].TextContent).IsEqualTo(MergedText);
        await Assert.That(cells[1].TextContent).IsEqualTo("A");
        await Assert.That(cells[2].TextContent).IsEqualTo("B");
    }

    /// <summary>Uses trailing nested row properties and restores parent depth.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_NestedTable_EmitsNestedRowGrammar()
    {
        var rtf = RtfDocumentConverter.ToRtf(NestedTable);
        await Assert.That(rtf).Contains("\\itap2");
        await Assert.That(rtf).Contains("\\nestcell}");
        await Assert.That(rtf).Contains("{\\*\\nesttableprops\\trowd");
        await Assert.That(rtf).Contains("\\nestrow}");
        await Assert.That(rtf).Contains("\\pard\\intbl\\itap1 after");
        await Assert.That(CountControl(rtf, "\\row ")).IsEqualTo(1);
    }

    /// <summary>Writes a combined merge and nested-table document for native Word inspection.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_AdvancedTables_WritesWordVerificationFixture()
    {
        const string html = $"<html>{RectangularTable}{NestedTable}</html>";
        var rtf = RtfDocumentConverter.ToRtf(html);
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CrissCrossAdvancedTables.rtf");
        await System.IO.File.WriteAllTextAsync(path, rtf, System.Text.Encoding.ASCII);
    }

    /// <summary>Keeps descendant rows inside their owning parent cell.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_NestedTable_PreservesHierarchyAndFollowingText()
    {
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf(NestedTable)));
        await Assert.That(document.QuerySelectorAll("table").Length).IsEqualTo(NestedLevels);
        await Assert.That(document.QuerySelectorAll("tr").Length).IsEqualTo(NestedLevels);
        await Assert.That(document.QuerySelectorAll("td").Length).IsEqualTo(NestedCellCount);
        await Assert.That(document.QuerySelector("td table td")!.TextContent).IsEqualTo("inner");
        await Assert.That(document.QuerySelector("td")!.TextContent).IsEqualTo("beforeinnersecondafter");
        await Assert.That(document.QuerySelector("body > table > tbody > tr > td:last-child")!.TextContent).IsEqualTo("outer");
    }

    /// <summary>Emits native cell widths, shading, and border controls.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_CellDecoration_EmitsNativeDescriptors()
    {
        var rtf = RtfDocumentConverter.ToRtf("<table><tr><td style='width:120px;background-color:#AABBCC;border:1px solid #123456;vertical-align:middle'>cell</td></tr></table>");
        await Assert.That(rtf).Contains("\\clftsWidth3\\clwWidth1800\\cellx1800");
        await Assert.That(rtf).Contains("\\clcbpat1");
        await Assert.That(rtf).Contains("\\clvertalc");
        await Assert.That(rtf).Contains("\\clbrdrt\\brdrw15\\brdrs\\brdrcf2");
        await Assert.That(rtf).Contains("\\clbrdrb");
        await Assert.That(rtf).Contains("\\clbrdrl");
        await Assert.That(rtf).Contains("\\clbrdrr");
    }

    /// <summary>Rejects overlapping HTML rectangles instead of silently changing them.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToRtf_OverlappingSpans_ThrowsFormatException() =>
        await Assert.That(static () => RtfDocumentConverter.ToRtf("<table><tr><td>A</td><td rowspan='2'>B</td></tr><tr><td colspan='2'>overlap</td></tr></table>")).Throws<FormatException>();

    /// <summary>Combines nested tables and physical merge placeholders in the same row.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_NestedRectangularMerge_PreservesInnerSpan()
    {
        const string html = $"<table><tr><td>outer{RectangularTable}tail</td></tr></table>";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf(html)));
        var inner = document.QuerySelector("td table td")!;
        await Assert.That(inner.GetAttribute("colspan")).IsEqualTo("2");
        await Assert.That(inner.GetAttribute(RowSpanAttribute)).IsEqualTo("2");
        await Assert.That(inner.TextContent).IsEqualTo("merged");
        await Assert.That(document.QuerySelector("td")!.TextContent).IsEqualTo("outermergedABtail");
    }

    /// <summary>Restricts HTML rowspan zero to its originating row group.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task RoundTrip_RowSpanZero_PreservesRowGroupBoundary()
    {
        const string Html = "<table><tbody><tr><td rowspan='0'>span</td><td>A</td></tr><tr><td>B</td></tr></tbody><tbody><tr><td>C</td><td>D</td></tr></tbody></table>";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(RtfDocumentConverter.ToRtf(Html)));
        await Assert.That(document.QuerySelector("td")!.GetAttribute("rowspan")).IsEqualTo("2");
        await Assert.That(document.QuerySelectorAll("tr")[NestedLevels].QuerySelector("td")!.TextContent).IsEqualTo("C");
    }

    /// <summary>Counts a literal control sequence without confusing its longer variants.</summary>
    /// <param name="rtf">The exported document.</param>
    /// <param name="control">The exact sequence.</param>
    /// <returns>The occurrence count.</returns>
    private static int CountControl(string rtf, string control)
    {
        var count = 0;
        var offset = 0;
        for (offset = 0; (offset = rtf.IndexOf(control, offset, StringComparison.Ordinal)) >= 0; offset += control.Length)
        {
            count++;
        }

        return count;
    }
}
