// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.ArrowShapes;

/// <summary>Draws a filled double-headed arrow along a line.</summary>
public class Double : IArrowShape
{
    /// <summary>The divisor used to calculate half of an arrow width.</summary>
    private const float HalfDivisor = 2F;

    /// <summary>The rotation applied to align the arrowhead with its line.</summary>
    private const float QuarterTurnDegrees = 90F;

    /// <summary>Renders this arrow shape for the specified line and style.</summary>
    /// <param name="rp">The render pack providing the canvas and paint.</param>
    /// <param name="pixelLine">The line that determines the arrow position and direction.</param>
    /// <param name="arrowStyle">The dimensions and drawing styles for the arrow.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1318", Justification = "Retained parameter name for compatibility with existing named-argument callers.")]
    public void Render(RenderPack rp, PixelLine pixelLine, ArrowStyle arrowStyle)
    {
        float length = pixelLine.Length;

        rp.CanvasState.Save();
        rp.CanvasState.Translate(pixelLine.Pixel2);
        rp.CanvasState.RotateDegrees(pixelLine.AngleDegrees + QuarterTurnDegrees);

        // origin is the tip, base extends to the right
        Pixel[] pixels = [
            new(0, 0),
            new(arrowStyle.ArrowheadLength, arrowStyle.ArrowheadWidth / HalfDivisor),
            new(arrowStyle.ArrowheadAxisLength, arrowStyle.ArrowWidth / HalfDivisor),
            new(length - arrowStyle.ArrowheadAxisLength, arrowStyle.ArrowWidth / HalfDivisor),
            new(length - arrowStyle.ArrowheadLength, arrowStyle.ArrowheadWidth / HalfDivisor),
            new(length, 0),
            new(length - arrowStyle.ArrowheadLength, -arrowStyle.ArrowheadWidth / HalfDivisor),
            new(length - arrowStyle.ArrowheadAxisLength, -arrowStyle.ArrowWidth / HalfDivisor),
            new(arrowStyle.ArrowheadAxisLength, -arrowStyle.ArrowWidth / HalfDivisor),
            new(arrowStyle.ArrowheadLength, -arrowStyle.ArrowheadWidth / HalfDivisor),
            new(0, 0),
        ];

        Drawing.FillPath(rp.Canvas, rp.Paint, pixels, arrowStyle.FillStyle);
        Drawing.DrawPath(rp.Canvas, rp.Paint, pixels, arrowStyle.LineStyle);

        rp.CanvasState.Restore();
    }
}
