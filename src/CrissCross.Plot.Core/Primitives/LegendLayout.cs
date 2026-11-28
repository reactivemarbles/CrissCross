// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Contains the calculated rectangles and items for a rendered legend.</summary>
public readonly struct LegendLayout : IEquatable<LegendLayout>
{
    /// <summary>Gets an empty layout with a NaN legend rectangle.</summary>
    public static LegendLayout NoLegend => new() { LegendItems = [], LegendRect = PixelRect.NaN, LabelRects = [], SymbolRects = [], };

    /// <summary>Gets the legend items in display order.</summary>
    public required LegendItem[] LegendItems { get; init; }

    /// <summary>Gets the rectangle occupied by the legend.</summary>
    public required PixelRect LegendRect { get; init; }

    /// <summary>Gets the rectangles occupied by item labels.</summary>
    public required PixelRect[] LabelRects { get; init; }

    /// <summary>Gets the rectangles occupied by item symbols.</summary>
    public required PixelRect[] SymbolRects { get; init; }

    /// <summary>Determines whether two legend layouts are equal.</summary>
    /// <param name="left">The first layout.</param>
    /// <param name="right">The second layout.</param>
    /// <returns><see langword="true"/> when the layouts are equal.</returns>
    public static bool operator ==(LegendLayout left, LegendLayout right) => left.Equals(right);

    /// <summary>Determines whether two legend layouts differ.</summary>
    /// <param name="left">The first layout.</param>
    /// <param name="right">The second layout.</param>
    /// <returns><see langword="true"/> when the layouts differ.</returns>
    public static bool operator !=(LegendLayout left, LegendLayout right) => !left.Equals(right);

    /// <summary>Determines whether two legend layouts are equal.</summary>
    /// <param name="other">The layout to compare.</param>
    /// <returns><see langword="true"/> when corresponding layout values are equal.</returns>
    public bool Equals(LegendLayout other) =>
        ReferenceEquals(LegendItems, other.LegendItems)
        && LegendRect.Equals(other.LegendRect)
        && ReferenceEquals(LabelRects, other.LabelRects)
        && ReferenceEquals(SymbolRects, other.SymbolRects);

    /// <summary>Determines whether this layout equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal layout.</returns>
    public override bool Equals(object? obj) => obj is LegendLayout other && Equals(other);

    /// <summary>Gets a hash code for this legend layout.</summary>
    /// <returns>A hash code based on the layout values.</returns>
    public override int GetHashCode() =>
        (LegendItems?.GetHashCode() ?? 0)
        ^ LegendRect.GetHashCode()
        ^ (LabelRects?.GetHashCode() ?? 0)
        ^ (SymbolRects?.GetHashCode() ?? 0);
}
