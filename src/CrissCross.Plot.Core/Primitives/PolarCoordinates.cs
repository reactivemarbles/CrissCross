// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a point in polar coordinate space.</summary>
/// <param name="radius">The distance from the origin.</param>
/// <param name="angle">The direction from the origin.</param>
public struct PolarCoordinates(double radius, Angle angle) : IEquatable<PolarCoordinates>
{
    /// <summary>Gets or sets the distance from the origin.</summary>
    public double Radius { get; set; } = radius;

    /// <summary>Gets or sets the direction from the origin.</summary>
    public Angle Angle { get; set; } = angle;

    /// <summary>Gets the equivalent Cartesian coordinates.</summary>
    [Obsolete("use ToCartesian()", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "This obsolete public member is retained for API compatibility; callers are directed to ToCartesian().")]
    public readonly Coordinates CartesianCoordinates => ToCartesian();

    /// <summary>Determines whether two polar coordinate values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the radius and angle are equal.</returns>
    public static bool operator ==(PolarCoordinates left, PolarCoordinates right) => left.Equals(right);

    /// <summary>Determines whether two polar coordinate values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the values differ.</returns>
    public static bool operator !=(PolarCoordinates left, PolarCoordinates right) => !left.Equals(right);

    /// <summary>Creates polar coordinates from Cartesian coordinates relative to the origin.</summary>
    /// <param name="pt">The Cartesian coordinates.</param>
    /// <returns>The equivalent polar coordinates.</returns>
    public static PolarCoordinates FromCartesian(Coordinates pt) => FromCartesian(pt, Coordinates.Origin);

    /// <summary>Creates polar coordinates from Cartesian coordinates relative to a specified origin.</summary>
    /// <param name="pt">The Cartesian coordinates.</param>
    /// <param name="origin">The origin for the conversion.</param>
    /// <returns>The equivalent polar coordinates.</returns>
    public static PolarCoordinates FromCartesian(Coordinates pt, Coordinates origin)
    {
        double radius = pt.Distance(origin);
        double radians = Math.Atan2(pt.Y - origin.Y, pt.X - origin.X);
        return new(radius, Angle.FromRadians(radians));
    }

    /// <summary>Creates a copy with the specified radius.</summary>
    /// <param name="radius">The new radius.</param>
    /// <returns>The updated polar coordinates.</returns>
    public readonly PolarCoordinates WithRadius(double radius) => new(radius, Angle);

    /// <summary>Creates a copy with the specified angle.</summary>
    /// <param name="angle">The new angle.</param>
    /// <returns>The updated polar coordinates.</returns>
    public readonly PolarCoordinates WithAngle(Angle angle) => new(Radius, angle);

    /// <summary>Creates a copy with the specified angle in degrees.</summary>
    /// <param name="degrees">The new angle in degrees.</param>
    /// <returns>The updated polar coordinates.</returns>
    public readonly PolarCoordinates WithAngleDegrees(double degrees) => new(Radius, Angle.FromDegrees(degrees));

    /// <summary>Creates a copy with the specified angle in radians.</summary>
    /// <param name="radians">The new angle in radians.</param>
    /// <returns>The updated polar coordinates.</returns>
    public readonly PolarCoordinates WithAngleRadians(double radians) => new(Radius, Angle.FromRadians(radians));

    /// <summary>Formats these coordinates as text.</summary>
    /// <returns>A textual representation of these coordinates.</returns>
    public override readonly string ToString() => $"PolarCoordinates {{ Radius = {Radius}, {Angle} }}";

    /// <summary>Converts these coordinates to Cartesian coordinates relative to the origin.</summary>
    /// <returns>The equivalent Cartesian coordinates.</returns>
    public readonly Coordinates ToCartesian() => ToCartesian(Coordinates.Origin);

    /// <summary>Converts these coordinates to Cartesian coordinates relative to a specified origin.</summary>
    /// <param name="origin">The origin for the conversion.</param>
    /// <returns>The equivalent Cartesian coordinates.</returns>
    public readonly Coordinates ToCartesian(Coordinates origin)
    {
        double x = (Radius * Math.Cos(Angle.Radians)) + origin.X;
        double y = (Radius * Math.Sin(Angle.Radians)) + origin.Y;
        return new(x, y);
    }

    /// <summary>Determines whether these coordinates equal another value.</summary>
    /// <param name="other">The value to compare.</param>
    /// <returns><see langword="true"/> when the radius and angle are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Exact equality is required for value equality to remain consistent with the hash code.")]
    public readonly bool Equals(PolarCoordinates other) => Radius.Equals(other.Radius) && Angle.Equals(other.Angle);

    /// <summary>Determines whether these coordinates equal the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal polar coordinate value.</returns>
    public override readonly bool Equals(object? obj) => obj is PolarCoordinates other && Equals(other);

    /// <summary>Gets a hash code for these coordinates.</summary>
    /// <returns>A hash code based on the radius and angle.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "This mutable struct retains value hashing for equality consistency; callers must not mutate it while used as a hash key.")]
    public override readonly int GetHashCode() => Radius.GetHashCode() ^ Angle.GetHashCode();
}
