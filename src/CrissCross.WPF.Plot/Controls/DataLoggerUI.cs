// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Versioning;
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
[SupportedOSPlatform("windows")]
public partial class DataLoggerUI : RxObject, IPlottableUI
{
    private List<double> _data = [];
    private List<double> _time = [];

    [Reactive]
    private ChartObjects _chartSettings;
    [Reactive]
    private bool _autoScale;
    [Reactive]
    private bool _manualScale;
    [Reactive]
    private int _mode;
    [Reactive]
    private int _numberPointsPlotted;

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
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateDataLogger(color);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);

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
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateDataLogger(color);
        UpdateDataLogger(observable);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
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
    public DataLogger? PlotLine { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateDataLogger(string color)
    {
        double[] y = [0];
        double[] x = [0];

        PlotLine = Plot.Plot.Add.DataLogger();
        PlotLine.ViewSlide();
        PlotLine.ManageAxisLimits = false;
        PlotLine.LineStyle.Width = 1;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateDataLogger(IObservable<(string? Name, IList<double>? Value, int Axis, int nPoints)> observable) =>
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

                if (PlotLine!.Data.Coordinates.Count >= nPoints)
                {
                    PlotLine.Data.Coordinates.Clear();
                    _data.Clear();
                }

                // INSERT DATA INTO SIGNALXY
                if (data.Value != null && data.Name != null)
                {
                    var values = new List<double>(data.Value).ToArray();

                    _data.AddRange(values);

                    PlotLine!.Add(values);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        Plot.Plot.Axes.SetLimitsX(_data.Count - 10000, _data.Count, PlotLine.Axes.XAxis);
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

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ChartSettings.IsCheckedCmd?.Dispose();
            ChartSettings.Dispose();
        }
    }
}
