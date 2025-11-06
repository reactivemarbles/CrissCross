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
/// Nice for plotting XY random values.
/// </summary>
/// <seealso cref="ScatterUI" />
[SupportedOSPlatform("windows")]
public partial class ScatterUI : RxObject, IPlottableUI
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
    /// Initializes a new instance of the <see cref="ScatterUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    public ScatterUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;

        CreateScatter(color);
        UpdateScatter(observable);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="data">The data.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    public ScatterUI(WpfPlot plot, (string? Name, IList<double> X, IList<double> Y, int Axis) data, string color, bool autoscale = true, bool manualscale = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        ChartSettings.DisplayedValue = 0;

        CreateScatter(color);
        InsertData(data.X, data.Y);
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
    public Scatter? PlotLine { get; set; }

    /// <summary>
    /// Gets or sets the axes.
    /// </summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
    public void CreateScatter(string color)
    {
        double[] y = [0];
        double[] x = [0];
        PlotLine = Plot.Plot.Add.Scatter(x, y);
        PlotLine.LineStyle.Width = 1f;
        PlotLine.MarkerSize = 1f;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Inserts data into scatter.
    /// </summary>
    /// <param name="x">X array.</param>
    /// <param name="y">Y array.</param>
    public void InsertData(IList<double> x, IList<double> y)
    {
        Axes = PlotLine!.Axes;
        Plot.Plot.Remove(PlotLine!);
        var xs = x.ToArray();
        var ys = y.ToArray();
        PlotLine = Plot.Plot.Add.Scatter(xs, ys);
        PlotLine.LineStyle.Width = 1f;
        PlotLine.LineStyle.IsVisible = false;
        PlotLine.MarkerSize = 1f;
        PlotLine!.Axes = Axes;
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateScatter(IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable) =>
        observable.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(data =>
            {
                if (data.Name == null || data.X == null || data.Y == null)
                {
                    return;
                }

                if (data!.X!.Count == 0 || data.Y.Count == 0)
                {
                    return;
                }

                // INSERT DATA INTO SCATTER
                if (data.X.Count == data.Y.Count && data.Name != null)
                {
                    InsertData(data.X, data.Y);
                    ChartSettings.ItemName = data.Name;

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        Plot.Plot.Axes.SetLimitsX(data.X.Min(x => x) - 1, data.X.Max(x => x) + 1, Plot!.Plot.Axes.Bottom);
                        Plot.Plot.Axes.SetLimitsY(data.Y.Min(x => x) - 1, data.Y.Max(x => x) + 1, Plot!.Plot.Axes.Left);
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
