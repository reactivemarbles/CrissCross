// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Describes an observable source of chart updates for one logical plot series.</summary>
public interface IReactivePlotSource
{
    /// <summary>Gets the stable series key used to preserve identity across updates.</summary>
    PlotSeriesKey Key { get; }

    /// <summary>Gets the plot type rendered by this source.</summary>
    PlotType PlotType { get; }

    /// <summary>Gets the observable update stream for this series.</summary>
    IObservable<ReactivePlotUpdate> Updates { get; }
}
