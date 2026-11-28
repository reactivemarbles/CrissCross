// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisPanels;

/// <summary>Provides shared behavior for horizontal axes.</summary>
public abstract class XAxisBase : AxisBase, IXAxis
{
    /// <summary>Initializes a new instance of the <see cref="XAxisBase"/> class with unrotated labels.</summary>
    protected XAxisBase()
    {
        LabelRotation = 0;
    }

    /// <summary>Gets the coordinate span displayed by the axis.</summary>
    public double Width => Range.Span;

    /// <summary>Measures the space required to render the axis.</summary>
    /// <param name="paint">The paint used to measure labels.</param>
    /// <returns>The required height in pixels.</returns>
    public virtual float Measure(Paint paint)
    {
        if (!IsVisible)
        {
            return 0;
        }

        if (!Range.HasBeenSet)
        {
            return SizeWhenNoData;
        }

        float tickHeight = MajorTickStyle.Length;

        float maxTickLabelHeight = 0;
        foreach (Tick tick in TickGenerator.Ticks)
        {
            float tickLabelHeight = TickLabelStyle.Measure(tick.Label, paint).Height;
            maxTickLabelHeight = Math.Max(maxTickLabelHeight, tickLabelHeight);
        }

        float axisLabelHeight = string.IsNullOrEmpty(LabelStyle.Text) && LabelStyle.Image is null
            ? EmptyLabelPadding.Vertical
            : LabelStyle.Measure(LabelText, paint).Height
                + PaddingBetweenTickAndAxisLabels.Vertical
                + PaddingOutsideAxisLabels.Vertical;

        return tickHeight + maxTickLabelHeight + axisLabelHeight;
    }

    /// <summary>Converts a coordinate position to a pixel position.</summary>
    /// <param name="position">The coordinate position.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The corresponding horizontal pixel position.</returns>
    public float GetPixel(double position, PixelRect dataArea)
    {
        double pixelsPerUnit = dataArea.Width / Width;
        double unitsFromLeftEdge = position - Min;
        float pixelsFromEdge = (float)(unitsFromLeftEdge * pixelsPerUnit);
        return dataArea.Left + pixelsFromEdge;
    }

    /// <summary>Converts a pixel position to a coordinate position.</summary>
    /// <param name="pixel">The horizontal pixel position.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The corresponding coordinate position.</returns>
    public double GetCoordinate(float pixel, PixelRect dataArea)
    {
        double pixelsPerUnit = dataArea.Width / Width;
        float pixelsFromLeftEdge = pixel - dataArea.Left;
        double unitsFromEdge = pixelsFromLeftEdge / pixelsPerUnit;
        return Min + unitsFromEdge;
    }

    /// <summary>Gets the rectangle occupied by the axis panel.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The axis panel height.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <param name="paint">The paint used to render the axis.</param>
    /// <returns>The axis panel rectangle.</returns>
    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset, Paint paint) => Edge == Edge.Bottom
        ? GetPanelRectangleBottom(dataRect, size, offset)
        : GetPanelRectangleTop(dataRect, size, offset);

    /// <summary>Renders the horizontal axis and its labels.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="size">The axis panel height.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    public virtual void Render(RenderPack rp, float size, float offset)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelRect panelRect = GetPanelRect(rp.DataRect, size, offset, rp.Paint);

        float y = Edge == Edge.Bottom
            ? panelRect.Bottom - PaddingOutsideAxisLabels.Vertical
            : panelRect.Top + PaddingOutsideAxisLabels.Vertical;

        Pixel labelPoint = new(panelRect.HorizontalCenter, y);

        if (ShowDebugInformation)
        {
            Drawing.DrawDebugRectangle(rp.Canvas, rp.Paint, panelRect, labelPoint, LabelFontColor);
        }

        LabelAlignment = Alignment.LowerCenter;

        rp.CanvasState.Save();

        if (ClipLabel)
        {
            rp.CanvasState.Clip(panelRect);
        }

        LabelStyle.Render(rp.Canvas, labelPoint, rp.Paint);

        rp.CanvasState.Restore();

        DrawTicks(rp, TickLabelStyle, panelRect, TickGenerator.Ticks, this, MajorTickStyle, MinorTickStyle);
        DrawFrame(rp, panelRect, Edge, FrameLineStyle);
    }

    /// <summary>Converts a coordinate distance to a pixel distance.</summary>
    /// <param name="coordinateDistance">The distance in coordinate units.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The distance in pixels.</returns>
    public double GetPixelDistance(double coordinateDistance, PixelRect dataArea) => coordinateDistance * dataArea.Width / Width;

    /// <summary>Converts a pixel distance to a coordinate distance.</summary>
    /// <param name="pixelDistance">The distance in pixels.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The distance in coordinate units.</returns>
    public double GetCoordinateDistance(float pixelDistance, PixelRect dataArea) => pixelDistance / (dataArea.Width / Width);

    /// <summary>Regenerates tick positions and labels for the specified axis size.</summary>
    /// <param name="size">The size available for the axis ticks.</param>
    /// <param name="paint">The paint used to measure tick labels.</param>
    public void RegenerateTicks(PixelLength size, Paint paint)
    {
        TickLabelStyle.ApplyToPaint(paint);
        TickGenerator.Regenerate(Range.ToCoordinateRange, Edge, size, paint, TickLabelStyle);
    }

    /// <summary>Gets the panel rectangle for an axis on the bottom side.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The axis panel height.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <returns>The bottom axis panel rectangle.</returns>
    private PixelRect GetPanelRectangleBottom(PixelRect dataRect, float size, float offset) => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Bottom + offset + size,
            top: dataRect.Bottom + offset);

    /// <summary>Gets the panel rectangle for an axis on the top side.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The axis panel height.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <returns>The top axis panel rectangle.</returns>
    private PixelRect GetPanelRectangleTop(PixelRect dataRect, float size, float offset) => new(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Top - offset,
            top: dataRect.Top - offset - size);
}
