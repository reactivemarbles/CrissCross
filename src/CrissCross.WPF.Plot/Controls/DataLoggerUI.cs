// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using Color = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Provides a user interface component for visualizing and interacting with data streams using a plot, supporting
/// real-time updates and scaling options. Nice for continuous live data.
/// </summary>
/// <remarks>This class is intended for use on Windows platforms and integrates with reactive observables to
/// display streaming data. It supports automatic and manual scaling of the plot, and can be configured to display a
/// fixed number of data points. The UI is designed to work with ScottPlot's WpfPlot and DataLogger for efficient data
/// visualization. Thread safety is managed internally for observable subscriptions and UI updates. Dispose of instances
/// to release resources when no longer needed.</remarks>
[SupportedOSPlatform("windows")]
public partial class DataLoggerUI : RxObject, IPlottableUI
{
    private double[]? _valueBuffer;

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
    /// Initializes a new instance of the <see cref="DataLoggerUI"/> class, configuring a data logger visualization for a WpfPlot.
    /// using data from an observable sequence.
    /// </summary>
    /// <remarks>The constructor subscribes to the observable to set the logger's item name from the first
    /// available emission. Appearance settings are applied to the plot upon initialization. Thread safety is ensured by
    /// observing on the main thread scheduler.</remarks>
    /// <param name="plot">The WpfPlot control where the data logger visualization will be rendered.</param>
    /// <param name="observable">An observable sequence providing data points and metadata for the logger. The first emission with a non-empty
    /// name sets the logger's item name.</param>
    /// <param name="color">The color used for the data logger's plot line and appearance.</param>
    /// <param name="autoscale">true to automatically scale the plot axes to fit incoming data; otherwise, false.</param>
    /// <param name="manualscale">true to enable manual axis scaling; otherwise, false.</param>
    /// <param name="points">true to display individual data points on the plot; otherwise, false.</param>
    public DataLoggerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false, bool points = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateDataLogger(color);

        // Set name from first emission of the observable
        observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataLoggerUI"/> class, configuring the plot and data logger to visualize.
    /// observable data streams with customizable appearance and scaling options.
    /// </summary>
    /// <remarks>The constructor subscribes to the observable to set the chart's item name from the first
    /// valid emission and configures appearance and scaling according to the provided parameters. Thread safety is
    /// ensured by observing on the main thread scheduler.</remarks>
    /// <param name="plot">The WpfPlot control used to display the data visualization.</param>
    /// <param name="observable">An observable sequence providing data points and metadata to be visualized. The first emission with a non-empty
    /// name sets the chart's item name.</param>
    /// <param name="color">The color used for the data logger's plot line and appearance.</param>
    /// <param name="autoscale">true to enable automatic axis scaling based on incoming data; otherwise, false.</param>
    /// <param name="manualscale">true to enable manual axis scaling; otherwise, false.</param>
    /// <param name="points">true to display individual data points on the plot; otherwise, false.</param>
    public DataLoggerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, int Axis, int nPoints)> observable, string color, bool autoscale = true, bool manualscale = false, bool points = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateDataLogger(color);

        // Set name from first emission of the observable
        observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateDataLogger(observable);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>
    /// Gets or sets the WPF plot control used to display graphical data within the application.
    /// </summary>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the data logger used for recording plot line information.
    /// </summary>
    public DataLogger? PlotLine { get; set; }

    /// <summary>
    /// Initializes a new data logger line on the plot and sets its color using the specified hex value.
    /// </summary>
    /// <remarks>The data logger line is configured with a fixed line width and axis limits are not managed
    /// automatically. The color is applied using the provided hex code. If the color string is not a valid hex code,
    /// the resulting color may be unpredictable.</remarks>
    /// <param name="color">A string representing the color of the data logger line in hexadecimal format (e.g., "#FF0000" for red). Must be
    /// a valid hex color code.</param>
    public void CreateDataLogger(string color)
    {
        PlotLine = Plot.Plot.Add.DataLogger();
        PlotLine.ViewSlide();
        PlotLine.ManageAxisLimits = false;
        PlotLine.LineStyle.Width = 1;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Subscribes to an observable sequence of data points and updates the data logger plot with incoming values.
    /// </summary>
    /// <remarks>The method filters out invalid data points and ensures that only the most recent nPoints are
    /// retained in the plot. Data updates are processed on background and main thread schedulers to optimize
    /// responsiveness. If the chart is not paused, the plot is refreshed automatically after each update.</remarks>
    /// <param name="observable">An observable sequence providing tuples containing the data point name, value list, axis index, and the number
    /// of points to retain. The value list must not be null or empty, and nPoints must be greater than zero.</param>
    public void UpdateDataLogger(IObservable<(string? Name, IList<double>? Value, int Axis, int nPoints)> observable) => observable
        .ObserveOn(RxSchedulers.TaskpoolScheduler)
        .Where(d => !string.IsNullOrEmpty(d.Name) && d.Value != null && d.Value.Count > 0 && d.nPoints > 0)
        .Select(data => (data.Value!, Math.Min(data.nPoints, 100_000_000)))
        .Retry()
        .ObserveOn(RxSchedulers.MainThreadScheduler)
        .Subscribe(d =>
        {
            var (valueList, nPoints) = d;
            var count = valueList.Count;

            if (PlotLine!.Data.Coordinates.Count >= nPoints)
            {
                PlotLine.Data.Coordinates.RemoveRange(0, PlotLine!.Data.Coordinates.Count - nPoints);
            }

            try
            {
                // Reuse or grow buffer to avoid allocations
                if (_valueBuffer == null || _valueBuffer.Length < count)
                {
                    _valueBuffer = new double[count];
                }

                // Copy values to buffer
                for (var i = 0; i < count; i++)
                {
                    _valueBuffer[i] = valueList[i];
                }

                // Use ArraySegment since ScottPlot DataLogger doesn't support Span
                if (count == _valueBuffer.Length)
                {
                    PlotLine!.Add(_valueBuffer);
                }
                else
                {
                    var values = new double[count];
                    Array.Copy(_valueBuffer, values, count);
                    PlotLine!.Add(values);
                }
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
            _valueBuffer = null;
        }
    }
}
