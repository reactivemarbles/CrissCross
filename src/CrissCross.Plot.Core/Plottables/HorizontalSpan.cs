// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A horizontal span marks the full vertical range between two horizontal values.</summary>
public class HorizontalSpan : AxisSpan
{
    /// <summary>Gets or sets the first x-coordinate of the span.</summary>
    public double X1 { get; set; }

    /// <summary>Gets or sets the second x-coordinate of the span.</summary>
    public double X2 { get; set; }

    /// <summary>Gets the normalized x-coordinate range of the span.</summary>
    public CoordinateRange XRange => new(Left, Right);

    /// <summary>Gets or sets the leftmost x-coordinate of the span.</summary>
    public double Left
    {
        get => Math.Min(X1, X2);
        set => X1 = value;
    }

    /// <summary>Gets or sets the rightmost x-coordinate of the span.</summary>
    public double Right
    {
        get => Math.Max(X1, X2);
        set => X2 = value;
    }

    /// <summary>Gets the horizontal axis limits required by this span.</summary>
    /// <returns>The span's horizontal limits, or no limits when autoscaling is disabled.</returns>
    public override AxisLimits GetAxisLimits() => EnableAutoscale ? AxisLimits.HorizontalOnly(Left, Right) : AxisLimits.NoLimits;

    /// <summary>Renders the horizontal span.</summary>
    /// <param name="rp">The render context.</param>
    public override void Render(RenderPack rp)
    {
        PixelRangeY vert = new(rp.DataRect.Bottom, rp.DataRect.Top);
        PixelRangeX horiz = new(Axes.GetPixelX(Left), Axes.GetPixelX(Right));
        if (horiz.Span < Constants.MinimumPixelSpan)
        {
            float middle = (horiz.Left + horiz.Right) / Constants.HalfPixelDivisor;
            horiz = new(middle - Constants.HalfPixelSpan, middle + Constants.HalfPixelSpan);
        }

        PixelRect rect = new(horiz, vert);
        Render(rp, rect);
    }

    /// <summary>Determines whether the mouse is over this span or one of its resize edges.</summary>
    /// <param name="rect">The mouse location and hit-test region.</param>
    /// <returns>The span interaction state when hit; otherwise <see langword="null"/>.</returns>
    public override AxisSpanUnderMouse? UnderMouse(CoordinateRect rect)
    {
        AxisSpanUnderMouse spanUnderMouse = new() { Span = this, MouseStart = rect.Center, OriginalRange = new(X1, X2), };

        if (IsResizable && rect.ContainsX(X1))
        {
            spanUnderMouse.ResizeEdge1 = true;
            return spanUnderMouse;
        }

        if (IsResizable && rect.ContainsX(X2))
        {
            spanUnderMouse.ResizeEdge2 = true;
            return spanUnderMouse;
        }

        return IsDraggable && rect.XRange.Overlaps(XRange) ? spanUnderMouse : null;
    }

    /// <summary>Moves or resizes the span based on the mouse interaction.</summary>
    /// <param name="start">The span state captured at the start of the interaction.</param>
    /// <param name="mouseNow">The current mouse coordinates.</param>
    public override void DragTo(AxisSpanUnderMouse start, Coordinates mouseNow)
    {
        if (start.ResizeEdge1)
        {
            X1 = mouseNow.X;
        }
        else if (start.ResizeEdge2)
        {
            X2 = mouseNow.X;
        }
        else
        {
            double deltaX = start.MouseStart.X - mouseNow.X;
            X1 = start.OriginalRange.Min - deltaX;
            X2 = start.OriginalRange.Max - deltaX;
        }
    }

    /// <summary>Contains pixel geometry constants used to render the span.</summary>
    private static class Constants
    {
        /// <summary>The minimum horizontal span in pixels.</summary>
        public const float MinimumPixelSpan = 1;

        /// <summary>The divisor used to calculate the center of the pixel span.</summary>
        public const float HalfPixelDivisor = 2;

        /// <summary>The minimum extent on either side of the span center.</summary>
        public const float HalfPixelSpan = 0.5F;
    }
}
