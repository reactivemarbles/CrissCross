// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents an active chart-source binding.
/// </summary>
public interface IReactivePlotConnection : IDisposable
{
    /// <summary>
    /// Gets lifecycle state transitions for the binding.
    /// </summary>
    IObservable<ReactivePlotConnectionState> State { get; }

    /// <summary>
    /// Gets recoverable update errors observed after binding.
    /// </summary>
    IObservable<Exception> Errors { get; }

    /// <summary>
    /// Gets the current lifecycle state.
    /// </summary>
    ReactivePlotConnectionState CurrentState { get; }

    /// <summary>
    /// Gets a value indicating whether all source streams completed.
    /// </summary>
    bool IsCompleted { get; }
}
