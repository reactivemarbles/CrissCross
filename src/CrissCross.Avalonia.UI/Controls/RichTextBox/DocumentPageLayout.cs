// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Partitions measured lines without cutting through text or embedded objects.</summary>
internal static class DocumentPageLayout
{
    /// <summary>Creates page slices from measured document lines.</summary>
    /// <param name="lines">Measured lines in document order.</param>
    /// <param name="contentHeight">Available height inside each sheet.</param>
    /// <returns>At least one page, with oversized lines preserved on expanded sheets.</returns>
    internal static List<PageSlice> Create(IReadOnlyList<MeasuredLine> lines, double contentHeight)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (!double.IsFinite(contentHeight) || contentHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(contentHeight));
        }

        var pages = new List<PageSlice>();
        var top = 0.0;
        var pageTop = 0.0;
        var height = 0.0;
        var start = 0;
        var end = 0;
        foreach (var line in lines)
        {
            ValidateLine(line);
            if (ShouldStartPage(line, height, contentHeight))
            {
                pages.Add(new(pageTop, height, start, end, height > contentHeight));
                pageTop = top;
                height = 0;
                start = line.Start;
            }

            if (height == 0)
            {
                start = line.Start;
            }

            height += line.Height;
            top += line.Height;
            end = line.End;
            if (line.BreakAfter)
            {
                pages.Add(new(pageTop, height, start, end, height > contentHeight));
                pageTop = top;
                height = 0;
                start = end;
            }
        }

        if (NeedsFinalPage(lines, pages.Count, height))
        {
            pages.Add(new(pageTop, height, start, end, height > contentHeight));
        }

        return pages;
    }

    /// <summary>Validates measured geometry before partitioning.</summary>
    /// <param name="line">The measured line.</param>
    private static void ValidateLine(MeasuredLine line)
    {
        if (double.IsFinite(line.Height) && line.Height >= 0 && line.Start >= 0 && line.End >= line.Start)
        {
            return;
        }

        throw new ArgumentException("Measured lines must have finite nonnegative height and ordered offsets.", nameof(line));
    }

    /// <summary>Determines whether a line begins a new sheet.</summary>
    /// <param name="line">The next line.</param>
    /// <param name="height">The accumulated sheet height.</param>
    /// <param name="contentHeight">The available sheet height.</param>
    /// <returns>Whether to finish the preceding sheet.</returns>
    private static bool ShouldStartPage(MeasuredLine line, double height, double contentHeight) =>
        height > 0 && (line.BreakBefore || height + line.Height > contentHeight);

    /// <summary>Determines whether a final sheet is required.</summary>
    /// <param name="lines">The measured lines.</param>
    /// <param name="pageCount">The completed sheet count.</param>
    /// <param name="height">The accumulated final sheet height.</param>
    /// <returns>Whether to append the final sheet.</returns>
    private static bool NeedsFinalPage(IReadOnlyList<MeasuredLine> lines, int pageCount, double height) =>
        height > 0 || pageCount == 0 || (lines.Count > 0 && lines[^1].BreakAfter);

    /// <summary>A measured text line, including any atomic embedded object.</summary>
    /// <param name="Start">The first document offset.</param>
    /// <param name="End">The exclusive last document offset.</param>
    /// <param name="Height">The measured line height.</param>
    /// <param name="BreakBefore">Whether to begin a new sheet before the line.</param>
    /// <param name="BreakAfter">Whether to begin a new sheet after the line.</param>
    internal readonly record struct MeasuredLine(int Start, int End, double Height, bool BreakBefore = false, bool BreakAfter = false);

    /// <summary>A contiguous vertical slice of the measured document.</summary>
    /// <param name="DocumentTop">The vertical position in the unpaged document.</param>
    /// <param name="Height">The slice height.</param>
    /// <param name="Start">The first document offset.</param>
    /// <param name="End">The exclusive last document offset.</param>
    /// <param name="HasOverflow">Whether the slice exceeds the nominal sheet height.</param>
    internal readonly record struct PageSlice(double DocumentTop, double Height, int Start, int End, bool HasOverflow);
}
