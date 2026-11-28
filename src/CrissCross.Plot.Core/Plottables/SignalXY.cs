// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Plots paired signal data with configurable line and marker styles.</summary>
/// <param name="dataSource">The source of the paired signal values.</param>
public class SignalXY(ISignalXYSource dataSource) : IPlottable, IHasLine, IHasMarker, IHasLegendText, IGetNearest
{
    /// <summary>Gets or sets the paired signal data source.</summary>
    public ISignalXYSource Data { get; set; } = dataSource;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the horizontal axis index.</summary>
    public int XAxisIndex { get; set; }

    /// <summary>Gets or sets the vertical axis index.</summary>
    public int YAxisIndex { get; set; }

    /// <summary>Gets or sets the axes used to position and render this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { Size = 0, Shape = MarkerShape.FilledCircle };

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker outline color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets the marker color.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the marker outline width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets or sets the style of lines to use when connecting points.</summary>
    public ConnectStyle ConnectStyle { get; set; } = ConnectStyle.Straight;

    /// <summary>Gets or sets the first data index to render.</summary>
    public int MinRenderIndex { get => Data.MinimumIndex; set => Data.MinimumIndex = value; }

    /// <summary>Gets or sets the last data index to render.</summary>
    public int MaxRenderIndex { get => Data.MaximumIndex; set => Data.MaximumIndex = value; }

    /// <summary>Gets or sets the shared line and marker color.</summary>
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

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility; callers should use LegendText.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets the legend item for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle, MarkerStyle);

    /// <summary>Gets the axis limits occupied by this plot.</summary>
    /// <returns>The axis limits of the signal data.</returns>
    public AxisLimits GetAxisLimits() => Data.GetAxisLimits();

    /// <summary>Gets the data point nearest to a location.</summary>
    /// <param name="mouseLocation">The location to search for, in plot coordinates.</param>
    /// <param name="renderInfo">The render details used to map coordinates.</param>
    /// <param name="maxDistance">The maximum distance at which a point is considered.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15) => Data is IDataSource ds
        ? DataSourceUtilities.GetNearestFast(ds, mouseLocation, renderInfo, maxDistance, Axes.XAxis, Axes.YAxis)
        : Data.GetNearest(mouseLocation, renderInfo, maxDistance);

    /// <summary>Gets the data point with the nearest X coordinate to a location.</summary>
    /// <param name="mouseLocation">The location to search for, in plot coordinates.</param>
    /// <param name="renderInfo">The render details used to map coordinates.</param>
    /// <param name="maxDistance">The maximum distance at which a point is considered.</param>
    /// <returns>The nearest data point by X coordinate.</returns>
    public DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15) =>
        Data is IDataSource ds ? DataSourceUtilities.GetNearestXFast(ds, mouseLocation, renderInfo, maxDistance, Axes.XAxis) : Data.GetNearestX(mouseLocation, renderInfo, maxDistance);

    /// <summary>Renders the paired signal values.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible || Data.Count == 0)
        {
            return;
        }

        IReadOnlyList<Pixel> markerPixels = Data.GetPixelsToDraw(rp, Axes, ConnectStyle);

        IReadOnlyList<Pixel> linePixels = ConnectStyle switch
        {
            ConnectStyle.Straight => markerPixels,
            ConnectStyle.StepHorizontal => Scatter.GetStepDisplayPixels(markerPixels, true),
            ConnectStyle.StepVertical => Scatter.GetStepDisplayPixels(markerPixels, false),
            _ => throw new ArgumentOutOfRangeException(nameof(ConnectStyle), ConnectStyle, "Unsupported connection style."),
        };

        Drawing.DrawLines(rp.Canvas, rp.Paint, linePixels, LineStyle);
        Drawing.DrawMarkers(rp.Canvas, rp.Paint, markerPixels, MarkerStyle);
    }
}
