// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a distance in pixel units.</summary>
/// <param name="length">The distance in pixels.</param>
public readonly struct PixelLength(float length) : IEquatable<PixelLength>
{
    /// <summary>Gets the distance in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float Length = length;

    /// <summary>Determines whether two distances are equal.</summary>
    /// <param name="left">The first distance.</param>
    /// <param name="right">The second distance.</param>
    /// <returns><see langword="true"/> when the distances are equal.</returns>
    public static bool operator ==(PixelLength left, PixelLength right) => left.Equals(right);

    /// <summary>Determines whether two distances differ.</summary>
    /// <param name="left">The first distance.</param>
    /// <param name="right">The second distance.</param>
    /// <returns><see langword="true"/> when the distances differ.</returns>
    public static bool operator !=(PixelLength left, PixelLength right) => !left.Equals(right);

    /// <summary>Converts a numeric pixel distance to a <see cref="PixelLength"/>.</summary>
    /// <param name="length">The distance in pixels.</param>
    public static implicit operator PixelLength(float length) => new(length);

    /// <summary>Formats this distance as text.</summary>
    /// <returns>A textual representation of the distance.</returns>
    public override string ToString() => $"{Length} pixels";

    /// <summary>Returns this value as a <see cref="PixelLength"/>.</summary>
    /// <returns>This pixel length.</returns>
    public PixelLength ToPixelLength() => this;

    /// <summary>Determines whether this distance equals another distance.</summary>
    /// <param name="other">The distance to compare.</param>
    /// <returns><see langword="true"/> when the distances are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality is required to remain consistent with the hash code.")]
    public bool Equals(PixelLength other) => Length.Equals(other.Length);

    /// <summary>Determines whether this distance equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal distance.</returns>
    public override bool Equals(object? obj) => obj is PixelLength other && Equals(other);

    /// <summary>Gets a hash code for this distance.</summary>
    /// <returns>A hash code based on the distance.</returns>
    public override int GetHashCode() => Length.GetHashCode();
}
