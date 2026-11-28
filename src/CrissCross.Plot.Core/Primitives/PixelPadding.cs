// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents pixel padding on each edge of a rectangle.</summary>
public struct PixelPadding : IEquatable<PixelPadding>
{
    /// <summary>Gets or sets the left padding.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Left;

    /// <summary>Gets or sets the right padding.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Right;

    /// <summary>Gets or sets the bottom padding.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Bottom;

    /// <summary>Gets or sets the top padding.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Top;

    /// <summary>Initializes a new instance of the <see cref="PixelPadding"/> struct with equal padding on all sides.</summary>
    /// <param name="padding">The padding amount.</param>
    public PixelPadding(float padding)
    {
        Left = padding;
        Right = padding;
        Bottom = padding;
        Top = padding;
    }

    /// <summary>Initializes a new instance of the <see cref="PixelPadding"/> struct with horizontal and vertical padding.</summary>
    /// <param name="x">The horizontal padding.</param>
    /// <param name="y">The vertical padding.</param>
    public PixelPadding(float x, float y)
    {
        Left = x;
        Right = x;
        Bottom = y;
        Top = y;
    }

    /// <summary>Initializes a new instance of the <see cref="PixelPadding"/> struct from individual edges.</summary>
    /// <param name="left">The left padding.</param>
    /// <param name="right">The right padding.</param>
    /// <param name="bottom">The bottom padding.</param>
    /// <param name="top">The top padding.</param>
    public PixelPadding(float left, float right, float bottom, float top)
    {
        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
    }

    /// <summary>Initializes a new instance of the <see cref="PixelPadding"/> struct from the figure size and data area.</summary>
    /// <param name="figureSize">The full figure size.</param>
    /// <param name="dataArea">The data area rectangle.</param>
    public PixelPadding(PixelSize figureSize, PixelRect dataArea)
    {
        Left = dataArea.Left;
        Right = figureSize.Width - dataArea.Right;
        Top = dataArea.Top;
        Bottom = figureSize.Height - dataArea.Bottom;
    }

    /// <summary>Gets zero padding on every edge.</summary>
    public static PixelPadding Zero => new();

    /// <summary>Gets the total horizontal padding.</summary>
    public readonly float Horizontal => Left + Right;

    /// <summary>Gets the total vertical padding.</summary>
    public readonly float Vertical => Top + Bottom;

    /// <summary>Determines whether two padding values are equal.</summary>
    /// <param name="left">The first padding value.</param>
    /// <param name="right">The second padding value.</param>
    /// <returns><see langword="true"/> when all edges are equal.</returns>
    public static bool operator ==(PixelPadding left, PixelPadding right) => left.Equals(right);

    /// <summary>Determines whether two padding values differ.</summary>
    /// <param name="left">The first padding value.</param>
    /// <param name="right">The second padding value.</param>
    /// <returns><see langword="true"/> when any edge differs.</returns>
    public static bool operator !=(PixelPadding left, PixelPadding right) => !left.Equals(right);

    /// <summary>Expands all padding edges by an amount.</summary>
    /// <param name="amount">The amount to add to each edge.</param>
    public void Expand(float amount)
    {
        Left += amount;
        Right += amount;
        Bottom += amount;
        Top += amount;
    }

    /// <summary>Contracts all padding edges by an amount.</summary>
    /// <param name="amount">The amount to subtract from each edge.</param>
    public void Contract(float amount) => Expand(-amount);

    /// <summary>Determines whether this padding equals another padding value.</summary>
    /// <param name="other">The padding value to compare.</param>
    /// <returns><see langword="true"/> when all edges are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality is required to remain consistent with the hash code.")]
    public readonly bool Equals(PixelPadding other) =>
        Left.Equals(other.Left)
        && Right.Equals(other.Right)
        && Bottom.Equals(other.Bottom)
        && Top.Equals(other.Top);

    /// <summary>Determines whether this padding equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal padding value.</returns>
    public override readonly bool Equals(object? obj) => obj is PixelPadding other && Equals(other);

    /// <summary>Gets a hash code for this padding.</summary>
    /// <returns>A hash code based on all four edges.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1482", Justification = "Mutable public fields and value equality are retained for API compatibility.")]
    public override readonly int GetHashCode() =>
        Left.GetHashCode()
        ^ Right.GetHashCode()
        ^ Bottom.GetHashCode()
        ^ Top.GetHashCode();
}
