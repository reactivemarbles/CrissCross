// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot.Collections;

namespace CrissCross.Plot.Plottables;

/// <summary>Streams coordinate pairs into a fixed-capacity buffer for plotting.</summary>
/// <param name="capacity">The maximum number of points retained by the streamer.</param>
public class DataStreamerXY(int capacity) : IPlottable, IManagesAxisLimits, IHasLine, IHasLegendText, IHasMarker
{
    /// <summary>Gets or sets whether the streamer is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render points.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item associated with this streamer.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle);

    /// <summary>Gets or sets the text displayed for this streamer in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the streamer adjusts axis limits to its current data.</summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>Gets or sets the line style used to render the stream.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the marker style used to render streamed points.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new(MarkerShape.FilledCircle, 0);

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets the marker color.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets or sets both the line and marker fill colors.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineColor = value;
            MarkerFillColor = value;
        }
    }

    /// <summary>Gets the fixed-capacity buffer containing streamed coordinates.</summary>
    private CircularBuffer<Coordinates> Buffer { get; } = new(capacity);

    /// <summary>Gets the axis limits covering the streamed coordinates.</summary>
    /// <returns>The limits of the current data.</returns>
    public AxisLimits GetAxisLimits() => new(Buffer);

    /// <summary>Updates the axes to include the current data when automatic limits are enabled.</summary>
    /// <param name="plot">The plot whose axes are updated.</param>
    public void UpdateAxisLimits(Plot plot)
    {
        if (!ManageAxisLimits)
        {
            return;
        }

        AxisLimits dataLimits = GetAxisLimits();
        Axes.XAxis.Range.Set(dataLimits.XRange);
        Axes.YAxis.Range.Set(dataLimits.YRange);
    }

    /// <summary>Adds a point with numeric X and Y coordinates.</summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public void Add(double x, double y) => Buffer.Add(new(x, y));

    /// <summary>Adds a point with a date/time X coordinate and numeric Y coordinate.</summary>
    /// <param name="x">The date/time converted to an OLE Automation date.</param>
    /// <param name="y">The Y coordinate.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016:DateTime public boundary",
        Justification = "Retained for compatibility; the existing overload explicitly converts DateTime to its OLE Automation date.")]
    public void Add(DateTime x, double y) => Buffer.Add(new(x.ToOADate(), y));

    /// <summary>Adds a coordinate pair to the stream.</summary>
    /// <param name="point">The coordinate pair to add.</param>
    public void Add(Coordinates point) => Buffer.Add(point);

    /// <summary>Renders the streamed points in ascending pixel X order.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    public virtual void Render(RenderPack rp)
    {
        List<(Pixel Point, int Order)> orderedPixels = [];
        int order = 0;
        foreach (Coordinates coordinate in Buffer)
        {
            orderedPixels.Add((Axes.GetPixel(coordinate), order));
            order++;
        }

        orderedPixels.Sort(static (left, right) =>
        {
            int horizontalOrder = left.Point.X.CompareTo(right.Point.X);
            return horizontalOrder != 0 ? horizontalOrder : left.Order.CompareTo(right.Order);
        });

        Pixel[] pixels = new Pixel[orderedPixels.Count];
        for (int i = 0; i < orderedPixels.Count; i++)
        {
            pixels[i] = orderedPixels[i].Point;
        }

        Drawing.DrawLines(rp.Canvas, rp.Paint, pixels, LineStyle);
        Drawing.DrawMarkers(rp.Canvas, rp.Paint, pixels, MarkerStyle);
    }
}
