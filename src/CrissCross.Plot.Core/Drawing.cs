// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SkiaSharp.HarfBuzz;

namespace CrissCross.Plot;

// TODO: obsolete methods in this class that create paints. Pass paints in to minimize allocations at render time.
// TODO: obsolete methods that don't take Style objects
/// <summary>Common operations using the default rendering system.</summary>
public static class Drawing
{
    /// <summary>The minimum number of points required for a line path.</summary>
    private const int MinimumLinePointCount = 2;

    /// <summary>The stroke width used to render a one-pixel hairline.</summary>
    private const float HairlineStrokeWidth = 1F;

    /// <summary>The precision scale used when measuring text paths.</summary>
    private const float PathMeasurePrecisionScale = 1F;

    /// <summary>The alpha value used for the translucent debug rectangle.</summary>
    private const byte DebugFillAlpha = 20;

    /// <summary>The radius used for the debug marker circle.</summary>
    private const float DebugMarkerRadius = 5F;

    /// <summary>The maximum value of an RGB or alpha color component.</summary>
    private const float ColorComponentMaximum = 255F;

    /// <summary>The number of degrees in a full circle.</summary>
    private const double FullCircleDegrees = 360D;

    /// <summary>The number of degrees in a half circle.</summary>
    private const double HalfCircleDegrees = 180D;

    /// <summary>The number of degrees in a negative half circle.</summary>
    private const double NegativeHalfCircleDegrees = -180D;

    /// <summary>The divisor used to center text on a path.</summary>
    private const float PathTextCenterDivisor = 4F;

    /// <summary>The strategy used to build straight line paths.</summary>
    private static readonly PathStrategies.Straight StraightLineStrategy = new();

    /// <summary>Draws a line segment, optionally applying the supplied stroke style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixelLine">The line segment endpoints to render.</param>
    public static void DrawLine(SKCanvas canvas, Paint paint, PixelLine pixelLine) => DrawLine(canvas, paint, pixelLine.Pixel1, pixelLine.Pixel2);

    /// <summary>Draws a line segment, optionally applying the supplied stroke style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pt1">The first endpoint of the line segment.</param>
    /// <param name="pt2">The second endpoint of the line segment.</param>
    public static void DrawLine(SKCanvas canvas, Paint paint, Pixel pt1, Pixel pt2) => canvas.DrawLine(pt1.ToSKPoint(), pt2.ToSKPoint(), paint.SKPaint);

    /// <summary>Draws a line segment, optionally applying the supplied stroke style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixelLine">The line segment endpoints to render.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawLine(SKCanvas canvas, Paint paint, PixelLine pixelLine, LineStyle lineStyle) => DrawLine(canvas, paint, pixelLine.Pixel1, pixelLine.Pixel2, lineStyle);

