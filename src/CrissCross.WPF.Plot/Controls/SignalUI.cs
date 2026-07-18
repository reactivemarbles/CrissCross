// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>
/// Provides a Windows UI component for plotting and streaming time-series signal data, supporting real-time updates,
/// autoscaling, and interactive features. Nice for historical data (performance).
/// </summary>
/// <remarks>SignalUI integrates with ScottPlot and ReactiveUI.Primitives to visualize data streams in WPF applications.
/// It
/// manages plot appearance, data buffering, and user interaction such as crosshair and marker updates based on mouse
/// coordinates. The class supports both automatic and manual scaling, and can limit the number of displayed points for
/// performance. Thread safety is maintained for UI updates via scheduler usage. SignalUI is intended for use on Windows
/// platforms.</remarks>
[SupportedOSPlatform("windows")]
public partial class SignalUI : RxObject, IPlottableUI
{
    /// <summary>The initial number of signal points shown in the viewport.</summary>
    private const int InitialViewPointCount = 100;

    /// <summary>Stores the time set value.</summary>
    private readonly HashSet<double> _timeSet = [];

    /// <summary>Stores the unique data buffer value.</summary>
    private readonly List<double> _uniqueDataBuffer = [];

    /// <summary>Stores the unique time buffer value.</summary>
    private readonly List<double> _uniqueTimeBuffer = [];

    /// <summary>Stores the chart settings value.</summary>
    [Reactive]
    private ChartObjects _chartSettings;

    /// <summary>Stores the auto scale value.</summary>
    [Reactive]
    private bool _autoScale;

    /// <summary>Stores the manual scale value.</summary>
    [Reactive]
    private bool _manualScale;

    /// <summary>Stores the mode value.</summary>
    [Reactive]
    private int _mode;

    /// <summary>Stores the number points plotted value.</summary>
    [Reactive]
    private int _numberPointsPlotted;

    /// <summary>Stores the use fixed number of points value.</summary>
    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>Stores the ticks value.</summary>
    [Reactive]
    private bool _ticks;

