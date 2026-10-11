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

/// <summary>Contains Moving Average Convergence Divergence output series.</summary>
/// <param name="Macd">The fast-minus-slow EMA series.</param>
/// <param name="Signal">The EMA of the MACD series.</param>
/// <param name="Histogram">The MACD-minus-signal histogram.</param>
public sealed record MacdResult(PlotSeriesData Macd, PlotSeriesData Signal, PlotSeriesData Histogram);
