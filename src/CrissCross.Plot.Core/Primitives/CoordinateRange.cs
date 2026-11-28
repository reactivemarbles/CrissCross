// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a pair of bounding coordinates on a single axis. Inverted ranges are permitted.</summary>
/// <param name="value1">The first endpoint.</param>
/// <param name="value2">The second endpoint.</param>
public readonly struct CoordinateRange(double value1, double value2) : IEquatable<CoordinateRange>
{
    /// <summary>Gets the first endpoint as provided.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public readonly double Value1 = value1;

    /// <summary>Gets the second endpoint as provided.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public readonly double Value2 = value2;

    /// <summary>Gets the lower of the two endpoints.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public readonly double Min = Math.Min(value1, value2);

    /// <summary>Gets the higher of the two endpoints.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public readonly double Max = Math.Max(value1, value2);

    /// <summary>Gets whether the first endpoint is greater than the second.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API.")]
    public readonly bool IsInverted = value1 > value2;

    /// <summary>The divisor used to calculate the range center.</summary>
    private const double CenterDivisor = 2;

    // TODO: Figure out how to avoid using NotSet and NoLimits magic numbers.
    // This struct should probably be broken into a small collection of types.
    /// <summary>Gets a value indicating that the range has not been set.</summary>
    public static CoordinateRange NotSet => new(double.PositiveInfinity, double.NegativeInfinity);

    /// <summary>Gets a value indicating that the range has no defined limits.</summary>
    public static CoordinateRange NoLimits => new(double.NaN, double.NaN);

    /// <summary>Gets the signed distance from <see cref="Value1"/> to <see cref="Value2"/>.</summary>
    public double Span => Value2 - Value1;

    /// <summary>Gets the center between the two endpoints.</summary>
    public double Center => (Value1 + Value2) / CenterDivisor;

    /// <summary>Gets the non-negative distance between <see cref="Min"/> and <see cref="Max"/>.</summary>
    public double Length => Math.Abs(Span);

    /// <summary>Determines whether two ranges are equal.</summary>
    public static bool operator ==(CoordinateRange a, CoordinateRange b) => a.Equals(b);

    /// <summary>Determines whether two ranges are different.</summary>
    public static bool operator !=(CoordinateRange a, CoordinateRange b) => !a.Equals(b);

    /// <summary>Returns the extrema of values, ignoring NaN entries.</summary>
    /// <param name="values">The values to examine.</param>
    /// <returns>The minimum and maximum values, or <see cref="NoLimits"/> when none are real.</returns>
    public static CoordinateRange Extrema(IEnumerable<double> values)
    {
        double minimum = double.PositiveInfinity;
        double maximum = double.NegativeInfinity;
        bool hasValue = false;
        foreach (double value in values)
        {
            if (double.IsNaN(value))
            {
                continue;
            }

            minimum = Math.Min(minimum, value);
            maximum = Math.Max(maximum, value);
            hasValue = true;
        }

        return hasValue ? new(minimum, maximum) : NoLimits;
    }

    /// <summary>Returns the range with its endpoints in ascending order.</summary>
    /// <returns>A range whose first endpoint is not greater than its second.</returns>
    public CoordinateRange Rectified() => new(Min, Max);

    /// <summary>Formats this range and its inversion state.</summary>
    /// <returns>A textual representation of this range.</returns>
    public override string ToString() => IsInverted
        ? $"CoordinateRange [{Min}, {Max}] (inverted)"
        : $"CoordinateRange [{Min}, {Max}]";

    /// <summary>Determines whether the range contains a value, including its endpoints.</summary>
    /// <param name="value">The value to test.</param>
    /// <returns><see langword="true"/> when the value is within the range.</returns>
    public bool Contains(double value) => Min <= value && value <= Max;

    /// <summary>Determines whether this range overlaps another range.</summary>
    /// <param name="other">The range to compare.</param>
    /// <returns><see langword="true"/> when the ranges share any value.</returns>
    public bool Overlaps(CoordinateRange other) =>
        Contains(other.Min) || Contains(other.Max) || other.Contains(Min) || other.Contains(Max);

    /// <summary>Returns a copy of this range expanded to include a point.</summary>
    /// <param name="value">The value to include.</param>
    /// <returns>The expanded range.</returns>
    public CoordinateRange Expanded(double value)
    {
        double minimum = Math.Min(Min, value);
        double maximum = Math.Max(Max, value);
        return new(minimum, maximum);
    }

    /// <summary>Determines whether this range equals another range.</summary>
    /// <param name="other">The range to compare.</param>
    /// <returns><see langword="true"/> when the endpoints are equal.</returns>
    public bool Equals(CoordinateRange other) => Equals(Value1, other.Value1) && Equals(Value2, other.Value2);

    /// <summary>Determines whether this range equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal range.</returns>
    public override bool Equals(object? obj) => obj is CoordinateRange other && Equals(other);

    /// <summary>Gets a hash code for this range.</summary>
    /// <returns>A hash code based on both endpoints.</returns>
    public override int GetHashCode()
    {
        int hash = 17;
        hash = (hash * 23) + Value1.GetHashCode();
        hash = (hash * 23) + Value2.GetHashCode();
        return hash;
    }
}
