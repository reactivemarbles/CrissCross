// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a width and height in coordinate units.</summary>
/// <param name="width">The size width.</param>
/// <param name="height">The size height.</param>
public struct CoordinateSize(double width, double height) : IEquatable<CoordinateSize>
{
    /// <summary>Gets or sets the width.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public double Width = width;

    /// <summary>Gets or sets the height.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public double Height = height;

    /// <summary>Gets the size area.</summary>
    public readonly double Area => Width * Height;

    /// <summary>Determines whether two sizes are equal.</summary>
    /// <param name="a">The first size.</param>
    /// <param name="b">The second size.</param>
    /// <returns><see langword="true"/> when both dimensions are equal.</returns>
    public static bool operator ==(CoordinateSize a, CoordinateSize b) => a.Equals(b);

    /// <summary>Determines whether two sizes differ.</summary>
    /// <param name="a">The first size.</param>
    /// <param name="b">The second size.</param>
    /// <returns><see langword="true"/> when either dimension differs.</returns>
    public static bool operator !=(CoordinateSize a, CoordinateSize b) => !a.Equals(b);

    /// <summary>Determines whether this size equals another size.</summary>
    /// <param name="other">The size to compare.</param>
    /// <returns><see langword="true"/> when both dimensions are equal.</returns>
    public readonly bool Equals(CoordinateSize other) => Equals(Width, other.Width) && Equals(Height, other.Height);

    /// <summary>Determines whether this size equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal size.</returns>
    public override readonly bool Equals(object? obj) => obj is CoordinateSize other && Equals(other);

    /// <summary>Gets a hash code for this size.</summary>
    /// <returns>A hash code based on its dimensions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1482", Justification = "Mutable public fields and value equality are retained for API compatibility.")]
    public override readonly int GetHashCode() => Width.GetHashCode() ^ Height.GetHashCode();
}
