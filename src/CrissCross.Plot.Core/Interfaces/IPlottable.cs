// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Any object that renders shapes on the canvas using scale information from the axes must implement this interface.</summary>
public interface IPlottable
{
    /// <summary>Gets or sets whether this plottable is shown and contributes to automatic axis limits.</summary>
    bool IsVisible { get; set; }

    /// <summary>Gets or sets the axes and pixel data area used to render this plottable.</summary>
    IAxes Axes { get; set; }

    /// <summary>Gets the items that appear in the legend.</summary>
    IEnumerable<LegendItem> LegendItems { get; } // TODO: this should be a method GetLegendItems()

    /// <summary>Gets the 2D coordinate-space area occupied by this plottable's data.</summary>
    /// <returns>The axis limits occupied by the data.</returns>
    AxisLimits GetAxisLimits();

    /// <summary>Draws the data into the area defined by <see cref="Axes"/>.</summary>
    /// <param name="rp">The render state used to draw the plottable.</param>
    void Render(RenderPack rp);
}
