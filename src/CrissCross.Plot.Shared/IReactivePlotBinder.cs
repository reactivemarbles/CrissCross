// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if PLOT_MAUI && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#elif PLOT_MAUI
namespace CrissCross.Maui.Plot;
#elif PLOT_AVALONIA && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#elif PLOT_AVALONIA
namespace CrissCross.Avalonia.Plot;
#elif REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Binds reactive plot sources to chart adapters.</summary>
public interface IReactivePlotBinder
{
    /// <summary>Binds the supplied sources to a live chart view model.</summary>
    /// <param name="chart">The live chart view model to bind.</param>
    /// <param name="sources">The reactive sources to bind.</param>
    /// <returns>An owned connection that controls source subscriptions.</returns>
    IReactivePlotConnection Bind(LiveChartViewModel chart, IEnumerable<IReactivePlotSource> sources);

    /// <summary>Binds the supplied sources to a live chart view model.</summary>
    /// <param name="chart">The live chart view model to bind.</param>
    /// <param name="sources">The reactive sources to bind.</param>
    /// <param name="options">Optional binding options.</param>
    /// <returns>An owned connection that controls source subscriptions.</returns>
    IReactivePlotConnection Bind(
        LiveChartViewModel chart,
        IEnumerable<IReactivePlotSource> sources,
        ReactivePlotBindingOptions? options);
}
