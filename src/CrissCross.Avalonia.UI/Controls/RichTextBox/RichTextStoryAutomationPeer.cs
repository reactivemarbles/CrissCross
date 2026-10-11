// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.VisualTree;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Exposes a real measured page story independently of the body text range.</summary>
internal sealed class RichTextStoryAutomationPeer : ControlAutomationPeer, IValueProvider, IInvokeProvider
{
    /// <summary>The host responsible for hyperlink events.</summary>
    private readonly RichTextBox _editor;

    /// <summary>The region's real clipping control.</summary>
    private readonly Control _region;

    /// <summary>The region's actual formatted presenter.</summary>
    private readonly FormattedTextPresenter _presenter;

    /// <summary>The stable story identity.</summary>
    private readonly string _identity;

    /// <summary>The accessible story or segment name.</summary>
    private readonly string _name;

    /// <summary>The semantic role.</summary>
    private readonly AutomationControlType _role;

    /// <summary>The positioned story segment, when this is an element peer.</summary>
    private readonly TextSegment? _segment;

    /// <summary>The positioned table cell, when this is a cell peer.</summary>
    private readonly RichTextTableCell? _cell;

    /// <summary>Initializes a new instance of the <see cref="RichTextStoryAutomationPeer"/> class.</summary>
    /// <param name="editor">The editor.</param>
    /// <param name="region">The actual clipped page region.</param>
    /// <param name="presenter">The actual story presenter.</param>
    /// <param name="kind">The region kind.</param>
    /// <param name="page">The physical sheet number.</param>
    internal RichTextStoryAutomationPeer(RichTextBox editor, Control region, FormattedTextPresenter presenter, string kind, int page)
        : this(editor, region, presenter, new($"RichTextStory-{page}-{kind}", $"Page {page} {kind}", AutomationControlType.Group, null, null))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="RichTextStoryAutomationPeer"/> class.</summary>
    /// <param name="editor">The editor.</param>
    /// <param name="region">The actual clipping region.</param>
    /// <param name="presenter">The story presenter.</param>
    /// <param name="details">The semantic identity and position details.</param>
    private RichTextStoryAutomationPeer(RichTextBox editor, Control region, FormattedTextPresenter presenter, StoryDetails details)
        : base(region)
    {
        _editor = editor;
        _region = region;
        _presenter = presenter;
        _identity = details.Identity;
        _name = details.Name;
        _role = details.Role;
        _segment = details.Segment;
        _cell = details.Cell;
    }

    /// <summary>Gets the page story's read-only state.</summary>
    public bool IsReadOnly => true;

    /// <summary>Gets rendered story text, including resolved page fields.</summary>
    public string Value => _presenter.Document?.PlainText ?? string.Empty;

    /// <summary>Rejects changes to the separate read-only story.</summary>
    /// <param name="value">The proposed replacement.</param>
    public void SetValue(string? value) => throw new InvalidOperationException("Page stories are read-only.");

    /// <summary>Invokes a story hyperlink through the editor's host event.</summary>
    public void Invoke()
    {
        EnsureEnabled();
        if (_role != AutomationControlType.Hyperlink || string.IsNullOrEmpty(_segment?.HyperlinkNavigationUri))
        {
            throw new InvalidOperationException("This story element is not a hyperlink.");
        }

        _editor.InvokeAutomationHyperlink(_segment.HyperlinkNavigationUri, _segment.HyperlinkTarget);
    }

    /// <inheritdoc/>
    protected override object? GetProviderCore(Type providerType) =>
        (providerType == typeof(IValueProvider) && _segment is not null)
            || (providerType == typeof(IInvokeProvider) && _role != AutomationControlType.Hyperlink)
            ? null
            : base.GetProviderCore(providerType);

    /// <inheritdoc/>
    protected override AutomationControlType GetAutomationControlTypeCore() => _role;

    /// <inheritdoc/>
    protected override AutomationControlType GetControlTypeOverrideCore() => _role;

