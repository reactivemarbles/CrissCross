// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests measured line partitioning for document sheets.</summary>
public sealed class DocumentPageLayoutTests
{
    /// <summary>The first fixture line end.</summary>
    private const int FirstLineEnd = 5;

    /// <summary>The second fixture line end.</summary>
    private const int SecondLineEnd = 10;

    /// <summary>The third fixture line end.</summary>
    private const int ThirdLineEnd = 15;

    /// <summary>The first fixture line height.</summary>
    private const double FirstLineHeight = 12;

    /// <summary>The second fixture line height.</summary>
    private const double SecondLineHeight = 18;

    /// <summary>The third fixture line height.</summary>
    private const double ThirdLineHeight = 11;

    /// <summary>The height fitting the first two fixture lines.</summary>
    private const double TwoLineContentHeight = 30;

    /// <summary>The expected two-sheet count.</summary>
    private const int TwoSheets = 2;

    /// <summary>The expected three-sheet count.</summary>
    private const int ThreeSheets = 3;

    /// <summary>The measured oversized object height.</summary>
    private const double ObjectHeight = 90;

    /// <summary>The short nominal content area.</summary>
    private const double ShortContentHeight = 40;

    /// <summary>The normal fixture line height.</summary>
    private const double NormalLineHeight = 10;

    /// <summary>The roomy nominal content area.</summary>
    private const double RoomyContentHeight = 100;

    /// <summary>The first explicit break offset.</summary>
    private const int FirstBreakOffset = 3;

    /// <summary>The second explicit break offset.</summary>
    private const int SecondBreakOffset = 6;

    /// <summary>The final explicit break offset.</summary>
    private const int FinalBreakOffset = 9;

    /// <summary>Partitions at the last complete line that fits a sheet.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Create_MixedLineHeights_PreservesCompleteLines()
    {
        DocumentPageLayout.MeasuredLine[] lines =
        [
            new(0, FirstLineEnd, FirstLineHeight),
            new(FirstLineEnd, SecondLineEnd, SecondLineHeight),
            new(SecondLineEnd, ThirdLineEnd, ThirdLineHeight),
        ];
        var pages = DocumentPageLayout.Create(lines, TwoLineContentHeight);
        await Assert.That(pages.Count).IsEqualTo(TwoSheets);
        await Assert.That(pages[0].Height).IsEqualTo(TwoLineContentHeight);
        await Assert.That(pages[1].DocumentTop).IsEqualTo(TwoLineContentHeight);
        await Assert.That(pages[1].Start).IsEqualTo(SecondLineEnd);
        await Assert.That(pages[1].End).IsEqualTo(ThirdLineEnd);
    }

    /// <summary>Preserves indivisible oversized objects rather than clipping them.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Create_OversizedObject_ExpandsOneSheet()
    {
        DocumentPageLayout.MeasuredLine[] lines = [new(0, 1, ObjectHeight), new(1, FirstBreakOffset, NormalLineHeight)];
        var pages = DocumentPageLayout.Create(lines, ShortContentHeight);
        await Assert.That(pages.Count).IsEqualTo(TwoSheets);
        await Assert.That(pages[0].HasOverflow).IsTrue();
        await Assert.That(pages[0].Height).IsEqualTo(ObjectHeight);
        await Assert.That(pages[1].DocumentTop).IsEqualTo(ObjectHeight);
    }

    /// <summary>Honors explicit breaks even when the next line fits.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Create_ExplicitBreaks_StartsNewSheets()
    {
        DocumentPageLayout.MeasuredLine[] lines =
        [
            new(0, FirstBreakOffset, NormalLineHeight, BreakAfter: true),
            new(FirstBreakOffset, SecondBreakOffset, NormalLineHeight, BreakBefore: true),
            new(SecondBreakOffset, FinalBreakOffset, NormalLineHeight, BreakAfter: true),
        ];
        var pages = DocumentPageLayout.Create(lines, RoomyContentHeight);
        await Assert.That(pages.Count).IsEqualTo(ThreeSheets);
        await Assert.That(pages[0].End).IsEqualTo(FirstBreakOffset);
        await Assert.That(pages[1].Start).IsEqualTo(FirstBreakOffset);
        await Assert.That(pages[TwoSheets].Start).IsEqualTo(FinalBreakOffset);
        await Assert.That(pages[TwoSheets].Height).IsEqualTo(0);
    }

    /// <summary>Provides one sheet for an empty document.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Create_EmptyDocument_ReturnsOneSheet()
    {
        var pages = DocumentPageLayout.Create([], RoomyContentHeight);
        await Assert.That(pages.Count).IsEqualTo(1);
        await Assert.That(pages[0].Start).IsEqualTo(0);
    }

    /// <summary>Rejects dimensions that cannot produce a bounded pagination result.</summary>
    /// <param name="height">An invalid content height.</param>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    public async Task Create_InvalidHeight_Throws(double height) =>
        await Assert.That(() => DocumentPageLayout.Create([], height)).Throws<ArgumentOutOfRangeException>();

    /// <summary>Rejects invalid measured lines before attempting pagination.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Create_InvalidMeasuredHeight_Throws()
    {
        DocumentPageLayout.MeasuredLine[] lines = [new(0, 1, double.NaN)];
        await Assert.That(() => DocumentPageLayout.Create(lines, RoomyContentHeight)).Throws<ArgumentException>();
    }
}
