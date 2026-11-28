// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Panels;

/// <summary>Provides common visibility, sizing, and geometry behavior for plot panels.</summary>
public abstract class PanelBase : IPanel
{
    /// <summary>Gets or sets whether the panel is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the plot edge containing the panel.</summary>
    public Edge Edge { get; set; }

    /// <summary>Gets or sets whether debug information is displayed.</summary>
    public bool ShowDebugInformation { get; set; }

    /// <summary>Gets or sets the minimum permitted panel size.</summary>
    public float MinimumSize { get; set; }

    /// <summary>Gets or sets the maximum permitted panel size.</summary>
    public float MaximumSize { get; set; } = float.MaxValue;

    /// <summary>Measures the panel content.</summary>
    /// <param name="paint">The paint object used for measurement.</param>
    /// <returns>The required panel size in pixels.</returns>
    public abstract float Measure(Paint paint);

    /// <summary>Renders the panel content.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    public abstract void Render(RenderPack rp, float size, float offset);

    /// <summary>Gets the rectangle occupied by the panel.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <param name="paint">The paint object used to measure panel contents.</param>
    /// <returns>The rectangle for the panel.</returns>
    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset, Paint paint) => Edge switch
    {
        Edge.Left => new(
            left: dataRect.Left - size - offset,
            right: dataRect.Left - offset,
            bottom: dataRect.Bottom,
            top: dataRect.Top),
        Edge.Right => new(
            left: dataRect.Right + offset,
            right: dataRect.Right + size + offset,
            bottom: dataRect.Bottom,
            top: dataRect.Top),
        Edge.Bottom => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Bottom + size + offset,
            top: dataRect.Bottom + offset),
        Edge.Top => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Top - offset,
            top: dataRect.Top - size - offset),
        _ => throw new ArgumentOutOfRangeException(nameof(Edge), Edge, "Unknown plot edge.")
    };
}
