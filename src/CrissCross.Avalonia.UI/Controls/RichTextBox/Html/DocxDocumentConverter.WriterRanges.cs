// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Xml.Linq;
using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Coalesces editor leaf annotations into one native Word range per logical identifier.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>Tracks range boundaries across separately formatted leaves.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>The number of leaves in each logical bookmark.</summary>
        private readonly Dictionary<string, int> _bookmarkCounts = [with(StringComparer.Ordinal)];

        /// <summary>The number of leaves in each logical comment.</summary>
        private readonly Dictionary<string, int> _commentCounts = [with(StringComparer.Ordinal)];

        /// <summary>The assigned bookmark identifiers and remaining leaves.</summary>
        private readonly Dictionary<string, (string Id, int Remaining)> _bookmarkRanges = [with(StringComparer.Ordinal)];

        /// <summary>The remaining leaves for each assigned native comment identifier.</summary>
        private readonly Dictionary<string, int> _remainingComments = [with(StringComparer.Ordinal)];

        /// <summary>Counts each logical identifier in a semantic annotation attribute.</summary>
        /// <param name="body">The HTML body.</param>
        /// <param name="attribute">The annotation attribute.</param>
        /// <param name="counts">The counts destination.</param>
        private static void CountRanges(IElement body, string attribute, Dictionary<string, int> counts)
        {
            foreach (var element in body.QuerySelectorAll($"[{attribute}]"))
            {
                if (element.GetAttribute(attribute) is { } identifier)
                {
                    counts[identifier] = counts.GetValueOrDefault(identifier) + 1;
                }
            }
        }

        /// <summary>Counts annotation leaves before exporting their boundaries.</summary>
        /// <param name="body">The HTML body.</param>
        private void PrepareRanges(IElement body)
        {
            CountRanges(body, "data-bookmark", _bookmarkCounts);
            CountRanges(body, "data-comment-id", _commentCounts);
        }

        /// <summary>Begins the first leaf of a logical bookmark.</summary>
        /// <param name="name">The bookmark name.</param>
        /// <param name="target">The Word parent.</param>
        /// <returns>The native bookmark identifier.</returns>
        private string? BeginBookmark(string? name, XElement target)
        {
            if (name is null)
            {
                return null;
            }

            if (_bookmarkRanges.TryGetValue(name, out var existing))
            {
                return existing.Id;
            }

            var identifier = NextIdentifier();
            _bookmarkRanges.Add(name, (identifier, _bookmarkCounts.GetValueOrDefault(name, 1)));
            target.Add(new XElement(
                Word + "bookmarkStart",
                new XAttribute(Word + AnnotationIdentifier, identifier),
                new XAttribute(Word + "name", name)));
            return identifier;
        }

        /// <summary>Closes the final leaf of a logical bookmark.</summary>
        /// <param name="name">The bookmark name.</param>
        /// <param name="identifier">The native identifier.</param>
        /// <param name="target">The Word parent.</param>
        private void EndBookmark(string? name, string? identifier, XElement target)
        {
            if (name is null || identifier is null)
            {
                return;
            }

            var remaining = _bookmarkRanges[name].Remaining - 1;
            _bookmarkRanges[name] = (identifier, remaining);
            if (remaining != 0)
            {
                return;
            }

            target.Add(new XElement(Word + "bookmarkEnd", new XAttribute(Word + AnnotationIdentifier, identifier)));
        }

        /// <summary>Begins a logical comment once, before its first leaf.</summary>
        /// <param name="numeric">The native comment identifier.</param>
        /// <param name="target">The Word parent.</param>
        private void StartCommentRange(string numeric, XElement target)
        {
            if (_remainingComments.ContainsKey(numeric))
            {
                return;
            }

            var count = 1;
            foreach (var identifier in _commentIdentifiers)
            {
                if (identifier.Value == numeric)
                {
                    count = _commentCounts.GetValueOrDefault(identifier.Key, 1);
                }
            }

            _remainingComments.Add(numeric, count);
            target.Add(new XElement(Word + "commentRangeStart", new XAttribute(Word + AnnotationIdentifier, numeric)));
        }

        /// <summary>Closes a logical comment after its last leaf and emits one reference.</summary>
        /// <param name="numeric">The native comment identifier.</param>
        /// <param name="target">The Word parent.</param>
        private void FinishCommentRange(string numeric, XElement target)
        {
            var remaining = _remainingComments[numeric] - 1;
            _remainingComments[numeric] = remaining;
            if (remaining != 0)
            {
                return;
            }

            target.Add(
                new XElement(Word + "commentRangeEnd", new XAttribute(Word + AnnotationIdentifier, numeric)),
                new XElement(
                    Word + "r",
                    new XElement(Word + "commentReference", new XAttribute(Word + AnnotationIdentifier, numeric))));
        }
    }
}
