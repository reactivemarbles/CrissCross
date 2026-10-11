// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using ReactiveUI.Primitives.Concurrency;
using ScottPlot;
using ScottPlot.Plottables;
using SkiaSharp.Views.Maui;
#if REACTIVE_SHIM
using ReactiveUI.Primitives.Reactive.Concurrency;
#endif

using Color = ScottPlot.Color;
using Colors = ScottPlot.Colors;
using Label = Microsoft.Maui.Controls.Label;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>Native MAUI live plotting with lifecycle-owned reactive sources and editable settings.</summary>
public sealed partial class LiveChart : ContentView, IDisposable
{
    /// <summary>Native editor spacing.</summary>
    private const int EditorSpacing = 8;

    /// <summary>Native editor width.</summary>
    private const int EditorWidth = 300;

    /// <summary>Plot layout row.</summary>
    private const int PlotRow = 2;

    /// <summary>Stores the native title.</summary>
    private readonly Label _title = new();

    /// <summary>Stores the native settings editor.</summary>
    private readonly VerticalStackLayout _settings = new() { Spacing = EditorSpacing, Padding = 12 };

    /// <summary>Stores the reusable appearance editor.</summary>
    private readonly RightPropertiesView _rightProperties = new();

    /// <summary>Stores the editor scroll view.</summary>
    private readonly ScrollView _settingsScroll;

    /// <summary>Owns active sources.</summary>
    private IReactivePlotConnection? _reactivePlotConnection;

    /// <summary>Owns legacy crosshair observations.</summary>
    private IDisposable? _crosshairDisposable;

    /// <summary>Stores the draggable axis line.</summary>
    private AxisLine? _dragged;

    /// <summary>Stores whether the chart is locked.</summary>
    private bool _needLock = true;

    /// <summary>Stores whether automatic scaling is active.</summary>
    private bool _needAutoScale = true;

    /// <summary>Stores the update batch duration.</summary>
    private double _batchWindowMilliseconds;

    /// <summary>Stores the maximum batch size.</summary>
    private int _maximumBatchSize = 1;

    /// <summary>Stores the retention overflow policy.</summary>
    private ReactivePlotOverflowStrategy _overflowStrategy;

    /// <summary>Stores the source failure policy.</summary>
    private ReactivePlotErrorMode _errorMode;

    /// <summary>Tracks native detachment.</summary>
    private bool _detached;

    /// <summary>Tracks the selected legacy source input.</summary>
    private string? _lastSourceProperty;

    /// <summary>Enables tap-created coordinate markers.</summary>
    private bool _coordinateMarkers;

    /// <summary>Stores disposal state.</summary>
    private bool _disposed;

    /// <summary>Coalesces queued settings rebuilds during source replacement.</summary>
    private bool _settingsRebuildPending;

    /// <summary>Initializes a new instance of the <see cref="LiveChart"/> class.</summary>
    public LiveChart()
    {
        var host = new Grid();
        ViewModel = new(host) { NumberPointsPlotted = NumberPointsPlotted };
        ControlMenu = ViewModel.ControlMenu;
        var toolbar = new HorizontalStackLayout { Spacing = EditorSpacing };
        AddButton(toolbar, "Settings", OpenPlotSettings);
        AddButton(toolbar, "Lock", () =>
        {
            _needLock = !_needLock;
            ExecuteLockUnlock();
        });
        AddButton(toolbar, "Auto scale", () =>
        {
            _needAutoScale = !_needAutoScale;
            ExecuteManAutoScale();
        });
        AddButton(toolbar, "Pointer", () => SetPointerValues(!ViewModel.CrossHairEnabled));
        AddButton(toolbar, "Crosshair", ViewModel.AddCrosshair);
        AddButton(toolbar, "Label", () => _coordinateMarkers = !_coordinateMarkers);
        AddButton(toolbar, "Clear labels", ViewModel.ClearLabels);
        AddButton(toolbar, "Clear crosshairs", ViewModel.ClearAxisCrosshairs);
        _settingsScroll = new() { Content = _settings, IsVisible = false, WidthRequest = EditorWidth };
        var body = new Grid { ColumnDefinitions = { new(GridLength.Star), new(GridLength.Auto) } };
        body.Add(host);
        body.Add(_settingsScroll, 1);
        var layout = new Grid { RowDefinitions = { new(GridLength.Auto), new(GridLength.Auto), new(GridLength.Star) } };
        layout.Add(_title);
        layout.Add(new ScrollView { Content = toolbar, Orientation = ScrollOrientation.Horizontal }, 0, 1);
        layout.Add(body, 0, PlotRow);
        Content = layout;
        ViewModel.MauiPlot1vm!.Touch += PlotTouch;
        ViewModel.MauiPlot1vm.EnableTouchEvents = true;
        var tap = new TapGestureRecognizer();
        tap.Tapped += PlotTapped;
        ViewModel.MauiPlot1vm.GestureRecognizers.Add(tap);
        ViewModel.PropertyChanged += ModelChanged;
        ((INotifyCollectionChanged)ViewModel.SeriesSettings).CollectionChanged += SeriesChanged;
        Loaded += ChartLoaded;
        Unloaded += ChartUnloaded;
        ExecuteLockUnlock();
        ExecuteManAutoScale();
        ApplyTheme(ReactivePlotTheme.Dark);
    }

