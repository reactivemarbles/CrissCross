// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
#if REACTIVE_SHIM
using CP.Reactive.Collections;
#else
using CP.Primitives.Collections;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Maui;
using ScottPlot.Palettes;
using ScottPlot.Plottables;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ReactiveUIBindingExtensions;
#else
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;
#endif

using Color = ScottPlot.Color;
using Colors = ScottPlot.Colors;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>
/// Represents the view model for a live chart, providing properties and commands to manage chart data, axes, labels,
/// and user interactions in a MAUI application.
/// </summary>
/// <remarks>This class is intended for use with MAUI desktop platforms. It exposes collections for
/// chart elements, axis management, and commands for interactive features such as crosshairs, labels, and menu
/// expansion. The view model supports dynamic updates to chart visuals and user-driven configuration. Thread safety is
/// not guaranteed; interactions should occur on the UI thread.</remarks>
public partial class LiveChartViewModel : RxObject
{
    /// <summary>The upper boundary used for manual Y-axis scaling.</summary>
    private const double ManualYAxisMaximum = 100;

    /// <summary>The default number of right-side axes.</summary>
    private const int DefaultRightAxisCount = 3;

    /// <summary>The axis index used for temperature data.</summary>
    private const int TemperatureAxisIndex = 2;

    /// <summary>The primary axis label color.</summary>
    private const string PrimaryAxisColor = "#377eb8";

    /// <summary>Logs failures while publishing chart mouse coordinates.</summary>
    private static readonly Action<ILogger, Exception?> MouseLocationErrorLog = LoggerMessage.Define(
        LogLevel.Error,
        new(1, nameof(MouseLocationErrorLog)),
        "Mouse location error.");

    /// <summary>Stores the MAUI plot value.</summary>
    private MauiPlot? _mauiPlot1;

    /// <summary>Stores the selected setting value.</summary>
    [Reactive]
    private ChartObjects? _selectedSetting;

    /// <summary>Stores the right property visibility value.</summary>
    [Reactive]
    private bool _rightPropertyVisibility;

    /// <summary>Stores the cross hair enabled value.</summary>
    [Reactive]
    private bool _crossHairEnabled;

    /// <summary>Stores the title value.</summary>
    [Reactive]
    private string _title = " ";

    /// <summary>Stores the legend position value.</summary>
    [Reactive]
    private LegendPosition _legendPosition;

    /// <summary>Initializes a new instance of the <see cref="LiveChartViewModel"/> class.</summary>
    /// <remarks>This constructor configures chart axes, UI collections, and command bindings required for
    /// interactive chart functionality. The chart view is added to the provided grid and initialized with default
    /// settings. If the grid is not null, its child elements are cleared before adding the chart view.</remarks>
    /// <param name="grid">The optional grid that hosts the chart view.</param>
    public LiveChartViewModel(Grid grid)
    {
        InitializeChart(grid);
        AxesSetup(); // axes colors setup
        ApplyTheme(CurrentTheme);
        Initializations2();
        LinePropCommand = ReactiveCommand.Create(CollapseRightPropertyPanel);
        _ = LinePropCommand.DisposeWith(Disposables);
        ObservePlotSettings();
    }

    /// <summary>Gets or sets the collection of Y-axis objects used for plotting data on the chart.</summary>
    /// <remarks>The collection supports up to four Y-axes, allowing advanced charting scenarios such as
    /// multiple scales or overlaying different data series. Modifying this property updates the axes displayed on the
    /// chart. The order of axes in the collection determines their placement and association with data
    /// series.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<IYAxis> Axes { get; } = [];
#else
    public ReactiveList<IAxis> Axes { get; } = [];
#endif

    /// <summary>Gets the collection of axes used for plotting data in the chart.</summary>
    /// <remarks>Each axis in the collection defines a coordinate system for the chart. Modifying this
    /// collection allows customization of axis types, scales, and appearance. Changes to the collection may affect how
    /// data is rendered and interpreted.</remarks>
#if NET8_0_OR_GREATER
    public QuaternaryList<IPlottableUI> PlotLinesCollectionUI { get; private set; } = [];
#else
    public ReactiveList<IPlottableUI> PlotLinesCollectionUI { get; private set; } = [];
#endif

