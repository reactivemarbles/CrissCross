// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Describes the rich content and grid placement of a table cell.</summary>
public sealed class RichTextTableCell
{
    /// <summary>Gets the zero based grid row.</summary>
    public int Row { get; init; }

    /// <summary>Gets the zero based grid column.</summary>
    public int Column { get; init; }

    /// <summary>Gets the number of grid rows occupied by the cell.</summary>
    public int RowSpan { get; init; } = 1;

    /// <summary>Gets the number of grid columns occupied by the cell.</summary>
    public int ColumnSpan { get; init; } = 1;

    /// <summary>Gets the cell start relative to the table's document range.</summary>
    public int StartIndex { get; init; }

    /// <summary>Gets the cell content length in document positions.</summary>
    public int Length { get; init; }

    /// <summary>Gets the rich segments with offsets relative to this cell.</summary>
    public IReadOnlyList<TextSegment> Segments { get; init; } = [];

    /// <summary>Gets the optional cell background brush.</summary>
    public IBrush? Background { get; init; }

    /// <summary>Gets the optional cell width in device independent pixels.</summary>
    public double? Width { get; init; }
}
