// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Contains Moving Average Convergence Divergence output series.</summary>
/// <param name="Macd">The fast-minus-slow EMA series.</param>
/// <param name="Signal">The EMA of the MACD series.</param>
/// <param name="Histogram">The MACD-minus-signal histogram.</param>
public sealed record MacdResult(
    PlotSeriesData Macd,
    PlotSeriesData Signal,
    PlotSeriesData Histogram);
