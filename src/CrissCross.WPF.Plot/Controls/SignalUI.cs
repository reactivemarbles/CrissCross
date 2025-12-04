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
/// Nice for historical data (performance).
/// </summary>
/// <seealso cref="SignalUI" />
[SupportedOSPlatform("windows")]
public partial class SignalUI : RxObject, IPlottableUI
{
    private readonly List<double> _time = [];

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
    [Reactive]
    private bool _ticks;

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
    /// <param name="numberPointsPlotted">The number points plotted.</param>
    /// <param name="ticks">if set to <c>true</c> [ticks].</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color,
        bool autoscale = true,
        bool manualscale = false,
        IObservable<bool>? fixedPoints = null,
        IObservable<int>? numberPointsPlotted = null,
        bool ticks = true)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;
        fixedPoints?.Subscribe(x => UseFixedNumberOfPoints = x).DisposeWith(Disposables);
        numberPointsPlotted?.Subscribe(x => NumberPointsPlotted = x).DisposeWith(Disposables);

        _ticks = ticks;
        Plot = plot;
        CreateDataLogger(color);
        UpdateSignal(observable);
        ChartSettings.CreateCursorValues(Plot, color);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);

        MouseCoordinatesObs = coordinatesObs.Retry().Subscribe(x =>
        {
            var plotLinedataList = new List<Coordinates>(PlotLine?.Data?.Coordinates!);
            if (plotLinedataList?.Count <= 0)
            {
                return;
            }

            var closestCoordinate = plotLinedataList!
            .OrderBy(coordinate => Math.Abs(coordinate.X - x.X))
            .FirstOrDefault();

            ChartSettings.Crosshair!.Position = closestCoordinate;
            ChartSettings.Marker!.Position = closestCoordinate;
            ChartSettings.MarkerText!.Location = closestCoordinate;
            ChartSettings.MarkerText!.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";

            Plot?.Refresh();
        }).DisposeWith(Disposables);
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
    /// Gets or sets the mouse coordinates.
    /// </summary>
    /// <value>
    /// The mouse coordinates.
    /// </value>
    public IDisposable? MouseCoordinatesObs { get; set; }

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
        PlotLine.ViewSlide(100);
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateSignal(IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable) => observable
        .ObserveOn(RxSchedulers.TaskpoolScheduler)
        .Select(data =>
        {
            var now = DateTime.Now;
            double[] datetime = [];

            if (_ticks)
            {
                // option 2: with ticks
                now = new(Convert.ToInt64(data.DateTime.Last()));
                datetime = [.. data.DateTime.ToList().ConvertAll(x => new DateTime(Convert.ToInt64(x)).ToOADate())];
            }
            else
            {
                // option 1: with timestamp
                datetime = [.. data.DateTime];
                var oaDate = data.DateTime.Last();
                now = DateTime.FromOADate(oaDate);
            }

            return (data, now, datetime);
        })
        .Where(d => !string.IsNullOrEmpty(d.data.Name) && d.data.Value != null && d.data.DateTime != null && d.data.Value.Count > 0 && d.data.DateTime.Count > 0 && d.data.Value.Count == d.data.DateTime.Count)
        .Select(d =>
        {
            // CALCULATE TIMESPAN TO PLOT
            var doublenow = d.now.ToOADate();
            var limits = d.now.Add(TimeSpan.FromMinutes(-0.1));
            var doublelimits = limits.ToOADate();

            ////// INSERT DATA INTO SIGNALXY
            var combinedValues = d.data.Value!.Zip(d.datetime, (v, d) => (v, d));
            var uniqueValues = combinedValues.Where(item => !_time.Contains(item.d)).OrderBy(x => x.d);
            var uniqueDataValues = uniqueValues.Select(x => x.v).ToArray();
            var uniqueTimeValues = uniqueValues.Select(x => x.d).ToArray();
            return (uniqueDataValues, uniqueTimeValues, d.data.Name);
        })
        .Retry()
        .ObserveOn(RxSchedulers.MainThreadScheduler)
        .Subscribe(d =>
        {
            _time.AddRange(d.uniqueTimeValues);
            if (UseFixedNumberOfPoints && PlotLine!.Data.Coordinates.Count > NumberPointsPlotted)
            {
                PlotLine!.Data.Coordinates.RemoveRange(0, PlotLine!.Data.Coordinates.Count - NumberPointsPlotted);
            }

            try
            {
                PlotLine!.Add(d.uniqueTimeValues, d.uniqueDataValues);
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
            ChartSettings.Dispose();
            MouseCoordinatesObs?.Dispose();
        }
    }
}
