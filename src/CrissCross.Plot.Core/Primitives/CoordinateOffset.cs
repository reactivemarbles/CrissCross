// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a distance between coordinates in axis units.</summary>
/// <param name="x">The horizontal offset.</param>
/// <param name="y">The vertical offset.</param>
public readonly struct CoordinateOffset(double x, double y) : IEquatable<CoordinateOffset>
{
    /// <summary>Gets the horizontal offset.</summary>
    public double X { get; } = x;

    /// <summary>Gets the vertical offset.</summary>
    public double Y { get; } = y;

    /// <summary>Determines whether two coordinate offsets are equal.</summary>
    public static bool operator ==(CoordinateOffset left, CoordinateOffset right) => left.Equals(right);

    /// <summary>Determines whether two coordinate offsets are not equal.</summary>
    public static bool operator !=(CoordinateOffset left, CoordinateOffset right) => !left.Equals(right);

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Value equality must compare exact components so it remains transitive and consistent with GetHashCode; a tolerance would violate the equality contract.")]
    public bool Equals(CoordinateOffset other) => X.Equals(other.X) && Y.Equals(other.Y);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CoordinateOffset other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
}
