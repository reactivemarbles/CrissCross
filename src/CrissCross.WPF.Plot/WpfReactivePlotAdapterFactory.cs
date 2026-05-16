// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

internal sealed class WpfReactivePlotAdapterFactory(LiveChartViewModel chart) : IReactivePlotAdapterFactory
{
    private static readonly string[] Colors = ["#377eb8", "#ff7f00", "#4daf4a", "#f781bf", "#a65628", "#984ea3", "#999999", "#e41a1c"];
    private int _colorIndex;

    public IReactivePlotAdapter Create(PlotSeriesKey key, PlotType plotType) =>
        new WpfReactivePlotAdapter(chart, key, plotType, Colors[_colorIndex++ % Colors.Length]);
}
