// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if PLOT_MAUI && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#elif PLOT_MAUI
namespace CrissCross.Maui.Plot;
#elif PLOT_AVALONIA && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#elif PLOT_AVALONIA
namespace CrissCross.Avalonia.Plot;
#elif REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

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
