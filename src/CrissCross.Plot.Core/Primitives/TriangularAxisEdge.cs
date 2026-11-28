// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines one edge of a triangular axis and its associated ticks and labels.</summary>
public class TriangularAxisEdge
{
    /// <summary>Gets the maximum vertical coordinate of the unit triangle.</summary>
    public static readonly double MaxY = Math.Sqrt(Defaults.TriangleRadicand) / Defaults.TriangleHeightDivisor;

    /// <summary>Gets the axis limits for a unit triangle.</summary>
    public static readonly AxisLimits AxisLimits = new(0, 1, 0, MaxY);

    /// <summary>The top vertex of the unit triangle.</summary>
    private static Coordinates _topCorner = new(Defaults.TriangleCenterX, Math.Sqrt(Defaults.TriangleRadicand) / Defaults.TriangleHeightDivisor);

    /// <summary>The left vertex of the unit triangle.</summary>
    private static Coordinates _leftCorner = new(0, 0);

    /// <summary>The right vertex of the unit triangle.</summary>
    private static Coordinates _rightCorner = new(1, 0);

    /// <summary>Initializes a new instance of the <see cref="TriangularAxisEdge"/> class.</summary>
    /// <param name="line">The coordinate line for the edge.</param>
    public TriangularAxisEdge(CoordinateLine line)
    {
        Line = line;
        RegenerateTicks();
    }

    /// <summary>Gets the coordinate line defining this edge.</summary>
    public CoordinateLine Line { get; }

    /// <summary>Gets the starting coordinate of this edge.</summary>
    public Coordinates Start => Line.Start;

    /// <summary>Gets the ending coordinate of this edge.</summary>
    public Coordinates End => Line.End;

    /// <summary>Gets the points and labels for the ticks on this edge.</summary>
    public List<(Coordinates Point, string Label)> Ticks { get; } = [];

    /// <summary>Gets or sets the line style for the edge.</summary>
    public LineStyle EdgeLineStyle { get; set; } = new() { IsVisible = true, Width = 1, Color = Colors.Black, };

    /// <summary>Gets or sets the tick mark style for the edge.</summary>
    public TickMarkStyle TickMarkStyle { get; set; } = new() { Length = Defaults.TickLength, Color = Colors.Black };

    /// <summary>Gets or sets the label style for the tick labels.</summary>
    public LabelStyle TickLabelStyle { get; set; } = new();

    /// <summary>Gets or sets the style for the edge label.</summary>
    public LabelStyle LabelStyle { get; set; } = new() { FontSize = Defaults.LabelFontSize, Bold = true };

    /// <summary>Gets or sets the offset applied to the tick marks.</summary>
    public PixelOffset TickOffset { get; set; }

    /// <summary>Gets or sets the text displayed as the edge label.</summary>
    public string LabelText { get; set; } = string.Empty;

    /// <summary>Creates the left edge of the unit triangle.</summary>
    /// <param name="clockwise">Whether the edge direction is clockwise.</param>
    /// <returns>The configured left edge.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is retained for source and binary compatibility.")]
    public static TriangularAxisEdge Left(bool clockwise = true)
    {
        CoordinateLine line = new(_leftCorner, _topCorner);
        TriangularAxisEdge ax = new(clockwise ? line : line.Reversed());
        ax.TickOffset = new(-Defaults.HorizontalTickOffset, 0);
        ax.TickLabelStyle.Alignment = Alignment.MiddleRight;
        ax.LabelStyle.Rotation = -Defaults.LabelRotationDegrees;
        ax.LabelStyle.Alignment = Alignment.LowerCenter;
        ax.LabelStyle.OffsetX = -Defaults.HorizontalLabelOffset;
        ax.LabelStyle.OffsetY = ax.LabelStyle.OffsetX * (float)Math.Sqrt(Defaults.TriangleRadicand) / Defaults.TriangleHeightDivisor;
        return ax;
    }

    /// <summary>Creates the right edge of the unit triangle.</summary>
    /// <param name="clockwise">Whether the edge direction is clockwise.</param>
    /// <returns>The configured right edge.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is retained for source and binary compatibility.")]
    public static TriangularAxisEdge Right(bool clockwise = true)
    {
        CoordinateLine line = new(_topCorner, _rightCorner);
        TriangularAxisEdge ax = new(clockwise ? line : line.Reversed());
        ax.TickOffset = new(Defaults.HorizontalTickOffset, 0);
        ax.TickLabelStyle.Alignment = Alignment.MiddleLeft;
        ax.LabelStyle.Rotation = Defaults.LabelRotationDegrees;
        ax.LabelStyle.Alignment = Alignment.LowerCenter;
        ax.LabelStyle.OffsetX = Defaults.HorizontalLabelOffset;
        ax.LabelStyle.OffsetY = -ax.LabelStyle.OffsetX * (float)Math.Sqrt(Defaults.TriangleRadicand) / Defaults.TriangleHeightDivisor;
        return ax;
    }

