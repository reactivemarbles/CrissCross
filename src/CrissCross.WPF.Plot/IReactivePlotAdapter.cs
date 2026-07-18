// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Applies normalized reactive plot updates to a chart-specific plottable.</summary>
public interface IReactivePlotAdapter : IDisposable
{
    /// <summary>Gets the stable series key handled by this adapter.</summary>
    PlotSeriesKey Key { get; }

    /// <summary>Gets the plot type handled by this adapter.</summary>
    PlotType PlotType { get; }

    /// <summary>Applies a normalized reactive plot update.</summary>
    /// <param name="update">The update to apply.</param>
    void Apply(ReactivePlotUpdate update);
}
