// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using ReactiveUI;
using Splat;

namespace CrissCross;

/// <summary>Provides build and observable collection helpers.</summary>
public static class RxObjectMixins
{
    /// <summary>Signals when dependency registration has completed.</summary>
    private static readonly ReplaySignal<Unit> _buildCompleteSubject = new(1);

    /// <summary>Provides build-completion helpers for build-aware objects.</summary>
    /// <param name="target">The build-aware object.</param>
    extension(IAmBuilt target)
    {
        /// <summary>Runs an action when the IOC container build has completed.</summary>
        /// <param name="action">The action.</param>
        public void BuildComplete(Action action)
        {
            ThrowHelper.ThrowIfNull(target, nameof(target));
            ThrowHelper.ThrowIfNull(action, nameof(action));
            _ = _buildCompleteSubject.Subscribe(unused => action());
        }

        /// <summary>Subscribes to IOC build completion and returns an IDisposable to unsubscribe.</summary>
        /// <param name="action">The action.</param>
        /// <returns>The subscription.</returns>
        public IDisposable BuildCompleteDisposable(Action action)
        {
            ThrowHelper.ThrowIfNull(target, nameof(target));
            ThrowHelper.ThrowIfNull(action, nameof(action));
            return _buildCompleteSubject.Subscribe(unused => action());
        }
    }

    /// <summary>Provides build-completion setup for the dependency resolver.</summary>
    /// <param name="resolver">The dependency resolver.</param>
    extension(IMutableDependencyResolver resolver)
    {
        /// <summary>Sets the IOC container build complete, Execute this once after completion of IOC registrations.</summary>
        public void SetupComplete()
        {
            ThrowHelper.ThrowIfNull(resolver, nameof(resolver));
            _buildCompleteSubject.OnNext(Unit.Default);
        }
    }

    /// <summary>Provides matching helpers for observable lists of observables.</summary>
    /// <typeparam name="T">The observed value type.</typeparam>
    /// <param name="sourceList">The observable list of observable values.</param>
    extension<T>(IObservable<IEnumerable<IObservable<T>>> sourceList)
    {
        /// <summary>Returns true when any observable in the current list matches the predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An observable boolean result.</returns>
        public IObservable<bool> AnyMatch(Func<T, bool> predicate) =>
            Observable.Create<bool>(observer =>
            {
                ThrowHelper.ThrowIfNull(sourceList, nameof(sourceList));
                ThrowHelper.ThrowIfNull(predicate, nameof(predicate));

                var disposable = new CompositeDisposable();
                var innerSubscriptions = new SerialDisposable();
                var valuesByObservable = new Dictionary<IObservable<T>, T>();
                var lastResult = false;
                var first = true;

                void Publish(bool result)
                {
                    if (!first && result == lastResult)
                    {
                        return;
                    }

                    first = false;
                    lastResult = result;
                    observer.OnNext(result);
                }

                _ = sourceList.Subscribe(items =>
                {
                    if (items is null)
                    {
                        return;
                    }

                    var currentInnerSubscriptions = new CompositeDisposable();
                    innerSubscriptions.Disposable = currentInnerSubscriptions;
                    valuesByObservable.Clear();

                    var hasItems = false;
                    foreach (var itemObservable in items)
                    {
                        hasItems = true;
                        _ = itemObservable.Subscribe(value =>
                        {
                            valuesByObservable[itemObservable] = value;
                            Publish(valuesByObservable.Values.Any(predicate));
                        }).DisposeWith(currentInnerSubscriptions);
                    }

                    Publish(hasItems && valuesByObservable.Values.Any(predicate));
                }).DisposeWith(disposable);

                disposable.Add(innerSubscriptions);
                return disposable;
            });
    }

    /// <summary>Provides projection helpers for observable lists.</summary>
    /// <typeparam name="T">The source item type.</typeparam>
    /// <param name="source">The observable sequence of source items.</param>
    extension<T>(IObservable<IEnumerable<T>> source)
        where T : ReactiveObject
    {
        /// <summary>Converts items into observables for the selected property.</summary>
        /// <typeparam name="TResult">The selected property type.</typeparam>
        /// <param name="predicate">The property selector.</param>
        /// <returns>An observable of item property observables.</returns>
#if NET8_0_OR_GREATER
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
#endif
        public IObservable<IEnumerable<IObservable<TResult>>> ToListOfObservables<TResult>(Expression<Func<T, TResult>> predicate) =>
            Observable.Create<IEnumerable<IObservable<TResult>>>(observer =>
            {
                ThrowHelper.ThrowIfNull(source, nameof(source));
                ThrowHelper.ThrowIfNull(predicate, nameof(predicate));

                var disposable = new CompositeDisposable();
                var result = new List<IObservable<TResult>>();
                _ = source.Subscribe(items =>
                {
                    if (items is null)
                    {
                        return;
                    }

                    result.Clear();
                    foreach (var item in items)
                    {
                        result.Add(item.WhenAnyValue(predicate));
                    }

                    observer.OnNext(result);
                }).DisposeWith(disposable);
                return disposable;
            });
    }
}
