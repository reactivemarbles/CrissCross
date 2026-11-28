// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>A stateful counterpart to <see cref="AxisLimits"/> that expands to include data.</summary>
public sealed class ExpandingAxisLimits : IEquatable<ExpandingAxisLimits>
{
    /// <summary>Initializes a new instance of the <see cref="ExpandingAxisLimits"/> class with empty limits.</summary>
    public ExpandingAxisLimits()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ExpandingAxisLimits"/> class with the specified initial limits.</summary>
    /// <param name="initialLimits">The initial axis limits.</param>
    public ExpandingAxisLimits(AxisLimits initialLimits)
    {
        Expand(initialLimits);
    }

    /// <summary>Initializes a new instance of the <see cref="ExpandingAxisLimits"/> class from the limits of the specified plottables.</summary>
    /// <param name="plottables">The plottables whose limits are included.</param>
    public ExpandingAxisLimits(IEnumerable<IPlottable> plottables)
    {
        foreach (IPlottable plottable in plottables)
        {
            Expand(plottable.GetAxisLimits());
        }
    }

    /// <summary>Gets or sets the left bound.</summary>
    public double Left { get; set; } = double.NaN;

    /// <summary>Gets or sets the right bound.</summary>
    public double Right { get; set; } = double.NaN;

    /// <summary>Gets or sets the bottom bound.</summary>
    public double Bottom { get; set; } = double.NaN;

    /// <summary>Gets or sets the top bound.</summary>
    public double Top { get; set; } = double.NaN;

    /// <summary>Gets the horizontal span.</summary>
    public double HorizontalSpan => Right - Left;

    /// <summary>Gets the vertical span.</summary>
    public double VerticalSpan => Top - Bottom;

    /// <summary>Gets whether the horizontal bounds are finite real values.</summary>
    public bool IsRealX => NumericConversion.AreReal(Left, Right);

    /// <summary>Gets whether the vertical bounds are finite real values.</summary>
    public bool IsRealY => NumericConversion.AreReal(Bottom, Top);

    /// <summary>Gets whether all bounds are finite real values.</summary>
    public bool IsReal => IsRealX && IsRealY;

    /// <summary>Gets whether the limits describe a non-zero area.</summary>
    public bool HasArea => IsReal && HorizontalSpan != 0 && VerticalSpan != 0;

    /// <summary>Gets these bounds as immutable axis limits.</summary>
    public AxisLimits AxisLimits => new(Left, Right, Bottom, Top);

