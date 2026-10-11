// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies inherited formatting toggles preserve adjacent content and document structure.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxFormattingToggleTests
{
    /// <summary>The middle selection start and length.</summary>
    private const int MiddleOffset = 2;

    /// <summary>The alphabet fixture length.</summary>
    private const int AlphabetLength = 6;

    /// <summary>Removes effective formatting from a subset and then restores it.</summary>
    /// <param name="tag">The inherited formatting element.</param>
    /// <param name="format">The formatting command.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments("strong", TextFormatType.Bold)]
    [Arguments("em", TextFormatType.Italic)]
    [Arguments("u", TextFormatType.Underline)]
    [Arguments("s", TextFormatType.Strikethrough)]
    public async Task ToggleFormatting_InheritedSubset_PreservesAdjacentFormatting(string tag, TextFormatType format)
    {
        var document = new FlowDocument();
        document.SetText($"<{tag}>abcdef</{tag}>");

        var applied = document.ToggleFormatting(MiddleOffset, MiddleOffset, format);

        await Assert.That(applied).IsFalse();
        await Assert.That(document.Length).IsEqualTo(AlphabetLength);
        await Assert.That(IsFormatted(document, "ab", format)).IsTrue();
        await Assert.That(IsFormatted(document, "cd", format)).IsFalse();
        await Assert.That(IsFormatted(document, "ef", format)).IsTrue();
        await Assert.That(document.ToggleFormatting(MiddleOffset, MiddleOffset, format)).IsTrue();
        await Assert.That(IsFormatted(document, "cd", format)).IsTrue();
    }

    /// <summary>Removes formatting inherited through multiple nested containers.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ToggleFormatting_FullInheritedSelection_RemovesEffectiveFormatting()
    {
        var document = new FlowDocument();
        document.SetText("<strong><em>ab</em><a href=\"https://example.com\">cd</a>ef</strong>");

        await Assert.That(document.ToggleFormatting(0, AlphabetLength, TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "ab", TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "ef", TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "ab", TextFormatType.Italic)).IsTrue();
        await Assert.That(document.Text).Contains("href=\"https://example.com\"");
    }

    /// <summary>Overrides word processor styles without removing other decorations.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ToggleFormatting_InlineStyles_PreservesOtherDecoration()
    {
        var document = new FlowDocument();
        document.SetText("<span style=\"font-weight:bold;font-style:italic;text-decoration:underline line-through\">abcdef</span>");

        await Assert.That(document.ToggleFormatting(MiddleOffset, MiddleOffset, TextFormatType.Underline)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Underline)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Strikethrough)).IsTrue();
        await Assert.That(document.ToggleFormatting(MiddleOffset, MiddleOffset, TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Italic)).IsTrue();
        await Assert.That(IsFormatted(document, "ab", TextFormatType.Bold)).IsTrue();
    }

    /// <summary>Formats leaves across table cells while retaining their containers.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ToggleFormatting_TableAndImage_PreservesStructure()
    {
        var document = new FlowDocument();
        document.SetText("<table><tr><td><a href=\"https://example.com\">ab</a></td><td><img src=\"file:///tmp/photo.png\">cd</td></tr></table>");

        await Assert.That(document.ToggleFormatting(0, document.Length, TextFormatType.Italic)).IsTrue();
        await Assert.That(IsFormatted(document, "ab", TextFormatType.Italic)).IsTrue();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Italic)).IsTrue();
        await Assert.That(document.Text).Contains("<table><tr><td><a");
        await Assert.That(document.Text).Contains("</a></td><td>");
        await Assert.That(document.Text).Contains("src=\"file:///tmp/photo.png\"");
        await Assert.That(document.ToggleFormatting(0, document.Length, TextFormatType.Italic)).IsFalse();
        await Assert.That(IsFormatted(document, "ab", TextFormatType.Italic)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Italic)).IsFalse();
    }

    /// <summary>Removes bold from formatted table cells even when an image is selected.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ToggleFormatting_AllBoldTableCells_RemovesBold()
    {
        var document = new FlowDocument();
        document.SetText("<table><tr><td><strong>ab</strong></td><td><strong><img src=\"file:///tmp/photo.png\">cd</strong></td></tr></table>");

        await Assert.That(document.ToggleFormatting(0, document.Length, TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "ab", TextFormatType.Bold)).IsFalse();
        await Assert.That(IsFormatted(document, "cd", TextFormatType.Bold)).IsFalse();
        await Assert.That(document.Text).Contains("<table><tr><td>");
        await Assert.That(document.Text).Contains("src=\"file:///tmp/photo.png\"");
    }

    /// <summary>Checks the effective formatting of a particular text leaf.</summary>
    /// <param name="document">The rendered document.</param>
    /// <param name="text">The leaf text.</param>
    /// <param name="format">The requested formatting.</param>
    /// <returns>Whether the matching leaf has the requested formatting.</returns>
    private static bool IsFormatted(FlowDocument document, string text, TextFormatType format)
    {
        foreach (var segment in GetTextLeaves(document.Segments))
        {
            if (segment.Text == text)
            {
                return format switch
                {
                    TextFormatType.Bold => segment.IsBold,
                    TextFormatType.Italic => segment.IsItalic,
                    TextFormatType.Underline => segment.IsUnderline,
                    TextFormatType.Strikethrough => segment.IsStrikethrough,
                    _ => false,
                };
            }
        }

        throw new InvalidOperationException($"Missing text leaf: {text}");
    }

    /// <summary>Enumerates leaves inside document table cells.</summary>
    /// <param name="segments">The document segments.</param>
    /// <returns>The text leaves in source order.</returns>
    private static IEnumerable<TextSegment> GetTextLeaves(IReadOnlyList<TextSegment> segments)
    {
        foreach (var segment in segments)
        {
            if (segment.Table is { } table)
            {
                foreach (var cell in table.Cells)
                {
                    foreach (var leaf in GetTextLeaves(cell.Segments))
                    {
                        yield return leaf;
                    }
                }
            }
            else
            {
                yield return segment;
            }
        }
    }
}
