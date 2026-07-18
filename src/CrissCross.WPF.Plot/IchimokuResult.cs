// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
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
