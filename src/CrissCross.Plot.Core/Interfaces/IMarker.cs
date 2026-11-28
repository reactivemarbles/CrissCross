// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes logic necessary to render a marker at a point.</summary>
public interface IMarker
{
    /// <summary>Renders a marker at the specified pixel.</summary>
    /// <param name="canvas">The canvas on which to render.</param>
    /// <param name="paint">The paint used to draw the marker.</param>
    /// <param name="center">The center point of the marker.</param>
    /// <param name="size">The marker size in pixels.</param>
    /// <param name="markerStyle">The style of the marker.</param>
    void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle);
}
