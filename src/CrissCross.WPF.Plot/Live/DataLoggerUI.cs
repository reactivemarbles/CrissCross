// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using Color = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Nice for continuous live data.
/// </summary>
/// <seealso cref="DataLoggerUI" />
public partial class DataLoggerUI : RxObject
{
    private readonly List<double> _data = [];
    private readonly List<double> _time = [];

    [Reactive]
    private Settings _chartSettings = new();

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
    /// Initializes a new instance of the <see cref="DataLoggerUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="points">observer with points.</param>
    public DataLoggerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false, bool points = false)
    {
        ChartSettings.Color = color;
        ChartSettings.ItemName = "---";
        ChartSettings.ColorText = "#FFD3D3D3";
        ChartSettings.OpacityCheckBox = "1";
        ChartSettings.LineWidth = 3;
        ChartSettings.IsVisible = true;
        ChartSettings.IsChecked = false;
        ChartSettings.DisplayedValue = 0;
        ChartSettings.Visibility = "Visible";
        ManualScale = manualscale;
        AutoScale = autoscale;
        ZoomXY = false;

        Plot = plot;
        CreateDataLogger(color);
        AppearanceSubsriptions();

        // TODO: Add with timestamp
        ////UpdateDataLogger(observable);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataLoggerUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="points">observer with points.</param>
    public DataLoggerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, int Axis, int nPoints)> observable, string color, bool autoscale = true, bool manualscale = false, bool points = false)
    {
        ChartSettings.Color = color;
        ChartSettings.ItemName = "---";
        ChartSettings.ColorText = "#FFD3D3D3";
        ChartSettings.OpacityCheckBox = "1";
        ChartSettings.LineWidth = 3;
        ChartSettings.IsVisible = true;
        ChartSettings.IsChecked = false;
        ManualScale = manualscale;
        AutoScale = autoscale;
        ZoomXY = false;
        ChartSettings.DisplayedValue = 0;
        ChartSettings.Visibility = "Visible";

        Plot = plot;
        CreateDataLogger(color);
        UpdateDataLogger(observable);
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
    public DataLogger? DataLogger { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateDataLogger(string color)
    {
        double[] y = [0];
        double[] x = [0];

        DataLogger = Plot.Plot.Add.DataLogger();
        DataLogger.ViewSlide();
        DataLogger.ManageAxisLimits = false;
        DataLogger.LineStyle.Width = 1;
        DataLogger.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateDataLogger(IObservable<(string? Name, IList<double>? Value, int Axis, int nPoints)> observable)
    {
        ////observable.ObserveOn(RxApp.MainThreadScheduler)
        ////    .Subscribe(data =>
            observable
            .SubscribeOn(RxApp.TaskpoolScheduler) // Procesa en un hilo de fondo
            .ObserveOn(RxApp.MainThreadScheduler) // Actualiza la UI en el hilo principal
            .Subscribe(data =>
            {
                // CHECKS
                if (string.IsNullOrEmpty(data.Name) || data.Value == null || data.nPoints <= 0)
                {
                    return;
                }

                if (data.Value.Count == 0)
                {
                    return;
                }

                var nPoints = Math.Min(data.nPoints, 100_000_000);

                if (DataLogger!.Data.Coordinates.Count >= nPoints)
                {
                    DataLogger.Data.Coordinates.Clear();
                    _data.Clear();
                }

                // INSERT DATA INTO SIGNALXY
                if (data.Value != null && data.Name != null)
                {
                    var values = new List<double>(data.Value).ToArray();

                    _data.AddRange(values);

                    DataLogger!.Add(values);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        Plot.Plot.Axes.SetLimitsX(_data.Count - 10000, _data.Count, DataLogger.Axes.XAxis);
                    }
                }

                // UPDATE IF IS NOT PAUSED
                if (!ChartSettings.IsPaused)
                {
                    Plot.Refresh();
                }

                // UPDATE NAME
                ChartSettings.ItemName = data.Name;
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
            DataLogger!.LineStyle.Width = (float)x.Item1;
            DataLogger!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
            ChartSettings.IsChecked = x.Item3 == "Invisible";
            DataLogger.IsVisible = x.Item3 == "Invisible";
            Plot.Refresh();
        }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsChecked)
            .Subscribe(x => ChartSettings.Visibility = x == true ? "Invisible" : "Visible")
            .DisposeWith(Disposables);
    }
}
