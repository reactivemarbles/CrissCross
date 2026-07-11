// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CrissCross;

namespace CP.Reactive.Collections;

/// <summary>Observable collection compatibility type used by CrissCross controls.</summary>
/// <typeparam name="T">The item type.</typeparam>
public class ReactiveList<T> : ObservableCollection<T>, IDisposable
{
    /// <summary>Publishes snapshots of the current collection items.</summary>
    private readonly StateSignal<IEnumerable<T>> _currentItems = new([]);

    /// <summary>Tracks whether the list has been disposed.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="ReactiveList{T}"/> class.</summary>
    public ReactiveList()
    {
        _currentItems.OnNext(CreateSnapshot());
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveList{T}"/> class.</summary>
    /// <param name="collection">The initial items.</param>
    public ReactiveList(IEnumerable<T> collection)
        : base(collection)
    {
        _currentItems.OnNext(CreateSnapshot());
    }

    /// <summary>Gets the current items in the collection.</summary>
    public IObservable<IEnumerable<T>> CurrentItems => _currentItems;

    /// <summary>Applies a batch edit to the list and raises a reset notification.</summary>
    /// <param name="edit">The edit action.</param>
    public void Edit(Action<IList<T>> edit)
    {
        ThrowHelper.ThrowIfNull(edit, nameof(edit));
        edit(Items);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>Replaces all items in the collection.</summary>
    /// <param name="items">The replacement items.</param>
    public void ReplaceAll(IEnumerable<T> items)
    {
        ThrowHelper.ThrowIfNull(items, nameof(items));
        Edit(list =>
        {
            list.Clear();
            foreach (var item in items)
            {
                list.Add(item);
            }
        });
    }

    /// <summary>Clears the collection.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases managed resources used by the collection.</summary>
    /// <param name="disposing">A value indicating whether managed resources should be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Clear();
            _currentItems.OnCompleted();
            _currentItems.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc/>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        if (_disposed)
        {
            return;
        }

        _currentItems.OnNext(CreateSnapshot());
    }

    /// <summary>Creates an immutable snapshot of the current items.</summary>
    /// <returns>The current item snapshot.</returns>
    private T[] CreateSnapshot() => this.ToArray();
}
