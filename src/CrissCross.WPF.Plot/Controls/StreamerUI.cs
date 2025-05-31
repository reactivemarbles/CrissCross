// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamerUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="fixedPoints">if set to <c>true</c> [fixed points].</param>
    public StreamerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false, bool fixedPoints = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        NumberPointsPlotted = 1024;
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

                // INSERT DATA INTO STREAMER
                if (data.Y != null && data.Y.Count == data.X.Count && data.Name != null)
                {
                    var values = new List<double>(data.Y).Take(NumberPointsPlotted).ToArray();

                    PlotLine!.AddRange(values!);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        ////Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, PlotLine.Axes.XAxis);
                        ////Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, PlotLine.Axes.XAxis);
                    }
                }

                // UPDATE NAME
                ChartSettings.ItemName = Name!;
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
