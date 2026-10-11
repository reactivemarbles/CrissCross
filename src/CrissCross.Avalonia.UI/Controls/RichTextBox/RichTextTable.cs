// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Describes an HTML table with positioned, optionally spanning cells.</summary>
public sealed class RichTextTable
{
    /// <summary>Gets the cells in source order.</summary>
    public IReadOnlyList<RichTextTableCell> Cells { get; init; } = [];

    /// <summary>Gets the number of grid rows.</summary>
    public int RowCount { get; init; }

    /// <summary>Gets the number of grid columns.</summary>
    public int ColumnCount { get; init; }

    /// <summary>Gets the optional table width in device independent pixels.</summary>
    public double? Width { get; init; }
}
