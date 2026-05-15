// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using ReactiveUI;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;

namespace CrissCross.WPF.Plot;

internal sealed class WpfReactivePlotAdapter : IReactivePlotAdapter
{
    private readonly LiveChartViewModel _chart;
    private readonly string _color;
    private readonly Subject<(string? Name, IList<double>? Value, IList<double> X, int Axis)>? _signalSubject;
    private readonly Subject<(string? Name, IList<double>? X, IList<double> Y, int Axis)>? _scatterSubject;
    private readonly Subject<(string? Name, IList<double>? Value, int Axis, int nPoints)>? _dataLoggerSubject;
    private readonly Subject<(string? Name, IList<double>? Y, IList<double> X, int Axis)>? _streamerSubject;
    private readonly List<double> _retainedX = [];
    private readonly List<double> _retainedY = [];
    private IPlottableUI? _ui;
    private bool _disposed;

    public WpfReactivePlotAdapter(LiveChartViewModel chart, PlotSeriesKey key, PlotType plotType, string color)
    {
        _chart = chart ?? throw new ArgumentNullException(nameof(chart));
        _color = color;
        Key = key;
        PlotType = plotType;

        switch (plotType)
        {
            case PlotType.Signal:
                _signalSubject = new Subject<(string? Name, IList<double>? Value, IList<double> X, int Axis)>();
                _ui = new SignalUI(_chart.WpfPlot1vm!, _signalSubject, _chart.MouseCoordinatesObservable, _color, fixedPoints: _chart.WhenAnyValue(x => x.UseFixedNumberOfPoints), numberPointsPlotted: _chart.WhenAnyValue(x => x.NumberPointsPlotted));
                AddUi();
                break;
            case PlotType.Scatter:
                _scatterSubject = new Subject<(string? Name, IList<double>? X, IList<double> Y, int Axis)>();
                _ui = new ScatterUI(_chart.WpfPlot1vm!, _scatterSubject, _color);
                AddUi();
                break;
            case PlotType.DataLogger:
                _dataLoggerSubject = new Subject<(string? Name, IList<double>? Value, int Axis, int nPoints)>();
                _ui = new DataLoggerUI(_chart.WpfPlot1vm!, _dataLoggerSubject, _color);
                AddUi();
                break;
            case PlotType.Streamer:
                _streamerSubject = new Subject<(string? Name, IList<double>? Y, IList<double> X, int Axis)>();
                _ui = new StreamerUI(_chart.WpfPlot1vm!, _streamerSubject, fs: 1, nSamples: 1, nPointsPlotted: Math.Max(1, _chart.NumberPointsPlotted), _color);
                AddUi();
                break;
            case PlotType.SignalXY:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(plotType), plotType, "Unsupported reactive plot type.");
        }
    }

    public PlotSeriesKey Key { get; }

    public PlotType PlotType { get; }

    public void Apply(ReactivePlotUpdate update)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(WpfReactivePlotAdapter));
        }

        if (update.Kind == ReactivePlotUpdateKind.Clear)
        {
            ApplyClear(update);
            return;
        }

        if (update.Kind == ReactivePlotUpdateKind.Replace)
        {
            ApplyClear(update);
        }

        switch (PlotType)
        {
            case PlotType.Signal:
                _signalSubject?.OnNext((update.Key.Name, update.Y.ToList(), update.X.ToList(), update.Key.Axis));
                break;
            case PlotType.Scatter:
                var scatterUpdate = PrepareSnapshotUpdate(update);
                _scatterSubject?.OnNext((scatterUpdate.Key.Name, scatterUpdate.X.ToList(), scatterUpdate.Y.ToList(), scatterUpdate.Key.Axis));
                break;
            case PlotType.DataLogger:
                _dataLoggerSubject?.OnNext((update.Key.Name, update.Y.ToList(), update.Key.Axis, update.MaxPoints ?? int.MaxValue));
                break;
            case PlotType.Streamer:
                _streamerSubject?.OnNext((update.Key.Name, update.Y.ToList(), update.X.ToList(), update.Key.Axis));
                break;
            case PlotType.SignalXY:
                ApplySignalXy(PrepareSnapshotUpdate(update));
                break;
        }

        AssignAxis(update.Key.Axis);
        _chart.WpfPlot1vm?.Refresh();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _signalSubject?.OnCompleted();
        _scatterSubject?.OnCompleted();
        _dataLoggerSubject?.OnCompleted();
        _streamerSubject?.OnCompleted();
        _signalSubject?.Dispose();
        _scatterSubject?.Dispose();
        _dataLoggerSubject?.Dispose();
        _streamerSubject?.Dispose();
        _ui?.Dispose();
    }

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

    private void ApplyClear(ReactivePlotUpdate update)
    {
        _retainedX.Clear();
        _retainedY.Clear();

        switch (_ui)
        {
            case SignalXY_UI signalXy:
                signalXy.PlotLine!.Data = new SignalXYSourceDoubleArray([], []);
                break;
            case ScatterUI scatter:
                scatter.InsertData([], []);
                break;
            case SignalUI signal:
                signal.ClearData();
                break;
            case DataLoggerUI dataLogger:
                dataLogger.PlotLine!.Data.Coordinates.Clear();
                break;
            case StreamerUI streamer when streamer.PlotLine?.Data is not null:
                streamer.PlotLine.Data = new(new double[Math.Max(1, _chart.NumberPointsPlotted)]);
                break;
        }

        AssignAxis(update.Key.Axis);
        _chart.WpfPlot1vm?.Refresh();
    }

    private void AssignAxis(int axis)
    {
        if (_ui is null || axis < 0 || axis >= _chart.YAxisList.Count)
        {
            return;
        }

        var yAxis = _chart.YAxisList[axis];
        yAxis.IsVisible = true;
        _ui.ChartSettings.Marker!.Axes.YAxis = yAxis;
        _ui.ChartSettings.MarkerText!.Axes.YAxis = yAxis;
        _ui.ChartSettings.Crosshair!.Axes.YAxis = yAxis;

        IPlottable? plottable = _ui switch
        {
            SignalUI signal => signal.PlotLine,
            ScatterUI scatter => scatter.PlotLine,
            DataLoggerUI logger => logger.PlotLine,
            StreamerUI streamer => streamer.PlotLine,
            SignalXY_UI signalXy => signalXy.PlotLine,
            _ => null,
        };

        if (plottable is not null)
        {
            plottable.Axes.YAxis = yAxis;
            plottable.Axes.XAxis = _chart.XAxis1;
        }
    }
}
