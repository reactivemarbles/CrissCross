// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.StarAxes;

/// <summary>Renders a polygon connecting the ticks around a spoked star axis.</summary>
public class PolygonalStarAxis : SpokedStarAxis
{
    /// <summary>The multiplier used to convert a half-turn into a full turn.</summary>
    private const double FullTurnMultiplier = 2.0;

    /// <summary>The divisor used to find the midpoint of each spoke interval.</summary>
    private const double HalfIntervalDivisor = 2.0;

    /// <summary>Gets or sets the line style used to draw the polygon.</summary>
    public override LineStyle LineStyle { get; set; } = new() { Color = Colors.DarkGray };

    /// <summary>Renders the polygonal star axis and its spokes.</summary>
    /// <param name="rp">The render pack used to draw the axis.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="maxSpokeLength">The maximum spoke length as a fraction of the available radius.</param>
    /// <param name="numSpokes">The number of spokes in the axis.</param>
    /// <param name="rotationDegrees">The rotation of the axis in degrees.</param>
    public override void Render(RenderPack rp, IAxes axes, double maxSpokeLength, int numSpokes, float rotationDegrees = 0)
    {
        var ticks = new float[] { 0.25F, 0.5F, 1 };
        Pixel origin = axes.GetPixel(Coordinates.Origin);

        float minX = Math.Abs(axes.GetPixelX(1) - origin.X);
        float minY = Math.Abs(axes.GetPixelY(1) - origin.Y);
        var maxRadius = Math.Min(minX, minY) * maxSpokeLength;

        RenderSpokes(rp, axes, maxRadius, numSpokes, rotationDegrees);

        using SKAutoCanvasRestore _ = new(rp.Canvas);
        rp.Canvas.Translate(origin.X, origin.Y);
        rp.Canvas.RotateDegrees(rotationDegrees);

        var sweepAngle = (FullTurnMultiplier * Math.PI) / numSpokes;
        foreach (var tick in ticks)
        {
            double cumRotation = 0;
            using SKPathBuilder builder = new();
            for (int i = 0; i < numSpokes; i++)
            {
                var theta = cumRotation + (sweepAngle / HalfIntervalDivisor);
                var x = (float)(tick * maxRadius * Math.Cos(theta));
                var y = (float)(tick * maxRadius * Math.Sin(theta));
                if (i == 0)
                {
                    builder.MoveTo(x, y);
                }
                else
                {
                    builder.LineTo(x, y);
                }

                cumRotation += sweepAngle;
            }

            builder.Close();

            using SKPath path = builder.Detach();
            Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);
        }
    }
}
