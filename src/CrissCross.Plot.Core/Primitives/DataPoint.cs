// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a specific point in a data source.</summary>
public readonly struct DataPoint : IEquatable<DataPoint>
{
    /// <summary>Gets the horizontal coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public readonly double X;

    /// <summary>Gets the vertical coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public readonly double Y;

    /// <summary>Gets the data point's index.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public readonly int Index;

    /// <summary>Initializes a new instance of the <see cref="DataPoint"/> struct from coordinates and an index.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <param name="index">The data point index.</param>
    public DataPoint(double x, double y, int index)
    {
        X = x;
        Y = y;
        Index = index;
    }

    /// <summary>Initializes a new instance of the <see cref="DataPoint"/> struct from a coordinate and an index.</summary>
    /// <param name="coordinates">The point coordinates.</param>
    /// <param name="index">The data point index.</param>
    public DataPoint(Coordinates coordinates, int index)
    {
        X = coordinates.X;
        Y = coordinates.Y;
        Index = index;
    }

    /// <summary>Gets a sentinel point with NaN coordinates and an index of -1.</summary>
    public static DataPoint None => new(double.NaN, double.NaN, -1);

    /// <summary>Gets the point as a two-dimensional coordinate.</summary>
    public Coordinates Coordinates => new(X, Y);

    /// <summary>Gets whether both coordinates are finite real values.</summary>
    public bool IsReal => NumericConversion.IsReal(X) && NumericConversion.IsReal(Y);

    /// <summary>Determines whether two data points are equal.</summary>
    /// <param name="a">The first data point.</param>
    /// <param name="b">The second data point.</param>
    /// <returns><see langword="true"/> when all data point values are equal.</returns>
    public static bool operator ==(DataPoint a, DataPoint b) => a.Equals(b);

    /// <summary>Determines whether two data points differ.</summary>
    /// <param name="a">The first data point.</param>
    /// <param name="b">The second data point.</param>
    /// <returns><see langword="true"/> when any data point value differs.</returns>
    public static bool operator !=(DataPoint a, DataPoint b) => !a.Equals(b);

    /// <summary>Determines whether this data point equals another data point.</summary>
    /// <param name="other">The data point to compare.</param>
    /// <returns><see langword="true"/> when all values are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact value equality is required to remain consistent with the data point hash code.")]
    public bool Equals(DataPoint other) => X.Equals(other.X) && Y.Equals(other.Y) && Index == other.Index;

    /// <summary>Determines whether this data point equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal data point.</returns>
    public override bool Equals(object? obj) => obj is DataPoint other && Equals(other);

    /// <summary>Gets a hash code for this data point.</summary>
    /// <returns>A hash code based on its coordinates and index.</returns>
    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Index.GetHashCode();
}
