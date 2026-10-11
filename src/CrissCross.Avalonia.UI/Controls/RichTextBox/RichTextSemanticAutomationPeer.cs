// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents document content without exposing presentation-only visual children.</summary>
internal sealed class RichTextSemanticAutomationPeer : ControlAutomationPeer, IInvokeProvider
{
    /// <summary>The associated editor.</summary>
    private readonly RichTextBox _editor;

    /// <summary>The document segment.</summary>
    private readonly TextSegment _segment;

    /// <summary>The accessible name.</summary>
    private readonly string _name;

    /// <summary>The accessible role.</summary>
    private readonly AutomationControlType _role;

    /// <summary>The containing table cell, if any.</summary>
    private readonly RichTextTableCell? _cell;

    /// <summary>Initializes a new instance of the <see cref="RichTextSemanticAutomationPeer"/> class.</summary>
    /// <param name="editor">The editor.</param>
    /// <param name="segment">The segment.</param>
    /// <param name="name">The accessible name.</param>
    /// <param name="role">The accessible role.</param>
    /// <param name="cell">The table cell.</param>
    private RichTextSemanticAutomationPeer(RichTextBox editor, TextSegment segment, string name, AutomationControlType role, RichTextTableCell? cell = null)
        : base(editor)
    {
        _editor = editor;
        _segment = segment;
        _name = name;
        _role = role;
        _cell = cell;
    }

    /// <summary>Gets the associated document editor.</summary>
    internal RichTextBox Editor => _editor;

    /// <summary>Gets the exact document start, including table cell offsets.</summary>
    internal int DocumentStart => GetStart();

    /// <summary>Gets the exclusive document end.</summary>
    internal int DocumentEnd => GetStart() + GetLength();

    /// <summary>Gets the semantic table model, if present.</summary>
    internal RichTextTable? TableModel => _segment.Table;

    /// <summary>Gets the semantic cell model, if present.</summary>
    internal RichTextTableCell? CellModel => _cell;

    /// <summary>Invokes the original hyperlink target through the host event.</summary>
    public void Invoke()
    {
        EnsureEnabled();
        if (_role != AutomationControlType.Hyperlink || string.IsNullOrEmpty(_segment.HyperlinkNavigationUri))
        {
            throw new InvalidOperationException("This document element is not a hyperlink.");
        }

        _editor.InvokeAutomationHyperlink(_segment.HyperlinkNavigationUri, _segment.HyperlinkTarget);
    }

    /// <summary>Creates semantic children from document segments in reading order.</summary>
    /// <param name="editor">The editor.</param>
    /// <returns>The document elements.</returns>
    internal static IReadOnlyList<AutomationPeer> CreateChildren(RichTextBox editor) => CreateSegmentChildren(editor, editor.Document.Segments, 0);

    /// <inheritdoc/>
    protected override object? GetProviderCore(Type providerType) =>
        providerType == typeof(IInvokeProvider) && _role != AutomationControlType.Hyperlink ? null : base.GetProviderCore(providerType);

    /// <inheritdoc/>
    protected override AutomationControlType GetAutomationControlTypeCore() => _role;

    /// <inheritdoc/>
    protected override AutomationControlType GetControlTypeOverrideCore() => _role;

    /// <inheritdoc/>
    protected override string GetClassNameOverrideCore() => GetClassNameCore();

    /// <inheritdoc/>
    protected override string GetAutomationIdCore() => $"RichText-{_role}-{GetStart()}-{GetLength()}";

    /// <inheritdoc/>
    protected override string GetClassNameCore() => "RichTextDocumentElement";

    /// <inheritdoc/>
    protected override string GetNameCore() => _name;

    /// <inheritdoc/>
    protected override string? GetHelpTextCore() => _role == AutomationControlType.Hyperlink ? _segment.HyperlinkNavigationUri : null;

    /// <inheritdoc/>
    protected override string? GetItemStatusCore() => _cell is { } cell ? $"Spans {cell.RowSpan} rows and {cell.ColumnSpan} columns" : null;

    /// <inheritdoc/>
    protected override bool IsKeyboardFocusableCore() => _role == AutomationControlType.Hyperlink && _editor.Focusable;

