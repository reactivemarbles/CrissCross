// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ReactiveUIBindingExtensions;
#else
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Applies normalized reactive plot updates to WPF plot UI elements.</summary>
internal sealed partial class WpfReactivePlotAdapter : IReactivePlotAdapter
{
    /// <summary>Stores the owning live chart view model.</summary>
    private readonly LiveChartViewModel _chart;

    /// <summary>Stores the configured series color.</summary>
    private readonly string _color;

    /// <summary>Stores editable settings retained across source updates.</summary>
    private readonly ReactivePlotSeriesSettings _settings;

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

    /// <summary>Stores the current snapshot-oriented ScottPlot plottable.</summary>
    private IPlottable? _snapshotPlottable;

    /// <summary>Stores the signal X-axis kind currently used by the UI.</summary>
    private PlotXAxisKind? _signalXAxisKind;

    /// <summary>Stores whether the adapter has been disposed.</summary>
    private bool _disposed;

    /// <summary>Stores whether source defaults have been applied.</summary>
    private bool _settingsInitialized;

    /// <summary>Stores whether the binder supplied a retained snapshot.</summary>
    private bool _incomingSnapshot;

    /// <summary>Stores whether initial axis visibility has been established.</summary>
    private bool _axisAssigned;

    /// <summary>Prevents recursive synchronization of the series selectors.</summary>
    private bool _applyingSettings;

    /// <summary>Stores the latest X-coordinate interpretation for static redraws.</summary>
    private PlotXAxisKind _lastXAxisKind;

    /// <summary>Initializes a new instance of the <see cref="WpfReactivePlotAdapter"/> class.</summary>
    /// <param name="chart">The chart value.</param>
    /// <param name="key">The key value.</param>
    /// <param name="plotType">The plotType value.</param>
    /// <param name="color">The color value.</param>
    internal WpfReactivePlotAdapter(LiveChartViewModel chart, PlotSeriesKey key, PlotType plotType, string color)
    {
        ThrowHelper.ThrowIfNull(chart, nameof(chart));
        ThrowHelper.ThrowIfNull(color, nameof(color));

        _chart = chart;
        _color = color;
        Key = key;
        PlotType = plotType;
        _settings = new(key, plotType, color);
        _chart.SeriesSettings.Add(_settings);
        _settings.PropertyChanged += SettingsChanged;
        _chart.PropertyChanged += ChartSettingsChanged;

        var initialization = CreateInitialization(plotType);
        _signalSubject = initialization.SignalSubject;
        _scatterSubject = initialization.ScatterSubject;
        _dataLoggerSubject = initialization.DataLoggerSubject;
        _streamerSubject = initialization.StreamerSubject;
        _ui = initialization.Ui;
        AddUiIfPresent();
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
        if (!_settingsInitialized && update.Kind != ReactivePlotUpdateKind.Clear)
        {
            _settings.Initialize(update);
            _settingsInitialized = true;
        }

        if (_settings.IsPaused && update.Kind != ReactivePlotUpdateKind.Clear)
        {
            return;
        }

        _incomingSnapshot = update.MaxPoints is not null && PlotType != PlotType.DataLogger;
        update = update with { Style = _settings.CreateStyle(), MaxPoints = _settings.MaxPoints ?? update.MaxPoints };
        ConfigureXAxis(update.XAxisKind);
        _lastXAxisKind = update.XAxisKind;
        if (TryApplyClear(update))
        {
            return;
        }

        ApplyPlotUpdate(update);
        TrimUiPoints();
        ApplyStyle(update.Style);
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
        _settings.PropertyChanged -= SettingsChanged;
        _chart.PropertyChanged -= ChartSettingsChanged;
        _ = _chart.SeriesSettings.Remove(_settings);
        _settings.Dispose();
        if (_ui is not null)
        {
            _ui.ChartSettings.PropertyChanged -= UiSettingsChanged;
            _ = _chart.PlotLinesCollectionUI.Remove(_ui);
        }

        DisposeSubject(_signalSubject);
        DisposeSubject(_scatterSubject);
        DisposeSubject(_dataLoggerSubject);
        DisposeSubject(_streamerSubject);
        _ui?.Dispose();
        RemoveSnapshotPlottable();
    }

