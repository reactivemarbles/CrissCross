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

/// <summary>Maintains native accessibility insertion endpoints in the rich document.</summary>
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Windows UI Automation uses built-in COM callable wrappers and VARIANT marshaling.")]
[RequiresUnreferencedCode("Built-in COM callable wrappers require the public provider interface metadata.")]
internal sealed partial class WindowsTextRangeProvider : IWindowsTextRangeProvider
{
    /// <summary>The number of endpoints enclosing a unit.</summary>
    private const int EndpointCount = 2;

    /// <summary>The number of values in a native bounding rectangle.</summary>
    private const int RectangleValueCount = 4;

    /// <summary>The native rectangle width index.</summary>
    private const int RectangleWidthIndex = 2;

    /// <summary>The native rectangle height index.</summary>
    private const int RectangleHeightIndex = 3;

    /// <summary>Stores the _node value.</summary>
    private readonly WindowsAutomationNode _node;

    /// <summary>Stores the _start value.</summary>
    private int _start;

    /// <summary>Stores the _end value.</summary>
    private int _end;

    /// <summary>Initializes a new instance of the <see cref="WindowsTextRangeProvider"/> class.</summary>
    /// <param name="node">The owning document provider.</param>
    /// <param name="start">The first insertion position.</param>
    /// <param name="end">The last insertion position.</param>
    internal WindowsTextRangeProvider(WindowsAutomationNode node, int start, int end)
    {
        _node = node;
        _start = Math.Min(start, end);
        _end = Math.Max(start, end);
    }

    /// <summary>Gets the document editor.</summary>
    private RichTextBox Editor => _node.Editor ?? throw new InvalidOperationException("The node does not own a rich document.");

    /// <summary>Gets the clamped first insertion endpoint.</summary>
    private int Start => Math.Clamp(_start, 0, Editor.Document.Length);

    /// <summary>Gets the clamped last insertion endpoint.</summary>
    private int End => Math.Clamp(_end, Start, Editor.Document.Length);

    /// <inheritdoc/>
    public IWindowsTextRangeProvider Clone() => Invoke<IWindowsTextRangeProvider>(() => new WindowsTextRangeProvider(_node, Start, End));

    /// <inheritdoc/>
    public bool Compare(IWindowsTextRangeProvider range) => Invoke(() =>
    {
        var other = ValidateRange(range);
        return Start == other.Start && End == other.End;
    });

    /// <inheritdoc/>
    public int CompareEndpoints(
        WindowsTextPatternRangeEndpoint endpoint,
        IWindowsTextRangeProvider targetRange,
        WindowsTextPatternRangeEndpoint targetEndpoint) =>
        Invoke(() => GetEndpoint(endpoint) - ValidateRange(targetRange).GetEndpoint(targetEndpoint));

    /// <inheritdoc/>
    public void ExpandToEnclosingUnit(WindowsTextUnit unit) => Invoke(() => ExpandCore(unit));

    /// <inheritdoc/>
    public IWindowsTextRangeProvider? FindText(string text, bool backward, bool ignoreCase) => Invoke(() => FindTextCore(text, backward, ignoreCase));

    /// <inheritdoc/>
    public double[] GetBoundingRectangles() => Invoke(GetBoundingRectanglesCore);

    /// <inheritdoc/>
    public IWindowsRawElementProviderSimple GetEnclosingElement() => Invoke(GetEnclosingElementCore);

