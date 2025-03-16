// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
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
public partial class ScatterUI : RxObject
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
    /// Initializes a new instance of the <see cref="ScatterUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    public ScatterUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false)
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

        CreateScatter(color);
        UpdateScatter(observable);
        AppearanceSubsriptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="data">The data.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    public ScatterUI(WpfPlot plot, (string? Name, IList<double>? X, IList<double> Y, int Axis) data, string color, bool autoscale = true, bool manualscale = false)
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

        CreateScatter(color);
        InsertData(data.X, data.Y);
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
    public Scatter? Scatter { get; set; }

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
        Scatter = Plot.Plot.Add.Scatter(x, y);
        Scatter.LineStyle.Width = 1f;
        Scatter.MarkerSize = 1f;
        Scatter.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Inserts data into scatter.
    /// </summary>
    /// <param name="x">X array.</param>
    /// <param name="y">Y array.</param>
    public void InsertData(IList<double>? x, IList<double> y)
    {
        Axes = Scatter!.Axes;
        Plot.Plot.Remove(Scatter!);
        var xs = x.ToArray();
        var ys = y.ToArray();
        Scatter = Plot.Plot.Add.Scatter(xs, ys);
        Scatter.LineStyle.Width = 1f;
        Scatter.LineStyle.IsVisible = false;
        Scatter.MarkerSize = 1f;
        Scatter!.Axes = Axes;
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateScatter(IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable) => observable.ObserveOn(RxApp.MainThreadScheduler)
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
            ChartSettings.Dispose();
            _chartSettings.Dispose();
        }
    }

    private void AppearanceSubsriptions()
    {
        this.WhenAnyValue(x => x.ChartSettings.LineWidth, x => x.ChartSettings.Color, x => x.ChartSettings.Visibility)
            .Subscribe(x =>
            {
                Scatter!.LineStyle.Width = (float)x.Item1;
                Scatter!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2));
                ChartSettings.IsChecked = x.Item3 == "Invisible";
                Scatter.IsVisible = x.Item3 != "Invisible";
                Plot.Refresh();
            }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsChecked)
            .Subscribe(x => ChartSettings.Visibility = x ? "Invisible" : "Visible")
            .DisposeWith(Disposables);
    }
}
