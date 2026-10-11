// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Exposes measured text lines in document reading order.</summary>
public partial class FormattedTextPresenter
{
    /// <summary>Gets formatted line ranges, including independently wrapped table cells.</summary>
    /// <returns>The document positions of measured lines.</returns>
    internal List<(int Start, int End)> GetAutomationLineRanges()
    {
        var result = new List<(int Start, int End)>();
        foreach (var line in TextLayout.TextLines)
        {
            var range = FindParagraphRange(line.FirstTextSourceIndex, line.Length);
            if (range is { Child: FormattedTextPresenter paragraph } paragraphRange)
            {
                AppendAutomationLines(result, paragraph, paragraphRange.DocumentStart);
                if (result.Count > 0)
                {
                    result[^1] = (result[^1].Start, GetDocumentOffset(line.FirstTextSourceIndex + line.Length));
                }
            }
            else if (range is { Child: Grid grid, Table: { } table } tableRange)
            {
                for (var index = 0; index < table.Cells.Count; index++)
                {
                    if (grid.Children[index] is Border { Child: FormattedTextPresenter cell })
                    {
                        AppendAutomationLines(result, cell, tableRange.DocumentStart + table.Cells[index].StartIndex);
                    }
                }
            }
            else
            {
                result.Add((GetDocumentOffset(line.FirstTextSourceIndex), GetDocumentOffset(line.FirstTextSourceIndex + line.Length)));
            }
        }

        return result;
    }

    /// <summary>Translates nested measured lines to document positions.</summary>
    /// <param name="result">The destination ranges.</param>
    /// <param name="presenter">The nested formatter.</param>
    /// <param name="offset">Its document start.</param>
    private static void AppendAutomationLines(List<(int Start, int End)> result, FormattedTextPresenter presenter, int offset)
    {
        foreach (var line in presenter.GetAutomationLineRanges())
        {
            result.Add((offset + line.Start, offset + line.End));
        }
    }
}
