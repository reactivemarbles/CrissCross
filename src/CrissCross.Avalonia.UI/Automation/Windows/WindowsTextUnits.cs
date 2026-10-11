// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Partitions the rendered document into native accessibility text units.</summary>
internal static class WindowsTextUnits
{
    /// <summary>Gets sorted insertion boundaries for a text unit.</summary>
    /// <param name="editor">The rich editor.</param>
    /// <param name="unit">The requested unit.</param>
    /// <returns>Document boundaries including both document endpoints.</returns>
    internal static List<int> GetBoundaries(RichTextBox editor, WindowsTextUnit unit)
    {
        var text = editor.Document.PlainText;
        var result = new SortedSet<int> { 0, text.Length };
        if (unit == WindowsTextUnit.Character)
        {
            foreach (var offset in StringInfo.ParseCombiningCharacters(text))
            {
                _ = result.Add(offset);
            }
        }
        else if (unit == WindowsTextUnit.Word)
        {
            AddWordBoundaries(text, result);
        }
        else if (unit == WindowsTextUnit.Format)
        {
            AddFormatBoundaries(editor.Document.Segments, 0, result);
        }
        else if (unit is WindowsTextUnit.Line or WindowsTextUnit.Page)
        {
            foreach (var range in editor.GetAutomationUnitRanges(unit == WindowsTextUnit.Page))
            {
                _ = result.Add(range.Start);
                _ = result.Add(range.End);
            }
        }
        else if (unit == WindowsTextUnit.Paragraph)
        {
            AddParagraphBoundaries(editor.Document.Segments, 0, result);
        }
        else if (unit != WindowsTextUnit.Document)
        {
            throw new ArgumentOutOfRangeException(nameof(unit));
        }

        return new(result);
    }

    /// <summary>Gets the index of the enclosing unit, preferring the following unit at boundaries.</summary>
    /// <param name="boundaries">Sorted insertion boundaries.</param>
    /// <param name="offset">The insertion position.</param>
    /// <returns>The index of the enclosing unit.</returns>
    internal static int GetUnitIndex(List<int> boundaries, int offset)
    {
        const int enclosingUnitBoundaryCount = 2;
        var index = boundaries.BinarySearch(offset);
        return Math.Clamp(index < 0 ? ~index - 1 : index, 0, Math.Max(0, boundaries.Count - enclosingUnitBoundaryCount));
    }

    /// <summary>Moves an insertion point through unit boundaries.</summary>
    /// <param name="boundaries">Sorted boundaries.</param>
    /// <param name="offset">The insertion position.</param>
    /// <param name="count">The signed requested movement.</param>
    /// <returns>The resulting position and actual signed movement.</returns>
    internal static (int Offset, int Moved) MoveEndpoint(List<int> boundaries, int offset, int count)
    {
        if (count == 0)
        {
            return (offset, 0);
        }

        var index = boundaries.BinarySearch(offset);
        var origin = index;
        if (index < 0)
        {
            origin = count > 0 ? ~index - 1 : ~index;
        }

        var target = (int)Math.Clamp((long)origin + count, 0, boundaries.Count - 1);
        return (boundaries[target], target - origin);
    }

    /// <summary>Adds Unicode word starts, attaching trailing whitespace to the preceding word.</summary>
    /// <param name="text">The rendered text.</param>
    /// <param name="result">The boundary collection.</param>
    private static void AddWordBoundaries(string text, SortedSet<int> result)
    {
        const int whitespaceCategory = 0;
        const int wordCategory = 1;
        const int otherCategory = 2;
        var elements = StringInfo.ParseCombiningCharacters(text);
        var previous = -1;
        foreach (var offset in elements)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(text, offset);
            var current = whitespaceCategory;
            if (!char.IsWhiteSpace(text, offset))
            {
                current = IsWordCategory(category) ? wordCategory : otherCategory;
            }

            if (current != whitespaceCategory && current != previous)
            {
                _ = result.Add(offset);
            }

            previous = current;
        }
    }

    /// <summary>Identifies letters and numbers that form a word.</summary>
    /// <param name="category">The Unicode category.</param>
    /// <returns>Whether the category belongs to a word.</returns>
    private static bool IsWordCategory(UnicodeCategory category) => category is
        UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter or UnicodeCategory.TitlecaseLetter or
        UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter or UnicodeCategory.DecimalDigitNumber or
        UnicodeCategory.LetterNumber or UnicodeCategory.OtherNumber or UnicodeCategory.ConnectorPunctuation;

    /// <summary>Adds format boundaries, retaining embedded-object boundaries.</summary>
    /// <param name="segments">The source segments.</param>
    /// <param name="offset">The containing document offset.</param>
    /// <param name="result">The boundary collection.</param>
    private static void AddFormatBoundaries(IReadOnlyList<TextSegment> segments, int offset, SortedSet<int> result)
    {
        TextSegment? previous = null;
        foreach (var segment in segments)
        {
            if (previous?.HasSameFormatting(segment) != true || segment.Table is not null || previous?.Table is not null)
            {
                _ = result.Add(offset + segment.StartIndex);
            }

            if (segment.Table is { } table)
            {
                foreach (var cell in table.Cells)
                {
                    AddFormatBoundaries(cell.Segments, offset + segment.StartIndex + cell.StartIndex, result);
                }
            }

            previous = segment;
        }
    }

    /// <summary>Adds paragraph and table-cell boundaries.</summary>
    /// <param name="segments">The source segments.</param>
    /// <param name="offset">The containing document offset.</param>
    /// <param name="result">The boundary collection.</param>
    private static void AddParagraphBoundaries(IReadOnlyList<TextSegment> segments, int offset, SortedSet<int> result)
    {
        foreach (var segment in segments)
        {
            if (segment.IsParagraphBreak || segment.IsPageBreak)
            {
                _ = result.Add(offset + segment.EndIndex);
            }

            if (segment.Table is not { } table)
            {
                continue;
            }

            foreach (var cell in table.Cells)
            {
                var start = offset + segment.StartIndex + cell.StartIndex;
                _ = result.Add(start);
                _ = result.Add(start + cell.Length);
                AddParagraphBoundaries(cell.Segments, start, result);
            }
        }
    }
}
