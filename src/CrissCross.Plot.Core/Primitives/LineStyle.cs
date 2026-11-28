// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Configures line color, width, pattern, and rendering behavior.</summary>
public class LineStyle
{
    /// <summary>The divisor used to calculate half a line width.</summary>
    private const int HalfWidthDivisor = 2;

    /// <summary>Initializes a new instance of the <see cref="LineStyle"/> class.</summary>
    public LineStyle()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="LineStyle"/> class with a width.</summary>
    /// <param name="width">The line width in pixels.</param>
    public LineStyle(float width)
    {
        Width = width;
    }

    /// <summary>Initializes a new instance of the <see cref="LineStyle"/> class with a width and color.</summary>
    /// <param name="width">The line width in pixels.</param>
    /// <param name="color">The line color.</param>
    public LineStyle(float width, Color color)
    {
        Width = width;
        Color = color;
    }

    /// <summary>Initializes a new instance of the <see cref="LineStyle"/> class with a width, color, and pattern.</summary>
    /// <param name="width">The line width in pixels.</param>
    /// <param name="color">The line color.</param>
    /// <param name="pattern">The line pattern.</param>
    public LineStyle(float width, Color color, LinePattern pattern)
    {
        Width = width;
        Color = color;
        Pattern = pattern;
    }

    /// <summary>Gets a style that does not render a line.</summary>
    public static LineStyle None => new() { Width = 0 };

    /// <summary>Gets or sets the line width in pixels.</summary>
    public float Width { get; set; }

    /// <summary>Gets or sets whether to render the line as one pixel regardless of scale.</summary>
    public bool Hairline { get; set; }

    /// <summary>Gets or sets whether the line appears hand drawn.</summary>
    public bool HandDrawn { get; set; }

    /// <summary>Gets or sets the hand-drawn segment length.</summary>
    public double HandDrawnSegmentLength { get; set; } = 7;

    /// <summary>Gets or sets the hand-drawn displacement amount.</summary>
    public double HandDrawnJitter { get; set; } = 1.2;

    /// <summary>Gets or sets the line color.</summary>
    public Color Color { get; set; } = Colors.Black;

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern Pattern { get; set; } = LinePattern.Solid;

    /// <summary>Gets or sets whether the line is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets whether antialiasing is enabled.</summary>
    public bool AntiAlias { get; set; } = true;

    /// <summary>Gets or sets whether the line caps and joins are rounded.</summary>
    public bool Rounded
    {
        get => StrokeCap == SKStrokeCap.Round;
        set
        {
            StrokeCap = value ? SKStrokeCap.Round : SKStrokeCap.Butt;
            StrokeJoin = value ? SKStrokeJoin.Round : SKStrokeJoin.Miter;
        }
    }

    /// <summary>Gets or sets the line cap style.</summary>
    public SKStrokeCap StrokeCap { get; set; } = SKStrokeCap.Butt;

    /// <summary>Gets or sets the line join style.</summary>
    public SKStrokeJoin StrokeJoin { get; set; } = SKStrokeJoin.Miter;

    /// <summary>Gets or sets the miter limit for line joins.</summary>
    public float StrokeMiter { get; set; } = 4;

    /// <summary>Gets whether this style can render a visible line.</summary>
    public bool CanBeRendered =>
        IsVisible
        && Color.Alpha != 0
        && Color != Colors.Transparent
        && (Width != 0 || Hairline);

    /// <summary>Creates a copy of this line style.</summary>
    /// <returns>A new style with the copied line settings.</returns>
    public LineStyle Clone() => new() { Width = Width, Color = Color, Hairline = Hairline, Pattern = Pattern, AntiAlias = AntiAlias, IsVisible = IsVisible, };

    /// <summary>Renders corresponding pixel line segments.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="starts">The line start points.</param>
    /// <param name="ends">The line end points.</param>
    /// <param name="paint">The paint to use.</param>
    public void Render(SKCanvas canvas, Pixel[] starts, Pixel[] ends, Paint paint)
    {
        if (starts.Length != ends.Length)
        {
            throw new ArgumentException($"{nameof(starts)} and {nameof(ends)} must have equal length");
        }

        using SKPathBuilder builder = new();

        for (int i = 0; i < starts.Length; i++)
        {
            builder.MoveTo(starts[i].X, starts[i].Y);
            builder.LineTo(ends[i].X, ends[i].Y);
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, this);
    }

