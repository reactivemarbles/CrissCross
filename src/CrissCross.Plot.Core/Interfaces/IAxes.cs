// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a pair of axes and provides coordinate-to-pixel conversion operations.</summary>
public interface IAxes
{
    /// <summary>Gets or sets the central figure region where plottable data is displayed.</summary>
    PixelRect DataRect { get; set; }

    /// <summary>Gets or sets the horizontal axis used by this axis pair.</summary>
    IXAxis XAxis { get; set; }

    /// <summary>Gets or sets the vertical axis used by this axis pair.</summary>
    IYAxis YAxis { get; set; }

    /// <summary>Converts a coordinate rectangle to a pixel rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to convert.</param>
    /// <returns>The corresponding pixel rectangle.</returns>
    PixelRect GetPixelRect(CoordinateRect rect);

    /// <summary>Converts a coordinate line to a pixel line.</summary>
    /// <param name="line">The coordinate line to convert.</param>
    /// <returns>The corresponding pixel line.</returns>
    PixelLine GetPixelLine(CoordinateLine line);

    /// <summary>Converts coordinates to a pixel position.</summary>
    /// <param name="coordinates">The coordinates to convert.</param>
    /// <returns>The corresponding pixel position.</returns>
    Pixel GetPixel(Coordinates coordinates);

    /// <summary>Converts a horizontal coordinate to a pixel position.</summary>
    /// <param name="xCoordinate">The horizontal coordinate to convert.</param>
    /// <returns>The corresponding horizontal pixel position.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter name is retained for existing named-argument callers.")]
    float GetPixelX(double xCoordinate);

    /// <summary>Converts a vertical coordinate to a pixel position.</summary>
    /// <param name="yCoordinate">The vertical coordinate to convert.</param>
    /// <returns>The corresponding vertical pixel position.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter name is retained for existing named-argument callers.")]
    float GetPixelY(double yCoordinate);

    /// <summary>Converts a pixel position to coordinates.</summary>
    /// <param name="pixel">The pixel position to convert.</param>
    /// <returns>The corresponding coordinates.</returns>
    Coordinates GetCoordinates(Pixel pixel);

    /// <summary>Converts a horizontal pixel position to a coordinate.</summary>
    /// <param name="pixel">The horizontal pixel position to convert.</param>
    /// <returns>The corresponding horizontal coordinate.</returns>
    double GetCoordinateX(float pixel);

    /// <summary>Converts a vertical pixel position to a coordinate.</summary>
    /// <param name="pixel">The vertical pixel position to convert.</param>
    /// <returns>The corresponding vertical coordinate.</returns>
    double GetCoordinateY(float pixel);
}
