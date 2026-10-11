// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reserves space for footnotes on their reference page.</summary>
public sealed partial class PagedDocumentPresenter
{
    /// <summary>Determines whether the final or trailing empty sheet is needed.</summary>
    /// <param name="bodyHeight">The remaining body height.</param>
    /// <param name="pageCount">The completed sheet count.</param>
    /// <param name="lines">The measured section.</param>
    /// <returns>Whether a final sheet is needed.</returns>
    private static bool NeedsFinalNotePage(double bodyHeight, int pageCount, List<DocumentPageLayout.MeasuredLine> lines) =>
        bodyHeight > 0 || pageCount == 0 || (lines.Count > 0 && lines[^1].BreakAfter);

    /// <summary>Finds the note height referenced by a line.</summary>
    /// <param name="notes">The measured notes.</param>
    /// <param name="line">The body line.</param>
    /// <returns>The note height.</returns>
    private static double GetLineNoteHeight(List<MeasuredNote> notes, DocumentPageLayout.MeasuredLine line)
    {
        var height = 0.0;
        foreach (var note in notes)
        {
            if (note.Offset >= line.Start && note.Offset < line.End)
            {
                height += note.Height;
            }
        }

        return height;
    }

    /// <summary>Appends the notes referenced by a body line.</summary>
    /// <param name="notes">The measured notes.</param>
    /// <param name="line">The body line.</param>
    /// <param name="html">The note region builder.</param>
    private static void AppendLineNotes(List<MeasuredNote> notes, DocumentPageLayout.MeasuredLine line, StringBuilder html)
    {
        foreach (var note in notes)
        {
            if (note.Offset >= line.Start && note.Offset < line.End)
            {
                _ = html.Append(note.Html);
            }
        }
    }

    /// <summary>Paginates body lines with their footnotes.</summary>
    /// <param name="lines">The measured section body.</param>
    /// <param name="height">The printable height.</param>
    /// <param name="width">The printable width.</param>
    /// <param name="section">The section range.</param>
    /// <returns>The body and note slices.</returns>
    private List<NotePage> CreateNotePages(List<DocumentPageLayout.MeasuredLine> lines, double height, double width, RichDocumentStructure.Section section)
    {
        var notes = MeasureFootnotes(width, section);
        var pages = new List<NotePage>();
        var pageNotes = new StringBuilder();
        var top = 0.0;
        var pageTop = 0.0;
        var bodyHeight = 0.0;
        var noteHeight = 0.0;
        var start = section.Start;
        var end = start;
        foreach (var line in lines)
        {
            var lineNoteHeight = GetLineNoteHeight(notes, line);
            if (bodyHeight > 0 && (line.BreakBefore || bodyHeight + line.Height + noteHeight + lineNoteHeight > height))
            {
                pages.Add(new(new(pageTop, bodyHeight, start, end, bodyHeight + noteHeight > height), pageNotes.ToString(), noteHeight));
                pageTop = top;
                bodyHeight = 0;
                noteHeight = 0;
                _ = pageNotes.Clear();
            }

            if (bodyHeight == 0)
            {
                start = line.Start;
            }

            AppendLineNotes(notes, line, pageNotes);
            noteHeight += lineNoteHeight;
            bodyHeight += line.Height;
            top += line.Height;
            end = line.End;
            if (line.BreakAfter)
            {
                pages.Add(new(new(pageTop, bodyHeight, start, end, bodyHeight + noteHeight > height), pageNotes.ToString(), noteHeight));
                pageTop = top;
                bodyHeight = 0;
                noteHeight = 0;
                start = end;
                _ = pageNotes.Clear();
            }
        }

        if (NeedsFinalNotePage(bodyHeight, pages.Count, lines))
        {
            pages.Add(new(new(pageTop, bodyHeight, start, end, bodyHeight + noteHeight > height), pageNotes.ToString(), noteHeight));
        }

        return pages;
    }

    /// <summary>Measures referenced note bodies at the section width.</summary>
    /// <param name="width">The printable width.</param>
    /// <param name="section">The section range.</param>
    /// <returns>The measured notes.</returns>
    private List<MeasuredNote> MeasureFootnotes(double width, RichDocumentStructure.Section section)
    {
        var notes = new List<MeasuredNote>();
        foreach (var note in RichDocumentFootnotes.Read(Document?.Text ?? string.Empty, section))
        {
            var document = new FlowDocument();
            document.SetText(note.Html);
            var presenter = CreatePresenter();
            presenter.Document = document;
            notes.Add(new(note.Offset, note.Html, presenter.MeasureDocument(width).Height));
        }

        return notes;
    }

    /// <summary>A page body and its bottom notes.</summary>
    /// <param name="Slice">The body slice.</param>
    /// <param name="Html">The footnote region.</param>
    /// <param name="Height">The footnote region height.</param>
    private sealed record NotePage(DocumentPageLayout.PageSlice Slice, string Html, double Height);

    /// <summary>A measured note body.</summary>
    /// <param name="Offset">The reference position.</param>
    /// <param name="Html">The note HTML.</param>
    /// <param name="Height">The rendered height.</param>
    private sealed record MeasuredNote(int Offset, string Html, double Height);
}
