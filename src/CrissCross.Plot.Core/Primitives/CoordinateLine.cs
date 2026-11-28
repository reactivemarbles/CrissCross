// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a straight line in coordinate space.</summary>
public readonly struct CoordinateLine : IEquatable<CoordinateLine>
{
    /// <summary>Gets the X coordinate of the line's first endpoint.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double X1;

    /// <summary>Gets the X coordinate of the line's second endpoint.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double X2;

    /// <summary>Gets the Y coordinate of the line's first endpoint.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double Y1;

    /// <summary>Gets the Y coordinate of the line's second endpoint.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing struct API and binary compatibility.")]
    public readonly double Y2;

    /// <summary>Initializes a new instance of the <see cref="CoordinateLine"/> struct from endpoint coordinates.</summary>
    /// <param name="x1">The first endpoint's X coordinate.</param>
    /// <param name="y1">The first endpoint's Y coordinate.</param>
    /// <param name="x2">The second endpoint's X coordinate.</param>
    /// <param name="y2">The second endpoint's Y coordinate.</param>
    public CoordinateLine(double x1, double y1, double x2, double y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateLine"/> struct from two points.</summary>
    /// <param name="pt1">The first endpoint.</param>
    /// <param name="pt2">The second endpoint.</param>
    public CoordinateLine(Coordinates pt1, Coordinates pt2)
    {
        X1 = pt1.X;
        Y1 = pt1.Y;
        X2 = pt2.X;
        Y2 = pt2.Y;
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateLine"/> struct from a point and slope.</summary>
    /// <param name="x">The X coordinate of the first point.</param>
    /// <param name="y">The Y coordinate of the first point.</param>
    /// <param name="slope">The line slope.</param>
    public CoordinateLine(double x, double y, double slope)
    {
        X1 = x;
        Y1 = y;
        X2 = x + 1;
        Y2 = y + slope;
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateLine"/> struct from a point and slope.</summary>
    /// <param name="point">The first point.</param>
    /// <param name="slope">The line slope.</param>
    public CoordinateLine(Coordinates point, double slope)
    {
        X1 = point.X;
        Y1 = point.Y;
        X2 = point.X + 1;
        Y2 = point.Y + slope;
    }

    /// <summary>Gets the minimum X coordinate of the endpoints.</summary>
    public double XMin => Math.Min(X1, X2);

    /// <summary>Gets the maximum X coordinate of the endpoints.</summary>
    public double XMax => Math.Max(X1, X2);

    /// <summary>Gets the minimum Y coordinate of the endpoints.</summary>
    public double YMin => Math.Min(Y1, Y2);

    /// <summary>Gets the maximum Y coordinate of the endpoints.</summary>
    public double YMax => Math.Max(Y1, Y2);

    /// <summary>Gets the horizontal difference between the endpoints.</summary>
    public double XSpan => X2 - X1;

    /// <summary>Gets the vertical difference between the endpoints.</summary>
    public double YSpan => Y2 - Y1;

    /// <summary>Gets the slope, or NaN for a vertical line.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact equality identifies a vertical line; a tolerance would change geometric behavior.")]
    public double Slope => X1 == X2 ? double.NaN : YSpan / XSpan;

    /// <summary>Gets the slope angle in radians.</summary>
    public double SlopeRadians => Math.Atan(Slope);

    /// <summary>Gets the slope angle in degrees.</summary>
    public double SlopeDegrees => SlopeRadians * Constants.DegreesPerRadian;

    /// <summary>Gets the Y coordinate where the line crosses the Y axis.</summary>
    public double YIntercept => Y1 - (Slope * X1);

    /// <summary>Gets the length of the line.</summary>
    public float Length => (float)Math.Sqrt((XSpan * XSpan) + (YSpan * YSpan));

    /// <summary>Gets the first endpoint.</summary>
    public Coordinates Start => new(X1, Y1);

    /// <summary>Gets the second endpoint.</summary>
    public Coordinates End => new(X2, Y2);

    /// <summary>Gets the midpoint of the line.</summary>
    public Coordinates Center => new((X1 + X2) / Constants.Two, (Y1 + Y2) / Constants.Two);

    /// <summary>Determines whether two coordinate lines are equal.</summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true"/> when the lines match.</returns>
    public static bool operator ==(CoordinateLine left, CoordinateLine right) => left.Equals(right);

    /// <summary>Determines whether two coordinate lines are different.</summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true"/> when the lines differ.</returns>
    public static bool operator !=(CoordinateLine left, CoordinateLine right) => !left.Equals(right);

    /// <summary>Adjusts the line to fit within a rectangle without changing its slope or intercept.</summary>
    /// <param name="rect">The rectangle boundaries.</param>
    /// <returns>The line segment clipped to the rectangle's top and bottom edges.</returns>
    public CoordinateLine ExtendTo(CoordinateRect rect)
    {
        double bottomDeltaY = Y1 - rect.Bottom;
        double horizontalAtBottom = X1 - (bottomDeltaY * Slope);
        Coordinates bottom = new(horizontalAtBottom, rect.Bottom);

        double topDeltaY = rect.Top - Y1;
        double horizontalAtTop = X1 + (topDeltaY * Slope);
        Coordinates top = new(horizontalAtTop, rect.Top);

        return new(bottom, top);
    }

    /// <summary>Gets the X position on the line at the given Y coordinate.</summary>
    /// <param name="y">The Y coordinate; defaults to zero.</param>
    /// <returns>The corresponding X coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "The optional default is retained for compatibility with the existing public API.")]
    public double X(double y = 0)
    {
        double verticalDelta = Y1 - y;
        return X1 - (verticalDelta * Slope);
    }

    /// <summary>Gets the Y position on the line at the given X coordinate.</summary>
    /// <param name="x">The X coordinate; defaults to zero.</param>
    /// <returns>The corresponding Y coordinate.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "The optional default is retained for compatibility with the existing public API.")]
    public double Y(double x = 0) => (Slope * x) + YIntercept;

    /// <summary>Creates a translated copy of this line.</summary>
    /// <param name="dX">The horizontal translation.</param>
    /// <param name="dY">The vertical translation.</param>
    /// <returns>The translated line.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1305", Justification = "The parameter names are retained for named-argument compatibility.")]
    public CoordinateLine WithDelta(double dX, double dY) => new(X1 + dX, Y1 + dY, X2 + dX, Y2 + dY);

    /// <summary>Creates a line with its endpoints reversed.</summary>
    /// <returns>The line with its second endpoint first.</returns>
    public CoordinateLine Reversed() => new(End, Start);

    /// <summary>Gets the smallest rectangle enclosing this line.</summary>
    /// <returns>The bounding rectangle.</returns>
    public CoordinateRect BoundingBox() => new(XMin, XMax, YMin, YMax);

    /// <summary>Determines whether this line equals another line.</summary>
    /// <param name="other">The line to compare.</param>
    /// <returns><see langword="true"/> when the endpoint coordinates match exactly.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact component equality is required for value equality and a hash code consistent with the compared endpoints.")]
    public bool Equals(CoordinateLine other) =>
        X1.Equals(other.X1)
        && X2.Equals(other.X2)
        && Y1.Equals(other.Y1)
        && Y2.Equals(other.Y2);

    /// <summary>Determines whether this line equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal coordinate line.</returns>
    public override bool Equals(object? obj) => obj is CoordinateLine other && Equals(other);

    /// <summary>Gets a hash code for this line's endpoints.</summary>
    /// <returns>A hash code based on the endpoint coordinates.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = X1.GetHashCode();
            hashCode = (hashCode * 397) ^ X2.GetHashCode();
            hashCode = (hashCode * 397) ^ Y1.GetHashCode();
            return (hashCode * 397) ^ Y2.GetHashCode();
        }
    }

    /// <summary>Formats this line as text.</summary>
    /// <returns>A description of the endpoints.</returns>
    public override string ToString() => $"CoordinateLine from ({X1}, {Y1}) to ({X2}, {Y2})";

    /// <summary>Contains constants used by coordinate lines.</summary>
    private static class Constants
    {
        /// <summary>The divisor used to calculate the center.</summary>
        internal const double Two = 2;

        /// <summary>The conversion factor from radians to degrees.</summary>
        internal const double DegreesPerRadian = 180 / Math.PI;
    }
}
