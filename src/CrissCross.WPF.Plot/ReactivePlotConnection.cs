// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace CrissCross.WPF.Plot;

internal sealed class ReactivePlotConnection : IReactivePlotConnection
{
    private readonly BehaviorSubject<ReactivePlotConnectionState> _state = new(ReactivePlotConnectionState.Connecting);
    private readonly ReplaySubject<Exception> _errors = new();
    private CompositeDisposable? _subscriptions;
    private IReadOnlyCollection<IReactivePlotAdapter> _adapters = [];
    private bool _disposed;

    public IObservable<ReactivePlotConnectionState> State => _state.AsObservable();

    public IObservable<Exception> Errors => _errors.AsObservable();

    public ReactivePlotConnectionState CurrentState { get; private set; } = ReactivePlotConnectionState.Connecting;

    public bool IsCompleted { get; private set; }

    public void Attach(CompositeDisposable subscriptions, IReadOnlyCollection<IReactivePlotAdapter> adapters)
    {
        _subscriptions = subscriptions;
        _adapters = adapters;
    }

    public void SetState(ReactivePlotConnectionState state)
    {
        if (_disposed && state != ReactivePlotConnectionState.Disposed)
        {
            return;
        }

        CurrentState = state;
        _state.OnNext(state);
    }

    public void AddError(Exception error) => _errors.OnNext(error);

    public void MarkCompleted()
    {
        IsCompleted = true;
        SetState(ReactivePlotConnectionState.Completed);
    }

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
