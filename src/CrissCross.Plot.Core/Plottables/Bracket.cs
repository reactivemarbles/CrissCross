// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Numerics;

namespace CrissCross.Plot.Plottables;

/// <summary>Draws a bracket between two coordinates with an optional label.</summary>
public class Bracket : IPlottable
{
    /// <summary>Gets or sets the pixel size of the bracket edges and label stub.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public float EdgeLength = 5;

    /// <summary>The fraction used to find the midpoint of the bracket base.</summary>
    private const float Half = 0.5F;

    /// <summary>The number of degrees in a half-turn.</summary>
    private const float HalfTurnDegrees = 180;

    /// <summary>The number of degrees in a full turn.</summary>
    private const float FullTurnDegrees = 360;

    /// <summary>The start angle of the upside-down text range.</summary>
    private const float UprightRangeStartDegrees = 90;

    /// <summary>The end angle of the upside-down text range.</summary>
    private const float UprightRangeEndDegrees = 270;

    /// <summary>Gets or sets whether this bracket is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this bracket.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this bracket.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the first coordinate of the bracket base.</summary>
    public Coordinates Point1 { get; set; }

    /// <summary>Gets or sets the second coordinate of the bracket base.</summary>
    public Coordinates Point2 { get; set; }

    /// <summary>Gets or sets whether the bracket edge extends counter-clockwise from its base.</summary>
    public bool LabelCounterClockwise { get; set; }

    /// <summary>Gets or sets the text displayed near the center of the bracket.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the style used to draw the bracket lines.</summary>
    public LineStyle LineStyle { get; set; } = new() { IsVisible = true, Width = 1 };

    /// <summary>Gets or sets the style used to draw the bracket label.</summary>
    public LabelStyle LabelStyle { get; set; } = new() { IsVisible = true, Alignment = Alignment.LowerCenter };

    /// <summary>Gets the axis limits occupied by the bracket.</summary>
    /// <returns>The axis limits containing both bracket coordinates.</returns>
    public AxisLimits GetAxisLimits() => new([Point1, Point2]);

    /// <summary>Renders the bracket and its optional label.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        double x1 = Point1.X;
        double y1 = Point1.Y;
        double x2 = Point2.X;
        double y2 = Point2.Y;
        double pixelsPerUnitX = rp.DataRect.Width / Axes.XAxis.Width;
        double pixelsPerUnitY = rp.DataRect.Height / Axes.YAxis.Height;

        Vector2 coordinateVector = new((float)(x2 - x1), (float)(y2 - y1));
        Vector2 pixelVector = new((float)(coordinateVector.X * pixelsPerUnitX), (float)(coordinateVector.Y * pixelsPerUnitY));
        (Vector2 clockwiseNormal, Vector2 counterClockwiseNormal, Vector2 antiNormal) = GetEdgeVectors(pixelVector, coordinateVector);
        Vector2 edgeVector = LabelCounterClockwise ? counterClockwiseNormal : clockwiseNormal;

        Vector2 globalTranslation = edgeVector * EdgeLength;
        rp.CanvasState.Translate(new(globalTranslation.X, globalTranslation.Y));

        Pixel start1 = Axes.GetPixel(new(x1, y1));
        Pixel end1 = Axes.GetPixel(new(x2, y2));
        Vector2 bracketHeadTranslation = edgeVector * EdgeLength;
        Pixel start2 = start1.WithOffset(bracketHeadTranslation.X, bracketHeadTranslation.Y);
        Pixel end2 = end1.WithOffset(bracketHeadTranslation.X, bracketHeadTranslation.Y);
        PixelLine mainLine = new(start1.X, start1.Y, start2.X, start2.Y);
        PixelLine edgeLine1 = new(end1.X, end1.Y, end2.X, end2.Y);
        PixelLine edgeLine2 = new(start2.X, start2.Y, end2.X, end2.Y);

