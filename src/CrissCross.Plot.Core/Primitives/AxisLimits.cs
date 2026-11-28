// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents limits for the visible rectangular area on a two-dimensional coordinate system.</summary>
public readonly struct AxisLimits : IEquatable<AxisLimits>
{
    /// <summary>The default absolute limit on either axis.</summary>
    private const double DefaultLimit = 10;

    /// <summary>The multiplier used to calculate half of a value.</summary>
    private const double Half = 0.5;

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from a coordinate.</summary>
    /// <param name="coordinates">The coordinate used for all four limits.</param>
    public AxisLimits(Coordinates coordinates)
    {
        Left = coordinates.X;
        Right = coordinates.X;
        Bottom = coordinates.Y;
        Top = coordinates.Y;
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from a rectangle.</summary>
    /// <param name="rect">The rectangle whose edges define the limits.</param>
    public AxisLimits(CoordinateRect rect)
    {
        Left = rect.Left;
        Right = rect.Right;
        Bottom = rect.Bottom;
        Top = rect.Top;
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from edge values.</summary>
    /// <param name="left">The left horizontal limit.</param>
    /// <param name="right">The right horizontal limit.</param>
    /// <param name="bottom">The bottom vertical limit.</param>
    /// <param name="top">The top vertical limit.</param>
    public AxisLimits(double left, double right, double bottom, double top)
    {
        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from axis ranges.</summary>
    /// <param name="xRange">The horizontal range.</param>
    /// <param name="yRange">The vertical range.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "Parameter names are part of the public API for named-argument callers.")]
    public AxisLimits(CoordinateRange xRange, CoordinateRange yRange)
    {
        Left = xRange.IsInverted ? xRange.Max : xRange.Min;
        Right = xRange.IsInverted ? xRange.Min : xRange.Max;
        Bottom = yRange.IsInverted ? yRange.Max : yRange.Min;
        Top = yRange.IsInverted ? yRange.Min : yRange.Max;
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from plottables.</summary>
    /// <param name="plottables">The plottables whose limits are included.</param>
    public AxisLimits(IEnumerable<IPlottable> plottables)
    {
        ExpandingAxisLimits limits = new(plottables);
        Left = limits.Left;
        Right = limits.Right;
        Bottom = limits.Bottom;
        Top = limits.Top;
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from coordinates.</summary>
    /// <param name="coordinates">The coordinates whose limits are included.</param>
    public AxisLimits(IEnumerable<Coordinates> coordinates)
    {
        ExpandingAxisLimits limits = new();
        limits.Expand(coordinates);

        Left = limits.Left;
        Right = limits.Right;
        Bottom = limits.Bottom;
        Top = limits.Top;
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLimits"/> struct from 3D coordinates.</summary>
    /// <param name="coordinates">The coordinates whose XY limits are included.</param>
    public AxisLimits(IEnumerable<Coordinates3d> coordinates)
    {
        ExpandingAxisLimits limits = new();
        limits.Expand(coordinates);

        Left = limits.Left;
        Right = limits.Right;
        Bottom = limits.Bottom;
        Top = limits.Top;
    }

    /// <summary>Gets the default limits, spanning from -10 to 10 on both axes.</summary>
    public static AxisLimits Default { get; } = new(-DefaultLimit, DefaultLimit, -DefaultLimit, DefaultLimit);

    /// <summary>Gets limits with no defined bounds.</summary>
    public static AxisLimits NoLimits => new(double.NaN, double.NaN, double.NaN, double.NaN);

    /// <summary>Gets unset limits, represented by inverted infinite ranges.</summary>
    public static AxisLimits Unset => new(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity, double.NegativeInfinity);

    /// <summary>Gets the left horizontal limit.</summary>
    public double Left { get; }

    /// <summary>Gets the right horizontal limit.</summary>
    public double Right { get; }

    /// <summary>Gets the bottom vertical limit.</summary>
    public double Bottom { get; }

    /// <summary>Gets the top vertical limit.</summary>
    public double Top { get; }

    /// <summary>Gets the signed horizontal span from <see cref="Left"/> to <see cref="Right"/>.</summary>
    public double HorizontalSpan => Right - Left;

    /// <summary>Gets the signed vertical span from <see cref="Bottom"/> to <see cref="Top"/>.</summary>
    public double VerticalSpan => Top - Bottom;

    /// <summary>Gets the horizontal center of these limits.</summary>
    public double HorizontalCenter => Midpoint(Left, Right);

    /// <summary>Gets the vertical center of these limits.</summary>
    public double VerticalCenter => Midpoint(Bottom, Top);

    /// <summary>Gets the center coordinate of these limits.</summary>
    public Coordinates Center => new(HorizontalCenter, VerticalCenter);

    /// <summary>Gets the horizontal coordinate range.</summary>
    public CoordinateRange XRange => new(Left, Right);

    /// <summary>Gets the vertical coordinate range.</summary>
    public CoordinateRange YRange => new(Bottom, Top);

    /// <summary>Gets the horizontal coordinate range.</summary>
    public CoordinateRange HorizontalRange => XRange;

    /// <summary>Gets the vertical coordinate range.</summary>
    public CoordinateRange VerticalRange => YRange;

    /// <summary>Gets the rectangular representation of these limits.</summary>
    public CoordinateRect Rect => new(Left, Right, Bottom, Top);

    /// <summary>Gets whether both horizontal limits are real numbers.</summary>
    public bool IsRealX => NumericConversion.AreReal(Left, Right);

    /// <summary>Gets whether both vertical limits are real numbers.</summary>
    public bool IsRealY => NumericConversion.AreReal(Bottom, Top);

    /// <summary>Gets whether all limits are real numbers.</summary>
    public bool IsReal => IsRealX && IsRealY;

    /// <summary>Gets whether both spans are non-zero and all limits are real.</summary>
    public bool HasArea => IsReal && HorizontalSpan != 0 && VerticalSpan != 0;

    /// <summary>Compares two limits for equality.</summary>
    /// <param name="a">The first limits.</param>
    /// <param name="b">The second limits.</param>
    /// <returns><see langword="true"/> if the limits are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(AxisLimits a, AxisLimits b) => a.Equals(b);

    /// <summary>Compares two limits for inequality.</summary>
    /// <param name="a">The first limits.</param>
    /// <param name="b">The second limits.</param>
    /// <returns><see langword="true"/> if the limits are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(AxisLimits a, AxisLimits b) => !a.Equals(b);

    /// <summary>Creates limits for a single point.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <returns>Limits whose horizontal and vertical bounds equal the given coordinates.</returns>
    public static AxisLimits FromPoint(double x, double y) => new(x, x, y, y);

    /// <summary>Creates limits for a single point.</summary>
    /// <param name="c">The point coordinate.</param>
    /// <returns>Limits whose horizontal and vertical bounds equal the point coordinates.</returns>
    public static AxisLimits FromPoint(Coordinates c) => new(c.X, c.X, c.Y, c.Y);

    /// <summary>Creates limits with only the vertical bounds defined.</summary>
    /// <param name="yMin">The minimum vertical limit.</param>
    /// <param name="yMax">The maximum vertical limit.</param>
    /// <returns>Limits with the specified vertical bounds and undefined horizontal bounds.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "Parameter names are part of the public API for named-argument callers.")]
    public static AxisLimits VerticalOnly(double yMin, double yMax) => new(double.NaN, double.NaN, yMin, yMax);

    /// <summary>Creates limits with only the horizontal bounds defined.</summary>
    /// <param name="xMin">The minimum horizontal limit.</param>
    /// <param name="xMax">The maximum horizontal limit.</param>
    /// <returns>Limits with the specified horizontal bounds and undefined vertical bounds.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "Parameter names are part of the public API for named-argument callers.")]
    public static AxisLimits HorizontalOnly(double xMin, double xMax) => new(xMin, xMax, double.NaN, double.NaN);

    /// <summary>Creates limits expanded to include the specified point.</summary>
    /// <param name="x">The horizontal coordinate to include.</param>
    /// <param name="y">The vertical coordinate to include.</param>
    /// <returns>A copy of these limits expanded to include the point.</returns>
    public AxisLimits Expanded(double x, double y)
    {
        double minX = !double.IsNaN(Left) ? Math.Min(Left, x) : x;
        double maxX = !double.IsNaN(Right) ? Math.Max(Right, x) : x;
        double minY = !double.IsNaN(Bottom) ? Math.Min(Bottom, y) : y;
        double maxY = !double.IsNaN(Top) ? Math.Max(Top, y) : y;
        return new(minX, maxX, minY, maxY);
    }

    /// <summary>Creates limits expanded to include the specified coordinate.</summary>
    /// <param name="coordinates">The coordinate to include.</param>
    /// <returns>A copy of these limits expanded to include the coordinate.</returns>
    public AxisLimits Expanded(Coordinates coordinates) => Expanded(coordinates.X, coordinates.Y);

    /// <summary>Creates limits expanded to include all corners of the specified rectangle.</summary>
    /// <param name="rect">The rectangle to include.</param>
    /// <returns>A copy of these limits expanded to include the rectangle.</returns>
    public AxisLimits Expanded(CoordinateRect rect) => Expanded(rect.TopLeft).Expanded(rect.BottomRight);

    /// <summary>Creates limits expanded to include the specified limits.</summary>
    /// <param name="limits">The limits to include.</param>
    /// <returns>A copy of these limits expanded to include the specified limits.</returns>
    public AxisLimits Expanded(AxisLimits limits) => limits.Expanded(Left, Bottom)
            .Expanded(Right, Top);

    /// <summary>Returns limits with their vertical direction reversed.</summary>
    /// <returns>A copy of these limits with the bottom and top values exchanged.</returns>
    public AxisLimits InvertedVertically()
    {
        double invertedBottom = Top;
        double invertedTop = Bottom;
        return new(Left, Right, invertedBottom, invertedTop);
    }

    /// <summary>Returns limits with their horizontal direction reversed.</summary>
    /// <returns>A copy of these limits with the left and right values exchanged.</returns>
    public AxisLimits InvertedHorizontally()
    {
        double invertedLeft = Right;
        double invertedRight = Left;
        return new(invertedLeft, invertedRight, Bottom, Top);
    }

    /// <summary>Returns limits expanded to include the specified limits.</summary>
    /// <param name="otherLimits">The limits to include.</param>
    /// <returns>The expanded limits.</returns>
    public AxisLimits ExpandedToInclude(AxisLimits otherLimits)
    {
        ExpandingAxisLimits expandingLimits = new(this);
        expandingLimits.Expand(otherLimits);
        return expandingLimits.AxisLimits;
    }

    /// <summary>Returns a string representation of these limits.</summary>
    /// <returns>A string representation of the horizontal and vertical bounds.</returns>
    public override string ToString() => $"AxisLimits: X=[{Left}, {Right}], Y=[{Bottom}, {Top}]";

    /// <summary>Returns a string representation of these limits rounded to the specified number of digits.</summary>
    /// <param name="digits">The number of decimal digits used to round the bounds.</param>
    /// <returns>A string representation of the rounded horizontal and vertical bounds.</returns>
    public string ToString(int digits) => $"AxisLimits: X=[{Math.Round(Left, digits)}, {Math.Round(Right, digits)}], Y=[{Math.Round(Bottom, digits)}, {Math.Round(Top, digits)}]";

    /// <summary>Returns limits translated by the specified horizontal and vertical offsets.</summary>
    /// <param name="deltaX">The horizontal offset.</param>
    /// <param name="deltaY">The vertical offset.</param>
    /// <returns>A copy of these limits translated by the offsets.</returns>
    public AxisLimits WithPan(double deltaX, double deltaY) => new(Left + deltaX, Right + deltaX, Bottom + deltaY, Top + deltaY);

    /// <summary>Returns limits zoomed by the specified fractions around their center.</summary>
    /// <param name="fracX">The horizontal zoom fraction.</param>
    /// <param name="fracY">The vertical zoom fraction.</param>
    /// <returns>A copy of these limits zoomed around their center.</returns>
    public AxisLimits WithZoom(double fracX, double fracY) => WithZoom(fracX, fracY, HorizontalCenter, VerticalCenter);

    /// <summary>Returns limits zoomed by the specified fractions around a coordinate.</summary>
    /// <param name="fracX">The horizontal zoom fraction.</param>
    /// <param name="fracY">The vertical zoom fraction.</param>
    /// <param name="zoomToX">The horizontal coordinate to zoom around.</param>
    /// <param name="zoomToY">The vertical coordinate to zoom around.</param>
    /// <returns>A copy of these limits zoomed around the specified coordinate.</returns>
    public AxisLimits WithZoom(double fracX, double fracY, double zoomToX, double zoomToY)
    {
        double left = Left;
        double right = Right;
        double spanLeft = zoomToX - left;
        double spanRight = right - zoomToX;
        left = zoomToX - (spanLeft / fracX);
        right = zoomToX + (spanRight / fracX);

        double bottom = Bottom;
        double top = Top;
        double spanBottom = zoomToY - bottom;
        double spanTop = top - zoomToY;
        bottom = zoomToY - (spanBottom / fracY);
        top = zoomToY + (spanTop / fracY);

        return new(left, right, bottom, top);
    }

    /// <summary>Indicates whether the limits contain the specified coordinate.</summary>
    /// <param name="x">The horizontal coordinate to test.</param>
    /// <param name="y">The vertical coordinate to test.</param>
    /// <returns><see langword="true"/> if the coordinate is contained; otherwise, <see langword="false"/>.</returns>
    public bool Contains(double x, double y) => Rect.Contains(x, y);

    /// <summary>Indicates whether the limits contain the specified coordinate.</summary>
    /// <param name="pt">The coordinate to test.</param>
    /// <returns><see langword="true"/> if the coordinate is contained; otherwise, <see langword="false"/>.</returns>
    public bool Contains(Coordinates pt) => Rect.Contains(pt);

    /// <inheritdoc />
    public bool Equals(AxisLimits other) => Equals(Left, other.Left)
            && Equals(Right, other.Right)
            && Equals(Top, other.Top)
            && Equals(Bottom, other.Bottom);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is AxisLimits other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Left.GetHashCode()
            ^ Right.GetHashCode()
            ^ Bottom.GetHashCode()
            ^ Top.GetHashCode();

    /// <summary>Calculates a midpoint without overflowing when finite values have the same sign.</summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <returns>The midpoint of the two values.</returns>
    private static double Midpoint(double first, double second)
    {
        double sum = first + second;
        return double.IsInfinity(sum) && !double.IsInfinity(first) && !double.IsInfinity(second)
            ? first + ((second - first) * Half)
            : sum * Half;
    }
}
