// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a location on screen in pixel units.</summary>
/// <remarks>Pixels use float precision and are distinct from axis-space <see cref="Coordinates"/> values, which use double precision.</remarks>
public struct Pixel : IEquatable<Pixel>
{
    /// <summary>Gets or sets the horizontal screen position; larger values are farther right.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float X;

    /// <summary>Gets or sets the vertical screen position; larger values are farther down.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Y;

    /// <summary>Initializes a new instance of the <see cref="Pixel"/> struct.</summary>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    public Pixel(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>Initializes a new instance of the <see cref="Pixel"/> struct from double coordinates.</summary>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    public Pixel(double x, double y)
    {
        X = (float)x;
        Y = (float)y;
    }

    /// <summary>Gets an invalid pixel location with NaN components.</summary>
    public static Pixel NaN => new(float.NaN, float.NaN);

    /// <summary>Gets the pixel at the origin.</summary>
    public static Pixel Zero => new(0, 0);

    /// <summary>Determines whether two pixels are equal.</summary>
    /// <param name="left">The first pixel.</param>
    /// <param name="right">The second pixel.</param>
    /// <returns><see langword="true"/> when the pixels are equal.</returns>
    public static bool operator ==(Pixel left, Pixel right) => left.Equals(right);

    /// <summary>Determines whether two pixels differ.</summary>
    /// <param name="left">The first pixel.</param>
    /// <param name="right">The second pixel.</param>
    /// <returns><see langword="true"/> when the pixels differ.</returns>
    public static bool operator !=(Pixel left, Pixel right) => !left.Equals(right);

    /// <summary>Adds two pixel coordinates.</summary>
    /// <param name="left">The first pixel.</param>
    /// <param name="right">The second pixel.</param>
    /// <returns>The component-wise sum.</returns>
    public static Pixel operator +(Pixel left, Pixel right) => new(left.X + right.X, left.Y + right.Y);

    /// <summary>Subtracts one pixel coordinate from another.</summary>
    /// <param name="left">The pixel to subtract from.</param>
    /// <param name="right">The pixel to subtract.</param>
    /// <returns>The component-wise difference.</returns>
    public static Pixel operator -(Pixel left, Pixel right) => new(left.X - right.X, left.Y - right.Y);

    /// <summary>Scales a pixel coordinate.</summary>
    /// <param name="pixel">The pixel coordinate.</param>
    /// <param name="factor">The scale factor.</param>
    /// <returns>The scaled pixel.</returns>
    public static Pixel operator *(Pixel pixel, float factor) => new(pixel.X * factor, pixel.Y * factor);

    /// <summary>Divides a pixel coordinate by a scalar.</summary>
    /// <param name="pixel">The pixel coordinate.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The divided pixel.</returns>
    public static Pixel operator /(Pixel pixel, float divisor) => new(pixel.X / divisor, pixel.Y / divisor);

    /// <summary>Adds two pixels component by component.</summary>
    /// <param name="left">The first pixel.</param>
    /// <param name="right">The second pixel.</param>
    /// <returns>The component-wise sum.</returns>
    public static Pixel Add(Pixel left, Pixel right) => left + right;

    /// <summary>Subtracts two pixels component by component.</summary>
    /// <param name="left">The pixel to subtract from.</param>
    /// <param name="right">The pixel to subtract.</param>
    /// <returns>The component-wise difference.</returns>
    public static Pixel Subtract(Pixel left, Pixel right) => left - right;

    /// <summary>Formats this pixel as text.</summary>
    /// <returns>A textual representation of this pixel.</returns>
    public override readonly string ToString() => $"Pixel {{ X = {X}, Y = {Y} }}";

    /// <summary>Converts this pixel to a SkiaSharp point.</summary>
    /// <returns>The equivalent SkiaSharp point.</returns>
    public readonly SKPoint ToSKPoint() => new(X, Y);

    /// <summary>Determines whether this pixel equals another pixel.</summary>
    /// <param name="other">The pixel to compare.</param>
    /// <returns><see langword="true"/> when both components are equal.</returns>
    public readonly bool Equals(Pixel other) => Equals(X, other.X) && Equals(Y, other.Y);

    /// <summary>Determines whether this pixel equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal pixel.</returns>
    public override readonly bool Equals(object? obj) => obj is Pixel other && Equals(other);

    /// <summary>Gets a hash code for this pixel.</summary>
    /// <returns>A hash code based on both components.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1482", Justification = "Mutable public fields are retained for binary API compatibility.")]
    public override readonly int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

    /// <summary>Gets the distance from this pixel to another pixel.</summary>
    /// <param name="pixel">The pixel to measure to.</param>
    /// <returns>The distance in pixels.</returns>
    public readonly float DistanceFrom(Pixel pixel)
    {
        float deltaX = pixel.X - X;
        float deltaY = pixel.Y - Y;
        return (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
    }

    /// <summary>Creates a pixel translated by the specified horizontal and vertical offsets.</summary>
    /// <param name="dX">The horizontal offset.</param>
    /// <param name="dY">The vertical offset.</param>
    /// <returns>The translated pixel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter names are retained for named-argument compatibility.")]
    public readonly Pixel WithOffset(float dX, float dY) => new(X + dX, Y + dY);

    /// <summary>Creates a pixel translated by an offset.</summary>
    /// <param name="offset">The offset to apply.</param>
    /// <returns>The translated pixel.</returns>
    public readonly Pixel WithOffset(PixelOffset offset) => new(X + offset.X, Y + offset.Y);

    /// <summary>Creates a pixel moved to the right by a distance.</summary>
    /// <param name="dX">The horizontal distance.</param>
    /// <returns>The translated pixel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public readonly Pixel MovedRight(float dX) => new(X + dX, Y);

    /// <summary>Creates a pixel moved to the left by a distance.</summary>
    /// <param name="dX">The horizontal distance.</param>
    /// <returns>The translated pixel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public readonly Pixel MovedLeft(float dX) => new(X - dX, Y);

    /// <summary>Creates a pixel moved up by a distance.</summary>
    /// <param name="dY">The vertical distance.</param>
    /// <returns>The translated pixel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public readonly Pixel MovedUp(float dY) => new(X, Y - dY);

    /// <summary>Creates a pixel moved down by a distance.</summary>
    /// <param name="dY">The vertical distance.</param>
    /// <returns>The translated pixel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public readonly Pixel MovedDown(float dY) => new(X, Y + dY);

    /// <summary>Divides both pixel components by a value.</summary>
    /// <param name="value">The divisor.</param>
    /// <returns>The divided pixel.</returns>
    public readonly Pixel Divide(float value) => new(X / value, Y / value);

    /// <summary>Divides each pixel component by its corresponding value.</summary>
    /// <param name="x">The horizontal divisor.</param>
    /// <param name="y">The vertical divisor.</param>
    /// <returns>The divided pixel.</returns>
    public readonly Pixel Divide(float x, float y) => new(X / x, Y / y);

    /// <summary>Multiplies both pixel components by a value.</summary>
    /// <param name="value">The multiplier.</param>
    /// <returns>The scaled pixel.</returns>
    public readonly Pixel Multiply(float value) => new(X * value, Y * value);

    /// <summary>Multiplies each pixel component by its corresponding value.</summary>
    /// <param name="x">The horizontal multiplier.</param>
    /// <param name="y">The vertical multiplier.</param>
    /// <returns>The scaled pixel.</returns>
    public readonly Pixel Multiply(float x, float y) => new(X * x, Y * y);
}