        Drawing.DrawLine(rp.Canvas, rp.Paint, mainLine, LineStyle);
        Drawing.DrawLine(rp.Canvas, rp.Paint, edgeLine1, LineStyle);
        Drawing.DrawLine(rp.Canvas, rp.Paint, edgeLine2, LineStyle);

        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        RenderLabel(rp, coordinateVector, edgeVector, antiNormal, pixelsPerUnitX, pixelsPerUnitY, bracketHeadTranslation);
    }

    /// <summary>Calculates the two possible edge directions for the bracket.</summary>
    /// <param name="pixelVector">The bracket base vector in pixels.</param>
    /// <param name="coordinateVector">The bracket base vector in coordinates.</param>
    /// <returns>The clockwise and counter-clockwise directions and the anti-normal vector.</returns>
    private static (Vector2 ClockwiseNormal, Vector2 CounterClockwiseNormal, Vector2 AntiNormal) GetEdgeVectors(Vector2 pixelVector, Vector2 coordinateVector)
    {
        Vector2 direction = Vector2.Normalize(pixelVector);
        if (coordinateVector.X < 0 || (coordinateVector.X == 0 && coordinateVector.Y < 0))
        {
            direction = Vector2.Negate(direction);
        }

        Vector2 normal = Vector2.Normalize(new(pixelVector.Y, pixelVector.X));
        Vector2 antiNormal = Vector2.Negate(normal);
        Vector2 clockwiseNormal = AngleBetweenVectors(direction, normal) > 0 ? normal : antiNormal;
        Vector2 counterClockwiseNormal = normal == clockwiseNormal ? antiNormal : normal;
        return (clockwiseNormal, counterClockwiseNormal, antiNormal);
    }

    /// <summary>Calculates the angle between two normalized vectors.</summary>
    /// <param name="reference">The reference vector.</param>
    /// <param name="vector">The vector to compare.</param>
    /// <returns>The angle between the vectors in radians.</returns>
    private static double AngleBetweenVectors(Vector2 reference, Vector2 vector)
    {
        reference = Vector2.Normalize(reference);
        vector = Vector2.Normalize(vector);
        return Math.Acos(Vector2.Dot(reference, vector));
    }

    /// <summary>Renders the label and its connecting stub.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="coordinateVector">The bracket base vector in coordinates.</param>
    /// <param name="edgeVector">The selected bracket edge vector.</param>
    /// <param name="antiNormal">The anti-normal vector for the bracket base.</param>
    /// <param name="pixelsPerUnitX">The horizontal pixel scale.</param>
    /// <param name="pixelsPerUnitY">The vertical pixel scale.</param>
    /// <param name="bracketHeadTranslation">The pixel offset of the bracket edge.</param>
    private void RenderLabel(
        RenderPack rp,
        Vector2 coordinateVector,
        Vector2 edgeVector,
        Vector2 antiNormal,
        double pixelsPerUnitX,
        double pixelsPerUnitY,
        Vector2 bracketHeadTranslation)
    {
        Vector2 halfVector = new((float)Point1.X, (float)Point1.Y);
        halfVector += Half * coordinateVector;
        Pixel stubPixel1 = Axes.GetPixel(new(halfVector.X, halfVector.Y)).WithOffset(bracketHeadTranslation.X, bracketHeadTranslation.Y);
        Pixel stubPixel2 = stubPixel1.WithOffset(bracketHeadTranslation.X, bracketHeadTranslation.Y);
        PixelLine stubLine = new(stubPixel1, stubPixel2);

        Drawing.DrawLine(rp.Canvas, rp.Paint, stubLine, LineStyle);
        rp.CanvasState.Translate(stubPixel2);

        float angle = (float)(-Math.Atan2(coordinateVector.Y * pixelsPerUnitY, coordinateVector.X * pixelsPerUnitX) * HalfTurnDegrees / Math.PI);
        if (angle < 0)
        {
            angle += FullTurnDegrees;
        }

        bool flippedText = angle is > UprightRangeStartDegrees and < UprightRangeEndDegrees;
        if (flippedText)
        {
            angle -= HalfTurnDegrees;
        }

        rp.CanvasState.RotateDegrees(angle);

        bool isInverted = edgeVector == antiNormal;
        float labelHeight = LabelStyle.Measure(Text, rp.Paint).Height;
        float translateY = labelHeight * ((isInverted ^ flippedText) ? 0 : 1);
        rp.CanvasState.Translate(new(0, translateY));
        LabelStyle.Render(rp.Canvas, Pixel.Zero, rp.Paint, Text);
    }
}
