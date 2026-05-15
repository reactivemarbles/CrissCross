// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Binds reactive plot sources to chart adapters.
/// </summary>
public interface IReactivePlotBinder
{
    /// <summary>
    /// Binds the supplied sources to a live chart view model.
    /// </summary>
    /// <param name="chart">The chart view model to update.</param>
    /// <param name="sources">The reactive plot sources to subscribe.</param>
    /// <param name="options">Optional binding options.</param>
    /// <returns>An owned connection that controls source subscriptions.</returns>
    IReactivePlotConnection Bind(LiveChartViewModel chart, IEnumerable<IReactivePlotSource> sources, ReactivePlotBindingOptions? options = null);
}
