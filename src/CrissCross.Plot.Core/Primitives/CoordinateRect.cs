// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a rectangle in 2D coordinate space.</summary>
public struct CoordinateRect : IEquatable<CoordinateRect>
{
    /// <summary>The divisor used to calculate the center of each dimension.</summary>
    private const int CenterDivisor = 2;

    /// <summary>Initializes a new instance of the <see cref="CoordinateRect"/> struct from immutable coordinate ranges.</summary>
    /// <param name="xRange">The horizontal coordinate range.</param>
    /// <param name="yRange">The vertical coordinate range.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "The parameter names are established public named-argument identifiers.")]
    public CoordinateRect(CoordinateRange xRange, CoordinateRange yRange)
    {
        Left = xRange.Min;
        Right = xRange.Max;
        Bottom = yRange.Min;
        Top = yRange.Max;
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateRect"/> struct from mutable coordinate ranges.</summary>
    /// <param name="xRange">The horizontal coordinate range.</param>
    /// <param name="yRange">The vertical coordinate range.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "The parameter names are established public named-argument identifiers.")]
    public CoordinateRect(CoordinateRangeMutable xRange, CoordinateRangeMutable yRange)
    {
        Left = xRange.Min;
        Right = xRange.Max;
        Bottom = yRange.Min;
        Top = yRange.Max;
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateRect"/> struct from the ranges of the specified axes.</summary>
    /// <param name="axes">The axes whose ranges define the rectangle.</param>
    public CoordinateRect(IAxes axes)
        : this(axes.XAxis.Range, axes.YAxis.Range)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateRect"/> struct from normalized corner points.</summary>
    /// <param name="pt1">The first corner point.</param>
    /// <param name="pt2">The opposite corner point.</param>
    public CoordinateRect(Coordinates pt1, Coordinates pt2)
    {
        Left = Math.Min(pt1.X, pt2.X);
        Right = Math.Max(pt1.X, pt2.X);
        Bottom = Math.Min(pt1.Y, pt2.Y);
        Top = Math.Max(pt1.Y, pt2.Y);
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateRect"/> struct from its four bounds.</summary>
    /// <param name="left">The left bound.</param>
    /// <param name="right">The right bound.</param>
    /// <param name="bottom">The bottom bound.</param>
    /// <param name="top">The top bound.</param>
    public CoordinateRect(double left, double right, double bottom, double top)
    {
        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
    }

    /// <summary>Initializes a new instance of the <see cref="CoordinateRect"/> struct from a point and size.</summary>
    /// <param name="point">The point defining one corner.</param>
    /// <param name="size">The rectangle's width and height.</param>
    public CoordinateRect(Coordinates point, CoordinateSize size)
    {
        Coordinates pt2 = new(point.X + size.Width, point.Y + size.Height);
        Left = Math.Min(point.X, pt2.X);
        Right = Math.Max(point.X, pt2.X);
        Bottom = Math.Min(point.Y, pt2.Y);
        Top = Math.Max(point.Y, pt2.Y);
    }

    /// <summary>Gets the unit square spanning zero to one on both axes.</summary>
    public static CoordinateRect UnitSquare => new(0, 1, 0, 1);

    /// <summary>Gets an empty rectangle whose bounds are all <see cref="double.NaN"/>.</summary>
    public static CoordinateRect Empty => new(double.NaN, double.NaN, double.NaN, double.NaN);

    /// <summary>Gets or sets the left bound.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "Mutable bounds preserve the existing public API.")]
    public double Left { get; set; }

    /// <summary>Gets or sets the right bound.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "Mutable bounds preserve the existing public API.")]
    public double Right { get; set; }

    /// <summary>Gets or sets the bottom bound.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "Mutable bounds preserve the existing public API.")]
    public double Bottom { get; set; }

    /// <summary>Gets or sets the top bound.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "Mutable bounds preserve the existing public API.")]
    public double Top { get; set; }

    /// <summary>Gets the horizontal center coordinate.</summary>
    public readonly double HorizontalCenter => (Right + Left) / CenterDivisor;

    /// <summary>Gets the vertical center coordinate.</summary>
    public readonly double VerticalCenter => (Top + Bottom) / CenterDivisor;

    /// <summary>Gets the center point.</summary>
    public readonly Coordinates Center => new(HorizontalCenter, VerticalCenter);

    /// <summary>Gets the top-left corner.</summary>
    public readonly Coordinates TopLeft => new(Left, Top);

    /// <summary>Gets the top-right corner.</summary>
    public readonly Coordinates TopRight => new(Right, Top);

    /// <summary>Gets the bottom-left corner.</summary>
    public readonly Coordinates BottomLeft => new(Left, Bottom);

    /// <summary>Gets the bottom-right corner.</summary>
    public readonly Coordinates BottomRight => new(Right, Bottom);

    /// <summary>Gets the horizontal coordinate range.</summary>
    public readonly CoordinateRange XRange => new(Left, Right);

    /// <summary>Gets the vertical coordinate range.</summary>
    public readonly CoordinateRange YRange => new(Bottom, Top);

    /// <summary>Gets the rectangle's size.</summary>
    public readonly CoordinateSize Size => new(Width, Height);

    /// <summary>Gets the rectangle's width.</summary>
    public readonly double Width => Right - Left;

    /// <summary>Gets the rectangle's height.</summary>
    public readonly double Height => Top - Bottom;

    /// <summary>Gets the rectangle's area.</summary>
    public readonly double Area => Width * Height;

    /// <summary>Gets a value indicating whether the rectangle has finite, non-zero area.</summary>
    public readonly bool HasArea => Area != 0 && !double.IsNaN(Area) && !double.IsInfinity(Area);

    /// <summary>Gets a value indicating whether the horizontal bounds are inverted.</summary>
    public readonly bool IsInvertedX => Left > Right;

    /// <summary>Gets a value indicating whether the vertical bounds are inverted.</summary>
    public readonly bool IsInvertedY => Top < Bottom;

    /// <summary>Determines whether two rectangles are equal.</summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns><see langword="true"/> when the rectangles are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(CoordinateRect a, CoordinateRect b) => a.Equals(b);

    /// <summary>Determines whether two rectangles are not equal.</summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns><see langword="true"/> when the rectangles are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(CoordinateRect a, CoordinateRect b) => !a.Equals(b);

    /// <summary>Determines whether the specified point is inside the rectangle.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <returns><see langword="true"/> if the point is contained; otherwise, <see langword="false"/>.</returns>
    public readonly bool Contains(double x, double y) => ContainsX(x) && ContainsY(y);

    /// <summary>Determines whether the specified point is inside the rectangle.</summary>
    /// <param name="point">The point to test.</param>
    /// <returns><see langword="true"/> if the point is contained; otherwise, <see langword="false"/>.</returns>
    public readonly bool Contains(Coordinates point) => Contains(point.X, point.Y);

    /// <summary>Determines whether the horizontal coordinate is inside the rectangle.</summary>
    /// <param name="x">The horizontal coordinate to test.</param>
    /// <returns><see langword="true"/> if the coordinate is contained; otherwise, <see langword="false"/>.</returns>
    public readonly bool ContainsX(double x) => IsInvertedX ? x >= Right && x <= Left : x >= Left && x <= Right;

    /// <summary>Determines whether the vertical coordinate is inside the rectangle.</summary>
    /// <param name="y">The vertical coordinate to test.</param>
    /// <returns><see langword="true"/> if the coordinate is contained; otherwise, <see langword="false"/>.</returns>
    public readonly bool ContainsY(double y) => IsInvertedY ? y >= Top && y <= Bottom : y >= Bottom && y <= Top;

    /// <summary>Returns a rectangle expanded to include the specified point.</summary>
    /// <param name="point">The point to include.</param>
    /// <returns>The expanded rectangle.</returns>
    public readonly CoordinateRect Expanded(Coordinates point)
    {
        double expandedLeft = Left;
        double expandedRight = Right;
        double expandedBottom = Bottom;
        double expandedTop = Top;

        if (!Contains(point))
        {
            expandedLeft = Math.Min(expandedLeft, point.X);
            expandedRight = Math.Max(expandedRight, point.X);
            expandedBottom = Math.Min(expandedBottom, point.Y);
            expandedTop = Math.Max(expandedTop, point.Y);
        }

        return new(expandedLeft, expandedRight, expandedBottom, expandedTop);
    }

    /// <summary>Returns a translated copy of this rectangle.</summary>
    /// <param name="p">The translation vector.</param>
    /// <returns>The translated rectangle.</returns>
    public readonly CoordinateRect WithTranslation(Coordinates p) => new(Left + p.X, Right + p.X, Bottom + p.Y, Top + p.Y);

    /// <summary>Returns a translated copy of this rectangle.</summary>
    /// <param name="x">The horizontal translation.</param>
    /// <param name="y">The vertical translation.</param>
    /// <returns>The translated rectangle.</returns>
    public readonly CoordinateRect WithTranslation(double x, double y) => new(Left + x, Right + x, Bottom + y, Top + y);

    /// <summary>Returns a string representation of this rectangle.</summary>
    /// <returns>A string representation of the rectangle's bounds.</returns>
    public override readonly string ToString() => $"PixelRect: Left={Left} Right={Right} Bottom={Bottom} Top={Top}";

    /// <summary>Determines whether this rectangle is equal to another rectangle.</summary>
    /// <param name="other">The rectangle to compare with this instance.</param>
    /// <returns><see langword="true"/> if the rectangles are equal; otherwise, <see langword="false"/>.</returns>
    public readonly bool Equals(CoordinateRect other) => Equals(Left, other.Left)
            && Equals(Right, other.Right)
            && Equals(Top, other.Top)
            && Equals(Bottom, other.Bottom);

    /// <summary>Determines whether this rectangle is equal to the specified object.</summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><see langword="true"/> if <paramref name="obj"/> is a rectangle equal to this instance; otherwise, <see langword="false"/>.</returns>
    public override readonly bool Equals(object? obj) => obj is CoordinateRect other && Equals(other);

    /// <summary>Returns the hash code for this rectangle.</summary>
    /// <returns>The hash code for this rectangle.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "Mutable rectangle bounds and value equality are retained for public API compatibility.")]
    public override readonly int GetHashCode() => Left.GetHashCode()
            ^ Right.GetHashCode()
            ^ Bottom.GetHashCode()
            ^ Top.GetHashCode();
}
