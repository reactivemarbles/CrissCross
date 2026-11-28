// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Collections;

/// <summary>
/// The circular buffer starts with an empty list and grows to a maximum size.
/// When the buffer is full, adding or inserting a new item removes the first item in the buffer.
/// </summary>
/// <typeparam name="T">The type of items stored in the buffer.</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CircularBuffer<>.CircularBufferDebugView))]
public sealed class CircularBuffer<T> : IList<T>, IReadOnlyList<T>
{
    /// <summary>The backing list that stores the buffer's items.</summary>
    private readonly List<T> _buffer;

    /// <summary>The physical index of the first logical item.</summary>
    private int _start;

    /// <summary>The physical index where the next item is written.</summary>
    private int _end;

    /// <summary>Initializes a new instance of the <see cref="CircularBuffer{T}"/> class with the specified capacity.</summary>
    /// <param name="capacity">The maximum number of items the buffer can hold.</param>
    public CircularBuffer(int capacity)
        : this([], capacity, start: 0, end: 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CircularBuffer{T}"/> class with backing storage and indices.</summary>
    /// <param name="buffer">The backing storage.</param>
    /// <param name="capacity">The maximum number of items the buffer can hold.</param>
    /// <param name="start">The physical index of the first logical item.</param>
    /// <param name="end">The physical index where the next item is written.</param>
    internal CircularBuffer(List<T> buffer, int capacity, int start, int end)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Circular buffer must have a capacity greater than 0.", nameof(capacity));
        }

