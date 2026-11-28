// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides signal data backed by a segmented tree for range queries.</summary>
/// <typeparam name="T">The numeric sample type.</typeparam>
public class SignalConstSource<T> : SignalSourceGenericArray<T>
    where T : struct, IComparable
{
    /// <summary>The segmented tree used to query minimum and maximum values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "SegmentedTree is an existing public field; changing it to a property would break binary compatibility.")]
    public readonly SegmentedTree<T> SegmentedTree = new();

    /// <summary>Initializes a new instance of the <see cref="SignalConstSource{T}"/> class.</summary>
    /// <param name="ys">The signal sample values.</param>
    /// <param name="period">The interval between samples.</param>
    public SignalConstSource(T[] ys, double period)
        : base(ys, period)
    {
        SegmentedTree.SourceArray = ys;
    }

    /// <summary>Gets the minimum and maximum values over the specified sample range.</summary>
    /// <param name="firstIndex">The first sample index in the range.</param>
    /// <param name="lastIndex">The last sample index in the range.</param>
    /// <returns>The minimum and maximum signal values in the range.</returns>
    public override SignalRangeY GetLimitsY(int firstIndex, int lastIndex)
    {
        SegmentedTree.MinMaxRangeQuery(firstIndex, lastIndex, out double min, out double max);
        return new(min, max);
    }
}
