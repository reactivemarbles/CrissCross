// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Microsoft.Maui.Dispatching;
#if REACTIVE_SHIM
using ReactiveUI.Reactive;
#else
using ReactiveUI;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Supplies deterministic dispatch for actual neutral-target MAUI controls.</summary>
internal static class PlotTestApp
{
    /// <summary>The bounded notification timeout.</summary>
    private static readonly TimeSpan NotificationTimeout = TimeSpan.FromSeconds(5);

    /// <summary>Drains native dispatch work after the current source notification completes.</summary>
    internal static void Drain() => ((TestDispatcherProvider)DispatcherProvider.Current).Drain();

    /// <summary>Waits for asynchronous collection notifications while draining native dispatch work.</summary>
    /// <param name="ready">The completed UI state.</param>
    internal static void WaitUntil(Func<bool> ready)
    {
        var completed = SpinWait.SpinUntil(
            () =>
            {
                Drain();
                return ready();
            },
            NotificationTimeout);
        if (completed)
        {
            return;
        }

        throw new TimeoutException("Native collection notifications did not complete.");
    }

    /// <summary>Runs native MAUI work with synchronous reactive notifications.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The native control work.</param>
    /// <returns>The result.</returns>
    internal static Task<T> Run<T>(Func<T> action)
    {
        var previousProvider = DispatcherProvider.Current;
        var previousScheduler = RxSchedulers.MainThreadScheduler;
        var previousTaskpool = RxSchedulers.TaskpoolScheduler;
        var previousContext = SynchronizationContext.Current;
        var provider = new TestDispatcherProvider();
        _ = DispatcherProvider.SetCurrent(provider);
        SynchronizationContext.SetSynchronizationContext(new NativeSynchronizationContext(provider.GetForCurrentThread()));
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
        RxSchedulers.TaskpoolScheduler = ImmediateScheduler.Instance;
        try
        {
            return Task.FromResult(action());
        }
        finally
        {
            _ = DispatcherProvider.SetCurrent(previousProvider);
            RxSchedulers.MainThreadScheduler = previousScheduler;
            RxSchedulers.TaskpoolScheduler = previousTaskpool;
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    /// <summary>Marshals collection notifications to the test's native dispatcher.</summary>
    /// <param name="dispatcher">The dispatcher.</param>
    private sealed class NativeSynchronizationContext(IDispatcher dispatcher) : SynchronizationContext
    {
        /// <inheritdoc />
        public override void Post(SendOrPostCallback d, object? state) => _ = dispatcher.Dispatch(() => d(state));
    }

    /// <summary>Provides dispatch on the test's owning thread.</summary>
    private sealed class TestDispatcherProvider : IDispatcherProvider
    {
        /// <summary>Stores the test dispatcher.</summary>
        private readonly QueuedDispatcher _dispatcher = new();

        /// <inheritdoc />
        public IDispatcher GetForCurrentThread() => _dispatcher;

        /// <summary>Drains the test dispatcher queue.</summary>
        internal void Drain() => _dispatcher.Drain();
    }

    /// <summary>Dispatches immediate native-control operations.</summary>
    private sealed class QueuedDispatcher : IDispatcher
    {
        /// <summary>Stores queued native dispatch work.</summary>
        private readonly ConcurrentQueue<Action> _pending = new();

        /// <summary>Stores the owning native test thread.</summary>
        private readonly int _ownerThread = Environment.CurrentManagedThreadId;

        /// <inheritdoc />
        public bool IsDispatchRequired => Environment.CurrentManagedThreadId != _ownerThread;

        /// <inheritdoc />
        public bool Dispatch(Action action)
        {
            _pending.Enqueue(action);
            return true;
        }

        /// <inheritdoc />
        public bool DispatchDelayed(TimeSpan delay, Action action) => throw new NotSupportedException("Use the manual scheduler for delayed scheduling tests.");

        /// <inheritdoc />
        public IDispatcherTimer CreateTimer() => throw new NotSupportedException("These native-control tests do not start dispatcher timers.");

        /// <summary>Runs work after native collection notifications have completed.</summary>
        internal void Drain()
        {
            while (_pending.TryDequeue(out var action))
            {
                action();
            }
        }
    }
}