    /// <summary>Gets the collection of plot line UI elements displayed in the chart.</summary>
    /// <remarks>The collection is reactive, allowing dynamic updates to the chart's plot lines in response to
    /// changes. Modifying this collection will automatically update the chart's visual representation. Thread safety is
    /// not guaranteed; access from multiple threads should be synchronized.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<ChartObjects> ControlMenu { get; private set; } = [];
#else
    public ReactiveList<ChartObjects> ControlMenu { get; private set; } = [];
#endif

    /// <summary>Gets the chart objects exposed in the menu.</summary>
#if NET6_0_OR_GREATER
    public QuaternaryList<AxisLinesUI> AxisLinesUI { get; private set; } = [];
#else
    public ReactiveList<AxisLinesUI> AxisLinesUI { get; private set; } = [];
#endif

    /// <summary>Gets the collection of axis line UI elements associated with the control.</summary>
    /// <remarks>The returned list is reactive, allowing dynamic updates to the axis lines in response to
    /// changes in the underlying data or user interactions. Modifications to this collection will be reflected in the
    /// UI automatically.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<Crosshair_UI> CrosshairCollection { get; private set; } = [];
#else
    public ReactiveList<Crosshair_UI> CrosshairCollection { get; private set; } = [];
#endif

    /// <summary>Gets the collection of crosshair UI elements currently managed by the control.</summary>
    /// <remarks>The collection is reactive, allowing observers to track changes such as additions or removals
    /// of crosshair elements in real time. This property is read-only; to modify the collection, use the methods
    /// provided by the underlying ReactiveList.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<(Marker marker, Text text)> LabelCollection { get; private set; } = [];
#else
    public ReactiveList<(Marker marker, Text text)> LabelCollection { get; private set; } = [];
#endif

    /// <summary>Gets or sets the collection of marker and text label pairs displayed on the plot.</summary>
    /// <remarks>The collection is reactive and updates automatically when items are added or removed. Each
    /// tuple contains a marker and its associated text label, allowing for dynamic annotation of the plot.</remarks>
    public MauiPlot? MauiPlot1vm
    {
        get => _mauiPlot1;
        set => this.RaiseAndSetIfChanged(ref _mauiPlot1, value);
    }

    /// <summary>Gets the complete theme currently applied to the plot surface.</summary>
    public ReactivePlotTheme CurrentTheme { get; private set; } = ReactivePlotTheme.Dark;

    /// <summary>Gets editable settings for the attached reactive data series.</summary>
#if NET6_0_OR_GREATER
    public QuaternaryList<ReactivePlotSeriesSettings> SeriesSettings { get; } = [];
#else
    public ReactiveList<ReactivePlotSeriesSettings> SeriesSettings { get; } = [];
#endif

    /// <summary>Gets or sets the observable stream for mouse coordinate updates.</summary>
    /// <remarks>Subscribers to this observable receive notifications whenever the mouse coordinates change.
    /// The stream emits values of type <see cref="Coordinates"/> representing the current mouse position.</remarks>
    public Signal<Coordinates> MouseCoordinatesObservable { get; set; } = new();

    /// <summary>Gets or sets the visibility state of the left panel.</summary>
    public bool LeftPanelVisibility { get; set; }

    /// <summary>Gets the command that executes the line property action.</summary>
    /// <remarks>This command can be used to trigger logic related to line property changes, typically in
    /// response to user interaction. The command does not require input parameters and does not produce a result value.
    /// It is intended for use in reactive UI scenarios where command binding is supported.</remarks>
    public ReactiveCommand<Unit, Unit> LinePropCommand { get; }

    /// <summary>Gets or sets the collection of Y-axis objects used for plotting data.</summary>
    /// <remarks>Use this property to configure multiple Y-axes for advanced charting scenarios. The
    /// collection supports up to four Y-axis instances, allowing for complex visualizations with multiple scales or
    /// units.</remarks>
    public List<IYAxis> YAxisList { get; } = [];

    /// <summary>Gets or sets the collection of Y-axis definitions used for plotting data on the chart.</summary>
    /// <remarks>Each item in the collection represents a separate Y-axis. Modifying the collection affects
    /// how data series are mapped to Y-axes in the chart. The order of axes in the list determines their display
    /// sequence.</remarks>
    public IXAxis XAxis1 { get; set; } = null!;

