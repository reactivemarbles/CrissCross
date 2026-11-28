// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Provides shared behavior for plottable spans drawn across axes.</summary>
public abstract class AxisSpan : IPlottable, IHasLine, IHasFill, IHasLegendText
{
    /// <summary>Gets or sets whether this span is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this span.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the legacy label style.</summary>
    [Obsolete("set LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public LabelStyle Label { get => field; set => LegendText = value.Text; } = new();

    /// <summary>Gets or sets the text shown for this span in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the line style used to draw this span.</summary>
    public LineStyle LineStyle { get; set; } = new();

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the fill style used to draw this span.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the fill hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the fill hatch.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets the legend items for this span.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, FillStyle);

    /// <summary>Gets or sets whether this span contributes to axis autoscaling.</summary>
    public bool EnableAutoscale { get; set; } = true;

    /// <summary>Gets or sets whether this span can be dragged.</summary>
    public bool IsDraggable { get; set; }

    /// <summary>Gets or sets whether this span can be resized.</summary>
    public bool IsResizable { get; set; }

    /// <summary>Gets the axis limits occupied by this span.</summary>
    /// <returns>The axis limits occupied by this span.</returns>
    public abstract AxisLimits GetAxisLimits();

    /// <summary>Renders this span.</summary>
    /// <param name="rp">The render context.</param>
    public abstract void Render(RenderPack rp);

    /// <summary>Gets the span under the specified coordinate position, if any.</summary>
    /// <param name="rect">The coordinate rectangle to inspect.</param>
    /// <returns>The span hit, or <see langword="null"/> if no span is hit.</returns>
    public abstract AxisSpanUnderMouse? UnderMouse(CoordinateRect rect);

    /// <summary>Determines whether this span is under the specified coordinate position.</summary>
    /// <param name="rect">The coordinate rectangle to inspect.</param>
    /// <returns><see langword="true"/> if this span is under the position; otherwise, <see langword="false"/>.</returns>
    public bool IsUnderMouse(CoordinateRect rect) => UnderMouse(rect) is not null;

    /// <summary>Moves this span from the starting hit position to the specified coordinate.</summary>
    /// <param name="start">The initial span hit information.</param>
    /// <param name="mouseNow">The current mouse coordinate.</param>
    public abstract void DragTo(AxisSpanUnderMouse start, Coordinates mouseNow);

    /// <summary>Renders the span within the specified pixel rectangle.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="rect">The pixel rectangle to draw.</param>
    protected void Render(RenderPack rp, PixelRect rect)
    {
        Drawing.FillRectangle(rp.Canvas, rect, rp.Paint, FillStyle);
        Drawing.DrawRectangle(rp.Canvas, rect, rp.Paint, LineStyle);
    }
}
