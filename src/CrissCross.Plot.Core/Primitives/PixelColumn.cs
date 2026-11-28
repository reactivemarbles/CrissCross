// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a vertical pixel column representing the Y span of an X range of data points.</summary>
/// <param name="x">The X coordinate of the pixel column.</param>
/// <param name="enter">The first edge value in the column.</param>
/// <param name="exit">The second edge value in the column.</param>
/// <param name="bottom">The bottom Y coordinate.</param>
/// <param name="top">The top Y coordinate.</param>
public readonly struct PixelColumn(float x, float enter, float exit, float bottom, float top) : IEquatable<PixelColumn>
{
    /// <summary>Gets the X coordinate of the pixel column.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields are retained for compatibility with the existing data-structure API.")]
    public readonly float X = x;

    /// <summary>Gets the first edge value in the column.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields are retained for compatibility with the existing data-structure API.")]
    public readonly float Enter = enter;

    /// <summary>Gets the second edge value in the column.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields are retained for compatibility with the existing data-structure API.")]
    public readonly float Exit = exit;

    /// <summary>Gets the bottom Y coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields are retained for compatibility with the existing data-structure API.")]
    public readonly float Bottom = bottom;

    /// <summary>Gets the top Y coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields are retained for compatibility with the existing data-structure API.")]
    public readonly float Top = top;

    /// <summary>Gets whether this column contains data.</summary>
    public bool HasData => !float.IsNaN(Enter);

    /// <summary>Creates a pixel column with no data values.</summary>
    /// <param name="x">The X coordinate of the pixel column.</param>
    /// <returns>A pixel column whose data values are <see cref="float.NaN"/>.</returns>
    public static PixelColumn WithoutData(float x) => new(x, float.NaN, float.NaN, float.NaN, float.NaN);

    /// <summary>Determines whether two pixel columns are equal.</summary>
    /// <param name="left">The first pixel column.</param>
    /// <param name="right">The second pixel column.</param>
    /// <returns><see langword="true"/> when the columns are equal.</returns>
    public static bool operator ==(PixelColumn left, PixelColumn right) => left.Equals(right);

    /// <summary>Determines whether two pixel columns are not equal.</summary>
    /// <param name="left">The first pixel column.</param>
    /// <param name="right">The second pixel column.</param>
    /// <returns><see langword="true"/> when the columns are not equal.</returns>
    public static bool operator !=(PixelColumn left, PixelColumn right) => !left.Equals(right);

    /// <summary>Returns a string representation of this pixel column.</summary>
    /// <returns>The formatted coordinates and edge values.</returns>
    public override string ToString() => $"x={X} y=[{Bottom}, {Top}], edges=[{Enter}, {Exit}]";

    /// <summary>Determines whether this column equals another pixel column.</summary>
    /// <param name="other">The pixel column to compare.</param>
    /// <returns><see langword="true"/> when all values match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Exact component equality is required for consistent hash-based equality; tolerance-based equality would not be transitive.")]
    public bool Equals(PixelColumn other) =>
        X.Equals(other.X)
        && Enter.Equals(other.Enter)
        && Exit.Equals(other.Exit)
        && Bottom.Equals(other.Bottom)
        && Top.Equals(other.Top);

    /// <summary>Determines whether this column equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal pixel column.</returns>
    public override bool Equals(object? obj) => obj is PixelColumn other && Equals(other);

    /// <summary>Gets a hash code for this pixel column.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCodeCombiner.Combine(X, Enter, Exit, Bottom, Top);
}
