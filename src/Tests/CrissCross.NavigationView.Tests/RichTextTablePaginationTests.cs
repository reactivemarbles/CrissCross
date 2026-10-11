// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies that table rows participate in page layout.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextTablePaginationTests
{
    /// <summary>Table sheets provide room for individual rows.</summary>
    private const double PageWidth = 240;

    /// <summary>The printable height can contain one ordinary row.</summary>
    private const double PageHeight = 60;

    /// <summary>The sheet margin.</summary>
    private const double Margin = 10;

    /// <summary>The caret midpoint fraction.</summary>
    private const double Half = 0.5;

    /// <summary>A long table flows across sheets at row boundaries.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task TableRows_FlowAcrossSheetsAndMapCaret()
    {
        var document = new FlowDocument();
        document.SetText("<table><tr><td>First</td><td>A</td></tr><tr><td>Second</td><td>B</td></tr><tr><td>Third</td><td>C</td></tr></table>");
        var surface = new PagedDocumentPresenter { Document = document, PageSize = new(PageWidth, PageHeight), PagePadding = new(Margin) };
        surface.Measure(new(PageWidth, double.PositiveInfinity));
        surface.Arrange(new(surface.DesiredSize));
        await Assert.That(surface.PageCount).IsGreaterThan(1);
        await Assert.That(surface.HasOverflow).IsFalse();
        var offset = document.PlainText.IndexOf("Second", StringComparison.Ordinal);
        var caret = surface.GetDocumentCaretRect(offset);
        await Assert.That(caret.Top).IsGreaterThan(PageHeight);
        await Assert.That(surface.GetDocumentPosition(new(caret.Left, caret.Top + (caret.Height * Half)))).IsEqualTo(offset);
    }
}
