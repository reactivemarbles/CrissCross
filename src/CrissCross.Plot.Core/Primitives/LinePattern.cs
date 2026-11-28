// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a line dash pattern.</summary>
/// <param name="intervals">The alternating on/off intervals.</param>
/// <param name="phase">The dash phase offset.</param>
/// <param name="name">The pattern name.</param>
public readonly struct LinePattern(float[] intervals, float phase, string name) : IEquatable<LinePattern>
{
    /// <summary>The on/off interval for a standard dash.</summary>
    private const float StandardDashLength = 10;

    /// <summary>The on/off interval for a dense dash.</summary>
    private const float DenseDashLength = 6;

    /// <summary>The on interval for a dot.</summary>
    private const float DotLength = 3;

    /// <summary>The off interval for a dot.</summary>
    private const float DotGap = 5;

    /// <summary>Gets a solid line pattern.</summary>
    public static LinePattern Solid { get; } = new([], 0, nameof(Solid));

    /// <summary>Gets a dashed line pattern.</summary>
    public static LinePattern Dashed { get; } = new([StandardDashLength, StandardDashLength], 0, nameof(Dashed));

    /// <summary>Gets a densely dashed line pattern.</summary>
    public static LinePattern DenselyDashed { get; } = new([DenseDashLength, DenseDashLength], 0, nameof(DenselyDashed));

    /// <summary>Gets a dotted line pattern.</summary>
    public static LinePattern Dotted { get; } = new([DotLength, DotGap], 0, nameof(Dotted));

    /// <summary>Gets the alternating on/off intervals.</summary>
    public float[] Intervals { get; } = intervals;

    /// <summary>Gets the dash phase offset.</summary>
    public float Phase { get; } = phase;

    /// <summary>Gets the pattern name.</summary>
    public string Name { get; } = name;

    /// <summary>Gets all built-in line patterns.</summary>
    /// <returns>A new array containing the built-in patterns.</returns>
    public static LinePattern[] GetAllPatterns() => [Solid, Dashed, DenselyDashed, Dotted];

    /// <summary>Determines whether two line patterns are equal.</summary>
    /// <param name="left">The first pattern.</param>
    /// <param name="right">The second pattern.</param>
    /// <returns><see langword="true"/> when the patterns are equal.</returns>
    public static bool operator ==(LinePattern left, LinePattern right) => left.Equals(right);

    /// <summary>Determines whether two line patterns differ.</summary>
    /// <param name="left">The first pattern.</param>
    /// <param name="right">The second pattern.</param>
    /// <returns><see langword="true"/> when the patterns differ.</returns>
    public static bool operator !=(LinePattern left, LinePattern right) => !left.Equals(right);

    /// <summary>Creates a Skia path effect for this pattern.</summary>
    /// <returns>The path effect for this pattern.</returns>
    public SKPathEffect GetPathEffect() => Intervals is null
        ? Solid.GetPathEffect()
        : SKPathEffect.CreateDash(Intervals, Phase);

    /// <summary>Determines whether this pattern equals another pattern.</summary>
    /// <param name="other">The pattern to compare.</param>
    /// <returns><see langword="true"/> when the pattern values are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality is required to remain consistent with the pattern hash code.")]
    public bool Equals(LinePattern other) =>
        ReferenceEquals(Intervals, other.Intervals)
        && Phase.Equals(other.Phase)
        && string.Equals(Name, other.Name, StringComparison.Ordinal);

    /// <summary>Determines whether this pattern equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal pattern.</returns>
    public override bool Equals(object? obj) => obj is LinePattern other && Equals(other);

    /// <summary>Gets a hash code for this pattern.</summary>
    /// <returns>A hash code based on the pattern values.</returns>
    public override int GetHashCode() =>
        (Intervals?.GetHashCode() ?? 0)
        ^ Phase.GetHashCode()
        ^ (Name?.GetHashCode() ?? 0);
}
