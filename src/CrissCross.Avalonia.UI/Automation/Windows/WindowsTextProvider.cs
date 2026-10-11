// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Automation.Peers;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Exposes the rich document through the native Windows text pattern.</summary>
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Windows UI Automation uses built-in COM callable wrappers and VARIANT marshaling.")]
[RequiresUnreferencedCode("Built-in COM callable wrappers require the public provider interface metadata.")]
internal sealed class WindowsTextProvider : IWindowsTextProvider
{
    /// <summary>Stores the _node value.</summary>
    private readonly WindowsAutomationNode _node;

    /// <summary>Initializes a new instance of the <see cref="WindowsTextProvider"/> class.</summary>
    /// <param name="node">The document provider node.</param>
    internal WindowsTextProvider(WindowsAutomationNode node) => _node = node;

    /// <summary>Gets the associated rich document.</summary>
    private RichTextBox Editor => _node.Editor ?? throw new InvalidOperationException("The node does not own a rich document.");

    /// <inheritdoc/>
    public IWindowsTextRangeProvider GetDocumentRange() => _node.Context.Invoke<IWindowsTextRangeProvider>(() => CreateRange(0, Editor.Document.Length));

    /// <inheritdoc/>
    public WindowsSupportedTextSelection GetSupportedTextSelection() => _node.Context.Invoke(static () => WindowsSupportedTextSelection.Single);

    /// <inheritdoc/>
    public IWindowsTextRangeProvider[] GetSelection() => _node.Context.Invoke<IWindowsTextRangeProvider[]>(() => [CreateRange(Editor.SelectionStart, Editor.SelectionEnd)]);

    /// <inheritdoc/>
    public IWindowsTextRangeProvider[] GetVisibleRanges() => _node.Context.Invoke(GetVisibleRangesCore);

    /// <inheritdoc/>
    public IWindowsTextRangeProvider RangeFromChild(IWindowsRawElementProviderSimple childElement) => _node.Context.Invoke<IWindowsTextRangeProvider>(() => RangeFromChildCore(childElement));

    /// <inheritdoc/>
    public IWindowsTextRangeProvider RangeFromPoint(WindowsAutomationPoint point) => _node.Context.Invoke<IWindowsTextRangeProvider>(() =>
    {
        var offset = Editor.GetAutomationDocumentPosition(_node.Context.ToRootPoint(point.X, point.Y));
        return CreateRange(offset, offset);
    });

    /// <summary>Rounds an insertion offset to a grapheme boundary.</summary>
    /// <param name="boundaries">The document boundaries.</param>
    /// <param name="offset">The insertion offset.</param>
    /// <param name="roundUp">Whether to round toward the range end.</param>
    /// <returns>The boundary index.</returns>
    private static int GetBoundaryIndex(List<int> boundaries, int offset, bool roundUp)
    {
        var index = boundaries.BinarySearch(offset);
        if (index >= 0)
        {
            return index;
        }

        return roundUp ? ~index : ~index - 1;
    }

    /// <summary>Creates a range for this document.</summary>
    /// <param name="start">The first insertion position.</param>
    /// <param name="end">The last insertion position.</param>
    /// <returns>The native range.</returns>
    private WindowsTextRangeProvider CreateRange(int start, int end) => new(_node, start, end);

    /// <summary>Maps a semantic document descendant to its rendered range.</summary>
    /// <param name="child">The semantic descendant.</param>
    /// <returns>The descendant text range.</returns>
    private WindowsTextRangeProvider RangeFromChildCore(IWindowsRawElementProviderSimple child)
    {
        if (child is not WindowsAutomationNode node || node.Context != _node.Context
            || node.Peer is not RichTextSemanticAutomationPeer peer || !ReferenceEquals(peer.Editor, Editor))
        {
            throw new ArgumentException("The element is not a descendant of this text provider.", nameof(child));
        }

        return CreateRange(peer.DocumentStart, peer.DocumentEnd);
    }

    /// <summary>Collects visible grapheme intervals from a single measured document query.</summary>
    /// <returns>The visible ranges in document order.</returns>
    private IWindowsTextRangeProvider[] GetVisibleRangesCore()
    {
        if (Editor.Document.Length == 0)
        {
            return Editor.IsAutomationCaretVisible() ? [CreateRange(0, 0)] : [];
        }

        var rectangles = Editor.GetAutomationVisibleRectangles(0, Editor.Document.Length);
        var boundaries = WindowsTextUnits.GetBoundaries(Editor, WindowsTextUnit.Character);
        var intervals = new List<(int Start, int End)>();
        foreach (var rectangle in rectangles)
        {
            var interval = MapVisibleRectangle(rectangle, boundaries);
            if (interval.End > interval.Start)
            {
                intervals.Add(interval);
            }
        }

        intervals.Sort(static (left, right) => left.Start.CompareTo(right.Start));
        return CreateVisibleRanges(intervals);
    }

    /// <summary>Maps a clipped line rectangle and checks only its boundary characters.</summary>
    /// <param name="rectangle">The visible root rectangle.</param>
    /// <param name="boundaries">The document grapheme boundaries.</param>
    /// <returns>The visible insertion interval.</returns>
    private (int Start, int End) MapVisibleRectangle(in global::Avalonia.Rect rectangle, List<int> boundaries)
    {
        const double centerFactor = 0.5;
        var middle = rectangle.Top + (rectangle.Height * centerFactor);
        var left = Editor.GetAutomationDocumentPosition(new(rectangle.Left, middle));
        var right = Editor.GetAutomationDocumentPosition(new(rectangle.Right, middle));
        var first = GetBoundaryIndex(boundaries, Math.Min(left, right), false);
        var last = GetBoundaryIndex(boundaries, Math.Max(left, right), true);
        first = Math.Max(0, first - 1);
        last = Math.Min(boundaries.Count - 1, last + 1);
        while (first < last && !IsVisibleGrapheme(boundaries, first))
        {
            first++;
        }

        while (last > first && !IsVisibleGrapheme(boundaries, last - 1))
        {
            last--;
        }

        return (boundaries[first], boundaries[last]);
    }

    /// <summary>Checks whether an edge grapheme intersects the viewport.</summary>
    /// <param name="boundaries">The document grapheme boundaries.</param>
    /// <param name="index">The grapheme index.</param>
    /// <returns>Whether any measured portion is visible.</returns>
    private bool IsVisibleGrapheme(List<int> boundaries, int index) =>
        Editor.GetAutomationVisibleRectangles(boundaries[index], boundaries[index + 1] - boundaries[index]).Count > 0;

    /// <summary>Combines overlapping intervals while retaining document gaps.</summary>
    /// <param name="intervals">The sorted visible intervals.</param>
    /// <returns>The native ranges.</returns>
    private IWindowsTextRangeProvider[] CreateVisibleRanges(List<(int Start, int End)> intervals)
    {
        var result = new List<IWindowsTextRangeProvider>();
        if (intervals.Count == 0)
        {
            return [];
        }

        var current = intervals[0];
        for (var index = 1; index < intervals.Count; index++)
        {
            var next = intervals[index];
            if (next.Start <= current.End)
            {
                current.End = Math.Max(current.End, next.End);
            }
            else
            {
                result.Add(CreateRange(current.Start, current.End));
                current = next;
            }
        }

        result.Add(CreateRange(current.Start, current.End));
        return result.ToArray();
    }
}