    /// <summary>Initializes a new instance of the <see cref="SignalUI" /> class.</summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="coordinatesObs">The coordinates obs.</param>
    /// <param name="color">The color.</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color)
        : this(plot, observable, coordinatesObs, color, new SignalUIOptions())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SignalUI"/> class.</summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="coordinatesObs">The coordinates observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">A value indicating whether automatic scaling is enabled.</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color,
        bool autoscale)
        : this(plot, observable, coordinatesObs, color, new SignalUIOptions { AutoScale = autoscale })
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SignalUI"/> class.</summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="coordinatesObs">The coordinates observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">A value indicating whether automatic scaling is enabled.</param>
    /// <param name="manualscale">A value indicating whether manual scaling is enabled.</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color,
        bool autoscale,
        bool manualscale)
        : this(
            plot,
            observable,
            coordinatesObs,
            color,
            new SignalUIOptions { AutoScale = autoscale, ManualScale = manualscale })
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SignalUI"/> class.</summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="coordinatesObs">The coordinates observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">A value indicating whether automatic scaling is enabled.</param>
    /// <param name="manualscale">A value indicating whether manual scaling is enabled.</param>
    /// <param name="fixedPoints">The optional stream controlling fixed-point display mode.</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color,
        bool autoscale,
        bool manualscale,
        IObservable<bool>? fixedPoints)
        : this(
            plot,
            observable,
            coordinatesObs,
            color,
            new SignalUIOptions
            {
                AutoScale = autoscale,
                ManualScale = manualscale,
                FixedPoints = fixedPoints,
            })
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SignalUI"/> class.</summary>
    /// <param name="plot">The plot.</param>
    /// <param name="observable">The observable.</param>
    /// <param name="coordinatesObs">The coordinates observable.</param>
    /// <param name="color">The color.</param>
    /// <param name="options">The signal configuration.</param>
    public SignalUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable,
        IObservable<Coordinates> coordinatesObs,
        string color,
        SignalUIOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        ChartSettings = new("---", color);
        ManualScale = options.ManualScale;
        AutoScale = options.AutoScale;
        options.FixedPoints?.Subscribe(x => UseFixedNumberOfPoints = x).DisposeWith(Disposables);
        options.NumberPointsPlotted?.Subscribe(x => NumberPointsPlotted = x).DisposeWith(Disposables);

        _ticks = options.Ticks;
        Plot = plot;
        CreateDataLogger(color);

        // Set name from first emission of the observable
        _ = observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateSignal(observable);
        ChartSettings.CreateCursorValues(Plot, color);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);

        MouseCoordinatesObs = coordinatesObs
            .Retry(int.MaxValue)
            .Subscribe(x =>
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
                ChartSettings.MarkerText!.LabelText = _ticks
                    ? $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}"
                    : $"{closestCoordinate.Y:0.##}\n{closestCoordinate.X:0.##}";

                Plot?.Refresh();
            })
            .DisposeWith(Disposables);
    }

    /// <summary>Gets or sets the WpfPlot control used for rendering interactive plots within the application.</summary>
    /// <remarks>Assigning a new WpfPlot instance replaces the current plot displayed. This property is
    /// typically used to embed or update plot visuals in WPF-based user interfaces.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the data logger used for plotting line data.</summary>
    public DataLogger? PlotLine { get; set; }

    /// <summary>Gets or sets an observable subscription for mouse coordinate updates.</summary>
    /// <remarks>Dispose the returned object to unsubscribe from mouse coordinate notifications and release
    /// resources. The property may be null if no subscription is active.</remarks>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>Provides the ClearData member.</summary>
    public void ClearData()
    {
        PlotLine!.Data.Coordinates.Clear();
        _timeSet.Clear();
        _uniqueDataBuffer.Clear();
        _uniqueTimeBuffer.Clear();
    }

    /// <summary>Provides the CreateDataLogger member.</summary>
    /// <param name="color">The color value.</param>
    public void CreateDataLogger(string color)
    {
        PlotLine = Plot.Plot.Add.DataLogger();
        PlotLine.ViewSlide();
        PlotLine.ManageAxisLimits = false;
        PlotLine.LineStyle.Width = 1;
        PlotLine.Color = Color.FromHex(color);
        PlotLine.ViewSlide(InitialViewPointCount);
    }

    /// <summary>Provides the UpdateSignal member.</summary>
    /// <param name="observable">The observable value.</param>
    public void UpdateSignal(
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable) =>
        observable
            .ObserveOn(RxSchedulers.TaskpoolScheduler)
            .Where(d =>
                !string.IsNullOrEmpty(d.Name)
                && d.Value is not null
                && d.DateTime is not null
                && d.Value.Count > 0
                && d.DateTime.Count > 0
                && d.Value.Count == d.DateTime.Count)
            .Select(data => (Value: data.Value!, Time: CreateTimeValues(data.DateTime)))
            .Retry(int.MaxValue)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => AppendSignalData(data.Value, data.Time))
            .DisposeWith(Disposables);

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">The disposing value.</param>
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            base.Dispose(disposing);
            return;
        }

        ChartSettings.Dispose();
        MouseCoordinatesObs?.Dispose();
        _timeSet.Clear();
        _uniqueDataBuffer.Clear();
        _uniqueTimeBuffer.Clear();
        base.Dispose(disposing);
    }

    /// <summary>Provides the FindClosestCoordinate member.</summary>
    /// <remarks>If multiple coordinates are equally close to the target X value, the first such coordinate in
    /// the collection is returned. The method does not perform any validation on the input collection; callers should
    /// ensure it is not empty.</remarks>
    /// <param name="coordinates">The collection of coordinates to search. Must contain at least one element.</param>
    /// <param name="targetX">The target X value to compare against each coordinate's X value.</param>
    /// <returns>The coordinate whose X value is nearest to the specified target X value.</returns>
    private static Coordinates FindClosestCoordinate(List<Coordinates> coordinates, double targetX)
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

    /// <summary>Converts source X values to the representation used by the plot.</summary>
    /// <param name="source">The source X values.</param>
    /// <returns>The converted X values.</returns>
    private double[] CreateTimeValues(IList<double> source)
    {
        var result = new double[source.Count];
        for (var i = 0; i < source.Count; i++)
        {
            result[i] = _ticks
                ? new DateTime(Convert.ToInt64(source[i]), DateTimeKind.Local).ToOADate()
                : source[i];
        }

        return result;
    }

    /// <summary>Adds a validated signal batch to the plot.</summary>
    /// <param name="values">The Y values.</param>
    /// <param name="timeValues">The converted X values.</param>
    private void AppendSignalData(IList<double> values, double[] timeValues)
    {
        _uniqueDataBuffer.Clear();
        _uniqueTimeBuffer.Clear();

        for (var i = 0; i < timeValues.Length; i++)
        {
            var timeValue = timeValues[i];
            if (!_timeSet.Add(timeValue))
            {
                continue;
            }

            _uniqueTimeBuffer.Add(timeValue);
            _uniqueDataBuffer.Add(values[i]);
        }

        if (_uniqueTimeBuffer.Count == 0)
        {
            return;
        }

        SortBuffersByTime();

        TrimDisplayedPoints();
        PlotLine!.Add([.. _uniqueTimeBuffer], [.. _uniqueDataBuffer]);
        PlotLine.ManageAxisLimits = false;

        if (ChartSettings.IsPaused)
        {
            return;
        }

        Plot.Refresh();
    }

    /// <summary>Trims plotted coordinates when fixed-point display mode is active.</summary>
    private void TrimDisplayedPoints()
    {
        if (!UseFixedNumberOfPoints || PlotLine!.Data.Coordinates.Count <= NumberPointsPlotted)
        {
            return;
        }

        PlotLine.Data.Coordinates.RemoveRange(0, PlotLine.Data.Coordinates.Count - NumberPointsPlotted);
    }

    /// <summary>Sorts the internal time and data buffers in ascending order of time values.</summary>
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
