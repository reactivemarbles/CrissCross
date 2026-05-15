// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Factory that creates one adapter per logical reactive plot series.
/// </summary>
public interface IReactivePlotAdapterFactory
{
    /// <summary>
    /// Creates an adapter for the supplied series key and plot type.
    /// </summary>
    /// <param name="key">The stable series key.</param>
    /// <param name="plotType">The ScottPlot-backed plot type.</param>
    /// <returns>A plot adapter.</returns>
    IReactivePlotAdapter Create(PlotSeriesKey key, PlotType plotType);
}
