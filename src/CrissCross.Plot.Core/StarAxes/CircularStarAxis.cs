// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.StarAxes;

/// <summary>Renders a star axis with concentric circular guides.</summary>
public class CircularStarAxis : SpokedStarAxis
{
    /// <summary>Gets or sets the style used to render the circles and spokes.</summary>
    public override LineStyle LineStyle { get; set; } = new() { Color = Colors.DarkGray, Width = 1, };

    /// <inheritdoc/>
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
        rp.Canvas.RotateDegrees(rotationDegrees); // Won't matter for the circles, but will if and when we add spokes

        foreach (var tick in ticks)
        {
            Drawing.DrawCircle(rp.Canvas, Pixel.Zero, (float)(tick * maxRadius), LineStyle, rp.Paint);
        }
    }
}
