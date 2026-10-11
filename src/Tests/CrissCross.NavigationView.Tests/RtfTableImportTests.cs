// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies import of native RTF table definitions independently of the writer.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RtfTableImportTests
{
    /// <summary>The number of cells remaining after a merge.</summary>
    private const int MergedCellCount = 2;

    /// <summary>The HTML cell style attribute.</summary>
    private const string StyleAttribute = "style";

    /// <summary>The HTML column span attribute.</summary>
    private const string ColumnSpanAttribute = "colspan";

    /// <summary>Reads horizontal merge placeholders as a single HTML cell.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_HorizontalMerge_CombinesCellsAndWidth()
    {
        const string Rtf = @"{\rtf1\ansi\trowd\clmgf\cellx1500\clmrg\cellx3000\cellx4500\pard\intbl A\cell B\cell C\cell\row}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        var cells = document.QuerySelectorAll("td");
        await Assert.That(cells.Length).IsEqualTo(MergedCellCount);
        await Assert.That(cells[0].GetAttribute(ColumnSpanAttribute)).IsEqualTo("2");
        await Assert.That(cells[0].TextContent).IsEqualTo("AB");
        await Assert.That(cells[0].GetAttribute(StyleAttribute)!).Contains("width:200px");
        await Assert.That(cells[1].TextContent).IsEqualTo("C");
    }

    /// <summary>Matches vertical continuations by physical column boundaries.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_VerticalMerge_ExtendsOriginAcrossRows()
    {
        const string Rtf = @"{\rtf1\trowd\clvmgf\cellx1500\cellx3000\intbl A\cell B\cell\row"
            + @"\trowd\clvmrg\cellx1500\cellx3000\intbl\cell C\cell\row}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        var rows = document.QuerySelectorAll("tr");
        await Assert.That(rows.Length).IsEqualTo(MergedCellCount);
        await Assert.That(rows[0].QuerySelector("td")!.GetAttribute("rowspan")).IsEqualTo("2");
        await Assert.That(rows[1].QuerySelectorAll("td").Length).IsEqualTo(1);
        await Assert.That(rows[1].TextContent).IsEqualTo("C");
    }

    /// <summary>Reads a rectangular merge using both native merge directions.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_RectangularMerge_PreservesBothSpans()
    {
        const string Rtf = @"{\rtf1\trowd\clmgf\clvmgf\cellx1500\clmrg\clvmgf\cellx3000\intbl A\cell\cell\row"
            + @"\trowd\clmgf\clvmrg\cellx1500\clmrg\clvmrg\cellx3000\intbl\cell\cell\row}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        var cells = document.QuerySelectorAll("td");
        await Assert.That(cells.Length).IsEqualTo(1);
        await Assert.That(cells[0].GetAttribute(ColumnSpanAttribute)).IsEqualTo("2");
        await Assert.That(cells[0].GetAttribute("rowspan")).IsEqualTo("2");
        await Assert.That(cells[0].TextContent).IsEqualTo("A");
    }

    /// <summary>Reads Word's trailing nested definition and ignores legacy reader fallback text.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_TrailingNestedProperties_PreservesParentContents()
    {
        const string Rtf = @"{\rtf1\trowd\cellx4500\intbl\itap1 before"
            + @"\pard\intbl\itap2 inside\nestcell{\nonesttables duplicate\par}"
            + @"{\*\nesttableprops\trowd\cellx1500\nestrow}"
            + @"\pard\intbl\itap1 after\cell\row}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        await Assert.That(document.QuerySelectorAll("table").Length).IsEqualTo(MergedCellCount);
        await Assert.That(document.QuerySelector("td table td")!.TextContent).IsEqualTo("inside");
        await Assert.That(document.QuerySelector("td")!.TextContent).IsEqualTo("beforeinsideafter");
        await Assert.That(document.QuerySelector("td table td")!.GetAttribute(StyleAttribute)!).Contains("width:100px");
    }

    /// <summary>Converts native cell shading, border geometry, and explicit width.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_CellDecoration_PreservesDimensionsAndColors()
    {
        const string Rtf = @"{\rtf1{\colortbl;\red255\green0\blue0;\red0\green0\blue255;}"
            + @"\trowd\clftsWidth3\clwWidth1800\clcbpat1\clvertalc"
            + @"\clbrdrt\brdrdb\brdrw30\brdrcf2\cellx3000\intbl decorated\cell\row}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        var style = document.QuerySelector("td")!.GetAttribute(StyleAttribute)!;
        await Assert.That(style).Contains("width:120px");
        await Assert.That(style).Contains("background-color:#FF0000");
        await Assert.That(style).Contains("border-top:2px double #0000FF");
        await Assert.That(style).Contains("vertical-align:middle");
    }

    /// <summary>Uses the final row definition when Word repeats properties after cell contents.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_RepeatedRowDefinition_UsesTrailingProperties()
    {
        const string Rtf = @"{\rtf1\trowd\cellx1500\intbl A\cell"
            + @"{\trowd\cellx3000\row}}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        await Assert.That(document.QuerySelectorAll("td").Length).IsEqualTo(1);
        await Assert.That(document.QuerySelector("td")!.TextContent).IsEqualTo("A");
        await Assert.That(document.QuerySelector("td")!.GetAttribute(StyleAttribute)!).Contains("width:200px");
    }

    /// <summary>Keeps the outer edge border supplied by a horizontal continuation cell.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_MergedCellEdge_PreservesContinuationBorder()
    {
        const string Rtf = @"{\rtf1\trowd\clmgf\clbrdrl\brdrs\brdrw15\cellx1500"
            + @"\clmrg\clbrdrr\brdrdash\brdrw30\cellx3000\intbl A\cell\cell\row}";
        var document = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf));
        var style = document.QuerySelector("td")!.GetAttribute(StyleAttribute)!;
        await Assert.That(style).Contains("border-left:1px solid currentColor");
        await Assert.That(style).Contains("border-right:2px dashed currentColor");
    }

    /// <summary>Retains inherited row geometry when the next row omits its definition.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task ToHtml_InheritedRowDefinition_PreservesCellWidths()
    {
        const string Rtf = @"{\rtf1\trowd\cellx1500\intbl A\cell\row\intbl B\cell\row}";
        var cells = new HtmlParser().ParseDocument(RtfDocumentConverter.ToHtml(Rtf)).QuerySelectorAll("td");
        await Assert.That(cells.Length).IsEqualTo(MergedCellCount);
        await Assert.That(cells[1].TextContent).IsEqualTo("B");
        await Assert.That(cells[1].GetAttribute(StyleAttribute)!).Contains("width:100px");
    }

    /// <summary>Pastes a native merged RTF table when the clipboard has no HTML representation.</summary>
    /// <returns>The test execution task.</returns>
    [Test]
    public async Task Paste_RtfOnlyClipboard_PreservesMergedTable()
    {
        var clipboard = new RichTextMemoryClipboardAdapter { RtfText = @"{\rtf1\trowd\clmgf\cellx1500\clmrg\cellx3000\intbl merged\cell\cell\row}" };
        var editor = new RichTextBox { ClipboardAdapter = clipboard };
        await editor.PasteFromClipboardAsync();
        var document = new HtmlParser().ParseDocument(editor.Html);
        await Assert.That(document.QuerySelector("td")!.GetAttribute(ColumnSpanAttribute)).IsEqualTo("2");
        await Assert.That(document.QuerySelector("td")!.TextContent).IsEqualTo("merged");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo(string.Empty);
        editor.Redo();
        await Assert.That(new HtmlParser().ParseDocument(editor.Html).QuerySelector("td")!.TextContent).IsEqualTo("merged");
    }

    /// <summary>Rejects malformed merge ranges and cell definitions instead of losing structure.</summary>
    /// <param name="rtf">The invalid table fixture.</param>
    /// <returns>The test execution task.</returns>
    [Test]
    [Arguments(@"{\rtf1\trowd\clmrg\cellx1500\intbl A\cell\row}")]
    [Arguments(@"{\rtf1\trowd\clvmrg\cellx1500\intbl A\cell\row}")]
    [Arguments(@"{\rtf1\trowd\cellx1500\cellx1000\intbl A\cell\row}")]
    [Arguments(@"{\rtf1\trowd\cellx1500\cellx3000\intbl A\cell\row}")]
    [Arguments(@"{\rtf1\trowd\cellx1500\intbl A\cell}")]
    [Arguments(@"{\rtf1\itap3 orphan}")]
    public async Task ToHtml_InvalidTable_ThrowsFormatException(string rtf) =>
        await Assert.That(() => RtfDocumentConverter.ToHtml(rtf)).Throws<FormatException>();
}
