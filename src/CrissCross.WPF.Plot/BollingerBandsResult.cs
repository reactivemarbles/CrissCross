// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Contains Bollinger Band output series.</summary>
/// <param name="Middle">The simple moving average.</param>
/// <param name="Upper">The upper standard-deviation band.</param>
/// <param name="Lower">The lower standard-deviation band.</param>
public sealed record BollingerBandsResult(PlotSeriesData Middle, PlotSeriesData Upper, PlotSeriesData Lower);
