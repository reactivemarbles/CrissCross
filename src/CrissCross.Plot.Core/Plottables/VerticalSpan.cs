// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A vertical span marks the full horizontal range between two vertical values.</summary>
public class VerticalSpan : AxisSpan
{
    /// <summary>Gets or sets the first vertical coordinate of the span.</summary>
    public double Y1 { get; set; }

    /// <summary>Gets or sets the second vertical coordinate of the span.</summary>
    public double Y2 { get; set; }

    /// <summary>Gets the normalized vertical range of the span.</summary>
    public CoordinateRange YRange => new(Bottom, Top);

    /// <summary>Gets or sets the lower vertical coordinate of the span.</summary>
    public double Bottom
    {
        get => Math.Min(Y1, Y2);
        set => Y1 = value;
    }

    /// <summary>Gets or sets the upper vertical coordinate of the span.</summary>
    public double Top
    {
        get => Math.Max(Y1, Y2);
        set => Y2 = value;
    }

    /// <summary>Gets the axis limits contributed by this span.</summary>
    /// <returns>The vertical limits when autoscaling is enabled; otherwise no limits.</returns>
    public override AxisLimits GetAxisLimits() => EnableAutoscale ? AxisLimits.VerticalOnly(Bottom, Top) : AxisLimits.NoLimits;

    /// <summary>Renders this vertical span.</summary>
    /// <param name="rp">The render context.</param>
    public override void Render(RenderPack rp)
    {
        PixelRangeY vert = new(Axes.GetPixelY(Bottom), Axes.GetPixelY(Top));
        if (vert.Span < 1)
        {
            float middle = (vert.Top + vert.Bottom) / Constants.HalfRangeDivisor;
            vert = new(middle - Constants.HalfPixel, middle + Constants.HalfPixel);
        }

        PixelRangeX horiz = new(rp.DataRect.Left, rp.DataRect.Right);
        PixelRect rect = new(horiz, vert);
        Render(rp, rect);
    }

    /// <summary>Determines whether the span is under the specified coordinate rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The span interaction data when the span is hit; otherwise null.</returns>
    public override AxisSpanUnderMouse? UnderMouse(CoordinateRect rect)
    {
        AxisSpanUnderMouse spanUnderMouse = new() { Span = this, MouseStart = rect.Center, OriginalRange = new(Y1, Y2), };

        if (IsResizable && rect.ContainsY(Y1))
        {
            spanUnderMouse.ResizeEdge1 = true;
            return spanUnderMouse;
        }

        if (IsResizable && rect.ContainsY(Y2))
        {
            spanUnderMouse.ResizeEdge2 = true;
            return spanUnderMouse;
        }

        return IsDraggable && rect.YRange.Overlaps(YRange) ? spanUnderMouse : null;
    }

    /// <summary>Moves or resizes the span in response to a drag operation.</summary>
    /// <param name="start">The span interaction data captured when dragging began.</param>
    /// <param name="mouseNow">The current pointer location.</param>
    public override void DragTo(AxisSpanUnderMouse start, Coordinates mouseNow)
    {
        if (start.ResizeEdge1)
        {
            Y1 = mouseNow.Y;
        }
        else if (start.ResizeEdge2)
        {
            Y2 = mouseNow.Y;
        }
        else
        {
            double verticalDelta = start.MouseStart.Y - mouseNow.Y;
            Y1 = start.OriginalRange.Min - verticalDelta;
            Y2 = start.OriginalRange.Max - verticalDelta;
        }
    }

    /// <summary>Contains constants used by vertical spans.</summary>
    internal static class Constants
    {
        /// <summary>The divisor used to calculate half of a vertical range.</summary>
        internal const float HalfRangeDivisor = 2;

        /// <summary>The minimum half-height of a one-pixel span.</summary>
        internal const float HalfPixel = 0.5F;
    }
}
