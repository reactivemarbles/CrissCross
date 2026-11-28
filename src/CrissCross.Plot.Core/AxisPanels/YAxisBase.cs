// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisPanels;

/// <summary>Provides shared behavior for vertical axes.</summary>
public abstract class YAxisBase : AxisBase, IYAxis
{
    /// <summary>Gets the coordinate span displayed by the axis.</summary>
    public double Height => Range.Span;

    /// <summary>Converts a coordinate position to a pixel position.</summary>
    /// <param name="position">The coordinate position.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The corresponding vertical pixel position.</returns>
    public float GetPixel(double position, PixelRect dataArea)
    {
        double pixelsPerUnit = dataArea.Height / Height;
        double unitsFromMinimum = position - Min;
        float pixelsFromEdge = (float)(unitsFromMinimum * pixelsPerUnit);
        return dataArea.Bottom - pixelsFromEdge;
    }

    /// <summary>Converts a pixel position to a coordinate position.</summary>
    /// <param name="pixel">The vertical pixel position.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The corresponding coordinate position.</returns>
    public double GetCoordinate(float pixel, PixelRect dataArea)
    {
        double pixelsPerUnit = dataArea.Height / Height;
        float pixelsFromMinimum = pixel - dataArea.Bottom;
        double unitsFromMinimum = pixelsFromMinimum / pixelsPerUnit;
        return Min - unitsFromMinimum;
    }

    /// <summary>Measures the space required to render the axis.</summary>
    /// <param name="paint">The paint used to measure labels.</param>
    /// <returns>The required width in pixels.</returns>
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

        float maxTickLabelWidth = 0;
        foreach (Tick tick in TickGenerator.Ticks)
        {
            float tickLabelWidth = TickLabelStyle.Measure(tick.Label, paint).Width;
            maxTickLabelWidth = Math.Max(maxTickLabelWidth, tickLabelWidth);
        }

        float axisLabelHeight = string.IsNullOrEmpty(LabelStyle.Text) && LabelStyle.Image is null
            ? EmptyLabelPadding.Horizontal
            : LabelStyle.Measure(LabelText, paint).Height
                + PaddingBetweenTickAndAxisLabels.Horizontal
                + PaddingOutsideAxisLabels.Horizontal;

        return maxTickLabelWidth + axisLabelHeight;
    }

    /// <summary>Gets the rectangle occupied by the axis panel.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The axis panel width.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <param name="paint">The paint used to render the axis.</param>
    /// <returns>The axis panel rectangle.</returns>
    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset, Paint paint) => Edge == Edge.Left
        ? GetPanelRectangleLeft(dataRect, size, offset)
        : GetPanelRectangleRight(dataRect, size, offset);

    /// <summary>Renders the vertical axis and its labels.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="size">The axis panel width.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    public virtual void Render(RenderPack rp, float size, float offset)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelRect panelRect = GetPanelRect(rp.DataRect, size, offset, rp.Paint);
        float x = Edge == Edge.Left
            ? panelRect.Left + PaddingOutsideAxisLabels.Horizontal
            : panelRect.Right - PaddingOutsideAxisLabels.Horizontal;
        Pixel labelPoint = new(x, rp.DataRect.VerticalCenter);

        if (ShowDebugInformation)
        {
            Drawing.DrawDebugRectangle(rp.Canvas, rp.Paint, panelRect, labelPoint, LabelFontColor);
        }

        LabelAlignment = Alignment.UpperCenter;
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
    public double GetPixelDistance(double coordinateDistance, PixelRect dataArea) => coordinateDistance * dataArea.Height / Height;

    /// <summary>Converts a pixel distance to a coordinate distance.</summary>
    /// <param name="pixelDistance">The distance in pixels.</param>
    /// <param name="dataArea">The data rectangle where the coordinate is rendered.</param>
    /// <returns>The distance in coordinate units.</returns>
    public double GetCoordinateDistance(float pixelDistance, PixelRect dataArea) => pixelDistance / (dataArea.Height / Height);

    /// <summary>Regenerates tick positions and labels for the specified axis size.</summary>
    /// <param name="size">The size available for the axis ticks.</param>
    /// <param name="paint">The paint used to measure tick labels.</param>
    public void RegenerateTicks(PixelLength size, Paint paint)
    {
        TickLabelStyle.ApplyToPaint(paint);
        TickGenerator.Regenerate(Range.ToCoordinateRange, Edge, size, paint, TickLabelStyle);
    }

    /// <summary>Gets the panel rectangle for an axis on the left side.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The axis panel width.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <returns>The left axis panel rectangle.</returns>
    private PixelRect GetPanelRectangleLeft(PixelRect dataRect, float size, float offset) => new(
            left: dataRect.Left - offset - size,
            right: dataRect.Left - offset,
            bottom: dataRect.Bottom,
            top: dataRect.Top);

    /// <summary>Gets the panel rectangle for an axis on the right side.</summary>
    /// <param name="dataRect">The plot data rectangle.</param>
    /// <param name="size">The axis panel width.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    /// <returns>The right axis panel rectangle.</returns>
    private PixelRect GetPanelRectangleRight(PixelRect dataRect, float size, float offset) => new(
            left: dataRect.Right + offset,
            right: dataRect.Right + offset + size,
            bottom: dataRect.Bottom,
            top: dataRect.Top);
}
