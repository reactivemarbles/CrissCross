// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a range of pixels on the vertical axis, with <see cref="Top"/> no greater than <see cref="Bottom"/>.</summary>
/// <param name="y1">The first vertical coordinate.</param>
/// <param name="y2">The second vertical coordinate.</param>
public struct PixelRangeY(float y1, float y2) : IEquatable<PixelRangeY>
{
    /// <summary>Gets the smallest vertical coordinate in the range.</summary>
    public float Top { get; private set; } = Math.Min(y1, y2);

    /// <summary>Gets the largest vertical coordinate in the range.</summary>
    public float Bottom { get; private set; } = Math.Max(y1, y2);

    /// <summary>Gets the distance between the range endpoints.</summary>
    public readonly float Span => Bottom - Top;

    /// <summary>Determines whether two ranges are equal.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when both endpoints match.</returns>
    public static bool operator ==(PixelRangeY left, PixelRangeY right) => left.Equals(right);

    /// <summary>Determines whether two ranges differ.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when either endpoint differs.</returns>
    public static bool operator !=(PixelRangeY left, PixelRangeY right) => !left.Equals(right);

    /// <summary>Expands this range to include a vertical coordinate.</summary>
    /// <param name="value">The coordinate to include.</param>
    public void Expand(float value)
    {
        if (value < Top)
        {
            Top = value;
        }

        if (value <= Bottom)
        {
            return;
        }

        Bottom = value;
    }

    /// <summary>Formats this range as text.</summary>
    /// <returns>A textual representation of this range.</returns>
    public override readonly string ToString() => $"Top={Top}, Bottom={Bottom}, Span={Span}";

    /// <summary>Determines whether a vertical coordinate is within this range, inclusive.</summary>
    /// <param name="position">The coordinate to test.</param>
    /// <returns><see langword="true"/> when the coordinate lies within the range.</returns>
    public readonly bool Contains(float position) => position >= Top && position <= Bottom;

    /// <summary>Determines whether this range equals another range.</summary>
    /// <param name="other">The range to compare.</param>
    /// <returns><see langword="true"/> when both endpoints match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact endpoint equality is required to remain consistent with the hash code.")]
    public readonly bool Equals(PixelRangeY other) => Top.Equals(other.Top) && Bottom.Equals(other.Bottom);

    /// <summary>Determines whether this range equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal range.</returns>
    public override readonly bool Equals(object? obj) => obj is PixelRangeY other && Equals(other);

    /// <summary>Gets a hash code for this range.</summary>
    /// <returns>A hash code based on both endpoints.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "This mutable range type retains value hashing for equality consistency; callers must not mutate it while used as a hash key.")]
    public override readonly int GetHashCode() => Top.GetHashCode() ^ Bottom.GetHashCode();
}
