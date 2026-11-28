// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.AxisLimitManagers;
using CrissCross.Plot.DataSources;

namespace CrissCross.Plot.Plottables;

/// <summary>Plots a dynamically updated sequence of coordinate data.</summary>
/// <param name="coordinates">The initial coordinates, or null to start with an empty sequence.</param>
public class DataLogger(List<Coordinates>? coordinates = null) : IPlottable, IManagesAxisLimits, IHasLine, IHasMarker, IHasLegendText
{
    /// <summary>Gets the underlying data source.</summary>
    public DataLoggerSource Data { get; } = new(coordinates ?? []);

    /// <summary>Gets or sets whether this plottable is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this plottable.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the strategy used to calculate axis limits.</summary>
    public IAxisLimitManager AxisManager { get; set; } = new Full();

    /// <summary>Gets or sets whether the X and Y coordinates are exchanged.</summary>
    public bool Rotated { get; set; }

    /// <summary>Gets or sets the line style used to connect data points.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the marker style used for data points.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { Size = 0, Shape = MarkerShape.FilledCircle };

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets both the marker fill and line colors.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets whether data has been added since the last render.</summary>
    public bool HasNewData => Data.HasNewData;

    /// <summary>Gets or sets whether the horizontal axis is inverted.</summary>
    public bool InvertX { get; set; }

    /// <summary>Gets or sets whether the vertical axis is inverted.</summary>
    public bool InvertY { get; set; }

    /// <summary>Gets or sets the style of lines to use when connecting points.</summary>
    public ConnectStyle ConnectStyle { get; set; } = ConnectStyle.Straight;

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

