// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Tracks subscriptions, adapters, state, and errors for a reactive plot binding.</summary>
internal sealed class ReactivePlotConnection : IReactivePlotConnection
{
    /// <summary>Stores the state value.</summary>
    private readonly StateSignal<ReactivePlotConnectionState> _state = new(ReactivePlotConnectionState.Connecting);

    /// <summary>Stores the errors value.</summary>
    private readonly ReplaySignal<Exception> _errors = new();

    /// <summary>Stores the active source subscriptions.</summary>
    private CompositeDisposable? _subscriptions;

    /// <summary>Stores the adapters owned by the connection.</summary>
    private IReadOnlyCollection<IReactivePlotAdapter> _adapters = [];

    /// <summary>Stores whether the connection has been disposed.</summary>
    private bool _disposed;

    /// <summary>Gets the state changes for the connection.</summary>
    /// <returns>The result.</returns>
    public IObservable<ReactivePlotConnectionState> State => _state.AsObservable();

    /// <summary>Gets the errors surfaced by the connection.</summary>
    /// <returns>The result.</returns>
    public IObservable<Exception> Errors => _errors.AsObservable();

    /// <summary>Gets the current connection state.</summary>
    public ReactivePlotConnectionState CurrentState { get; private set; } = ReactivePlotConnectionState.Connecting;

    /// <summary>Gets a value indicating whether all sources completed.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Handles the Attach operation.</summary>
    /// <param name="subscriptions">The subscriptions value.</param>
    /// <param name="adapters">The adapters value.</param>
    public void Attach(CompositeDisposable subscriptions, IReadOnlyCollection<IReactivePlotAdapter> adapters)
    {
        _subscriptions = subscriptions;
        _adapters = adapters;
    }

    /// <summary>Handles the SetState operation.</summary>
    /// <param name="state">The state value.</param>
    public void SetState(ReactivePlotConnectionState state)
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
    public void AddError(Exception error) => _errors.OnNext(error);

    /// <summary>Marks the connection as completed.</summary>
    public void MarkCompleted()
    {
        IsCompleted = true;
        SetState(ReactivePlotConnectionState.Completed);
    }

    /// <inheritdoc />
    public void Dispose()
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
}
