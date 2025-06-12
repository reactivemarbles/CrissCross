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
    [Reactive]
    private bool _useFixedNumberOfPoints;

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
    public void UpdateDataLogger(IObservable<(string? Name, IList<double>? Value, int Axis, int nPoints)> observable) => observable
        .ObserveOn(RxApp.TaskpoolScheduler)
        .Select(data =>
        {
            var nPoints = Math.Min(data.nPoints, 100_000_000);
            return (data, nPoints);
        })
        .Where(d => !string.IsNullOrEmpty(d.data.Name) && d.data.Value != null && d.data.Value.Count > 0 && d.nPoints > 0)
        .Retry()
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(d =>
        {
            if (PlotLine!.Data.Coordinates.Count >= d.nPoints)
            {
                PlotLine.Data.Coordinates.RemoveRange(0, PlotLine!.Data.Coordinates.Count - d.nPoints);
            }

            try
            {
                PlotLine!.Add(d.data.Value!.ToArray());
            }
            catch
            {
            }

            PlotLine!.ManageAxisLimits = false;

            //// UPDATE IF IS NOT PAUSED
            if (!ChartSettings.IsPaused)
            {
                try
                {
                    Plot.Refresh();
                }
                catch
                {
                }
            }

            // UPDATE NAME
            ChartSettings.ItemName = d.data.Name;
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
