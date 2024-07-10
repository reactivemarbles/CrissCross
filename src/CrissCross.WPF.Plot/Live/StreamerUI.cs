// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ScottPlot;
using ScottPlot.AxisLimitManagers;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// EquationData.
/// </summary>
/// <seealso cref="StreamerUI" />
public class StreamerUI : RxObject
{
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
        ColorCheckBox = color;
        ItemName = "---";
        ColorText = "#FFD3D3D3";
        OpacityCheckBox = "1";
        LineWidth = 3;
        IsVisible = true;
        ManualScale = manualscale;
        AutoScale = autoscale;
        ZoomXY = false;
        IsChecked = true;

        Plot = plot;
        DisplayedValue = 0;
        this.WhenAnyValue(x => x.IsChecked)
            .Where(_ => Streamer != null)
            .Subscribe(value => Streamer!.IsVisible = value)
            .DisposeWith(Disposables);
        CreateStream(color);
        UpdateStream(observable);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic scale]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool AutoScale { get; set; }

    /// <summary>
    /// Gets or sets the color CheckBox.
    /// </summary>
    /// <value>
    /// The color CheckBox.
    /// </value>
    [Reactive]
    public string? ColorCheckBox { get; set; }

    /// <summary>
    /// Gets or sets the color text.
    /// </summary>
    /// <value>
    /// The color text.
    /// </value>
    [Reactive]
    public string? ColorText { get; set; }

    /// <summary>
    /// Gets or sets the displayed value.
    /// </summary>
    /// <value>
    /// The displayed value.
    /// </value>
    [Reactive]
    public int DisplayedValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is paused.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool IsPaused { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    /// <value>
    /// The name of the item.
    /// </value>
    [Reactive]
    public string? ItemName { get; set; }

    /// <summary>
    /// Gets or sets the width of the line.
    /// </summary>
    /// <value>
    /// The width of the line.
    /// </value>
    [Reactive]
    public int LineWidth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [manual scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [manual scale]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool ManualScale { get; set; }

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    [Reactive]
    public int Mode { get; set; }

    /// <summary>
    /// Gets or sets the number points plotted.
    /// </summary>
    /// <value>
    /// The number points plotted.
    /// </value>
    [Reactive]
    public int NumberPointsPlotted { get; set; }

    /// <summary>
    /// Gets or sets the opacity CheckBox.
    /// </summary>
    /// <value>
    /// The opacity CheckBox.
    /// </value>
    [Reactive]
    public string? OpacityCheckBox { get; set; }

    /// <summary>
    /// Gets or sets the plot.
    /// </summary>
    /// <value>
    /// The plot.
    /// </value>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [select area].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [select area]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool SelectArea { get; set; }

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
    /// Gets or sets a value indicating whether [zoom xy].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [zoom xy]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    public bool ZoomXY { get; set; }

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
                    var coord = datetime.Zip(values, (d, v) => new Coordinates(d, v));
                    ItemName = data.Name;
                    Streamer!.Data.Clear();
                    Streamer.Add(coord);

                    // UPDATE X AXIS
                    if (ManualScale || AutoScale)
                    {
                        Plot.Plot.Axes.SetLimitsX(doublelimits, doublenow, Streamer.Axes.XAxis);
                    }
                }

                // UPDATE IF IS NOT PAUSED
                if (!IsPaused)
                {
                    Plot.Refresh();
                }
            }).DisposeWith(Disposables);
    }
}
