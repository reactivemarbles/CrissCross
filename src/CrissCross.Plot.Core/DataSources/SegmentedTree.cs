// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CrissCross.Plot.DataSources;

/// <summary>Stores paired min/max values in a segment tree for range queries.</summary>
/// <typeparam name="T">The comparable value type stored in the tree.</typeparam>
public class SegmentedTree<T>
    where T : struct, IComparable
{
    /// <summary>The number of child nodes per segment-tree parent.</summary>
    private const int TreeBranchingFactor = 2;

    /// <summary>The largest supported power-of-two tree capacity.</summary>
    private const int MaximumTreeCapacity = 0x40_00_00_00;

    /// <summary>Combines two values by selecting the smaller value.</summary>
    private static readonly Func<T, T, T> _minExp = static (left, right) =>
        Comparer<T>.Default.Compare(left, right) <= 0 ? left : right;

    /// <summary>Combines two values by selecting the larger value.</summary>
    private static readonly Func<T, T, T> _maxExp = static (left, right) =>
        Comparer<T>.Default.Compare(left, right) >= 0 ? left : right;

    /// <summary>Determines whether two values are equal.</summary>
    private static readonly Func<T, T, bool> _equalExp = EqualityComparer<T>.Default.Equals;

    /// <summary>Determines whether the first value is smaller than the second.</summary>
    private static readonly Func<T, T, bool> _lessThanExp = static (left, right) =>
        Comparer<T>.Default.Compare(left, right) < 0;

    /// <summary>Determines whether the first value is larger than the second.</summary>
    private static readonly Func<T, T, bool> _greaterThanExp = static (left, right) =>
        Comparer<T>.Default.Compare(left, right) > 0;

    /// <summary>Indicates whether the segment tree has been built for the current source array.</summary>
    [SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    [SuppressMessage("StyleSharp", "SST1202", Justification = "Static helper fields must precede instance members under SST1204, and this existing public field must remain public.")]
    public bool TreesReady;

    /// <summary>Source values indexed by callers of this segment tree.</summary>
    private T[] _sourceArray = [];

    /// <summary>Minimum values stored at each segment-tree node.</summary>
    private T[] _treeMin = [];

    /// <summary>Maximum values stored at each segment-tree node.</summary>
    private T[] _treeMax = [];

    /// <summary>Number of leaves in the padded segment-tree representation.</summary>
    private int _n;

    /// <summary>Initializes a new instance of the <see cref="SegmentedTree{T}"/> class.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value type cannot be converted to a double.</exception>
    public SegmentedTree()
    {
        try
        {
            var value = default(T);
            _ = NumericConversion.GenericToDouble(ref value);
        }
        catch (Exception exception) when (exception is InvalidCastException or FormatException or OverflowException or ArgumentException)
        {
            throw new ArgumentOutOfRangeException(typeof(T).Name, typeof(T), "Unsupported data type; provide a type convertible to double.");
        }
    }

    /// <summary>Gets or sets the source values and rebuilds the segment tree.</summary>
    /// <exception cref="ArgumentNullException">The assigned array is <see langword="null"/>.</exception>
    [SuppressMessage("StyleSharp", "SST2305", Justification = "The settable property is part of the existing public API and remains the supported way to replace the source array.")]
    public T[] SourceArray
    {
        get => _sourceArray;
        set
        {
            _sourceArray = value ?? throw new ArgumentNullException(nameof(value));
            UpdateTrees();
        }
    }

    /// <summary>Sets the source values and rebuilds the segment tree on a worker thread.</summary>
    /// <param name="data">The values to store.</param>
    /// <returns>A task that completes when the tree has been rebuilt.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public async Task SetSourceAsync(T[] data)
    {
        _sourceArray = data ?? throw new ArgumentNullException(nameof(data));
        await Task.Run(UpdateTrees);
    }

    /// <summary>Replaces one source value and updates its segment-tree ancestors.</summary>
    /// <param name="index">The index of the value to replace.</param>
    /// <param name="newValue">The replacement value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the source array.</exception>
    public void updateElement(int index, T newValue)
    {
        if ((uint)index >= (uint)_sourceArray.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _sourceArray[index] = newValue;
        var nodeIndex = (_n / TreeBranchingFactor) + (index / TreeBranchingFactor);

        if (index == _sourceArray.Length - 1)
        {
            _treeMin[nodeIndex] = _sourceArray[index];
            _treeMax[nodeIndex] = _sourceArray[index];
        }
        else if (index % TreeBranchingFactor == 0)
        {
            _treeMin[nodeIndex] = _minExp(_sourceArray[index], _sourceArray[index + 1]);
            _treeMax[nodeIndex] = _maxExp(_sourceArray[index], _sourceArray[index + 1]);
        }
        else
        {
            _treeMin[nodeIndex] = _minExp(_sourceArray[index], _sourceArray[index - 1]);
            _treeMax[nodeIndex] = _maxExp(_sourceArray[index], _sourceArray[index - 1]);
        }

        UpdateAncestors(nodeIndex / TreeBranchingFactor, updateMinimum: true);
        UpdateAncestors(nodeIndex / TreeBranchingFactor, updateMinimum: false);
    }

    /// <summary>Updates the source range and recalculates affected segment-tree nodes.</summary>
    /// <param name="from">The first source index to replace, inclusive.</param>
    /// <param name="to">The first source index not to replace.</param>
    /// <param name="newData">The array containing replacement values.</param>
    /// <param name="fromData">The index in <paramref name="newData"/> from which to copy.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newData"/> is <see langword="null"/>.</exception>
    [SuppressMessage("StyleSharp", "SST2309", Justification = "The optional parameter default is part of the existing public API and is retained for source compatibility.")]
    public void updateRange(int from, int to, T[] newData, int fromData = 0)
    {
        newData = newData ?? throw new ArgumentNullException(nameof(newData));

        for (int i = from; i < to; i++)
        {
            _sourceArray[i] = newData[i - from + fromData];
        }

        for (int i = (_n / TreeBranchingFactor) + (from / TreeBranchingFactor); i < (_n / TreeBranchingFactor) + (to / TreeBranchingFactor); i++)
        {
            _treeMin[i] = _minExp(_sourceArray[(i * TreeBranchingFactor) - _n], _sourceArray[(i * TreeBranchingFactor) + 1 - _n]);
            _treeMax[i] = _maxExp(_sourceArray[(i * TreeBranchingFactor) - _n], _sourceArray[(i * TreeBranchingFactor) + 1 - _n]);
        }

        if (to == _sourceArray.Length)
        {
            _treeMin[(_n / TreeBranchingFactor) + (to / TreeBranchingFactor)] = _sourceArray[to - 1];
            _treeMax[(_n / TreeBranchingFactor) + (to / TreeBranchingFactor)] = _sourceArray[to - 1];
        }
        else if (to % TreeBranchingFactor != 0)
        {
            _treeMin[(_n / TreeBranchingFactor) + (to / TreeBranchingFactor)] = _minExp(_sourceArray[to - 1], _sourceArray[to]);
            _treeMax[(_n / TreeBranchingFactor) + (to / TreeBranchingFactor)] = _maxExp(_sourceArray[to - 1], _sourceArray[to]);
        }

        var firstAncestor = ((_n / TreeBranchingFactor) + (from / TreeBranchingFactor)) / TreeBranchingFactor;
        var lastAncestor = ((_n / TreeBranchingFactor) + (to / TreeBranchingFactor)) / TreeBranchingFactor;
        UpdateRangeAncestors(firstAncestor, lastAncestor);
    }

    /// <summary>Updates source values starting at <paramref name="from"/>.</summary>
    /// <param name="from">The first source index to replace.</param>
    /// <param name="newData">The values to copy into the source array.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newData"/> is <see langword="null"/>.</exception>
    public void updateData(int from, T[] newData) =>
        updateRange(from, newData?.Length ?? throw new ArgumentNullException(nameof(newData)), newData);

    /// <summary>Replaces all source values with <paramref name="newData"/>.</summary>
    /// <param name="newData">The replacement source values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newData"/> is <see langword="null"/>.</exception>
    public void updateData(T[] newData) =>
        updateRange(0, newData?.Length ?? throw new ArgumentNullException(nameof(newData)), newData);

    /// <summary>Rebuilds the segment tree asynchronously on a worker thread.</summary>
    public void UpdateTreesInBackground() => _ = Task.Run(UpdateTrees);

    /// <summary>Builds the segment tree for the current source array.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The source is empty or exceeds the supported tree capacity.</exception>
    public void UpdateTrees()
    {
        TreesReady = false;
        var sourceArray = _sourceArray;

        if (sourceArray.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(SourceArray), "The source array cannot be empty.");
        }

        if (sourceArray.Length > MaximumTreeCapacity)
        {
            throw new ArgumentOutOfRangeException(nameof(SourceArray), sourceArray.Length, $"The source array cannot exceed {MaximumTreeCapacity} elements.");
        }

        var treeSize = 1;
        while (treeSize < MaximumTreeCapacity && treeSize < sourceArray.Length)
        {
            treeSize *= TreeBranchingFactor;
        }

        var leafStart = treeSize / TreeBranchingFactor;
        var treeMin = new T[treeSize];
        var treeMax = new T[treeSize];

        for (int i = 0; i < sourceArray.Length / TreeBranchingFactor; i++)
        {
            var left = sourceArray[i * TreeBranchingFactor];
            var right = sourceArray[(i * TreeBranchingFactor) + 1];
            treeMin[leafStart + i] = _minExp(left, right);
            treeMax[leafStart + i] = _maxExp(left, right);
        }

        if (sourceArray.Length % TreeBranchingFactor != 0)
        {
            var lastValue = sourceArray[^1];
            treeMin[leafStart + (sourceArray.Length / TreeBranchingFactor)] = lastValue;
            treeMax[leafStart + (sourceArray.Length / TreeBranchingFactor)] = lastValue;
        }

        var paddingValue = sourceArray[^1];
        for (int i = leafStart + ((sourceArray.Length + 1) / TreeBranchingFactor); i < treeSize; i++)
        {
            treeMin[i] = paddingValue;
            treeMax[i] = paddingValue;
        }

        for (int i = leafStart - 1; i > 0; i--)
        {
            treeMin[i] = _minExp(treeMin[TreeBranchingFactor * i], treeMin[(TreeBranchingFactor * i) + 1]);
            treeMax[i] = _maxExp(treeMax[TreeBranchingFactor * i], treeMax[(TreeBranchingFactor * i) + 1]);
        }

        _n = treeSize;
        _treeMin = treeMin;
        _treeMax = treeMax;
        TreesReady = true;
    }

    /// <summary>Returns the minimum and maximum values in the requested source range.</summary>
    /// <param name="l">The first source index in the range.</param>
    /// <param name="r">The ending source index used by the existing range-query convention.</param>
    /// <param name="lowestValue">Receives the minimum value converted to a double.</param>
    /// <param name="highestValue">Receives the maximum value converted to a double.</param>
    public void MinMaxRangeQuery(int l, int r, out double lowestValue, out double highestValue)
    {
        if (!TreesReady)
        {
            MinMaxSourceRangeQuery(l, r, out lowestValue, out highestValue);
            return;
        }

        MinMaxTreeRangeQuery(l, r, out lowestValue, out highestValue);
    }

    /// <summary>Calculates a range minimum and maximum directly from the source array.</summary>
    /// <param name="l">The first source index in the range.</param>
    /// <param name="r">The ending source index used by the existing range-query convention.</param>
    /// <param name="lowestValue">Receives the minimum value converted to a double.</param>
    /// <param name="highestValue">Receives the maximum value converted to a double.</param>
    private void MinMaxSourceRangeQuery(int l, int r, out double lowestValue, out double highestValue)
    {
        var lowestValueT = _sourceArray[l];
        var highestValueT = lowestValueT;

        for (int i = l; i < r; i++)
        {
            if (_lessThanExp(_sourceArray[i], lowestValueT))
            {
                lowestValueT = _sourceArray[i];
            }

            if (_greaterThanExp(_sourceArray[i], highestValueT))
            {
                highestValueT = _sourceArray[i];
            }
        }

        lowestValue = NumericConversion.GenericToDouble(ref lowestValueT);
        highestValue = NumericConversion.GenericToDouble(ref highestValueT);
    }

    /// <summary>Calculates a range minimum and maximum using the segment tree.</summary>
    /// <param name="l">The first source index in the range.</param>
    /// <param name="r">The ending source index used by the existing range-query convention.</param>
    /// <param name="lowestValue">Receives the minimum value converted to a double.</param>
    /// <param name="highestValue">Receives the maximum value converted to a double.</param>
    private void MinMaxTreeRangeQuery(int l, int r, out double lowestValue, out double highestValue)
    {
        if (l == r)
        {
            lowestValue = NumericConversion.GenericToDouble(ref _sourceArray[l]);
            highestValue = lowestValue;
            return;
        }

        var lowestValueT = _sourceArray[l];
        var highestValueT = lowestValueT;

        if ((l & 1) == 1)
        {
            lowestValueT = _minExp(lowestValueT, _sourceArray[l]);
            highestValueT = _maxExp(highestValueT, _sourceArray[l]);
        }

        if ((r & 1) != 1)
        {
            lowestValueT = _minExp(lowestValueT, _sourceArray[r]);
            highestValueT = _maxExp(highestValueT, _sourceArray[r]);
        }

        l = (l + _n + 1) / TreeBranchingFactor;
        r = (r + _n - 1) / TreeBranchingFactor;

        while (l <= r)
        {
            if ((l & 1) == 1)
            {
                lowestValueT = _minExp(lowestValueT, _treeMin[l]);
                highestValueT = _maxExp(highestValueT, _treeMax[l]);
            }

            if ((r & 1) != 1)
            {
                lowestValueT = _minExp(lowestValueT, _treeMin[r]);
                highestValueT = _maxExp(highestValueT, _treeMax[r]);
            }

            l = (l + 1) / TreeBranchingFactor;
            r = (r - 1) / TreeBranchingFactor;
        }

        lowestValue = NumericConversion.GenericToDouble(ref lowestValueT);
        highestValue = NumericConversion.GenericToDouble(ref highestValueT);
    }

    /// <summary>Recalculates the ancestors of a node until reaching an unchanged aggregate.</summary>
    /// <param name="nodeIndex">The first node index to recalculate.</param>
    /// <param name="updateMinimum"><see langword="true"/> to update minimum nodes; otherwise update maximum nodes.</param>
    private void UpdateAncestors(int nodeIndex, bool updateMinimum)
    {
        var tree = updateMinimum ? _treeMin : _treeMax;
        var combine = updateMinimum ? _minExp : _maxExp;

        for (int i = nodeIndex; i > 0; i /= TreeBranchingFactor)
        {
            var candidate = combine(tree[i * TreeBranchingFactor], tree[(i * TreeBranchingFactor) + 1]);
            if (_equalExp(tree[i], candidate))
            {
                break;
            }

            tree[i] = candidate;
        }
    }

    /// <summary>Recalculates all ancestors affected by a range update.</summary>
    /// <param name="firstAncestor">The first affected ancestor at the lowest changed level.</param>
    /// <param name="lastAncestor">The last affected ancestor at the lowest changed level.</param>
    private void UpdateRangeAncestors(int firstAncestor, int lastAncestor)
    {
        while (firstAncestor != 0)
        {
            if (firstAncestor == lastAncestor)
            {
                UpdateAncestors(firstAncestor, updateMinimum: true);
                UpdateAncestors(firstAncestor, updateMinimum: false);
                return;
            }

            for (int i = firstAncestor; i <= lastAncestor; i++)
            {
                RecalculateNode(i);
            }

            firstAncestor /= TreeBranchingFactor;
            lastAncestor /= TreeBranchingFactor;
        }
    }

    /// <summary>Recalculates the minimum and maximum aggregate for one tree node.</summary>
    /// <param name="nodeIndex">The node to recalculate from its children.</param>
    private void RecalculateNode(int nodeIndex)
    {
        _treeMin[nodeIndex] = _minExp(_treeMin[nodeIndex * TreeBranchingFactor], _treeMin[(nodeIndex * TreeBranchingFactor) + 1]);
        _treeMax[nodeIndex] = _maxExp(_treeMax[nodeIndex * TreeBranchingFactor], _treeMax[(nodeIndex * TreeBranchingFactor) + 1]);
    }
}