    /// <summary>Determines whether two expanding limits are equal.</summary>
    /// <param name="a">The first limits.</param>
    /// <param name="b">The second limits.</param>
    /// <returns><see langword="true"/> when all bounds are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2464",
        Justification = "Value equality is retained for API compatibility; mutable limits are not safe hash keys.")]
    public static bool operator ==(ExpandingAxisLimits a, ExpandingAxisLimits b) => a.Equals(b);

    /// <summary>Determines whether two expanding limits differ.</summary>
    /// <param name="a">The first limits.</param>
    /// <param name="b">The second limits.</param>
    /// <returns><see langword="true"/> when any bound differs.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2464",
        Justification = "Value equality is retained for API compatibility; mutable limits are not safe hash keys.")]
    public static bool operator !=(ExpandingAxisLimits a, ExpandingAxisLimits b) => !a.Equals(b);

    /// <summary>Determines whether these expanding limits equal the specified axis limits.</summary>
    /// <param name="a">The expanding limits.</param>
    /// <param name="b">The axis limits.</param>
    /// <returns><see langword="true"/> when all bounds are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2464",
        Justification = "Value equality is retained for API compatibility; mutable limits are not safe hash keys.")]
    public static bool operator ==(ExpandingAxisLimits a, AxisLimits b) => a.Equals(b);

    /// <summary>Determines whether these expanding limits differ from the specified axis limits.</summary>
    /// <param name="a">The expanding limits.</param>
    /// <param name="b">The axis limits.</param>
    /// <returns><see langword="true"/> when any bound differs.</returns>
    public static bool operator !=(ExpandingAxisLimits a, AxisLimits b) => !a.Equals(b);

    /// <summary>Formats the current bounds as text.</summary>
    /// <returns>A textual representation of the bounds.</returns>
    public override string ToString() => $"Expanding Limits: X=[{Left}, {Right}] Y=[{Bottom}, {Top}]";

    /// <summary>Sets the horizontal bounds.</summary>
    /// <param name="left">The left bound.</param>
    /// <param name="right">The right bound.</param>
    public void SetX(double left, double right)
    {
        Left = left;
        Right = right;
    }

    /// <summary>Sets the vertical bounds.</summary>
    /// <param name="bottom">The bottom bound.</param>
    /// <param name="top">The top bound.</param>
    public void SetY(double bottom, double top)
    {
        Bottom = bottom;
        Top = top;
    }

    /// <summary>Expands the limits to include the specified coordinate pair.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    public void Expand(double x, double y)
    {
        ExpandX(x);
        ExpandY(y);
    }

    /// <summary>Expands the limits to include the specified plottable.</summary>
    /// <param name="plottable">The plottable whose limits are included.</param>
    public void Expand(IPlottable plottable)
    {
        AxisLimits limits = plottable.GetAxisLimits();
        Expand(limits);
    }

    /// <summary>Expands the limits to include the specified coordinate.</summary>
    /// <param name="coordinates">The coordinate to include.</param>
    public void Expand(Coordinates coordinates) => Expand(coordinates.X, coordinates.Y);

    /// <summary>Expands the limits to include the specified coordinates.</summary>
    /// <param name="coordinates">The coordinates to include.</param>
    public void Expand(IEnumerable<Coordinates> coordinates)
    {
        foreach (Coordinates coordinate in coordinates)
        {
            Expand(coordinate);
        }
    }

    /// <summary>Expands the limits to include the specified three-dimensional coordinates.</summary>
    /// <param name="coordinates">The coordinates to include, using their X and Y values.</param>
    public void Expand(IEnumerable<Coordinates3d> coordinates)
    {
        foreach (Coordinates3d coordinate in coordinates)
        {
            Expand(coordinate.X, coordinate.Y);
        }
    }

    /// <summary>Expands the limits to include the specified rectangle.</summary>
    /// <param name="rect">The rectangle whose bounds are included.</param>
    public void Expand(CoordinateRect rect)
    {
        Expand(rect.Left, rect.Top);
        Expand(rect.Right, rect.Bottom);
    }

    /// <summary>Expands the limits to include the specified axis limits.</summary>
    /// <param name="limits">The limits to include.</param>
    public void Expand(AxisLimits limits)
    {
        ExpandX(limits);
        ExpandY(limits);
    }

    /// <summary>Expands the horizontal limits to include a coordinate.</summary>
    /// <param name="x">The horizontal coordinate to include.</param>
    public void ExpandX(double x)
    {
        if (double.IsNaN(x))
        {
            return;
        }

        if (double.IsNaN(Left))
        {
            Left = x;
        }

        if (double.IsNaN(Right))
        {
            Right = x;
        }

        Left = Math.Min(Left, x);
        Right = Math.Max(Right, x);
    }

    /// <summary>Expands the horizontal limits to include the specified axis limits.</summary>
    /// <param name="limits">The axis limits to include.</param>
    public void ExpandX(AxisLimits limits)
    {
        ExpandX(limits.Left);
        ExpandX(limits.Right);
    }

    /// <summary>Expands the vertical limits to include a coordinate.</summary>
    /// <param name="y">The vertical coordinate to include.</param>
    public void ExpandY(double y)
    {
        if (double.IsNaN(y))
        {
            return;
        }

        if (double.IsNaN(Bottom))
        {
            Bottom = y;
        }

        if (double.IsNaN(Top))
        {
            Top = y;
        }

        Bottom = Math.Min(Bottom, y);
        Top = Math.Max(Top, y);
    }

    /// <summary>Expands the vertical limits to include the specified axis limits.</summary>
    /// <param name="limits">The axis limits to include.</param>
    public void ExpandY(AxisLimits limits)
    {
        ExpandY(limits.Bottom);
        ExpandY(limits.Top);
    }

    /// <summary>Determines whether these limits equal another set of expanding limits.</summary>
    /// <param name="other">The limits to compare.</param>
    /// <returns><see langword="true"/> when all bounds are equal.</returns>
    public bool Equals(ExpandingAxisLimits? other) => other is not null
        && Equals(Left, other.Left)
        && Equals(Right, other.Right)
        && Equals(Top, other.Top)
        && Equals(Bottom, other.Bottom);

    /// <summary>Determines whether these limits equal immutable axis limits.</summary>
    /// <param name="other">The axis limits to compare.</param>
    /// <returns><see langword="true"/> when all bounds are equal.</returns>
    public bool Equals(AxisLimits other) => Equals(Left, other.Left)
        && Equals(Right, other.Right)
        && Equals(Top, other.Top)
        && Equals(Bottom, other.Bottom);

    /// <summary>Determines whether these limits equal the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal set of expanding limits.</returns>
    public override bool Equals(object? obj) => obj is ExpandingAxisLimits other && Equals(other);

    /// <summary>Gets a hash code for the current limits.</summary>
    /// <returns>A hash code based on the bounds.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1482", Justification = "Mutable bounds and their existing value-equality semantics are retained for public API compatibility.")]
    public override int GetHashCode() => Left.GetHashCode()
        ^ Right.GetHashCode()
        ^ Bottom.GetHashCode()
        ^ Top.GetHashCode();
}
