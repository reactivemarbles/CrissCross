// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>This object holds an X axis and Y axis and performs 2D coordinate/pixel conversions.</summary>
public class Axes : IAxes
{
    /// <summary>Gets a default axes object.</summary>
    public static Axes Default => new();

    // TODO: these should probably be readonly and passed into the constructor
    /// <summary>Gets or sets the horizontal axis.</summary>
    public IXAxis XAxis { get; set; } = null!;

    /// <summary>Gets or sets the vertical axis.</summary>
    public IYAxis YAxis { get; set; } = null!;

    /// <summary>Gets or sets the pixel rectangle occupied by the axes.</summary>
    public PixelRect DataRect { get; set; }

    /// <summary>Converts a pixel position to data coordinates.</summary>
    /// <param name="pixel">The pixel position.</param>
    /// <returns>The corresponding data coordinates.</returns>
    public Coordinates GetCoordinates(Pixel pixel)
    {
        double x = XAxis.GetCoordinate(pixel.X, DataRect);
        double y = YAxis.GetCoordinate(pixel.Y, DataRect);
        return new(x, y);
    }

    /// <summary>Converts a horizontal pixel position to a data coordinate.</summary>
    /// <param name="pixel">The horizontal pixel position.</param>
    /// <returns>The corresponding horizontal data coordinate.</returns>
    public double GetCoordinateX(float pixel) => XAxis.GetCoordinate(pixel, DataRect);

    /// <summary>Converts a vertical pixel position to a data coordinate.</summary>
    /// <param name="pixel">The vertical pixel position.</param>
    /// <returns>The corresponding vertical data coordinate.</returns>
    public double GetCoordinateY(float pixel) => YAxis.GetCoordinate(pixel, DataRect);

    /// <summary>Converts data coordinates to a pixel position.</summary>
    /// <param name="coordinates">The data coordinates.</param>
    /// <returns>The corresponding pixel position.</returns>
    public Pixel GetPixel(Coordinates coordinates)
    {
        float x = XAxis.GetPixel(coordinates.X, DataRect);
        float y = YAxis.GetPixel(coordinates.Y, DataRect);
        return new(x, y);
    }

    /// <summary>Converts a coordinate line to a pixel line.</summary>
    /// <param name="line">The line in data coordinates.</param>
    /// <returns>The corresponding pixel line.</returns>
    public PixelLine GetPixelLine(CoordinateLine line)
    {
        Pixel pt1 = GetPixel(line.Start);
        Pixel pt2 = GetPixel(line.End);
        return new(pt1, pt2);
    }

    /// <summary>Converts a horizontal data coordinate to a pixel position.</summary>
    /// <param name="xCoordinate">The horizontal data coordinate.</param>
    /// <returns>The corresponding horizontal pixel position.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1305",
        Justification = "The parameter name is retained for named-argument compatibility.")]
    public float GetPixelX(double xCoordinate) => XAxis.GetPixel(xCoordinate, DataRect);

    /// <summary>Converts a vertical data coordinate to a pixel position.</summary>
    /// <param name="yCoordinate">The vertical data coordinate.</param>
    /// <returns>The corresponding vertical pixel position.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1305",
        Justification = "The parameter name is retained for named-argument compatibility.")]
    public float GetPixelY(double yCoordinate) => YAxis.GetPixel(yCoordinate, DataRect);

    /// <summary>Converts a coordinate rectangle to a pixel rectangle.</summary>
    /// <param name="rect">The rectangle in data coordinates.</param>
    /// <returns>The corresponding pixel rectangle.</returns>
    public PixelRect GetPixelRect(CoordinateRect rect) => new(
            left: GetPixelX(rect.Left),
            right: GetPixelX(rect.Right),
            bottom: GetPixelY(rect.Bottom),
            top: GetPixelY(rect.Top));
}
