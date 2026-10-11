// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Adapts public Avalonia patterns to native UI Automation.</summary>
internal static partial class WindowsPatternAdapters
{
    /// <summary>The native grid pattern identifier.</summary>
    private const int GridPatternId = 10_006;

    /// <summary>The native grid item pattern identifier.</summary>
    private const int GridItemPatternId = 10_007;

    /// <summary>The native table pattern identifier.</summary>
    private const int TablePatternId = 10_012;

    /// <summary>The native table item pattern identifier.</summary>
    private const int TableItemPatternId = 10_013;

    /// <summary>The native scroll item pattern identifier.</summary>
    private const int ScrollItemPatternId = 10_017;

    /// <summary>The native window pattern identifier.</summary>
    private const int WindowPatternId = 10_009;

    /// <summary>Gets the native patterns represented by rich document semantics.</summary>
    /// <param name="node">The node argument.</param>
    /// <param name="patternId">The patternId argument.</param>
    /// <returns>The native pattern result.</returns>
    private static object? GetDocumentProvider(WindowsAutomationNode node, int patternId)
    {
        if (patternId == WindowPatternId && node.Peer is global::Avalonia.Automation.Peers.ControlAutomationPeer { Owner: global::Avalonia.Controls.Window window })
        {
            return new WindowAdapter(node, window);
        }

        if (patternId == ScrollItemPatternId)
        {
            return new ScrollItemAdapter(node);
        }

        return node.Peer is RichTextSemanticAutomationPeer peer ? patternId switch
        {
            GridPatternId or TablePatternId when peer.CellModel is null && peer.TableModel is not null => new TableAdapter(node),
            GridItemPatternId or TableItemPatternId when peer.CellModel is not null => new CellAdapter(node),
            _ => null,
        } : null;
    }

    /// <summary>Brings an Avalonia peer into view through its public contract.</summary>
    /// <param name="node">The associated node.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class ScrollItemAdapter(WindowsAutomationNode node) : IWindowsScrollItemProvider
    {
        /// <summary>The associated node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <inheritdoc/>
        public void ScrollIntoView() => _node.Context.Invoke(_node.Peer.BringIntoView);
    }

    /// <summary>Exposes the logical rows, columns and headers of a rich table.</summary>
    /// <param name="node">The associated node.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class TableAdapter(WindowsAutomationNode node) : IWindowsGridProvider, IWindowsTableProvider
    {
        /// <summary>The associated table node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <inheritdoc/>
        public int GetRowCount() => _node.Context.Invoke(() => GetModel().RowCount);

        /// <inheritdoc/>
        public int GetColumnCount() => _node.Context.Invoke(() => GetModel().ColumnCount);

        /// <inheritdoc/>
        public int GetRowOrColumnMajor() => 0;

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple GetItem(int row, int column) => _node.Context.Invoke(() => FindItem(row, column));

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple[] GetRowHeaders() => _node.Context.Invoke(() => Headers(false));

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple[] GetColumnHeaders() => _node.Context.Invoke(() => Headers(true));

        /// <summary>Gets the associated table model.</summary>
        /// <returns>The table model.</returns>
        private RichTextTable GetModel() => ((RichTextSemanticAutomationPeer)_node.Peer).TableModel
            ?? throw new InvalidOperationException("The document element no longer represents a table.");

        /// <summary>Gets the cell covering the specified logical grid coordinate.</summary>
        /// <param name="row">The logical row.</param>
        /// <param name="column">The logical column.</param>
        /// <returns>The cell node.</returns>
        private WindowsAutomationNode FindItem(int row, int column)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(row);
            ArgumentOutOfRangeException.ThrowIfNegative(column);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, GetModel().RowCount);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(column, GetModel().ColumnCount);
            foreach (var child in _node.Peer.GetChildren())
            {
                if (child is RichTextSemanticAutomationPeer { CellModel: { } cell }
                    && row >= cell.Row && row < cell.Row + cell.RowSpan
                    && column >= cell.Column && column < cell.Column + cell.ColumnSpan)
                {
                    return _node.Context.Node(child);
                }
            }

            throw new InvalidOperationException("The specified grid position has no cell.");
        }

        /// <summary>Gets explicitly marked header cells at the leading grid boundary.</summary>
        /// <param name="columns">Whether to get column headers.</param>
        /// <returns>The header nodes.</returns>
        private IWindowsRawElementProviderSimple[] Headers(bool columns)
        {
            var result = new List<IWindowsRawElementProviderSimple>();
            foreach (var child in _node.Peer.GetChildren())
            {
                if (child is RichTextSemanticAutomationPeer { CellModel: { IsHeader: true } cell }
                    && (columns ? cell.Row == 0 : cell.Column == 0))
                {
                    result.Add(_node.Context.Node(child));
                }
            }

            return [.. result];
        }
    }

    /// <summary>Exposes a cell's logical position, spans and header relationships.</summary>
    /// <param name="node">The associated node.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class CellAdapter(WindowsAutomationNode node) : IWindowsGridItemProvider, IWindowsTableItemProvider
    {
        /// <summary>The associated cell node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <inheritdoc/>
        public int GetRow() => _node.Context.Invoke(() => GetModel().Row);

        /// <inheritdoc/>
        public int GetColumn() => _node.Context.Invoke(() => GetModel().Column);

        /// <inheritdoc/>
        public int GetRowSpan() => _node.Context.Invoke(() => GetModel().RowSpan);

        /// <inheritdoc/>
        public int GetColumnSpan() => _node.Context.Invoke(() => GetModel().ColumnSpan);

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple GetContainingGrid() => _node.Context.Invoke(() => _node.Context.Node(
            _node.Peer.GetParent() ?? throw new InvalidOperationException("The table cell has no parent.")));

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple[] GetRowHeaderItems() => _node.Context.Invoke(() => Headers(false));

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple[] GetColumnHeaderItems() => _node.Context.Invoke(() => Headers(true));

        /// <summary>Gets the associated cell model.</summary>
        /// <returns>The cell model.</returns>
        private RichTextTableCell GetModel() => ((RichTextSemanticAutomationPeer)_node.Peer).CellModel
            ?? throw new InvalidOperationException("The document element no longer represents a table cell.");

        /// <summary>Gets the headers whose spans overlap this cell's row or column.</summary>
        /// <param name="columns">Whether to get column headers.</param>
        /// <returns>The header nodes.</returns>
        private IWindowsRawElementProviderSimple[] Headers(bool columns)
        {
            var result = new List<IWindowsRawElementProviderSimple>();
            var parent = _node.Peer.GetParent() ?? throw new InvalidOperationException("The table cell has no parent.");
            foreach (var child in parent.GetChildren())
            {
                if (child is not RichTextSemanticAutomationPeer { CellModel: { IsHeader: true } header })
                {
                    continue;
                }

                var overlaps = columns
                    ? header.Row == 0 && header.Column < GetModel().Column + GetModel().ColumnSpan && GetModel().Column < header.Column + header.ColumnSpan
                    : header.Column == 0 && header.Row < GetModel().Row + GetModel().RowSpan && GetModel().Row < header.Row + header.RowSpan;
                if (overlaps)
                {
                    result.Add(_node.Context.Node(child));
                }
            }

            return [.. result];
        }
    }
}
