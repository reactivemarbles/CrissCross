// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Controls.Automation.Windows;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies Unicode insertion boundaries and native signed movement semantics.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class WindowsTextUnitsTests
{
    /// <summary>The end offset of the first text unit.</summary>
    private const int FirstUnitEnd = 4;

    /// <summary>The end offset of the document.</summary>
    private const int DocumentEnd = 9;

    /// <summary>An offset inside the first text unit.</summary>
    private const int InteriorOffset = 2;

    /// <summary>The number of text units in the boundary fixtures.</summary>
    private const int UnitCount = 2;

    /// <summary>Treats combining marks and emoji sequences as complete characters.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Character_CombiningAndJoinedEmoji_PreservesGraphemes()
    {
        const int boundaryCount = 4;
        const int combiningCharacterEnd = 2;
        var editor = new RichTextBox { Text = "a\u0301👩‍👩‍👧‍👦z" };
        var boundaries = WindowsTextUnits.GetBoundaries(editor, WindowsTextUnit.Character);
        await Assert.That(boundaries.Count).IsEqualTo(boundaryCount);
        await Assert.That(boundaries[1]).IsEqualTo(combiningCharacterEnd);
        await Assert.That(boundaries[^1]).IsEqualTo(editor.Document.Length);
    }

    /// <summary>Counts movement to the first boundary when starting inside a unit.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task MoveEndpoint_InsideUnit_CountsFirstReachedBoundary()
    {
        var boundaries = new List<int> { 0, FirstUnitEnd, DocumentEnd };
        var forward = WindowsTextUnits.MoveEndpoint(boundaries, InteriorOffset, 1);
        var backward = WindowsTextUnits.MoveEndpoint(boundaries, InteriorOffset, -1);
        await Assert.That(forward.Offset).IsEqualTo(FirstUnitEnd);
        await Assert.That(forward.Moved).IsEqualTo(1);
        await Assert.That(backward.Offset).IsEqualTo(0);
        await Assert.That(backward.Moved).IsEqualTo(-1);
    }

    /// <summary>Clamps large signed requests without integer overflow.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task MoveEndpoint_ExtremeCounts_ReportsActualMovement()
    {
        var boundaries = new List<int> { 0, FirstUnitEnd, DocumentEnd };
        var forward = WindowsTextUnits.MoveEndpoint(boundaries, 0, int.MaxValue);
        var backward = WindowsTextUnits.MoveEndpoint(boundaries, DocumentEnd, int.MinValue);
        await Assert.That(forward.Offset).IsEqualTo(DocumentEnd);
        await Assert.That(forward.Moved).IsEqualTo(UnitCount);
        await Assert.That(backward.Offset).IsEqualTo(0);
        await Assert.That(backward.Moved).IsEqualTo(-UnitCount);
    }

    /// <summary>Normalizes a boundary to the following unit and the document end to the final unit.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task EnclosingUnit_ExactBoundary_ChoosesFollowingUnit()
    {
        var boundaries = new List<int> { 0, FirstUnitEnd, DocumentEnd };
        await Assert.That(WindowsTextUnits.GetUnitIndex(boundaries, FirstUnitEnd)).IsEqualTo(1);
        await Assert.That(WindowsTextUnits.GetUnitIndex(boundaries, DocumentEnd)).IsEqualTo(1);
    }

    /// <summary>Attaches whitespace to preceding words while retaining punctuation boundaries.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Word_PunctuationAndWhitespace_ExposesReadingUnits()
    {
        const int secondWordStart = 7;
        const int punctuationStart = 12;
        const int finalWordStart = 14;
        const int whitespaceStart = 5;
        var editor = new RichTextBox { Text = "hello  world! next" };
        var boundaries = WindowsTextUnits.GetBoundaries(editor, WindowsTextUnit.Word);
        await Assert.That(boundaries.Contains(secondWordStart)).IsTrue();
        await Assert.That(boundaries.Contains(punctuationStart)).IsTrue();
        await Assert.That(boundaries.Contains(finalWordStart)).IsTrue();
        await Assert.That(boundaries.Contains(whitespaceStart)).IsFalse();
    }
}
