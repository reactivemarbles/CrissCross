// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents an inclusive range of indexes in an array.</summary>
/// <param name="Min">The minimum index in the range.</param>
/// <param name="Max">The maximum index in the range.</param>
public readonly record struct IndexRange(int Min, int Max)
{
    /// <summary>Gets the range representing an empty collection.</summary>
    public static readonly IndexRange None = new(-1, -1, 0);

    /// <summary>Initializes a new instance of the <see cref="IndexRange"/> struct with an explicit length.</summary>
    /// <param name="min">The minimum index in the range.</param>
    /// <param name="max">The maximum index in the range.</param>
    /// <param name="length">The number of indexes in the range.</param>
    private IndexRange(int min, int max, int length)
        : this(min, max)
    {
        Length = length;
    }

    /// <summary>Gets the number of indexes in this inclusive range.</summary>
    public int Length { get; } = Max - Min + 1;

    /// <summary>Gets whether this range represents a valid non-negative index range.</summary>
    public bool IsValid => Min >= 0 && Max >= Min;
}
