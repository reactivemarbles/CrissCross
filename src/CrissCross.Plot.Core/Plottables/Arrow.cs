// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders an arrow between two data coordinates.</summary>
public class Arrow : IPlottable, IHasArrow, IHasLegendText
{
    /// <summary>The default width of an arrow line.</summary>
    private const float DefaultLineWidth = 2;

    /// <summary>Gets or sets the coordinate at the base of the arrow.</summary>
    public Coordinates Base { get; set; } = Coordinates.Origin;

    /// <summary>Gets or sets the coordinate at the tip of the arrow.</summary>
    public Coordinates Tip { get; set; } = Coordinates.Origin;

    /// <summary>Gets or sets whether the arrow is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render the arrow.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items associated with this arrow.</summary>
    public IEnumerable<LegendItem> LegendItems => [new LegendItem() { ArrowStyle = ArrowStyle, LabelText = LegendText }];

    /// <summary>Gets or sets the text displayed for this arrow in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the style used to render the arrow.</summary>
    public ArrowStyle ArrowStyle { get; set; } = new() { LineWidth = DefaultLineWidth, LineColor = Colors.Black };

    /// <summary>Gets or sets the arrow line color.</summary>
    public Color ArrowLineColor { get => ArrowStyle.LineStyle.Color; set => ArrowStyle.LineStyle.Color = value; }

    /// <summary>Gets or sets the arrow line width.</summary>
    public float ArrowLineWidth { get => ArrowStyle.LineStyle.Width; set => ArrowStyle.LineStyle.Width = value; }

    /// <summary>Gets or sets the arrow fill color.</summary>
    public Color ArrowFillColor { get => ArrowStyle.FillStyle.Color; set => ArrowStyle.FillStyle.Color = value; }

    /// <summary>Gets or sets the minimum arrow length.</summary>
    public float ArrowMinimumLength { get => ArrowStyle.MinimumLength; set => ArrowStyle.MinimumLength = value; }

    /// <summary>Gets or sets the maximum arrow length.</summary>
    public float ArrowMaximumLength { get => ArrowStyle.MaximumLength; set => ArrowStyle.MaximumLength = value; }

    /// <summary>Gets or sets the offset applied to the arrow.</summary>
    public float ArrowOffset { get => ArrowStyle.Offset; set => ArrowStyle.Offset = value; }

    /// <summary>Gets or sets the anchor point for the arrow.</summary>
    public ArrowAnchor ArrowAnchor { get => ArrowStyle.Anchor; set => ArrowStyle.Anchor = value; }

    /// <summary>Gets or sets the width of the arrow body.</summary>
    public float ArrowWidth { get => ArrowStyle.ArrowWidth; set => ArrowStyle.ArrowWidth = value; }

    /// <summary>Gets or sets the length of the arrowhead axis.</summary>
    public float ArrowheadAxisLength { get => ArrowStyle.ArrowheadAxisLength; set => ArrowStyle.ArrowheadAxisLength = value; }

    /// <summary>Gets or sets the arrowhead length.</summary>
    public float ArrowheadLength { get => ArrowStyle.ArrowheadLength; set => ArrowStyle.ArrowheadLength = value; }

    /// <summary>Gets or sets the arrowhead width.</summary>
    public float ArrowheadWidth { get => ArrowStyle.ArrowheadWidth; set => ArrowStyle.ArrowheadWidth = value; }

    /// <summary>Gets or sets the obsolete label alias for <see cref="LegendText"/>.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility alias for the established public API.")]
    [Obsolete("use LegendText")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the obsolete arrow color.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("use ArrowLineColor or ArrowFillColor", true)]
    public Color ArrowColor { get; set; }

    /// <summary>Gets or sets the obsolete color alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("use ArrowLineColor or ArrowFillColor", true)]
    public Color Color { get; set; }

    /// <summary>Gets or sets the obsolete arrow offset.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("use ArrowOffset", true)]
    public float Offset { get; set; }

    /// <summary>Gets or sets the obsolete minimum arrow length.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("use ArrowMinimumLength", true)]
    public float MinimumLength { get; set; }

    /// <summary>Gets or sets the obsolete line width.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("use ArrowLineWidth", true)]
    public float LineWidth { get; set; }

    /// <summary>Gets or sets the obsolete arrow line style.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("use ArrowLineWidth, ArrowLineColor, or ArrowStyle.LineStyle", true)]
    public LineStyle LineStyle { get => ArrowStyle.LineStyle; set => ArrowStyle.LineStyle = value; }

    /// <summary>Gets or sets the shape used to render the arrowhead.</summary>
    public IArrowShape ArrowShape { get; set; } = new ArrowShapes.Single();

    /// <summary>Gets the axis limits covering the base and tip coordinates.</summary>
    /// <returns>The minimum and maximum axis values required by the arrow.</returns>
    public AxisLimits GetAxisLimits() => new(
        Math.Min(Base.X, Tip.X),
        Math.Max(Base.X, Tip.X),
        Math.Min(Base.Y, Tip.Y),
        Math.Max(Base.Y, Tip.Y));

    /// <summary>Renders the arrow on the supplied canvas.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    public virtual void Render(RenderPack rp)
    {
        Pixel basePixel = Axes.GetPixel(Base);
        Pixel tipPixel = Axes.GetPixel(Tip);
        PixelLine pixelLine = new(basePixel, tipPixel);

        if (ArrowOffset != 0)
        {
            pixelLine = pixelLine.BackedUpBy(ArrowOffset);
        }

        ArrowShape.Render(rp, pixelLine, ArrowStyle);
    }
}
