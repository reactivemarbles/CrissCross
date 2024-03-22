// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace CrissCross.WPF.UI.Controls
{
    /// <summary>
    /// ControlMixins.
    /// </summary>
    public static class ControlMixins
    {
        /// <summary>
        /// Flattens the and select.
        /// </summary>
        /// <typeparam name="T">The type of the select.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>An IObservable of T.</returns>
        public static IObservable<T> FlattenAndSelect<T>(this IObservable<IEnumerable<ReactiveTreeItem>> list, Func<ReactiveTreeItem, IObservable<T>> selector) =>
            Observable.Create<T>(o =>
            {
                var disposable = new CompositeDisposable();
                var dis = new CompositeDisposable();
                disposable.Add(list.Flatten()
                    .Do(_ =>
                    {
                        dis?.Dispose();
                        dis = [];
                    })
                    .SelectMany(y => y.Select(z => z))
                    .Subscribe(sub => sub.SelectMany(selector).Skip(1).Subscribe(o.OnNext).DisposeWith(dis)));
                disposable.Add(dis);
                return disposable;
            });

        /// <summary>
        /// Flattens the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>
        /// An IObservable of IEnumerable of ReactiveTreeItem.
        /// </returns>
        public static IObservable<IEnumerable<IObservable<ReactiveTreeItem>>> Flatten(this IObservable<IEnumerable<ReactiveTreeItem>> list) =>
            Observable.Create<IEnumerable<IObservable<ReactiveTreeItem>>>(o =>
            {
                var disposable = new CompositeDisposable();
                var dis = new CompositeDisposable();
                var listRTI = new List<IObservable<ReactiveTreeItem>>();
                var isSetup = new ReplaySubject<bool>(1);
                isSetup.OnNext(false);
                disposable.Add(list.Subscribe(l =>
                {
                    if (l == null)
                    {
                        return;
                    }

                    listRTI.Clear();
                    dis?.Dispose();
                    dis = [];

                    foreach (var rti in l)
                    {
                        listRTI.Add(Observable.Return(rti));
                        rti.Children.CurrentItems.Flatten(isSetup).CombineLatest(isSetup, (x, s) => (x, s)).Subscribe(x =>
                        {
                            if (x.x?.Any() != true)
                            {
                                return;
                            }

                            foreach (var y in x.x)
                            {
                                if (!listRTI.Contains(y))
                                {
                                    listRTI.Add(y);
                                }
                            }

                            if (x.s)
                            {
                                o.OnNext(listRTI);
                            }
                        }).DisposeWith(dis);
                    }

                    o.OnNext(listRTI);
                    isSetup.OnNext(true);
                }));
                disposable.Add(dis);
                return disposable;
            });

        /// <summary>
        /// Flattens the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="isSetup">The is setup.</param>
        /// <returns>
        /// An IObservable of IEnumerable of ReactiveTreeItem.
        /// </returns>
        private static IObservable<IEnumerable<IObservable<ReactiveTreeItem>>> Flatten(this IObservable<IEnumerable<ReactiveTreeItem>> list, IObservable<bool> isSetup) =>
            Observable.Create<IEnumerable<IObservable<ReactiveTreeItem>>>(o =>
            {
                var disposable = new CompositeDisposable();
                var dis = new CompositeDisposable();
                var listRTI = new List<IObservable<ReactiveTreeItem>>();
                disposable.Add(list.Subscribe(l =>
                {
                    if (l == null)
                    {
                        return;
                    }

                    listRTI.Clear();
                    dis?.Dispose();
                    dis = [];

                    foreach (var rti in l)
                    {
                        listRTI.Add(Observable.Return(rti));
                        rti.Children.CurrentItems.Flatten(isSetup).CombineLatest(isSetup, (x, s) => (x, s)).Subscribe(x =>
                        {
                            if (x.x?.Any() != true)
                            {
                                return;
                            }

                            foreach (var y in x.x)
                            {
                                if (!listRTI.Contains(y))
                                {
                                    listRTI.Add(y);
                                }
                            }

                            if (x.s)
                            {
                                o.OnNext(listRTI);
                            }
                        }).DisposeWith(dis);
                    }

                    o.OnNext(listRTI);
                }));
                disposable.Add(dis);
                return disposable;
            });
    }
}
