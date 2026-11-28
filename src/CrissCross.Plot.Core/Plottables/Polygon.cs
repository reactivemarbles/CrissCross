// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A collection of connected coordinates forming a closed shape.</summary>
public class Polygon : IPlottable, IHasLine, IHasFill, IHasMarker, IHasLegendText
{
    /// <summary>The cached axis limits of the polygon coordinates.</summary>
    private AxisLimits _limits;

    /// <summary>Initializes a new instance of the <see cref="Polygon"/> class.</summary>
    /// <param name="coords">The polygon vertex coordinates.</param>
    public Polygon(Coordinates[] coords)
    {
        UpdateCoordinates(coords);
    }

    /// <summary>Initializes a new instance of the <see cref="Polygon"/> class without coordinates.</summary>
    private Polygon()
    {
        Coordinates = [];
        IsEmpty = true;
    }

    /// <summary>Gets an empty polygon.</summary>
    public static Polygon Empty => new();

    /// <summary>Gets whether this polygon contains no coordinates.</summary>
    public bool IsEmpty { get; private set; }

    // TODO: replace with a generic data source
    /// <summary>Gets the polygon vertex coordinates.</summary>
    public Coordinates[] Coordinates { get; private set; } = Array.Empty<Coordinates>();

    /// <summary>Gets or sets the legacy legend label.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    [Obsolete("use LegendText")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this polygon in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this polygon is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the outline line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the outline line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the outline line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the outline line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the fill style.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the fill hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the fill hatch.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the vertex marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new();

    /// <summary>Gets or sets the vertex marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the vertex marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the vertex marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the vertex marker line color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets the vertex marker color.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the vertex marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets the number of vertices in the polygon.</summary>
    public int PointCount => Coordinates.Length;

    /// <summary>Gets or sets the axes used to position and render this polygon.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item represented by this polygon.</summary>
    public IEnumerable<LegendItem> LegendItems => [new LegendItem() { LabelText = LegendText, FillStyle = FillStyle, OutlineStyle = LineStyle, MarkerStyle = MarkerStyle, }];

    /// <summary>Returns a description of this polygon and its vertex count.</summary>
    /// <returns>A description of this polygon.</returns>
    public override string ToString()
    {
        string label = string.IsNullOrWhiteSpace(LegendText) ? string.Empty : $" ({LegendText})";
        return $"PlottablePolygon{label} with {PointCount} points";
    }

    /// <summary>Replaces the polygon coordinates and recalculates its axis limits.</summary>
    /// <param name="newCoordinates">The new polygon vertex coordinates.</param>
    public void UpdateCoordinates(Coordinates[] newCoordinates)
    {
        Coordinates = newCoordinates;
        _limits = AxisLimits.NoLimits;
        IsEmpty = Coordinates.Length == 0;
        if (IsEmpty)
        {
            return;
        }

        double minimumX = Coordinates[0].X;
        double maximumX = Coordinates[0].X;
        double minimumY = Coordinates[0].Y;
        double maximumY = Coordinates[0].Y;
        foreach (Coordinates coordinate in Coordinates)
        {
            minimumX = Math.Min(minimumX, coordinate.X);
            maximumX = Math.Max(maximumX, coordinate.X);
            minimumY = Math.Min(minimumY, coordinate.Y);
            maximumY = Math.Max(maximumY, coordinate.Y);
        }

        _limits = new(minimumX, maximumX, minimumY, maximumY);
    }

    /// <summary>Gets the axis limits occupied by this polygon.</summary>
    /// <returns>The cached axis limits.</returns>
    public AxisLimits GetAxisLimits() => _limits;

    /// <summary>Renders this polygon, its outline, and any configured vertex markers.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (IsEmpty)
        {
            return;
        }

        Pixel[] pixels = new Pixel[Coordinates.Length + 1];
        SKPoint[] points = new SKPoint[pixels.Length];
        for (int i = 0; i < Coordinates.Length; i++)
        {
            pixels[i] = Axes.GetPixel(Coordinates[i]);
            points[i] = pixels[i].ToSKPoint();
        }

        pixels[^1] = pixels[0];
        points[^1] = points[0];
        using SKPathBuilder builder = new();
        SKPoint firstPoint = points[0];
        builder.MoveTo(firstPoint);
        float maximumX = firstPoint.X;
        float minimumX = firstPoint.X;
        float maximumY = firstPoint.Y;
        float minimumY = firstPoint.Y;
        for (int i = 1; i < points.Length; i++)
        {
            SKPoint point = points[i];
            maximumX = Math.Max(maximumX, point.X);
            minimumX = Math.Min(minimumX, point.X);
            maximumY = Math.Max(maximumY, point.Y);
            minimumY = Math.Min(minimumY, point.Y);
            builder.LineTo(point);
        }

        using SKPath path = builder.Detach();
        if (FillStyle.HasValue)
        {
            PixelRect rect = new(minimumX, maximumX, minimumY, maximumY);
            Drawing.FillPath(rp.Canvas, rp.Paint, path, FillStyle, rect);
        }

        if (LineStyle is { IsVisible: true, Width: > 0 })
        {
            Drawing.DrawLines(rp.Canvas, rp.Paint, pixels, LineStyle);
        }

        if (!MarkerStyle.IsVisible)
        {
            return;
        }

        Drawing.DrawMarkers(rp.Canvas, rp.Paint, pixels, MarkerStyle);
    }
}
