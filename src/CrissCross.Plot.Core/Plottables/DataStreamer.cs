// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.AxisLimitManagers;
using CrissCross.Plot.DataSources;
using CrissCross.Plot.DataViews;

namespace CrissCross.Plot.Plottables;

/// <summary>Displays a fixed-length stream of values that can be updated as new data arrives.</summary>
public class DataStreamer : IPlottable, IManagesAxisLimits, IHasLine, IHasLegendText, IHasMarker
{
    /// <summary>The alpha fraction used for the default fill colors.</summary>
    private const double FillTransparency = 0.2;

    /// <summary>The plot whose axis limits are adjusted by this plottable.</summary>
    private readonly Plot _plot;

    /// <summary>Initializes a new instance of the <see cref="DataStreamer"/> class.</summary>
    /// <param name="plot">The plot whose axes are managed by this streamer.</param>
    /// <param name="data">The initial fixed-length data buffer.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2403:This escapes during construction",
        Justification = "The wipe renderer must retain this streamer to read and render its live buffer.")]
    public DataStreamer(Plot plot, double[] data)
    {
        _plot = plot;
        Data = new(data);
        Renderer = new Wipe(this, true);
    }

    /// <summary>Gets or sets whether this plottable is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this plottable.</summary>
    public IAxes Axes { get; set; } = CrissCross.Plot.Axes.Default;

    /// <summary>Gets the legend item for this data streamer.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle);

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained to preserve the established public API.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the line style used to render the data.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the marker style for data points.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new(MarkerShape.FilledCircle, 0);

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

    /// <summary>Gets or sets whether the area relative to <see cref="FillYValue"/> is filled.</summary>
    public bool FillY { get; set; }

    /// <summary>Gets or sets the Y coordinate used as the fill baseline.</summary>
    public double FillYValue { get; set; }

    /// <summary>Gets or sets the fill color above the baseline.</summary>
    public Color FillYAboveColor { get; set; } = Colors.Blue.WithAlpha(FillTransparency);

    /// <summary>Gets or sets the fill color below the baseline.</summary>
    public Color FillYBelowColor { get; set; } = Colors.Blue.WithAlpha(FillTransparency);

    /// <summary>Gets or sets both fill colors.</summary>
    public Color FillYColor
    {
        get => FillYAboveColor;
        set
        {
            FillYAboveColor = value;
            FillYBelowColor = value;
        }
    }

    /// <summary>Gets or sets the shared line and marker color.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineColor = value;
            MarkerFillColor = value;
        }
    }

    /// <summary>Gets or sets the fixed-length data source.</summary>
    public DataStreamerSource Data { get; set; }

    /// <summary>Gets the number of values currently in the data buffer.</summary>
    public int Count => Data.Length;

    /// <summary>Gets the total number of values added to the data source.</summary>
    public int CountTotal => Data.CountTotal;

    /// <summary>Gets or sets the sample period used to space incoming values.</summary>
    public double Period { get => Data.SamplePeriod; set => Data.SamplePeriod = value; }

    /// <summary>Gets whether data has been added since the last render.</summary>
    public bool HasNewData => Data.CountTotal != Data.CountTotalOnLastRender;

    /// <summary>Gets or sets whether axis limits adjust automatically when data leaves the view.</summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>Gets or sets the obsolete continuous autoscaling option.</summary>
    [Obsolete("set Plot.Axes.ContinuouslyAutoscale", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained as an obsolete error to preserve the established public API.")]
    public bool ContinuouslyAutoscale { get; set; }

    /// <summary>Gets or sets the view that displays the fixed-length Y data in <see cref="Data"/>.</summary>
    public IDataStreamerView Renderer { get; set; }

    /// <summary>
    /// Gets or sets contains logic for automatically adjusting axis limits if new data runs off the screen.
    /// Only used if <see cref="ManageAxisLimits"/> is true.
    /// </summary>
    private IAxisLimitManager AxisManager { get; } = new FixedWidth();

    /// <summary>Adds a Y value to the data stream.</summary>
    /// <param name="value">The value to add.</param>
    public void Add(double value) => Data.Add(value);

    /// <summary>Adds several Y values to the data stream.</summary>
    /// <param name="ys">The values to add.</param>
    public void Add(double[] ys)
    {
        if (ys is null)
        {
            throw new ArgumentException($"{nameof(ys)} must not be null");
        }

        for (int i = 0; i < ys.Length; i++)
        {
            Data.Add(ys[i]);
        }
    }

    /// <summary>Adds a collection of Y values to the data stream.</summary>
    /// <param name="values">The values to add.</param>
    public void AddRange(IEnumerable<double> values) => Data.AddRange(values);

    /// <summary>Clears the buffer by setting all Y points to the specified value.</summary>
    /// <param name="value">The value assigned to each buffer position.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309:Optional public API parameters",
        Justification = "The optional default preserves the established public call signature.")]
    public void Clear(double value = 0) => Data.Clear(value);

    /// <summary>Displays data with new values overlapping old values from left to right.</summary>
    /// <param name="blankFraction">The fraction of the view left blank before new values are drawn.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309:Optional public API parameters",
        Justification = "The optional default preserves the established public call signature.")]
    public void ViewWipeRight(double blankFraction = 0) => Renderer = new Wipe(this, true) { BlankFraction = blankFraction };

    /// <summary>Displays data with new values overlapping old values from right to left.</summary>
    public void ViewWipeLeft() => Renderer = new Wipe(this, false);

    /// <summary>Displays data while continuously shifting the newest values to the right.</summary>
    public void ViewScrollLeft() => Renderer = new Scroll(this, true);

    /// <summary>Displays data while continuously shifting the newest values to the left.</summary>
    public void ViewScrollRight() => Renderer = new Scroll(this, false);

    /*
    // TODO: slide axes
    /// <summary>
    /// Display the data using a view that continuously shifts data to the left, 
    /// placing the newest data on the right, and sliding the horizontal axis
    /// to track the latest data coming in.
    /// </summary>
    public void ViewSlideLeft()
    {
        Renderer = new DataViews.Scroll(this, true);
    }
    */

    /// <summary>Displays data using a custom rendering view.</summary>
    /// <param name="view">The view used to render the data.</param>
    public void ViewCustom(IDataStreamerView view) => Renderer = view;

    /// <summary>Gets the axis limits occupied by the data.</summary>
    /// <returns>The calculated data limits.</returns>
    public AxisLimits GetAxisLimits() => Data.GetAxisLimits();

    /// <summary>Updates the axes limits when the streamed data changes.</summary>
    /// <param name="plot">The plot whose axes are updated.</param>
    public void UpdateAxisLimits(Plot plot)
    {
        if (Data.CountTotal == 0)
        {
            return;
        }

        AxisLimits limits = _plot.Axes.GetLimits(Axes);
        AxisLimits dataLimits = GetAxisLimits();
        AxisLimits newLimits = AxisManager.GetAxisLimits(limits, dataLimits);
        _plot.Axes.SetLimits(newLimits, Axes.XAxis, Axes.YAxis);
    }

    /// <summary>Renders the current data buffer and updates its rendered-data state.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    public virtual void Render(RenderPack rp)
    {
        if (Renderer is not IDataStreamerView viewWithSegs)
        {
            Renderer.Render(rp);
            return;
        }

        var segs = viewWithSegs.GetSegments(rp);
        if (segs.Count == 0)
        {
            return;
        }

        if (FillY)
        {
            float baselinePixelY = Axes.YAxis.GetPixel(FillYValue + Data.OffsetY, rp.DataRect);

            foreach (Pixel[] seg in segs)
            {
                using SKPathBuilder builder = new();
                builder.MoveTo(seg[0].X, seg[0].Y);
                for (int i = 1; i < seg.Length; i++)
                {
                    builder.LineTo(seg[i].X, seg[i].Y);
                }

                PixelRect rect = new(seg);
                builder.LineTo(rect.Right, baselinePixelY);
                builder.LineTo(rect.Left, baselinePixelY);
                using SKPath fill = builder.Detach();

                // above
                if (baselinePixelY > rect.Top)
                {
                    PixelRect clip = new(rp.DataRect.Left, rp.DataRect.Right, rp.DataRect.Top, baselinePixelY);
                    FillStyle fs = new() { IsVisible = true, Color = FillYAboveColor };
                    rp.CanvasState.Save();
                    rp.CanvasState.Clip(clip);
                    Drawing.FillPath(rp.Canvas, rp.Paint, fill, fs, clip);
                    rp.CanvasState.Restore();
                }

                // below
                if (baselinePixelY < rect.Bottom)
                {
                    PixelRect clip = new(rp.DataRect.Left, rp.DataRect.Right, baselinePixelY, rp.DataRect.Bottom);
                    FillStyle fs = new() { IsVisible = true, Color = FillYBelowColor };
                    rp.CanvasState.Save();
                    rp.CanvasState.Clip(clip);
                    Drawing.FillPath(rp.Canvas, rp.Paint, fill, fs, clip);
                    rp.CanvasState.Restore();
                }
            }
        }

        Renderer.Render(rp);

        Data.CountTotalOnLastRender = Data.CountTotal;
    }
}
