// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders data points as stems with markers at their coordinate values.</summary>
/// <param name="coordinates">The coordinates represented by the lollipop plot.</param>
public class LollipopPlot(IEnumerable<Coordinates> coordinates) : IPlottable, IHasLine, IHasMarker, IGetNearest
{
    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the text shown for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets the legend item represented by this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle, MarkerStyle);

    /// <summary>Gets or sets the stem line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the stem line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the stem line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the stem line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { Size = Constants.MarkerSize, Shape = MarkerShape.FilledCircle, IsVisible = true };

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

    /// <summary>Gets or sets the stem line and marker color.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineStyle.Color = value;
            MarkerStyle.FillColor = value;
            MarkerStyle.LineColor = value;
        }
    }

    /// <summary>Gets or sets the plotted coordinates.</summary>
    public IEnumerable<Coordinates> Coordinates { get; set; } = coordinates;

    /// <summary>Gets or sets whether stems extend vertically or horizontally.</summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    /// <summary>Gets or sets the coordinate value where each stem begins.</summary>
    public double BaseValue { get; set; }

    /// <summary>Gets the axis limits occupied by the plotted coordinates.</summary>
    /// <returns>The axis limits of the coordinate set.</returns>
    public AxisLimits GetAxisLimits() => new(Coordinates);

    /// <summary>Finds the nearest plotted point to the specified coordinate.</summary>
    /// <param name="mouseLocation">The coordinate to search from.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <returns>The nearest data point within the default search distance.</returns>
    public DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo) =>
        GetNearest(mouseLocation, renderInfo, Constants.DefaultMaximumDistance);

    /// <summary>Finds the nearest plotted point to the specified coordinate within a distance.</summary>
    /// <param name="mouseLocation">The coordinate to search from.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest data point within the specified distance.</returns>
    public DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance)
    {
        DataSources.CoordinateDataSource dataSource = CreateDataSource();
        return DataSourceUtilities.GetNearest(dataSource, mouseLocation, renderInfo, maxDistance, Axes.XAxis, Axes.YAxis);
    }

    /// <summary>Finds the nearest plotted point along the x-axis.</summary>
    /// <param name="mouseLocation">The coordinate to search from.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <returns>The nearest data point within the default search distance.</returns>
    public DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo) =>
        GetNearestX(mouseLocation, renderInfo, Constants.DefaultMaximumDistance);

    /// <summary>Finds the nearest plotted point along the x-axis within a distance.</summary>
    /// <param name="mouseLocation">The coordinate to search from.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum distance in pixels.</param>
    /// <returns>The nearest data point within the specified distance.</returns>
    public DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance)
    {
        DataSources.CoordinateDataSource dataSource = CreateDataSource();
        return DataSourceUtilities.GetNearestX(dataSource, mouseLocation, renderInfo, maxDistance, Axes.XAxis);
    }

    /// <summary>Renders the stems and markers for this plot.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        foreach (Coordinates coordinate in Coordinates)
        {
            Coordinates lineBase = Orientation == Orientation.Vertical
                ? new(coordinate.X, BaseValue)
                : new(BaseValue, coordinate.Y);
            CoordinateLine line = new(lineBase, coordinate);
            PixelLine pixelLine = Axes.GetPixelLine(line);
            Pixel pixel = Axes.GetPixel(coordinate);
            Drawing.DrawLine(rp.Canvas, rp.Paint, pixelLine, LineStyle);
            Drawing.DrawMarker(rp.Canvas, rp.Paint, pixel, MarkerStyle);
        }
    }

    /// <summary>Creates a coordinate data source from the current plotted coordinates.</summary>
    /// <returns>A data source containing a snapshot of the coordinates.</returns>
    private DataSources.CoordinateDataSource CreateDataSource() =>
        new(new List<Coordinates>(Coordinates));

    /// <summary>Contains default values used by lollipop plots.</summary>
    private static class Constants
    {
        /// <summary>The default marker size in pixels.</summary>
        public const float MarkerSize = 5;

        /// <summary>The default nearest-point search distance in pixels.</summary>
        public const float DefaultMaximumDistance = 15;
    }
}
