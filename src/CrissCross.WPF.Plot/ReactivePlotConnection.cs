// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using ExceptionReplaySignal = ReactiveUI.Primitives.Reactive.Signals.ReplaySignal<System.Exception>;
#else
using ExceptionReplaySignal = ReactiveUI.Primitives.Signals.ReplaySignal<System.Exception>;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Tracks subscriptions, adapters, state, and errors for a reactive plot binding.</summary>
internal sealed class ReactivePlotConnection : IReactivePlotConnection
{
    /// <summary>Stores the state value.</summary>
    private readonly StateSignal<ReactivePlotConnectionState> _state = new(ReactivePlotConnectionState.Connecting);

    /// <summary>Stores the errors value.</summary>
    private readonly ExceptionReplaySignal _errors = new();

    /// <summary>Stores the active source subscriptions.</summary>
    private CompositeDisposable? _subscriptions;

    /// <summary>Stores the adapters owned by the connection.</summary>
    private IReadOnlyCollection<IReactivePlotAdapter> _adapters = [];

    /// <summary>Stores whether the connection has been disposed.</summary>
    private bool _disposed;

    /// <summary>Gets the state changes for the connection.</summary>
    /// <returns>The result.</returns>
    internal IObservable<ReactivePlotConnectionState> State => _state.AsObservable();

    /// <summary>Gets the errors surfaced by the connection.</summary>
    /// <returns>The result.</returns>
    internal IObservable<Exception> Errors => _errors.AsObservable();

    /// <summary>Gets the current connection state.</summary>
    internal ReactivePlotConnectionState CurrentState { get; private set; } = ReactivePlotConnectionState.Connecting;

    /// <summary>Gets a value indicating whether all sources completed.</summary>
    internal bool IsCompleted { get; private set; }

    /// <inheritdoc />
    IObservable<ReactivePlotConnectionState> IReactivePlotConnection.State => State;

    /// <inheritdoc />
    IObservable<Exception> IReactivePlotConnection.Errors => Errors;

    /// <inheritdoc />
    ReactivePlotConnectionState IReactivePlotConnection.CurrentState => CurrentState;

    /// <inheritdoc />
    bool IReactivePlotConnection.IsCompleted => IsCompleted;

    /// <summary>Handles the Attach operation.</summary>
    /// <param name="subscriptions">The subscriptions value.</param>
    /// <param name="adapters">The adapters value.</param>
    internal void Attach(CompositeDisposable subscriptions, IReadOnlyCollection<IReactivePlotAdapter> adapters)
    {
        ThrowHelper.ThrowIfNull(subscriptions, nameof(subscriptions));
        ThrowHelper.ThrowIfNull(adapters, nameof(adapters));

        _subscriptions = subscriptions;
        _adapters = adapters;
    }

    /// <summary>Handles the SetState operation.</summary>
    /// <param name="state">The state value.</param>
    internal void SetState(ReactivePlotConnectionState state)
    {
        if (_disposed && state != ReactivePlotConnectionState.Disposed)
        {
            return;
        }

        CurrentState = state;
        _state.OnNext(state);
    }

    /// <summary>Handles the AddError operation.</summary>
    /// <param name="error">The error value.</param>
    internal void AddError(Exception error)
    {
        ThrowHelper.ThrowIfNull(error, nameof(error));
        _errors.OnNext(error);
    }

    /// <summary>Marks the connection as completed.</summary>
    internal void MarkCompleted()
    {
        IsCompleted = true;
        SetState(ReactivePlotConnectionState.Completed);
    }

    /// <summary>Disposes the connection and all adapters it owns.</summary>
    internal void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _subscriptions?.Dispose();
        foreach (var adapter in _adapters)
        {
            adapter.Dispose();
        }

        CurrentState = ReactivePlotConnectionState.Disposed;
        _state.OnNext(ReactivePlotConnectionState.Disposed);
        _state.OnCompleted();
        _errors.OnCompleted();
        _state.Dispose();
        _errors.Dispose();
    }

    /// <inheritdoc />
    void IDisposable.Dispose() => Dispose();
}
