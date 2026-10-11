// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides a document-position range for accessibility integrations.</summary>
/// <remarks>Offsets are clamped to the current document after edits. This is an application contract; Avalonia does not expose a native text-pattern provider.</remarks>
public sealed class RichTextAutomationRange
{
    /// <summary>The associated editor.</summary>
    private readonly RichTextBox _editor;

    /// <summary>The original start position.</summary>
    private readonly int _start;

    /// <summary>The original end position.</summary>
    private readonly int _end;

    /// <summary>Initializes a new instance of the <see cref="RichTextAutomationRange"/> class.</summary>
    /// <param name="editor">The associated editor.</param>
    /// <param name="start">The first insertion position.</param>
    /// <param name="end">The last insertion position.</param>
    internal RichTextAutomationRange(RichTextBox editor, int start, int end)
    {
        _editor = editor;
        _start = Math.Min(start, end);
        _end = Math.Max(start, end);
    }

    /// <summary>Gets the first insertion position in the current document.</summary>
    public int Start => Math.Clamp(_start, 0, _editor.Document.Length);

    /// <summary>Gets the last insertion position in the current document.</summary>
    public int End => Math.Clamp(_end, Start, _editor.Document.Length);

    /// <summary>Gets the rendered text in the range.</summary>
    /// <param name="maximumLength">The maximum character count, or -1 for all text.</param>
    /// <returns>The current document text in the range.</returns>
    public string GetText(int maximumLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumLength, -1);
        var length = End - Start;
        return _editor.Document.PlainText.Substring(Start, maximumLength < 0 ? length : Math.Min(maximumLength, length));
    }

    /// <summary>Gets all rendered text in the range.</summary>
    /// <returns>The current document text in the range.</returns>
    public string GetText() => GetText(-1);

    /// <summary>Gets measured rectangles in top-level device-independent coordinates.</summary>
    /// <returns>One or more rectangles for laid out text.</returns>
    public IReadOnlyList<Rect> GetBoundingRectangles() => _editor.GetAutomationRangeRectangles(Start, End - Start);

    /// <summary>Selects the range without changing document content.</summary>
    public void Select()
    {
        if (!_editor.IsEffectivelyEnabled)
        {
            throw new global::Avalonia.Automation.ElementNotEnabledException();
        }

        _editor.Select(Start, End - Start);
    }

    /// <summary>Scrolls the start or end insertion position into view.</summary>
    /// <param name="alignToTop">Whether to expose the start instead of the end.</param>
    public void ScrollIntoView(bool alignToTop) => _editor.BringAutomationPositionIntoView(alignToTop ? Start : End);

    /// <summary>Scrolls the start insertion position into view.</summary>
    public void ScrollIntoView() => ScrollIntoView(true);
}
