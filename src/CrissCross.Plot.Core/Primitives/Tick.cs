// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a labeled major or minor axis tick.</summary>
public readonly struct Tick : IEquatable<Tick>
{
    /// <summary>Gets the tick position.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly double Position;

    /// <summary>Gets the tick label.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly string Label;

    /// <summary>Gets a value indicating whether this is a major tick.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly bool IsMajor;

    /// <summary>Initializes a new instance of the <see cref="Tick"/> struct for a major tick.</summary>
    /// <param name="position">The tick position.</param>
    /// <param name="label">The tick label.</param>
    public Tick(double position, string label)
    {
        Position = position;
        Label = label;
        IsMajor = true;
    }

    /// <summary>Initializes a new instance of the <see cref="Tick"/> struct.</summary>
    /// <param name="position">The tick position.</param>
    /// <param name="label">The tick label.</param>
    /// <param name="isMajor"><see langword="true"/> for a major tick.</param>
    public Tick(double position, string label, bool isMajor)
    {
        Position = position;
        Label = label;
        IsMajor = isMajor;
    }

    /// <summary>Creates a major tick.</summary>
    /// <param name="position">The tick position.</param>
    /// <param name="label">The tick label.</param>
    /// <returns>The major tick.</returns>
    public static Tick Major(double position, string label) => new(position, label, true);

    /// <summary>Creates a minor tick.</summary>
    /// <param name="position">The tick position.</param>
    /// <returns>The minor tick.</returns>
    public static Tick Minor(double position) => new(position, string.Empty, false);

    /// <summary>Determines whether two ticks are equal.</summary>
    /// <param name="left">The first tick.</param>
    /// <param name="right">The second tick.</param>
    /// <returns><see langword="true"/> when the ticks are equal.</returns>
    public static bool operator ==(Tick left, Tick right) => left.Equals(right);

    /// <summary>Determines whether two ticks differ.</summary>
    /// <param name="left">The first tick.</param>
    /// <param name="right">The second tick.</param>
    /// <returns><see langword="true"/> when the ticks differ.</returns>
    public static bool operator !=(Tick left, Tick right) => !left.Equals(right);

    /// <summary>Determines whether this tick equals another tick.</summary>
    /// <param name="other">The tick to compare.</param>
    /// <returns><see langword="true"/> when the position, label, and major state match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact position equality is required to remain consistent with the hash code.")]
    public bool Equals(Tick other) => Position.Equals(other.Position) && string.Equals(Label, other.Label, StringComparison.Ordinal) && IsMajor == other.IsMajor;

    /// <summary>Determines whether this tick equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal tick.</returns>
    public override bool Equals(object? obj) => obj is Tick other && Equals(other);

    /// <summary>Gets a hash code for this tick.</summary>
    /// <returns>A hash code based on the position, label, and major state.</returns>
    public override int GetHashCode() => Position.GetHashCode() ^ (Label?.GetHashCode() ?? 0) ^ IsMajor.GetHashCode();
}