    /// <summary>Gets or sets the logger used for chart diagnostics.</summary>
    public ILogger Logger { get; set; } = NullLogger.Instance;

    /// <summary>
    /// Creates an action that automatically adjusts the horizontal axis scaling of a plot using the specified X-axis
    /// configuration.
    /// </summary>
    /// <param name="xaxis">The X-axis configuration to use for auto-scaling. Cannot be null.</param>
    /// <returns>An action that automatically scales the plot X-axis.
    /// based on the provided configuration.</returns>
    public static Action<RenderPack> AutoScaleX(IXAxis xaxis) => rp => rp.Plot.Axes.AutoScaleX(xaxis);

    /// <summary>Creates an action that automatically scales the plot Y-axis.</summary>
    /// <remarks>This method is useful for ensuring that all data points are visible along the Y-axis after
    /// data changes or updates. The X-axis range is not affected by this action.</remarks>
    /// <returns>An action that rescales the plot Y-axis in the render pack.
    /// provided <see cref="RenderPack"/> to encompass all visible data.</returns>
    public static Action<RenderPack> AutoScaleY() => static rp => rp.Plot.Axes.AutoScaleY();

    /// <summary>Creates an action that automatically adjusts all axes in a plot to fit the current data.</summary>
    /// <remarks>Use this action to ensure that all plot axes are scaled to include the full range of data.
    /// This is useful after adding, removing, or modifying data in the plot to guarantee that all content is
    /// visible.</remarks>
    /// <returns>An action that rescales all plot axes in the render pack.
    /// <see cref="RenderPack"/> to display all data points.</returns>
    public static Action<RenderPack> AutoScaleAll()
    {
        List<(IAxis Axis, double Min, double Max)> previousRanges = [];
        return rp =>
        {
            previousRanges.Clear();
            foreach (var axis in rp.Plot.Axes.GetAxes())
            {
                previousRanges.Add((axis, axis.Min, axis.Max));
            }

            rp.Plot.Axes.AutoScale();
            foreach (var (axis, min, max) in previousRanges)
            {
                if (!HasFiniteRange(axis.Min, axis.Max))
                {
                    SetFiniteRange(axis, min, max);
                }
            }
        };
    }

    /// <summary>Hides all Y-axis elements by setting their visibility to false.</summary>
    /// <remarks>Call this method to remove all Y-axis visuals from the chart or plot. This affects only the
    /// visibility of the Y-axis elements; it does not remove or alter the underlying axis data.</remarks>
    public void HideAllYAxis()
    {
        foreach (var axis in YAxisList)
        {
            axis.IsVisible = false;
        }
    }

    /// <summary>Configures the plot to use a bottom axis displaying date and time ticks with finite initial limits.</summary>
    /// <remarks>This method removes any existing bottom axis from the plot and replaces it with a new axis
    /// configured for date and time representation. Existing finite date limits are retained while an empty plot
    /// uses the current day. Call this method when the plot displays time-based data. The axis color is also
    /// set according to the application's configuration.</remarks>
    public void CreateAxisWithTimeStamp()
    {
        var previousAxis = MauiPlot1vm!.Plot.Axes.Bottom;
        var today = DateTime.Today.ToOADate();
        var min = previousAxis is ScottPlot.AxisPanels.DateTimeXAxis ? previousAxis.Min : today;
        var max = previousAxis is ScottPlot.AxisPanels.DateTimeXAxis ? previousAxis.Max : today + 1;
        XAxis1 = new ScottPlot.AxisPanels.DateTimeXAxis();
        SetFiniteRange(XAxis1, min, max, today, today + 1);
        MauiPlot1vm.Plot.Axes.Remove(Edge.Bottom);
        MauiPlot1vm.Plot.Axes.AddBottomAxis(XAxis1);
        MauiPlot1vm.Plot.Axes.DefaultGrid.XAxis = XAxis1;
        foreach (var plottable in MauiPlot1vm.Plot.GetPlottables())
        {
            if (ReferenceEquals(plottable.Axes.XAxis, previousAxis))
            {
                plottable.Axes.XAxis = XAxis1;
            }
        }

        SetXAxisColour();
    }

