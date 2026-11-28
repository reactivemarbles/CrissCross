// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a straight line in pixel space.</summary>
public readonly struct PixelLine : IEquatable<PixelLine>
{
    /// <summary>Gets the start X coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float X1;

    /// <summary>Gets the end X coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float X2;

    /// <summary>Gets the start Y coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float Y1;

    /// <summary>Gets the end Y coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly float Y2;

    /// <summary>The conversion factor from radians to degrees.</summary>
    private const float RadiansToDegrees = 180;

    /// <summary>The divisor used to calculate the center point.</summary>
    private const int CenterDivisor = 2;

    /// <summary>Initializes a new instance of the <see cref="PixelLine"/> struct from endpoint components.</summary>
    /// <param name="x1">The start X coordinate.</param>
    /// <param name="y1">The start Y coordinate.</param>
    /// <param name="x2">The end X coordinate.</param>
    /// <param name="y2">The end Y coordinate.</param>
    public PixelLine(float x1, float y1, float x2, float y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>Initializes a new instance of the <see cref="PixelLine"/> struct from two pixel endpoints.</summary>
    /// <param name="px1">The start point.</param>
    /// <param name="px2">The end point.</param>
    public PixelLine(Pixel px1, Pixel px2)
    {
        X1 = px1.X;
        Y1 = px1.Y;
        X2 = px2.X;
        Y2 = px2.Y;
    }

    /// <summary>Gets the horizontal span.</summary>
    public float XSpan => X2 - X1;

    /// <summary>Gets the vertical span.</summary>
    public float YSpan => Y2 - Y1;

    /// <summary>Gets the line slope, or NaN for a vertical line.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality detects a vertical line and preserves established geometry behavior.")]
    public float Slope => X1 == X2 ? float.NaN : YSpan / XSpan;

    /// <summary>Gets the line slope in radians.</summary>
    public float SlopeRadians => (float)Math.Atan(Slope);

    /// <summary>Gets the line slope in degrees.</summary>
    public float SlopeDegrees => SlopeRadians * RadiansToDegrees / (float)Math.PI;

    /// <summary>Gets the line angle in radians.</summary>
    public float AngleRadians => (float)Math.Atan2(XSpan, -YSpan);

    /// <summary>Gets the line angle in degrees.</summary>
    public float AngleDegrees => AngleRadians * RadiansToDegrees / (float)Math.PI;

    /// <summary>Gets the line's Y intercept.</summary>
    public float YIntercept => Y1 - (Slope * X1);

    /// <summary>Gets the line length.</summary>
    public float Length => (float)Math.Sqrt((XSpan * XSpan) + (YSpan * YSpan));

    /// <summary>Gets the horizontal delta from start to end.</summary>
    public float DeltaX => X2 - X1;

    /// <summary>Gets the vertical delta from start to end.</summary>
    public float DeltaY => Y2 - Y1;

    /// <summary>Gets the start point.</summary>
    public Pixel Pixel1 => new(X1, Y1);

    /// <summary>Gets the end point.</summary>
    public Pixel Pixel2 => new(X2, Y2);

    /// <summary>Gets the center point.</summary>
    public Pixel Center => new((X1 + X2) / CenterDivisor, (Y1 + Y2) / CenterDivisor);

    /// <summary>Determines whether two lines are equal.</summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true"/> when all endpoints match.</returns>
    public static bool operator ==(PixelLine left, PixelLine right) => left.Equals(right);

    /// <summary>Determines whether two lines differ.</summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true"/> when any endpoint differs.</returns>
    public static bool operator !=(PixelLine left, PixelLine right) => !left.Equals(right);

    /// <summary>Formats this line as text.</summary>
    /// <returns>A textual representation of the line.</returns>
    public override string ToString() => $"PixelLine from ({X1}, {Y1}) to ({X2}, {Y2})";

    /// <summary>Moves the line backward along its direction.</summary>
    /// <param name="distance">The distance to move.</param>
    /// <returns>The translated line.</returns>
    public PixelLine BackedUpBy(float distance)
    {
        float deltaX = distance * (float)Math.Cos(SlopeRadians);
        float deltaY = distance * (float)Math.Sin(SlopeRadians);
        return WithDelta(deltaX, deltaY);
    }

    /// <summary>Creates a translated copy of this line.</summary>
    /// <param name="dx">The horizontal translation.</param>
    /// <param name="dy">The vertical translation.</param>
    /// <returns>The translated line.</returns>
    public PixelLine WithDelta(float dx, float dy) => new(X1 + dx, Y1 + dy, X2 + dx, Y2 + dy);

    /// <summary>Returns the X position on the line at the given Y.</summary>
    /// <param name="y">The Y coordinate at which to evaluate the line.</param>
    /// <returns>The corresponding X coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is part of the existing public API.")]
    public float X(float y = 0)
    {
        float deltaX = Y1 - y;
        return X1 - (deltaX * Slope);
    }

    /// <summary>Returns the Y position on the line at the given X.</summary>
    /// <param name="x">The X coordinate at which to evaluate the line.</param>
    /// <returns>The corresponding Y coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is part of the existing public API.")]
    public float Y(float x = 0) => (Slope * x) + YIntercept;

    /// <summary>Determines whether this line equals another line.</summary>
    /// <param name="other">The line to compare.</param>
    /// <returns><see langword="true"/> when all endpoints match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality is required to remain consistent with the line hash code.")]
    public bool Equals(PixelLine other) =>
        X1.Equals(other.X1)
        && Y1.Equals(other.Y1)
        && X2.Equals(other.X2)
        && Y2.Equals(other.Y2);

    /// <summary>Determines whether this line equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal line.</returns>
    public override bool Equals(object? obj) => obj is PixelLine other && Equals(other);

    /// <summary>Gets a hash code for this line.</summary>
    /// <returns>A hash code based on the endpoints.</returns>
    public override int GetHashCode() => X1.GetHashCode() ^ Y1.GetHashCode() ^ X2.GetHashCode() ^ Y2.GetHashCode();
}
