// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using CrissCross.Plot.Collections;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the bounded, circular collection behavior.</summary>
public sealed class CircularBufferTests
{
    /// <summary>The capacity used by wraparound scenarios.</summary>
    private const int BufferCapacity = 3;

    /// <summary>The capacity used by two-item scenarios.</summary>
    private const int SmallBufferCapacity = 2;

    /// <summary>The first value in a simple ordered sequence.</summary>
    private const int FirstItem = 1;

    /// <summary>The second value in a simple ordered sequence.</summary>
    private const int SecondItem = 2;

    /// <summary>The third value in a simple ordered sequence.</summary>
    private const int ThirdItem = 3;

    /// <summary>The fourth value in a simple ordered sequence.</summary>
    private const int FourthItem = 4;

    /// <summary>The fifth value in a simple ordered sequence.</summary>
    private const int FifthItem = 5;

    /// <summary>The value assigned through the indexer in the mutation test.</summary>
    private const int UpdatedItem = 40;

    /// <summary>The value inserted before a buffer reaches capacity.</summary>
    private const int FirstInsertedItem = 9;

    /// <summary>The value inserted into a full buffer.</summary>
    private const int SecondInsertedItem = 8;

    /// <summary>The first value used by search scenarios.</summary>
    private const int SearchFirstItem = 10;

    /// <summary>The second value used by search scenarios.</summary>
    private const int SearchSecondItem = 20;

    /// <summary>The third value used by search scenarios.</summary>
    private const int SearchThirdItem = 30;

    /// <summary>A value absent from the search buffer.</summary>
    private const int MissingItem = 99;

    /// <summary>The logical index of the second item.</summary>
    private const int SecondIndex = 2;

    /// <summary>The logical index immediately beyond a full buffer.</summary>
    private const int ThirdIndex = 3;

    /// <summary>An insertion index beyond the valid range.</summary>
    private const int FourthIndex = 4;

    /// <summary>The destination length used when testing offset copying.</summary>
    private const int DestinationLength = 4;

    /// <summary>Checks that capacities below one are rejected.</summary>
    /// <param name="capacity">The invalid capacity.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public async Task Constructor_InvalidCapacity_Throws(int capacity) =>
        await Assert.That(() => new CircularBuffer<int>(capacity)).Throws<ArgumentException>();

    /// <summary>Checks that a new buffer has the expected empty and collection state.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Constructor_InitializesEmptyBuffer()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);

