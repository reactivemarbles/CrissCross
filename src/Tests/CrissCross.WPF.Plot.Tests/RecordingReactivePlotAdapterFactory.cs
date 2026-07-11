// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Creates recording plot adapters for binder tests.</summary>
internal sealed class RecordingReactivePlotAdapterFactory : IReactivePlotAdapterFactory
{
    /// <summary>Stores adapters by key.</summary>
    private readonly Dictionary<PlotSeriesKey, RecordingReactivePlotAdapter> _adapters = [];

    /// <summary>Gets the number of created adapters.</summary>
    public int CreatedAdapters { get; private set; }

    /// <summary>Gets the created adapters.</summary>
    public IReadOnlyList<RecordingReactivePlotAdapter> Adapters => [.. _adapters.Values];

    /// <inheritdoc />
    public IReactivePlotAdapter Create(PlotSeriesKey key, PlotType plotType)
    {
        CreatedAdapters++;
        var adapter = new RecordingReactivePlotAdapter(key, plotType);
        _adapters.Add(key, adapter);
        return adapter;
    }

    /// <summary>Finds an adapter by key.</summary>
    /// <param name="key">The series key.</param>
    /// <returns>The adapter for the key.</returns>
    public RecordingReactivePlotAdapter Find(PlotSeriesKey key) => _adapters[key];
}