    /// <summary>Gets or sets the default X increment when adding Y values.</summary>
    public double Period { get; set; } = 1.0;

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained to preserve the established public API.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets the legend item for this data logger.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle, MarkerStyle);

    /// <summary>Gets or sets whether this plottable manages its axes limits.</summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>Gets the axis limits required to display the current data.</summary>
    /// <returns>The limits occupied by the data.</returns>
    public AxisLimits GetAxisLimits()
    {
        CoordinateRange rangeX = Data.GetRangeX();
        CoordinateRange rangeY = Data.GetRangeY(rangeX);

        return Rotated
            ? new AxisLimits(rangeY, rangeX)
            : new AxisLimits(rangeX, rangeY);
    }

    /// <summary>Gets the data point nearest to the specified mouse location.</summary>
    /// <param name="mouseLocation">The mouse location in data coordinates.</param>
    /// <param name="dataRect">The data rectangle from the render details.</param>
    /// <param name="maxDistance">The maximum distance to the point in pixels.</param>
    /// <returns>
    /// The nearest data point, or <see cref="DataPoint.None"/> when no point is within the maximum distance.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309:Optional public API parameters",
        Justification = "The optional default preserves the established public call signature.")]
    public DataPoint GetNearest(Coordinates mouseLocation, PixelRect dataRect, float maxDistance = 15)
    {
        double pixelsPerUnitX = dataRect.Width / Axes.XAxis.GetRange().Span;
        double pixelsPerUnitY = dataRect.Height / Axes.YAxis.GetRange().Span;

        double maxDistanceSquared = maxDistance * maxDistance;
        double closestDistanceSquared = double.PositiveInfinity;

        int closestIndex = 0;
        double closestX = double.PositiveInfinity;
        double closestY = double.PositiveInfinity;

        for (int i = 0; i < Data.Coordinates.Count; i++)
        {
            double x = Data.Coordinates[i].X + Data.XOffset;
            double y = (Data.Coordinates[i].Y * Data.YScale) + Data.YOffset;

            if (Rotated)
            {
                (x, y) = (y, x);
            }

            double deltaX = (x - mouseLocation.X) * pixelsPerUnitX;
            double deltaY = (y - mouseLocation.Y) * pixelsPerUnitY;
            double distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

            if (distanceSquared <= closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closestX = x;
                closestY = y;
                closestIndex = i;
            }
        }

        if (closestDistanceSquared > maxDistanceSquared)
        {
            return DataPoint.None;
        }

        return Rotated
            ? new DataPoint(closestY, closestX, closestIndex)
            : new DataPoint(closestX, closestY, closestIndex);
    }

    /// <summary>Gets the data point nearest to the specified mouse location, considering only X values.</summary>
    /// <param name="mouseLocation">The mouse location in data coordinates.</param>
    /// <param name="dataRect">The data rectangle from the render details.</param>
    /// <param name="maxDistance">The maximum horizontal distance to the point in pixels.</param>
    /// <returns>
    /// The nearest data point by X, or <see cref="DataPoint.None"/> when no point is within the maximum distance.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309:Optional public API parameters",
        Justification = "The optional default preserves the established public call signature.")]
    public DataPoint GetNearestX(Coordinates mouseLocation, PixelRect dataRect, float maxDistance = 15)
    {
        double pixelsPerUnit;

        if (Rotated)
        {
            mouseLocation = mouseLocation.Rotated;
            pixelsPerUnit = dataRect.Height / Axes.YAxis.GetRange().Span;
        }
        else
        {
            pixelsPerUnit = dataRect.Width / Axes.XAxis.GetRange().Span;
        }

        int i = Data.GetIndex(mouseLocation.X); // TODO: check the index after too?
        double distance = (Data.Coordinates[i].X + Data.XOffset - mouseLocation.X) * pixelsPerUnit;
        return distance <= maxDistance
            ? new DataPoint(Data.Coordinates[i].X, Data.Coordinates[i].Y, i)
            : DataPoint.None;
    }

    /// <summary>Renders the current data points and marks the source as rendered.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    public virtual void Render(RenderPack rp)
    {
        Pixel[] markerPixels = Rotated
            ? Data.GetPixelsToDrawVertically(rp, Axes, ConnectStyle)
            : Data.GetPixelsToDrawHorizontally(rp, Axes, ConnectStyle);

        IReadOnlyList<Pixel> linePixels = ConnectStyle switch
        {
            ConnectStyle.Straight => markerPixels,
            ConnectStyle.StepHorizontal => Scatter.GetStepDisplayPixels(markerPixels, true),
            ConnectStyle.StepVertical => Scatter.GetStepDisplayPixels(markerPixels, false),
            _ => throw new NotSupportedException($"unsupported {nameof(ConnectStyle)}: {ConnectStyle}"),
        };

        Drawing.DrawLines(rp.Canvas, rp.Paint, linePixels, LineStyle);
        Drawing.DrawMarkers(rp.Canvas, rp.Paint, markerPixels, MarkerStyle);

        Data.OnRendered();
    }

    /// <summary>Adds one coordinate to the data source.</summary>
    /// <param name="coordinates">The coordinate to add.</param>
    public void Add(Coordinates coordinates) => Data.Add(coordinates);

    /// <summary>Adds one X and Y coordinate pair to the data source.</summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public void Add(double x, double y) => Add(new Coordinates(x, y));

    /// <summary>Adds one Y value using the configured period to determine its X coordinate.</summary>
    /// <param name="y">The Y value to add.</param>
    public void Add(double y)
    {
        double x = Data.Coordinates.Count == 0 ? 0 : Data.Coordinates[^1].X + Period;
        Add(x, y);
    }

    /// <summary>Adds Y values using the configured period to determine their X coordinates.</summary>
    /// <param name="ys">The Y values to add.</param>
    public void Add(IEnumerable<double> ys)
    {
        foreach (double y in ys)
        {
            Add(y);
        }
    }

    /// <summary>Adds matching X and Y values to the data source.</summary>
    /// <param name="xs">The X coordinates.</param>
    /// <param name="ys">The Y coordinates.</param>
    public void Add(double[] xs, double[] ys)
    {
        if (xs is null || ys is null)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must not be null");
        }

        if (xs.Length != ys.Length)
        {
            throw new ArgumentException($"{nameof(xs).Length} and {nameof(ys).Length} must have equal length");
        }

        for (int i = 0; i < xs.Length; i++)
        {
            Add(xs[i], ys[i]);
        }
    }

    /// <summary>Adds a sequence of coordinates to the data source.</summary>
    /// <param name="coordinates">The coordinates to add.</param>
    public void Add(Coordinates[] coordinates)
    {
        if (coordinates is null)
        {
            throw new ArgumentException($"{coordinates} must not be null");
        }

        for (int i = 0; i < coordinates.Length; i++)
        {
            Add(coordinates[i]);
        }
    }

    /// <summary>Removes all coordinates from the data source.</summary>
    public void Clear() => Data.Clear();

    /// <summary>Updates the axes limits to include the data managed by this plottable.</summary>
    /// <param name="plot">The plot whose axes are updated.</param>
    public void UpdateAxisLimits(Plot plot)
    {
        bool firstTimeRenderingData = !Data.WasRendered;

        IAxis horizontalAxis = Rotated ? Axes.YAxis : Axes.XAxis;
        IAxis verticalAxis = Rotated ? Axes.XAxis : Axes.YAxis;

        CoordinateRange dataRangeX = Data.GetRangeX();
        CoordinateRange viewRangeX = firstTimeRenderingData
            ? dataRangeX
            : horizontalAxis.GetRange().Rectified();

        CoordinateRange newRangeX = AxisManager.GetRangeX(viewRangeX, dataRangeX);

        // Get the Y range only for the newly calculated X range
        CoordinateRange dataRangeY = Data.GetRangeY(newRangeX);
        CoordinateRange viewRangeY = firstTimeRenderingData
            ? dataRangeY
            : verticalAxis.GetRange().Rectified();

        CoordinateRange newRangeY = AxisManager.GetRangeY(viewRangeY, dataRangeY);

        if (Rotated)
        {
            (newRangeX, newRangeY) = (newRangeY, newRangeX);
        }

        if (InvertX)
        {
            newRangeX = new(newRangeX.Max, newRangeX.Min);
        }

        if (InvertY)
        {
            newRangeY = new(newRangeY.Max, newRangeY.Min);
        }

        AxisLimits newLimits = new(newRangeX, newRangeY);

        plot.Axes.SetLimits(newLimits, Axes.XAxis, Axes.YAxis);
    }

    /// <summary>Automatically expands the axis as needed to display the full dataset before each render.</summary>
    public void ViewFull()
    {
        ManageAxisLimits = true;
        AxisManager = new Full();
        Data.WasRendered = false;
    }

    /// <summary>Tracks incoming data by jumping the axis limits when it reaches the visible edge.</summary>
    /// <param name="width">The visible horizontal range.</param>
    /// <param name="paddingFraction">The fraction of the range retained as padding before the jump.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309:Optional public API parameters",
        Justification = "The optional defaults preserve the established public call signature.")]
    public void ViewJump(double width = 1000, double paddingFraction = .5)
    {
        ManageAxisLimits = true;
        AxisManager = new Slide { Width = width, PaddingFractionX = paddingFraction };
        Data.WasRendered = false;
    }

    /// <summary>Tracks incoming data by sliding the axis limits continuously.</summary>
    /// <param name="width">The visible horizontal range.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309:Optional public API parameters",
        Justification = "The optional default preserves the established public call signature.")]
    public void ViewSlide(double width = 1000)
    {
        ManageAxisLimits = true;
        AxisManager = new Slide { Width = width, PaddingFractionX = 0 };
        Data.WasRendered = false;
    }
}
