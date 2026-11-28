// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Numerics;

namespace CrissCross.Plot;

/// <summary>Represents a vector at a point in pixel space.</summary>
/// <param name="point">The pixel at which the vector originates.</param>
/// <param name="vector">The vector components.</param>
public struct RootedPixelVector(Pixel point, Vector2 vector) : IEquatable<RootedPixelVector>
{
    /// <summary>Gets or sets the pixel at which the vector originates.</summary>
    public Pixel Point { get; set; } = point;

    /// <summary>Gets or sets the vector components.</summary>
    public Vector2 Vector { get; set; } = vector;

    /// <summary>Gets the angle of the vector in radians.</summary>
    public readonly float Angle => (float)Math.Atan2(Vector.Y, Vector.X);

    /// <summary>Gets the squared length of the vector in pixel units.</summary>
    public readonly float MagnitudeSquared => Vector.LengthSquared();

    /// <summary>Gets the length of the vector in pixel units.</summary>
    public readonly float Magnitude => Vector.Length();

    /// <summary>Determines whether two rooted pixel vectors are equal.</summary>
    public static bool operator ==(RootedPixelVector left, RootedPixelVector right) => left.Equals(right);

    /// <summary>Determines whether two rooted pixel vectors are not equal.</summary>
    public static bool operator !=(RootedPixelVector left, RootedPixelVector right) => !left.Equals(right);

    /// <inheritdoc />
    public readonly bool Equals(RootedPixelVector other) => Point.Equals(other.Point) && Vector.Equals(other.Vector);

    /// <inheritdoc />
    public override readonly bool Equals(object? obj) => obj is RootedPixelVector other && Equals(other);

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "The struct's mutable public properties are retained for API compatibility, so its value hash can change when callers mutate it.")]
    public override readonly int GetHashCode() => Point.GetHashCode() ^ Vector.GetHashCode();
}
