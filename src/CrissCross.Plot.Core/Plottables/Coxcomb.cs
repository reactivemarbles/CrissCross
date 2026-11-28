// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Plottables;

/// <summary>Displays pie slices as radial bars whose lengths represent their values.</summary>
public class Coxcomb : PieBase
{
    /// <summary>Initializes a new instance of the <see cref="Coxcomb"/> class.</summary>
    /// <param name="slices">The slices displayed by the plot.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2241", Justification = "The constructor assigns the inherited Slices property without hiding its public API.")]
    public Coxcomb(IList<PieSlice> slices)
    {
        Slices = slices;
    }

    /// <summary>Gets or sets the axis used to render the radial grid.</summary>
    public IStarAxis StarAxis { get; set; } = new StarAxes.CircularStarAxis();

    /// <summary>Gets the axis limits occupied by the normalized slices.</summary>
    /// <returns>The axis limits containing all slices and labels.</returns>
    public override AxisLimits GetAxisLimits()
    {
        double maxRadius = GetMaximumRadius(GetNormalizedSlices());
        double radius = Math.Max(maxRadius * SliceLabelDistance, maxRadius) + Padding;
        return new(-radius, radius, -radius, radius);
    }

    /// <summary>Renders the coxcomb slices and their labels.</summary>
    /// <param name="rp">The render context.</param>
    public override void Render(RenderPack rp)
    {
        rp.Paint.IsAntialias = true;

        float startAngle = (float)Rotation.Degrees;
        float[] sliceSizes = GetNormalizedSlices();
        double maxRadius = GetMaximumRadius(sliceSizes);
        StarAxis.Render(rp, Axes, maxRadius, Slices.Count, startAngle);

        Pixel origin = Axes.GetPixel(Coordinates.Origin);
        float rotationPerSlice = CoxcombConstants.FullCircleDegrees / Slices.Count;

        using SKAutoCanvasRestore _ = new(rp.Canvas);
        rp.Canvas.Translate(origin.X, origin.Y);
        rp.Canvas.RotateDegrees(startAngle);
        rp.Canvas.RotateDegrees(-rotationPerSlice);

        for (int i = 0; i < Slices.Count; i++)
        {
            rp.Canvas.RotateDegrees(rotationPerSlice);
            SKPoint point = GetRotatedPoint(sliceSizes[i], 0F);

            float minX = Math.Abs(Axes.GetPixelX(sliceSizes[i]) - origin.X);
            float minY = Math.Abs(Axes.GetPixelY(sliceSizes[i]) - origin.Y);
            float radius = Math.Min(minX, minY);
            SKRect rect = new(-radius, -radius, radius, radius);

            using SKPathBuilder builder = new();
            if (Slices.Count != 1)
            {
                builder.MoveTo(point);
                builder.LineTo(point);
                builder.ArcTo(rect, 0, rotationPerSlice, false);
                builder.Close();
            }
            else
            {
                builder.AddOval(rect);
            }

            using SKPath path = builder.Detach();
            Slices[i].Fill.ApplyToPaint(rp.Paint, new(origin, radius));
            SKShader? shader = rp.Paint.SKShader?.WithLocalMatrix(SKMatrix.CreateRotationDegrees((-rotationPerSlice * i) - startAngle));
            Drawing.DrawPath(rp.Canvas, rp.Paint, path, shader);
            Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);

            double cumulativeRotation = i * rotationPerSlice;
            double halfSliceRotation = rotationPerSlice * CoxcombConstants.Half;
            double labelAngle = -(cumulativeRotation + startAngle + halfSliceRotation) * Math.PI / CoxcombConstants.DegreesPerHalfTurn;
            double x = SliceLabelDistance * maxRadius * Math.Cos(labelAngle);
            double y = SliceLabelDistance * maxRadius * Math.Sin(labelAngle);
            Pixel labelPixel = Axes.GetPixel(new(x, y));

            using var textTransform = new SKAutoCanvasRestore(rp.Canvas);
            rp.CanvasState.RotateDegrees((float)-cumulativeRotation - startAngle);
            rp.CanvasState.Translate(-origin.X, -origin.Y);
            Slices[i].LabelStyle.Render(rp.Canvas, labelPixel, rp.Paint);
        }
    }

    /// <summary>Finds the maximum normalized slice radius.</summary>
    /// <param name="normalizedSlices">The normalized slice radii.</param>
    /// <returns>The largest normalized radius.</returns>
    private static float GetMaximumRadius(float[] normalizedSlices)
    {
        if (normalizedSlices.Length == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        float maximumRadius = normalizedSlices[0];
        for (int i = 1; i < normalizedSlices.Length; i++)
        {
            maximumRadius = Math.Max(maximumRadius, normalizedSlices[i]);
        }

        return maximumRadius;
    }

    /// <summary>Creates a Cartesian point at the specified radius and angle.</summary>
    /// <param name="radius">The radius in plot coordinates.</param>
    /// <param name="angleInDegrees">The angle in degrees.</param>
    /// <returns>The point expressed in pixel-like coordinate units.</returns>
    private static SKPoint GetRotatedPoint(double radius, double angleInDegrees)
    {
        Coordinates point = new PolarCoordinates(radius, Angle.FromDegrees(angleInDegrees)).ToCartesian();
        return new((float)point.X, (float)point.Y);
    }

    /// <summary>Creates a normalized radius for each slice.</summary>
    /// <returns>The normalized slice values.</returns>
    private float[] GetNormalizedSlices()
    {
        double total = 0;
        foreach (PieSlice slice in Slices)
        {
            total += slice.Value;
        }

        float[] normalizedSlices = new float[Slices.Count];
        for (int i = 0; i < Slices.Count; i++)
        {
            normalizedSlices[i] = (float)(Slices[i].Value / total);
        }

        return normalizedSlices;
    }

    /// <summary>Stores angular constants used by the coxcomb plot.</summary>
    private static class CoxcombConstants
    {
        /// <summary>The degrees in one full rotation.</summary>
        public const float FullCircleDegrees = 360F;

        /// <summary>The multiplier used to calculate half of a slice's rotation.</summary>
        public const double Half = 0.5;

        /// <summary>The degrees in a half rotation.</summary>
        public const double DegreesPerHalfTurn = 180;
    }
}
