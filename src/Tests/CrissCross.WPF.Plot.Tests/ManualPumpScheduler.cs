// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace CrissCross.WPF.Plot.Tests;

internal sealed class ManualPumpScheduler : IScheduler
{
    private readonly Queue<Action> _actions = new();

    public DateTimeOffset Now { get; private set; }

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        _actions.Enqueue(() => action(this, state));
        return new ScheduledDisposable(() => { });
    }

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        _actions.Enqueue(() =>
        {
            Now = Now.Add(dueTime);
            action(this, state);
        });
        return new ScheduledDisposable(() => { });
    }

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        _actions.Enqueue(() =>
        {
            Now = dueTime;
            action(this, state);
        });
        return new ScheduledDisposable(() => { });
    }

    public void RunAll()
    {
        while (_actions.Count > 0)
        {
            _actions.Dequeue().Invoke();
        }
    }

    private sealed class ScheduledDisposable(Action dispose) : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                dispose();
            }
        }
    }
}
