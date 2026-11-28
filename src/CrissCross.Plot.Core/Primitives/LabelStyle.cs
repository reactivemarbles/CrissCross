// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using SkiaSharp.HarfBuzz;

namespace CrissCross.Plot;

/// <summary>Defines the appearance and rendering of a text or image label.</summary>
public class LabelStyle
{
    /// <summary>Indicates whether the resolved font uses italic text when Font is null.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "Italic is an existing public field; replacing it with a property breaks compiled callers.")]
    public bool Italic;

    /// <summary>The obsolete antialias setting; rendering uses the background and text settings instead.</summary>
    [Obsolete("use AntiAliasBackground and AntiAliasText", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "AntiAlias remains available to compiled consumers of the obsolete public member.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "AntiAlias is an existing public field; replacing it with a property breaks compiled callers.")]
    public bool AntiAlias = true;

    /// <summary>The point marker radius in pixels; nonpositive values hide the marker.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "PointSize is an existing public field; replacing it with a property breaks compiled callers.")]
    public float PointSize;

    /// <summary>Indicates whether the point marker is filled rather than outlined.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "PointFilled is an existing public field; replacing it with a property breaks compiled callers.")]
    public bool PointFilled;

    /// <summary>The color used for the point marker.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "PointColor is an existing public field; replacing it with a property breaks compiled callers.")]
    public Color PointColor = Colors.Magenta;

    /// <summary>The horizontal label offset in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "OffsetX is an existing public field; replacing it with a property breaks compiled callers.")]
    public float OffsetX; // TODO: automatic padding support for arbitrary rotations

    /// <summary>The vertical label offset in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "OffsetY is an existing public field; replacing it with a property breaks compiled callers.")]
    public float OffsetY; // TODO: automatic padding support for arbitrary rotations

    /// <summary>The horizontal radius of the background and border corners in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "BorderRadiusX is an existing public field; replacing it with a property breaks compiled callers.")]
    public float BorderRadiusX;

    /// <summary>The vertical radius of the background and border corners in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1401", Justification = "BorderRadiusY is an existing public field; replacing it with a property breaks compiled callers.")]
    public float BorderRadiusY;

    /// <summary>The default shadow offset in pixels.</summary>
    private const float DefaultShadowOffset = 3;

    /// <summary>The default font size.</summary>
    private const float DefaultFontSize = 12;

    /// <summary>The default underline offset in pixels.</summary>
    private const double DefaultUnderlineOffset = 2;

    /// <summary>The fraction for centered alignment.</summary>
    private const float CenterFraction = 0.5F;

    /// <summary>The divisor used to calculate half a distance.</summary>
    private const int HalfDivisor = 2;

    /// <summary>The path effect used when rendering text.</summary>
    private readonly SKPathEffect _solidPathEffect = LinePattern.Solid.GetPathEffect();

    /// <summary>Gets or sets a value indicating whether right-to-left language support is enabled globally.</summary>
    public static bool RTLSupport { get; set; }

    /// <summary>Gets a new label style with the default foreground color.</summary>
    public static LabelStyle Default => new() { IsVisible = true, ForeColor = Colors.Black };

    /// <summary>Gets or sets a value indicating whether the label is visible.</summary>
    public bool IsVisible { get; set; } = true;

    // TODO: deprecate this and pass text into the render method
    /// <summary>Gets or sets the label text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the alignment of the label contents.</summary>
    public Alignment Alignment { get; set; } = Alignment.UpperLeft;

    /// <summary>Gets or sets rotation in degrees clockwise from 0 (horizontal text).</summary>
    public float Rotation { get; set; }

    /// <summary>Gets or sets the text color.</summary>
    public Color ForeColor { get; set; } = Colors.Black;

    /// <summary>Gets or sets the obsolete background color property.</summary>
    [Obsolete("use BackgroundColor", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "BackColor remains available to compiled consumers of the obsolete public member.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public Color BackColor { get; set; }

    /// <summary>Gets or sets the background color.</summary>
    public Color BackgroundColor { get; set; } = Colors.Transparent;

    /// <summary>Gets or sets the border color.</summary>
    public Color BorderColor { get; set; } = Colors.Transparent;

    /// <summary>Gets or sets the border width in pixels.</summary>
    public float BorderWidth { get; set; } = 1;

    /// <summary>Gets the pixel rectangle calculated during the last background render.</summary>
    public PixelRect LastRenderPixelRect { get; private set; }

    /// <summary>Gets or sets the shadow color.</summary>
    public Color ShadowColor { get; set; } = Colors.Transparent;

    /// <summary>Gets or sets the shadow offset in pixels.</summary>
    public PixelOffset ShadowOffset { get; set; } = new(DefaultShadowOffset, DefaultShadowOffset);

    /// <summary>Gets or sets a value indicating whether to antialias the background and border.</summary>
    public bool AntiAliasBackground { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to antialias the text.</summary>
    public bool AntiAliasText { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to use subpixel text rendering.</summary>
    public bool SubpixelText { get; set; } = true;

    /// <summary>Gets or sets the typeface, or null to resolve it from the font name and style.</summary>
    public SKTypeface? Font { get; set; } = Fonts.DefaultFontStyle;

    /// <summary>Gets or sets the name used to resolve the typeface.</summary>
    public string FontName { get; set; } = Fonts.Default;

    /// <summary>Gets or sets the font size.</summary>
    public float FontSize { get; set; } = DefaultFontSize;

    /// <summary>Gets or sets a value indicating whether the text is bold.</summary>
    public bool Bold { get; set; }

    /// <summary>Gets or sets a value indicating whether the text is underlined.</summary>
    public bool Underline { get; set; }

    /// <summary>Gets or sets the underline width in pixels.</summary>
    public double UnderlineWidth { get; set; } = 1;

    /// <summary>Gets or sets the underline offset in pixels.</summary>
    public double UnderlineOffset { get; set; } = DefaultUnderlineOffset;

    /// <summary>
    /// Gets or sets manually defined line height in pixels.
    /// If not defined, the default line spacing will be used (according to the typeface, size, etc.)
    /// </summary>
    public float? LineSpacing { get; set; }

    /// <summary>Gets or sets uniform pixel padding; the getter is obsolete.</summary>
    public float Padding
    {
        [Obsolete("Get PixelPadding instead", true)]
        get => 0;
        set => PixelPadding = new(value);
    }

    // TODO: should add padding and margin
    /// <summary>Gets or sets the padding around the label contents.</summary>
    public PixelPadding PixelPadding { get; set; } = new(0, 0, 0, 0);

    /// <summary>Gets or sets both border radii in pixels.</summary>
    public float BorderRadius
    {
        get => BorderRadiusX;
        set
        {
            BorderRadiusX = value;
            BorderRadiusY = value;
        }
    }

    /// <summary>Gets or sets an image to render instead of the label text and styling.</summary>
    public Image? Image { get; set; }

    /// <summary>Gets a value indicating whether an underline should be rendered.</summary>
    private bool RenderUnderline => Underline && UnderlineWidth != 0;

    /// <summary>Use the characters in <see cref="Text"/> to determine an installed system font most likely to support this character set.</summary>
    public void SetBestFont() => FontName = Fonts.Detect(Text);

    /// <summary>Applies this label's text settings to the specified paint.</summary>
    /// <param name="paint">The paint to configure.</param>
    public void ApplyToPaint(Paint paint) => ApplyTextPaint(paint);

    /// <summary>Return size information for the contents of the <see cref="Text"/> property.</summary>
    /// <param name="paint">The paint used to measure the text.</param>
    /// <returns>The measured text dimensions and metrics.</returns>
    public MeasuredText Measure(Paint paint) => Measure(Text, paint); // NOTE: This should never be called internally.

    /// <summary>Measures the supplied text or image.</summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="paint">The paint used to measure the text.</param>
    /// <returns>The measured dimensions and metrics.</returns>
    public MeasuredText Measure(string text, Paint paint)
    {
        if (Image is not null)
        {
            return new() { Size = Image.Size, LineHeight = Image.Height, LineWidths = [Image.Width], VerticalOffset = 0, Bottom = 0, };
        }

        string[] lines = string.IsNullOrEmpty(text) ? [] : text.Split('\n');
        ApplyToPaint(paint);
        (float lineHeight, SKFontMetrics metrics) = paint.GetFontMetrics();
        float[] lineWidths = new float[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            lineWidths[i] = paint.MeasureText(lines[i]).Width;
        }

        float maxWidth = 0;
        foreach (float lineWidth in lineWidths)
        {
            if (lineWidth > maxWidth)
            {
                maxWidth = lineWidth;
            }
        }

        PixelSize size = new(maxWidth, lineHeight * lines.Length);

        float verticalOffset = metrics.Top + metrics.CapHeight - (metrics.Bottom / HalfDivisor);

        return new() { Size = size, LineHeight = lineHeight, LineWidths = lineWidths, VerticalOffset = verticalOffset, Bottom = metrics.Bottom, };
    }

    /// <summary>
    /// Use the Label's size and <see cref="Alignment"/> to determine where it should be drawn
    /// relative to the given rectangle (aligned to the rectangle according to <paramref name="rectAlignment"/>).
    /// </summary>
    /// <param name="rect">The rectangle relative to which the label is positioned.</param>
    /// <param name="rectAlignment">The alignment within the rectangle.</param>
    /// <param name="offsetX">The horizontal offset in pixels.</param>
    /// <param name="offsetY">The vertical offset in pixels.</param>
    /// <param name="paint">The paint used to measure the label.</param>
    /// <returns>The label's render location.</returns>
    public Pixel GetRenderLocation(PixelRect rect, Alignment rectAlignment, float offsetX, float offsetY, Paint paint)
    {
        PixelSize textSize = Measure(Text, paint).Size;
        if (Alignment != Alignment.UpperLeft)
        {
            throw new NotSupportedException("This method only works for labels with upper-left aligned text.");
        }

        float x = GetHorizontalLocation(rect, rectAlignment, textSize.Width, offsetX);
        float y = GetVerticalLocation(rect, rectAlignment, textSize.Height, offsetY);
        return new(x, y);

        static float GetHorizontalLocation(PixelRect bounds, Alignment alignment, float textWidth, float horizontalOffset) => alignment switch
        {
            Alignment.UpperLeft or Alignment.MiddleLeft or Alignment.LowerLeft => bounds.Left + horizontalOffset,
            Alignment.UpperCenter or Alignment.MiddleCenter or Alignment.LowerCenter => bounds.BottomCenter.X - (CenterFraction * textWidth),
            Alignment.UpperRight or Alignment.MiddleRight or Alignment.LowerRight => bounds.Right - textWidth - horizontalOffset,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "The alignment value is not supported."),
        };

        static float GetVerticalLocation(PixelRect bounds, Alignment alignment, float textHeight, float verticalOffset) => alignment switch
        {
            Alignment.UpperLeft or Alignment.UpperCenter or Alignment.UpperRight => bounds.Top + verticalOffset,
            Alignment.MiddleLeft or Alignment.MiddleCenter or Alignment.MiddleRight => bounds.LeftCenter.Y - (CenterFraction * textHeight),
            Alignment.LowerLeft or Alignment.LowerCenter or Alignment.LowerRight => bounds.Bottom - textHeight - verticalOffset,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "The alignment value is not supported."),
        };
    }

    /// <summary>Sets the label text and renders the label.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="px">The label location.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="bottom">True to use the bottom text metric; false to use the vertical offset. Defaults to true.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional bottom argument and its true default preserve existing source calls to this overload.")]
    public void Render(SKCanvas canvas, Pixel px, Paint paint, string text, bool bottom = true)
    {
        Text = text;
        Render(canvas, px, paint, bottom);
    }

    // TODO: deprecate this and require a string to be passed in
    /// <summary>Renders the current label text or image.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="px">The label location.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="bottom">True to use the bottom text metric; false to use the vertical offset. Defaults to true.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional bottom argument and its true default preserve existing source calls to this overload.")]
    public void Render(SKCanvas canvas, Pixel px, Paint paint, bool bottom = true)
    {
        if (!IsVisible)
        {
            return;
        }

        if (string.IsNullOrEmpty(Text) && Image is null)
        {
            return;
        }

        ApplyToPaint(paint);
        MeasuredText measured = Measure(Text, paint);
        PixelRect textRect = measured.Rect(Alignment);

        CanvasState canvasState = new(canvas);
        canvasState.Save();

        canvas.Translate(px.X + OffsetX, px.Y + OffsetY);
        canvas.RotateDegrees(Rotation);

        if (Image is null)
        {
            DrawBackground(canvas, px, paint, textRect);
            DrawText(canvas, measured, paint, textRect, bottom);
            DrawBorder(canvas, paint, textRect);
            DrawPoint(canvas, px, paint);
        }
        else
        {
            Image.Render(canvas, textRect, paint, false);
        }

        canvasState.Restore();
    }

    /// <summary>Finds the string with the greatest measured height.</summary>
    /// <param name="strings">The strings to measure.</param>
    /// <param name="paint">The paint used to measure the strings.</param>
    /// <returns>The first string with the greatest height and its measurement, or an empty string and zero.</returns>
    public (string text, float height) MeasureHighestString(string[] strings, Paint paint)
    {
        float maxHeight = 0;
        string maxText = string.Empty;

        for (int i = 0; i < strings.Length; i++)
        {
            PixelSize size = Measure(strings[i], paint).Size;
            if (size.Height > maxHeight)
            {
                maxHeight = size.Height;
                maxText = strings[i];
            }
        }

        return (maxText, maxHeight);
    }

    /// <summary>Finds the string with the greatest measured width.</summary>
    /// <param name="strings">The strings to measure.</param>
    /// <param name="paint">The paint used to measure the strings.</param>
    /// <returns>The first string with the greatest width and its measurement, or an empty string and zero.</returns>
    public (string text, PixelLength width) MeasureWidestString(string[] strings, Paint paint)
    {
        float maxWidth = 0;
        string maxText = string.Empty;

        for (int i = 0; i < strings.Length; i++)
        {
            PixelSize size = Measure(strings[i], paint).Size;
            if (size.Width > maxWidth)
            {
                maxWidth = size.Width;
                maxText = strings[i];
            }
        }

        return (maxText, maxWidth);
    }

    /// <summary>Configures paint for the border.</summary>
    /// <param name="paint">The paint to configure.</param>
    private void ApplyBorderPaint(Paint paint)
    {
        paint.IsStroke = true;
        paint.StrokeWidth = BorderWidth;
        paint.Color = BorderColor;
        paint.IsAntialias = AntiAliasBackground;
        paint.SKShader = null;
    }

    /// <summary>Configures paint for the shadow.</summary>
    /// <param name="paint">The paint to configure.</param>
    private void ApplyShadowPaint(Paint paint)
    {
        paint.IsStroke = false;
        paint.Color = ShadowColor;
        paint.IsAntialias = AntiAliasBackground;
        paint.SKShader = null;
    }

    /// <summary>Configures paint for the background.</summary>
    /// <param name="paint">The paint to configure.</param>
    private void ApplyBackgroundPaint(Paint paint)
    {
        paint.IsStroke = false;
        paint.Color = BackgroundColor;
        paint.IsAntialias = AntiAliasBackground;
        paint.SKShader = null;
    }

    /// <summary>Configures paint for the text.</summary>
    /// <param name="paint">The paint to configure.</param>
    private void ApplyTextPaint(Paint paint)
    {
        paint.TextAlign = HorizontalAlignment.Left;
        paint.IsStroke = false;
        paint.SKTypeface = Font ?? Fonts.GetTypeface(FontName, Bold, Italic);
        paint.TextSize = FontSize;
        paint.Color = ForeColor;
        paint.IsAntialias = AntiAliasText;
        paint.SubpixelText = SubpixelText;
        paint.SKShader = null;
        paint.SKPathEffect = _solidPathEffect;
    }

    /// <summary>Draws the label border.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="textRect">The measured text rectangle.</param>
    private void DrawBorder(SKCanvas canvas, Paint paint, PixelRect textRect)
    {
        if (BorderWidth == 0)
        {
            return;
        }

        PixelRect backgroundRect = textRect.Expand(PixelPadding);
        ApplyBorderPaint(paint);
        Drawing.DrawRoundRectangle(canvas, backgroundRect, paint, BorderRadiusX, BorderRadiusY);
    }

    /// <summary>Draws the label text and any underline.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="measured">The measured text dimensions and metrics.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="textRect">The measured text rectangle.</param>
    /// <param name="bottom">Whether to use the bottom text metric for vertical positioning.</param>
    private void DrawText(SKCanvas canvas, in MeasuredText measured, Paint paint, PixelRect textRect, bool bottom)
    {
        if (Text is null)
        {
            return;
        }

        ApplyTextPaint(paint);
        float verticalOffset = bottom ? -measured.Bottom : measured.VerticalOffset;

#if NETFRAMEWORK
        if (Text.IndexOf('\n') >= 0)
#else
        if (Text.Contains('\n'))
#endif
        {
            DrawMultilineText(canvas, Text.Split('\n'), measured, paint, textRect, verticalOffset);
            return;
        }

        Pixel location = new(textRect.Left, textRect.Bottom + verticalOffset);
        DrawTextLine(canvas, Text, location, paint);
    }

    /// <summary>Draws each line of a multiline label.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="lines">The lines of text to draw.</param>
    /// <param name="measured">The measured dimensions and line widths.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="textRect">The measured text rectangle.</param>
    /// <param name="verticalOffset">The vertical offset from the measured baseline.</param>
    private void DrawMultilineText(SKCanvas canvas, string[] lines, in MeasuredText measured, Paint paint, PixelRect textRect, float verticalOffset)
    {
        float lineHeight = LineSpacing ?? paint.FontSpacing;
        float horizontalFraction = Alignment.HorizontalFraction();

        for (int i = 0; i < lines.Length; i++)
        {
            float horizontalOffset = GetHorizontalOffset(horizontalFraction, textRect.Width, measured.LineWidths[i]);
            float pixelX = textRect.Left + horizontalOffset;
            float pixelY = textRect.Top + ((i + 1) * lineHeight) + verticalOffset;
            DrawTextLine(canvas, lines[i], new(pixelX, pixelY), paint);
        }

        static float GetHorizontalOffset(float fraction, float textWidth, float lineWidth) => fraction switch
        {
            0 => 0,
            CenterFraction => (textWidth / HalfDivisor) - (lineWidth / HalfDivisor),
            1 => textWidth - lineWidth,
            _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, "The horizontal alignment fraction is not supported."),
        };
    }

    /// <summary>Draws a line of label text and its optional underline.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="text">The text line to draw.</param>
    /// <param name="location">The text baseline location.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void DrawTextLine(SKCanvas canvas, string text, Pixel location, Paint paint)
    {
        float textWidth;
        if (LabelStyle.RTLSupport)
        {
            using SKShaper shaper = new(paint.SKTypeface);
            textWidth = shaper.Shape(text, paint.SKFont).Width;
            Drawing.DrawShapedText(canvas, shaper, text, location, paint);
        }
        else
        {
            Drawing.DrawText(canvas, text, location, paint);
            textWidth = paint.MeasureText(text).Width;
        }

        if (!RenderUnderline)
        {
            return;
        }

        DrawUnderline(canvas, paint, location, textWidth);
    }

    /// <summary>Draws an underline beneath a text line.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="location">The text baseline location.</param>
    /// <param name="textWidth">The width of the rendered text.</param>
    private void DrawUnderline(SKCanvas canvas, Paint paint, Pixel location, float textWidth)
    {
        float underlineY = location.Y + (float)UnderlineOffset;
        paint.StrokeWidth = (float)UnderlineWidth;
        paint.IsStroke = true;
        PixelLine line = new(location.X, underlineY, location.X + textWidth, underlineY);
        Drawing.DrawLine(canvas, paint, line);
    }

    /// <summary>Draws the label background and shadow.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="px">The label location.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="textRect">The measured text rectangle.</param>
    private void DrawBackground(SKCanvas canvas, Pixel px, Paint paint, PixelRect textRect)
    {
        PixelRect backgroundRect = textRect.Expand(PixelPadding);
        PixelRect shadowRect = backgroundRect.WithOffset(ShadowOffset);

        if (ShadowColor != Colors.Transparent)
        {
            ApplyShadowPaint(paint);
            Drawing.DrawRoundRectangle(canvas, shadowRect, paint, BorderRadiusX, BorderRadiusY);
        }

        if (BackgroundColor != Colors.Transparent)
        {
            ApplyBackgroundPaint(paint);
            Drawing.DrawRoundRectangle(canvas, backgroundRect, paint, BorderRadiusX, BorderRadiusY);
        }

        // TODO: support rotation
        LastRenderPixelRect = backgroundRect.Expand(shadowRect).WithDelta(px.X + OffsetX, px.Y + OffsetY);
    }

    /// <summary>Draws the point marker when enabled.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="px">The marker location.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void DrawPoint(SKCanvas canvas, Pixel px, Paint paint)
    {
        if (PointSize <= 0)
        {
            return;
        }

        if (PointFilled)
        {
            FillStyle fillStyle = new() { Color = PointColor, AntiAlias = AntiAliasBackground, };

            Drawing.FillCircle(canvas, px, PointSize, fillStyle, paint);
        }
        else
        {
            LineStyle lineStyle = new() { Color = PointColor, AntiAlias = AntiAliasBackground, };

            Drawing.DrawCircle(canvas, px, PointSize, lineStyle, paint);
        }
    }
}
