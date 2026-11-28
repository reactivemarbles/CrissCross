// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Adapts SkiaSharp paint and font settings for plotting.</summary>
public sealed class Paint : IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="Paint"/> class for internal use.</summary>
    private Paint()
    {
        SKFont.Hinting = SKFontHinting.Slight;
    }

    /// <summary>Gets or sets the paint color.</summary>
    public Color Color { get => Color.FromSKColor(SKPaint.Color); set => SKPaint.Color = value.ToSKColor(); }

    /// <summary>Gets or sets whether the paint draws a stroke.</summary>
    public bool IsStroke { get => SKPaint.IsStroke; set => SKPaint.IsStroke = value; }

    /// <summary>Gets or sets the stroke width.</summary>
    public float StrokeWidth { get => SKPaint.StrokeWidth; set => SKPaint.StrokeWidth = value; }

    /// <summary>Gets or sets whether paint antialiasing is enabled.</summary>
    public bool IsAntialias { get => SKPaint.IsAntialias; set => SKPaint.IsAntialias = value; }

    /// <summary>Gets or sets the stroke miter limit.</summary>
    public float StrokeMiter { get => SKPaint.StrokeMiter; set => SKPaint.StrokeMiter = value; }

    /// <summary>Gets or sets options for customizing font.</summary>
    public float TextSize { get => SKFont.Size; set => SKFont.Size = value; }

    /// <summary>Gets the font spacing.</summary>
    public float FontSpacing { get => SKFont.Spacing; }

    /// <summary>Gets or sets horizontal text alignment.</summary>
    public HorizontalAlignment TextAlign
    {
        get => SKTextAlign switch
        {
            SKTextAlign.Left => HorizontalAlignment.Left,
            SKTextAlign.Center => HorizontalAlignment.Center,
            SKTextAlign.Right => HorizontalAlignment.Right,
            _ => throw new NotSupportedException($"Unsupported text alignment: {SKTextAlign}")
        };
        set => SKTextAlign = value.ToSKTextAlign();
    }

    /// <summary>Gets or sets whether subpixel text rendering is enabled.</summary>
    public bool SubpixelText { get => SKFont.Subpixel; set => SKFont.Subpixel = value; }

    /// <summary>Gets or sets whether the font is emboldened.</summary>
    public bool Bold { get => SKFont.Embolden; set => SKFont.Embolden = value; }

    /// <summary>Sets options for images.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "The write-only option intentionally configures an external sampling struct without exposing it as API.")]
    public ResizeFilter ResizeFilter
    {
        set => SKSamplingOptions = value switch
            {
                ResizeFilter.NearestNeighbor => new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None),
                ResizeFilter.Bilinear => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
                ResizeFilter.Bicubic => new SKSamplingOptions(SKCubicResampler.Mitchell),
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unknown filter quality: '{value}'"),
            };
    }

    /// <summary>Gets or sets the underlying Skia paint instance.</summary>
    public SKPaint SKPaint { get; set; } = new();

    /// <summary>Gets or sets the underlying Skia font instance.</summary>
    public SKFont SKFont { get; set; } = new(SKTypeface.Default);

    /// <summary>Gets or sets the obsolete Skia color.</summary>
    [Obsolete("use Color instead of SKColor", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete public property is retained for source and binary compatibility.")]
    public SKColor SKColor { get => SKPaint.Color; set => SKPaint.Color = value; }

    /// <summary>Gets or sets the Skia text alignment.</summary>
    public SKTextAlign SKTextAlign { get; set; }

    /// <summary>Gets or sets the Skia typeface.</summary>
    public SKTypeface SKTypeface { get => SKFont.Typeface; set => SKFont.Typeface = value; }

    /// <summary>Gets or sets the Skia shader.</summary>
    public SKShader? SKShader { get => SKPaint.Shader; set => SKPaint.Shader = value; }

    /// <summary>Gets or sets the Skia path effect.</summary>
    public SKPathEffect SKPathEffect { get => SKPaint.PathEffect; set => SKPaint.PathEffect = value; }

    /// <summary>Gets or sets the Skia stroke cap.</summary>
    public SKStrokeCap SKStrokeCap { get => SKPaint.StrokeCap; set => SKPaint.StrokeCap = value; }

    /// <summary>Gets or sets the Skia stroke join.</summary>
    public SKStrokeJoin SKStrokeJoin { get => SKPaint.StrokeJoin; set => SKPaint.StrokeJoin = value; }

    /// <summary>Gets or sets the Skia sampling options.</summary>
    public SKSamplingOptions SKSamplingOptions { get; set; }

    /// <summary>Gets or sets the Skia paint style.</summary>
    public SKPaintStyle SKPaintStyle { get => SKPaint.Style; set => SKPaint.Style = value; }

    /// <summary>Creates a disposable paint instance.</summary>
    /// <returns>A new paint instance.</returns>
    public static Paint NewDisposablePaint() => new();

    /// <summary>Releases the underlying Skia paint and font resources.</summary>
    public void Dispose()
    {
        SKPaint.Dispose();
        SKFont.Dispose();
    }

    /// <summary>Measures the bounds of a string using this font.</summary>
    /// <param name="str">The string to measure.</param>
    /// <returns>The measured pixel bounds.</returns>
    public PixelRect MeasureText(string str)
    {
        float width = SKFont.MeasureText(str, out SKRect textBounds);
        return new(width, textBounds.Height);
    }

    /// <summary>Gets the line height and metrics for this font.</summary>
    /// <returns>The line height and font metrics.</returns>
    public (float lineHeight, SKFontMetrics metrics) GetFontMetrics()
    {
        float lineHeight = SKFont.GetFontMetrics(out SKFontMetrics metrics);
        return (lineHeight, metrics);
    }
}
