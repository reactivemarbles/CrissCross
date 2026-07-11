// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Records applied plot updates for binder tests.</summary>
/// <param name="key">The series key.</param>
/// <param name="plotType">The plot type.</param>
internal sealed class RecordingReactivePlotAdapter(PlotSeriesKey key, PlotType plotType) : IReactivePlotAdapter
{
    /// <inheritdoc />
    public PlotSeriesKey Key { get; } = key;

    /// <inheritdoc />
    public PlotType PlotType { get; } = plotType;

    /// <summary>Gets applied updates.</summary>
    public List<ReactivePlotUpdate> Updates { get; } = [];

    /// <summary>Gets the number of apply calls.</summary>
    public int ApplyCallCount { get; private set; }

    /// <summary>Gets the number of clear updates.</summary>
    public int ClearCount { get; private set; }

    /// <summary>Gets a value indicating whether the adapter has been disposed.</summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc />
    public void Apply(ReactivePlotUpdate update)
    {
        ApplyCallCount++;
        Updates.Add(update);
        if (update.Kind != ReactivePlotUpdateKind.Clear)
        {
            return;
        }

        ClearCount++;
    }

    /// <inheritdoc />
    public void Dispose() => IsDisposed = true;
}
