// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>Creates observable event streams backed by ReactiveUI.Primitives.</summary>
public static class EventSignal
{
    /// <summary>Creates an observable sequence for an event and projects each notification to its event arguments.</summary>
    /// <typeparam name="TEventHandler">The event handler delegate type.</typeparam>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="addHandler">The handler subscription callback.</param>
    /// <param name="removeHandler">The handler unsubscription callback.</param>
    /// <returns>An observable sequence of event arguments.</returns>
    public static IObservable<TEventArgs> From<TEventHandler, TEventArgs>(
        Action<TEventHandler> addHandler,
        Action<TEventHandler> removeHandler)
        where TEventHandler : Delegate
        where TEventArgs : EventArgs =>
        ReactiveUI.Primitives.Signals.Signal
            .FromEventPattern<TEventHandler, TEventArgs>(addHandler, removeHandler)
            .Select(static pattern => pattern.EventArgs);
}
