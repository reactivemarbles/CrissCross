// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents the minimum and maximum coordinates of a three-dimensional data set.</summary>
public readonly struct AxisLimits3d : IEquatable<AxisLimits3d>
{
    /// <summary>Gets the minimum X coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double XMin;

    /// <summary>Gets the maximum X coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double XMax;

    /// <summary>Gets the minimum Y coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double YMin;

    /// <summary>Gets the maximum Y coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double YMax;

    /// <summary>Gets the minimum Z coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double ZMin;

    /// <summary>Gets the maximum Z coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double ZMax;

    /// <summary>Gets the two-dimensional limits for the X and Y coordinates.</summary>
    public AxisLimits AxisLimits2d => new(XMin, XMax, YMin, YMax);

    /// <summary>Creates limits enclosing the provided three-dimensional coordinates.</summary>
    /// <param name="coordinates">The coordinates to enclose.</param>
    /// <returns>The minimum and maximum values for each axis.</returns>
    public static AxisLimits3d FromPoints(IEnumerable<Coordinates3d> coordinates)
    {
        CoordinateRangeMutable horizontalRange = new(double.MaxValue, double.MinValue);
        CoordinateRangeMutable verticalRange = new(double.MaxValue, double.MinValue);
        CoordinateRangeMutable depthRange = new(double.MaxValue, double.MinValue);
        foreach (Coordinates3d coordinate in coordinates)
        {
            horizontalRange.Expand(coordinate.X);
            verticalRange.Expand(coordinate.Y);
            depthRange.Expand(coordinate.Z);
        }

        return new(horizontalRange.Min, horizontalRange.Max, verticalRange.Min, verticalRange.Max, depthRange.Min, depthRange.Max);
    }

    /// <summary>Determines whether two sets of three-dimensional limits are equal.</summary>
    /// <param name="left">The first set of limits.</param>
    /// <param name="right">The second set of limits.</param>
    /// <returns><see langword="true"/> when all six bounds are equal.</returns>
    public static bool operator ==(AxisLimits3d left, AxisLimits3d right) => left.Equals(right);

    /// <summary>Determines whether two sets of three-dimensional limits are different.</summary>
    /// <param name="left">The first set of limits.</param>
    /// <param name="right">The second set of limits.</param>
    /// <returns><see langword="true"/> when any of the six bounds differ.</returns>
    public static bool operator !=(AxisLimits3d left, AxisLimits3d right) => !left.Equals(right);

    /// <summary>Determines whether this value equals another set of three-dimensional limits.</summary>
    /// <param name="other">The limits to compare.</param>
    /// <returns><see langword="true"/> when all six bounds are exactly equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact component equality is required for value equality and a hash code consistent with the compared bounds.")]
    public bool Equals(AxisLimits3d other) =>
        XMin.Equals(other.XMin)
        && XMax.Equals(other.XMax)
        && YMin.Equals(other.YMin)
        && YMax.Equals(other.YMax)
        && ZMin.Equals(other.ZMin)
        && ZMax.Equals(other.ZMax);

    /// <summary>Determines whether this value equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal set of limits.</returns>
    public override bool Equals(object? obj) => obj is AxisLimits3d other && Equals(other);

    /// <summary>Gets a hash code for this set of limits.</summary>
    /// <returns>A hash code based on all six coordinate bounds.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = XMin.GetHashCode();
            hashCode = (hashCode * 397) ^ XMax.GetHashCode();
            hashCode = (hashCode * 397) ^ YMin.GetHashCode();
            hashCode = (hashCode * 397) ^ YMax.GetHashCode();
            hashCode = (hashCode * 397) ^ ZMin.GetHashCode();
            return (hashCode * 397) ^ ZMax.GetHashCode();
        }
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits3d"/> class.</summary>
    /// <param name="minimumX">The minimum X coordinate.</param>
    /// <param name="maximumX">The maximum X coordinate.</param>
    /// <param name="minimumY">The minimum Y coordinate.</param>
    /// <param name="maximumY">The maximum Y coordinate.</param>
    /// <param name="minimumZ">The minimum Z coordinate.</param>
    /// <param name="maximumZ">The maximum Z coordinate.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1201",
        Justification = "Keep construction private; moving this constructor before public members violates the analyzer's accessibility ordering.")]
    private AxisLimits3d(double minimumX, double maximumX, double minimumY, double maximumY, double minimumZ, double maximumZ)
    {
        XMin = minimumX;
        XMax = maximumX;
        YMin = minimumY;
        YMax = maximumY;
        ZMin = minimumZ;
        ZMax = maximumZ;
    }
}