    /// <inheritdoc/>
    public string GetText(int maxLength) => Invoke(() =>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, -1);
        var length = End - Start;
        return Editor.Document.PlainText.Substring(Start, maxLength < 0 ? length : Math.Min(length, maxLength));
    });

    /// <inheritdoc/>
    public int Move(WindowsTextUnit unit, int count) => Invoke(() => MoveCore(unit, count));

    /// <inheritdoc/>
    public int MoveEndpointByUnit(WindowsTextPatternRangeEndpoint endpoint, WindowsTextUnit unit, int count) => Invoke(() =>
    {
        var moved = WindowsTextUnits.MoveEndpoint(WindowsTextUnits.GetBoundaries(Editor, unit), GetEndpoint(endpoint), count);
        SetEndpoint(endpoint, moved.Offset);
        return moved.Moved;
    });

    /// <inheritdoc/>
    public void MoveEndpointByRange(
        WindowsTextPatternRangeEndpoint endpoint,
        IWindowsTextRangeProvider targetRange,
        WindowsTextPatternRangeEndpoint targetEndpoint) =>
        Invoke(() => SetEndpoint(endpoint, ValidateRange(targetRange).GetEndpoint(targetEndpoint)));

    /// <inheritdoc/>
    public void Select() => Invoke(() => new RichTextAutomationRange(Editor, Start, End).Select());

    /// <inheritdoc/>
    public void AddToSelection() => Invoke(static () => throw new InvalidOperationException("The document supports one selection."));

    /// <inheritdoc/>
    public void RemoveFromSelection() => AddToSelection();

    /// <inheritdoc/>
    public void ScrollIntoView(bool alignToTop) => Invoke(() => Editor.ScrollAutomationRange(Start, End, alignToTop));

    /// <inheritdoc/>
    public IWindowsRawElementProviderSimple[] GetChildren() => Invoke(GetChildrenCore);

    /// <summary>Runs an accessibility query on the editor thread.</summary>
    /// <typeparam name="T">The query result.</typeparam>
    /// <param name="action">The query.</param>
    /// <returns>The result.</returns>
    private T Invoke<T>(Func<T> action) => _node.Context.Invoke(action);

    /// <summary>Runs an accessibility mutation on the editor thread.</summary>
    /// <param name="action">The mutation.</param>
    private void Invoke(Action action) => _node.Context.Invoke(action);

    /// <summary>Validates that a comparison range belongs to this document.</summary>
    /// <param name="range">The comparison range.</param>
    /// <returns>The local range.</returns>
    private WindowsTextRangeProvider ValidateRange(IWindowsTextRangeProvider range) =>
        range is WindowsTextRangeProvider other && other._node.Context == _node.Context && ReferenceEquals(other.Editor, Editor)
            ? other
            : throw new ArgumentException("The range belongs to another document.", nameof(range));

    /// <summary>Gets a validated insertion endpoint.</summary>
    /// <param name="endpoint">The requested endpoint.</param>
    /// <returns>The insertion position.</returns>
    private int GetEndpoint(WindowsTextPatternRangeEndpoint endpoint) => endpoint switch
    {
        WindowsTextPatternRangeEndpoint.Start => Start,
        WindowsTextPatternRangeEndpoint.End => End,
        _ => throw new ArgumentOutOfRangeException(nameof(endpoint)),
    };

    /// <summary>Moves an endpoint and collapses the opposing endpoint if crossed.</summary>
    /// <param name="endpoint">The endpoint to move.</param>
    /// <param name="offset">The new insertion position.</param>
    private void SetEndpoint(WindowsTextPatternRangeEndpoint endpoint, int offset)
    {
        var start = Start;
        var end = End;
        _ = GetEndpoint(endpoint);
        _start = endpoint == WindowsTextPatternRangeEndpoint.Start ? offset : Math.Min(start, offset);
        _end = endpoint == WindowsTextPatternRangeEndpoint.End ? offset : Math.Max(end, offset);
    }

    /// <summary>Normalizes the range to one enclosing unit.</summary>
    /// <param name="unit">The requested unit.</param>
    private void ExpandCore(WindowsTextUnit unit)
    {
        var boundaries = WindowsTextUnits.GetBoundaries(Editor, unit);
        var index = WindowsTextUnits.GetUnitIndex(boundaries, Start);
        _start = boundaries[index];
        _end = boundaries[Math.Min(index + 1, boundaries.Count - 1)];
    }

    /// <summary>Moves a collapsed position or a normalized nonempty unit.</summary>
    /// <param name="unit">The requested unit.</param>
    /// <param name="count">The signed requested movement.</param>
    /// <returns>The signed actual movement.</returns>
    private int MoveCore(WindowsTextUnit unit, int count)
    {
        if (count == 0)
        {
            return 0;
        }

        var boundaries = WindowsTextUnits.GetBoundaries(Editor, unit);
        if (Start == End)
        {
            var moved = WindowsTextUnits.MoveEndpoint(boundaries, Start, count);
            _start = moved.Offset;
            _end = moved.Offset;
            return moved.Moved;
        }

        var origin = WindowsTextUnits.GetUnitIndex(boundaries, Start);
        var target = (int)Math.Clamp((long)origin + count, 0, Math.Max(0, boundaries.Count - EndpointCount));
        _start = boundaries[target];
        _end = boundaries[Math.Min(target + 1, boundaries.Count - 1)];
        return target - origin;
    }

    /// <summary>Searches only within this range.</summary>
    /// <param name="text">The search text.</param>
    /// <param name="backward">Whether to find the last match.</param>
    /// <param name="ignoreCase">Whether to ignore case.</param>
    /// <returns>The matching range, or null.</returns>
    private WindowsTextRangeProvider? FindTextCore(string text, bool backward, bool ignoreCase)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        var source = Editor.Document.PlainText.Substring(Start, End - Start);
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var index = backward ? source.LastIndexOf(text, comparison) : source.IndexOf(text, comparison);
        return index < 0 ? null : new WindowsTextRangeProvider(_node, Start + index, Start + index + text.Length);
    }

    /// <summary>Converts clipped root rectangles to native screen coordinates.</summary>
    /// <returns>The flattened screen rectangles.</returns>
    private double[] GetBoundingRectanglesCore()
    {
        var rectangles = Editor.GetAutomationVisibleRectangles(Start, End - Start);
        var result = new double[rectangles.Count * RectangleValueCount];
        for (var index = 0; index < rectangles.Count; index++)
        {
            var rectangle = _node.Context.ToScreenRectangle(rectangles[index]);
            var first = index * RectangleValueCount;
            result[first] = rectangle.Left;
            result[first + 1] = rectangle.Top;
            result[first + RectangleWidthIndex] = rectangle.Width;
            result[first + RectangleHeightIndex] = rectangle.Height;
        }

        return result;
    }

    /// <summary>Gets embedded semantic children, excluding ordinary text runs.</summary>
    /// <returns>The embedded native provider nodes.</returns>
    private IWindowsRawElementProviderSimple[] GetChildrenCore()
    {
        var result = new List<IWindowsRawElementProviderSimple>();
        foreach (var child in _node.Peer.GetChildren())
        {
            if (child is RichTextSemanticAutomationPeer peer && peer.DocumentStart >= Start && peer.DocumentEnd <= End
                && peer.GetAutomationControlType() != AutomationControlType.Text)
            {
                result.Add(_node.Context.Node(peer));
            }
        }

        return result.ToArray();
    }

    /// <summary>Finds the innermost enclosing semantic element.</summary>
    /// <returns>The element nearest to this range.</returns>
    private IWindowsRawElementProviderSimple GetEnclosingElementCore()
    {
        var peer = FindEnclosingPeer(_node.Peer);
        return _node.Context.Node(peer);
    }

    /// <summary>Descends through semantic containers enclosing both endpoints.</summary>
    /// <param name="parent">The current enclosing peer.</param>
    /// <returns>The innermost enclosing peer.</returns>
    private AutomationPeer FindEnclosingPeer(AutomationPeer parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is RichTextSemanticAutomationPeer peer && peer.DocumentStart <= Start && peer.DocumentEnd >= End
                && peer.GetAutomationControlType() != AutomationControlType.Text)
            {
                return FindEnclosingPeer(peer);
            }
        }

        return parent;
    }
}
