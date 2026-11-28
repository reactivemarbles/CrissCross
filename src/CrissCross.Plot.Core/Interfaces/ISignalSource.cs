// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides Y data sampled along an X axis at a fixed period.</summary>
public interface ISignalSource
{
    /// <summary>Gets or sets the X distance between Y values.</summary>
    double Period { get; set; }

    /// <summary>Gets or sets the X coordinate of the first data point.</summary>
    double XOffset { get; set; }

    /// <summary>Gets or sets the offset applied to all Y values.</summary>
    double YOffset { get; set; }

    /// <summary>Gets or sets the scale applied to Y values before the offset.</summary>
    double YScale { get; set; }

    /// <summary>Gets or sets the maximum data index to display.</summary>
    int MaximumIndex { get; set; }

    /// <summary>Gets or sets the minimum data index to display.</summary>
    int MinimumIndex { get; set; }

    /// <summary>Gets the data range at a specific horizontal pixel index.</summary>
    /// <param name="axes">The axes used to map pixels to coordinates.</param>
    /// <param name="xPixelIndex">The horizontal pixel index.</param>
    /// <returns>The pixel column and its corresponding data range.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter name is retained for existing named-argument callers.")]
    PixelColumn GetPixelColumn(IAxes axes, int xPixelIndex);

    /// <summary>Gets the predicted index nearest a horizontal coordinate.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="clamp">Whether to clamp the result to the valid data-index range.</param>
    /// <returns>The nearest data index.</returns>
    int GetIndex(double x, bool clamp);

    /// <summary>Gets the X coordinate at a data index.</summary>
    /// <param name="index">The data index.</param>
    /// <returns>The X coordinate.</returns>
    double GetX(int index);

    /// <summary>Gets the Y value at a data index.</summary>
    /// <param name="index">The data index.</param>
    /// <returns>The Y value.</returns>
    double GetY(int index);

    /// <summary>Gets all Y values.</summary>
    /// <returns>The Y values in source order.</returns>
    IReadOnlyList<double> GetYs();

    /// <summary>Gets Y values between two indexes, inclusive.</summary>
    /// <param name="index1">The first data index.</param>
    /// <param name="index2">The final data index.</param>
    /// <returns>The Y values in the requested range.</returns>
    IEnumerable<double> GetYs(int index1, int index2);

    /// <summary>Gets the horizontal coordinate limits.</summary>
    /// <returns>The horizontal limits.</returns>
    CoordinateRange GetLimitsX();

    /// <summary>Gets the vertical coordinate limits.</summary>
    /// <returns>The vertical limits.</returns>
    CoordinateRange GetLimitsY();

    /// <summary>Gets the combined coordinate limits.</summary>
    /// <returns>The combined axis limits.</returns>
    AxisLimits GetLimits();
}
