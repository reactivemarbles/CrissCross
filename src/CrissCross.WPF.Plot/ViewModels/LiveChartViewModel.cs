// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CP.Primitives.Collections;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Palettes;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents the view model for a live chart, providing properties and commands to manage chart data, axes, labels,
/// and user interactions in a WPF application.
/// </summary>
/// <remarks>This class is intended for use with Windows 10 version 19041 or later. It exposes collections for
/// chart elements, axis management, and commands for interactive features such as crosshairs, labels, and menu
/// expansion. The view model supports dynamic updates to chart visuals and user-driven configuration. Thread safety is
/// not guaranteed; interactions should occur on the UI thread.</remarks>
[SupportedOSPlatform("windows")]
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

    /// <summary>Stores the wpf plot1 value.</summary>
    private WpfPlot? _wpfPlot1;

    /// <summary>Stores the selected setting value.</summary>
    [Reactive]
    private ChartObjects? _selectedSetting;

    /// <summary>Stores the right property visibility value.</summary>
    [Reactive]
    private Visibility _rightPropertyVisibility = Visibility.Collapsed;

    /// <summary>Stores the cross hair enabled value.</summary>
    [Reactive]
    private bool _crossHairEnabled;

    /// <summary>Stores the is menu expanded value.</summary>
    [Reactive]
    private bool _isMenuExpanded;

    /// <summary>Stores the title value.</summary>
    [Reactive]
    private string _title = " ";

    /// <summary>Stores the legend position value.</summary>
    [Reactive]
    private LegendPosition _legendPosition = LegendPosition.Top;

    /// <summary>Initializes a new instance of the <see cref="LiveChartViewModel"/> class.</summary>
    /// <remarks>This constructor configures chart axes, UI collections, and command bindings required for
    /// interactive chart functionality. The chart view is added to the provided grid and initialized with default
    /// settings. If the grid is not null, its child elements are cleared before adding the chart view.</remarks>
    /// <param name="grid">The optional grid that hosts the chart view.</param>
    public LiveChartViewModel(Grid grid)
    {
        InitializeChart(grid);
        InitializeCommands();
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
    public QuaternaryList<IYAxis> Axes { get; set; } = [];
#else
    public ReactiveList<IAxis> Axes { get; set; } = [];
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
    public WpfPlot? WpfPlot1vm
    {
        get => _wpfPlot1;
        set => this.RaiseAndSetIfChanged(ref _wpfPlot1, value);
    }

    /// <summary>Gets the complete theme currently applied to the plot surface.</summary>
    public ReactivePlotTheme CurrentTheme { get; private set; } = ReactivePlotTheme.Dark;

    /// <summary>Gets or sets the observable stream for mouse coordinate updates.</summary>
    /// <remarks>Subscribers to this observable receive notifications whenever the mouse coordinates change.
    /// The stream emits values of type <see cref="Coordinates"/> representing the current mouse position.</remarks>
    public Signal<Coordinates> MouseCoordinatesObservable { get; set; } = new();

    /// <summary>Gets the command that is executed when the graph is locked, preventing further modifications.</summary>
    /// <remarks>Use this command to signal that the graph should enter a locked state. While the graph is
    /// locked, operations that modify its structure or data may be disabled or ignored. The command completes when the
    /// lock is applied.</remarks>
    public ReactiveCommand<Unit, Unit>? GraphLocked { get; private set; }

    /// <summary>Gets the command that enables the marker button in the user interface.</summary>
    /// <remarks>The command can be bound to UI elements to control the enabled state of the marker button.
    /// The command is reactive and may be null if the marker button is not available in the current context.</remarks>
    public ReactiveCommand<Unit, Unit>? EnableMarkerBtn { get; private set; }

    /// <summary>Gets the command that adds a crosshair to the current context when executed.</summary>
    /// <remarks>The command is typically bound to a user interface element, such as a button, to enable users
    /// to add a crosshair interactively. The command is disabled if adding a crosshair is not currently
    /// permitted.</remarks>
    public ReactiveCommand<Unit, Unit>? AddCrosshairBtn { get; private set; }

    /// <summary>Gets the command that removes all labels from the current selection.</summary>
    /// <remarks>The command is enabled only when labels can be removed from the selection. Use this property
    /// to bind UI elements, such as a button, to the label removal functionality.</remarks>
    public ReactiveCommand<Unit, Unit>? RemoveLabelsBtn { get; private set; }

    /// <summary>Gets the command that expands the menu when executed.</summary>
    /// <remarks>The command can be bound to UI elements to trigger menu expansion. The property may be null
    /// if the command is not available in the current context.</remarks>
    public ReactiveCommand<Unit, Unit>? ExpandMenuBtn { get; private set; }

    /// <summary>Gets or sets the visibility state of the left panel.</summary>
    public Visibility LeftPanelVisibility { get; set; }

    /// <summary>Gets the command that executes the line property action.</summary>
    /// <remarks>This command can be used to trigger logic related to line property changes, typically in
    /// response to user interaction. The command does not require input parameters and does not produce a result value.
    /// It is intended for use in reactive UI scenarios where command binding is supported.</remarks>
    public ReactiveCommand<Unit, Unit> LinePropCommand { get; }

    /// <summary>Gets or sets the collection of Y-axis objects used for plotting data.</summary>
    /// <remarks>Use this property to configure multiple Y-axes for advanced charting scenarios. The
    /// collection supports up to four Y-axis instances, allowing for complex visualizations with multiple scales or
    /// units.</remarks>
    public List<IYAxis> YAxisList { get; set; } = [];

    /// <summary>Gets or sets the collection of Y-axis definitions used for plotting data on the chart.</summary>
    /// <remarks>Each item in the collection represents a separate Y-axis. Modifying the collection affects
    /// how data series are mapped to Y-axes in the chart. The order of axes in the list determines their display
    /// sequence.</remarks>
    public IXAxis XAxis1 { get; set; } = null!;

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
    public static Action<RenderPack> AutoScaleY() => rp => rp.Plot.Axes.AutoScaleY();

    /// <summary>Creates an action that automatically adjusts all axes in a plot to fit the current data.</summary>
    /// <remarks>Use this action to ensure that all plot axes are scaled to include the full range of data.
    /// This is useful after adding, removing, or modifying data in the plot to guarantee that all content is
    /// visible.</remarks>
    /// <returns>An action that rescales all plot axes in the render pack.
    /// <see cref="RenderPack"/> to display all data points.</returns>
    public static Action<RenderPack> AutoScaleAll() => rp => rp.Plot.Axes.AutoScale();

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

    /// <summary>
    /// Configures the plot to use a bottom axis displaying date and time ticks, and sets the axis range to cover all
    /// possible date values.
    /// </summary>
    /// <remarks>This method removes any existing bottom axis from the plot and replaces it with a new axis
    /// configured for date and time representation. The axis range is set from the minimum to the maximum supported
    /// date values. Call this method when you need the plot's X-axis to display time-based data. The axis color is also
    /// set according to the application's configuration.</remarks>
    public void CreateAxisWithTimeStamp()
    {
        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Bottom);
        XAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        XAxis1 = WpfPlot1vm!.Plot.Axes.DateTimeTicksBottom();
        XAxis1.Max = DateTime.MaxValue.ToOADate();
        XAxis1.Min = DateTime.MinValue.ToOADate();
        SetXAxisColour();
    }

    /// <summary>Configures frameless plot axes without labels or tick marks.</summary>
    /// <remarks>Call this method to create a minimal plot appearance by disabling axis tick labels, hiding
    /// major and minor tick marks, and removing the grid's major lines and plot frame. This is useful for scenarios
    /// where a clean, frameless visualization is desired.</remarks>
    public void AxisStyle()
    {
        WpfPlot1vm!.Plot!.Axes.Bottom.TickLabelStyle.IsVisible = false;
        WpfPlot1vm!.Plot!.Axes.Bottom.MajorTickStyle.Length = 0;
        WpfPlot1vm!.Plot!.Axes.Bottom.MinorTickStyle.Length = 0;

        XAxis1.TickLabelStyle.IsVisible = false;
        XAxis1.MajorTickStyle.Length = 0;
        XAxis1.MinorTickStyle.Length = 0;

        WpfPlot1vm!.Plot!.Grid.YAxisStyle.MajorLineStyle.Width = 0;
        WpfPlot1vm!.Plot!.Layout.Frameless();
    }

    /// <summary>Replaces the bottom axis with the current settings.</summary>
    /// <remarks>Call this method to reset the bottom axis of the plot, typically when axis configuration or
    /// appearance needs to be refreshed. The method also updates the axis color to match the current settings. If the
    /// plot view model is not initialized, the method does nothing.</remarks>
    public void CreateAxisWithPoints()
    {
        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Bottom);
        XAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
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
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(marker!);
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(text!);
        }

        LabelCollection.Clear();

        WpfPlot1vm!.Refresh();
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
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(item.PlotLine!);
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.Crosshair!);
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.Marker!);
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.MarkerText!);
            item.Dispose();
        }

        CrosshairCollection.Clear();

        WpfPlot1vm!.Refresh();
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
            _ = WpfPlot1vm!.Plot.PlottableList.Remove(item.AxisLine!);
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
        WpfPlot1vm?.Plot.Clear();

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
            WpfPlot1vm?.Plot.Axes.Remove(axis);
        }

        YAxisList.Clear();

        // combine yName and hexColors lists
        var combinedList = data.yNames.Zip(data.hexColors, (name, color) => (name, color));

        // create the axes
        foreach (var (name, color) in combinedList)
        {
            var rightAxis = WpfPlot1vm!.Plot.Axes.AddRightAxis();
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
            WpfPlot1vm?.Plot.Axes.SetLimitsY(0, ManualYAxisMaximum, axis);
        }

        WpfPlot1vm?.Refresh();
    }

    /// <summary>Applies a complete plot theme and refreshes the rendered surface.</summary>
    /// <param name="theme">The theme to apply.</param>
    public void ApplyTheme(ReactivePlotTheme theme)
    {
        if (theme is null)
        {
            throw new ArgumentNullException(nameof(theme));
        }

        CurrentTheme = theme;
        var figureBackground = Color.FromHex(theme.FigureBackground);
        var dataBackground = Color.FromHex(theme.DataBackground);
        var axisColor = Color.FromHex(theme.Axis);
        WpfPlot1vm!.Plot.Add.Palette = new Penumbra();
        WpfPlot1vm.Plot.Axes.Color(axisColor);
        WpfPlot1vm.Plot.Grid.MajorLineColor = Color.FromHex(theme.Grid);
        WpfPlot1vm.Plot.FigureBackground.Color = figureBackground;
        WpfPlot1vm.Plot.DataBackground.Color = dataBackground;
        WpfPlot1vm.Plot.Legend.BackgroundColor = Color.FromHex(theme.LegendBackground);
        WpfPlot1vm.Plot.Legend.FontColor = Color.FromHex(theme.LegendForeground);
        WpfPlot1vm.Plot.Legend.OutlineColor = Color.FromHex(theme.LegendForeground);
        WpfPlot1vm.Refresh();
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
            "Red",];

        // add data
        var n = legend.Count() % colors.Count;
        return colors[n];
    }

    /// <summary>Creates the plot and initializes its UI collections.</summary>
    /// <param name="grid">The optional plot host.</param>
    private void InitializeChart(Grid grid)
    {
        if (WpfPlot1vm is null)
        {
            WpfPlot1vm = new() { Name = grid?.Name + "WpfPlot" };
            ApplyTheme(ReactivePlotTheme.Dark);
            grid?.Children.Clear();
            grid?.Children.Add(WpfPlot1vm);
        }

        PlotLinesCollectionUI = [];
        ControlMenu = [];
        AxisLinesUI = [];
        CrosshairCollection = [];
        LabelCollection = [];
        Axes = [];
        LeftPanelVisibility = Visibility.Visible;
        MouseCoordinatesObservable = new();
        YAxisList = [];
        XAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        CreateAxisWithTimeStamp();
    }

    /// <summary>Creates the interactive chart commands.</summary>
    private void InitializeCommands()
    {
        GraphLocked = ReactiveCommand.Create(() => { });
        EnableMarkerBtn = ReactiveCommand.Create(() =>
        {
            foreach (var plotLine in PlotLinesCollectionUI)
            {
                plotLine.ChartSettings.IsCrossHairVisible = !plotLine.ChartSettings.IsCrossHairVisible;
            }
        });
        AddCrosshairBtn = ReactiveCommand.Create(AddCrosshair);
        RemoveLabelsBtn = ReactiveCommand.Create(() =>
        {
            ClearLabels();
            ClearAxisCrosshairs();
        });
        ExpandMenuBtn = ReactiveCommand.Create(ToggleMenuExpansion);
        _ = ExpandMenuBtn.DisposeWith(Disposables);
    }

    /// <summary>Adds a crosshair bound to the primary axes.</summary>
    private void AddCrosshair()
    {
        CrosshairCollection.Add(
            new Crosshair_UI(
                WpfPlot1vm!,
                ("crosshair 1", 0),
                color: "Blue",
                isXAxisDateTime: IsXAxisDateTime,
                coordinatesObs: MouseCoordinatesObservable));
        var crosshair = CrosshairCollection.Last().PlotLine!;
        crosshair.Axes.YAxis = YAxisList[0];
        crosshair.Axes.XAxis = XAxis1;
        crosshair.VerticalLine.Axes.YAxis = YAxisList[0];
        crosshair.VerticalLine.Axes.XAxis = XAxis1;
        crosshair.HorizontalLine.Axes.YAxis = YAxisList[0];
        crosshair.HorizontalLine.Axes.XAxis = XAxis1;
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
            var rightAxis = WpfPlot1vm!.Plot.Axes.AddRightAxis();
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

    /// <summary>Toggles the menu expansion state.</summary>
    private void ToggleMenuExpansion() => IsMenuExpanded = !IsMenuExpanded;

    /// <summary>Collapses the right property panel.</summary>
    private void CollapseRightPropertyPanel() => RightPropertyVisibility = Visibility.Collapsed;

    /// <summary>
    /// Hides the left axis of the plot by setting its foreground and tick colors to a background color, and updates the
    /// axis frame color.
    /// </summary>
    /// <remarks>This method visually conceals the left axis labels and ticks by matching their colors to the
    /// plot background, while allowing the frame line to remain visible in the specified color.</remarks>
    /// <param name="color">The color to apply to the left axis frame line.</param>
    private void HideLeftAxis(in Color color)
    {
        var l = WpfPlot1vm?.Plot.Axes.Left;
        var backColour = Color.FromHex(CurrentTheme.DataBackground);
        l!.Label.ForeColor = backColour;
        l.FrameLineStyle.Color = color;
        l.TickLabelStyle.ForeColor = backColour;
        l.MajorTickStyle.Color = backColour;
        l.MinorTickStyle.Color = backColour;
    }
}
