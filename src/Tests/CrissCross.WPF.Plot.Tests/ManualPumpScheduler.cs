// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Provides a manually pumped scheduler for deterministic binder tests.</summary>
public sealed class ManualPumpScheduler : IScheduler
{
#if REACTIVELIST_REACTIVE
    /// <summary>Stores queued work items.</summary>
    private readonly Queue<Action> _actions = new();
#else
    /// <summary>Stores queued work items.</summary>
    private readonly Queue<IWorkItem> _actions = new();
#endif

    /// <inheritdoc />
    public DateTimeOffset Now { get; } = TimeProvider.System.GetUtcNow();

#if REACTIVELIST_REACTIVE
    /// <inheritdoc />
    public IDisposable Schedule<TState>(
        TState state,
        Func<IScheduler, TState, IDisposable> action)
    {
        _actions.Enqueue(() => _ = action(this, state));
        return System.Reactive.Disposables.Disposable.Empty;
    }

    /// <inheritdoc />
    public IDisposable Schedule<TState>(
        TState state,
        TimeSpan dueTime,
        Func<IScheduler, TState, IDisposable> action) =>
        Schedule(state, action);

    /// <inheritdoc />
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<IScheduler, TState, IDisposable> action) =>
        Schedule(state, dueTime - Now, action);
#else
    /// <summary>Gets the last scheduled timestamp.</summary>
    public long Timestamp { get; private set; }

    /// <inheritdoc />
    public void Schedule(IWorkItem item) => _actions.Enqueue(item);

    /// <inheritdoc />
    public void Schedule(IWorkItem item, long dueTimestamp)
    {
        Timestamp = dueTimestamp;
        _actions.Enqueue(item);
    }
#endif

    /// <summary>Runs all queued work items.</summary>
    internal void RunAll()
    {
        while (_actions.Count > 0)
        {
#if REACTIVELIST_REACTIVE
            _actions.Dequeue()();
#else
            _actions.Dequeue().Execute();
#endif
        }
    }
}
