// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a legacy line; use <see cref="CoordinateLine"/> or <see cref="PixelLine"/> instead.</summary>
[Obsolete("use CoordinateLine or PixelLine", true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete public type is retained for source and binary compatibility.")]
public class Line
{
    /// <summary>Initializes a new instance of the <see cref="Line"/> class from coordinate values.</summary>
    /// <param name="x1">The start X coordinate.</param>
    /// <param name="y1">The start Y coordinate.</param>
    /// <param name="x2">The end X coordinate.</param>
    /// <param name="y2">The end Y coordinate.</param>
    public Line(double x1, double y1, double x2, double y2)
    {
        Start = new(x1, y1);
        End = new(x2, y2);
    }

    /// <summary>Initializes a new instance of the <see cref="Line"/> class from coordinates.</summary>
    /// <param name="start">The line start coordinate.</param>
    /// <param name="end">The line end coordinate.</param>
    public Line(Coordinates start, Coordinates end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Initializes a new instance of the <see cref="Line"/> class with coordinate values and a style.</summary>
    /// <param name="x1">The start X coordinate.</param>
    /// <param name="y1">The start Y coordinate.</param>
    /// <param name="x2">The end X coordinate.</param>
    /// <param name="y2">The end Y coordinate.</param>
    /// <param name="style">The line style.</param>
    public Line(double x1, double y1, double x2, double y2, LineStyle style)
    {
        Start = new(x1, y1);
        End = new(x2, y2);
        Style = style;
    }

    /// <summary>Initializes a new instance of the <see cref="Line"/> class from coordinates and a style.</summary>
    /// <param name="start">The line start coordinate.</param>
    /// <param name="end">The line end coordinate.</param>
    /// <param name="style">The line style.</param>
    public Line(Coordinates start, Coordinates end, LineStyle style)
    {
        Start = start;
        End = end;
        Style = style;
    }

    /// <summary>Gets the default diagonal line.</summary>
    public static Line Default => new(0, 0, 1, 1);

    /// <summary>Gets or sets the start coordinate.</summary>
    public Coordinates Start { get; set; }

    /// <summary>Gets or sets the end coordinate.</summary>
    public Coordinates End { get; set; }

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle Style { get; set; } = new();

    /// <summary>Formats this line as text.</summary>
    /// <returns>A textual representation of the line.</returns>
    public override string ToString() => $"Line {{ X1 = {Start.X}, Y1 = {Start.Y}, X2 = {End.X}, Y2 = {End.Y} }}";
}
