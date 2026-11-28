// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>A rectangular region outside the data area, such as an axis, colorbar, or title.</summary>
public interface IPanel
{
    /// <summary>Gets or sets whether the panel is visible and reports its size.</summary>
    bool IsVisible { get; set; }

    /// <summary>Gets or sets the minimum allowed panel size in pixels.</summary>
    float MinimumSize { get; set; }

    /// <summary>Gets or sets the maximum allowed panel size in pixels.</summary>
    float MaximumSize { get; set; }

    /// <summary>Gets the edge of the data rectangle occupied by this panel.</summary>
    Edge Edge { get; }

    /// <summary>Gets or sets whether the panel displays extra development information.</summary>
    bool ShowDebugInformation { get; set; }

    /// <summary>Measures the panel perpendicular to its edge.</summary>
    /// <param name="paint">The paint used to measure text and graphics.</param>
    /// <returns>The panel size in pixels.</returns>
    float Measure(Paint paint);

    /// <summary>Draws this panel on the plot canvas.</summary>
    /// <param name="rp">The render context containing the canvas and plot state.</param>
    /// <param name="size">The size of this panel in pixels.</param>
    /// <param name="offset">The distance from this panel to the data area in pixels.</param>
    void Render(RenderPack rp, float size, float offset);

    /// <summary>Gets the rectangle occupied by this panel.</summary>
    /// <param name="dataRect">The rectangle containing the data area.</param>
    /// <param name="size">The panel size in pixels.</param>
    /// <param name="offset">The distance from the panel to the data area.</param>
    /// <param name="paint">The paint used to measure the panel.</param>
    /// <returns>The panel rectangle.</returns>
    PixelRect GetPanelRect(PixelRect dataRect, float size, float offset, Paint paint);
}
