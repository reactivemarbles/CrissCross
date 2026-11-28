// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a sequence of pixel coordinates and whether the path is closed.</summary>
/// <param name="pixels">The pixel coordinates in the path.</param>
/// <param name="close"><see langword="true"/> to close the path.</param>
public readonly struct PixelPath(Pixel[] pixels, bool close) : IEquatable<PixelPath>
{
    /// <summary>Gets the pixel coordinates in this path.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "This mutable array field is retained for binary API compatibility.")]
    public readonly Pixel[] Pixels = pixels;

    /// <summary>Gets a value indicating whether this path is closed.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly bool Close = close;

    /// <summary>Creates a closed path from an array of pixels.</summary>
    /// <param name="pixels">The pixel coordinates.</param>
    /// <returns>The closed path.</returns>
    public static PixelPath Closed(Pixel[] pixels) => new(pixels, true);

    /// <summary>Creates a closed path from a sequence of pixels.</summary>
    /// <param name="pixels">The pixel coordinates.</param>
    /// <returns>The closed path.</returns>
    public static PixelPath Closed(IEnumerable<Pixel> pixels) => new(new List<Pixel>(pixels).ToArray(), true);

    /// <summary>Creates an open path from an array of pixels.</summary>
    /// <param name="pixels">The pixel coordinates.</param>
    /// <returns>The open path.</returns>
    public static PixelPath Open(Pixel[] pixels) => new(pixels, false);

    /// <summary>Creates an open path from a sequence of pixels.</summary>
    /// <param name="pixels">The pixel coordinates.</param>
    /// <returns>The open path.</returns>
    public static PixelPath Open(IEnumerable<Pixel> pixels) => new(new List<Pixel>(pixels).ToArray(), false);

    /// <summary>Determines whether two paths are equal.</summary>
    /// <param name="left">The first path.</param>
    /// <param name="right">The second path.</param>
    /// <returns><see langword="true"/> when the paths are equal.</returns>
    public static bool operator ==(PixelPath left, PixelPath right) => left.Equals(right);

    /// <summary>Determines whether two paths differ.</summary>
    /// <param name="left">The first path.</param>
    /// <param name="right">The second path.</param>
    /// <returns><see langword="true"/> when the paths differ.</returns>
    public static bool operator !=(PixelPath left, PixelPath right) => !left.Equals(right);

    /// <summary>Determines whether this path equals another path.</summary>
    /// <param name="other">The path to compare.</param>
    /// <returns><see langword="true"/> when the backing pixel array and closed state match.</returns>
    public bool Equals(PixelPath other) => ReferenceEquals(Pixels, other.Pixels) && Close == other.Close;

    /// <summary>Determines whether this path equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal path.</returns>
    public override bool Equals(object? obj) => obj is PixelPath other && Equals(other);

    /// <summary>Gets a hash code for this path.</summary>
    /// <returns>A hash code based on the backing array and closed state.</returns>
    public override int GetHashCode() => (Pixels?.GetHashCode() ?? 0) ^ Close.GetHashCode();
}
