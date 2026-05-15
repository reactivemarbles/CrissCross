// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Immutable update envelope used by all reactive chart sources.
/// </summary>
/// <param name="Key">The stable series identity.</param>
/// <param name="PlotType">The chart type to update.</param>
/// <param name="Kind">The operation to apply.</param>
/// <param name="X">The X-axis values.</param>
/// <param name="Y">The Y-axis values.</param>
/// <param name="XAxisKind">The interpretation of X-axis values.</param>
/// <param name="Sequence">A monotonic sequence value supplied by the source adapter.</param>
/// <param name="MaxPoints">The optional maximum number of points that should be retained for this update's target series.</param>
public sealed record ReactivePlotUpdate(
    PlotSeriesKey Key,
    PlotType PlotType,
    ReactivePlotUpdateKind Kind,
    IReadOnlyList<double> X,
    IReadOnlyList<double> Y,
    PlotXAxisKind XAxisKind,
    long Sequence,
    int? MaxPoints = null);
