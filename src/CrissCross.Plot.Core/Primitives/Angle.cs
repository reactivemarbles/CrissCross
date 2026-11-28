// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot;

/// <summary>Represents an angle whose value can be expressed in degrees or radians.</summary>
public struct Angle : IEquatable<Angle>
{
    /// <summary>Gets the number of degrees in a full circle.</summary>
    private const double DegreesPerCircle = 360;

    /// <summary>Gets the number of degrees in a half circle.</summary>
    private const double DegreesPerHalfCircle = 180;

    /// <summary>Gets or sets the angle in degrees.</summary>
    public double Degrees { get; set; }

    /// <summary>Gets or sets the angle in radians.</summary>
    public double Radians
    {
        readonly get => Degrees * Math.PI / DegreesPerHalfCircle;
        set => Degrees = value * DegreesPerHalfCircle / Math.PI;
    }

    /// <summary>Gets the angle with the opposite direction.</summary>
    public readonly Angle Inverted => FromDegrees(-Degrees);

    /// <summary>Gets the equivalent angle in the range from zero to one full circle.</summary>
    public readonly Angle Normalized
    {
        get
        {
            double normalized = Degrees % DegreesPerCircle;
            double degrees = normalized < 0 ? normalized + DegreesPerCircle : normalized;
            return FromDegrees(degrees);
        }
    }

    /// <summary>Creates an angle from a degree value.</summary>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The equivalent angle.</returns>
    public static Angle FromDegrees(double degrees) => new() { Degrees = degrees };

    /// <summary>Creates an angle from a radian value.</summary>
    /// <param name="radians">The angle in radians.</param>
    /// <returns>The equivalent angle.</returns>
    public static Angle FromRadians(double radians) => new() { Radians = radians };

    /// <summary>Creates an angle from a fraction of a full circle.</summary>
    /// <param name="fraction">The fraction of a full circle.</param>
    /// <returns>The equivalent angle, measured counterclockwise.</returns>
    public static Angle FromFraction(double fraction) => FromFraction(fraction, false);

    /// <summary>Creates an angle from a fraction of a full circle.</summary>
    /// <param name="fraction">The fraction of a full circle.</param>
    /// <param name="clockwise">Whether to measure the angle clockwise.</param>
    /// <returns>The equivalent angle.</returns>
    public static Angle FromFraction(double fraction, bool clockwise)
    {
        if (clockwise)
        {
            fraction *= -1;
        }

        return new() { Degrees = DegreesPerCircle * fraction };
    }

    /// <summary>Creates an angle from a fraction of a full circle and a starting angle.</summary>
    /// <param name="fraction">The fraction of a full circle.</param>
    /// <param name="start">The starting angle.</param>
    /// <returns>The resulting angle, measured counterclockwise from the starting angle.</returns>
    public static Angle FromFraction(double fraction, Angle start) => FromFraction(fraction, start, false);

    /// <summary>Creates an angle from a fraction of a full circle and a starting angle.</summary>
    /// <param name="fraction">The fraction of a full circle.</param>
    /// <param name="start">The starting angle.</param>
    /// <param name="clockwise">Whether to measure the angle clockwise.</param>
    /// <returns>The resulting angle from the starting angle.</returns>
    public static Angle FromFraction(double fraction, Angle start, bool clockwise)
    {
        if (clockwise)
        {
            fraction *= -1;
        }

        return new() { Degrees = (DegreesPerCircle * fraction) + start.Degrees };
    }

    /// <summary>Returns the angle unchanged.</summary>
    /// <param name="item">The angle.</param>
    /// <returns>The same angle.</returns>
    public static Angle Plus(Angle item) => item;

    /// <summary>Returns the angle with its direction reversed.</summary>
    /// <param name="item">The angle.</param>
    /// <returns>The negated angle.</returns>
    public static Angle Negate(Angle item) => FromDegrees(-item.Degrees);

    /// <summary>Adds two angles.</summary>
    /// <param name="left">The first angle.</param>
    /// <param name="right">The second angle.</param>
    /// <returns>The sum of the angles.</returns>
    public static Angle Add(Angle left, Angle right) => FromDegrees(left.Degrees + right.Degrees);

    /// <summary>Subtracts one angle from another.</summary>
    /// <param name="left">The angle to subtract from.</param>
    /// <param name="right">The angle to subtract.</param>
    /// <returns>The difference between the angles.</returns>
    public static Angle Subtract(Angle left, Angle right) => FromDegrees(left.Degrees - right.Degrees);

    /// <summary>Multiplies the degree values of two angles.</summary>
    /// <param name="left">The first angle.</param>
    /// <param name="right">The second angle.</param>
    /// <returns>An angle whose degree value is the product.</returns>
    public static Angle Multiply(Angle left, Angle right) => FromDegrees(left.Degrees * right.Degrees);