    /// <summary>Creates the bottom edge of the unit triangle.</summary>
    /// <param name="clockwise">Whether the edge direction is clockwise.</param>
    /// <returns>The configured bottom edge.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is retained for source and binary compatibility.")]
    public static TriangularAxisEdge Bottom(bool clockwise = true)
    {
        CoordinateLine line = new(_rightCorner, _leftCorner);
        TriangularAxisEdge ax = new(clockwise ? line : line.Reversed());
        ax.TickOffset = new(0, Defaults.VerticalTickOffset);
        ax.TickLabelStyle.Alignment = Alignment.UpperCenter;
        ax.LabelStyle.Alignment = Alignment.UpperCenter;
        ax.LabelStyle.OffsetY = Defaults.VerticalLabelOffset;
        return ax;
    }

    /// <summary>Sets the edge label text.</summary>
    /// <param name="title">The label text.</param>
    public void Title(string title) => LabelText = title;

    /// <summary>Sets the edge label text and style color.</summary>
    /// <param name="title">The label text.</param>
    /// <param name="color">The color applied to the edge, tick marks, and labels.</param>
    public void Title(string title, Color color)
    {
        LabelText = title;
        Color(color);
    }

    /// <summary>Sets the color of the edge, tick marks, and labels.</summary>
    /// <param name="color">The color to apply.</param>
    public void Color(Color color)
    {
        LabelStyle.ForeColor = color;
        TickLabelStyle.ForeColor = color;
        TickMarkStyle.Color = color;
        EdgeLineStyle.Color = color;
    }

    /// <summary>Returns the point at a fraction along this axis edge.</summary>
    /// <param name="fraction">The fraction between zero and one.</param>
    /// <returns>The coordinate at the requested fraction.</returns>
    public Coordinates GetCoordinates(double fraction)
    {
        double x = Start.X + ((End.X - Start.X) * fraction);
        double y = Start.Y + ((End.Y - Start.Y) * fraction);
        return new(x, y);
    }

    /// <summary>Recreates the tick positions and labels for this edge.</summary>
    /// <param name="ticksPerEdge">The number of tick intervals along the edge.</param>
    private void RegenerateTicks(int ticksPerEdge = 10)
    {
        Ticks.Clear();
        for (int i = 0; i <= ticksPerEdge; i++)
        {
            double fraction = i / (double)ticksPerEdge;
            double tickX = Start.X + (fraction * (End.X - Start.X));
            double tickY = Start.Y + (fraction * (End.Y - Start.Y));
            Coordinates tickPoint = new(tickX, tickY);
            string tickLabel = $"{fraction * Defaults.TickLabelScale}";
            Ticks.Add((tickPoint, tickLabel));
        }
    }

    /// <summary>Stores named defaults used when constructing triangular axis edges.</summary>
    private static class Defaults
    {
        /// <summary>The radicand used to calculate the unit triangle height.</summary>
        public const int TriangleRadicand = 3;

        /// <summary>The divisor used to calculate the unit triangle height.</summary>
        public const int TriangleHeightDivisor = 2;

        /// <summary>The default length of a tick mark.</summary>
        public const float TickLength = 5;

        /// <summary>The default font size for the axis label.</summary>
        public const int LabelFontSize = 16;

        /// <summary>The default horizontal tick offset magnitude.</summary>
        public const int HorizontalTickOffset = 5;

        /// <summary>The default vertical tick offset magnitude.</summary>
        public const int VerticalTickOffset = 5;

        /// <summary>The default edge label rotation magnitude in degrees.</summary>
        public const int LabelRotationDegrees = 60;

        /// <summary>The default horizontal label offset magnitude.</summary>
        public const int HorizontalLabelOffset = 10;

        /// <summary>The default vertical label offset.</summary>
        public const int VerticalLabelOffset = 20;

        /// <summary>The scale used to label each tenth of an edge.</summary>
        public const int TickLabelScale = 10;

        /// <summary>The horizontal coordinate of the triangle's top vertex.</summary>
        public const double TriangleCenterX = 0.5;
    }
}
