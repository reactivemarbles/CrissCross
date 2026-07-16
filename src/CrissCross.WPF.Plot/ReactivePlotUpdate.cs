// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Immutable update envelope used by all reactive chart sources.</summary>
/// <param name="Key">The stable series identity.</param>
/// <param name="PlotType">The chart type to update.</param>
/// <param name="Kind">The operation to apply.</param>
/// <param name="X">The X-axis values.</param>
/// <param name="Y">The Y-axis values.</param>
/// <param name="XAxisKind">The interpretation of X-axis values.</param>
/// <param name="Sequence">A monotonic sequence value supplied by the source adapter.</param>
/// <param name="MaxPoints">The optional retention limit for the target series.</param>
/// <param name="Style">Optional immutable series styling.</param>
public sealed record ReactivePlotUpdate(
    PlotSeriesKey Key,
    PlotType PlotType,
    ReactivePlotUpdateKind Kind,
    IReadOnlyList<double> X,
    IReadOnlyList<double> Y,
    PlotXAxisKind XAxisKind,
    long Sequence,
    int? MaxPoints = null,
    ReactivePlotSeriesStyle? Style = null);
