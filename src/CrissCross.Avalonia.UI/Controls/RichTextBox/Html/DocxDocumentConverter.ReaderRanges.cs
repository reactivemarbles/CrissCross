// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Projects crossing Word ranges into balanced editable inline wrappers.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The number of boundaries in a complete annotation range.</summary>
    private const int AnnotationBoundaryCount = 2;

    /// <summary>Finds the independent Word story containing a paragraph.</summary>
    /// <param name="paragraph">The paragraph.</param>
    /// <returns>The story container.</returns>
    private static XElement FindAnnotationStory(XElement paragraph)
    {
        foreach (var ancestor in paragraph.Ancestors())
        {
            if (ancestor.Name.Namespace == Word && ancestor.Name.LocalName is "body" or "hdr" or "ftr" or "footnote" or "endnote")
            {
                return ancestor;
            }
        }

        return paragraph.Parent ?? paragraph;
    }

    /// <summary>Indexes complete, uniquely paired ranges within one story.</summary>
    /// <param name="story">The story container.</param>
    /// <returns>The boundaries safe to project.</returns>
    private static HashSet<XElement> FindCompleteAnnotationRanges(XElement story)
    {
        Dictionary<string, List<XElement>> boundaries = [with(StringComparer.Ordinal)];
        foreach (var element in story.Descendants())
        {
            var key = AnnotationRangeKey(element);
            if (key is null || element.Parent?.Name != Word + "p")
            {
                continue;
            }

            ref var markers = ref CollectionsMarshal.GetValueRefOrAddDefault(boundaries, key, out _);
            markers ??= [];
            markers.Add(element);
        }

        HashSet<XElement> complete = [];
        foreach (var markers in boundaries.Values)
        {
            if (markers.Count != AnnotationBoundaryCount || !IsValidAnnotationStart(markers[0]) || IsAnnotationRangeStart(markers[1]))
            {
                continue;
            }

            _ = complete.Add(markers[0]);
            _ = complete.Add(markers[1]);
        }

        return complete;
    }

    /// <summary>Gets the identity of a supported range boundary.</summary>
    /// <param name="element">The possible boundary.</param>
    /// <returns>The range identity, or null for another element.</returns>
    private static string? AnnotationRangeKey(XElement element) =>
        element.Name.Namespace == Word && Value(element, AnnotationIdentifier) is { Length: > 0 } identifier
            ? element.Name.LocalName switch
            {
                "bookmarkStart" or "bookmarkEnd" => $"bookmark:{identifier}",
                "commentRangeStart" or "commentRangeEnd" => $"comment:{identifier}",
                _ => null,
            }
            : null;

    /// <summary>Identifies a supported range start.</summary>
    /// <param name="element">The boundary.</param>
    /// <returns>Whether the boundary starts a range.</returns>
    private static bool IsAnnotationRangeStart(XElement element) => element.Name.LocalName is "bookmarkStart" or "commentRangeStart";

    /// <summary>Checks that a start has the metadata required by its editable contract.</summary>
    /// <param name="element">The possible start.</param>
    /// <returns>Whether the start is supported.</returns>
    private static bool IsValidAnnotationStart(XElement element) =>
        IsAnnotationRangeStart(element)
        && (element.Name.LocalName != AnnotationBookmarkStart || Value(element, "name") is { Length: > 0 });

    /// <summary>Updates active ranges for a complete boundary.</summary>
    /// <param name="element">The possible boundary.</param>
    /// <param name="complete">The complete boundaries.</param>
    /// <param name="active">The active starts.</param>
    /// <returns>Whether the boundary was handled.</returns>
    private static bool UpdateAnnotationRange(
        XElement element,
        HashSet<XElement> complete,
        Dictionary<string, XElement> active)
    {
        if (!complete.Contains(element) || AnnotationRangeKey(element) is not { } key)
        {
            return false;
        }

        if (IsAnnotationRangeStart(element))
        {
            active[key] = element;
        }
        else
        {
            _ = active.Remove(key);
        }

        return true;
    }

    /// <summary>Restores the active ranges at the start of a paragraph.</summary>
    /// <param name="paragraph">The paragraph.</param>
    /// <param name="complete">The complete boundaries.</param>
    /// <returns>The active starts.</returns>
    private static Dictionary<string, XElement> FindActiveAnnotationRanges(
        XElement paragraph,
        HashSet<XElement> complete)
    {
        Dictionary<string, XElement> active = [with(StringComparer.Ordinal)];
        foreach (var element in FindAnnotationStory(paragraph).Descendants())
        {
            if (ReferenceEquals(element, paragraph))
            {
                break;
            }

            _ = UpdateAnnotationRange(element, complete, active);
        }

        return active;
    }

    /// <summary>Writes balanced wrappers for the active range starts.</summary>
    /// <param name="active">The active starts.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="parts">The package.</param>
    /// <param name="path">The story path.</param>
    /// <param name="rels">The story relationships.</param>
    /// <returns>The closing tags in reverse nesting order.</returns>
    private static Stack<string> OpenAnnotationRanges(
        Dictionary<string, XElement> active,
        StringBuilder output,
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels)
    {
        var closing = new Stack<string>();
        foreach (var marker in active.Values)
        {
            if (marker.Name.LocalName == AnnotationBookmarkStart)
            {
                _ = output.Append("<span");
                Attribute(output, "data-bookmark", Value(marker, "name"));
                closing.Push("</span>");
            }
            else
            {
                ReadCommentStart(marker, output, parts, path, rels);
                closing.Push("</mark>");
            }

            _ = output.Append('>');
        }

        return closing;
    }
}