    /// <summary>Renders the specified pixel lines.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="lines">The lines to render.</param>
    /// <param name="paint">The paint to use.</param>
    public void Render(SKCanvas canvas, PixelLine[] lines, Paint paint)
    {
        using SKPathBuilder builder = new();

        for (int i = 0; i < lines.Length; i++)
        {
            builder.MoveTo(lines[i].X1, lines[i].Y1);
            builder.LineTo(lines[i].X2, lines[i].Y2);
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, this);
    }

    /// <summary>Renders a pixel line using a render pack.</summary>
    /// <param name="rp">The render pack providing the canvas and paint.</param>
    /// <param name="line">The line to render.</param>
    public void Render(RenderPack rp, PixelLine line) => Render(rp.Canvas, line, rp.Paint);

    /// <summary>Renders a pixel line.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="line">The line to render.</param>
    /// <param name="paint">The paint to use.</param>
    public void Render(SKCanvas canvas, PixelLine line, Paint paint)
    {
        if (!IsVisible)
        {
            return;
        }

        Drawing.DrawLine(canvas, paint, line, this);
    }

    /// <summary>Renders a rectangle outline using a render pack.</summary>
    /// <param name="rp">The render pack providing the canvas and paint.</param>
    /// <param name="rect">The rectangle to outline.</param>
    public void Render(RenderPack rp, PixelRect rect) => Render(rp.Canvas, rect, rp.Paint);

    /// <summary>Renders a rectangle outline.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="rect">The rectangle to outline.</param>
    /// <param name="paint">The paint to use.</param>
    /// <param name="contract">Whether to contract the rectangle by half the line width.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional parameter default is part of the existing public API.")]
    public void Render(SKCanvas canvas, PixelRect rect, Paint paint, bool contract = false)
    {
        if (!IsVisible)
        {
            return;
        }

        if (contract)
        {
            rect = rect.Contract(Width / HalfWidthDivisor);
        }

        Pixel[] pixels =
        [
            rect.BottomLeft,
            rect.TopLeft,
            rect.TopRight,
            rect.BottomRight,
            rect.BottomLeft,
        ];

        Drawing.DrawLines(canvas, paint, pixels, this);
    }

    /// <summary>Renders a path.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="path">The path to render.</param>
    /// <param name="paint">The paint to use.</param>
    public void Render(SKCanvas canvas, SKPath path, Paint paint)
    {
        if (!IsVisible)
        {
            return;
        }

        Drawing.DrawPath(canvas, paint, path, this);
    }

    /// <summary>Renders a pixel line using the legacy paint argument order.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="paint">The paint to use.</param>
    /// <param name="line">The line to render.</param>
    [Obsolete("use the overload where the paint is passed last")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete overload is retained for source and binary compatibility.")]
    public void Render(SKCanvas canvas, Paint paint, PixelLine line)
    {
        if (!IsVisible)
        {
            return;
        }

        Drawing.DrawLine(canvas, paint, line, this);
    }

    /// <summary>Applies this style's settings to a paint.</summary>
    /// <param name="paint">The paint to configure.</param>
    public void ApplyToPaint(Paint paint)
    {
        paint.SKShader = null;
        paint.IsStroke = true;
        paint.Color = Color;
        paint.StrokeWidth = Hairline ? 1 : Width;
        paint.SKPathEffect = Pattern.GetPathEffect();
        paint.IsAntialias = AntiAlias;
        paint.SKStrokeCap = StrokeCap;
        paint.SKStrokeJoin = StrokeJoin;
        paint.StrokeMiter = StrokeMiter;
        paint.SKPaintStyle = SKPaintStyle.Stroke;

        if (!HandDrawn)
        {
            return;
        }

        SKPathEffect handDrawnEffect = SKPathEffect.CreateDiscrete((float)HandDrawnSegmentLength, (float)HandDrawnJitter);
        if (paint.SKPathEffect is null)
        {
            paint.SKPathEffect = handDrawnEffect;
        }
        else
        {
            paint.SKPathEffect = SKPathEffect.CreateCompose(paint.SKPathEffect, handDrawnEffect);
        }
    }
}