    /// <summary>Divides the degree value of one angle by another.</summary>
    /// <param name="left">The angle to divide.</param>
    /// <param name="right">The divisor angle.</param>
    /// <returns>An angle whose degree value is the quotient.</returns>
    public static Angle Divide(Angle left, Angle right) => FromDegrees(left.Degrees / right.Degrees);

    /// <summary>Calculates the remainder of one angle's degree value divided by another.</summary>
    /// <param name="left">The angle to divide.</param>
    /// <param name="right">The divisor angle.</param>
    /// <returns>An angle whose degree value is the remainder.</returns>
    public static Angle Mod(Angle left, Angle right) => FromDegrees(left.Degrees % right.Degrees);

    /// <summary>Applies the unary plus operator to an angle.</summary>
    /// <param name="a">The angle.</param>
    /// <returns>The same angle.</returns>
    public static Angle operator +(Angle a) => FromDegrees(+a.Degrees);

    /// <summary>Negates an angle.</summary>
    /// <param name="a">The angle.</param>
    /// <returns>The negated angle.</returns>
    public static Angle operator -(Angle a) => FromDegrees(-a.Degrees);

    /// <summary>Adds two angles.</summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns>The sum of the angles.</returns>
    public static Angle operator +(Angle a, Angle b) => FromDegrees(a.Degrees + b.Degrees);

    /// <summary>Subtracts one angle from another.</summary>
    /// <param name="a">The angle to subtract from.</param>
    /// <param name="b">The angle to subtract.</param>
    /// <returns>The difference between the angles.</returns>
    public static Angle operator -(Angle a, Angle b) => FromDegrees(a.Degrees - b.Degrees);

    /// <summary>Multiplies an angle by a scalar.</summary>
    /// <param name="a">The angle.</param>
    /// <param name="b">The scalar.</param>
    /// <returns>The scaled angle.</returns>
    public static Angle operator *(Angle a, double b) => FromDegrees(a.Degrees * b);

    /// <summary>Multiplies a scalar by an angle.</summary>
    /// <param name="a">The scalar.</param>
    /// <param name="b">The angle.</param>
    /// <returns>The scaled angle.</returns>
    public static Angle operator *(double a, Angle b) => FromDegrees(a * b.Degrees);

    /// <summary>Divides an angle by a scalar.</summary>
    /// <param name="a">The angle.</param>
    /// <param name="b">The scalar divisor.</param>
    /// <returns>The scaled angle.</returns>
    public static Angle operator /(Angle a, double b) => FromDegrees(a.Degrees / b);

    /// <summary>Calculates the remainder of an angle's degree value and a scalar.</summary>
    /// <param name="a">The angle.</param>
    /// <param name="b">The scalar divisor.</param>
    /// <returns>The angle whose degree value is the remainder.</returns>
    public static Angle operator %(Angle a, double b) => FromDegrees(a.Degrees % b);

    /// <summary>Determines whether two angles have equal degree values.</summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns><see langword="true"/> when the angles are equal.</returns>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact floating-point equality preserves the established value semantics and equality/hash-code contract; tolerance-based equality would not be transitive.")]
    public static bool operator ==(Angle a, Angle b) => a.Degrees == b.Degrees;

    /// <summary>Determines whether two angles have different degree values.</summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns><see langword="true"/> when the angles are not equal.</returns>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact floating-point equality preserves the established value semantics; tolerance-based inequality would not be complementary to equality.")]
    public static bool operator !=(Angle a, Angle b) => a.Degrees != b.Degrees;

    /// <summary>Converts the angle to a descriptive string.</summary>
    /// <returns>The angle expressed in degrees.</returns>
    public override readonly string ToString() => $"Angle = {Degrees} degrees";

    /// <summary>Determines whether this angle equals another object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal angle.</returns>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact floating-point equality preserves the established value semantics and matching hash-code behavior; tolerance-based equality is not transitive.")]
    public override readonly bool Equals(object? obj) => obj is Angle other && Degrees == other.Degrees;

    /// <summary>Determines whether this angle equals another angle.</summary>
    /// <param name="other">The angle to compare.</param>
    /// <returns><see langword="true"/> when the angles have equal degree values.</returns>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact floating-point equality preserves the established value semantics and equality/hash-code contract; tolerance-based equality is not transitive.")]
    public readonly bool Equals(Angle other) => Degrees.Equals(other.Degrees);

    /// <summary>Gets the hash code for this angle's degree value.</summary>
    /// <returns>The hash code of the degree value.</returns>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1482",
        Justification = "The public mutable Degrees setter is retained for API compatibility; callers must not mutate an Angle while it is a hash-based collection key.")]
    public override readonly int GetHashCode() => Degrees.GetHashCode();
}
