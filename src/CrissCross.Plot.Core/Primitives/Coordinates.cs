// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a point in coordinate space (X and Y axis units).</summary>
/// <param name="x">The horizontal coordinate.</param>
/// <param name="y">The vertical coordinate.</param>
public struct Coordinates(double x, double y) : IEquatable<Coordinates>
{
    /// <summary>Gets a coordinate whose values are both NaN.</summary>
    public static Coordinates NaN => new(double.NaN, double.NaN);

    /// <summary>Gets the coordinate at the origin.</summary>
    public static Coordinates Zero => new(0, 0);

    /// <summary>Gets the origin coordinate.</summary>
    public static Coordinates Origin => new(0, 0);

    /// <summary>Gets a coordinate whose values are positive infinity.</summary>
    public static Coordinates Infinity => new(double.PositiveInfinity, double.PositiveInfinity);

    /// <summary>Gets or sets the horizontal coordinate.</summary>
    public double X { get; set; } = x;

    /// <summary>Gets or sets the vertical coordinate.</summary>
    public double Y { get; set; } = y;

    /// <summary>Gets whether both coordinates are finite real values.</summary>
    public bool AreReal => NumericConversion.IsReal(X) && NumericConversion.IsReal(Y);

    /// <summary>Gets the coordinate with its axes exchanged.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2400", Justification = "Exchanging axes intentionally passes the vertical value as the new horizontal coordinate.")]
    public readonly Coordinates Rotated => new(Y, X);

    /// <summary>Creates a coordinate for each corresponding pair of array values.</summary>
    /// <param name="xs">The horizontal coordinate values.</param>
    /// <param name="ys">The vertical coordinate values.</param>
    /// <returns>The coordinates formed from the paired values.</returns>
    public static Coordinates[] Zip(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have equal length");
        }

        Coordinates[] coordinates = new Coordinates[xs.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i] = new(xs[i], ys[i]);
        }

        return coordinates;
    }

    /// <summary>Determines whether two coordinates are equal.</summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns><see langword="true"/> when both coordinates are equal.</returns>
    public static bool operator ==(Coordinates a, Coordinates b) => a.Equals(b);

    /// <summary>Determines whether two coordinates differ.</summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns><see langword="true"/> when either coordinate differs.</returns>
    public static bool operator !=(Coordinates a, Coordinates b) => !a.Equals(b);

    /// <summary>Adds two coordinates component by component.</summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns>The component-wise sum.</returns>
    public static Coordinates operator +(Coordinates a, Coordinates b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>Adds two coordinates component by component.</summary>
    /// <param name="left">The first coordinate.</param>
    /// <param name="right">The second coordinate.</param>
    /// <returns>The component-wise sum.</returns>
    public static Coordinates Add(Coordinates left, Coordinates right) => left + right;

    /// <summary>Gets the squared Euclidean distance to another coordinate.</summary>
    /// <param name="pt">The coordinate to measure to.</param>
    /// <returns>The squared distance.</returns>
    public readonly double DistanceSquared(Coordinates pt)
    {
        double deltaX = Math.Abs(X - pt.X);
        double deltaY = Math.Abs(Y - pt.Y);
        return (deltaX * deltaX) + (deltaY * deltaY);
    }

    /// <summary>Gets the Euclidean distance to another coordinate.</summary>
    /// <param name="pt">The coordinate to measure to.</param>
    /// <returns>The distance.</returns>
    public readonly double Distance(Coordinates pt) => Math.Sqrt(DistanceSquared(pt));

    /// <summary>Formats this coordinate as text.</summary>
    /// <returns>A textual representation of the coordinate.</returns>
    public override readonly string ToString() => $"Coordinates {{ X = {X}, Y = {Y} }}";

    /// <summary>Determines whether this coordinate equals another coordinate.</summary>
    /// <param name="other">The coordinate to compare.</param>
    /// <returns><see langword="true"/> when both coordinates are equal.</returns>
    public readonly bool Equals(Coordinates other) => Equals(X, other.X) && Equals(Y, other.Y);

    /// <summary>Determines whether this coordinate equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal coordinate.</returns>
    public override readonly bool Equals(object? obj) => obj is Coordinates other && Equals(other);

    /// <summary>Gets a hash code for this coordinate.</summary>
    /// <returns>A hash code based on both coordinate values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1482", Justification = "The mutable coordinate properties are retained for public API compatibility.")]
    public override readonly int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

    /// <summary>Creates the rectangle centered on this coordinate with the specified radii.</summary>
    /// <param name="radiusX">The horizontal radius.</param>
    /// <param name="radiusY">The vertical radius.</param>
    /// <returns>The centered rectangle.</returns>
    public readonly CoordinateRect ToRect(double radiusX, double radiusY) => new(
        X - radiusX,
        X + radiusX,
        Y - radiusY,
        Y + radiusY);

    /// <summary>Creates a square centered on this coordinate.</summary>
    /// <param name="radius">The radius for both axes.</param>
    /// <returns>The centered square.</returns>
    public readonly CoordinateRect ToRect(double radius) => ToRect(radius, radius);

    /// <summary>Creates a translated copy of this coordinate.</summary>
    /// <param name="dX">The horizontal translation.</param>
    /// <param name="dY">The vertical translation.</param>
    /// <returns>The translated coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1305", Justification = "The parameter names are retained for named-argument compatibility.")]
    public readonly Coordinates WithDelta(double dX, double dY) => new(X + dX, Y + dY);

    /// <summary>Deconstructs this coordinate into its horizontal and vertical components.</summary>
    /// <returns>The horizontal and vertical values.</returns>
    public readonly (double X, double Y) Deconstruct() => (X, Y);
}