    /// <summary>Draws a line segment, optionally applying the supplied stroke style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pt1">The first endpoint of the line segment.</param>
    /// <param name="pt2">The second endpoint of the line segment.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawLine(SKCanvas canvas, Paint paint, Pixel pt1, Pixel pt2, LineStyle lineStyle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawLine(pt1.ToSKPoint(), pt2.ToSKPoint(), paint.SKPaint);
    }

    /// <summary>Draws a line segment with explicit stroke color, width, antialiasing, and dash pattern.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pt1">The first endpoint of the line segment.</param>
    /// <param name="pt2">The second endpoint of the line segment.</param>
    /// <param name="color">The stroke color assigned to the paint.</param>
    /// <param name="width">The stroke width in pixels.</param>
    /// <param name="antiAlias">true to enable antialiasing; otherwise, false.</param>
    /// <param name="pattern">The dash pattern applied to the stroke.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST1472:Parameter count is limited", Justification = "The signature is preserved for public API compatibility.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309:Optional public API parameters", Justification = "Optional parameters preserve existing caller and binary source compatibility.")]
    public static void DrawLine(SKCanvas canvas, Paint paint, Pixel pt1, Pixel pt2, Color color, float width = 1, bool antiAlias = true, LinePattern pattern = default)
    {
        if (width == 0)
        {
            return;
        }

        paint.Color = color;
        paint.IsStroke = true;
        paint.IsAntialias = antiAlias;
        paint.StrokeWidth = width;
        paint.SKPathEffect = pattern.GetPathEffect();

        canvas.DrawLine(pt1.ToSKPoint(), pt2.ToSKPoint(), paint.SKPaint);
    }

    /// <summary>Draws each supplied line segment with the shared line style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixelLines">The line segments to render.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawLines(SKCanvas canvas, Paint paint, IEnumerable<PixelLine> pixelLines, LineStyle lineStyle)
    {
        foreach (PixelLine line in pixelLines)
        {
            DrawLine(canvas, paint, line.Pixel1, line.Pixel2, lineStyle);
        }
    }

    /// <summary>Connects the ordered points and draws the resulting path.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawLines(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, LineStyle lineStyle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        Pixel[] pixelArray = CopyPixels(pixels);
        if (pixelArray.Length < MinimumLinePointCount)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        using SKPath path = StraightLineStrategy.GetPath(pixelArray);
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Draws the existing path with the supplied line style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawLines(SKCanvas canvas, Paint paint, SKPath path, LineStyle lineStyle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Draws an arrow along the supplied path using its arrow style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="arrowStyle">The arrow stroke and marker configuration.</param>
    public static void DrawArrow(SKCanvas canvas, Paint paint, SKPath path, ArrowStyle arrowStyle)
    {
        arrowStyle.ApplyToPaint(paint);
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Assigns the shader to the paint and draws the existing path.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="shader">The shader assigned to the paint before drawing the path.</param>
    public static void DrawPath(SKCanvas canvas, Paint paint, SKPath path, SKShader? shader)
    {
        paint.SKShader = shader;
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Strokes ordered points, optionally connecting the last point to the first.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="close">true to connect the final point to the first point; otherwise, false.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309:Optional public API parameters", Justification = "The optional close parameter is retained for source compatibility.")]
    public static void DrawPath(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, LineStyle lineStyle, bool close = false)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        Pixel[] pixelArray = CopyPixels(pixels);
        using SKPathBuilder builder = new();
        builder.MoveTo(pixelArray[0].ToSKPoint());
        for (int i = 1; i < pixelArray.Length; i++)
        {
            Pixel px = pixelArray[i];
            builder.LineTo(px.ToSKPoint());
        }

        if (close)
        {
            builder.LineTo(pixelArray[0].ToSKPoint());
        }

        using SKPath path = builder.Detach();
        DrawPath(canvas, paint, path, lineStyle);
    }

    /// <summary>Strokes ordered points and positions a visible label along the path.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="label">The text to position along the path.</param>
    /// <param name="labelStyle">The text style; the label is omitted when it is not visible.</param>
    /// <param name="close">true to connect the final point to the first point; otherwise, false.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309:Optional public API parameters", Justification = "The optional close parameter is retained for source compatibility.")]
    public static void DrawPath(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, LineStyle lineStyle, string label, LabelStyle labelStyle, bool close = false)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        Pixel[] pixelArray = CopyPixels(pixels);
        using SKPathBuilder builder = new();
        builder.MoveTo(pixelArray[0].ToSKPoint());
        for (int i = 1; i < pixelArray.Length; i++)
        {
            Pixel px = pixelArray[i];
            builder.LineTo(px.ToSKPoint());
        }

        if (close)
        {
            builder.LineTo(pixelArray[0].ToSKPoint());
        }

        using SKPath path = builder.Detach();
        DrawPath(canvas, paint, path, lineStyle);

        if (!labelStyle.IsVisible)
        {
            return;
        }

        labelStyle.ApplyToPaint(paint);

        var measuredText = paint.MeasureText(label).Width;
        using SKPathMeasure pathMeasure = new(path, false, PathMeasurePrecisionScale);
        float horizontalOffset = (pathMeasure.Length / PathTextCenterDivisor) - (measuredText / PathTextCenterDivisor);
        const float verticalOffset = 0;
        DrawTextOnPath(canvas, paint, path, label, horizontalOffset, verticalOffset);
    }

    /// <summary>Strokes the supplied path or pixel sequence with the selected line style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawPath(SKCanvas canvas, Paint paint, PixelPath path, LineStyle lineStyle) => DrawPath(canvas, paint, path.Pixels, lineStyle);

    /// <summary>Strokes the supplied path or pixel sequence with the selected line style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="labelStyle">The text style; the label is omitted when it is not visible.</param>
    public static void DrawPath(SKCanvas canvas, Paint paint, PixelPath path, LineStyle lineStyle, string text, LabelStyle labelStyle) =>
        DrawPath(canvas, paint, path.Pixels, lineStyle, text, labelStyle);

    /// <summary>Fills the supplied path or pixel sequence with the selected fill style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    [Obsolete("use FillPath()", false)]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "The obsolete overload is retained for source compatibility.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2318:Equivalent forwarding API", Justification = "The forwarding overload preserves the existing public API.")]
    public static void DrawPath(SKCanvas canvas, Paint paint, PixelPath path, FillStyle fillStyle) => FillPath(canvas, paint, path, fillStyle);

    /// <summary>Fills the supplied path or pixel sequence with the selected fill style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    [Obsolete("use FillPath()", false)]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "The obsolete overload is retained for source compatibility.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2318:Equivalent forwarding API", Justification = "The forwarding overload preserves the existing public API.")]
    public static void DrawPath(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, FillStyle fillStyle) => FillPath(canvas, paint, pixels, fillStyle);

    /// <summary>Strokes the supplied path or pixel sequence with the selected line style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2318:Equivalent line drawing implementation",
        Justification = "The overload preserves the public SKPath API while intentionally sharing line rendering behavior.")]
    public static void DrawPath(SKCanvas canvas, Paint paint, SKPath path, LineStyle lineStyle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Fills the supplied path or pixel sequence with the selected fill style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    [Obsolete("use FillPath()", false)]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "The obsolete overload is retained for source compatibility.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2318:Equivalent forwarding API", Justification = "The forwarding overload preserves the existing public API.")]
    public static void DrawPath(SKCanvas canvas, Paint paint, SKPath path, FillStyle fillStyle, PixelRect rect) => FillPath(canvas, paint, path, fillStyle, rect);

    /// <summary>Fills the path using the supplied rectangle to configure the fill.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    public static void FillPath(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, FillStyle fillStyle, PixelRect rect)
    {
        if (!fillStyle.CanBeRendered)
        {
            return;
        }

        Pixel[] pixelArray = CopyPixels(pixels);
        using SKPathBuilder builder = new();
        builder.MoveTo(pixelArray[0].ToSKPoint());
        for (int i = 1; i < pixelArray.Length; i++)
        {
            Pixel px = pixelArray[i];
            builder.LineTo(px.ToSKPoint());
        }

        using SKPath path = builder.Detach();
        fillStyle.ApplyToPaint(paint, rect);
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Fills an existing path using its bounds to configure the fill.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    public static void FillPath(SKCanvas canvas, Paint paint, SKPath path, FillStyle fillStyle)
    {
        if (!fillStyle.CanBeRendered)
        {
            return;
        }

        fillStyle.ApplyToPaint(paint, path.GetRect().ToPixelRect());
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Fills an existing path using its bounds to configure the fill.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    public static void FillPath(SKCanvas canvas, Paint paint, PixelPath path, FillStyle fillStyle) => FillPath(canvas, paint, path.Pixels, fillStyle);

    /// <summary>Fills the path formed by ordered pixels, deriving its bounds for the fill.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    public static void FillPath(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, FillStyle fillStyle)
    {
        if (!fillStyle.IsVisible || fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        Pixel[] pixelArray = CopyPixels(pixels);
        float minimumX = pixelArray[0].X;
        float maximumX = pixelArray[0].X;
        float minimumY = pixelArray[0].Y;
        float maximumY = pixelArray[0].Y;

        using SKPathBuilder builder = new();
        builder.MoveTo(pixelArray[0].ToSKPoint());
        for (int i = 1; i < pixelArray.Length; i++)
        {
            Pixel px = pixelArray[i];
            builder.LineTo(px.ToSKPoint());
            minimumX = Math.Min(minimumX, px.X);
            maximumX = Math.Max(maximumX, px.X);
            minimumY = Math.Min(minimumY, px.Y);
            maximumY = Math.Max(maximumY, px.Y);
        }

        using SKPath path = builder.Detach();
        PixelRect rect = new(minimumX, maximumX, maximumY, minimumY);
        FillPath(canvas, paint, path, fillStyle, rect);
    }

    /// <summary>Fills the path using the supplied rectangle to configure the fill.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    public static void FillPath(SKCanvas canvas, Paint paint, SKPath path, FillStyle fillStyle, PixelRect rect)
    {
        if (!fillStyle.IsVisible || fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        fillStyle.ApplyToPaint(paint, rect);
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Draws text along a path at the specified offsets.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="path">The path or pixel sequence to render.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="horizontalOffset">The horizontal offset along the path in pixels.</param>
    /// <param name="verticalOffset">The vertical offset from the path in pixels.</param>
    public static void DrawTextOnPath(SKCanvas canvas, Paint paint, SKPath path, string text, float horizontalOffset, float verticalOffset)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        canvas.DrawTextOnPath(text, path, horizontalOffset, verticalOffset, paint.SKTextAlign, paint.SKFont, paint.SKPaint);
    }

    /// <summary>Fills a rectangle when the fill style is visible.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    public static void FillRectangle(SKCanvas canvas, PixelRect rect, Paint paint, FillStyle fillStyle)
    {
        if (!fillStyle.IsVisible)
        {
            return;
        }

        if (fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        fillStyle.ApplyToPaint(paint, rect);
        canvas.DrawRect(rect.ToSKRect(), paint.SKPaint);
    }

    /// <summary>Draws a rounded rectangle using the current paint settings.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="radiusX">The horizontal corner radius in pixels.</param>
    /// <param name="radiusY">The vertical corner radius in pixels.</param>
    public static void DrawRoundRectangle(SKCanvas canvas, PixelRect rect, Paint paint, float radiusX, float radiusY) =>
        canvas.DrawRoundRect(rect.ToSKRect(), radiusX, radiusY, paint.SKPaint);

    /// <summary>Strokes a rectangle when the line style can be rendered.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    public static void DrawRectangle(SKCanvas canvas, PixelRect rect, Paint paint, LineStyle lineStyle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawRect(rect.ToSKRect(), paint.SKPaint);
    }

    /// <summary>Draws a rectangle, diagonals, and an optional point marker with a translucent fill.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="point">The optional point marked inside the rectangle.</param>
    /// <param name="color">The optional stroke color; magenta is used when omitted.</param>
    /// <param name="lineWidth">The debug outline width in pixels.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309:Optional public API parameters", Justification = "The optional debug parameters are retained for source compatibility.")]
    public static void DrawDebugRectangle(SKCanvas canvas, Paint paint, PixelRect rect, Pixel? point = null, Color? color = null, float lineWidth = 3)
    {
        point ??= Pixel.NaN;
        color ??= Colors.Magenta;

        paint.Color = color.Value;
        paint.IsStroke = true;
        paint.StrokeWidth = lineWidth;
        paint.IsAntialias = true;

        canvas.DrawRect(rect.ToSKRect(), paint.SKPaint);
        canvas.DrawLine(rect.BottomLeft.ToSKPoint(), rect.TopRight.ToSKPoint(), paint.SKPaint);
        canvas.DrawLine(rect.TopLeft.ToSKPoint(), rect.BottomRight.ToSKPoint(), paint.SKPaint);

        canvas.DrawCircle(point.Value.ToSKPoint(), DebugMarkerRadius, paint.SKPaint);

        paint.IsStroke = false;
        paint.Color = paint.Color.WithAlpha(DebugFillAlpha);
        canvas.DrawRect(rect.ToSKRect(), paint.SKPaint);
    }

    /// <summary>Strokes a circle when its line style can be rendered.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="center">The center point of the circle.</param>
    /// <param name="radius">The circle radius in pixels.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void DrawCircle(SKCanvas canvas, Pixel center, float radius, LineStyle lineStyle, Paint paint)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawCircle(center.ToSKPoint(), radius, paint.SKPaint);
    }

    /// <summary>Fills a circle using the supplied fill style.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="center">The center point of the circle.</param>
    /// <param name="radius">The circle radius in pixels.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    [Obsolete("use FillCircle() instead of DrawCircle() when passing in a FillStyle", false)]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "The obsolete overload is retained for source compatibility.")]
    public static void DrawCircle(SKCanvas canvas, Pixel center, float radius, FillStyle fillStyle, Paint paint) => FillCircle(canvas, center, radius, fillStyle, paint);

    /// <summary>Fills a circle when the fill style is visible.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="center">The center point of the circle.</param>
    /// <param name="radius">The circle radius in pixels.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void FillCircle(SKCanvas canvas, Pixel center, float radius, FillStyle fillStyle, Paint paint)
    {
        if (!fillStyle.IsVisible)
        {
            return;
        }

        if (fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        PixelRect rect = new(center, radius);
        fillStyle.ApplyToPaint(paint, rect);
        canvas.DrawCircle(center.ToSKPoint(), radius, paint.SKPaint);
    }

    /// <summary>Strokes an oval when the line style can be rendered.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    public static void DrawOval(SKCanvas canvas, Paint paint, LineStyle lineStyle, PixelRect rect)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawOval(rect.ToSKRect(), paint.SKPaint);
    }

    /// <summary>Fills an oval when the fill style is visible.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    public static void FillOval(SKCanvas canvas, Paint paint, FillStyle fillStyle, PixelRect rect)
    {
        if (!fillStyle.IsVisible)
        {
            return;
        }

        if (fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        fillStyle.ApplyToPaint(paint, rect);
        canvas.DrawOval(rect.ToSKRect(), paint.SKPaint);
    }

    /// <summary>Draws an open arc using the supplied start and sweep angles in degrees.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    /// <param name="sweepAngle">The angular sweep in degrees.</param>
    public static void DrawArc(SKCanvas canvas, Paint paint, LineStyle lineStyle, PixelRect rect, float startAngle, float sweepAngle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawArc(rect.ToSKRect(), startAngle, sweepAngle, false, paint.SKPaint);
    }

    /// <summary>Draws an arc whose angles are corrected for the ellipse aspect ratio.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    /// <param name="sweepAngle">The angular sweep in degrees.</param>
    public static void DrawEllipticalArc(SKCanvas canvas, Paint paint, LineStyle lineStyle, PixelRect rect, float startAngle, float sweepAngle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        (Angle correctedStart, Angle correctedSweep) = CorrectEllipseAngle(
            Angle.FromDegrees(startAngle),
            Angle.FromDegrees(sweepAngle),
            rect);
        DrawArc(canvas, paint, lineStyle, rect, (float)correctedStart.Degrees, (float)correctedSweep.Degrees);
    }

    /// <summary>Strokes the region between outer and inner ellipses over an angular sweep.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="innerRect">The rectangle defining the inner ellipse.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    /// <param name="sweepAngle">The angular sweep in degrees.</param>
    public static void DrawAnnularSector(
        SKCanvas canvas,
        Paint paint,
        LineStyle lineStyle,
        PixelRect rect,
        PixelRect innerRect,
        float startAngle,
        float sweepAngle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        using SKPath path = GetEllipticalAnnularSector(rect, innerRect, startAngle, sweepAngle);
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Fills the region between outer and inner ellipses over an angular sweep.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="innerRect">The rectangle defining the inner ellipse.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    /// <param name="sweepAngle">The angular sweep in degrees.</param>
    public static void FillAnnularSector(SKCanvas canvas, Paint paint, FillStyle fillStyle, PixelRect rect, PixelRect innerRect, float startAngle, float sweepAngle)
    {
        if (!fillStyle.IsVisible)
        {
            return;
        }

        if (fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        fillStyle.ApplyToPaint(paint, rect);

        using SKPath path = GetEllipticalAnnularSector(rect, innerRect, startAngle, sweepAngle);
        canvas.DrawPath(path, paint.SKPaint);
    }

    /// <summary>Strokes the outer and inner boundaries of an elliptical annulus.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="outerRect">The rectangle defining the outer ellipse.</param>
    /// <param name="innerRect">The rectangle defining the inner ellipse.</param>
    public static void DrawEllipticalAnnulus(SKCanvas canvas, Paint paint, LineStyle lineStyle, PixelRect outerRect, PixelRect innerRect)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        canvas.DrawOval(outerRect.ToSKRect(), paint.SKPaint);
        canvas.DrawOval(innerRect.ToSKRect(), paint.SKPaint);
    }

    /// <summary>Fills an elliptical ring by subtracting its inner oval from the outer oval.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="outerRect">The rectangle defining the outer ellipse.</param>
    /// <param name="innerRect">The rectangle defining the inner ellipse.</param>
    public static void FillEllipticalAnnulus(SKCanvas canvas, Paint paint, FillStyle fillStyle, PixelRect outerRect, PixelRect innerRect)
    {
        if (!fillStyle.IsVisible)
        {
            return;
        }

        if (fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        CanvasState canvasState = new(canvas);
        canvasState.Save();
        fillStyle.ApplyToPaint(paint, outerRect);

        using SKPathBuilder builder = new() { FillType = SKPathFillType.EvenOdd };
        builder.AddOval(outerRect.ToSKRect());
        builder.AddOval(innerRect.ToSKRect());
        using SKPath path = builder.Detach();

        canvas.DrawPath(path, paint.SKPaint);

        canvasState.Restore();
    }

    /// <summary>Draws a pie-shaped arc with angles corrected for the ellipse aspect ratio.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="lineStyle">The stroke configuration; rendering is skipped when it cannot be rendered.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    /// <param name="sweepAngle">The angular sweep in degrees.</param>
    public static void DrawSector(SKCanvas canvas, Paint paint, LineStyle lineStyle, PixelRect rect, float startAngle, float sweepAngle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        lineStyle.ApplyToPaint(paint);
        if (lineStyle.Hairline)
        {
            paint.StrokeWidth = HairlineStrokeWidth / canvas.TotalMatrix.ScaleX;
        }

        (Angle correctedStart, Angle correctedSweep) =
            CorrectEllipseAngle(Angle.FromDegrees(startAngle), Angle.FromDegrees(sweepAngle), rect);
        canvas.DrawArc(rect.ToSKRect(), (float)correctedStart.Degrees, (float)correctedSweep.Degrees, true, paint.SKPaint);
    }

    /// <summary>Fills a pie-shaped region with angles corrected for the ellipse aspect ratio.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="fillStyle">The fill configuration used for rendering.</param>
    /// <param name="rect">The rectangle defining the drawing area.</param>
    /// <param name="startAngle">The starting angle in degrees.</param>
    /// <param name="sweepAngle">The angular sweep in degrees.</param>
    public static void FillSector(SKCanvas canvas, Paint paint, FillStyle fillStyle, PixelRect rect, float startAngle, float sweepAngle)
    {
        if (!fillStyle.IsVisible)
        {
            return;
        }

        if (fillStyle.Color == Colors.Transparent)
        {
            return;
        }

        fillStyle.ApplyToPaint(paint, rect);

        (Angle correctedStart, Angle correctedSweep) =
            CorrectEllipseAngle(Angle.FromDegrees(startAngle), Angle.FromDegrees(sweepAngle), rect);
        canvas.DrawArc(rect.ToSKRect(), (float)correctedStart.Degrees, (float)correctedSweep.Degrees, true, paint.SKPaint);
    }

    /// <summary>Renders one marker when the marker style is visible.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixel">The pixel position at which to render the marker.</param>
    /// <param name="style">The marker configuration; rendering is skipped when it is not visible.</param>
    public static void DrawMarker(SKCanvas canvas, Paint paint, Pixel pixel, MarkerStyle style)
    {
        if (!style.IsVisible)
        {
            return;
        }

        IMarker marker = style.CustomRenderer ?? style.Shape.GetMarker();

        marker.Render(canvas, paint, pixel, style.Size, style);
    }

    /// <summary>Renders a marker at each supplied pixel when the marker style is visible.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="style">The marker configuration; rendering is skipped when it is not visible.</param>
    public static void DrawMarkers(SKCanvas canvas, Paint paint, IEnumerable<Pixel> pixels, MarkerStyle style)
    {
        if (!style.IsVisible || (style.Shape == MarkerShape.None && style.CustomRenderer is null))
        {
            return;
        }

        IMarker marker = style.CustomRenderer ?? style.Shape.GetMarker();

        foreach (Pixel pixel in pixels)
        {
            marker.Render(canvas, paint, pixel, style.Size, style);
        }
    }

    /// <summary>Renders markers for the ordered pixels, assigning a colormap color to each index.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    /// <param name="pixels">The ordered points that define the connected geometry.</param>
    /// <param name="style">The marker configuration; rendering is skipped when it is not visible.</param>
    /// <param name="colormap">The color map used to assign a color to each marker index.</param>
    public static void DrawMarkers(SKCanvas canvas, Paint paint, IReadOnlyList<Pixel> pixels, MarkerStyle style, IColormap colormap)
    {
        if (!style.IsVisible)
        {
            return;
        }

        IMarker marker = style.CustomRenderer ?? style.Shape.GetMarker();

        for (int i = 0; i < pixels.Count; i++)
        {
            Pixel pixel = pixels[i];
            style.MarkerColor = colormap.GetColor(i, pixels.Count);
            marker.Render(canvas, paint, pixel, style.Size, style);
        }
    }

    /// <summary>Creates a bitmap containing ARGB pixel values.</summary>
    /// <param name="argbs">The ARGB pixel values.</param>
    /// <param name="width">The bitmap width.</param>
    /// <param name="height">The bitmap height.</param>
    /// <returns>The bitmap containing the supplied pixels.</returns>
    public static SKBitmap BitmapFromArgbs(uint[] argbs, int width, int height)
    {
        GCHandle handle = GCHandle.Alloc(argbs, GCHandleType.Pinned);

        // We need to specify a colour type for platforms where BGRA is not the default
        // BGRA is the SKia name for ARGB, the ambiguity in naming comes from endianness
        var imageInfo = new SKImageInfo(width, height, SKColorType.Bgra8888);
        var bmp = new SKBitmap(imageInfo);
        _ = bmp.InstallPixels(
            info: imageInfo,
            pixels: handle.AddrOfPinnedObject(),
            rowBytes: imageInfo.RowBytes,
            releaseProc: (_, _) => handle.Free());

        return bmp;
    }

    /// <summary>Creates a color filter that maps gray values between two colors.</summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The optional background color.</param>
    /// <returns>The SkiaSharp color filter.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309:Optional public API parameters", Justification = "The optional background parameter is retained for source compatibility.")]
    public static SKColorFilter GetMaskColorFilter(Color foreground, Color? background = null)
    {
        // This function and the math is explained here: https://bclehmann.github.io/2022/11/06/UnmaskingWithSKColorFilter.html
        background ??= Colors.Black;

        float redDifference = foreground.Red - background.Value.Red;
        float greenDifference = foreground.Green - background.Value.Green;
        float blueDifference = foreground.Blue - background.Value.Blue;
        float alphaDifference = foreground.Alpha - background.Value.Alpha;

        // See https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/effects/color-filters
        // for an explanation of this matrix
        // Essentially, this matrix maps all gray colors to a line from `background.Value` to `foreground`.
        // Black and white are at the extremes on this line,
        // so they get mapped to `background.Value` and `foreground` respectively
        var mat = new float[]
        {
                redDifference / ColorComponentMaximum, 0, 0, 0, background.Value.Red / ColorComponentMaximum,
                0, greenDifference / ColorComponentMaximum, 0, 0, background.Value.Green / ColorComponentMaximum,
                0, 0, blueDifference / ColorComponentMaximum, 0, background.Value.Blue / ColorComponentMaximum,
                alphaDifference / ColorComponentMaximum,
                0,
                0,
                0,
                background.Value.Alpha / ColorComponentMaximum,
            };

        return SKColorFilter.CreateColorMatrix(mat);
    }

    /// <summary>Creates an RGBA premultiplied surface.</summary>
    /// <param name="width">The surface width.</param>
    /// <param name="height">The surface height.</param>
    /// <returns>The newly created SkiaSharp surface.</returns>
    public static SKSurface CreateSurface(int width, int height)
    {
        SKImageInfo imageInfo = new(
            width: width,
            height: height,
            colorType: SKColorType.Rgba8888,
            alphaType: SKAlphaType.Premul);

        return SKSurface.Create(imageInfo);
    }

    /// <summary>Encodes the surface as a PNG file at the specified path.</summary>
    /// <param name="surface">The surface used by the drawing operation.</param>
    /// <param name="filename">The destination file path.</param>
    public static void SavePng(SKSurface surface, string filename) => _ = new Image(surface).SavePng(filename);

    /// <summary>Draws the bitmap into the target rectangle using the paint sampling and alpha settings.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="image">The image source to draw.</param>
    /// <param name="target">The destination rectangle for the image.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void DrawImage(SKCanvas canvas, SKBitmap image, PixelRect target, Paint paint)
    {
        paint.Color = Colors.Black; // must be solid or image is semitransparent
        Image img = new(image);
        DrawImage(canvas, img, target, paint);
    }

    /// <summary>Draws the Skia image into the target rectangle using the paint sampling and alpha settings.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="image">The image source to draw.</param>
    /// <param name="target">The destination rectangle for the image.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void DrawImage(SKCanvas canvas, SKImage image, PixelRect target, Paint paint)
    {
        paint.Color = Colors.Black; // must be solid or image is semitransparent
        canvas.DrawImage(image, target.ToSKRect(), paint.SKSamplingOptions, paint.SKPaint);
    }

    /// <summary>Draws the image wrapper into the target rectangle using the paint sampling and alpha settings.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="image">The image source to draw.</param>
    /// <param name="target">The destination rectangle for the image.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void DrawImage(SKCanvas canvas, Image image, PixelRect target, Paint paint)
    {
        paint.Color = Colors.Black; // must be solid or image is semitransparent
        canvas.DrawImage(image.SKImage, target.ToSKRect(), paint.SKSamplingOptions, paint.SKPaint);
    }

    /// <summary>Draws text with the paint font and alignment at the supplied baseline position.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="px">The text baseline position.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void DrawText(SKCanvas canvas, string text, Pixel px, Paint paint) =>
        canvas.DrawText(text, px.X, px.Y, paint.SKTextAlign, paint.SKFont, paint.SKPaint);

    /// <summary>Shapes and draws text with the supplied shaper and paint settings.</summary>
    /// <param name="canvas">The canvas that receives the drawing operation.</param>
    /// <param name="shaper">The text shaper used to shape glyph runs.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="px">The text baseline position.</param>
    /// <param name="paint">The mutable paint whose Skia settings are used for rendering.</param>
    public static void DrawShapedText(SKCanvas canvas, SKShaper shaper, string text, Pixel px, Paint paint) =>
        canvas.DrawShapedText(shaper, text, px.X, px.Y, paint.SKTextAlign, paint.SKFont, paint.SKPaint);

    /// <summary>Corrects ellipse angles for the supplied rectangle.</summary>
    /// <param name="startAngle">The starting angle.</param>
    /// <param name="sweepAngle">The sweep angle.</param>
    /// <param name="rect">The ellipse rectangle.</param>
    /// <returns>The corrected start and sweep angles.</returns>
    private static (Angle startAngle, Angle sweepAngle) CorrectEllipseAngle(Angle startAngle, Angle sweepAngle, PixelRect rect)
    {
        Angle Correct(Angle angle)
        {
            // Calculate the coordinates on the circle
            double x = Math.Cos(angle.Radians);
            double y = Math.Sin(angle.Radians);

            // Map back to the ellipse and use atan2 to calculate the corrected angle
            var correctAngle =
                Angle.FromRadians(Math.Atan2(y * rect.Right, x * rect.Bottom));

            // Map back to the original range
            int absAngleDegrees = (int)Math.Abs(angle.Degrees);
            int scalar = absAngleDegrees switch
            {
                < (int)HalfCircleDegrees => 0,
                < (int)FullCircleDegrees => 1,
                _ => absAngleDegrees / (int)FullCircleDegrees,
            };
            return angle.Degrees switch
            {
                < (float)NegativeHalfCircleDegrees => correctAngle - (scalar * Angle.FromDegrees((float)FullCircleDegrees)),
                > (float)HalfCircleDegrees => correctAngle + (scalar * Angle.FromDegrees((float)FullCircleDegrees)),
                _ => correctAngle,
            };
        }

        Angle start = startAngle;
        Angle end = start + sweepAngle;
        Angle correctedStart = Correct(start);
        Angle correctedEnd = Correct(end);
        Angle correctedSweep = correctedEnd - correctedStart;
        return (correctedStart, correctedSweep);
    }

    /// <summary>Creates the path for an elliptical annular sector.</summary>
    /// <param name="outerRect">The outer ellipse rectangle.</param>
    /// <param name="innerRect">The inner ellipse rectangle.</param>
    /// <param name="startAngle">The starting angle.</param>
    /// <param name="sweepAngle">The sweep angle.</param>
    /// <returns>The sector path.</returns>
    private static SKPath GetEllipticalAnnularSector(PixelRect outerRect, PixelRect innerRect, float startAngle, float sweepAngle)
    {
        (Angle correctedStart, Angle correctedSweep) =
            CorrectEllipseAngle(Angle.FromDegrees(startAngle), Angle.FromDegrees(sweepAngle), outerRect);
        float start = (float)correctedStart.Degrees;
        float sweep = (float)correctedSweep.Degrees;

        static SKPoint GetPointOnEllipse(double centerX, double centerY, double a, double b, Angle theta)
        {
            double x = centerX + (a * Math.Cos(theta.Radians));
            double y = centerY + (b * Math.Sin(theta.Radians));
            return new((float)x, (float)y);
        }

        SKPoint innerArcEndPoint = GetPointOnEllipse(
            innerRect.Center.X,
            innerRect.Center.Y,
            innerRect.Right,
            innerRect.Bottom,
            correctedStart + correctedSweep);

        SKPoint outerArcStartPoint = GetPointOnEllipse(
            outerRect.Center.X,
            outerRect.Center.Y,
            outerRect.Right,
            outerRect.Bottom,
            correctedStart);

        using SKPathBuilder builder = new();
        builder.MoveTo(outerArcStartPoint);
        builder.ArcTo(outerRect.ToSKRect(), start, sweep, false);
        builder.LineTo(innerArcEndPoint);
        builder.ArcTo(innerRect.ToSKRect(), start + sweep, -sweep, false);
        builder.LineTo(outerArcStartPoint);
        builder.Close();
        return builder.Detach();
    }

    /// <summary>Copies a pixel sequence to an array, reusing an existing array when possible.</summary>
    /// <param name="pixels">The pixel sequence to copy.</param>
    /// <returns>An array containing the supplied pixels.</returns>
    private static Pixel[] CopyPixels(IEnumerable<Pixel> pixels) =>
        pixels is Pixel[] pixelArray ? pixelArray : [.. pixels];
}
