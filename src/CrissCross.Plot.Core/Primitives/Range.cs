// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents an inclusive range between two finite values.</summary>
public readonly struct Range : IEquatable<Range> // TODO: evaluate if this can be replaced with more task-specific primitives
{
    /// <summary>Initializes a new instance of the <see cref="Range"/> struct.</summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public Range(double min, double max)
    {
        if (double.IsInfinity(min) || double.IsNaN(min))
        {
            throw new ArgumentException($"{nameof(min)} must be a real number");
        }

        if (double.IsInfinity(max) || double.IsNaN(max))
        {
            throw new ArgumentException($"{nameof(max)} must be a real number");
        }

        if (min > max)
        {
            throw new ArgumentException($"Argument ${nameof(min)} must be less than or equal to ${nameof(max)}.");
        }

        Min = min;
        Max = max;
    }

    /// <summary>Gets the minimum value in the range.</summary>
    public double Min { get; }

    /// <summary>Gets the maximum value in the range.</summary>
    public double Max { get; }

    /// <summary>Determines whether two ranges are equal.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when both endpoints match.</returns>
    public static bool operator ==(Range left, Range right) => left.Equals(right);

    /// <summary>Determines whether two ranges differ.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when either endpoint differs.</returns>
    public static bool operator !=(Range left, Range right) => !left.Equals(right);

    /// <summary>Creates a range containing the values in a two-dimensional array.</summary>
    /// <param name="input">The values to examine.</param>
    /// <returns>The range of finite values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "This overload preserves the established rectangular-array API; changing to a jagged array would break callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1001",
        Justification = "The multidimensional array rank syntax requires adjacent commas.")]
    public static Range GetRange(double[,] input)
    {
        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;
        for (int row = 0; row < input.GetLength(0); row++)
        {
            for (int column = 0; column < input.GetLength(1); column++)
            {
                double current = input[row, column];
                if (double.IsNaN(current))
                {
                    continue;
                }

                min = Math.Min(min, current);
                max = Math.Max(max, current);
            }
        }

        return new(min, max);
    }

    /// <summary>Creates a range containing the values in a sequence.</summary>
    /// <param name="input">The values to examine.</param>
    /// <returns>The range of finite values.</returns>
    public static Range GetRange(IEnumerable<double> input)
    {
        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;
        foreach (var curr in input)
        {
            if (double.IsNaN(curr))
            {
                continue;
            }

            min = Math.Min(min, curr);
            max = Math.Max(max, curr);
        }

        return new(min, max);
    }

    /// <summary>Formats this range as text.</summary>
    /// <returns>A textual representation of this range.</returns>
    public override string ToString() => $"Range: [{Min}, {Max}]";

    /// <summary>Returns the value's fraction of the range span for min-max scaling.</summary>
    /// <param name="value">The value to normalize.</param>
    /// <param name="clamp">Whether values outside the range are clamped to the unit interval.</param>
    /// <returns>The normalized value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional parameter default is retained for source and binary compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "An exact zero-span check preserves the range normalization contract.")]
    public double Normalize(double value, bool clamp = false)
    {
        if (Max == Min)
        {
            throw new ArgumentException($"Cannot normalize to the range if {nameof(Min)} == {nameof(Max)}");
        }

        double normalized = (value - Min) / (Max - Min);

        Range unitRange = new(0, 1);
        return clamp ? unitRange.Clamp(normalized) : normalized;
    }

    /// <summary>Clamps a value to this range, inclusive.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <returns>The clamped value.</returns>
    public double Clamp(double value)
    {
        if (value < Min)
        {
            return Min;
        }

        return value > Max ? Max : value;
    }

    /// <summary>Determines whether this range equals another range.</summary>
    /// <param name="other">The range to compare.</param>
    /// <returns><see langword="true"/> when both endpoints match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality is required for value equality to remain consistent with the hash code.")]
    public bool Equals(Range other) => Min.Equals(other.Min) && Max.Equals(other.Max);

    /// <summary>Determines whether this range equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal range.</returns>
    public override bool Equals(object? obj) => obj is Range other && Equals(other);

    /// <summary>Gets a hash code for this range.</summary>
    /// <returns>A hash code based on both endpoints.</returns>
    public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();
}
