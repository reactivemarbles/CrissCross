// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Panels;

/// <summary>An axis panel that displays a colormap and its value range.</summary>
/// <param name="source">The color-axis source displayed by the color bar.</param>
/// <param name="edge">The plot edge where the color bar is positioned.</param>
public class ColorBar(IHasColorAxis source, Edge edge = Edge.Right) : IPanel
{
    /// <summary>The width of the data rectangle used to estimate tick-label size.</summary>
    private const float MeasurementWidth = 600;

    /// <summary>The height of the data rectangle used to estimate tick-label size.</summary>
    private const float MeasurementHeight = 400;

    /// <summary>The error message used for invalid plot edges.</summary>
    private const string UnknownEdgeMessage = "Unknown plot edge.";

    /// <summary>Gets or sets whether the color bar is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the source of the color scale.</summary>
    public IHasColorAxis Source { get; set; } = source;

    /// <summary>Gets or sets the plot edge containing the color bar.</summary>
    public Edge Edge { get; set; } = edge;

    /// <summary>Gets the axis, including its spine, ticks, and label.</summary>
    public IAxis Axis { get; } = edge switch
    {
        Edge.Left => new AxisPanels.LeftAxis(),
        Edge.Right => new AxisPanels.RightAxis(),
        Edge.Bottom => new AxisPanels.BottomAxis(),
        Edge.Top => new AxisPanels.TopAxis(),
        _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, UnknownEdgeMessage)
    };

    /// <summary>Gets or sets the thickness of the color bar image in pixels.</summary>
    public float Width { get; set; } = 30;

    /// <summary>Gets or sets the title displayed outside the color bar ticks.</summary>
    public string Label
    {
        get => Axis.Label.Text;
        set => Axis.Label.Text = value;
    }

    /// <summary>Gets the label style used by the color bar axis.</summary>
    public LabelStyle LabelStyle => Axis.Label;

    /// <summary>Gets or sets whether debug information is shown.</summary>
    public bool ShowDebugInformation { get; set; }

    /// <summary>Gets or sets the minimum panel size.</summary>
    public float MinimumSize { get; set; }

    /// <summary>Gets or sets the maximum panel size.</summary>
    public float MaximumSize { get; set; } = float.MaxValue;

    /// <summary>Measures the color bar and its axis.</summary>
    /// <param name="paint">The paint object used to measure tick labels.</param>
    /// <returns>The required panel size in pixels.</returns>
    public float Measure(Paint paint)
    {
        if (!IsVisible)
        {
            return 0;
        }

        PixelRect guessedDataArea = new(0, MeasurementWidth, MeasurementHeight, 0);
        GenerateTicks(guessedDataArea, paint);
        float guessedAxisSize = Axis.Measure(paint);
        return Width + guessedAxisSize;
    }

    /// <summary>Gets the rectangle containing the color map image, axis, and ticks.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the plot data rectangle.</param>
    /// <param name="paint">The paint object used to measure tick labels.</param>
    /// <returns>The complete color bar panel rectangle.</returns>
    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset, Paint paint)
    {
        PixelRect bmpRect = GetColormapBitmapRect(dataRect, offset);
        GenerateTicks(dataRect, paint);
        float axisSize = Axis.Measure(paint);
        return Edge switch
        {
            Edge.Left => bmpRect.ExpandX(bmpRect.Left - axisSize),
            Edge.Right => bmpRect.ExpandX(bmpRect.Right + axisSize),
            Edge.Bottom => bmpRect.ExpandY(bmpRect.Bottom + axisSize),
            Edge.Top => bmpRect.ExpandY(bmpRect.Top - axisSize),
            _ => throw new ArgumentOutOfRangeException(nameof(Edge), Edge, UnknownEdgeMessage)
        };
    }

    /// <summary>Renders the color map and its axis.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    /// <param name="size">The size allocated for the panel.</param>
    /// <param name="offset">The distance from the plot data rectangle.</param>
    public void Render(RenderPack rp, float size, float offset)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelRect colormapRect = GetColormapBitmapRect(rp.DataRect, offset);
        RenderColorbarBitmap(rp, colormapRect);
        RenderColorbarAxis(rp, colormapRect, size);
    }

    /// <summary>Gets the rectangle beside the data area where the colormap bitmap is drawn.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <returns>The rectangle occupied by the colormap bitmap.</returns>
    private PixelRect GetColormapBitmapRect(PixelRect dataRect, float offset) => Edge switch
    {
        Edge.Left => new(
            left: dataRect.Left - Width - offset,
            right: dataRect.Left - offset,
            bottom: dataRect.Bottom,
            top: dataRect.Top),
        Edge.Right => new(
            left: dataRect.Right + offset,
            right: dataRect.Right + Width + offset,
            bottom: dataRect.Bottom,
            top: dataRect.Top),
        Edge.Bottom => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Bottom + Width + offset,
            top: dataRect.Bottom + offset),
        Edge.Top => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Top - offset,
            top: dataRect.Top - Width - offset),
        _ => throw new ArgumentOutOfRangeException(nameof(Edge), Edge, UnknownEdgeMessage)
    };

    /// <summary>Draws the color map bitmap.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    /// <param name="colormapRect">The bitmap destination rectangle.</param>
    private void RenderColorbarBitmap(RenderPack rp, PixelRect colormapRect)
    {
        using Image bmp = Edge.IsVertical() ? Source.Colormap.GetImageVertical() : Source.Colormap.GetImageHorizontal();
        rp.Paint.ResizeFilter = ResizeFilter.Bicubic;
        Drawing.DrawImage(rp.Canvas, bmp, colormapRect, rp.Paint);
    }

    /// <summary>Renders the color bar axis adjacent to the bitmap.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    /// <param name="colormapRect">The bitmap destination rectangle.</param>
    /// <param name="size">The size allocated for the panel.</param>
    private void RenderColorbarAxis(RenderPack rp, PixelRect colormapRect, float size)
    {
        GenerateTicks(rp.DataRect, rp.Paint);
        float axisSize = Edge switch
        {
            Edge.Left or Edge.Right => size - colormapRect.Width,
            Edge.Bottom or Edge.Top => size - colormapRect.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(Edge), Edge, UnknownEdgeMessage)
        };
        float axisOffset = Edge switch
        {
            Edge.Left => rp.DataRect.Left - colormapRect.Left,
            Edge.Right => colormapRect.Right - rp.DataRect.Right,
            Edge.Bottom => colormapRect.Bottom - rp.DataRect.Bottom,
            Edge.Top => rp.DataRect.Top - colormapRect.Top,
            _ => throw new ArgumentOutOfRangeException(nameof(Edge), Edge, UnknownEdgeMessage)
        };
        Axis.Render(rp, axisSize, axisOffset);
    }

    /// <summary>Regenerates axis ticks for the data range and available edge length.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="paint">The paint object used to measure tick labels.</param>
    private void GenerateTicks(PixelRect dataRect, Paint paint)
    {
        Range range = Source.GetRange();
        Axis.Min = range.Min;
        Axis.Max = range.Max;
        float edgeLength = Edge.IsVertical() ? dataRect.Height : dataRect.Width;
        Axis.RegenerateTicks(edgeLength, paint);
    }
}