    /// <summary>Creates plot-type specific adapter state.</summary>
    /// <param name="plotType">The plot type.</param>
    /// <returns>The initialized adapter state.</returns>
    private PlotAdapterInitialization CreateInitialization(PlotType plotType) =>
        plotType switch
        {
            PlotType.Signal => new(new(), null, null, null, null),
            PlotType.Scatter => CreateScatterInitialization(),
            PlotType.DataLogger => CreateDataLoggerInitialization(),
            PlotType.Streamer => CreateStreamerInitialization(),
            PlotType.SignalXY or PlotType.Line or PlotType.StepLine or PlotType.Area or PlotType.Bar or PlotType.Stem or PlotType.Points => new(null, null, null, null, null),
            _ => throw new ArgumentOutOfRangeException(nameof(plotType), plotType, "Unsupported reactive plot type."),
        };

    /// <summary>Creates scatter adapter state.</summary>
    /// <returns>The initialized adapter state.</returns>
    private PlotAdapterInitialization CreateScatterInitialization()
    {
        Signal<(string? Name, IList<double>? X, IList<double> Y, int Axis)> scatterSubject = new();
        return new(null, scatterSubject, null, null, new ScatterUI(_chart.WpfPlot1vm!, scatterSubject, _color));
    }

    /// <summary>Creates data logger adapter state.</summary>
    /// <returns>The initialized adapter state.</returns>
    private PlotAdapterInitialization CreateDataLoggerInitialization()
    {
        Signal<(string? Name, IList<double>? Value, int Axis, int nPoints)> dataLoggerSubject = new();
        return new(null, null, dataLoggerSubject, null, new DataLoggerUI(_chart.WpfPlot1vm!, dataLoggerSubject, _color));
    }

