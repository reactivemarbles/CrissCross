// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ScottPlot;
using ScottPlot.DataSources;

namespace CrissCross.WPF.Plot;

/// <summary>Applies normalized reactive plot updates to WPF plot UI elements.</summary>
internal sealed class WpfReactivePlotAdapter : IReactivePlotAdapter
{
    /// <summary>Stores the owning live chart view model.</summary>
    private readonly LiveChartViewModel _chart;

    /// <summary>Stores the configured series color.</summary>
    private readonly string _color;

    /// <summary>Stores the signal subject value.</summary>
    private readonly Signal<(string? Name, IList<double>? Value, IList<double> X, int Axis)>? _signalSubject;

    /// <summary>Stores the scatter subject value.</summary>
    private readonly Signal<(string? Name, IList<double>? X, IList<double> Y, int Axis)>? _scatterSubject;

    /// <summary>Stores the data logger subject value.</summary>
    private readonly Signal<(string? Name, IList<double>? Value, int Axis, int nPoints)>? _dataLoggerSubject;

    /// <summary>Stores the streamer subject value.</summary>
    private readonly Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)>? _streamerSubject;

    /// <summary>Stores retained X values for append operations.</summary>
    private readonly List<double> _retainedX = [];

    /// <summary>Stores retained Y values for append operations.</summary>
    private readonly List<double> _retainedY = [];

    /// <summary>Stores the current plottable UI element.</summary>
    private IPlottableUI? _ui;

    /// <summary>Stores the signal X-axis kind currently used by the UI.</summary>
    private PlotXAxisKind? _signalXAxisKind;

    /// <summary>Stores whether the adapter has been disposed.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="WpfReactivePlotAdapter"/> class.</summary>
    /// <param name="chart">The chart value.</param>
    /// <param name="key">The key value.</param>
    /// <param name="plotType">The plotType value.</param>
    /// <param name="color">The color value.</param>
    public WpfReactivePlotAdapter(LiveChartViewModel chart, PlotSeriesKey key, PlotType plotType, string color)
    {
        _chart = chart ?? throw new ArgumentNullException(nameof(chart));
        _color = color;
        Key = key;
        PlotType = plotType;

        switch (plotType)
        {
            case PlotType.Signal:
                {
                    _signalSubject = new();
                    break;
                }

            case PlotType.Scatter:
                {
                    _scatterSubject = new();
                    _ui = new ScatterUI(_chart.WpfPlot1vm!, _scatterSubject, _color);
                    AddUi();
                    break;
                }

            case PlotType.DataLogger:
                {
                    _dataLoggerSubject = new();
                    _ui = new DataLoggerUI(_chart.WpfPlot1vm!, _dataLoggerSubject, _color);
                    AddUi();
                    break;
                }

            case PlotType.Streamer:
                {
                    _streamerSubject = new();
                    _ui = new StreamerUI(_chart.WpfPlot1vm!, _streamerSubject, fs: 1, sampleCount: 1, plottedPointCount: Math.Max(1, _chart.NumberPointsPlotted), _color);
                    AddUi();
                    break;
                }

            case PlotType.SignalXY:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(plotType), plotType, "Unsupported reactive plot type.");
        }
    }

    /// <summary>Gets the key value.</summary>
    public PlotSeriesKey Key { get; }

    /// <summary>Gets the plot type value.</summary>
    public PlotType PlotType { get; }

    /// <summary>Handles the Apply operation.</summary>
    /// <param name="update">The update value.</param>
    public void Apply(ReactivePlotUpdate update)
    {
        EnsureNotDisposed();
        if (TryApplyClear(update))
        {
            return;
        }

        ApplyPlotUpdate(update);
        AssignAxis(update.Key.Axis);
        _chart.WpfPlot1vm?.Refresh();
    }

    /// <summary>Handles the Dispose operation.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        DisposeSubject(_signalSubject);
        DisposeSubject(_scatterSubject);
        DisposeSubject(_dataLoggerSubject);
        DisposeSubject(_streamerSubject);
        _ui?.Dispose();
    }

    /// <summary>Clears a SignalXY UI element.</summary>
    /// <param name="signalXy">The SignalXY UI element.</param>
    private static void ClearSignalXy(SignalXY_UI signalXy) =>
        signalXy.PlotLine!.Data = new SignalXYSourceDoubleArray([], []);

    /// <summary>Completes and disposes a subject.</summary>
    /// <typeparam name="T">The subject value type.</typeparam>
    /// <param name="subject">The subject to dispose.</param>
    private static void DisposeSubject<T>(Signal<T>? subject)
    {
        if (subject is null)
        {
            return;
        }

        subject.OnCompleted();
        subject.Dispose();
    }

    /// <summary>Throws when the adapter has been disposed.</summary>
    private void EnsureNotDisposed() =>
        _ = _disposed ? throw new ObjectDisposedException(nameof(WpfReactivePlotAdapter)) : false;

    /// <summary>Applies clear semantics for clear and replace updates.</summary>
    /// <param name="update">The update value.</param>
    /// <returns><see langword="true"/> when the update was a terminal clear.</returns>
    private bool TryApplyClear(ReactivePlotUpdate update)
    {
        switch (update.Kind)
        {
            case ReactivePlotUpdateKind.Clear:
                {
                    ApplyClear(update);
                    return true;
                }

            case ReactivePlotUpdateKind.Replace:
                {
                    ApplyClear(update);
                    return false;
                }

            default:
                return false;
        }
    }

    /// <summary>Applies a non-clear plot update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyPlotUpdate(ReactivePlotUpdate update)
    {
        switch (PlotType)
        {
            case PlotType.Signal:
                {
                    ApplySignal(update);
                    break;
                }

            case PlotType.Scatter:
                {
                    ApplyScatter(update);
                    break;
                }

            case PlotType.DataLogger:
                {
                    ApplyDataLogger(update);
                    break;
                }

            case PlotType.Streamer:
                {
                    ApplyStreamer(update);
                    break;
                }

            case PlotType.SignalXY:
                {
                    ApplySignalXy(PrepareSnapshotUpdate(update));
                    break;
                }
        }
    }

    /// <summary>Applies a signal update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplySignal(ReactivePlotUpdate update)
    {
        EnsureSignalUi(update.XAxisKind);
        _signalSubject?.OnNext((update.Key.Name, update.Y.ToList(), update.X.ToList(), update.Key.Axis));
    }

    /// <summary>Applies a scatter update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyScatter(ReactivePlotUpdate update)
    {
        var scatterUpdate = PrepareSnapshotUpdate(update);
        _scatterSubject?.OnNext((scatterUpdate.Key.Name, scatterUpdate.X.ToList(), scatterUpdate.Y.ToList(), scatterUpdate.Key.Axis));
    }

    /// <summary>Applies a data logger update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyDataLogger(ReactivePlotUpdate update) =>
        _dataLoggerSubject?.OnNext((update.Key.Name, update.Y.ToList(), update.Key.Axis, update.MaxPoints ?? int.MaxValue));

    /// <summary>Applies a streamer update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyStreamer(ReactivePlotUpdate update) =>
        _streamerSubject?.OnNext((update.Key.Name, update.Y.ToList(), update.X.ToList(), update.Key.Axis));

    /// <summary>Handles the AddUi operation.</summary>
    private void AddUi()
    {
        if (_ui is null)
        {
            return;
        }

        _chart.PlotLinesCollectionUI.Add(_ui);
        _chart.UpdateChartObjectsCollection();
        AssignAxis(Key.Axis);
    }

    /// <summary>Handles the EnsureSignalUi operation.</summary>
    /// <param name="axisKind">The X-axis kind value.</param>
    private void EnsureSignalUi(PlotXAxisKind axisKind)
    {
        if (_ui is SignalUI && _signalXAxisKind == axisKind)
        {
            return;
        }

        if (_ui is not null)
        {
            _ = _chart.PlotLinesCollectionUI.Remove(_ui);
            _ui.Dispose();
            _ui = null;
            _chart.UpdateChartObjectsCollection();
        }

        _signalXAxisKind = axisKind;
        _ui = new SignalUI(
            _chart.WpfPlot1vm!,
            _signalSubject!,
            _chart.MouseCoordinatesObservable,
            _color,
            fixedPoints: _chart.WhenAnyValue(x => x.UseFixedNumberOfPoints),
            numberPointsPlotted: _chart.WhenAnyValue(x => x.NumberPointsPlotted),
            ticks: axisKind == PlotXAxisKind.Ticks);
        AddUi();
    }

    /// <summary>Handles the PrepareSnapshotUpdate operation.</summary>
    /// <param name="update">The update value.</param>
    /// <returns>The result.</returns>
    private ReactivePlotUpdate PrepareSnapshotUpdate(ReactivePlotUpdate update)
    {
        if (update.Kind == ReactivePlotUpdateKind.Replace || update.MaxPoints is not null)
        {
            _retainedX.Clear();
            _retainedY.Clear();
        }

        if (update.Kind != ReactivePlotUpdateKind.Append && update.Kind != ReactivePlotUpdateKind.Replace)
        {
            return update;
        }

        _retainedX.AddRange(update.X);
        _retainedY.AddRange(update.Y);
        if (update.MaxPoints is { } maxPoints && maxPoints > 0 && _retainedX.Count > maxPoints)
        {
            var excess = _retainedX.Count - maxPoints;
            _retainedX.RemoveRange(0, excess);
            _retainedY.RemoveRange(0, excess);
        }

        return update with { X = _retainedX.ToArray(), Y = _retainedY.ToArray() };
    }

    /// <summary>Handles the ApplySignalXy operation.</summary>
    /// <param name="update">The update value.</param>
    private void ApplySignalXy(ReactivePlotUpdate update)
    {
        if (_ui is not SignalXY_UI signalXy)
        {
            _ui = new SignalXY_UI(_chart.WpfPlot1vm!, (update.Key.Name, update.Y.ToList(), update.X.ToList(), update.Key.Axis), _color, coordinatesObs: _chart.MouseCoordinatesObservable);
            AddUi();
            return;
        }

        signalXy.PlotLine!.Data = new SignalXYSourceDoubleArray(update.X.ToArray(), update.Y.ToArray());
    }

    /// <summary>Handles the ApplyClear operation.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyClear(ReactivePlotUpdate update)
    {
        _retainedX.Clear();
        _retainedY.Clear();

        ClearUi();

        AssignAxis(update.Key.Axis);
        _chart.WpfPlot1vm?.Refresh();
    }

    /// <summary>Clears the current UI element.</summary>
    private void ClearUi()
    {
        switch (_ui)
        {
            case SignalXY_UI signalXy:
                {
                    ClearSignalXy(signalXy);
                    break;
                }

            case ScatterUI scatter:
                {
                    scatter.InsertData([], []);
                    break;
                }

            case SignalUI signal:
                {
                    signal.ClearData();
                    break;
                }

            case DataLoggerUI dataLogger:
                {
                    dataLogger.PlotLine!.Data.Coordinates.Clear();
                    break;
                }

            case StreamerUI streamer:
                {
                    ClearStreamer(streamer);
                    break;
                }
        }
    }

    /// <summary>Clears a streamer UI element.</summary>
    /// <param name="streamer">The streamer UI element.</param>
    private void ClearStreamer(StreamerUI streamer)
    {
        if (streamer.PlotLine?.Data is null)
        {
            return;
        }

        streamer.PlotLine.Data = new(new double[Math.Max(1, _chart.NumberPointsPlotted)]);
    }

    /// <summary>Handles the AssignAxis operation.</summary>
    /// <param name="axis">The axis value.</param>
    private void AssignAxis(int axis)
    {
        if (_ui is null || axis < 0 || axis >= _chart.YAxisList.Count)
        {
            return;
        }

        var verticalAxis = _chart.YAxisList[axis];
        verticalAxis.IsVisible = true;
        _ui.ChartSettings.Marker!.Axes.YAxis = verticalAxis;
        _ui.ChartSettings.MarkerText!.Axes.YAxis = verticalAxis;
        _ui.ChartSettings.Crosshair!.Axes.YAxis = verticalAxis;

        IPlottable? plottable = _ui switch
        {
            SignalUI signal => signal.PlotLine,
            ScatterUI scatter => scatter.PlotLine,
            DataLoggerUI logger => logger.PlotLine,
            StreamerUI streamer => streamer.PlotLine,
            SignalXY_UI signalXy => signalXy.PlotLine,
            _ => null,
        };

        if (plottable is null)
        {
            return;
        }

        plottable.Axes.YAxis = verticalAxis;
        plottable.Axes.XAxis = _chart.XAxis1;
    }
}
