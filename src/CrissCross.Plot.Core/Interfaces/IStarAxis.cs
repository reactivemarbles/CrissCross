// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines an axis rendered as spokes around a center point.</summary>
public interface IStarAxis
{
    /// <summary>Gets or sets the style used to render the axis lines.</summary>
    LineStyle LineStyle { get; set; }

    /// <summary>Renders the star axis and its spokes.</summary>
    /// <param name="rp">The render state used to draw the axis.</param>
    /// <param name="axes">The axes used to map coordinates to pixels.</param>
    /// <param name="maxSpokeLength">The maximum spoke length relative to the visible range.</param>
    /// <param name="numSpokes">The number of spokes to render.</param>
    /// <param name="rotationDegrees">The rotation applied to the spokes.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The default rotation is retained for existing source compatibility.")]
    void Render(RenderPack rp, IAxes axes, double maxSpokeLength, int numSpokes, float rotationDegrees = 0);
}