    /// <inheritdoc/>
    protected override string GetClassNameCore() => "RichTextPageStory";

    /// <inheritdoc/>
    protected override string GetClassNameOverrideCore() => GetClassNameCore();

    /// <inheritdoc/>
    protected override string GetAutomationIdCore() => _identity;

    /// <inheritdoc/>
    protected override string GetNameCore() => _name;

    /// <inheritdoc/>
    protected override string? GetHelpTextCore() => _role == AutomationControlType.Hyperlink ? _segment?.HyperlinkNavigationUri : null;

    /// <inheritdoc/>
    protected override string? GetItemStatusCore() => _cell is { } cell ? $"Spans {cell.RowSpan} rows and {cell.ColumnSpan} columns" : null;

    /// <inheritdoc/>
    protected override bool IsKeyboardFocusableCore() => false;

    /// <inheritdoc/>
    protected override bool HasKeyboardFocusCore() => false;

    /// <inheritdoc/>
    protected override void SetFocusCore() => BringIntoViewCore();

    /// <inheritdoc/>
    protected override void BringIntoViewCore() => _region.BringIntoView();

    /// <inheritdoc/>
    protected override Rect GetBoundingRectangleCore()
    {
        var root = global::Avalonia.Controls.TopLevel.GetTopLevel(_region);
        if (root is null || _region.TransformToVisual(root) is not { } regionTransform)
        {
            return default;
        }

        var bounds = new Rect(_region.Bounds.Size).TransformToAABB(regionTransform);
        var segment = _segment;
        return segment is null || _presenter.TransformToVisual(root) is not { } transform
            ? bounds
            : GetSegmentBounds(bounds, transform, segment);
    }

    /// <inheritdoc/>
    protected override bool IsOffscreenCore()
    {
        var root = global::Avalonia.Controls.TopLevel.GetTopLevel(_region);
        var visible = GetBoundingRectangleCore();
        if (root is null || !_region.IsEffectivelyVisible)
        {
            return true;
        }

        visible = visible.Intersect(new(root.Bounds.Size));
        foreach (var ancestor in _region.GetVisualAncestors())
        {
            if (ancestor.ClipToBounds && ancestor.TransformToVisual(root) is { } transform)
            {
                visible = visible.Intersect(new Rect(ancestor.Bounds.Size).TransformToAABB(transform));
            }
        }

        return visible.Width <= 0 || visible.Height <= 0;
    }

    /// <inheritdoc/>
    protected override IReadOnlyList<AutomationPeer> GetChildrenCore()
    {
        if (_cell is { } cell)
        {
            return CreateCellSegments(cell);
        }

        if (_segment?.Table is { } table)
        {
            return CreateTableCells(table);
        }

        return _segment is null ? CreateSegments(_presenter.Document?.Segments ?? [], 0) : Array.Empty<AutomationPeer>();
    }

    /// <summary>Gets the semantic role of a story segment.</summary>
    /// <param name="segment">The positioned story segment.</param>
    /// <returns>The segment's accessible role.</returns>
    private static AutomationControlType GetSegmentRole(TextSegment segment)
    {
        if (segment.Table is not null)
        {
            return AutomationControlType.Table;
        }

        if (segment.IsImage)
        {
            return AutomationControlType.Image;
        }

        return !string.IsNullOrEmpty(segment.HyperlinkNavigationUri)
            ? AutomationControlType.Hyperlink
            : AutomationControlType.Text;
    }

    /// <summary>Gets the readable name of a story segment.</summary>
    /// <param name="segment">The positioned story segment.</param>
    /// <returns>The segment's readable content or object label.</returns>
    private static string GetSegmentName(TextSegment segment)
    {
        if (segment.Table is { } table)
        {
            return $"Table, {table.RowCount} rows, {table.ColumnCount} columns";
        }

        if (segment.IsImage)
        {
            return string.IsNullOrWhiteSpace(segment.ImageAlternativeText)
                ? nameof(Image)
                : segment.ImageAlternativeText;
        }

        return segment.Text;
    }

