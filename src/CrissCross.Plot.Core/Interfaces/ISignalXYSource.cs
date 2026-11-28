// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides paired X and Y signal values for rendering.</summary>
public interface ISignalXYSource
{
    /// <summary>Gets the number of values in the data source.</summary>
    int Count { get; }

    /// <summary>Gets or sets the X coordinate of the first data point.</summary>
    double XOffset { get; set; }

    /// <summary>Gets or sets the offset applied to all Y values.</summary>
    double YOffset { get; set; }

    /// <summary>Gets or sets the scale applied to Y values before the offset.</summary>
    double YScale { get; set; }

    /// <summary>Gets or sets the scale applied to X values before the offset.</summary>
    double XScale { get; set; }

    /// <summary>Gets or sets the minimum data index to display.</summary>
    int MinimumIndex { get; set; }

    /// <summary>Gets or sets the maximum data index to display.</summary>
    int MaximumIndex { get; set; }

    /// <summary>Gets or sets whether X and Y coordinates are rotated during rendering.</summary>
    bool Rotated { get; set; }

    /// <summary>Gets the axis limits covered by the data.</summary>
    /// <returns>The data axis limits.</returns>
    AxisLimits GetAxisLimits();

    /// <summary>Gets the pixels used to render the signal.</summary>
    /// <param name="rp">The render pack containing plot state.</param>
    /// <param name="axes">The axes used for coordinate conversion.</param>
    /// <param name="connectStyle">The connection style between points.</param>
    /// <returns>The pixels to render, possibly including one point outside each edge.</returns>
    IReadOnlyList<Pixel> GetPixelsToDraw(RenderPack rp, IAxes axes, ConnectStyle connectStyle);

    /// <summary>Gets the data point nearest a coordinate using pixel scaling from a prior render.</summary>
    /// <param name="location">The coordinate to search around.</param>
    /// <param name="renderInfo">The render details used to calculate pixel distances.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest point, or <see cref="DataPoint.None"/> when it exceeds the maximum distance.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The existing optional parameter is retained for compatibility with callers.")]
    DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15);

    /// <summary>Gets the data point nearest a horizontal coordinate using pixel scaling from a prior render.</summary>
    /// <param name="location">The coordinate whose horizontal component is searched.</param>
    /// <param name="renderInfo">The render details used to calculate pixel distances.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest point, or <see cref="DataPoint.None"/> when it exceeds the maximum distance.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The existing optional parameter is retained for compatibility with callers.")]
    DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15);
}
