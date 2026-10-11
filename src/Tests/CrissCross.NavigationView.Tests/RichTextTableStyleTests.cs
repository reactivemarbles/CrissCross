// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using CrissCross.Avalonia.UI.Controls;
using Border = Avalonia.Controls.Border;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies that imported cell styling reaches the rendered table.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextTableStyleTests
{
    /// <summary>The table width used for measurement.</summary>
    private const double TableWidth = 240;

    /// <summary>The authored border width.</summary>
    private const double BorderWidth = 3;

    /// <summary>The authored content padding.</summary>
    private const double ContentPadding = 8;

    /// <summary>The independent border widths in CSS side order.</summary>
    private static readonly Thickness EdgeWidths = new(1, 2, 3, 4);

    /// <summary>Independent edge widths emitted by Word RTF remain visible.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task IndependentBorderEdges_PreserveWidthsAndHeaderSemantics()
    {
        var document = new FlowDocument();
        document.SetText("<table><tr><th style='border-left:1px solid red;border-top:2px solid red;border-right:3px solid red;border-bottom:4px solid red'>Heading</th></tr></table>");
        var cell = document.Segments[0].Table?.Cells[0];
        await Assert.That(cell?.BorderThickness).IsEqualTo(EdgeWidths);
        await Assert.That(cell?.IsHeader).IsTrue();
    }

    /// <summary>Borders, padding and alignment are retained in the actual cell controls.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task CellStyles_ReachRenderedBorderAndContent()
    {
        var document = new FlowDocument();
        document.SetText("<table><tr><td style='border:3px solid red;padding:8px;vertical-align:middle'>Cell</td></tr></table>");
        var presenter = new FormattedTextPresenter { Document = document, TextWrapping = TextWrapping.Wrap };
        _ = presenter.MeasureDocument(TableWidth);
        presenter.Arrange(new(presenter.DesiredSize));
        var cellBorder = FindCellBorder(presenter);
        await Assert.That(cellBorder.BorderThickness).IsEqualTo(new(BorderWidth));
        await Assert.That((cellBorder.BorderBrush as ISolidColorBrush)?.Color).IsEqualTo(Colors.Red);
        await Assert.That(cellBorder.Padding).IsEqualTo(new(ContentPadding));
        await Assert.That(cellBorder.Child?.VerticalAlignment).IsEqualTo(VerticalAlignment.Center);
    }

    /// <summary>Finds the actual cell border created by the table renderer.</summary>
    /// <param name="presenter">The measured document presenter.</param>
    /// <returns>The rendered table cell border.</returns>
    private static Border FindCellBorder(FormattedTextPresenter presenter)
    {
        foreach (var visual in presenter.GetVisualDescendants())
        {
            if (visual is Border { Child: FormattedTextPresenter } border)
            {
                return border;
            }
        }

        throw new InvalidOperationException("The table renderer did not create a cell border.");
    }
}
