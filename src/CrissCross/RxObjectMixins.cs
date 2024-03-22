// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Splat;

namespace CrissCross;

#pragma warning disable RCS1175 // Unused 'this' parameter.
/// <summary>
/// RxObjectMixins.
/// </summary>
public static class RxObjectMixins
{
    private static readonly ReplaySubject<Unit> _buildCompleteSubject = new(1);

    /// <summary>
    /// Sets the IOC container build complete, Execute this once after completion of IOC registrations.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    public static void SetupComplete(this IMutableDependencyResolver dummy) => _buildCompleteSubject.OnNext(Unit.Default);

    /// <summary>
    /// Gets the build complete.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="action">The action.</param>
    /// <value>The build complete.</value>
    public static void BuildComplete(this IAmBuilt dummy, Action action) => _buildCompleteSubject.Subscribe(_ => action());

    /// <summary>
    /// Converts to listofobservable.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>An IObservable of IEnumerable of IObservable of TResult.</returns>
    public static IObservable<IEnumerable<IObservable<TResult>>> ToListOfObservables<T, TResult>(this IObservable<IEnumerable<T>> source, Expression<Func<T, TResult>> predicate)
        where T : ReactiveObject =>
        Observable.Create<IEnumerable<IObservable<TResult>>>(o =>
        {
            var disposable = new CompositeDisposable();
            var result = new List<IObservable<TResult>>();
            source.Subscribe(l =>
            {
                if (l == null)
                {
                    return;
                }

                result.Clear();
                foreach (var item in l)
                {
                    result.Add(item.WhenAnyValue(predicate));
                }

                o.OnNext(result);
            }).DisposeWith(disposable);
            return disposable;
        });

    /// <summary>
    /// Any observable in the list is true.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="sourceList">The source list.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// An IObservable of bool.
    /// </returns>
    public static IObservable<bool> AnyMatch<T>(this IObservable<IEnumerable<IObservable<T>>> sourceList, Func<T, bool> predicate) =>
    Observable.Create<bool>(o =>
    {
        var disposable = new CompositeDisposable();
        var dis = new CompositeDisposable();
        var valueDict = new Dictionary<IObservable<T>, T>();
        var lastResult = false;
        var first = true;
        sourceList.Subscribe(l =>
        {
            if (l?.Any() != true)
            {
                return;
            }

            dis?.Dispose();
            dis = [];
            valueDict.Clear();
            if (first)
            {
                first = false;
                o.OnNext(false);
            }

            foreach (var rti in l)
            {
                valueDict[rti] = default!;
                rti.Subscribe(x =>
                {
                    valueDict[rti] = x;
                    var result = valueDict.Values.Any(predicate);
                    if (result != lastResult)
                    {
                        lastResult = result;
                        o.OnNext(result);
                    }
                }).DisposeWith(dis);
            }
        }).DisposeWith(disposable);
        disposable.Add(dis);
        return disposable;
    });
}
#pragma warning restore RCS1175 // Unused 'this' parameter.

