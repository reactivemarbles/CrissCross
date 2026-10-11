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

/// <summary>Contains Ichimoku Kinko Hyo output series.</summary>
/// <param name="Conversion">Tenkan-sen conversion line.</param>
/// <param name="Base">Kijun-sen base line.</param>
/// <param name="LeadingSpanA">Senkou Span A shifted forward.</param>
/// <param name="LeadingSpanB">Senkou Span B shifted forward.</param>
/// <param name="Lagging">Chikou span shifted backward.</param>
public sealed record IchimokuResult(
    PlotSeriesData Conversion,
    PlotSeriesData Base,
    PlotSeriesData LeadingSpanA,
    PlotSeriesData LeadingSpanB,
    PlotSeriesData Lagging);
