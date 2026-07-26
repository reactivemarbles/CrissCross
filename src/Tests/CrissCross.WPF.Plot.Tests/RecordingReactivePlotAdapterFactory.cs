// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#else
using CrissCross.WPF.Plot;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Creates recording plot adapters for binder tests.</summary>
internal sealed class RecordingReactivePlotAdapterFactory : IReactivePlotAdapterFactory
{
    /// <summary>Stores adapters by key.</summary>
    private readonly Dictionary<PlotSeriesKey, RecordingReactivePlotAdapter> _adapters = [];

    /// <summary>Gets the number of created adapters.</summary>
    internal int CreatedAdapters { get; private set; }

    /// <summary>Gets the created adapters.</summary>
    internal IReadOnlyList<RecordingReactivePlotAdapter> Adapters => [.. _adapters.Values];

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
    internal RecordingReactivePlotAdapter Find(PlotSeriesKey key) => _adapters[key];
}
