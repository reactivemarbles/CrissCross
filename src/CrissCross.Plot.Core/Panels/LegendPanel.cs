// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Panels;

/// <summary>Provides the panel used to arrange and render a plot legend.</summary>
/// <param name="legend">The legend rendered by this panel.</param>
public class LegendPanel(Legend legend) : PanelBase
{
    /// <summary>The default padding around the legend.</summary>
    private const float DefaultPadding = 20;

    /// <summary>Gets the legend rendered by this panel.</summary>
    public Legend Legend { get; } = legend;

    /// <summary>Gets or sets the legend alignment within the panel.</summary>
    public Alignment Alignment { get; set; } = Alignment.MiddleCenter;

    /// <summary>Gets or sets the padding around the legend.</summary>
    public PixelPadding Padding { get; set; } = new(DefaultPadding, DefaultPadding);

    /// <summary>Measures the legend panel.</summary>
    /// <param name="paint">The paint object used to render the legend.</param>
    /// <returns>The panel size in pixels.</returns>
    public override float Measure(Paint paint)
    {
        if (Legend.LastRenderSize == PixelSize.NaN)
        {
            _ = Legend.GetImage();
        }

        PixelSize size = Legend.LastRenderSize;
        return Edge.IsVertical()
            ? size.Width + Padding.Horizontal
            : size.Height + Padding.Vertical;
    }

    /// <summary>Renders the legend within its panel rectangle.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    public override void Render(RenderPack rp, float size, float offset)
    {
        PixelRect rect = GetPanelRect(rp.DataRect, size, offset, rp.Paint);
        bool originalVisibility = Legend.IsVisible;
        Legend.IsVisible = true;
        Legend.Render(rp.Canvas, rp.Paint, rect, Alignment);
        Legend.IsVisible = originalVisibility;
    }
}
