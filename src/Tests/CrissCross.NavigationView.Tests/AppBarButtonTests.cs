// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests the Avalonia AppBarButton public property surface.</summary>
public class AppBarButtonTests
{
    /// <summary>The default circular icon surface diameter.</summary>
    private const double DefaultDiameter = 40D;

    /// <summary>The default icon dimension.</summary>
    private const double DefaultIconDimension = 25D;

    /// <summary>The assigned background glyph size.</summary>
    private const double AssignedBackgroundGlyphSize = 28D;

    /// <summary>The assigned circular icon surface diameter.</summary>
    private const double AssignedDiameter = 48D;

    /// <summary>The assigned foreground glyph size.</summary>
    private const double AssignedForegroundGlyphSize = 20D;

    /// <summary>The assigned icon height.</summary>
    private const double AssignedIconHeight = 30D;

    /// <summary>The assigned icon width.</summary>
    private const double AssignedIconWidth = 32D;

    /// <summary>The custom geometry test extent.</summary>
    private const double GeometryExtent = 10D;

    /// <summary>Verifies that a new button uses the documented WPF-compatible layout defaults.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_UsesDocumentedDefaults()
    {
        var button = new AppBarButton();

        await Assert.That(button.BackgroundGlyph).IsEmpty();
        await Assert.That(button.ForegroundGlyph).IsEmpty();
        await Assert.That(button.ElipseDiameter).IsEqualTo(DefaultDiameter);
        await Assert.That(button.Icon).IsEqualTo(SymbolRegular.Empty);
        await Assert.That(button.IconData).IsNull();
        await Assert.That(button.IconHeight).IsEqualTo(DefaultIconDimension);
        await Assert.That(button.IconWidth).IsEqualTo(DefaultIconDimension);
        await Assert.That(button.IsIconFilled).IsFalse();
    }

    /// <summary>Verifies that native symbols, custom geometry, and glyph layers remain bindable.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task StyledProperties_WhenAssigned_RetainValues()
    {
        var geometry = new RectangleGeometry(new Rect(0D, 0D, GeometryExtent, GeometryExtent));
        var brush = Brushes.OrangeRed;
        var button = new AppBarButton
        {
            BackgroundGlyph = "○",
            BackgroundGlyphFontSize = AssignedBackgroundGlyphSize,
            ElipseDiameter = AssignedDiameter,
            ForegroundGlyph = "+",
            ForegroundGlyphColor = brush,
            ForegroundGlyphFontSize = AssignedForegroundGlyphSize,
            Icon = SymbolRegular.Save,
            IconData = geometry,
            IconHeight = AssignedIconHeight,
            IconWidth = AssignedIconWidth,
            IsIconFilled = true,
        };

        await Assert.That(button.BackgroundGlyph).IsEqualTo("○");
        await Assert.That(button.BackgroundGlyphFontSize).IsEqualTo(AssignedBackgroundGlyphSize);
        await Assert.That(button.ElipseDiameter).IsEqualTo(AssignedDiameter);
        await Assert.That(button.ForegroundGlyph).IsEqualTo("+");
        await Assert.That(button.ForegroundGlyphColor).IsSameReferenceAs(brush);
        await Assert.That(button.ForegroundGlyphFontSize).IsEqualTo(AssignedForegroundGlyphSize);
        await Assert.That(button.Icon).IsEqualTo(SymbolRegular.Save);
        await Assert.That(button.IconData).IsSameReferenceAs(geometry);
        await Assert.That(button.IconHeight).IsEqualTo(AssignedIconHeight);
        await Assert.That(button.IconWidth).IsEqualTo(AssignedIconWidth);
        await Assert.That(button.IsIconFilled).IsTrue();
    }
}
