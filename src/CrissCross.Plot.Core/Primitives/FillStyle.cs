// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides fill color, hatch, and rendering settings for a styled object.</summary>
public class FillStyle
{
    /// <summary>Initializes a new instance of the <see cref="FillStyle"/> class.</summary>
    public FillStyle()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FillStyle"/> class with a fill color.</summary>
    /// <param name="color">The fill color.</param>
    public FillStyle(Color color)
    {
        Color = color;
    }

    /// <summary>Gets or sets the fill color.</summary>
    public Color Color { get; set; } = Colors.Transparent;

    /// <summary>Gets or sets the hatch color.</summary>
    public Color HatchColor { get; set; } = Colors.Gray;

    /// <summary>Gets or sets the optional hatch pattern.</summary>
    public IHatch? Hatch { get; set; }

    /// <summary>Gets whether the style specifies a visible fill or hatch.</summary>
    public bool HasValue => Color != Colors.Transparent || (Hatch is not null && HatchColor != Colors.Transparent);

    /// <summary>Gets or sets whether antialiasing is enabled.</summary>
    public bool AntiAlias { get; set; } = true;

    /// <summary>Gets or sets whether the style is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets whether this fill can be rendered.</summary>
    public bool CanBeRendered => IsVisible && Color.Alpha != 0 && Color != Colors.Transparent;

    /// <summary>Renders this style using the supplied render pack.</summary>
    /// <param name="rp">The render pack providing the canvas and paint.</param>
    /// <param name="rect">The rectangle to fill.</param>
    public void Render(RenderPack rp, PixelRect rect) => Render(rp.Canvas, rect, rp.Paint);

    /// <summary>Renders this style to a canvas.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="rect">The rectangle to fill.</param>
    /// <param name="paint">The paint to use.</param>
    public void Render(SKCanvas canvas, PixelRect rect, Paint paint)
    {
        if (!IsVisible)
        {
            return;
        }

        Drawing.FillRectangle(canvas, rect, paint, this);
    }

    /// <summary>Applies this style's settings to a paint.</summary>
    /// <param name="paint">The paint to configure.</param>
    /// <param name="rect">The rectangle used to configure the hatch shader.</param>
    public void ApplyToPaint(Paint paint, PixelRect rect)
    {
        paint.Color = Color;
        paint.IsStroke = false;
        paint.IsAntialias = AntiAlias;

        if (Hatch is not null)
        {
            paint.SKShader = Hatch.GetShader(Color, HatchColor, rect);
        }
        else
        {
            paint.SKShader = null;
        }
    }
}
