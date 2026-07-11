// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents ControlMixins.</summary>
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public static class ControlMixins
{
    /// <summary>Provides extension members.</summary>
    /// <param name="list">The list value.</param>
    extension(IObservable<IEnumerable<ReactiveTreeItem>> list)
    {
        /// <summary>Flattens a reactive tree and projects each item to an observable sequence.</summary>
        /// <typeparam name="T">The projected value type.</typeparam>
        /// <param name="selector">Projection from each tree item to an observable.</param>
        /// <returns>An observable of projected values.</returns>
        public IObservable<T> FlattenAndSelect<T>(Func<ReactiveTreeItem, IObservable<T>> selector)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(selector);
#else
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
#endif

            return list
                .Flatten()
                .SelectMany(item => selector(item)?.Skip(1) ?? Observable.Empty<T>());
        }

        /// <summary>Flattens the specified list of reactive tree roots.</summary>
        /// <returns>An observable sequence of flattened tree items.</returns>
        private IObservable<ReactiveTreeItem> Flatten() =>
            list
                .Select(items => FlattenItems(items ?? Enumerable.Empty<ReactiveTreeItem>()))
                .Switch();
    }

    /// <summary>Provides the FlattenItems member.</summary>
    /// <param name="items">The items value.</param>
    /// <returns>The result.</returns>
    private static IObservable<ReactiveTreeItem> FlattenItems(IEnumerable<ReactiveTreeItem> items)
    {
        var streams = items
            .Where(static item => item is not null)
            .Select(FlattenItem)
            .ToArray();

        return streams.Length == 0
            ? Observable.Empty<ReactiveTreeItem>()
            : streams.Merge();
    }

    /// <summary>Provides the FlattenItem member.</summary>
    /// <param name="item">The item value.</param>
    /// <returns>The result.</returns>
    private static IObservable<ReactiveTreeItem> FlattenItem(ReactiveTreeItem item) =>
        Observable.Return(item)
            .Concat(
                item.Children.CurrentItems
                    .Select(children => FlattenItems(children ?? Enumerable.Empty<ReactiveTreeItem>()))
                    .Switch());
}
