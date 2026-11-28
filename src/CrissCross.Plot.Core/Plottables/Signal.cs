// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a sampled signal as a line at both high and low display densities.</summary>
/// <param name="data">The signal source to render.</param>
public class Signal(ISignalSource data) : IPlottable, IHasLine, IHasMarker, IHasLegendText, IGetNearest
{
    /// <summary>Gets or sets whether this signal is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this signal.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the signal data source.</summary>
    public ISignalSource Data { get; set; } = data;

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility; callers should use LegendText.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed for this signal in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { Size = Constants.DefaultMarkerSize, Shape = MarkerShape.FilledCircle };

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

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = Constants.DefaultLineWidth };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the first data index to render.</summary>
    public int MinRenderIndex { get => Data.MinimumIndex; set => Data.MinimumIndex = value; }

    /// <summary>Gets or sets the last data index to render.</summary>
    public int MaxRenderIndex { get => Data.MaximumIndex; set => Data.MaximumIndex = value; }

    /// <summary>Gets or sets the maximum marker size in pixels when the plot is zoomed in.</summary>
    public float MaximumMarkerSize { get; set; } = Constants.MaximumMarkerSize;

    /// <summary>Gets or sets whether low-density rendering is always used.</summary>
    public bool AlwaysUseLowDensityMode { get; set; }

    /// <summary>Gets or sets the shared line and marker color.</summary>
    public Color Color
    {
        get => LineColor;
        set
        {
            LineColor = value;
            MarkerFillColor = value;
            MarkerLineColor = value;
        }
    }

    /// <summary>Gets the legend item for this signal.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, MarkerStyle, LineStyle);

    /// <summary>Gets the axis limits occupied by this signal.</summary>
    /// <returns>The axis limits of the signal.</returns>
    public AxisLimits GetAxisLimits() => Data.GetLimits();

    /// <summary>Renders this signal using the appropriate display-density strategy.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!HasVisibleValues())
        {
            return;
        }

        if (PointsPerPixel() < 1 || AlwaysUseLowDensityMode)
        {
            RenderLowDensity(rp);
        }
        else
        {
            RenderHighDensity(rp);
        }
    }

    /// <summary>Gets the data point nearest to a location.</summary>
    /// <param name="mouseLocation">The location to search for, in plot coordinates.</param>
    /// <param name="renderInfo">The render details used to map coordinates.</param>
    /// <param name="maxDistance">The maximum distance at which a point is considered.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15)
    {
        if (Data is IDataSource ds)
        {
            return DataSourceUtilities.GetNearestFast(ds, mouseLocation, renderInfo, maxDistance, Axes.XAxis, Axes.YAxis);
        }

        if (Data is IGetNearest gn)
        {
            return gn.GetNearest(mouseLocation, renderInfo, maxDistance);
        }

        throw new NotSupportedException("Signal data must implement IDataSource or IGetNearest to support nearest-point queries.");
    }

    /// <summary>Gets the data point with the nearest X coordinate to a location.</summary>
    /// <param name="mouseLocation">The location to search for, in plot coordinates.</param>
    /// <param name="renderInfo">The render details used to map coordinates.</param>
    /// <param name="maxDistance">The maximum distance at which a point is considered.</param>
    /// <returns>The nearest data point by X coordinate.</returns>
    public DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15)
    {
        if (Data is IDataSource ds)
        {
            return DataSourceUtilities.GetNearestXFast(ds, mouseLocation, renderInfo, maxDistance, Axes.XAxis);
        }

        if (Data is IGetNearest gn)
        {
            return gn.GetNearest(mouseLocation, renderInfo, maxDistance);
        }

        throw new NotSupportedException("Signal data must implement IDataSource or IGetNearest to support nearest-point queries.");
    }

    /// <summary>Checks whether the selected signal index range contains any values.</summary>
    /// <returns><see langword="true"/> when the selected range contains data.</returns>
    private bool HasVisibleValues()
    {
        using IEnumerator<double> values = Data.GetYs(MinRenderIndex, MaxRenderIndex).GetEnumerator();
        return values.MoveNext();
    }

    /// <summary>Gets the horizontal coordinate range currently visible in the plot.</summary>
    /// <param name="dataRect">The visible plot bounds in pixels.</param>
    /// <returns>The visible range in data coordinates.</returns>
    private CoordinateRange GetVisibleXRange(PixelRect dataRect)
    {
        double leftViewCoordinate = Axes.GetCoordinateX(dataRect.Left);
        double rightViewCoordinate = Axes.GetCoordinateX(dataRect.Right);
        return leftViewCoordinate <= rightViewCoordinate
            ? new CoordinateRange(leftViewCoordinate, rightViewCoordinate)
            : new CoordinateRange(rightViewCoordinate, leftViewCoordinate);
    }

    /// <summary>Gets the number of source points represented by a horizontal pixel.</summary>
    /// <returns>The number of points per pixel.</returns>
    private double PointsPerPixel() => GetVisibleXRange(Axes.DataRect).Span / Axes.DataRect.Width / Data.Period;

    /// <summary>Renders individual points when the signal is at low display density.</summary>
    /// <param name="rp">The render context.</param>
    private void RenderLowDensity(RenderPack rp)
    {
        CoordinateRange visibleXRange = GetVisibleXRange(Axes.DataRect);
        int firstIndex = Data.GetIndex(visibleXRange.Min, true);
        int lastIndex = Data.GetIndex(visibleXRange.Max + Data.Period, true);
        List<Pixel> points = [];

        for (int i = firstIndex; i <= lastIndex; i++)
        {
            float x = Axes.GetPixelX(Data.GetX(i));
            float y = Axes.GetPixelY((Data.GetY(i) * Data.YScale) + Data.YOffset);
            points.Add(new(x, y));
        }

        if (points.Count == 0)
        {
            return;
        }

        using SKPathBuilder builder = new();
        builder.MoveTo(points[0].ToSKPoint());
        foreach (Pixel point in points)
        {
            builder.LineTo(point.ToSKPoint());
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);
        double pointsPerPixel = PointsPerPixel();
        if (pointsPerPixel >= 1)
        {
            return;
        }

        rp.Paint.IsStroke = false;
        float radius = (float)Math.Min(Math.Sqrt(Constants.DensityMarkerFactor / pointsPerPixel), MaximumMarkerSize);
        MarkerSize = radius * MaximumMarkerSize * (float)Constants.DensityMarkerFactor;
        Drawing.DrawMarkers(rp.Canvas, rp.Paint, points, MarkerStyle);
    }

    /// <summary>Renders pixel columns containing the signal's vertical extrema at high display density.</summary>
    /// <param name="rp">The render context.</param>
    private void RenderHighDensity(RenderPack rp)
    {
        int pixelColumnCount = (int)Axes.DataRect.Width;
        List<PixelColumn> columns = [];
        for (int pixelColumnIndex = 0; pixelColumnIndex < pixelColumnCount; pixelColumnIndex++)
        {
            PixelColumn column = Data.GetPixelColumn(Axes, pixelColumnIndex);
            if (column.HasData)
            {
                columns.Add(column);
            }
        }

        if (columns.Count == 0)
        {
            return;
        }

        using SKPathBuilder builder = new();
        builder.MoveTo(columns[0].X, columns[0].Enter);
        foreach (PixelColumn column in columns)
        {
            builder.LineTo(column.X, column.Enter);
            builder.MoveTo(column.X, column.Bottom);
            builder.LineTo(column.X, column.Top);
            builder.MoveTo(column.X, column.Exit);
        }

        using SKPath path = builder.Detach();
        Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);
    }

    /// <summary>Contains constants used by signal rendering.</summary>
    internal static class Constants
    {
        /// <summary>The default marker size in pixels.</summary>
        internal const float DefaultMarkerSize = 5;

        /// <summary>The default line width in pixels.</summary>
        internal const float DefaultLineWidth = 1;

        /// <summary>The maximum marker size in pixels.</summary>
        internal const float MaximumMarkerSize = 4;

        /// <summary>The density-dependent marker scale factor.</summary>
        internal const double DensityMarkerFactor = 0.2;
    }
}
