// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Configures a marker's shape, dimensions, and rendering styles.</summary>
public class MarkerStyle : IHasLine, IHasFill, IHasOutline
{
    /// <summary>The default width for line-only markers.</summary>
    private const float LineOnlyWidth = 2;

    /// <summary>The default marker diameter.</summary>
    private const float DefaultMarkerSize = 5;

    /// <summary>Initializes a new instance of the <see cref="MarkerStyle"/> class.</summary>
    public MarkerStyle()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MarkerStyle"/> class with a shape and size.</summary>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The marker diameter in pixels.</param>
    public MarkerStyle(MarkerShape shape, float size)
        : this(shape, size, Colors.Gray)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MarkerStyle"/> class with a shape, size, and color.</summary>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The marker diameter in pixels.</param>
    /// <param name="color">The marker color.</param>
    public MarkerStyle(MarkerShape shape, float size, Color color)
    {
        Shape = shape;
        LineColor = color;
        if (shape.IsLineOnly())
        {
            FillColor = Colors.Transparent;
            LineWidth = LineOnlyWidth;
        }
        else
        {
            FillColor = color;
        }

        Size = size;
    }

    /// <summary>Gets a default filled-circle marker style.</summary>
    public static MarkerStyle Default => new(MarkerShape.FilledCircle, DefaultMarkerSize);

    /// <summary>Gets a style that does not render a marker.</summary>
    public static MarkerStyle None => new(MarkerShape.None, 0);

    /// <summary>Gets or sets whether the marker is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape Shape { get; set; }

    /// <summary>Gets or sets the marker diameter in pixels.</summary>
    public float Size { get; set; }

    /// <summary>Gets or sets the marker color for its fill and line.</summary>
    public Color MarkerColor
    {
        get => LineColor;
        set
        {
            FillColor = value;
            LineColor = value;
        }
    }

    /// <summary>Gets or sets the line style for shapes whose geometry consists of lines.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the marker line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the marker line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the fill style for filled marker shapes.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the marker hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the marker hatch pattern.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets a custom marker renderer.</summary>
    public IMarker? CustomRenderer { get; set; }

    /// <summary>Gets or sets the obsolete fill-outline setting.</summary>
    [Obsolete("This property is obsolete. An outline will be drawn if OutlineWidth width is greater than 0.", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete public property is retained for source and binary compatibility.")]
    public bool FillOutline { get; set; }

    /// <summary>Gets or sets the obsolete marker outline style.</summary>
    [Obsolete("use OutlineWidth, OutlineColor, OutlineStyle, etc", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete public property is retained for source and binary compatibility.")]
    public LineStyle Outline { get => OutlineStyle; set => OutlineStyle = value; }

    /// <summary>Gets or sets the marker outline style.</summary>
    public LineStyle OutlineStyle { get; set; } = new() { Color = Colors.Black, Width = 0 };

    /// <summary>Gets or sets the marker outline width.</summary>
    public float OutlineWidth { get => OutlineStyle.Width; set => OutlineStyle.Width = value; }

    /// <summary>Gets or sets the marker outline pattern.</summary>
    public LinePattern OutlinePattern { get => OutlineStyle.Pattern; set => OutlineStyle.Pattern = value; }

    /// <summary>Gets or sets the marker outline color.</summary>
    public Color OutlineColor { get => OutlineStyle.Color; set => OutlineStyle.Color = value; }

    /// <summary>Gets or sets the obsolete marker fill style.</summary>
    [Obsolete("use FillColor, FillHatchColor, FillHatch, or FillStyle", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete public property is retained for source and binary compatibility.")]
    public FillStyle Fill { get => FillStyle; set => FillStyle = value; }

    /// <summary>Renders this marker using a render pack.</summary>
    /// <param name="rp">The render pack providing the canvas and paint.</param>
    /// <param name="px">The pixel coordinate at which to render.</param>
    public void Render(RenderPack rp, Pixel px) => Render(rp.Canvas, px, rp.Paint);

    /// <summary>Renders this marker to a canvas.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="px">The pixel coordinate at which to render.</param>
    /// <param name="paint">The paint to use.</param>
    public void Render(SKCanvas canvas, Pixel px, Paint paint)
    {
        if (!IsVisible)
        {
            return;
        }

        Drawing.DrawMarker(canvas, paint, px, this);
    }
}
