// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading;

#if REACTIVE_SHIM
using SignalFactory = ReactiveUI.Primitives.Reactive.Signals.Signal;
#else
using SignalFactory = ReactiveUI.Primitives.Signals.Signal;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Creates observable event streams backed by ReactiveUI.Primitives.</summary>
public static class EventSignal
{
    /// <summary>Creates a projected observable event sequence.</summary>
    /// <typeparam name="TEventHandler">The event handler delegate type.</typeparam>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="handlerFactory">Creates the concrete event delegate from a standard event handler.</param>
    /// <param name="addHandler">The handler subscription callback.</param>
    /// <param name="removeHandler">The handler unsubscription callback.</param>
    /// <returns>An observable sequence of event arguments.</returns>
    public static IObservable<TEventArgs> From<TEventHandler, TEventArgs>(
        Func<EventHandler<TEventArgs>, TEventHandler> handlerFactory,
        Action<TEventHandler> addHandler,
        Action<TEventHandler> removeHandler)
        where TEventHandler : Delegate
        where TEventArgs : EventArgs =>
        SignalFactory.CreateSafe<TEventArgs>(observer =>
        {
            var handler = handlerFactory((_, eventArgs) => observer.OnNext(eventArgs));

            addHandler(handler);

            return new EventSubscription<TEventHandler>(removeHandler, handler);
        });

    /// <summary>Creates an observable sequence for a standard generic event handler.</summary>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="addHandler">The handler subscription callback.</param>
    /// <param name="removeHandler">The handler unsubscription callback.</param>
    /// <returns>An observable sequence of event arguments.</returns>
    public static IObservable<TEventArgs> From<TEventArgs>(
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler)
        where TEventArgs : EventArgs =>
        From<EventHandler<TEventArgs>, TEventArgs>(handler => handler, addHandler, removeHandler);

    /// <summary>Removes an event handler when the observable subscription is disposed.</summary>
    /// <typeparam name="TEventHandler">The event handler delegate type.</typeparam>
    /// <param name="removeHandler">The event unsubscription callback.</param>
    /// <param name="handler">The subscribed event handler.</param>
    private sealed class EventSubscription<TEventHandler>(Action<TEventHandler> removeHandler, TEventHandler handler)
        : IDisposable
        where TEventHandler : Delegate
    {
        /// <summary>The event unsubscription callback.</summary>
        private Action<TEventHandler>? _removeHandler = removeHandler;

        /// <summary>The subscribed event handler.</summary>
        private TEventHandler? _handler = handler;

        /// <summary>Removes the subscribed event handler once.</summary>
        public void Dispose()
        {
            var remove = Interlocked.Exchange(ref _removeHandler, null);
            var eventHandler = Interlocked.Exchange(ref _handler, null);

            if (remove is null || eventHandler is null)
            {
                return;
            }

            remove(eventHandler);
        }
    }
}
