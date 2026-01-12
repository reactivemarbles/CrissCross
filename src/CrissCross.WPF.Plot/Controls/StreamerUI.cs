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
/// Provides a user interface component for streaming and visualizing real-time data on a plot, supporting dynamic
/// scaling and configurable display options.
/// </summary>
/// <remarks>This class is intended for use on Windows platforms and integrates with reactive data sources to
/// update plots in real time. It supports automatic and manual scaling, fixed or variable point display, and
/// customizable appearance through chart settings. Thread safety is managed internally for UI updates. Dispose of
/// instances when no longer needed to release resources associated with chart settings and subscriptions.</remarks>
[SupportedOSPlatform("windows")]
public partial class StreamerUI : RxObject, IPlottableUI
{
    private readonly double[] _valueBuffer;
    private readonly uint _nSamples = 1;
    private readonly int _fs = 1;
    private readonly int _numberPointsPlottedSaved;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamerUI"/> class to display and update a streaming plot based on observable.
    /// data.
    /// </summary>
    /// <remarks>The chart's item name is set from the first emission of the observable sequence. Chart
    /// appearance and cursor values are initialized using the provided color and plot control. The autoscale,
    /// manualscale, and fixedPoints parameters control axis scaling and point display behavior.</remarks>
    /// <param name="plot">The WpfPlot control used to render the streaming chart.</param>
    /// <param name="observable">An observable sequence providing tuples containing the item name, value data, date/time data, and axis index for
    /// plotting.</param>
    /// <param name="fs">The sampling frequency, in Hz, used for interpreting the data stream.</param>
    /// <param name="nSamples">The number of samples to buffer for the streaming plot. Must be greater than 0.</param>
    /// <param name="nPointsPlotted">The initial number of data points to display on the plot.</param>
    /// <param name="color">The color used for the plot line and chart appearance.</param>
    /// <param name="autoscale">true to enable automatic scaling of the plot axes; otherwise, false.</param>
    /// <param name="manualscale">true to enable manual scaling of the plot axes; otherwise, false.</param>
    /// <param name="fixedPoints">true to fix the number of points displayed on the plot; otherwise, false.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when nSamples is less than or equal to 0.</exception>
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
        _valueBuffer = new double[nPointsPlotted];

        Plot = plot;

        CreateStreamer(color);

        // Set name from first emission of the observable
        observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateStreamerFixedPoints(observable);
        ChartSettings.CreateCursorValues(Plot, color);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>
    /// Gets or sets the WPF plot control used to display graphical data within the application.
    /// </summary>
    /// <remarks>Assign this property to embed or update the plot visualization in a WPF user interface. The
    /// property should be set before interacting with plot-specific features or rendering data.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the data streamer used to plot line data in the visualization.
    /// </summary>
    public DataStreamer? PlotLine { get; set; }

    /// <summary>
    /// Initializes a new data streamer line on the plot using the specified color.
    /// </summary>
    /// <remarks>The method configures the streamer line with default width and period settings, and scrolls
    /// the view to the left to display the most recent data points. The color parameter must be a valid hexadecimal
    /// color code; otherwise, an exception may be thrown by the color parsing logic.</remarks>
    /// <param name="color">A string representing the color of the streamer line, specified in hexadecimal format (e.g., "#FF0000" for red).</param>
    public void CreateStreamer(string color)
    {
        PlotLine = Plot.Plot.Add.DataStreamer(_numberPointsPlottedSaved);
        var darray = new double[_numberPointsPlottedSaved];
        PlotLine.Data = new(darray);
        PlotLine.ViewScrollLeft();
        PlotLine.Period = 32000 / 2048;
        PlotLine.LineStyle.Width = 1f;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Subscribes to an observable sequence of fixed-point data and updates the streamer plot with new values as they
    /// arrive.
    /// </summary>
    /// <remarks>The method processes incoming data on a background thread and updates the plot on the main
    /// thread. Data is only plotted if the chart is not paused. The observable sequence is retried on error, ensuring
    /// continuous updates unless disposed.</remarks>
    /// <param name="observable">An observable sequence that provides tuples containing the series name, Y-values, X-values, and axis index. Each
    /// tuple must have a non-empty name, non-null and non-empty Y and X lists of equal length.</param>
    public void UpdateStreamerFixedPoints(IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> observable) =>
        observable
        .ObserveOn(RxSchedulers.TaskpoolScheduler)
        .Where(d => !string.IsNullOrEmpty(d.Name) && d.Y != null && d.X != null && d.Y.Count > 0 && d.X.Count > 0 && d.Y.Count == d.X.Count)
        .Retry()
        .ObserveOn(RxSchedulers.MainThreadScheduler)
        .Subscribe(d =>
        {
            var sourceList = d.Y!;
            var count = Math.Min(sourceList.Count, _numberPointsPlottedSaved);

            // Copy to pre-allocated buffer to avoid allocations
            for (var i = 0; i < count; i++)
            {
                _valueBuffer[i] = sourceList[i];
            }

            // For DataStreamer, we need to provide an IEnumerable<double>
            // Use ArraySegment which implements IEnumerable without allocation
            var segment = new ArraySegment<double>(_valueBuffer, 0, count);
            PlotLine!.AddRange(segment);
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
            _chartSettings.Dispose();
        }
    }
}
