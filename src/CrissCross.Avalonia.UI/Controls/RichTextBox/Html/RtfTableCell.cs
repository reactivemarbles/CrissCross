// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Describes the rectangular grid occupied by one HTML cell.</summary>
internal sealed class RtfTableCell
{
    /// <summary>Initializes a new instance of the <see cref="RtfTableCell"/> class.</summary>
    /// <param name="element">The source element.</param>
    /// <param name="row">The starting row.</param>
    /// <param name="column">The starting column.</param>
    /// <param name="rowSpan">The occupied row count.</param>
    /// <param name="columnSpan">The occupied column count.</param>
    internal RtfTableCell(IElement element, int row, int column, int rowSpan, int columnSpan)
    {
        Element = element;
        Row = row;
        Column = column;
        RowSpan = rowSpan;
        ColumnSpan = columnSpan;
    }

    /// <summary>Gets the source element.</summary>
    internal IElement Element { get; }

    /// <summary>Gets the starting row.</summary>
    internal int Row { get; }

    /// <summary>Gets the starting column.</summary>
    internal int Column { get; }

    /// <summary>Gets the occupied row count.</summary>
    internal int RowSpan { get; }

    /// <summary>Gets the occupied column count.</summary>
    internal int ColumnSpan { get; }
}
