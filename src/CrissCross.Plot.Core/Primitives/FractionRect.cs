// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a rectangular region of a larger rectangle using fractional units.</summary>
public readonly struct FractionRect : IEquatable<FractionRect>
{
    /// <summary>Initializes a new instance of the <see cref="FractionRect"/> struct from its position and dimensions.</summary>
    /// <param name="left">The left edge as a fraction of the parent width.</param>
    /// <param name="top">The top edge as a fraction of the parent height.</param>
    /// <param name="width">The width as a fraction of the parent width.</param>
    /// <param name="height">The height as a fraction of the parent height.</param>
    public FractionRect(double left, double top, double width, double height)
    {
        if (left < 0 || left >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(left), "must be >=0 and <1");
        }

        if (top < 0 || top >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(top), "must be >=0 and <1");
        }

        Left = left;
        Top = top;
        Width = width;
        Height = height;

        if (Bottom > 1)
        {
            throw new ArgumentException($"{nameof(top)} + {nameof(height)} must not exceed 1");
        }

        if (Right <= 1)
        {
            return;
        }

        throw new ArgumentException($"{nameof(left)} + {nameof(width)} must not exceed 1");
    }

    /// <summary>Gets a fractional rectangle covering the entire parent rectangle.</summary>
    public static FractionRect Full => new(0, 0, 1, 1);

    /// <summary>Gets the left edge fraction.</summary>
    public double Left { get; }

    /// <summary>Gets the top edge fraction.</summary>
    public double Top { get; }

    /// <summary>Gets the width fraction.</summary>
    public double Width { get; }

    /// <summary>Gets the height fraction.</summary>
    public double Height { get; }

    /// <summary>Gets the right edge fraction.</summary>
    public double Right => Left + Width;

    /// <summary>Gets the bottom edge fraction.</summary>
    public double Bottom => Top + Height;

    /// <summary>Determines whether two fractional rectangles are equal.</summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns><see langword="true"/> when all rectangle values are equal.</returns>
    public static bool operator ==(FractionRect a, FractionRect b) => a.Equals(b);

    /// <summary>Determines whether two fractional rectangles differ.</summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns><see langword="true"/> when any rectangle value differs.</returns>
    public static bool operator !=(FractionRect a, FractionRect b) => !a.Equals(b);

    /// <summary>Creates a fractional rectangle for one column in a grid.</summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="columnCount">The number of columns.</param>
    /// <returns>The fractional rectangle for the column.</returns>
    public static FractionRect Column(int columnIndex, int columnCount)
    {
        double width = 1.0 / columnCount;
        double x = width * columnIndex;
        return new(x, 0, width, 1);
    }

    /// <summary>Creates a fractional rectangle for one row in a grid.</summary>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <param name="rowCount">The number of rows.</param>
    /// <returns>The fractional rectangle for the row.</returns>
    public static FractionRect Row(int rowIndex, int rowCount)
    {
        double height = 1.0 / rowCount;
        double y = height * rowIndex;
        return new(0, y, 1, height);
    }

    /// <summary>Creates a fractional rectangle for one grid cell.</summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <param name="columnCount">The number of columns.</param>
    /// <param name="rowCount">The number of rows.</param>
    /// <returns>The fractional rectangle for the cell.</returns>
    public static FractionRect GridCell(int columnIndex, int rowIndex, int columnCount, int rowCount)
    {
        double width = 1.0 / columnCount;
        double x = width * columnIndex;
        double height = 1.0 / rowCount;
        double y = height * rowIndex;
        return new(x, y, width, height);
    }

    /// <summary>Creates a pixel rectangle within the specified parent rectangle.</summary>
    /// <param name="fullRect">The parent rectangle.</param>
    /// <returns>The pixel rectangle corresponding to this fractional rectangle.</returns>
    public PixelRect GetPixelRect(PixelRect fullRect)
    {
        Pixel topLeft = new(Left * fullRect.Width, Top * fullRect.Height);
        PixelSize size = new(Width * fullRect.Width, Height * fullRect.Height);
        return new(topLeft, size);
    }

    /// <summary>Creates a pixel rectangle within a parent of the specified dimensions.</summary>
    /// <param name="width">The parent width in pixels.</param>
    /// <param name="height">The parent height in pixels.</param>
    /// <returns>The pixel rectangle corresponding to this fractional rectangle.</returns>
    public PixelRect GetPixelRect(float width, float height)
    {
        Pixel topLeft = new(Left * width, Top * height);
        PixelSize size = new(Width * width, Height * height);
        return new(topLeft, size);
    }

    /// <summary>Determines whether this fractional rectangle equals another rectangle.</summary>
    /// <param name="other">The rectangle to compare.</param>
    /// <returns><see langword="true"/> when all values are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact value equality is required to remain consistent with the hash code.")]
    public bool Equals(FractionRect other) => Left.Equals(other.Left)
        && Top.Equals(other.Top)
        && Width.Equals(other.Width)
        && Height.Equals(other.Height);

    /// <summary>Determines whether this fractional rectangle equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal rectangle.</returns>
    public override bool Equals(object? obj) => obj is FractionRect other && Equals(other);

    /// <summary>Gets a hash code for this fractional rectangle.</summary>
    /// <returns>A hash code based on its position and dimensions.</returns>
    public override int GetHashCode() => Left.GetHashCode()
        ^ Top.GetHashCode()
        ^ Width.GetHashCode()
        ^ Height.GetHashCode();
}
