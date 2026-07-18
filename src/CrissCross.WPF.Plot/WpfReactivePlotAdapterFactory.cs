// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Creates WPF reactive plot adapters with rotating series colors.</summary>
/// <param name="chart">The owning live chart view model.</param>
internal sealed class WpfReactivePlotAdapterFactory(LiveChartViewModel chart) : IReactivePlotAdapterFactory
{
    /// <summary>Provides the default color palette for created adapters.</summary>
    private static readonly string[] Colors =
    [
        "#377eb8",
        "#ff7f00",
        "#4daf4a",
        "#f781bf",
        "#a65628",
        "#984ea3",
        "#999999",
        "#e41a1c",];

    /// <summary>Stores the next color index.</summary>
    private int _colorIndex;

    /// <summary>Handles the Create operation.</summary>
    /// <param name="key">The key value.</param>
    /// <param name="plotType">The plotType value.</param>
    /// <returns>The result.</returns>
    public IReactivePlotAdapter Create(PlotSeriesKey key, PlotType plotType)
    {
        var color = Colors[_colorIndex];
        _colorIndex = (_colorIndex + 1) % Colors.Length;
        return new WpfReactivePlotAdapter(chart, key, plotType, color);
    }
}
