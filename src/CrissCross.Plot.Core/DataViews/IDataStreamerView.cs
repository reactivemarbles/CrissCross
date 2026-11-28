// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Plottables;

namespace CrissCross.Plot.DataViews;

/// <summary>Contains logic for rendering fixed-length data in a streaming data logger.</summary>
public interface IDataStreamerView
{
    /// <summary>Gets the data streamer rendered by this view.</summary>
    DataStreamer Streamer { get; }

    /// <summary>Renders the streamed data.</summary>
    /// <param name="rp">The render state used to draw the data.</param>
    void Render(RenderPack rp);

    /// <summary>Gets the visible line segments for rendering.</summary>
    /// <param name="rp">The render state used to calculate the segments.</param>
    /// <returns>The segments to render.</returns>
    IReadOnlyList<Pixel[]> GetSegments(RenderPack rp);
}
