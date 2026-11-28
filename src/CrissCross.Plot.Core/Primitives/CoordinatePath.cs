// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a path of coordinates and whether it is closed.</summary>
/// <param name="points">The points in the path.</param>
/// <param name="close">Whether the path connects its last point to its first.</param>
public readonly struct CoordinatePath(Coordinates[] points, bool close) : IEquatable<CoordinatePath>
{
    /// <summary>Gets the coordinate points in the path.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2322",
        Justification = "The mutable array preserves existing public API and mutation behavior.")]
    public readonly Coordinates[] Points = points;

    /// <summary>Gets whether the path connects its last point to its first.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public readonly bool Close = close;

    /// <summary>Creates a closed path from an existing point array.</summary>
    /// <param name="points">The path points.</param>
    /// <returns>The closed path.</returns>
    public static CoordinatePath Closed(Coordinates[] points) => new(points, true);

    /// <summary>Creates a closed path by enumerating the supplied points.</summary>
    /// <param name="points">The path points.</param>
    /// <returns>The closed path.</returns>
    public static CoordinatePath Closed(IEnumerable<Coordinates> points) => new(new List<Coordinates>(points).ToArray(), true);

    /// <summary>Creates an open path from an existing point array.</summary>
    /// <param name="points">The path points.</param>
    /// <returns>The open path.</returns>
    public static CoordinatePath Open(Coordinates[] points) => new(points, false);

    /// <summary>Creates an open path by enumerating the supplied points.</summary>
    /// <param name="points">The path points.</param>
    /// <returns>The open path.</returns>
    public static CoordinatePath Open(IEnumerable<Coordinates> points) => new(new List<Coordinates>(points).ToArray(), false);

    /// <summary>Determines whether two paths are equal.</summary>
    /// <param name="left">The first path.</param>
    /// <param name="right">The second path.</param>
    /// <returns><see langword="true"/> when the paths are equal.</returns>
    public static bool operator ==(CoordinatePath left, CoordinatePath right) => left.Equals(right);

    /// <summary>Determines whether two paths are different.</summary>
    /// <param name="left">The first path.</param>
    /// <param name="right">The second path.</param>
    /// <returns><see langword="true"/> when the paths differ.</returns>
    public static bool operator !=(CoordinatePath left, CoordinatePath right) => !left.Equals(right);

    /// <summary>Determines whether this path equals another path.</summary>
    /// <param name="other">The path to compare.</param>
    /// <returns><see langword="true"/> when the paths reference the same point array and have the same closed state.</returns>
    public bool Equals(CoordinatePath other) => ReferenceEquals(Points, other.Points) && Close == other.Close;

    /// <summary>Determines whether this path equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal path.</returns>
    public override bool Equals(object? obj) => obj is CoordinatePath other && Equals(other);

    /// <summary>Gets a hash code for this path.</summary>
    /// <returns>A hash code based on its point array and closed state.</returns>
    public override int GetHashCode() => HashCodeCombiner.Combine(Points, Close);
}
