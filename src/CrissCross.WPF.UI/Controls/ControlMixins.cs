// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// ControlMixins.
/// </summary>
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public static class ControlMixins
{
    /// <summary>
    /// Flattens a reactive tree and projects each item to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The projected value type.</typeparam>
    /// <param name="list">Root items observable.</param>
    /// <param name="selector">Projection from each tree item to an observable.</param>
    /// <returns>An observable of projected values.</returns>
    public static IObservable<T> FlattenAndSelect<T>(this IObservable<IEnumerable<ReactiveTreeItem>> list, Func<ReactiveTreeItem, IObservable<T>> selector)
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

    /// <summary>
    /// Flattens the specified list of reactive tree roots.
    /// </summary>
    /// <param name="list">The root list observable.</param>
    /// <returns>An observable sequence of flattened tree items.</returns>
    private static IObservable<ReactiveTreeItem> Flatten(this IObservable<IEnumerable<ReactiveTreeItem>> list) =>
        list
            .Select(items => FlattenItems(items ?? Enumerable.Empty<ReactiveTreeItem>()))
            .Switch();

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

    private static IObservable<ReactiveTreeItem> FlattenItem(ReactiveTreeItem item) =>
        Observable.Return(item)
            .Concat(
                item.Children.CurrentItems
                    .Select(children => FlattenItems(children ?? Enumerable.Empty<ReactiveTreeItem>()))
                    .Switch());
}
