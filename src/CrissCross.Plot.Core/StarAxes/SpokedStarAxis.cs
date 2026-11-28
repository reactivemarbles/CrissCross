// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.StarAxes;

/// <summary>Provides shared rendering behavior for star axes with radial spokes.</summary>
public abstract class SpokedStarAxis : IStarAxis
{
    /// <summary>The multiplier used to convert a full turn to radians.</summary>
    private const int FullTurnMultiplier = 2;

    /// <summary>Gets or sets the line style used to render the spokes.</summary>
    public abstract LineStyle LineStyle { get; set; }

    /// <summary>Renders this star axis using the specified number of spokes.</summary>
    /// <param name="rp">The render pack for the current render.</param>
    /// <param name="axes">The axes used to transform coordinates.</param>
    /// <param name="maxSpokeLength">The maximum spoke length.</param>
    /// <param name="numSpokes">The number of spokes to render.</param>
    /// <param name="rotationDegrees">The rotation applied to the axis.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional rotation default is retained for source and binary compatibility.")]
    public abstract void Render(RenderPack rp, IAxes axes, double maxSpokeLength, int numSpokes, float rotationDegrees = 0);

    /// <summary>Renders equally spaced spokes from the origin.</summary>
    /// <param name="rp">The render pack for the current render.</param>
    /// <param name="axes">The axes used to transform the origin.</param>
    /// <param name="spokeLength">The length of each spoke.</param>
    /// <param name="numSpokes">The number of spokes to render.</param>
    /// <param name="rotationDegrees">The rotation applied to the spokes.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional rotation default is retained for source and binary compatibility.")]
    public virtual void RenderSpokes(RenderPack rp, IAxes axes, double spokeLength, int numSpokes, float rotationDegrees = 0)
    {
        var sweepAngle = FullTurnMultiplier * Math.PI / numSpokes;
        Pixel origin = axes.GetPixel(Coordinates.Origin);

        using SKAutoCanvasRestore _ = new(rp.Canvas);
        rp.Canvas.Translate(origin.X, origin.Y);
        rp.Canvas.RotateDegrees(rotationDegrees);

        for (int i = 0; i < numSpokes; i++)
        {
            var theta = (i * sweepAngle) + (sweepAngle / FullTurnMultiplier);
            var x = (float)(spokeLength * Math.Cos(theta));
            var y = (float)(spokeLength * Math.Sin(theta));
            PixelLine line = new(0, 0, x, y);
            Drawing.DrawLine(rp.Canvas, rp.Paint, line, LineStyle);
        }
    }
}
