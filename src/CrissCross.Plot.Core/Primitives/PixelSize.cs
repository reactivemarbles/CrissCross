// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a width and height in pixel units.</summary>
public readonly struct PixelSize : IEquatable<PixelSize>
{
    /// <summary>Gets the width in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float Width;

    /// <summary>Gets the height in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float Height;

    /// <summary>Initializes a new instance of the <see cref="PixelSize"/> struct.</summary>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    public PixelSize(float width, float height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>Initializes a new instance of the <see cref="PixelSize"/> struct from double values.</summary>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    public PixelSize(double width, double height)
    {
        Width = (float)width;
        Height = (float)height;
    }

    /// <summary>Gets a size whose width and height are both zero.</summary>
    public static PixelSize Zero => new(0, 0);

    /// <summary>Gets a size whose width and height are both NaN.</summary>
    public static PixelSize NaN => new(float.NaN, float.NaN);

    /// <summary>Gets a size whose width and height are both positive infinity.</summary>
    public static PixelSize Infinity => new(float.PositiveInfinity, float.PositiveInfinity);

    /// <summary>Gets the area in square pixels.</summary>
    public float Area => Width * Height;

    /// <summary>Gets the diagonal length in pixels.</summary>
    public float Diagonal => (float)Math.Sqrt((Width * Width) + (Height * Height));

    /// <summary>Determines whether two sizes are equal.</summary>
    /// <param name="left">The first size.</param>
    /// <param name="right">The second size.</param>
    /// <returns><see langword="true"/> when both dimensions match.</returns>
    public static bool operator ==(PixelSize left, PixelSize right) => left.Equals(right);

    /// <summary>Determines whether two sizes differ.</summary>
    /// <param name="left">The first size.</param>
    /// <param name="right">The second size.</param>
    /// <returns><see langword="true"/> when either dimension differs.</returns>
    public static bool operator !=(PixelSize left, PixelSize right) => !left.Equals(right);

    /// <summary>Formats this size as text.</summary>
    /// <returns>A textual representation of this size.</returns>
    public override string ToString() => $"PixelSize: Width={Width}, Height={Height}";

    /// <summary>Creates a rectangle at the origin with this size.</summary>
    /// <returns>The pixel rectangle.</returns>
    public PixelRect ToPixelRect() => new(0, Width, Height, 0);

    /// <summary>Creates a rectangle of this size anchored to a pixel and alignment.</summary>
    /// <param name="pixel">The anchor pixel.</param>
    /// <param name="alignment">The alignment relative to the anchor.</param>
    /// <returns>The aligned pixel rectangle.</returns>
    public PixelRect ToPixelRect(Pixel pixel, Alignment alignment)
    {
        PixelRect rect = new(
            left: pixel.X,
            right: pixel.X + Width,
            bottom: pixel.Y + Height,
            top: pixel.Y);

        return rect.WithDelta(
            x: -Width * alignment.HorizontalFraction(),
            y: -Height * (1 - alignment.VerticalFraction()));
    }

    /// <summary>Determines whether this size is at least as large as another size in each dimension.</summary>
    /// <param name="size">The size to compare.</param>
    /// <returns><see langword="true"/> when this size contains the other size.</returns>
    public bool Contains(PixelSize size) => Width >= size.Width && Height >= size.Height;

    /// <summary>Returns a size with the larger dimension from either size.</summary>
    /// <param name="rect2">The size to compare with this one.</param>
    /// <returns>The maximum width and height.</returns>
    public PixelSize Max(PixelSize rect2) => new(
        width: Math.Max(Width, rect2.Width),
        height: Math.Max(Height, rect2.Height));

    /// <summary>Creates a size expanded by the specified padding.</summary>
    /// <param name="pad">The padding to add.</param>
    /// <returns>The expanded size.</returns>
    public PixelSize Expanded(PixelPadding pad) => new(Width + pad.Left + pad.Right, Height + pad.Top + pad.Bottom);

    /// <summary>Creates a size contracted by the specified padding.</summary>
    /// <param name="pad">The padding to subtract.</param>
    /// <returns>The contracted size.</returns>
    public PixelSize Contracted(PixelPadding pad) => new(Width - pad.Left - pad.Right, Height - pad.Top - pad.Bottom);

    /// <summary>Determines whether this size equals another size.</summary>
    /// <param name="other">The size to compare.</param>
    /// <returns><see langword="true"/> when both dimensions match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact dimension equality is required to remain consistent with the hash code.")]
    public bool Equals(PixelSize other) => Width.Equals(other.Width) && Height.Equals(other.Height);

    /// <summary>Determines whether this size equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal size.</returns>
    public override bool Equals(object? obj) => obj is PixelSize other && Equals(other);

    /// <summary>Gets a hash code for this size.</summary>
    /// <returns>A hash code based on width and height.</returns>
    public override int GetHashCode() => Width.GetHashCode() ^ Height.GetHashCode();
}