    /// <summary>Configures frameless plot axes without labels or tick marks.</summary>
    /// <remarks>Call this method to create a minimal plot appearance by disabling axis tick labels, hiding
    /// major and minor tick marks, and removing the grid's major lines and plot frame. This is useful for scenarios
    /// where a clean, frameless visualization is desired.</remarks>
    public void AxisStyle()
    {
        MauiPlot1vm!.Plot!.Axes.Bottom.TickLabelStyle.IsVisible = false;
        MauiPlot1vm!.Plot!.Axes.Bottom.MajorTickStyle.Length = 0;
        MauiPlot1vm!.Plot!.Axes.Bottom.MinorTickStyle.Length = 0;

        XAxis1.TickLabelStyle.IsVisible = false;
        XAxis1.MajorTickStyle.Length = 0;
        XAxis1.MinorTickStyle.Length = 0;

        MauiPlot1vm!.Plot!.Grid.YAxisStyle.MajorLineStyle.Width = 0;
        MauiPlot1vm!.Plot!.Layout.Frameless();
    }

    /// <summary>Replaces the bottom axis with the current settings.</summary>
    /// <remarks>Call this method to reset the bottom axis of the plot, typically when axis configuration or
    /// appearance needs to be refreshed. The method also updates the axis color to match the current settings. If the
    /// plot view model is not initialized, the method does nothing.</remarks>
    public void CreateAxisWithPoints()
    {
        var previousAxis = MauiPlot1vm!.Plot.Axes.Bottom;
        var min = previousAxis.Min;
        var max = previousAxis.Max;
        MauiPlot1vm.Plot.Axes.Remove(previousAxis);
        XAxis1 = MauiPlot1vm.Plot.Axes.AddBottomAxis();
        SetFiniteRange(XAxis1, min, max);
        MauiPlot1vm.Plot.Axes.DefaultGrid.XAxis = XAxis1;
        foreach (var plottable in MauiPlot1vm.Plot.GetPlottables())
        {
            if (ReferenceEquals(plottable.Axes.XAxis, previousAxis))
            {
                plottable.Axes.XAxis = XAxis1;
            }
        }

        SetXAxisColour();
    }

    /// <summary>Sets the X-axis label, tick, and frame colors.</summary>
    /// <remarks>This method updates the color scheme of the X-axis elements, including label text, tick
    /// marks, and frame lines. Call this method to apply a consistent color style to the X-axis in the chart. This
    /// operation does not affect other axes or chart elements.</remarks>
    public void SetXAxisColour()
    {
        var baseColor = Color.FromHex(CurrentTheme.Axis);

        // colors
        XAxis1.Label.ForeColor = Color.FromHex(PrimaryAxisColor);
        XAxis1.FrameLineStyle.Color = baseColor;
        XAxis1.TickLabelStyle.ForeColor = baseColor;
        XAxis1.MajorTickStyle.Color = baseColor;
        XAxis1.MinorTickStyle.Color = baseColor;
    }

    /// <summary>Removes all label markers and associated text from the plot and clears the label collection.</summary>
    /// <remarks>This method updates the plot by removing all labels currently stored in the label collection
    /// and then refreshes the display. If there are no labels to clear, the method has no effect.</remarks>
    public void ClearLabels()
    {
        if (LabelCollection is null)
        {
            return;
        }

        if (LabelCollection.Count == 0)
        {
            return;
        }

        foreach (var (marker, text) in LabelCollection)
        {
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(marker!);
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(text!);
        }

        LabelCollection.Clear();

        MauiPlot1vm!.Refresh();
    }

