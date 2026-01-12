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
/// Provides a Windows UI component for plotting and streaming time-series signal data, supporting real-time updates,
/// autoscaling, and interactive features. Nice for historical data (performance).
/// </summary>
/// <remarks>SignalUI integrates with ScottPlot and Rx.NET to visualize data streams in WPF applications. It
/// manages plot appearance, data buffering, and user interaction such as crosshair and marker updates based on mouse
/// coordinates. The class supports both automatic and manual scaling, and can limit the number of displayed points for
/// performance. Thread safety is maintained for UI updates via scheduler usage. SignalUI is intended for use on Windows
/// platforms.</remarks>
[SupportedOSPlatform("windows")]
public partial class SignalUI : RxObject, IPlottableUI
{
    private readonly HashSet<double> _timeSet = [];
    private readonly List<double> _uniqueDataBuffer = [];
    private readonly List<double> _uniqueTimeBuffer = [];

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

        // Set name from first emission of the observable
        observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateSignal(observable);
        ChartSettings.CreateCursorValues(Plot, color);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);

        MouseCoordinatesObs = coordinatesObs.Retry().Subscribe(x =>
        {
            var coordinates = PlotLine?.Data?.Coordinates;
            if (coordinates is null || coordinates.Count == 0)
            {
                return;
            }

            // Find closest coordinate using binary search-like approach for better performance
            var closestCoordinate = FindClosestCoordinate(coordinates, x.X);

            ChartSettings.Crosshair!.Position = closestCoordinate;
            ChartSettings.Marker!.Position = closestCoordinate;
            ChartSettings.MarkerText!.Location = closestCoordinate;
            ChartSettings.MarkerText!.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";

            Plot?.Refresh();
        }).DisposeWith(Disposables);
    }

    /// <summary>
    /// Gets or sets the WpfPlot control used for rendering interactive plots within the application.
    /// </summary>
    /// <remarks>Assigning a new WpfPlot instance replaces the current plot displayed. This property is
    /// typically used to embed or update plot visuals in WPF-based user interfaces.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the data logger used for plotting line data.
    /// </summary>
    public DataLogger? PlotLine { get; set; }

    /// <summary>
    /// Gets or sets an observable subscription for mouse coordinate updates.
    /// </summary>
    /// <remarks>Dispose the returned object to unsubscribe from mouse coordinate notifications and release
    /// resources. The property may be null if no subscription is active.</remarks>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>
    /// Initializes a new data logger line on the plot and sets its color using the specified hex value.
    /// </summary>
    /// <remarks>The data logger line is configured with a fixed line width and disables automatic axis limit
    /// management. The view is adjusted to display the most recent 100 data points. If the color string is not a valid
    /// hex code, an exception may be thrown by the color conversion method.</remarks>
    /// <param name="color">A string representing the color of the data logger line in hexadecimal format (e.g., "#FF0000" for red). Must be
    /// a valid hex color code.</param>
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
    /// Subscribes to an observable sequence of signal data and updates the plot with new values as they arrive.
    /// </summary>
    /// <remarks>The method processes incoming signal data, filters for valid entries, and updates the plot in
    /// real time. Data points with duplicate time values are ignored, and the plot is refreshed unless the chart is
    /// paused. If a fixed number of points is configured, older points are removed to maintain the limit. The method is
    /// thread-safe and uses background scheduling for data processing and main thread scheduling for UI
    /// updates.</remarks>
    /// <param name="observable">An observable sequence that provides tuples containing the signal name, value list, date/time list, and axis
    /// identifier. The value and date/time lists must be non-null, non-empty, and of equal length.</param>
    public void UpdateSignal(IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable) => observable
        .ObserveOn(RxSchedulers.TaskpoolScheduler)
        .Where(d => !string.IsNullOrEmpty(d.Name) && d.Value != null && d.DateTime != null && d.Value.Count > 0 && d.DateTime.Count > 0 && d.Value.Count == d.DateTime.Count)
        .Select(data =>
        {
            var dateTimeList = data.DateTime;
            var valueList = data.Value!;
            var count = dateTimeList.Count;

            // Pre-allocate arrays to avoid repeated allocations
            var datetime = new double[count];

            if (_ticks)
            {
                // Convert ticks to OADate
                for (var i = 0; i < count; i++)
                {
                    datetime[i] = new DateTime(Convert.ToInt64(dateTimeList[i])).ToOADate();
                }
            }
            else
            {
                // Direct copy for timestamp mode
                for (var i = 0; i < count; i++)
                {
                    datetime[i] = dateTimeList[i];
                }
            }

            return (valueList, datetime, data.Name);
        })
        .Retry()
        .ObserveOn(RxSchedulers.MainThreadScheduler)
        .Subscribe(d =>
        {
            // Clear and reuse buffers to avoid allocations
            _uniqueDataBuffer.Clear();
            _uniqueTimeBuffer.Clear();

            var valueList = d.valueList;
            var datetime = d.datetime;

            // Filter unique values using HashSet for O(1) lookup
            for (var i = 0; i < datetime.Length; i++)
            {
                var timeValue = datetime[i];
                if (_timeSet.Add(timeValue))
                {
                    _uniqueTimeBuffer.Add(timeValue);
                    _uniqueDataBuffer.Add(valueList[i]);
                }
            }

            if (_uniqueTimeBuffer.Count == 0)
            {
                return;
            }

            // Sort by time if needed (maintaining order)
            if (_uniqueTimeBuffer.Count > 1)
            {
                SortBuffersByTime();
            }

            if (UseFixedNumberOfPoints && PlotLine!.Data.Coordinates.Count > NumberPointsPlotted)
            {
                PlotLine!.Data.Coordinates.RemoveRange(0, PlotLine!.Data.Coordinates.Count - NumberPointsPlotted);
            }

            try
            {
                // Use ToArray since ScottPlot DataLogger doesn't have Span overload
                PlotLine!.Add([.. _uniqueTimeBuffer], [.. _uniqueDataBuffer]);
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
            ChartSettings.Dispose();
            MouseCoordinatesObs?.Dispose();
            _timeSet.Clear();
            _uniqueDataBuffer.Clear();
            _uniqueTimeBuffer.Clear();
        }
    }

    /// <summary>
    /// Finds the coordinate in the specified collection whose X value is closest to the given target X value.
    /// </summary>
    /// <remarks>If multiple coordinates are equally close to the target X value, the first such coordinate in
    /// the collection is returned. The method does not perform any validation on the input collection; callers should
    /// ensure it is not empty.</remarks>
    /// <param name="coordinates">The collection of coordinates to search. Must contain at least one element.</param>
    /// <param name="targetX">The target X value to compare against each coordinate's X value.</param>
    /// <returns>The coordinate whose X value is nearest to the specified target X value.</returns>
    private static Coordinates FindClosestCoordinate(IList<Coordinates> coordinates, double targetX)
    {
        var closest = coordinates[0];
        var minDistance = Math.Abs(closest.X - targetX);

        for (var i = 1; i < coordinates.Count; i++)
        {
            var distance = Math.Abs(coordinates[i].X - targetX);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = coordinates[i];
            }
        }

        return closest;
    }

    /// <summary>
    /// Sorts the internal time and data buffers in ascending order of time values.
    /// </summary>
    /// <remarks>This method ensures that the time and corresponding data buffers remain synchronized and
    /// ordered by time. It is intended for use with small datasets and assumes that the buffers are typically already
    /// sorted. Calling this method is necessary before performing operations that require the buffers to be in
    /// chronological order.</remarks>
    private void SortBuffersByTime()
    {
        // Simple insertion sort for small datasets, usually data comes pre-sorted
        for (var i = 1; i < _uniqueTimeBuffer.Count; i++)
        {
            var keyTime = _uniqueTimeBuffer[i];
            var keyData = _uniqueDataBuffer[i];
            var j = i - 1;

            while (j >= 0 && _uniqueTimeBuffer[j] > keyTime)
            {
                _uniqueTimeBuffer[j + 1] = _uniqueTimeBuffer[j];
                _uniqueDataBuffer[j + 1] = _uniqueDataBuffer[j];
                j--;
            }

            _uniqueTimeBuffer[j + 1] = keyTime;
            _uniqueDataBuffer[j + 1] = keyData;
        }
    }
}
