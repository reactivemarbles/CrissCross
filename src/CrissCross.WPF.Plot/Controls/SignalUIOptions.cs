// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Configures scaling, buffering, and X-axis behavior for a <see cref="SignalUI"/>.</summary>
public sealed class SignalUIOptions
{
    /// <summary>Gets a value indicating whether automatic axis scaling is enabled.</summary>
    public bool AutoScale { get; init; } = true;

    /// <summary>Gets a value indicating whether manual axis scaling is enabled.</summary>
    public bool ManualScale { get; init; }

    /// <summary>Gets the optional stream controlling fixed-point display mode.</summary>
    public IObservable<bool>? FixedPoints { get; init; }

    /// <summary>Gets the optional stream controlling the maximum displayed point count.</summary>
    public IObservable<int>? NumberPointsPlotted { get; init; }

    /// <summary>Gets a value indicating whether X values are represented as date-time ticks.</summary>
    public bool Ticks { get; init; } = true;
}
