// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Factory that creates one adapter per logical reactive plot series.</summary>
public interface IReactivePlotAdapterFactory
{
    /// <summary>Creates an adapter for the supplied series key and plot type.</summary>
    /// <param name="key">The stable series key.</param>
    /// <param name="plotType">The chart type rendered by the adapter.</param>
    /// <returns>A plot adapter.</returns>
    IReactivePlotAdapter Create(PlotSeriesKey key, PlotType plotType);
}