    /// <summary>Removes all crosshair lines and markers from the plot.</summary>
    /// <remarks>This method disposes of each crosshair item and removes its associated visual elements from
    /// the plot. After clearing, the plot is refreshed to reflect the changes. Calling this method when no crosshair
    /// items are present has no effect.</remarks>
    public void ClearAxisCrosshairs()
    {
        if (CrosshairCollection is null)
        {
            return;
        }

        if (CrosshairCollection.Count == 0)
        {
            return;
        }

        foreach (var item in CrosshairCollection)
        {
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(item.PlotLine!);
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.Crosshair!);
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.Marker!);
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.MarkerText!);
            item.Dispose();
        }

        CrosshairCollection.Clear();

        MauiPlot1vm!.Refresh();
    }

    /// <summary>Removes all axis lines from the plot and disposes of their associated resources.</summary>
    /// <remarks>This method clears the collection of axis line UI elements and removes their corresponding
    /// plottables from the plot. If there are no axis lines present, the method performs no action.</remarks>
    public void ClearAxisLines()
    {
        if (AxisLinesUI is null)
        {
            return;
        }

        if (AxisLinesUI.Count == 0)
        {
            return;
        }

        foreach (var item in AxisLinesUI)
        {
            _ = MauiPlot1vm!.Plot.PlottableList.Remove(item.AxisLine!);
            item.Dispose();
        }

        AxisLinesUI.Clear();
    }

    /// <summary>Removes plot data, controls, and labels and releases their resources.</summary>
    /// <remarks>Call this method to reset the plot and related UI elements to an empty state. Any disposable
    /// resources associated with plot lines and controls are released. After calling this method, the plot and UI
    /// collections will be empty, and all labels will be cleared.</remarks>
    public void ClearContent()
    {
        MauiPlot1vm?.Plot.Clear();

        // SIGNAL
        if (PlotLinesCollectionUI.Count > 0)
        {
            foreach (var element in PlotLinesCollectionUI)
            {
                element.Dispose();
            }

            PlotLinesCollectionUI.Clear();
        }

        if (ControlMenu?.Count > 0)
        {
            foreach (var element in ControlMenu)
            {
                element.IsCheckedCmd?.Dispose();
                element.Dispose();
            }

            ControlMenu.Clear();
        }

        // LABELS
        ClearLabels();
    }

    /// <summary>Replaces the plot Y-axes using the supplied names and colors.</summary>
    /// <param name="data">The data value.</param>
    public void YAxesSetup((IList<string> yNames, IList<string> hexColors) data)
    {
        var baseColor = Color.FromHex(CurrentTheme.Axis);

        // hide left axis always, i just want right ones
        HideLeftAxis(baseColor);

        // remove axes from graph and from the list
        foreach (var axis in YAxisList)
        {
            MauiPlot1vm?.Plot.Axes.Remove(axis);
        }

        YAxisList.Clear();

        var axisCount = Math.Min(data.yNames.Count, data.hexColors.Count);
        for (var index = 0; index < axisCount; index++)
        {
            var name = data.yNames[index];
            var color = data.hexColors[index];
            var rightAxis = MauiPlot1vm!.Plot.Axes.AddRightAxis();
            rightAxis.FrameLineStyle.Color = baseColor;
            rightAxis.TickLabelStyle.ForeColor = baseColor;
            rightAxis.MajorTickStyle.Color = baseColor;
            rightAxis.MinorTickStyle.Color = baseColor;
            rightAxis.LabelText = name;
            rightAxis.LabelFontColor = Color.FromHex(color);
            YAxisList.Add(rightAxis);
        }
    }

    /// <summary>Sets the Y-axis limits of all axes in the plot to the range 0 to 100.</summary>
    /// <remarks>This method applies the specified Y-axis range to each axis in the collection and refreshes
    /// the plot to reflect the changes. Use this method to manually reset the Y-axis scaling when automatic scaling is
    /// not desired.</remarks>
    public void ManualScaleY()
    {
        foreach (var axis in YAxisList)
        {
            MauiPlot1vm?.Plot.Axes.SetLimitsY(0, ManualYAxisMaximum, axis);
        }

        MauiPlot1vm?.Refresh();
    }

    /// <summary>Applies a complete plot theme and refreshes the rendered surface.</summary>
    /// <param name="theme">The theme to apply.</param>
    public void ApplyTheme(ReactivePlotTheme theme)
    {
        ThrowHelper.ThrowIfNull(theme, nameof(theme));

        CurrentTheme = theme;
        var figureBackground = Color.FromHex(theme.FigureBackground);
        var dataBackground = Color.FromHex(theme.DataBackground);
        var axisColor = Color.FromHex(theme.Axis);
        MauiPlot1vm!.Plot.Add.Palette = new Penumbra();
        MauiPlot1vm.Plot.Axes.Color(axisColor);
        MauiPlot1vm.Plot.Grid.MajorLineColor = Color.FromHex(theme.Grid);
        MauiPlot1vm.Plot.FigureBackground.Color = figureBackground;
        MauiPlot1vm.Plot.DataBackground.Color = dataBackground;
        MauiPlot1vm.Plot.Legend.BackgroundColor = Color.FromHex(theme.LegendBackground);
        MauiPlot1vm.Plot.Legend.FontColor = Color.FromHex(theme.LegendForeground);
        MauiPlot1vm.Plot.Legend.OutlineColor = Color.FromHex(theme.LegendForeground);
        MauiPlot1vm.Refresh();
    }

    /// <summary>Adds a crosshair bound to the primary axes.</summary>
    public void AddCrosshair()
    {
        CrosshairCollection.Add(
            new(
                MauiPlot1vm!,
                ("crosshair 1", 0),
                color: "Blue",
                isXAxisDateTime: IsXAxisDateTime,
                coordinatesObs: MouseCoordinatesObservable));
        var crosshair = CrosshairCollection[CrosshairCollection.Count - 1].PlotLine!;
        crosshair.Axes.YAxis = YAxisList[0];
        crosshair.Axes.XAxis = XAxis1;
        crosshair.VerticalLine.Axes.YAxis = YAxisList[0];
        crosshair.VerticalLine.Axes.XAxis = XAxis1;
        crosshair.HorizontalLine.Axes.YAxis = YAxisList[0];
        crosshair.HorizontalLine.Axes.XAxis = XAxis1;
    }

    /// <summary>Determines whether an axis has finite, nonzero geometry.</summary>
    /// <param name="min">The first limit.</param>
    /// <param name="max">The second limit.</param>
    /// <returns>Whether the limits define a finite range.</returns>
    private static bool HasFiniteRange(double min, double max) => double.IsFinite(min) && double.IsFinite(max) && Math.Abs(max - min) > 0 && double.IsFinite(max - min);

    /// <summary>Initializes an axis from usable limits or an empty-source fallback.</summary>
    /// <param name="axis">The axis.</param>
    /// <param name="min">The requested first limit.</param>
    /// <param name="max">The requested second limit.</param>
    /// <param name="fallbackMin">The empty-source first limit.</param>
    /// <param name="fallbackMax">The empty-source second limit.</param>
    private static void SetFiniteRange(IAxis axis, double min, double max, double fallbackMin = 0, double fallbackMax = 1)
    {
        axis.Min = HasFiniteRange(min, max) ? min : fallbackMin;
        axis.Max = HasFiniteRange(min, max) ? max : fallbackMax;
    }

    /// <summary>Selects a predefined color from the legend item count.</summary>
    /// <remarks>The returned color is chosen by computing the remainder of the legend's item count divided by
    /// the number of available colors. If the legend is empty, the first color in the set is returned.</remarks>
    /// <typeparam name="T">The type of elements contained in the legend collection.</typeparam>
    /// <param name="legend">The collection whose item count determines the selected color. Cannot be null.</param>
    /// <returns>A string representing the selected color name from the predefined set.</returns>
    private static string SetColorLegend<T>(IEnumerable<T> legend)
    {
        List<string> colors =
        [
            "Green",
            "Blue",
            "Yellow",
            "Magenta",
            "Cyan",
            "Orange",
            "Purple",
            "White",
            "Gold",
            "Crimson",
            "DeepPink",
            "DodgerBlue",
            "GreenYellow",
            "BlueViolet",
            "Gray",
            "Red",
        ];

        var legendItemCount = 0;
        foreach (var _ in legend)
        {
            legendItemCount++;
        }

        var n = legendItemCount % colors.Count;
        return colors[n];
    }

    /// <summary>Creates the plot and initializes its UI collections.</summary>
    /// <param name="grid">The optional plot host.</param>
    private void InitializeChart(Grid grid)
    {
        if (MauiPlot1vm is null)
        {
            MauiPlot1vm = new();
            ApplyTheme(ReactivePlotTheme.Dark);
            grid?.Children.Clear();
            grid?.Children.Add(MauiPlot1vm);
        }

        PlotLinesCollectionUI = [];
        ControlMenu = [];
        AxisLinesUI = [];
        CrosshairCollection = [];
        LabelCollection = [];
        Axes.Clear();
        LeftPanelVisibility = true;
        MouseCoordinatesObservable = new();
        YAxisList.Clear();
        XAxis1 = MauiPlot1vm!.Plot.Axes.AddBottomAxis();
        CreateAxisWithTimeStamp();
    }

    /// <summary>Propagates point-window settings to active plot lines.</summary>
    private void ObservePlotSettings()
    {
        _ = this.WhenAnyValue(x => x.NumberPointsPlotted)
            .Subscribe(numberOfPoints =>
            {
                foreach (var plotLine in PlotLinesCollectionUI)
                {
                    plotLine.NumberPointsPlotted = numberOfPoints;
                }
            })
            .DisposeWith(Disposables);
        _ = this.WhenAnyValue(x => x.UseFixedNumberOfPoints)
            .Subscribe(useFixedPointCount =>
            {
                foreach (var plotLine in PlotLinesCollectionUI)
                {
                    plotLine.UseFixedNumberOfPoints = useFixedPointCount;
                }
            })
            .DisposeWith(Disposables);
    }

    /// <summary>Configures plot-axis colors, labels, and units.</summary>
    /// <remarks>This method sets up the left axis, customizes the X axis color scheme, and adds three right Y
    /// axes with distinct labels and colors for phase, displacement, and temperature. It should be called during plot
    /// initialization to ensure axes are correctly styled and labeled before displaying data.</remarks>
    private void AxesSetup()
    {
        var baseColor = Color.FromHex(CurrentTheme.Axis);

        HideLeftAxis(baseColor);

        // colors
        XAxis1.Label.ForeColor = Color.FromHex(PrimaryAxisColor);
        XAxis1.FrameLineStyle.Color = baseColor;
        XAxis1.TickLabelStyle.ForeColor = baseColor;
        XAxis1.MajorTickStyle.Color = baseColor;
        XAxis1.MinorTickStyle.Color = baseColor;

        // Setup Y Axis
        for (var i = 0; i < DefaultRightAxisCount; i++)
        {
            var rightAxis = MauiPlot1vm!.Plot.Axes.AddRightAxis();
            rightAxis.FrameLineStyle.Color = baseColor;
            rightAxis.TickLabelStyle.ForeColor = baseColor;
            rightAxis.MajorTickStyle.Color = baseColor;
            rightAxis.MinorTickStyle.Color = baseColor;
            YAxisList.Add(rightAxis);
        }

        // Configure Axis Unit and Color
        YAxisList[0].Label.Text = "Phase [ ° ]";
        YAxisList[0].Label.ForeColor = Color.FromHex(PrimaryAxisColor);

        YAxisList[1].Label.Text = "[um]";
        YAxisList[1].Label.ForeColor = Color.FromHex("#4daf4a");

        YAxisList[TemperatureAxisIndex].Label.Text = "[°C]";
        YAxisList[TemperatureAxisIndex].Label.ForeColor = Color.FromHex("#984ea3");

        AxisStyle();
    }

    /// <summary>Collapses the right property panel.</summary>
    private void CollapseRightPropertyPanel() => RightPropertyVisibility = false;

    /// <summary>Logs a failure while publishing the current mouse location.</summary>
    /// <param name="exception">The publishing failure.</param>
    private void LogMouseLocationError(Exception exception) => MouseLocationErrorLog(Logger, exception);

    /// <summary>
    /// Hides the left axis of the plot by setting its foreground and tick colors to a background color, and updates the
    /// axis frame color.
    /// </summary>
    /// <remarks>This method visually conceals the left axis labels and ticks by matching their colors to the
    /// plot background, while allowing the frame line to remain visible in the specified color.</remarks>
    /// <param name="color">The color to apply to the left axis frame line.</param>
    private void HideLeftAxis(in Color color)
    {
        var l = MauiPlot1vm?.Plot.Axes.Left;
        var backColour = Color.FromHex(CurrentTheme.DataBackground);
        l!.Label.ForeColor = backColour;
        l.FrameLineStyle.Color = color;
        l.TickLabelStyle.ForeColor = backColour;
        l.MajorTickStyle.Color = backColour;
        l.MinorTickStyle.Color = backColour;
    }
}