        _buffer = buffer;
        Capacity = capacity;
        _start = start;
        _end = end;
    }

    /// <summary>Gets the maximum number of items the buffer can hold.</summary>
    public int Capacity { get; }

    /// <summary>Gets a value indicating whether the buffer has reached its capacity.</summary>
    public bool IsFull => Count == Capacity;

    /// <summary>Gets a value indicating whether the buffer contains no items.</summary>
    public bool IsEmpty => Count == 0;

    /// <summary>Gets the number of items currently stored in the buffer.</summary>
    public int Count => _buffer.Count;

    /// <summary>Gets a value indicating whether the buffer is read-only.</summary>
    public bool IsReadOnly { get; }

    /// <summary>Gets a value indicating whether the buffer has a fixed size.</summary>
    public bool IsFixedSize { get; } = true;

    /// <summary>Gets the object used to synchronize access to the buffer.</summary>
    public object SyncRoot { get; } = new();

    /// <summary>Gets a value indicating whether access to the buffer is synchronized.</summary>
    public bool IsSynchronized { get; }

    /// <summary>Gets or sets the item at the specified logical index.</summary>
    /// <param name="index">The zero-based index of the item.</param>
    /// <returns>The item at the specified index.</returns>
    public T this[int index]
    {
        get
        {
            ValidateIndexInRange(index);
            return _buffer[InternalIndex(index)];
        }
        set
        {
            ValidateIndexInRange(index);
            _buffer[InternalIndex(index)] = value;
        }
    }

    /// <summary>Finds the first occurrence of an item in the buffer.</summary>
    /// <param name="item">The item to locate.</param>
    /// <returns>The zero-based index of the item, or -1 if it is not present.</returns>
    public int IndexOf(T item)
    {
        for (var index = 0; index < Count; ++index)
        {
            if (EqualityComparer<T>.Default.Equals(this[index], item))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>Inserts an item at the specified index, dropping the oldest item if full.</summary>
    /// <param name="index">The zero-based index at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        List<T> items = [];
        items.AddRange(this);

        if (IsFull)
        {
            items.RemoveAt(0);
        }

        items.Insert(Math.Min(index, items.Count), item);
        _buffer.Clear();
        _buffer.AddRange(items);
        _start = 0;
        _end = items.Count == Capacity ? 0 : items.Count;
    }

    /// <summary>Removes the item at the specified index.</summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
        ValidateIndexInRange(index);

        var internalIndex = InternalIndex(index);
        _buffer.RemoveAt(internalIndex);
        if (internalIndex >= _end)
        {
            return;
        }

        Decrement(ref _end);
        if (_start <= 0)
        {
            return;
        }

        _start = _end;
    }

    /// <summary>Removes the first occurrence of an item from the buffer.</summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was removed; otherwise, <see langword="false"/>.</returns>
    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    /// <summary>Searches a range of the buffer for an item using binary search.</summary>
    /// <param name="index">The zero-based index of the range's first item.</param>
    /// <param name="count">The number of items in the range.</param>
    /// <param name="item">The item to locate.</param>
    /// <param name="comparer">The comparer used to order items.</param>
    /// <returns>The index of the item, or the bitwise complement of its insertion point.</returns>
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
        if (index < 0 || count < 0)
        {
            throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
        }

        if (Count - index < count)
        {
            throw new ArgumentException("Invalid offset length.");
        }

        int lo = index;
        int hi = index + count - 1;

        while (lo <= hi)
        {
            int i = lo + ((hi - lo) >> 1);
            int order = comparer.Compare(_buffer[InternalIndex(i)], item);

            if (order == 0)
            {
                return i;
            }

            if (order < 0)
            {
                lo = i + 1;
            }
            else
            {
                hi = i - 1;
            }
        }

        return ~lo;
    }

    /// <summary>Adds an item, replacing the oldest item if the buffer is full.</summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        if (IsFull)
        {
            _buffer[_end] = item;
            Increment(ref _end);
            _start = _end;
        }
        else
        {
            _buffer.Insert(_end, item);
            Increment(ref _end);
            if (_end != _buffer.Count)
            {
                _start = _end;
            }
        }
    }

    /// <summary>Removes all items from the buffer.</summary>
    public void Clear()
    {
        _start = 0;
        _end = 0;
        _buffer.Clear();
    }

    /// <summary>Determines whether the buffer contains an item.</summary>
    /// <param name="item">The item to locate.</param>
    /// <returns><see langword="true"/> if the item is present; otherwise, <see langword="false"/>.</returns>
    public bool Contains(T item) => IndexOf(item) != -1;

    /// <summary>Copies the buffer items to an array starting at the specified index.</summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in the destination array.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Array does not contain enough space for items");
        }

        for (var index = 0; index < Count; ++index)
        {
            array[index + arrayIndex] = this[index];
        }
    }

    /// <summary>Returns the buffer contents as a new array.</summary>
    /// <returns>An array containing the buffer items in logical order.</returns>
    public T[] ToArray()
    {
        if (IsEmpty)
        {
            return [];
        }

        var array = new T[Count];
        for (var index = 0; index < Count; ++index)
        {
            array[index] = this[index];
        }

        return array;
    }

    /// <summary>Returns an enumerator that iterates through the buffer.</summary>
    /// <returns>An enumerator over items in logical order.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; ++i)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Ensures an index refers to an existing buffer item.</summary>
    /// <param name="index">The zero-based index to validate.</param>
    private void ValidateIndexInRange(int index)
    {
        if (index >= 0 && index < Count)
        {
            return;
        }

        throw new InvalidOperationException($"Cannot access index {index}. Buffer size is {Count}");
    }

    /// <summary>Maps a logical index to its physical backing-list index.</summary>
    /// <param name="index">The logical index.</param>
    /// <returns>The corresponding physical index.</returns>
    private int InternalIndex(int index) => (_start + index) % _buffer.Count;

    /// <summary>Advances a circular index by one position.</summary>
    /// <param name="index">The index to advance.</param>
    private void Increment(ref int index)
    {
        index++;
        if (index < Capacity)
        {
            return;
        }

        index = 0;
    }

    /// <summary>Moves a circular index back by one position.</summary>
    /// <param name="index">The index to decrement.</param>
    private void Decrement(ref int index)
    {
        if (index <= 0)
        {
            index = Capacity - 1;
        }

        --index;
    }

    /// <summary>Provides debugger access to a circular buffer's items.</summary>
    /// <param name="collection">The circular buffer to display.</param>
    private sealed class CircularBufferDebugView(CircularBuffer<T> collection)
    {
        /// <summary>Gets a read-only view of the buffer items.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<T> Items => collection;
    }
}
