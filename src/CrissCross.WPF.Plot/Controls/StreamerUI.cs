// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// EquationData.
/// </summary>
/// <seealso cref="StreamerUI" />
[SupportedOSPlatform("windows")]
public partial class StreamerUI : RxObject, IPlottableUI
{
    [Reactive]
    private ChartObjects _chartSettings = new();
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
    private uint _nSamples = 1;
    private int _fs = 1;
    private int _numberPointsPlottedSaved;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamerUI"/> class.
    /// </summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="fs">The fs.</param>
    /// <param name="nSamples">The n samples.</param>
    /// <param name="nPointsPlotted">The n points plotted.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="fixedPoints">if set to <c>true</c> [fixed points].</param>
    /// <exception cref="System.IndexOutOfRangeException">nSamples must be greater than 0.</exception>
    public StreamerUI(
                        WpfPlot plot,
                        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable,
                        int fs,
                        uint nSamples,
                        int nPointsPlotted,
                        string color,
                        bool autoscale = true,
                        bool manualscale = false,
                        bool fixedPoints = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        if (nSamples <= 0)
        {
            throw new IndexOutOfRangeException("nSamples must be greater than 0");
        }

        _nSamples = nSamples;
        _fs = fs;
        _numberPointsPlottedSaved = nPointsPlotted;

        Plot = plot;

        CreateStreamer(color);
        UpdateStreamerFixedPoints(observable);
        ChartSettings.CreateCursorValues(Plot, color);
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
    public DataStreamer? PlotLine { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateStreamer(string color)
    {
        double[] y = [0];
        double[] x = [0];

        PlotLine = Plot.Plot.Add.DataStreamer(NumberPointsPlotted);
        PlotLine.ViewScrollLeft();
        PlotLine.Period = 32000 / 2048;
        PlotLine.LineStyle.Width = 1f;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateStreamerFixedPoints(IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> observable) =>
        observable
        .ObserveOn(RxApp.TaskpoolScheduler)
        .Where(d => !string.IsNullOrEmpty(d.Name) && d.Y != null && d.X != null && d.Y.Count > 0 && d.X.Count > 0 && d.Y.Count == d.X.Count)
        .Retry()
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(d =>
        {
            var values = new List<double>(d.Y!).Take(_numberPointsPlottedSaved).ToArray();

            PlotLine!.AddRange(values!);
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
            ChartSettings.ItemName = d.Name;
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
            _chartSettings.Dispose();
        }
    }
}
