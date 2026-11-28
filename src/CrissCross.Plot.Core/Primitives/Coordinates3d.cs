// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a three-dimensional point in coordinate space.</summary>
/// <param name="x">The X coordinate.</param>
/// <param name="y">The Y coordinate.</param>
/// <param name="z">The Z coordinate.</param>
public struct Coordinates3d(double x, double y, double z) : IEquatable<Coordinates3d>
{
    /// <summary>Gets a coordinate whose values are all NaN.</summary>
    public static Coordinates3d NaN => new(double.NaN, double.NaN, double.NaN);

    /// <summary>Gets the origin coordinate.</summary>
    public static Coordinates3d Origin => new(0, 0, 0);

    /// <summary>Gets a coordinate whose values are positive infinity.</summary>
    public static Coordinates3d Infinity => new(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);

    /// <summary>Gets or sets the X coordinate.</summary>
    public double X { get; set; } = x;

    /// <summary>Gets or sets the Y coordinate.</summary>
    public double Y { get; set; } = y;

    /// <summary>Gets or sets the Z coordinate.</summary>
    public double Z { get; set; } = z;

    /// <summary>Gets whether all three coordinates are finite real values.</summary>
    public bool AreReal => NumericConversion.IsReal(X) && NumericConversion.IsReal(Y) && NumericConversion.IsReal(Z);

    /// <summary>Determines whether two coordinates are equal.</summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns><see langword="true"/> when all coordinates are equal.</returns>
    public static bool operator ==(Coordinates3d a, Coordinates3d b) => a.Equals(b);

    /// <summary>Determines whether two coordinates differ.</summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns><see langword="true"/> when any coordinate differs.</returns>
    public static bool operator !=(Coordinates3d a, Coordinates3d b) => !a.Equals(b);

    /// <summary>Gets the squared Euclidean distance to another coordinate.</summary>
    /// <param name="pt">The coordinate to measure to.</param>
    /// <returns>The squared distance.</returns>
    public readonly double DistanceSquared(Coordinates3d pt)
    {
        double deltaX = Math.Abs(X - pt.X);
        double deltaY = Math.Abs(Y - pt.Y);
        double deltaZ = Math.Abs(Z - pt.Z);
        return (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);
    }

    /// <summary>Gets the Euclidean distance to another coordinate.</summary>
    /// <param name="pt">The coordinate to measure to.</param>
    /// <returns>The distance.</returns>
    public readonly double Distance(Coordinates3d pt) => Math.Sqrt(DistanceSquared(pt));

    /// <summary>Formats this coordinate as text.</summary>
    /// <returns>A textual representation of the coordinate.</returns>
    public override readonly string ToString() => $"Coordinates {{ X = {X}, Y = {Y}, Z = {Z} }}";

    /// <summary>Determines whether this coordinate equals another coordinate.</summary>
    /// <param name="other">The coordinate to compare.</param>
    /// <returns><see langword="true"/> when all coordinates are equal.</returns>
    public readonly bool Equals(Coordinates3d other) => Equals(X, other.X) && Equals(Y, other.Y) && Equals(Z, other.Z);

    /// <summary>Determines whether this coordinate equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal coordinate.</returns>
    public override readonly bool Equals(object? obj) => obj is Coordinates3d other && Equals(other);

    /// <summary>Gets a hash code for this coordinate.</summary>
    /// <returns>A hash code based on all three values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1482", Justification = "The mutable coordinate properties are retained for public API compatibility.")]
    public override readonly int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

    /// <summary>Creates a translated copy of this coordinate.</summary>
    /// <param name="dX">The X translation.</param>
    /// <param name="dY">The Y translation.</param>
    /// <param name="dZ">The Z translation.</param>
    /// <returns>The translated coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1305", Justification = "Parameter names are retained for named-argument compatibility.")]
    public readonly Coordinates3d WithDelta(double dX, double dY, double dZ) => new(X + dX, Y + dY, Z + dZ);

    /// <summary>Gets the X and Y coordinates as a two-dimensional coordinate.</summary>
    /// <returns>The two-dimensional coordinate.</returns>
    public readonly Coordinates Coordinates2d() => new(X, Y);
}