    /// <summary>Gets the owned chart view model.</summary>
    public LiveChartViewModel ViewModel
    {
        get;
    }

    /// <summary>Gets or sets whether the next legacy update is the first.</summary>
    public bool First
    {
        get;
        set;
    } = true;

    /// <summary>Gets the native series settings panel.</summary>
    public RightPropertiesView RightPropertiesPanel => _rightProperties;

    /// <summary>Gets the native chart title.</summary>
    public Label ChartTitleTextBlock => _title;

    /// <summary>Gets the active reactive connection.</summary>
    public IReactivePlotConnection? ReactivePlotConnection => _reactivePlotConnection;

    /// <summary>Applies a plot and control theme.</summary>
    /// <param name="theme">The theme.</param>
    public void ApplyTheme(ReactivePlotTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ViewModel.ApplyTheme(theme);
        BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb(theme.FigureBackground);
        _title.TextColor = Microsoft.Maui.Graphics.Color.FromArgb(theme.LegendForeground);
        _settings.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb(theme.LegendBackground);
    }

    /// <summary>Reconnects the configured sources.</summary>
    public void BindReactivePlotSources() => RefreshReactivePlotSources();

    /// <summary>Shows the native chart and series editors.</summary>
    public void OpenPlotSettings()
    {
        _settingsScroll.IsVisible = !_settingsScroll.IsVisible;
        if (!_settingsScroll.IsVisible)
        {
            return;
        }

        RebuildSettings();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Loaded -= ChartLoaded;
        Unloaded -= ChartUnloaded;
        ViewModel.MauiPlot1vm!.Touch -= PlotTouch;
        DisposeReactivePlotConnection();
        ViewModel.PropertyChanged -= ModelChanged;
        ((INotifyCollectionChanged)ViewModel.SeriesSettings).CollectionChanged -= SeriesChanged;
        _rightProperties.Dispose();
        ViewModel.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>Identifies source inputs.</summary>
    /// <param name="property">The property.</param>
    /// <returns>Whether the property owns subscriptions.</returns>
    private static bool IsSourceProperty(string property) => property is nameof(ReactivePlotSources) or nameof(Observables)
        or nameof(SignalObservablesWithTimeStamp) or nameof(DataLoggerObservablesWithPoints) or nameof(DataWithTimeStamp)
        or nameof(SignalWithPoints) or nameof(SignalsWithPoints) or nameof(SignalObservablesWithPoints)
        or nameof(ScatterObservablesWithTimeStamp) or nameof(ScatterWithPoints);

    /// <summary>Validates an edited color.</summary>
    /// <param name="value">The color.</param>
    /// <returns>Whether ScottPlot accepts the color.</returns>
    private static bool IsColor(string value)
    {
        if (!value.StartsWith('#'))
        {
            return System.Drawing.Color.FromName(value).IsKnownColor;
        }

        try
        {
            _ = ScottPlot.Color.FromHex(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>Adds a toolbar action.</summary>
    /// <param name="toolbar">The toolbar.</param>
    /// <param name="text">The caption.</param>
    /// <param name="action">The action.</param>
    private static void AddButton(HorizontalStackLayout toolbar, string text, Action action)
    {
        var button = new Button { Text = text };
        button.Clicked += (sender, args) => action();
        toolbar.Add(button);
    }

    /// <summary>Rebuilds the current editor snapshot.</summary>
    private void RebuildSettings()
    {
        if (_disposed || _detached || !_settingsScroll.IsVisible)
        {
            return;
        }

        _settings.Children.Clear();
        AddTextEditor("Chart title", TitleContent, value => TitleContent = value);
        AddEnumEditor("Legend", LegendPosition, value => LegendPosition = value);
        AddToggle("Fixed point window", UseFixedNumberOfPoints, value => UseFixedNumberOfPoints = value);
        AddNumberEditor("Displayed points", NumberPointsPlotted, value => NumberPointsPlotted = Math.Max(1, (int)value));
        AddNumberEditor("Batch window (ms)", _batchWindowMilliseconds, value =>
        {
            _batchWindowMilliseconds = Math.Max(0, value);
            RefreshReactivePlotSources();
        });
        AddNumberEditor("Maximum batch", _maximumBatchSize, value =>
        {
            _maximumBatchSize = Math.Max(1, (int)value);
            RefreshReactivePlotSources();
        });
        AddEnumEditor("Overflow", _overflowStrategy, value =>
        {
            _overflowStrategy = value;
            RefreshReactivePlotSources();
        });
        AddEnumEditor("Errors", _errorMode, value =>
        {
            _errorMode = value;
            RefreshReactivePlotSources();
        });
        AddSeriesEditors();
        AddLegacyEditors();
        _settings.Add(_rightProperties);
    }

    /// <summary>Adds retained series editors.</summary>
    private void AddSeriesEditors()
    {
        foreach (var series in ViewModel.SeriesSettings)
        {
            _settings.Add(new Label { Text = $"{series.SeriesLabel} ({series.PlotType})", FontAttributes = FontAttributes.Bold });
            AddTextEditor("Name", series.SeriesLabel ?? string.Empty, value => series.SeriesLabel = value);
            AddTextEditor("Color", series.Color ?? "#377eb8", value =>
            {
                if (!IsColor(value))
                {
                    return;
                }

                series.Color = value;
            });
            AddToggle("Visible", series.IsVisible, value => series.IsVisible = value);
            AddToggle("Paused", series.IsPaused, value => series.IsPaused = value);
            AddToggle("Pointer values", series.IsCrossHairVisible, value => series.IsCrossHairVisible = value);
            AddToggle("Show in legend", series.ShowInLegend, value => series.ShowInLegend = value);
            AddNumberEditor("Line width", series.LineWidth, value => series.LineWidth = (float)Math.Max(0, value));
            AddNumberEditor("Marker size", series.MarkerSize, value => series.MarkerSize = (float)Math.Max(0, value));
            AddNumberEditor("Point limit (0 = unlimited)", series.MaxPoints ?? 0, value => series.MaxPoints = value > 0 ? (int)value : null);
            AddNumberEditor("Sample period", series.SamplePeriod, value => series.SamplePeriod = Math.Max(double.Epsilon, value));
            AddEnumEditor("Line mode", series.LineMode, value => series.LineMode = value);
            AddEnumEditor("Baseline mode", series.BaselineMode, value => series.BaselineMode = value);
            AddNumberEditor("Baseline", series.Baseline, value => series.Baseline = value);
            AddEnumEditor("Streamer view", series.StreamerViewMode, value => series.StreamerViewMode = value);
        }
    }

    /// <summary>Adds legacy item editors.</summary>
    private void AddLegacyEditors()
    {
        foreach (var item in ViewModel.ChartObjectsCollection)
        {
            var select = new Button { Text = $"Edit {item.ItemName}" };
            select.Clicked += (sender, args) => ViewModel.SelectedSetting = item;
            _settings.Add(select);
            _settings.Add(new Label { Text = item.ItemName ?? string.Empty, FontAttributes = FontAttributes.Bold });
            AddTextEditor("Name", item.ItemName ?? string.Empty, value => item.ItemName = value);
            AddTextEditor("Color", item.Color ?? "#377eb8", value =>
            {
                if (!IsColor(value))
                {
                    return;
                }

                item.Color = value;
            });
            AddToggle("Visible", item.IsChecked, value => item.IsChecked = value);
            AddToggle("Paused", item.IsPaused, value => item.IsPaused = value);
            AddToggle("Pointer values", item.IsCrossHairVisible, value => item.IsCrossHairVisible = value);
            AddNumberEditor("Line width", item.LineWidth, value => item.LineWidth = (float)Math.Max(0, value));
        }
    }

    /// <summary>Adds an editable string.</summary>
    /// <param name="label">The caption.</param>
    /// <param name="value">The value.</param>
    /// <param name="set">The setter.</param>
    private void AddTextEditor(string label, string value, Action<string> set)
    {
        _settings.Add(new Label { Text = label });
        var entry = new Entry { Text = value };
        var current = value;
        entry.TextChanged += (sender, args) =>
        {
            if (args.NewTextValue == current)
            {
                return;
            }

            current = args.NewTextValue;
            set(current);
        };
        _settings.Add(entry);
    }

    /// <summary>Adds an editable number.</summary>
    /// <param name="label">The caption.</param>
    /// <param name="value">The value.</param>
    /// <param name="set">The setter.</param>
    private void AddNumberEditor(string label, double value, Action<double> set) =>
        AddTextEditor(label, value.ToString(System.Globalization.CultureInfo.InvariantCulture), text =>
        {
            if (!double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed) || !double.IsFinite(parsed))
            {
                return;
            }

            set(parsed);
        });

    /// <summary>Adds an editable boolean.</summary>
    /// <param name="label">The caption.</param>
    /// <param name="value">The value.</param>
    /// <param name="set">The setter.</param>
    private void AddToggle(string label, bool value, Action<bool> set)
    {
        var toggle = new Switch { IsToggled = value };
        var current = value;
        toggle.Toggled += (sender, args) =>
        {
            if (args.Value == current)
            {
                return;
            }

            current = args.Value;
            set(current);
        };
        var row = new HorizontalStackLayout() { new Label { Text = label, VerticalOptions = LayoutOptions.Center } };
        row.Add(toggle);
        _settings.Add(row);
    }

    /// <summary>Adds an enum selector.</summary>
    /// <typeparam name="T">The enum.</typeparam>
    /// <param name="label">The caption.</param>
    /// <param name="value">The value.</param>
    /// <param name="set">The setter.</param>
    private void AddEnumEditor<T>(string label, T value, Action<T> set)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        var picker = new Picker { Title = label, ItemsSource = values, SelectedItem = value };
        picker.SelectedIndexChanged += (sender, args) =>
        {
            if (picker.SelectedItem is not T selected || EqualityComparer<T>.Default.Equals(selected, value))
            {
                return;
            }

            value = selected;
            set(selected);
        };
        _settings.Add(picker);
    }

    /// <summary>Applies a changed bindable input.</summary>
    /// <param name="property">The input name.</param>
    private void InputChanged(string property)
    {
        if (ViewModel is null)
        {
            return;
        }

        if (IsSourceProperty(property))
        {
            _lastSourceProperty = property;
            DisposeReactivePlotConnection();
        }

        if (_detached)
        {
            return;
        }

        Action? apply = property switch
        {
            nameof(TitleContent) => UpdateTitle,
            nameof(LegendPosition) => UpdateLegend,
            nameof(NumberPointsPlotted) => () => ViewModel.NumberPointsPlotted = NumberPointsPlotted,
            nameof(UseFixedNumberOfPoints) => () => ViewModel.UseFixedNumberOfPoints = UseFixedNumberOfPoints,
            nameof(YAxisName) => UpdateAxes,
            nameof(ReactivePlotSources) => RefreshReactivePlotSources,
            nameof(SignalObservablesWithTimeStamp) => ChangeSignalObserver,
            nameof(DataLoggerObservablesWithPoints) => ChangeDataLoggerObserver,
            nameof(DataWithTimeStamp) => ChangeSignalData,
            nameof(SignalWithPoints) => ChangeSignalDataWithPoints,
            nameof(SignalsWithPoints) => ChangeSignalsDataWithPoints,
            nameof(SignalObservablesWithPoints) => ChangeSignalDataObserverWithPoints,
            nameof(ScatterObservablesWithTimeStamp) => ChangeScatterObserver,
            nameof(ScatterWithPoints) => ChangeScatterDataWithPoints,
            nameof(Observables) => BindLegacyValues,
            _ => null,
        };
        apply?.Invoke();
    }

    /// <summary>Applies title text.</summary>
    private void UpdateTitle()
    {
        _title.Text = TitleContent;
        ViewModel.Title = TitleContent;
    }

    /// <summary>Rebuilds source axes.</summary>
    private void UpdateAxes()
    {
        YAxisSetup();
        RefreshReactivePlotSources();
    }

    /// <summary>Adapts scalar legacy sources.</summary>
    private void BindLegacyValues()
    {
        if (Observables is null)
        {
            return;
        }

        List<IReactivePlotSource> sources = [];
        foreach (var source in Observables)
        {
            sources.Add(ReactivePlotSource.FromDataLoggerPoints(
                source.Select(static value => (Name: (string?)value.Name, Value: (IList<double>?)new double[] { value.Value }, Axis: 0, nMaxPoints: 600))));
        }

        ReactivePlotSources = sources;
    }

    /// <summary>Applies axis metadata.</summary>
    private void YAxisSetup()
    {
        if (YAxisName.yNames is not { Count: > 0 } || YAxisName.hexColors is not { Count: > 0 })
        {
            return;
        }

        ViewModel.YAxesSetup(YAxisName);
    }

    /// <summary>Reconnects current sources.</summary>
    private void RefreshReactivePlotSources()
    {
        if (_disposed || _detached || ReactivePlotSources is null)
        {
            return;
        }

        ChangeReactivePlotSources(ReactivePlotSources);
        UpdateLegend();
    }

    /// <summary>Configures source X semantics.</summary>
    /// <param name="sources">The sources.</param>
    private void ConfigureReactivePlotXAxis(IEnumerable<IReactivePlotSource> sources)
    {
        var isDateTime = false;
        foreach (var source in sources)
        {
            isDateTime |= source.XAxisKind is PlotXAxisKind.OADate or PlotXAxisKind.Ticks;
        }

        if (isDateTime)
        {
            ViewModel.CreateAxisWithTimeStamp();
        }
        else
        {
            ViewModel.CreateAxisWithPoints();
        }
    }

    /// <summary>Applies legend positioning.</summary>
    private void UpdateLegend()
    {
        _ = ViewModel.MauiPlot1vm!.Plot.ShowLegend(LegendPosition == LegendPosition.Right ? Alignment.UpperRight : Alignment.UpperLeft);
        ViewModel.MauiPlot1vm.Refresh();
    }

    /// <summary>Applies navigation lock.</summary>
    private void ExecuteLockUnlock()
    {
        ViewModel.MauiPlot1vm!.UserInputProcessor.IsEnabled = !_needLock;
        ViewModel.MauiPlot1vm.Plot.Axes.ContinuouslyAutoscale = _needLock;
    }

    /// <summary>Applies automatic/manual scale.</summary>
    private void ExecuteManAutoScale()
    {
        ViewModel.MauiPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = _needAutoScale ? LiveChartViewModel.AutoScaleAll() : LiveChartViewModel.AutoScaleX(ViewModel.XAxis1);
        if (!_needAutoScale)
        {
            ViewModel.ManualScaleY();
        }

        ViewModel.MauiPlot1vm.Refresh();
    }

    /// <summary>Applies pointer values.</summary>
    /// <param name="enabled">Whether values are enabled.</param>
    private void SetPointerValues(bool enabled)
    {
        ViewModel.CrossHairEnabled = enabled;
        foreach (var series in ViewModel.SeriesSettings)
        {
            series.IsCrossHairVisible = enabled;
        }

        ApplyCrosshairVisibility(enabled);
        ViewModel.MauiPlot1vm!.Refresh();
    }

    /// <summary>Reconnects on native attachment.</summary>
    /// <param name="sender">The view.</param>
    /// <param name="args">The event.</param>
    private void ChartLoaded(object? sender, EventArgs args)
    {
        _detached = false;
        UpdateTitle();
        ViewModel.NumberPointsPlotted = NumberPointsPlotted;
        ViewModel.UseFixedNumberOfPoints = UseFixedNumberOfPoints;
        YAxisSetup();
        if (_lastSourceProperty is not { } property)
        {
            return;
        }

        InputChanged(property);
    }

    /// <summary>Disconnects on native detachment.</summary>
    /// <param name="sender">The view.</param>
    /// <param name="args">The event.</param>
    private void ChartUnloaded(object? sender, EventArgs args)
    {
        _detached = true;
        DisposeReactivePlotConnection();
        ViewModel.ClearContent();
    }

    /// <summary>Drags native axis markers and publishes touch coordinates.</summary>
    /// <param name="sender">The plot.</param>
    /// <param name="args">The touch.</param>
    private void PlotTouch(object? sender, SKTouchEventArgs args)
    {
        var pixel = new Pixel(args.Location.X, args.Location.Y);
        var coordinate = ViewModel.MauiPlot1vm!.Plot.GetCoordinates(pixel);
        if (args.ActionType == SKTouchAction.Pressed)
        {
            _dragged = ViewModel.GetLineUnderMouse(pixel.X, pixel.Y);
            if (_dragged is not null)
            {
                ViewModel.MauiPlot1vm.UserInputProcessor.Disable();
            }
        }

        if (args.ActionType == SKTouchAction.Released)
        {
            _dragged = null;
            ExecuteLockUnlock();
        }

        if (_dragged is null)
        {
            return;
        }

        if (_dragged is VerticalLine vertical)
        {
            vertical.Position = coordinate.X;
            vertical.Text = ViewModel.IsXAxisDateTime ? DateTime.FromOADate(coordinate.X).ToLongTimeString() : $"{coordinate.X:F2}";
        }

        if (_dragged is HorizontalLine horizontal)
        {
            horizontal.Position = coordinate.Y;
            horizontal.Text = $"{coordinate.Y:F2}";
        }

        args.Handled = true;
        ViewModel.MauiPlot1vm.Refresh();
    }

    /// <summary>Converts MAUI logical positions to the active canvas coordinates.</summary>
    /// <param name="x">The logical X position.</param>
    /// <param name="y">The logical Y position.</param>
    /// <returns>The canvas pixel.</returns>
    private Pixel ToPlotPixel(double x, double y)
    {
        var plot = ViewModel.MauiPlot1vm!;
        var scaleX = plot.Width > 0 && plot.CanvasSize.Width > 0 ? plot.CanvasSize.Width / plot.Width : 1;
        var scaleY = plot.Height > 0 && plot.CanvasSize.Height > 0 ? plot.CanvasSize.Height / plot.Height : 1;
        return new(x * scaleX, y * scaleY);
    }

    /// <summary>Places coordinate labels using a native tap gesture.</summary>
    /// <param name="sender">The gesture.</param>
    /// <param name="args">The tap.</param>
    private void PlotTapped(object? sender, TappedEventArgs args)
    {
        if (!_coordinateMarkers || args.GetPosition(ViewModel.MauiPlot1vm!) is not { } point)
        {
            return;
        }

        var plot = ViewModel.MauiPlot1vm!.Plot;
        var position = plot.GetCoordinates(ToPlotPixel(point.X, point.Y), ViewModel.XAxis1, ViewModel.YAxisList[0]);
        var marker = plot.Add.Marker(position);
        var text = plot.Add.Text($"X: {position.X:F2}\nY: {position.Y:F2}", position);
        marker.Axes.XAxis = ViewModel.XAxis1;
        marker.Axes.YAxis = ViewModel.YAxisList[0];
        text.Axes.XAxis = ViewModel.XAxis1;
        text.Axes.YAxis = ViewModel.YAxisList[0];
        ViewModel.LabelCollection.Add((marker, text));
        ViewModel.MauiPlot1vm.Refresh();
    }

    /// <summary>Synchronizes the reusable selected-item editor.</summary>
    /// <param name="sender">The chart model.</param>
    /// <param name="args">The property event.</param>
    private void ModelChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(LiveChartViewModel.SelectedSetting) || ViewModel.SelectedSetting is not { } item)
        {
            return;
        }

        var editor = _rightProperties.ViewModel!;
        editor.SelectedSetting = item;
        editor.ItemName = item.ItemName;
        editor.LineWidth = item.LineWidth;
        editor.LineColor = item.Color;
        editor.ItemVisibility = item.Visibility;
    }

    /// <summary>Refreshes an open settings editor when series appear or disappear.</summary>
    /// <param name="sender">The series collection.</param>
    /// <param name="args">The collection event.</param>
    private void SeriesChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_settingsScroll.IsVisible || _detached || _disposed || _settingsRebuildPending)
        {
            return;
        }

        _settingsRebuildPending = true;
        if (Dispatcher.Dispatch(() =>
        {
            _settingsRebuildPending = false;
            RebuildSettings();
        }))
        {
            return;
        }

        _settingsRebuildPending = false;
    }
}