        await Assert.That(buffer.Capacity).IsEqualTo(BufferCapacity);
        await Assert.That(buffer.Count).IsEqualTo(0);
        await Assert.That(buffer.IsEmpty).IsTrue();
        await Assert.That(buffer.IsFull).IsFalse();
        await Assert.That(buffer.IsReadOnly).IsFalse();
        await Assert.That(buffer.IsFixedSize).IsTrue();
        await Assert.That(buffer.IsSynchronized).IsFalse();
        await Assert.That(buffer.SyncRoot).IsNotNull();
    }

    /// <summary>Checks that additions preserve insertion order until the capacity is reached.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Add_BeforeCapacity_PreservesItemsInOrder()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);

        await AssertContents(buffer, FirstItem, SecondItem);
        await Assert.That(buffer.IsFull).IsFalse();
    }

    /// <summary>Checks that adding to a full buffer replaces the oldest item in logical order.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Add_WhenFull_ReplacesOldestAndWraps()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);
        buffer.Add(ThirdItem);
        buffer.Add(FourthItem);
        buffer.Add(FifthItem);

        await AssertContents(buffer, ThirdItem, FourthItem, FifthItem);
        await Assert.That(buffer.IsFull).IsTrue();
    }

    /// <summary>Checks that indexed access follows logical order after the buffer wraps.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Indexer_AfterWrap_ReadsAndUpdatesLogicalItem()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);
        buffer.Add(ThirdItem);
        buffer.Add(FourthItem);
        buffer[1] = UpdatedItem;

        await Assert.That(buffer[0]).IsEqualTo(SecondItem);
        await Assert.That(buffer[1]).IsEqualTo(UpdatedItem);
        await Assert.That(buffer[SecondIndex]).IsEqualTo(FourthItem);
        await Assert.That(() => buffer[ThirdIndex]).Throws<InvalidOperationException>();
        await Assert.That(() => buffer[-1]).Throws<InvalidOperationException>();
    }

    /// <summary>Checks that insertion preserves order and evicts the oldest item at capacity.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Insert_InsertsAtLogicalPositionAndEvictsWhenFull()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);
        buffer.Insert(1, FirstInsertedItem);
        await AssertContents(buffer, FirstItem, FirstInsertedItem, SecondItem);

        buffer.Insert(SecondIndex, SecondInsertedItem);
        await AssertContents(buffer, FirstInsertedItem, SecondItem, SecondInsertedItem);
        await Assert.That(() => buffer.Insert(-1, 0)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => buffer.Insert(FourthIndex, 0)).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>Checks that removals retain the logical order of wrapped items.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RemoveAt_AfterWrap_PreservesRemainingItems()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);
        buffer.Add(ThirdItem);
        buffer.Add(FourthItem);

        buffer.RemoveAt(1);
        await AssertContents(buffer, SecondItem, FourthItem);
        buffer.RemoveAt(0);
        await AssertContents(buffer, FourthItem);
        buffer.RemoveAt(0);

        await Assert.That(buffer.IsEmpty).IsTrue();
        await Assert.That(() => buffer.RemoveAt(0)).Throws<InvalidOperationException>();
    }

    /// <summary>Checks search, containment, and removal behavior for present and missing values.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SearchAndRemove_FindsOnlyPresentItems()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(SearchFirstItem);
        buffer.Add(SearchSecondItem);
        buffer.Add(SearchThirdItem);

        await Assert.That(buffer.IndexOf(SearchSecondItem)).IsEqualTo(1);
        await Assert.That(buffer.IndexOf(MissingItem)).IsEqualTo(-1);
        await Assert.That(buffer.Contains(SearchFirstItem)).IsTrue();
        await Assert.That(buffer.Contains(MissingItem)).IsFalse();
        await Assert.That(buffer.Remove(SearchSecondItem)).IsTrue();
        await Assert.That(buffer.Remove(MissingItem)).IsFalse();
        await AssertContents(buffer, SearchFirstItem, SearchThirdItem);
    }

    /// <summary>Checks binary search results for wrapped data and invalid ranges.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BinarySearch_FindsItemsAndReturnsInsertionPoint()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);
        buffer.Add(ThirdItem);
        buffer.Add(FourthItem);
        IComparer<int> comparer = Comparer<int>.Default;

        await Assert.That(buffer.BinarySearch(0, buffer.Count, ThirdItem, comparer)).IsEqualTo(1);
        await Assert.That(buffer.BinarySearch(0, buffer.Count, FifthItem, comparer)).IsEqualTo(~BufferCapacity);
        await Assert.That(buffer.BinarySearch(buffer.Count, 0, FifthItem, comparer)).IsEqualTo(~buffer.Count);
        await Assert.That(() => buffer.BinarySearch(-1, 1, 1, comparer)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => buffer.BinarySearch(0, -1, 1, comparer)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => buffer.BinarySearch(SecondIndex, SecondIndex, 1, comparer)).Throws<ArgumentException>();
    }

    /// <summary>Checks that copying honors the destination offset and rejects insufficient space.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CopyTo_CopiesItemsAtOffset()
    {
        CircularBuffer<int> buffer = new(BufferCapacity);
        buffer.Add(SecondItem);
        buffer.Add(ThirdItem);
        int[] destination = new int[DestinationLength];

        buffer.CopyTo(destination, 1);

        await Assert.That(destination[0]).IsEqualTo(0);
        await Assert.That(destination[1]).IsEqualTo(SecondItem);
        await Assert.That(destination[SecondIndex]).IsEqualTo(ThirdItem);
        await Assert.That(destination[ThirdIndex]).IsEqualTo(0);
        await Assert.That(() => buffer.CopyTo(new int[1], 0)).Throws<ArgumentException>();
    }

    /// <summary>Checks array conversion, enumeration, clearing, and reuse.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToArrayAndEnumerators_PreserveLogicalOrder()
    {
        CircularBuffer<int> buffer = new(SmallBufferCapacity);
        int[] empty = buffer.ToArray();
        buffer.Add(FirstItem);
        buffer.Add(SecondItem);
        buffer.Add(ThirdItem);
        int[] copy = buffer.ToArray();
        copy[0] = MissingItem;

        await Assert.That(empty.Length).IsEqualTo(0);
        await AssertContents(buffer, SecondItem, ThirdItem);
        await Assert.That(buffer.SequenceEqual([SecondItem, ThirdItem])).IsTrue();
        IEnumerator enumerator = ((IEnumerable)buffer).GetEnumerator();
        await Assert.That(enumerator.MoveNext()).IsTrue();
        await Assert.That(enumerator.Current).IsEqualTo(SecondItem);
        await Assert.That(enumerator.MoveNext()).IsTrue();
        await Assert.That(enumerator.Current).IsEqualTo(ThirdItem);
        await Assert.That(enumerator.MoveNext()).IsFalse();

        buffer.Clear();
        buffer.Add(FourthItem);
        await AssertContents(buffer, FourthItem);
    }

    /// <summary>Asserts that the buffer contains exactly the expected items in logical order.</summary>
    /// <param name="buffer">The buffer to inspect.</param>
    /// <param name="expected">The expected logical items.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    private static async Task AssertContents(CircularBuffer<int> buffer, params int[] expected)
    {
        await Assert.That(buffer.Count).IsEqualTo(expected.Length);
        for (int index = 0; index < expected.Length; index++)
        {
            await Assert.That(buffer[index]).IsEqualTo(expected[index]);
        }
    }
}
