// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.AxisLimitManagers;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// EquationData.
/// </summary>
/// <seealso cref="StreamerUI" />
public partial class StreamerUI : RxObject
{
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
    /// Initializes a new instance of the <see cref="StreamerUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    public StreamerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false)
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

        Plot = plot;
        ChartSettings.DisplayedValue = 0;
        CreateStream(color);
        UpdateStream(observable);
        AppearanceSubsriptions();
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
    public DataLogger? Streamer { get; set; }

    /// <summary>
    /// Gets or sets the streamer.
    /// </summary>
    /// <value>
    /// The streamer.
    /// </value>
    public SignalXY? SignalXY { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateSignal(string color)
    {
        double[] y = [0];
        double[] x = [0];
        SignalXY = Plot.Plot.Add.SignalXY(x, y);
        SignalXY.LineStyle.Width = 3f;
        SignalXY.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateStream(string color)
    {
        if (Streamer == null)
        {
            Streamer = Plot.Plot.Add.DataLogger();
            Streamer.AxisManager = new Slide
            {
                Width = 10,
                PaddingFractionX = 0.0,
            };

            // configure line
            Streamer.LineStyle.Width = 3f;
            Streamer.Color = Color.FromHex(color);
        }
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateStream(IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable)
    {
        observable.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(data =>
            {
                // CALCULATE TIMESPAN TO PLOT
                DateTime now = new(Convert.ToInt64(data.DateTime.Last()));
                var doublenow = now.ToOADate();
                var limits = now.Add(TimeSpan.FromMinutes(-60));
                var doublelimits = limits.ToOADate();

                // INSERT DATA INTO STREAMER
                if (data.Value != null && data.Value.Count == data.DateTime.Count && data.Name != null)
                {
                    var values = new List<double>(data.Value);
                    var datetime = new List<double>(data.DateTime.ToList().ConvertAll(x => new DateTime(Convert.ToInt64(x)).ToOADate()));
                    var coord = datetime.Zip(values, (d, v) => new Coordinates(d, v)).ToArray();
                    ChartSettings.ItemName = data.Name;
                    Streamer!.Data.Clear();
                    Streamer.Add(coord);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, Streamer.Axes.XAxis);
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
        }
    }

    private void AppearanceSubsriptions()
    {
        this.WhenAnyValue(x => x.ChartSettings.LineWidth, x => x.ChartSettings.Color, x => x.ChartSettings.Visibility).Subscribe(x =>
        {
            Streamer!.LineStyle.Width = (float)x.Item1;
            Streamer!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2));
            ChartSettings.IsChecked = x.Item3 == "Invisible";
            Streamer.IsVisible = x.Item3 != "Invisible";
            Plot.Refresh();
        }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsChecked)
            .Subscribe(x => ChartSettings.Visibility = x ? "Invisible" : "Visible")
            .DisposeWith(Disposables);
    }
}
