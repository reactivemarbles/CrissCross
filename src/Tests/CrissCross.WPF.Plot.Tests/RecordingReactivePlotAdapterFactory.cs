// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

internal sealed class RecordingReactivePlotAdapterFactory : IReactivePlotAdapterFactory
{
    private readonly Dictionary<PlotSeriesKey, RecordingReactivePlotAdapter> _adapters = [];

    public int CreatedAdapters { get; private set; }

    public IReadOnlyList<RecordingReactivePlotAdapter> Adapters => [.. _adapters.Values];

    public IReactivePlotAdapter Create(PlotSeriesKey key, PlotType plotType)
    {
        CreatedAdapters++;
        var adapter = new RecordingReactivePlotAdapter(key, plotType);
        _adapters.Add(key, adapter);
        return adapter;
    }

    public RecordingReactivePlotAdapter Find(PlotSeriesKey key) => _adapters[key];
}
