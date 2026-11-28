// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a rectangle defined by pixel coordinates.</summary>
/// <remarks>
/// The left and right edges are normalized so that <see cref="Left"/> is no greater than <see cref="Right"/>.
/// The top and bottom edges are normalized for screen coordinates, where Y increases downward.
/// </remarks>
public readonly struct PixelRect : IEquatable<PixelRect>
{
    /// <summary>Gets the left edge of the rectangle.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp.Maintainability", "SST1401", Justification = "Public readonly fields are retained for binary and source compatibility.")]
    public readonly float Left;

    /// <summary>Gets the right edge of the rectangle.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp.Maintainability", "SST1401", Justification = "Public readonly fields are retained for binary and source compatibility.")]
    public readonly float Right;

    /// <summary>Gets the bottom edge of the rectangle.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp.Maintainability", "SST1401", Justification = "Public readonly fields are retained for binary and source compatibility.")]
    public readonly float Bottom;

    /// <summary>Gets the top edge of the rectangle.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp.Maintainability", "SST1401", Justification = "Public readonly fields are retained for binary and source compatibility.")]
    public readonly float Top;

    /// <summary>Divisor used to calculate rectangle midpoints.</summary>
    private const float CenterDivisor = 2F;

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="center">The center point of the rectangle.</param>
    /// <param name="radius">The distance from the center to each edge.</param>
    public PixelRect(Pixel center, float radius)
        : this(center.X - radius, center.X + radius, center.Y + radius, center.Y - radius)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="corner1">The first corner.</param>
    /// <param name="corner2">The opposite corner.</param>
    public PixelRect(Pixel corner1, Pixel corner2)
        : this(corner1.X, corner2.X, corner1.Y, corner2.Y)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="topLeftCorner">The top-left corner.</param>
    /// <param name="size">The rectangle size.</param>
    public PixelRect(Pixel topLeftCorner, PixelSize size)
        : this(topLeftCorner.X, topLeftCorner.X + size.Width, topLeftCorner.Y, topLeftCorner.Y + size.Height)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="offset">The top-left offset.</param>
    /// <param name="size">The rectangle size.</param>
    public PixelRect(PixelOffset offset, PixelSize size)
        : this(offset.X, offset.X + size.Width, offset.Y, offset.Y + size.Height)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="topLeftCorner">The top-left corner.</param>
    /// <param name="width">The rectangle width.</param>
    /// <param name="height">The rectangle height.</param>
    public PixelRect(Pixel topLeftCorner, float width, float height)
        : this(topLeftCorner.X, topLeftCorner.X + width, topLeftCorner.Y, topLeftCorner.Y + height)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="size">The rectangle size.</param>
    public PixelRect(PixelSize size)
        : this(0, size.Width, size.Height, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="width">The rectangle width.</param>
    /// <param name="height">The rectangle height.</param>
    public PixelRect(float width, float height)
        : this(0, width, height, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="size">The rectangle size.</param>
    /// <param name="offset">The top-left position.</param>
    public PixelRect(PixelSize size, Pixel offset)
        : this(offset.X, offset.X + size.Width, offset.Y + size.Height, offset.Y)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="size">The rectangle size.</param>
    /// <param name="offset">The top-left offset.</param>
    public PixelRect(PixelSize size, PixelOffset offset)
        : this(offset.X, offset.X + size.Width, offset.Y + size.Height, offset.Y)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="left">The left edge.</param>
    /// <param name="right">The right edge.</param>
    /// <param name="bottom">The bottom edge.</param>
    /// <param name="top">The top edge.</param>
    public PixelRect(float left, float right, float bottom, float top)
    {
        Left = Math.Min(left, right);
        Right = Math.Max(left, right);
        Bottom = Math.Max(top, bottom);
        Top = Math.Min(top, bottom);
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="xRange">The horizontal range.</param>
    /// <param name="yRange">The vertical range.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp.Naming", "SST1305", Justification = "Public parameter names preserve named-argument source compatibility.")]
    public PixelRect(PixelRangeX xRange, PixelRangeY yRange)
    {
        Left = xRange.Left;
        Right = xRange.Right;
        Bottom = yRange.Bottom;
        Top = yRange.Top;
    }

    /// <summary>Initializes a new instance of the <see cref="PixelRect"/> struct.</summary>
    /// <param name="pixels">The pixels to enclose. An empty sequence produces the zero rectangle.</param>
    /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is <see langword="null"/>.</exception>
    public PixelRect(IEnumerable<Pixel> pixels)
    {
        ArgumentChecks.ThrowIfNull(pixels, nameof(pixels));

        bool hasPixel = false;
        float left = 0;
        float right = 0;
        float bottom = 0;
        float top = 0;

        foreach (Pixel pixel in pixels)
        {
            if (!hasPixel)
            {
                left = pixel.X;
                right = pixel.X;
                bottom = pixel.Y;
                top = pixel.Y;
                hasPixel = true;
                continue;
            }

            left = Math.Min(pixel.X, left);
            right = Math.Max(pixel.X, right);
            bottom = Math.Max(pixel.Y, bottom);
            top = Math.Min(pixel.Y, top);
        }

        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
    }

    /// <summary>Gets the zero rectangle.</summary>
    public static PixelRect Zero => new(0, 0, 0, 0);

    /// <summary>Gets the rectangle whose edges are all not-a-number values.</summary>
    public static PixelRect NaN => new(Pixel.NaN, PixelSize.NaN);

    /// <summary>Gets the horizontal coordinate of the rectangle's center.</summary>
    public float HorizontalCenter => (Left + Right) / CenterDivisor;

    /// <summary>Gets the vertical coordinate of the rectangle's center.</summary>
    public float VerticalCenter => (Top + Bottom) / CenterDivisor;

    /// <summary>Gets the rectangle width.</summary>
    public float Width => Right - Left;

    /// <summary>Gets the rectangle height.</summary>
    public float Height => Bottom - Top;

    /// <summary>Gets whether the product of the rectangle's width and height is nonzero.</summary>
    public bool HasArea => Width * Height != 0;

    /// <summary>Gets the top-left corner.</summary>
    public Pixel TopLeft => new(Left, Top);

    /// <summary>Gets the top-right corner.</summary>
    public Pixel TopRight => new(Right, Top);

    /// <summary>Gets the bottom-left corner.</summary>
    public Pixel BottomLeft => new(Left, Bottom);

    /// <summary>Gets the bottom-right corner.</summary>
    public Pixel BottomRight => new(Right, Bottom);

    /// <summary>Gets the midpoint of the left edge.</summary>
    public Pixel LeftCenter => new(Left, VerticalCenter);

    /// <summary>Gets the midpoint of the right edge.</summary>
    public Pixel RightCenter => new(Right, VerticalCenter);

    /// <summary>Gets the midpoint of the bottom edge.</summary>
    public Pixel BottomCenter => new(HorizontalCenter, Bottom);

    /// <summary>Gets the midpoint of the top edge.</summary>
    public Pixel TopCenter => new(HorizontalCenter, Top);

    /// <summary>Gets the center point.</summary>
    public Pixel Center => new(HorizontalCenter, VerticalCenter);

    /// <summary>Gets the width and height as a pixel size.</summary>
    public PixelSize Size => new(Width, Height);

    /// <summary>Gets the line along the bottom edge.</summary>
    public PixelLine BottomLine => new(BottomLeft, BottomRight);

    /// <summary>Gets the line along the top edge.</summary>
    public PixelLine TopLine => new(TopLeft, TopRight);

    /// <summary>Gets the line along the left edge.</summary>
    public PixelLine LeftLine => new(TopLeft, BottomLeft);

    /// <summary>Gets the line along the right edge.</summary>
    public PixelLine RightLine => new(TopRight, BottomRight);

    /// <summary>Determines whether two rectangles have equal edges.</summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns><see langword="true"/> if their edges are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(PixelRect a, PixelRect b)
    {
        return a.Equals(b);
    }

    /// <summary>Determines whether two rectangles have different edges.</summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns><see langword="true"/> if any edge differs; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(PixelRect a, PixelRect b)
    {
        return !a.Equals(b);
    }

    /// <summary>Returns a copy translated by the specified horizontal and vertical distances.</summary>
    /// <param name="x">The horizontal translation.</param>
    /// <param name="y">The vertical translation.</param>
    /// <returns>The translated rectangle.</returns>
    public PixelRect WithPan(float x, float y) => new(Left + x, Right + x, Bottom + y, Top + y);

    /// <summary>Returns a string representation of this rectangle.</summary>
    /// <returns>A string containing the rectangle's edge coordinates.</returns>
    public override string ToString() => $"PixelRect: Left={Left} Right={Right} Bottom={Bottom} Top={Top}";

    /// <summary>Converts this rectangle to a SkiaSharp rectangle.</summary>
    /// <returns>A <see cref="SKRect"/> with the same edges.</returns>
    public SKRect ToSKRect() => new(Left, Top, Right, Bottom);

    /// <summary>Contracts the rectangle equally on every edge.</summary>
    /// <param name="delta">The distance to contract each edge.</param>
    /// <returns>The contracted rectangle.</returns>
    public PixelRect Contract(float delta) => Contract(delta, delta);

    /// <summary>Contracts the rectangle by separate horizontal and vertical distances.</summary>
    /// <param name="x">The horizontal contraction distance.</param>
    /// <param name="y">The vertical contraction distance.</param>
    /// <returns>The contracted rectangle, limited so its edges do not cross its center.</returns>
    public PixelRect Contract(float x, float y)
    {
        float left = Math.Min(Left + x, HorizontalCenter);
        float right = Math.Max(Right - x, HorizontalCenter);
        float bottom = Math.Max(Bottom - y, VerticalCenter);
        float top = Math.Min(Top + y, VerticalCenter);
        return new(left, right, bottom, top);
    }

    /// <summary>Contracts the rectangle by the specified edge padding.</summary>
    /// <param name="padding">The amounts to remove from each edge.</param>
    /// <returns>The contracted rectangle, limited so its edges do not cross its center.</returns>
    public PixelRect Contract(PixelPadding padding)
    {
        float left = Math.Min(Left + padding.Left, HorizontalCenter);
        float right = Math.Max(Right - padding.Right, HorizontalCenter);
        float bottom = Math.Max(Bottom - padding.Bottom, VerticalCenter);
        float top = Math.Min(Top + padding.Top, VerticalCenter);
        return new(left, right, bottom, top);
    }

    /// <summary>Expands this rectangle to include another rectangle.</summary>
    /// <param name="other">The rectangle to include.</param>
    /// <returns>The smallest rectangle containing both rectangles.</returns>
    public PixelRect Expand(PixelRect other)
    {
        float left = Math.Min(Left, other.Left);
        float right = Math.Max(Right, other.Right);
        float bottom = Math.Max(Bottom, other.Bottom);
        float top = Math.Min(Top, other.Top);
        return new(left, right, bottom, top);
    }

    /// <summary>Expands the rectangle by the specified edge padding.</summary>
    /// <param name="pad">The amounts to add beyond each edge.</param>
    /// <returns>The expanded rectangle.</returns>
    public PixelRect Expand(PixelPadding pad)
    {
        float left = Left - pad.Left;
        float right = Right + pad.Right;
        float bottom = Bottom + pad.Bottom;
        float top = Top - pad.Top;
        return new(left, right, bottom, top);
    }

    /// <summary>Expands the rectangle equally on every edge.</summary>
    /// <param name="delta">The distance to expand each edge.</param>
    /// <returns>The expanded rectangle.</returns>
    public PixelRect Expand(float delta) => Contract(-delta);

    /// <summary>Returns the intersection with another rectangle.</summary>
    /// <param name="other">The other rectangle.</param>
    /// <returns>The intersection rectangle, or <see cref="NaN"/> if the rectangles do not overlap.</returns>
    public PixelRect Intersect(PixelRect other)
    {
        float left = Math.Max(Left, other.Left);
        float right = Math.Min(Right, other.Right);

        if (left > right)
        {
            return NaN;
        }

        float bottom = Math.Min(Bottom, other.Bottom);
        float top = Math.Max(Top, other.Top);

        return top > bottom ? NaN : new PixelRect(left, right, bottom, top);
    }

    /// <summary>Expands the rectangle horizontally to include the specified coordinate.</summary>
    /// <param name="x">The horizontal coordinate to include.</param>
    /// <returns>The expanded rectangle.</returns>
    public PixelRect ExpandX(float x)
    {
        float left = Math.Min(Left, x);
        float right = Math.Max(Right, x);
        return new(left, right, Bottom, Top);
    }

    /// <summary>Expands the rectangle vertically to include the specified coordinate.</summary>
    /// <param name="y">The vertical coordinate to include.</param>
    /// <returns>The expanded rectangle.</returns>
    public PixelRect ExpandY(float y)
    {
        float top = Math.Min(Top, y);
        float bottom = Math.Max(Bottom, y);
        return new(Left, Right, bottom, top);
    }

    /// <summary>Returns a copy translated by the specified horizontal and vertical distances.</summary>
    /// <param name="x">The horizontal translation.</param>
    /// <param name="y">The vertical translation.</param>
    /// <returns>The translated rectangle.</returns>
    public PixelRect WithDelta(float x, float y) => WithPan(x, y);

    /// <summary>Returns a copy translated by the specified distances.</summary>
    /// <param name="x">The horizontal translation.</param>
    /// <param name="y">The vertical translation.</param>
    /// <param name="alignment">The alignment associated with the translation.</param>
    /// <returns>The translated rectangle. The alignment value does not change the translation.</returns>
    public PixelRect WithDelta(float x, float y, Alignment alignment) => new(Left + x, Right + x, Bottom + y, Top + y);

    /// <summary>Determines whether this rectangle has the same edges as another rectangle.</summary>
    /// <param name="other">The rectangle to compare.</param>
    /// <returns><see langword="true"/> if all edges are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(PixelRect other) => Equals(Left, other.Left)
            && Equals(Right, other.Right)
            && Equals(Top, other.Top)
            && Equals(Bottom, other.Bottom);

    /// <summary>Determines whether this rectangle is equal to the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="obj"/> is an equal <see cref="PixelRect"/>; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj) => obj is PixelRect other && Equals(other);

    /// <summary>Returns a hash code for this rectangle.</summary>
    /// <returns>A hash code based on the rectangle edges.</returns>
    public override int GetHashCode() => Left.GetHashCode()
            ^ Right.GetHashCode()
            ^ Bottom.GetHashCode()
            ^ Top.GetHashCode();

    /// <summary>Determines whether the rectangle contains a pixel.</summary>
    /// <param name="px">The pixel to test.</param>
    /// <returns><see langword="true"/> if the pixel lies on or inside every edge; otherwise, <see langword="false"/>.</returns>
    public bool Contains(Pixel px) => Contains(px.X, px.Y);

    /// <summary>Determines whether the rectangle contains the specified coordinates.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <returns><see langword="true"/> if the coordinates lie on or inside every edge; otherwise, <see langword="false"/>.</returns>
    public bool Contains(float x, float y) => Left <= x && x <= Right && Top <= y && y <= Bottom;

    /// <summary>Determines whether the rectangle contains a horizontal coordinate.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <returns><see langword="true"/> if the coordinate is on or between the left and right edges.</returns>
    public bool ContainsX(float x) => Left <= x && x <= Right;

    /// <summary>Determines whether the rectangle contains a vertical coordinate.</summary>
    /// <param name="y">The vertical coordinate.</param>
    /// <returns><see langword="true"/> if the coordinate is on or between the top and bottom edges.</returns>
    public bool ContainsY(float y) => Top <= y && y <= Bottom;

    /// <summary>Gets the point at the specified alignment within the rectangle.</summary>
    /// <param name="alignment">The alignment position.</param>
    /// <returns>The point at that position, or <see cref="Pixel.NaN"/> for an unrecognized value.</returns>
    public Pixel GetAlignedPixel(Alignment alignment) => alignment switch
        {
            Alignment.UpperLeft => TopLeft,
            Alignment.UpperCenter => TopCenter,
            Alignment.UpperRight => TopRight,
            Alignment.MiddleLeft => LeftCenter,
            Alignment.MiddleCenter => new(HorizontalCenter, VerticalCenter),
            Alignment.MiddleRight => RightCenter,
            Alignment.LowerLeft => BottomLeft,
            Alignment.LowerCenter => BottomCenter,
            Alignment.LowerRight => BottomRight,
            _ => Pixel.NaN,
        };

    /// <summary>Returns a copy translated by a pixel offset.</summary>
    /// <param name="offset">The horizontal and vertical translation.</param>
    /// <returns>The translated rectangle.</returns>
    public PixelRect WithOffset(PixelOffset offset) => new(
            left: Left + offset.X,
            right: Right + offset.X,
            bottom: Bottom + offset.Y,
            top: Top + offset.Y);

    /// <summary>Positions this rectangle inside a larger rectangle using the specified alignment.</summary>
    /// <param name="largerRect">The rectangle that will contain this rectangle.</param>
    /// <param name="alignment">The alignment position.</param>
    /// <returns>A rectangle with this rectangle's size positioned inside <paramref name="largerRect"/>.</returns>
    public PixelRect AlignedInside(PixelRect largerRect, Alignment alignment) => AlignedInside(largerRect, alignment, PixelPadding.Zero);

    /// <summary>Positions this rectangle inside a padded larger rectangle using the specified alignment.</summary>
    /// <param name="largerRect">The rectangle that will contain this rectangle.</param>
    /// <param name="alignment">The alignment position.</param>
    /// <param name="padding">The padding to contract from the containing rectangle.</param>
    /// <returns>A rectangle with this rectangle's size positioned in the padded area.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="alignment"/> is not a supported alignment.</exception>
    public PixelRect AlignedInside(PixelRect largerRect, Alignment alignment, PixelPadding padding)
    {
        PixelRect inner = largerRect.Contract(padding);

        return alignment switch
        {
            Alignment.UpperLeft => new PixelRect(
                left: inner.Left,
                right: inner.Left + Width,
                bottom: inner.Top + Height,
                top: inner.Top),

            Alignment.UpperCenter => new PixelRect(
                left: inner.HorizontalCenter - (Width / CenterDivisor),
                right: inner.HorizontalCenter + (Width / CenterDivisor),
                bottom: inner.Top + Height,
                top: inner.Top),

            Alignment.UpperRight => new PixelRect(
                left: inner.Right - Width,
                right: inner.Right,
                bottom: inner.Top + Height,
                top: inner.Top),

            Alignment.MiddleLeft => new PixelRect(
                left: inner.Left,
                right: inner.Left + Width,
                bottom: inner.VerticalCenter + (Height / CenterDivisor),
                top: inner.VerticalCenter - (Height / CenterDivisor)),

            Alignment.MiddleCenter => new PixelRect(
                left: inner.HorizontalCenter - (Width / CenterDivisor),
                right: inner.HorizontalCenter + (Width / CenterDivisor),
                bottom: inner.VerticalCenter + (Height / CenterDivisor),
                top: inner.VerticalCenter - (Height / CenterDivisor)),

            Alignment.MiddleRight => new PixelRect(
                left: inner.Right - Width,
                right: inner.Right,
                bottom: inner.VerticalCenter + (Height / CenterDivisor),
                top: inner.VerticalCenter - (Height / CenterDivisor)),

            Alignment.LowerLeft => new PixelRect(
                left: inner.Left,
                right: inner.Left + Width,
                bottom: inner.Bottom,
                top: inner.Bottom - Height),

            Alignment.LowerCenter => new PixelRect(
                left: inner.HorizontalCenter - (Width / CenterDivisor),
                right: inner.HorizontalCenter + (Width / CenterDivisor),
                bottom: inner.Bottom,
                top: inner.Bottom - Height),

            Alignment.LowerRight => new PixelRect(
                left: inner.Right - Width,
                right: inner.Right,
                bottom: inner.Bottom,
                top: inner.Bottom - Height),

            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Unsupported alignment."),
        };
    }

    /// <summary>Returns the subrectangle corresponding to a fractional region of this rectangle.</summary>
    /// <param name="frac">The fractional edges defining the region.</param>
    /// <returns>The rectangle described by the fractional region.</returns>
    public PixelRect Fraction(FractionRect frac)
    {
        float left = ((float)frac.Left * Width) + Left;
        float right = ((float)frac.Right * Width) + Left;
        float top = ((float)frac.Top * Height) + Top;
        float bottom = ((float)frac.Bottom * Height) + Top;
        return new(left, right, bottom, top);
    }
}
