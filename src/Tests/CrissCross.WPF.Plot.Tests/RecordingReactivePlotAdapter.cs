// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

internal sealed class RecordingReactivePlotAdapter(PlotSeriesKey key, PlotType plotType) : IReactivePlotAdapter
{
    public PlotSeriesKey Key { get; } = key;

    public PlotType PlotType { get; } = plotType;

    public List<ReactivePlotUpdate> Updates { get; } = [];

    public int ApplyCallCount { get; private set; }

    public int ClearCount { get; private set; }

    public bool IsDisposed { get; private set; }

    public void Apply(ReactivePlotUpdate update)
    {
        ApplyCallCount++;
        Updates.Add(update);
        if (update.Kind == ReactivePlotUpdateKind.Clear)
        {
            ClearCount++;
        }
    }

    public void Dispose() => IsDisposed = true;
}