    /// <inheritdoc/>
    protected override bool HasKeyboardFocusCore() => _editor.IsFocused && _editor.CaretIndex >= GetStart() && _editor.CaretIndex < GetStart() + GetLength();

    /// <inheritdoc/>
    protected override void SetFocusCore()
    {
        EnsureEnabled();
        _editor.Select(GetStart(), GetLength());
        _ = _editor.Focus();
    }

    /// <inheritdoc/>
    protected override void BringIntoViewCore() => _editor.BringAutomationPositionIntoView(GetStart());

    /// <inheritdoc/>
    protected override Rect GetBoundingRectangleCore()
    {
        var rectangles = _editor.GetAutomationRangeRectangles(GetStart(), GetLength());
        var result = default(Rect);
        foreach (var rectangle in rectangles)
        {
            result = result == default ? rectangle : result.Union(rectangle);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override bool IsOffscreenCore() => base.IsOffscreenCore() || GetBoundingRectangleCore().Intersect(base.GetBoundingRectangleCore()).Height <= 0;

    /// <inheritdoc/>
    protected override IReadOnlyList<AutomationPeer> GetChildrenCore()
    {
        if (_cell is { } cell)
        {
            return CreateSegmentChildren(_editor, cell.Segments, GetStart());
        }

        return _role == AutomationControlType.Table && _segment.Table is { } table ? CreateTableChildren(table) : [];
    }

    /// <summary>Builds text, link, image, and table semantics from positioned rich segments.</summary>
    /// <param name="editor">The editor.</param>
    /// <param name="segments">The rich segments.</param>
    /// <param name="offset">The document offset of the segment collection.</param>
    /// <returns>The semantic elements in source order.</returns>
    private static List<AutomationPeer> CreateSegmentChildren(RichTextBox editor, IReadOnlyList<TextSegment> segments, int offset)
    {
        var children = new List<AutomationPeer>();
        foreach (var source in segments)
        {
            var segment = source.Clone();
            segment.StartIndex += offset;
            if (segment.Table is { } table)
            {
                children.Add(new RichTextSemanticAutomationPeer(editor, segment, $"Table, {table.RowCount} rows, {table.ColumnCount} columns", AutomationControlType.Table));
            }
            else if (segment.IsImage)
            {
                var name = string.IsNullOrWhiteSpace(segment.ImageAlternativeText) ? nameof(Image) : segment.ImageAlternativeText;
                children.Add(new RichTextSemanticAutomationPeer(editor, segment, name, AutomationControlType.Image));
            }
            else if (!string.IsNullOrEmpty(segment.HyperlinkNavigationUri))
            {
                children.Add(new RichTextSemanticAutomationPeer(editor, segment, segment.Text, AutomationControlType.Hyperlink));
            }
            else if (!string.IsNullOrWhiteSpace(segment.Text))
            {
                children.Add(new RichTextSemanticAutomationPeer(editor, segment, segment.Text, AutomationControlType.Text));
            }
        }

        return children;
    }

    /// <summary>Creates positioned table cell elements in reading order.</summary>
    /// <param name="table">The table metadata.</param>
    /// <returns>The cell peers.</returns>
    private List<AutomationPeer> CreateTableChildren(RichTextTable table)
    {
        var children = new List<AutomationPeer>(table.Cells.Count);
        foreach (var cell in table.Cells)
        {
            var text = new StringBuilder();
            foreach (var part in cell.Segments)
            {
                _ = text.Append(part.Text);
            }

            children.Add(new RichTextSemanticAutomationPeer(_editor, _segment, $"Row {cell.Row + 1}, column {cell.Column + 1}: {text}", AutomationControlType.DataItem, cell));
        }

        return children;
    }

    /// <summary>Gets the element's document start position.</summary>
    /// <returns>The start position.</returns>
    private int GetStart() => _segment.StartIndex + (_cell?.StartIndex ?? 0);

    /// <summary>Gets the element's document length.</summary>
    /// <returns>The document length.</returns>
    private int GetLength() => _cell?.Length ?? _segment.Length;
}