    /// <summary>Creates streamer adapter state.</summary>
    /// <returns>The initialized adapter state.</returns>
    private PlotAdapterInitialization CreateStreamerInitialization()
    {
        Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)> streamerSubject = new();
        return new(
            null,
            null,
            null,
            streamerSubject,
            new StreamerUI(
                _chart.WpfPlot1vm!,
                streamerSubject,
                fs: 1,
                sampleCount: 1,
                plottedPointCount: Math.Max(1, _chart.NumberPointsPlotted),
                _color));
    }

    /// <summary>Ensures the shared chart X axis matches the incoming series.</summary>
    /// <param name="axisKind">The incoming X-axis interpretation.</param>
    private void ConfigureXAxis(PlotXAxisKind axisKind)
    {
        var isDateTime = axisKind is PlotXAxisKind.OADate or PlotXAxisKind.Ticks;
        if (_chart.IsXAxisDateTime == isDateTime)
        {
            return;
        }

        if (isDateTime)
        {
            _chart.CreateAxisWithTimeStamp();
        }
        else
        {
            _chart.CreateAxisWithPoints();
        }

        _chart.IsXAxisDateTime = isDateTime;
    }

    /// <summary>Throws when the adapter has been disposed.</summary>
    private void EnsureNotDisposed() =>
        ThrowHelper.ThrowIfDisposed(_disposed, this);

    /// <summary>Applies clear semantics for clear and replace updates.</summary>
    /// <param name="update">The update value.</param>
    /// <returns><see langword="true"/> when the update was a terminal clear.</returns>
    private bool TryApplyClear(ReactivePlotUpdate update) =>
        update.Kind switch
        {
            ReactivePlotUpdateKind.Clear => ApplyClear(update, true),
            ReactivePlotUpdateKind.Replace => ApplyClear(update, false),
            _ => false,
        };

    /// <summary>Applies a non-clear plot update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyPlotUpdate(ReactivePlotUpdate update)
    {
        var apply = ResolvePlotUpdateApplier();
        apply(update);
    }

    /// <summary>Resolves the update applier for the current plot type.</summary>
    /// <returns>The update applier.</returns>
    private Action<ReactivePlotUpdate> ResolvePlotUpdateApplier() =>
        PlotType switch
        {
            PlotType.Signal => ApplySignal,
            PlotType.Scatter => ApplyScatter,
            PlotType.DataLogger => ApplyDataLogger,
            PlotType.Streamer => ApplyStreamer,
            PlotType.SignalXY => ApplySignalXySnapshot,
            PlotType.Line or PlotType.StepLine or PlotType.Area or PlotType.Bar or PlotType.Stem or PlotType.Points => ApplySnapshot,
            _ => throw new ArgumentOutOfRangeException(
                nameof(PlotType),
                PlotType,
                "The plot type is not supported by the WPF adapter."),
        };

    /// <summary>Applies a signal XY update after preparing snapshot values.</summary>
    /// <param name="update">The update value.</param>
    private void ApplySignalXySnapshot(ReactivePlotUpdate update) =>
        ApplySignalXy(PrepareSnapshotUpdate(update));

    /// <summary>Renders a retained snapshot using one of the extended ScottPlot chart types.</summary>
    /// <param name="update">The source update.</param>
    private void ApplySnapshot(ReactivePlotUpdate update)
    {
        var snapshot = PrepareSnapshotUpdate(update);
        var x = ConvertXValues(snapshot.X, snapshot.XAxisKind);
        var y = CopyToArray(snapshot.Y);
        var plot =
            _chart.WpfPlot1vm?.Plot ?? throw new InvalidOperationException("The WPF plot has not been initialized.");
        RemoveSnapshotPlottable();
        _snapshotPlottable = PlotType switch
        {
            PlotType.Line => plot.Add.ScatterLine(x, y),
            PlotType.StepLine => CreateStepLine(plot, x, y),
            PlotType.Area => CreateArea(plot, x, y, snapshot.Style),
            PlotType.Bar => plot.Add.Bars(y, x),
            PlotType.Stem => plot.Add.Lollipop(x, y),
            PlotType.Points => plot.Add.ScatterPoints(x, y),
            _ => throw new InvalidOperationException($"Plot type '{PlotType}' is not a snapshot chart type."),
        };

        SetLegendText(_snapshotPlottable, _settings.SeriesLabel, snapshot.Style?.ShowInLegend ?? true);
    }

    /// <summary>Applies immutable styling to the active series.</summary>
    /// <param name="style">The optional series style.</param>
    private void ApplyStyle(ReactivePlotSeriesStyle? style)
    {
        if (style is null)
        {
            return;
        }

        ApplyUiSettings(style);

        if (_snapshotPlottable is null)
        {
            return;
        }

        var color = ResolveColor(style.Color ?? _color);
        SetLegendText(_snapshotPlottable, _settings.SeriesLabel, style.ShowInLegend);
        _snapshotPlottable.IsVisible = style.LineMode != PlotLineMode.Hidden;
        if (PlotType == PlotType.Area && _snapshotPlottable is Scatter area)
        {
            area.FillY = style.BaselineMode != PlotBaselineMode.None;
            area.FillYValue = style.BaselineMode == PlotBaselineMode.Custom ? style.Baseline : 0;
        }

        var apply = _snapshotPlottable switch
        {
            Scatter scatter => new Action(() => ApplyScatterStyle(scatter, color, style)),
            BarPlot bars => () => bars.Color = color,
            LollipopPlot stem => () => ApplyStemStyle(stem, color, style),
            _ => null,
        };
        apply?.Invoke();

        static void ApplyScatterStyle(Scatter scatter, Color color, ReactivePlotSeriesStyle style)
        {
            scatter.LineColor = color;
            scatter.MarkerColor = color;
            scatter.LineWidth = style.LineMode == PlotLineMode.MarkersOnly ? 0 : style.LineWidth;
            scatter.MarkerSize = style.LineMode == PlotLineMode.LineOnly ? 0 : style.MarkerSize;
        }

        static void ApplyStemStyle(LollipopPlot stem, Color color, ReactivePlotSeriesStyle style)
        {
            stem.LineColor = color;
            stem.MarkerColor = color;
            stem.LineWidth = style.LineMode == PlotLineMode.MarkersOnly ? 0 : style.LineWidth;
            stem.MarkerSize = style.LineMode == PlotLineMode.LineOnly ? 0 : style.MarkerSize;
        }
    }

    /// <summary>Updates settings on a UI-backed series.</summary>
    /// <param name="style">The effective style.</param>
    private void ApplyUiSettings(ReactivePlotSeriesStyle style)
    {
        if (_ui is null)
        {
            return;
        }

        _applyingSettings = true;
        try
        {
            _ui.ChartSettings.LineWidth = style.LineWidth;
            _ui.ChartSettings.ItemName = _settings.SeriesLabel;
            _ui.NumberPointsPlotted = GetPointLimit();
            _ui.UseFixedNumberOfPoints = GetPointLimit() != int.MaxValue;
            _ui.ChartSettings.IsCrossHairVisible = _settings.IsCrossHairVisible;
            _ui.ChartSettings.IsChecked = style.LineMode != PlotLineMode.Hidden;
            _ui.ChartSettings.Visibility = style.LineMode == PlotLineMode.Hidden ? "Invisible" : "Visible";
            if (!string.IsNullOrWhiteSpace(style.Color))
            {
                _ui.ChartSettings.Color = style.Color;
            }

            ApplyUiStyle(style);
        }
        finally
        {
            _applyingSettings = false;
        }
    }

    /// <summary>Preserves edits made through the existing external series selector.</summary>
    /// <param name="sender">The chart settings.</param>
    /// <param name="args">The changed property.</param>
    private void UiSettingsChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (_applyingSettings || !_settingsInitialized || _ui is null)
        {
            return;
        }

        if (args.PropertyName == nameof(ChartObjects.IsChecked))
        {
            _settings.IsVisible = _ui.ChartSettings.IsChecked;
        }

        if (args.PropertyName != nameof(ChartObjects.IsCrossHairVisible))
        {
            return;
        }

        _settings.IsCrossHairVisible = _ui.ChartSettings.IsCrossHairVisible;
    }

    /// <summary>Refreshes presentation immediately when settings change.</summary>
    /// <param name="sender">The settings instance.</param>
    /// <param name="args">The changed property.</param>
    private void SettingsChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (!_settingsInitialized || _disposed)
        {
            return;
        }

        ApplyStyle(_settings.CreateStyle());
        TrimUiPoints();
        if (args.PropertyName == nameof(ReactivePlotSeriesSettings.MaxPoints))
        {
            ReapplyRetainedData();
        }

        _chart.WpfPlot1vm?.Refresh();
    }

    /// <summary>Updates static feeds when the global display window changes.</summary>
    /// <param name="sender">The chart view model.</param>
    /// <param name="args">The changed property.</param>
    private void ChartSettingsChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is not (nameof(LiveChartViewModel.NumberPointsPlotted) or nameof(LiveChartViewModel.UseFixedNumberOfPoints)))
        {
            return;
        }

        ReapplyRetainedData();
        ApplyStyle(_settings.CreateStyle());
        TrimUiPoints();
        _chart.WpfPlot1vm?.Refresh();
    }

    /// <summary>Redraws retained snapshot data without awaiting another source emission.</summary>
    private void ReapplyRetainedData()
    {
        if (_retainedX.Count == 0 || _disposed)
        {
            return;
        }

        var replay = new ReactivePlotUpdate(
            Key,
            PlotType,
            ReactivePlotUpdateKind.Replace,
            _retainedX.ToArray(),
            _retainedY.ToArray(),
            _lastXAxisKind,
            0,
            _settings.MaxPoints,
            _settings.CreateStyle());
        ApplyPlotUpdate(replay);
        ApplyStyle(replay.Style);
        AssignAxis(Key.Axis);
    }

    /// <summary>Enforces the editable retained point limit on logger-backed series.</summary>
    private void TrimUiPoints()
    {
        var maximum = GetPointLimit();
        if (maximum == int.MaxValue)
        {
            return;
        }

        var logger = _ui switch
        {
            SignalUI signal => signal.PlotLine,
            DataLoggerUI dataLogger => dataLogger.PlotLine,
            _ => null,
        };

        if (logger is null || logger.Data.Coordinates.Count <= maximum)
        {
            return;
        }

        logger.Data.Coordinates.RemoveRange(0, logger.Data.Coordinates.Count - maximum);
    }

    /// <summary>Resolves the tightest configured presentation point limit.</summary>
    /// <returns>The active point limit.</returns>
    private int GetPointLimit() => _chart.UseFixedNumberOfPoints
        ? Math.Min(_settings.MaxPoints ?? int.MaxValue, Math.Max(1, _chart.NumberPointsPlotted))
        : _settings.MaxPoints ?? int.MaxValue;

    /// <summary>Applies line, marker, and legend settings to existing UI series.</summary>
    /// <param name="style">The effective series style.</param>
    private void ApplyUiStyle(ReactivePlotSeriesStyle style)
    {
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

        plottable.IsVisible = style.LineMode != PlotLineMode.Hidden;
        var color = ResolveColor(style.Color ?? _color);
        var line = (IHasLine)plottable;
        line.LineStyle.Width = style.LineMode == PlotLineMode.MarkersOnly ? 0 : style.LineWidth;
        line.LineStyle.Color = color;

        var marker = (IHasMarker)plottable;
        marker.MarkerStyle.Size = style.LineMode == PlotLineMode.LineOnly ? 0 : style.MarkerSize;
        marker.MarkerStyle.FillColor = color;
        marker.MarkerStyle.LineColor = color;

        ApplyStreamerSettings();

        SetUiLegendText(plottable, _settings.SeriesLabel, style.ShowInLegend);
    }

    /// <summary>Applies the fixed-buffer stream view and sample interval.</summary>
    private void ApplyStreamerSettings()
    {
        if (_ui is not StreamerUI { PlotLine: { } streamer })
        {
            return;
        }

        streamer.Period = _settings.SamplePeriod;
        var capacity = _settings.MaxPoints ?? Math.Max(1, _chart.NumberPointsPlotted);
        if (streamer.Data.Length != capacity)
        {
            streamer.Data = new(new double[capacity]);
        }

        var apply = _settings.StreamerViewMode switch
        {
            StreamerViewMode.ScrollLeft => new Action(streamer.ViewScrollLeft),
            StreamerViewMode.ScrollRight => streamer.ViewScrollRight,
            StreamerViewMode.WipeLeft => streamer.ViewWipeLeft,
            StreamerViewMode.WipeRight => () => streamer.ViewWipeRight(),
            _ => throw new ArgumentOutOfRangeException(nameof(_settings.StreamerViewMode)),
        };
        apply();
    }

    /// <summary>Applies a signal update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplySignal(ReactivePlotUpdate update)
    {
        EnsureSignalUi(update.XAxisKind);
        _signalSubject?.OnNext((update.Key.Name, CopyToList(update.Y), CopyToList(update.X), update.Key.Axis));
    }

    /// <summary>Applies a scatter update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyScatter(ReactivePlotUpdate update)
    {
        var scatterUpdate = PrepareSnapshotUpdate(update);
        _scatterSubject?.OnNext(
            (scatterUpdate.Key.Name, CopyToList(scatterUpdate.X), CopyToList(scatterUpdate.Y), scatterUpdate.Key.Axis));
    }

    /// <summary>Applies a data logger update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyDataLogger(ReactivePlotUpdate update) =>
        _dataLoggerSubject?.OnNext(
            (update.Key.Name, CopyToList(update.Y), update.Key.Axis, Math.Min(update.MaxPoints ?? int.MaxValue, GetPointLimit())));

    /// <summary>Applies a streamer update.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyStreamer(ReactivePlotUpdate update) =>
        _streamerSubject?.OnNext((update.Key.Name, CopyToList(update.Y), CopyToList(update.X), update.Key.Axis));

    /// <summary>Handles the AddUi operation.</summary>
    private void AddUi()
    {
        if (_ui is null)
        {
            return;
        }

        _chart.PlotLinesCollectionUI.Add(_ui);
        _ui.ChartSettings.PropertyChanged += UiSettingsChanged;
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
            _ui.ChartSettings.PropertyChanged -= UiSettingsChanged;
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
            new SignalUIOptions
            {
                FixedPoints = _chart.WhenAnyValue(x => x.UseFixedNumberOfPoints),
                NumberPointsPlotted = _chart.WhenAnyValue(x => x.NumberPointsPlotted),
                Ticks = axisKind == PlotXAxisKind.Ticks,
            });
        AddUi();
    }

    /// <summary>Handles the PrepareSnapshotUpdate operation.</summary>
    /// <param name="update">The update value.</param>
    /// <returns>The result.</returns>
    private ReactivePlotUpdate PrepareSnapshotUpdate(ReactivePlotUpdate update)
    {
        if (update.Kind == ReactivePlotUpdateKind.Replace || _incomingSnapshot)
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
        var maxPoints = _settings.MaxPoints ?? int.MaxValue;
        if (_retainedX.Count > maxPoints)
        {
            var excess = _retainedX.Count - maxPoints;
            _retainedX.RemoveRange(0, excess);
            _retainedY.RemoveRange(0, excess);
        }

        return update with
        {
            X = CopyTail(_retainedX, GetPointLimit()),
            Y = CopyTail(_retainedY, GetPointLimit()),
        };
    }

    /// <summary>Handles the ApplySignalXy operation.</summary>
    /// <param name="update">The update value.</param>
    private void ApplySignalXy(ReactivePlotUpdate update)
    {
        if (_ui is not SignalXY_UI signalXy)
        {
            _ui = new SignalXY_UI(
                _chart.WpfPlot1vm!,
                (update.Key.Name, CopyToList(update.Y), CopyToList(update.X), update.Key.Axis),
                _color,
                coordinatesObs: _chart.MouseCoordinatesObservable);
            AddUi();
            return;
        }

        signalXy.PlotLine!.Data = new SignalXYSourceDoubleArray(CopyToArray(update.X), CopyToArray(update.Y));
    }

    /// <summary>Handles the ApplyClear operation.</summary>
    /// <param name="update">The update value.</param>
    private void ApplyClear(ReactivePlotUpdate update)
    {
        _retainedX.Clear();
        _retainedY.Clear();

        ClearUi();
        RemoveSnapshotPlottable();

        AssignAxis(update.Key.Axis);
        _chart.WpfPlot1vm?.Refresh();
    }

    /// <summary>Applies a clear operation and returns the requested result.</summary>
    /// <param name="update">The update value.</param>
    /// <param name="result">The result to return.</param>
    /// <returns>The supplied result.</returns>
    private bool ApplyClear(ReactivePlotUpdate update, bool result)
    {
        ApplyClear(update);
        return result;
    }

    /// <summary>Clears the current UI element.</summary>
    private void ClearUi()
    {
        var clear = ResolveClearUiAction();
        clear?.Invoke();
    }

    /// <summary>Resolves the clear action for the current UI element.</summary>
    /// <returns>The clear action, or <see langword="null"/> when no UI element is active.</returns>
    private Action? ResolveClearUiAction() =>
        _ui switch
        {
            SignalXY_UI signalXy => () => ClearSignalXy(signalXy),
            ScatterUI scatter => () => scatter.InsertData([], []),
            SignalUI signal => signal.ClearData,
            DataLoggerUI dataLogger => () => dataLogger.PlotLine!.Data.Coordinates.Clear(),
            StreamerUI streamer => () => ClearStreamer(streamer),
            _ => null,
        };

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
        if (axis < 0 || axis >= _chart.YAxisList.Count)
        {
            return;
        }

        var verticalAxis = _chart.YAxisList[axis];
        if (!_axisAssigned)
        {
            verticalAxis.IsVisible = true;
            _axisAssigned = true;
        }

        if (_ui is not null)
        {
            _ui.ChartSettings.Marker!.Axes.YAxis = verticalAxis;
            _ui.ChartSettings.MarkerText!.Axes.YAxis = verticalAxis;
            _ui.ChartSettings.Crosshair!.Axes.YAxis = verticalAxis;
        }

        IPlottable? plottable = _ui switch
        {
            SignalUI signal => signal.PlotLine,
            ScatterUI scatter => scatter.PlotLine,
            DataLoggerUI logger => logger.PlotLine,
            StreamerUI streamer => streamer.PlotLine,
            SignalXY_UI signalXy => signalXy.PlotLine,
            _ => _snapshotPlottable,
        };

        if (plottable is null)
        {
            return;
        }

        plottable.Axes.YAxis = verticalAxis;
        plottable.Axes.XAxis = _chart.XAxis1;
    }

    /// <summary>Removes the current snapshot plottable from the underlying plot.</summary>
    private void RemoveSnapshotPlottable()
    {
        if (_snapshotPlottable is null)
        {
            return;
        }

        _chart.WpfPlot1vm?.Plot.Remove(_snapshotPlottable);
        _snapshotPlottable = null;
    }

    /// <summary>Adds the UI element when the current plot type owns one.</summary>
    private void AddUiIfPresent()
    {
        if (_ui is null)
        {
            return;
        }

        AddUi();
    }

    /// <summary>Stores the constructor output for plot-type specific adapter state.</summary>
    /// <param name="SignalSubject">The signal subject.</param>
    /// <param name="ScatterSubject">The scatter subject.</param>
    /// <param name="DataLoggerSubject">The data logger subject.</param>
    /// <param name="StreamerSubject">The streamer subject.</param>
    /// <param name="Ui">The UI plottable.</param>
    private sealed record PlotAdapterInitialization(
        Signal<(string? Name, IList<double>? Value, IList<double> X, int Axis)>? SignalSubject,
        Signal<(string? Name, IList<double>? X, IList<double> Y, int Axis)>? ScatterSubject,
        Signal<(string? Name, IList<double>? Value, int Axis, int nPoints)>? DataLoggerSubject,
        Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)>? StreamerSubject,
        IPlottableUI? Ui);
}
