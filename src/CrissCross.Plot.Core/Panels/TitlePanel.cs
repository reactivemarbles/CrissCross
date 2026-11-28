// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Panels;

/// <summary>Measures and renders the title above the plot data area.</summary>
public class TitlePanel : IPanel
{
    /// <summary>Gets or sets the extra space above the title text.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "The public field is retained to preserve the existing API for consumers.")]
    public float VerticalPadding = 10;

    /// <summary>The default font size used for the title.</summary>
    private const float DefaultFontSize = 16;

    /// <summary>Initializes a new instance of the <see cref="TitlePanel"/> class.</summary>
    public TitlePanel()
    {
        Label.Rotation = 0;
    }

    /// <summary>Gets or sets whether the title is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets the edge containing the title.</summary>
    public Edge Edge => Edge.Top;

    /// <summary>Gets or sets whether debug information is shown.</summary>
    public bool ShowDebugInformation { get; set; }

    /// <summary>Gets or sets the minimum panel size.</summary>
    public float MinimumSize { get; set; }

    /// <summary>Gets or sets the maximum panel size.</summary>
    public float MaximumSize { get; set; } = float.MaxValue;

    /// <summary>Gets or sets whether the title is centered across the full figure width.</summary>
    public bool FullFigureCenter { get; set; }

    /// <summary>Gets the title label style.</summary>
    public LabelStyle Label { get; } = new() { Text = string.Empty, FontSize = DefaultFontSize, Bold = true, Alignment = Alignment.LowerCenter, };

    /// <summary>Gets the rectangle occupied by the title panel.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <param name="paint">The paint object used to measure panel contents.</param>
    /// <returns>The rectangle for the title panel.</returns>
    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset, Paint paint) => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Top - offset,
            top: dataRect.Top - offset - size);

    /// <summary>Measures the title panel.</summary>
    /// <param name="paint">The paint object used to measure the title.</param>
    /// <returns>The required panel size in pixels.</returns>
    public float Measure(Paint paint)
    {
        if (!IsVisible)
        {
            return 0;
        }

        return string.IsNullOrWhiteSpace(Label.Text) ? 0 : Label.Measure(Label.Text, paint).Height + VerticalPadding;
    }

    /// <summary>Renders the title panel.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    public void Render(RenderPack rp, float size, float offset)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelRect panelRect = GetPanelRect(rp.DataRect, size, offset, rp.Paint);
        if (FullFigureCenter)
        {
            panelRect = new(rp.FigureRect.Left, rp.DataRect.Right, panelRect.Bottom, panelRect.Top);
        }

        Pixel labelPoint = new(panelRect.HorizontalCenter, panelRect.Bottom);
        if (ShowDebugInformation)
        {
            Drawing.DrawDebugRectangle(rp.Canvas, rp.Paint, panelRect, labelPoint, Label.ForeColor);
        }

        Label.Render(rp.Canvas, labelPoint, rp.Paint);
    }
}
