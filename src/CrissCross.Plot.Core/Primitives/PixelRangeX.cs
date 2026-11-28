// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a range of pixels on the horizontal axis, with <see cref="Left"/> no greater than <see cref="Right"/>.</summary>
/// <param name="x1">The first horizontal coordinate.</param>
/// <param name="x2">The second horizontal coordinate.</param>
public struct PixelRangeX(float x1, float x2) : IEquatable<PixelRangeX>
{
    /// <summary>Gets the smallest horizontal coordinate in the range.</summary>
    public float Left { get; private set; } = Math.Min(x1, x2);

    /// <summary>Gets the largest horizontal coordinate in the range.</summary>
    public float Right { get; private set; } = Math.Max(x1, x2);

    /// <summary>Gets the distance between the range endpoints.</summary>
    public readonly float Span => Right - Left;

    /// <summary>Determines whether two ranges are equal.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when both endpoints match.</returns>
    public static bool operator ==(PixelRangeX left, PixelRangeX right) => left.Equals(right);

    /// <summary>Determines whether two ranges differ.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when either endpoint differs.</returns>
    public static bool operator !=(PixelRangeX left, PixelRangeX right) => !left.Equals(right);

    /// <summary>Expands this range to include a horizontal coordinate.</summary>
    /// <param name="value">The coordinate to include.</param>
    public void Expand(float value)
    {
        if (value < Left)
        {
            Left = value;
        }

        if (value <= Right)
        {
            return;
        }

        Right = value;
    }

    /// <summary>Formats this range as text.</summary>
    /// <returns>A textual representation of this range.</returns>
    public override readonly string ToString() => $"Left={Left}, Right={Right}, Span={Span}";

    /// <summary>Determines whether a horizontal coordinate is within this range, inclusive.</summary>
    /// <param name="position">The coordinate to test.</param>
    /// <returns><see langword="true"/> when the coordinate lies within the range.</returns>
    public readonly bool Contains(float position) => position >= Left && position <= Right;

    /// <summary>Determines whether this range equals another range.</summary>
    /// <param name="other">The range to compare.</param>
    /// <returns><see langword="true"/> when both endpoints match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact endpoint equality is required to remain consistent with the hash code.")]
    public readonly bool Equals(PixelRangeX other) => Left.Equals(other.Left) && Right.Equals(other.Right);

    /// <summary>Determines whether this range equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal range.</returns>
    public override readonly bool Equals(object? obj) => obj is PixelRangeX other && Equals(other);

    /// <summary>Gets a hash code for this range.</summary>
    /// <returns>A hash code based on both endpoints.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "This mutable range type retains value hashing for equality consistency; callers must not mutate it while used as a hash key.")]
    public override readonly int GetHashCode() => Left.GetHashCode() ^ Right.GetHashCode();
}
