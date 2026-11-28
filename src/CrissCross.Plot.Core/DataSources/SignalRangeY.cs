// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Represents the minimum and maximum values in a signal range.</summary>
public readonly struct SignalRangeY : IEquatable<SignalRangeY>
{
    /// <summary>Initializes a new instance of the <see cref="SignalRangeY"/> struct.</summary>
    /// <param name="min">The minimum value in the range.</param>
    /// <param name="max">The maximum value in the range.</param>
    public SignalRangeY(double min, double max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>Gets the minimum value in the range.</summary>
    public double Min { get; }

    /// <summary>Gets the maximum value in the range.</summary>
    public double Max { get; }

    /// <summary>Determines whether two signal ranges are equal.</summary>
    public static bool operator ==(SignalRangeY left, SignalRangeY right) => left.Equals(right);

    /// <summary>Determines whether two signal ranges are not equal.</summary>
    public static bool operator !=(SignalRangeY left, SignalRangeY right) => !left.Equals(right);

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Value equality must compare exact bounds to remain transitive and consistent with GetHashCode; a tolerance would violate the equality contract.")]
    public bool Equals(SignalRangeY other) => Min.Equals(other.Min) && Max.Equals(other.Max);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is SignalRangeY other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();

    /// <summary>Returns a string representation of this range.</summary>
    /// <returns>The minimum and maximum values.</returns>
    public override string ToString() => $"Range [{Min}, {Max}]";
}
