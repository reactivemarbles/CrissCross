// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.ArrowShapes;

/// <summary>Draws an open, two-stroke arrowhead at the end of a line.</summary>
public class ArrowheadLine : IArrowShape
{
    /// <summary>The divisor used to calculate half of the arrowhead width.</summary>
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
        rp.CanvasState.Save();
        rp.CanvasState.Translate(pixelLine.Pixel2);
        rp.CanvasState.RotateDegrees(pixelLine.AngleDegrees + QuarterTurnDegrees);

        // origin is the tip, base extends to the right
        PixelLine line1 = new(new(0, 0), new(arrowStyle.ArrowheadLength, arrowStyle.ArrowheadWidth / HalfDivisor));
        PixelLine line2 = new(new(0, 0), new(arrowStyle.ArrowheadLength, -arrowStyle.ArrowheadWidth / HalfDivisor));

        Drawing.DrawLine(rp.Canvas, rp.Paint, line1, arrowStyle.LineStyle);
        Drawing.DrawLine(rp.Canvas, rp.Paint, line2, arrowStyle.LineStyle);

        rp.CanvasState.Restore();
    }
}
