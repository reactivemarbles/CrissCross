// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines line, fill, and geometry settings for rendering arrows.</summary>
public class ArrowStyle : IHasLine, IHasFill
{
    /// <summary>Gets or sets the line style used to draw the arrow.</summary>
    public LineStyle LineStyle { get; set; } = new();

    /// <summary>Gets or sets the arrow line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the arrow line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the arrow line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the fill style used to draw the arrow.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the arrow fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the arrow fill hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the hatch used to fill the arrow.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the position of the arrow anchor.</summary>
    public ArrowAnchor Anchor { get; set; } = ArrowAnchor.Center;

    /// <summary>Gets or sets whether the arrow is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the arrow width.</summary>
    public float ArrowWidth { get; set; } = 7;

    /// <summary>Gets or sets the arrowhead axis length.</summary>
    public float ArrowheadAxisLength { get; set; } = 15;

    /// <summary>Gets or sets the arrowhead length.</summary>
    public float ArrowheadLength { get; set; } = 20;

    /// <summary>Gets or sets the arrowhead width.</summary>
    public float ArrowheadWidth { get; set; } = 20;

    /// <summary>Gets or sets the minimum rendered arrow length, in pixels; its base moves away from the tip if too small.</summary>
    public float MinimumLength { get; set; }

    /// <summary>Gets or sets the maximum rendered arrow length, in pixels; its base moves toward the tip if too large.</summary>
    public float MaximumLength { get; set; } = float.MaxValue;

    /// <summary>Gets or sets the distance to offset the arrow from its tip along its axis.</summary>
    public float Offset { get; set; }

    /// <summary>Applies this style's line and fill settings to a paint object.</summary>
    /// <param name="paint">The paint object to configure.</param>
    public void ApplyToPaint(Paint paint)
    {
        LineStyle.ApplyToPaint(paint);
        paint.SKPaintStyle = SKPaintStyle.StrokeAndFill;
    }

    /// <summary>Renders an arrow line and its arrowhead.</summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="line">The line representing the arrow shaft.</param>
    /// <param name="paint">The paint object used to draw.</param>
    public void Render(SKCanvas canvas, PixelLine line, Paint paint)
    {
        if (!IsVisible)
        {
            return;
        }

        ApplyToPaint(paint);

        using SKPathBuilder builder = new();
        builder.MoveTo(line.Pixel1.ToSKPoint());
        builder.LineTo(line.Pixel2.ToSKPoint());

        // TODO: account for rotation
        builder.MoveTo(line.Pixel2.WithOffset(ArrowheadLength, ArrowheadWidth).ToSKPoint());
        builder.LineTo(line.Pixel2.ToSKPoint());
        builder.LineTo(line.Pixel2.WithOffset(ArrowheadLength, -ArrowheadWidth).ToSKPoint());

        using SKPath path = builder.Detach();
        ApplyToPaint(paint);
        LineStyle.Render(canvas, path, paint);
    }
}
