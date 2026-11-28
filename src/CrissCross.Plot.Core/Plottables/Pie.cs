// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders data as a pie or donut chart.</summary>
public class Pie : PieBase
{
    /// <summary>Initializes a new instance of the <see cref="Pie"/> class with the specified slices.</summary>
    /// <param name="slices">The slices displayed by the plot.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2241", Justification = "Retained as an explicit public constructor for binary compatibility.")]
    public Pie(IList<PieSlice> slices)
    {
        Slices = slices;
    }

    /// <summary>Gets or sets the radius of the pie.</summary>
    public double Radius { get; set; } = 1.0;

    /// <summary>Gets or sets how far slices are offset from the center.</summary>
    public double ExplodeFraction { get; set; }

    /// <summary>Gets or sets the fraction of the pie removed to create a donut hole.</summary>
    public double DonutFraction { get; set; }

    /// <summary>Gets the axis limits occupied by this plot.</summary>
    /// <returns>The axis limits of the pie and its labels.</returns>
    public override AxisLimits GetAxisLimits()
    {
        double radius = Math.Max(SliceLabelDistance, Radius + ExplodeFraction) + Padding;
        return new(-radius, radius, -radius, radius);
    }

    /// <summary>Renders the pie or donut slices.</summary>
    /// <param name="rp">The render context.</param>
    public override void Render(RenderPack rp)
    {
        Pixel origin = Axes.GetPixel(Coordinates.Origin);
        float minX = Math.Abs(Axes.GetPixelX(Radius) - origin.X);
        float minY = Math.Abs(Axes.GetPixelY(Radius) - origin.Y);
        float outerRadius = Math.Min(minX, minY);
        double totalValue = 0;
        foreach (PieSlice slice in Slices)
        {
            totalValue += slice.Value;
        }

        Angle totalAngle = Rotation;
        foreach (PieSlice slice in Slices)
        {
            using SKAutoCanvasRestore _ = new(rp.Canvas);
            Angle sliceAngle = Angle.FromFraction(slice.Value / totalValue);
            Angle centerAngle = totalAngle + (sliceAngle / PieConstants.HalfCircleDivisor);
            PolarCoordinates slicePolar = new(ExplodeFraction * outerRadius, centerAngle);
            Coordinates explosionOffset = slicePolar.ToCartesian();
            rp.Canvas.Translate((float)(origin.X + explosionOffset.X), (float)(origin.Y + explosionOffset.Y));

            if (DonutFraction > 0)
            {
                RenderDonutSlice(rp, rp.Paint, LineStyle, slice.Fill, outerRadius, (float)sliceAngle.Degrees, (float)totalAngle.Degrees);
            }
            else
            {
                RenderSlice(rp, rp.Paint, LineStyle, slice.Fill, outerRadius, (float)sliceAngle.Degrees, (float)totalAngle.Degrees);
            }

            Coordinates textPolar = slicePolar.WithRadius(Radius * SliceLabelDistance).ToCartesian();
            textPolar.Y = -textPolar.Y;
            Pixel labelPixel = Axes.GetPixel(textPolar) - origin;
            slice.LabelStyle.Render(rp.Canvas, labelPixel, rp.Paint);
            totalAngle += sliceAngle;
        }
    }

    /// <summary>Renders a donut-shaped slice.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="lineStyle">The outline style.</param>
    /// <param name="fillStyle">The slice fill style.</param>
    /// <param name="radius">The outer radius in pixels.</param>
    /// <param name="sliceAngle">The slice angle in degrees.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    protected virtual void RenderDonutSlice(RenderPack rp, Paint paint, LineStyle lineStyle, FillStyle fillStyle, float radius, float sliceAngle, float startAngle)
    {
        PixelRect outerRect = new(-radius, radius, -radius, radius);
        float innerRadius = radius * (float)DonutFraction;
        PixelRect innerRect = new(-innerRadius, innerRadius, -innerRadius, innerRadius);
        if (Math.Abs(sliceAngle) < PieConstants.FullCircleDegrees)
        {
            Drawing.FillAnnularSector(rp.Canvas, rp.Paint, fillStyle, outerRect, innerRect, startAngle, sliceAngle);
            Drawing.DrawAnnularSector(rp.Canvas, rp.Paint, lineStyle, outerRect, innerRect, startAngle, sliceAngle);
        }
        else
        {
            Drawing.FillEllipticalAnnulus(rp.Canvas, rp.Paint, fillStyle, outerRect, innerRect);
            Drawing.DrawEllipticalAnnulus(rp.Canvas, rp.Paint, lineStyle, outerRect, innerRect);
        }
    }

    /// <summary>Renders a solid pie slice.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="lineStyle">The outline style.</param>
    /// <param name="fillStyle">The slice fill style.</param>
    /// <param name="radius">The slice radius in pixels.</param>
    /// <param name="sliceAngle">The slice angle in degrees.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    protected virtual void RenderSlice(RenderPack rp, Paint paint, LineStyle lineStyle, FillStyle fillStyle, float radius, float sliceAngle, float startAngle)
    {
        PixelRect outerRect = new(-radius, radius, -radius, radius);
        if (Math.Abs(sliceAngle) < PieConstants.FullCircleDegrees)
        {
            Drawing.FillSector(rp.Canvas, rp.Paint, fillStyle, outerRect, startAngle, sliceAngle);
            Drawing.DrawSector(rp.Canvas, rp.Paint, lineStyle, outerRect, startAngle, sliceAngle);
        }
        else
        {
            Drawing.FillOval(rp.Canvas, rp.Paint, fillStyle, outerRect);
            Drawing.DrawOval(rp.Canvas, rp.Paint, lineStyle, outerRect);
        }
    }

    /// <summary>Contains constants used when rendering pie slices.</summary>
    internal static class PieConstants
    {
        /// <summary>Divisor for converting a full turn into half of a circle.</summary>
        internal const double HalfCircleDivisor = 2;

        /// <summary>Degrees in a full circle.</summary>
        internal const double FullCircleDegrees = 360;
    }
}
