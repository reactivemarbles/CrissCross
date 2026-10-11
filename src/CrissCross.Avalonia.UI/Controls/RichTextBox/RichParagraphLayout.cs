// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Groups flat document segments into independently aligned paragraphs.</summary>
internal static class RichParagraphLayout
{
    /// <summary>Partitions segments without changing their editable offsets.</summary>
    /// <param name="segments">The document segments.</param>
    /// <returns>The paragraphs in document order.</returns>
    internal static List<ParagraphContent> Create(IReadOnlyList<TextSegment> segments)
    {
        var paragraphs = new List<ParagraphContent>();
        var content = new List<TextSegment>();
        foreach (var segment in segments)
        {
            if (segment.IsParagraphBreak)
            {
                paragraphs.Add(new(content, segment));
                content = [];
            }
            else
            {
                content.Add(segment);
            }
        }

        if (content.Count > 0)
        {
            paragraphs.Add(new(content, null));
        }

        return paragraphs;
    }

    /// <summary>Determines whether paragraphs require independent text layouts.</summary>
    /// <param name="paragraphs">The document paragraphs.</param>
    /// <returns>Whether paragraph alignments differ.</returns>
    internal static bool HasMixedAlignment(IReadOnlyList<ParagraphContent> paragraphs)
    {
        for (var index = 1; index < paragraphs.Count; index++)
        {
            if (paragraphs[index].Alignment != paragraphs[0].Alignment)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>A paragraph's rich content and terminating document break.</summary>
    internal sealed class ParagraphContent
    {
        /// <summary>Initializes a new instance of the <see cref="ParagraphContent"/> class.</summary>
        /// <param name="segments">The paragraph's content segments.</param>
        /// <param name="paragraphBreak">The optional terminating break.</param>
        internal ParagraphContent(IReadOnlyList<TextSegment> segments, TextSegment? paragraphBreak)
        {
            Segments = segments;
            ParagraphBreak = paragraphBreak;
            Start = segments.Count > 0 ? segments[0].StartIndex : paragraphBreak?.StartIndex ?? 0;
            End = segments.Count > 0 ? segments[^1].EndIndex : Start;
            Alignment = paragraphBreak?.ParagraphAlignment ?? FindAlignment(segments);
        }

        /// <summary>Gets the content segments.</summary>
        internal IReadOnlyList<TextSegment> Segments { get; }

        /// <summary>Gets the terminating break.</summary>
        internal TextSegment? ParagraphBreak { get; }

        /// <summary>Gets the first editable offset.</summary>
        internal int Start { get; }

        /// <summary>Gets the exclusive content end offset.</summary>
        internal int End { get; }

        /// <summary>Gets the independent paragraph alignment.</summary>
        internal TextAlignment Alignment { get; }

        /// <summary>Clones paragraph segments into local editable coordinates.</summary>
        /// <returns>The local rich segments.</returns>
        internal List<TextSegment> CreateLocalSegments()
        {
            var result = new List<TextSegment>(Segments.Count);
            foreach (var segment in Segments)
            {
                var clone = segment.Clone();
                clone.StartIndex -= Start;
                result.Add(clone);
            }

            return result;
        }

        /// <summary>Finds alignment metadata within paragraph content.</summary>
        /// <param name="segments">The paragraph segments.</param>
        /// <returns>The specified alignment or the default left alignment.</returns>
        private static TextAlignment FindAlignment(IReadOnlyList<TextSegment> segments)
        {
            foreach (var segment in segments)
            {
                if (segment.ParagraphAlignment is { } alignment)
                {
                    return alignment;
                }
            }

            return TextAlignment.Left;
        }
    }
}