    /// <summary>Gets measured story-local bounds clipped to the page region.</summary>
    /// <param name="bounds">The page region's root bounds.</param>
    /// <param name="transform">The story's root transform.</param>
    /// <param name="segment">The positioned story segment.</param>
    /// <returns>The measured and clipped bounds.</returns>
    private Rect GetSegmentBounds(in Rect bounds, in Matrix transform, TextSegment segment)
    {
        var measured = default(Rect);
        var start = segment.StartIndex + (_cell?.StartIndex ?? 0);
        var length = _cell?.Length ?? segment.Length;
        foreach (var rectangle in _presenter.GetDocumentRangeRectangles(start, length))
        {
            var next = rectangle.TransformToAABB(transform).Intersect(bounds);
            if (next.Width > 0 && next.Height > 0)
            {
                measured = measured == default ? next : measured.Union(next);
            }
        }

        return measured;
    }

    /// <summary>Creates semantic children with story-local cell offsets.</summary>
    /// <param name="cell">The containing table cell.</param>
    /// <returns>The cell's semantic children.</returns>
    private List<AutomationPeer> CreateCellSegments(RichTextTableCell cell) =>
        CreateSegments(cell.Segments, (_segment?.StartIndex ?? 0) + cell.StartIndex);

    /// <summary>Creates semantic cells for a story table.</summary>
    /// <param name="table">The story table.</param>
    /// <returns>The table's semantic cells.</returns>
    private List<AutomationPeer> CreateTableCells(RichTextTable table)
    {
        var children = new List<AutomationPeer>(table.Cells.Count);
        foreach (var cell in table.Cells)
        {
            children.Add(CreateTableCellPeer(cell));
        }

        return children;
    }

    /// <summary>Creates a measured story cell peer.</summary>
    /// <param name="cell">The positioned story cell.</param>
    /// <returns>The story cell peer.</returns>
    private RichTextStoryAutomationPeer CreateTableCellPeer(RichTextTableCell cell)
    {
        var text = new StringBuilder();
        foreach (var part in cell.Segments)
        {
            _ = text.Append(part.Text);
        }

        var details = new StoryDetails(
            $"{_identity}-Cell-{cell.Row}-{cell.Column}",
            $"Row {cell.Row + 1}, column {cell.Column + 1}: {text}",
            AutomationControlType.DataItem,
            _segment,
            cell);
        return new(_editor, _region, _presenter, details);
    }

    /// <summary>Creates story-local semantic elements without using body offsets.</summary>
    /// <param name="segments">The story segments.</param>
    /// <param name="offset">The enclosing story-local offset.</param>
    /// <returns>The positioned story elements.</returns>
    private List<AutomationPeer> CreateSegments(IReadOnlyList<TextSegment> segments, int offset)
    {
        var children = new List<AutomationPeer>();
        foreach (var source in segments)
        {
            var segment = source.Clone();
            segment.StartIndex += offset;
            var role = GetSegmentRole(segment);
            var name = GetSegmentName(segment);
            if (role != AutomationControlType.Text || !string.IsNullOrWhiteSpace(name))
            {
                var details = new StoryDetails(
                    $"{_identity}-{role}-{segment.StartIndex}-{segment.Length}",
                    name,
                    role,
                    segment,
                    null);
                children.Add(new RichTextStoryAutomationPeer(_editor, _region, _presenter, details));
            }
        }

        return children;
    }

    /// <summary>Groups the identity and position of a story element.</summary>
    /// <param name="Identity">The stable automation identifier.</param>
    /// <param name="Name">The readable element name.</param>
    /// <param name="Role">The semantic control role.</param>
    /// <param name="Segment">The story-local segment.</param>
    /// <param name="Cell">The containing table cell.</param>
    private sealed record StoryDetails(
        string Identity,
        string Name,
        AutomationControlType Role,
        TextSegment? Segment,
        RichTextTableCell? Cell);
}
