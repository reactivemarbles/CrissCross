// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using CrissCross;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Nice for historical data (performance).
/// </summary>
/// <seealso cref="SignalUI" />
public partial class SignalUI : RxObject
{
    private List<double> _data = [];
    private List<double> _time = [];

    [Reactive]
    private Settings _chartSettings = new Settings();

    /// <summary>
    /// Gets or sets a value indicating whether [automatic scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic scale]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _autoScale;

    /// <summary>
    /// Gets or sets a value indicating whether [manual scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [manual scale]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _manualScale;

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    [Reactive]
    private int _mode;

    /// <summary>
    /// Gets or sets the number points plotted.
    /// </summary>
    /// <value>
    /// The number points plotted.
    /// </value>
    [Reactive]
    private int _numberPointsPlotted;

    /// <summary>
    /// Gets or sets a value indicating whether [select area].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [select area]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _selectArea;

    /// <summary>
    /// Gets or sets a value indicating whether [zoom xy].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [zoom xy]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _zoomXY;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalUI" /> class.
    /// </summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="coordinatesObs">The coordinates obs.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="fixedPoints">if set to <c>true</c> [fixed points].</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color,
        bool autoscale = true,
        bool manualscale = false,
        bool fixedPoints = false)
    {
        ChartSettings.Color = color;
        ChartSettings.ItemName = "---";
        ChartSettings.ColorText = "#FFD3D3D3";
        ChartSettings.OpacityCheckBox = "1";
        ChartSettings.LineWidth = 3;
        ChartSettings.IsVisible = true;
        ManualScale = manualscale;
        AutoScale = autoscale;
        ZoomXY = false;
        ChartSettings.IsChecked = true;
        Crosshair = new Crosshair();
        Marker = new Marker();
        MarkerText = new Text();

        NumberPointsPlotted = 1024;
        Plot = plot;
        ChartSettings.DisplayedValue = 0;
        if (fixedPoints)
        {
            CreateStreamer(color);
            UpdateStreamerFixedPoints(observable);
            AppearanceSubsriptionsStreamer();
            CreateCursorValues();
        }
        else
        {
            CreateSignal(color);
            UpdateSignal(observable);
            AppearanceSubsriptions();
            CreateCursorValues();
            MouseCoordinatesObs = coordinatesObs.Subscribe(x =>
                {
                    var coord = DataLogger!.GetNearestX(x, Plot.Plot.LastRender.DataRect).Coordinates;
                    Marker.Coordinates = x;
                    Crosshair.Position = coord;
                    Marker.Coordinates = coord;
                }).DisposeWith(Disposables);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="data">The data.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    public SignalUI(WpfPlot plot, (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data, string color, bool autoscale = true, bool manualscale = false)
    {
        ChartSettings.Color = color;
        ChartSettings.ItemName = "---";
        ChartSettings.ColorText = "#FFD3D3D3";
        ChartSettings.OpacityCheckBox = "1";
        ChartSettings.LineWidth = 3;
        ChartSettings.IsVisible = true;
        ManualScale = manualscale;
        AutoScale = autoscale;
        ZoomXY = false;
        ChartSettings.IsChecked = true;
        Crosshair = new Crosshair();
        Marker = new Marker();
        MarkerText = new Text();
        Plot = plot;
        ChartSettings.DisplayedValue = 0;
        CreateSignal(color);
        SignalXY!.Data = new SignalXYSourceDoubleArray(data.Value.ToArray(), data.DateTime.ToArray());
        AppearanceSubsriptions();
        CreateCursorValues();
    }

    /// <summary>
    /// Gets or sets the plot.
    /// </summary>
    /// <value>
    /// The plot.
    /// </value>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the streamer.
    /// </summary>
    /// <value>
    /// The streamer.
    /// </value>
    public SignalXY? SignalXY { get; set; }

    /// <summary>
    /// Gets or sets the streamer.
    /// </summary>
    /// <value>
    /// The streamer.
    /// </value>
    public DataLogger? DataLogger { get; set; }

    /// <summary>
    /// Gets or sets the streamer.
    /// </summary>
    /// <value>
    /// The streamer.
    /// </value>
    public DataStreamer? Streamer { get; set; }

    /// <summary>
    /// Gets or sets the marker.
    /// </summary>
    /// <value>
    /// The marker.
    /// </value>
    public Crosshair Crosshair { get; set; }
    /// <summary>
    /// Gets or sets the marker.
    /// </summary>
    /// <value>
    /// The marker.
    /// </value>
    public Marker Marker { get; set; }

    /// <summary>
    /// Gets or sets the marker text.
    /// </summary>
    /// <value>
    /// The marker text.
    /// </value>
    public Text MarkerText { get; set; }

    /// <summary>
    /// Gets or sets the mouse coordinates.
    /// </summary>
    /// <value>
    /// The mouse coordinates.
    /// </value>
    public IDisposable MouseCoordinatesObs { get; set; }

    /// <summary>
    /// Creates the cursor values.
    /// </summary>
    public void CreateCursorValues()
    {
        // Create a crosshair to highlight the point under the cursor
        Crosshair = Plot!.Plot.Add.Crosshair(0, 0);

        Crosshair.IsVisible = false;

        // Create a marker to highlight the point under the cursor
        Marker = Plot!.Plot.Add.Marker(0, 0);
        Marker.Shape = MarkerShape.OpenCircle;
        Marker.Size = 17;
        Marker.LineWidth = 2;
        Marker.IsVisible = false;

        // Create a text label to place near the highlighted value
        MarkerText = Plot!.Plot.Add.Text(" ", 0, 0);
        MarkerText.LabelAlignment = Alignment.LowerLeft;
        MarkerText.LabelBold = true;
        MarkerText.OffsetX = 7;
        MarkerText.OffsetY = -7;
        MarkerText.IsVisible = false;
    }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateSignal(string color)
    {
        double[] y = [0];
        double[] x = [0];
        ////SignalXY = Plot.Plot.Add.SignalXY(x, y);
        ////SignalXY.LineStyle.Width = 3f;
        ////SignalXY.Color = Color.FromHex(color);

        DataLogger = Plot.Plot.Add.DataLogger();
        DataLogger.ViewSlide();
        DataLogger.ManageAxisLimits = false;
        DataLogger.LineStyle.Width = 1;
        DataLogger.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateStreamer(string color)
    {
        double[] y = [0];
        double[] x = [0];

        Streamer = Plot.Plot.Add.DataStreamer(NumberPointsPlotted);
        Streamer.ViewScrollLeft();
        Streamer.Period = 32000 / 2048;
        Streamer.LineStyle.Width = 1f;
        Streamer.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateSignal(IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable)
    {
        observable.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(data =>
            {
                if (string.IsNullOrEmpty(data.Name) || data.Value == null || data.DateTime == null)
                {
                    return;
                }

                if (data.Value.Count == 0 || data.DateTime.Count == 0)
                {
                    return;
                }

                // CALCULATE TIMESPAN TO PLOT
                ////option 1: with timestamps
                var oaDate = data.DateTime.Last();
                var now = System.DateTime.FromOADate(oaDate);
                var doublenow = now.ToOADate();
                var limits = now.Add(TimeSpan.FromMinutes(-0.1));
                var doublelimits = limits.ToOADate();

                ////////// option 2: with ticks
                ////System.DateTime now = new(Convert.ToInt64(data.DateTime.Last()));
                ////var doublenow = now.ToOADate();
                ////var limits = now.Add(TimeSpan.FromMinutes(-0.1));
                ////var doublelimits = limits.ToOADate();

                // INSERT DATA INTO SIGNALXY
                if (data.Value != null && data.Value.Count == data.DateTime.Count && data.Name != null)
                {
                    var values = new List<double>(data.Value).ToArray();

                    // option 1: with timestamp
                    var datetime = data.DateTime.ToArray();

                    // option 2: with ticks
                    ////var datetime = new List<double>(data.DateTime.ToList().ConvertAll(x => new System.DateTime(Convert.ToInt64(x)).ToOADate())).ToArray();

                    var combinedValues = values.Zip(datetime, (v, d) => (v, d));

                    ////var values = new List<double>(data.Value).ToArray();
                    ////var datetime = new List<double>(data.DateTime.ToList().ConvertAll(x => new System.DateTime(Convert.ToInt64(x)).ToOADate())).ToArray();
                    var uniqueValues = combinedValues.Where(item => !_time.Contains(item.d)).OrderBy(x => x.d).ToList();
                    var uniqueDataValues = uniqueValues.Select(x => x.v).ToArray();
                    var uniqueTimeValues = uniqueValues.Select(x => x.d).ToArray();

                    _data.AddRange(uniqueDataValues);
                    _time.AddRange(uniqueTimeValues);
                    ////SignalXY!.Data = new SignalXYSourceDoubleArray(_time.ToArray(), _data.ToArray());

                    DataLogger!.Add(uniqueTimeValues, uniqueDataValues);

                    ////DataLogger!.Add(uniqueDataValues);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        ////Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, SignalXY.Axes.XAxis);
                        Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, DataLogger.Axes.XAxis);
                    }
                }

                // UPDATE IF IS NOT PAUSED
                if (!ChartSettings.IsPaused)
                {
                    Plot.Refresh();
                }

                // UPDATE NAME
                ChartSettings.ItemName = Name;
            }).DisposeWith(Disposables);
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateStreamerFixedPoints(IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> observable)
    {
        observable.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(data =>
            {
                if (string.IsNullOrEmpty(data.Name) || data.Y == null || data.X == null)
                {
                    return;
                }

                if (data.Y.Count == 0 || data.X.Count == 0)
                {
                    return;
                }

                ////// CALCULATE TIMESPAN TO PLOT
                ////////option 1: with timestamps
                ////var oaDate = data.DateTime.Last();
                ////System.DateTime now = System.DateTime.FromOADate(oaDate);
                ////var doublenow = now.ToOADate();
                ////var limits = now.Add(TimeSpan.FromMinutes(-0.1));
                ////var doublelimits = limits.ToOADate();

                ////////////// option 2: with ticks
                ////////System.DateTime now = new(Convert.ToInt64(data.DateTime.Last()));
                ////////var doublenow = now.ToOADate();
                ////////var limits = now.Add(TimeSpan.FromMinutes(-0.1));
                ////////var doublelimits = limits.ToOADate();

                // INSERT DATA INTO STREAMER
                if (data.Y != null && data.Y.Count == data.X.Count && data.Name != null)
                {
                    var values = new List<double>(data.Y).Take(NumberPointsPlotted).ToArray();

                    Streamer!.AddRange(values!);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        ////Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, SignalXY.Axes.XAxis);
                        ////Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, Streamer.Axes.XAxis);
                    }
                }

                // UPDATE IF IS NOT PAUSED
                if (!ChartSettings.IsPaused)
                {
                    Plot.Refresh();
                }

                // UPDATE NAME
                ChartSettings.ItemName = Name;
            }).DisposeWith(Disposables);
    }

    /////// <summary>
    /////// Updates the stream.
    /////// </summary>
    /////// <param name="observable">The observable.</param>
    ////public void UpdateSignalPoints(IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable)
    ////{
    ////    observable.ObserveOn(RxApp.MainThreadScheduler)
    ////        .Subscribe(data =>
    ////        {
    ////            // CALCULATE TIMESPAN TO PLOT
    ////            ////DateTime now = new(Convert.ToInt64(data.DateTime.Last()));
    ////            ////var doublenow = now.ToOADate();
    ////            ////var limits = now.Add(TimeSpan.FromMinutes(-60));
    ////            ////var doublelimits = limits.ToOADate();

    ////            // INSERT DATA INTO SIGNALXY
    ////            if (data.Value != null && data.Value.Count == data.DateTime.Count && data.Name != null)
    ////            {
    ////                var values = new List<double>(data.Value).ToArray();
    ////                var datetime = new List<double>(data.DateTime.ToList().ConvertAll(x => new DateTime(Convert.ToInt64(x)).ToOADate())).ToArray();
    ////                ItemName = data.Name;
    ////                SignalXY!.Data = new SignalXYSourceDoubleArray(datetime, values);

    ////                // UPDATE X AXIS
    ////                if (ManualScale || AutoScale)
    ////                {
    ////                    Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, SignalXY.Axes.XAxis);
    ////                }
    ////            }

    ////            // UPDATE IF IS NOT PAUSED
    ////            if (!IsPaused)
    ////            {
    ////                Plot.Refresh();
    ////            }
    ////        }).DisposeWith(Disposables);
    ////}

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ChartSettings.Dispose();
            _chartSettings.Dispose();
            MouseCoordinatesObs.Dispose();
        }
    }

    private void AppearanceSubsriptions()
    {
        this.WhenAnyValue(x => x.ChartSettings.LineWidth, x => x.ChartSettings.Color, x => x.ChartSettings.Visibility).Subscribe(x =>
        {
            DataLogger!.LineStyle.Width = (float)x.Item1;
            DataLogger!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2));
            ChartSettings.IsChecked = x.Item3 == "Invisible" ? true : false;
            DataLogger.IsVisible = x.Item3 == "Invisible" ? false : true;
            Plot.Refresh();
        }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsChecked).Subscribe(x =>
        {
            ChartSettings.Visibility = x == true ? "Invisible" : "Visible";
        }).DisposeWith(Disposables);
    }

    private void AppearanceSubsriptionsStreamer()
    {
        this.WhenAnyValue(x => x.ChartSettings.LineWidth, x => x.ChartSettings.Color, x => x.ChartSettings.Visibility).Subscribe(x =>
        {
            Streamer!.LineStyle.Width = (float)x.Item1;
            Streamer!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2));
            ChartSettings.IsChecked = x.Item3 == "Invisible" ? true : false;
            Streamer.IsVisible = x.Item3 == "Invisible" ? false : true;
            Crosshair.MarkerLineColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2));
            Plot.Refresh();
        }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsChecked).Subscribe(x =>
        {
            ChartSettings.Visibility = x == true ? "Invisible" : "Visible";
        }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsCrossHairVisible).Subscribe(x =>
        {
            Marker.IsVisible = x;
            Crosshair.IsVisible = x;
            MarkerText.IsVisible = x;
        }).DisposeWith(Disposables);
    }
}
