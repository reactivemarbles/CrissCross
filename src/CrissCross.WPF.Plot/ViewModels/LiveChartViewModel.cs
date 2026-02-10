// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CP.Reactive.Collections;
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
    private WpfPlot? _wpfPlot1;

    [Reactive]
    private ChartObjects? _selectedSetting;

    [Reactive]
    private Visibility _rightPropertyVisibility = Visibility.Collapsed;

    [Reactive]
    private bool _crossHairEnabled;

    [Reactive]
    private bool _isMenuExpanded;

    [Reactive]
    private string _title = " ";

    [Reactive]
    private LegendPosition _legendPosition = LegendPosition.Top;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChartViewModel"/> class and sets up the chart view within the specified grid.
    /// container.
    /// </summary>
    /// <remarks>This constructor configures chart axes, UI collections, and command bindings required for
    /// interactive chart functionality. The chart view is added to the provided grid and initialized with default
    /// settings. If the grid is not null, its child elements are cleared before adding the chart view.</remarks>
    /// <param name="grid">The Grid control that will host the chart view. If null, the chart view will not be added to a visual container.</param>
    public LiveChartViewModel(Grid grid)
    {
        // INITIALIZATION
        if (WpfPlot1vm == null)
        {
            WpfPlot1vm = new WpfPlot()
            {
                Name = grid?.Name + "WpfPlot"
            };

            UseDarkMode();
            grid?.Children.Clear();
            grid?.Children.Add(WpfPlot1vm);
        }

        // INITIALIZATION
        PlotLinesCollectionUI = [];
        ControlMenu = [];
        AxisLinesUI = [];
        CrosshairCollection = [];
        LabelCollection = [];
        Axes = [];
        LeftPanelVisibility = Visibility.Visible;
        MouseCoordinatesObservable = new Subject<Coordinates>();

        YAxisList = [];
        XAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        CreateAxisWithTimeStamp();
        GraphLocked = ReactiveCommand.Create(() => { });
        EnableMarkerBtn = ReactiveCommand.Create(() =>
        {
            foreach (var p in PlotLinesCollectionUI)
            {
                p.ChartSettings.IsCrossHairVisible = !p.ChartSettings.IsCrossHairVisible;
            }
        });

        AddCrosshairBtn = ReactiveCommand.Create(() =>
        {
            CrosshairCollection.Add(new Crosshair_UI(WpfPlot1vm, ("crosshair 1", 0), color: "Blue", isXAxisDateTime: IsXAxisDateTime, coordinatesObs: MouseCoordinatesObservable));
            CrosshairCollection!.Last().PlotLine!.Axes.YAxis = YAxisList[0];
            CrosshairCollection!.Last().PlotLine!.Axes.XAxis = XAxis1;

            CrosshairCollection!.Last().PlotLine!.VerticalLine.Axes.YAxis = YAxisList[0];
            CrosshairCollection!.Last().PlotLine!.VerticalLine.Axes.XAxis = XAxis1;

            CrosshairCollection!.Last().PlotLine!.HorizontalLine.Axes.YAxis = YAxisList[0];
            CrosshairCollection!.Last().PlotLine!.HorizontalLine.Axes.XAxis = XAxis1;
        });

        RemoveLabelsBtn = ReactiveCommand.Create(() =>
        {
            ClearLabels();
            ClearAxisCrosshairs();
        });

        ExpandMenuBtn = ReactiveCommand.Create(() =>
        {
            IsMenuExpanded = !IsMenuExpanded;
        }).DisposeWith(Disposables);

        AxesSetup(); // axes colors setup

        Initializations2();

        LinePropCommand = ReactiveCommand.Create(() =>
        {
            RightPropertyVisibility = Visibility.Collapsed;
        }).DisposeWith(Disposables);

        // update plottables when this changes
        this.WhenAnyValue(x => x.NumberPointsPlotted).Subscribe(x =>
        {
            foreach (var item in PlotLinesCollectionUI)
            {
                item.NumberPointsPlotted = x;
            }
        }).DisposeWith(Disposables);

        this.WhenAnyValue(x => x.UseFixedNumberOfPoints).Subscribe(x =>
        {
            foreach (var item in PlotLinesCollectionUI)
            {
                item.UseFixedNumberOfPoints = x;
            }
        }).DisposeWith(Disposables);
    }

    /// <summary>
    /// Gets or sets the collection of Y-axis objects used for plotting data on the chart.
    /// </summary>
    /// <remarks>The collection supports up to four Y-axes, allowing advanced charting scenarios such as
    /// multiple scales or overlaying different data series. Modifying this property updates the axes displayed on the
    /// chart. The order of axes in the collection determines their placement and association with data
    /// series.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<IYAxis> Axes { get; set; }
#else
    public ReactiveList<IAxis> Axes { get; set; }
#endif

    /// <summary>
    /// Gets the collection of axes used for plotting data in the chart.
    /// </summary>
    /// <remarks>Each axis in the collection defines a coordinate system for the chart. Modifying this
    /// collection allows customization of axis types, scales, and appearance. Changes to the collection may affect how
    /// data is rendered and interpreted.</remarks>
    #if NET8_0_OR_GREATER
    public QuaternaryList<IPlottableUI> PlotLinesCollectionUI { get; }
#else
    public ReactiveList<IPlottableUI> PlotLinesCollectionUI { get; }
#endif

    /// <summary>
    /// Gets the collection of plot line UI elements displayed in the chart.
    /// </summary>
    /// <remarks>The collection is reactive, allowing dynamic updates to the chart's plot lines in response to
    /// changes. Modifying this collection will automatically update the chart's visual representation. Thread safety is
    /// not guaranteed; access from multiple threads should be synchronized.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<ChartObjects> ControlMenu { get; }
#else
    public ReactiveList<ChartObjects> ControlMenu { get; }
#endif

    /// <summary>
    /// Gets the collection of chart objects that represent the controls available in the menu interface.
    /// </summary>
#if NET6_0_OR_GREATER
    public QuaternaryList<AxisLinesUI> AxisLinesUI { get; }
#else
    public ReactiveList<AxisLinesUI> AxisLinesUI { get; }
#endif

    /// <summary>
    /// Gets the collection of axis line UI elements associated with the control.
    /// </summary>
    /// <remarks>The returned list is reactive, allowing dynamic updates to the axis lines in response to
    /// changes in the underlying data or user interactions. Modifications to this collection will be reflected in the
    /// UI automatically.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<Crosshair_UI> CrosshairCollection { get; }
#else
    public ReactiveList<Crosshair_UI> CrosshairCollection { get; }
#endif

    /// <summary>
    /// Gets the collection of crosshair UI elements currently managed by the control.
    /// </summary>
    /// <remarks>The collection is reactive, allowing observers to track changes such as additions or removals
    /// of crosshair elements in real time. This property is read-only; to modify the collection, use the methods
    /// provided by the underlying ReactiveList.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<(Marker marker, Text text)> LabelCollection { get; }
#else
    public ReactiveList<(Marker marker, Text text)> LabelCollection { get; }
#endif

    /// <summary>
    /// Gets or sets the collection of marker and text label pairs displayed on the plot.
    /// </summary>
    /// <remarks>The collection is reactive and updates automatically when items are added or removed. Each
    /// tuple contains a marker and its associated text label, allowing for dynamic annotation of the plot.</remarks>
    public WpfPlot? WpfPlot1vm
    {
        get => _wpfPlot1;
        set => this.RaiseAndSetIfChanged(ref _wpfPlot1, value);
    }

    /// <summary>
    /// Gets or sets the observable stream for mouse coordinate updates.
    /// </summary>
    /// <remarks>Subscribers to this observable receive notifications whenever the mouse coordinates change.
    /// The stream emits values of type <see cref="Coordinates"/> representing the current mouse position.</remarks>
    public ISubject<Coordinates> MouseCoordinatesObservable { get; set; }

    /// <summary>
    /// Gets the command that is executed when the graph is locked, preventing further modifications.
    /// </summary>
    /// <remarks>Use this command to signal that the graph should enter a locked state. While the graph is
    /// locked, operations that modify its structure or data may be disabled or ignored. The command completes when the
    /// lock is applied.</remarks>
    public ReactiveCommand<Unit, Unit>? GraphLocked { get; }

    /// <summary>
    /// Gets the command that enables the marker button in the user interface.
    /// </summary>
    /// <remarks>The command can be bound to UI elements to control the enabled state of the marker button.
    /// The command is reactive and may be null if the marker button is not available in the current context.</remarks>
    public ReactiveCommand<Unit, Unit>? EnableMarkerBtn { get; }

    /// <summary>
    /// Gets the command that adds a crosshair to the current context when executed.
    /// </summary>
    /// <remarks>The command is typically bound to a user interface element, such as a button, to enable users
    /// to add a crosshair interactively. The command is disabled if adding a crosshair is not currently
    /// permitted.</remarks>
    public ReactiveCommand<Unit, Unit>? AddCrosshairBtn { get; }

    /// <summary>
    /// Gets the command that removes all labels from the current selection.
    /// </summary>
    /// <remarks>The command is enabled only when labels can be removed from the selection. Use this property
    /// to bind UI elements, such as a button, to the label removal functionality.</remarks>
    public ReactiveCommand<Unit, Unit>? RemoveLabelsBtn { get; }

    /// <summary>
    /// Gets the command that expands the menu when executed.
    /// </summary>
    /// <remarks>The command can be bound to UI elements to trigger menu expansion. The property may be null
    /// if the command is not available in the current context.</remarks>
    public ReactiveCommand<Unit, Unit>? ExpandMenuBtn { get; }

    /// <summary>
    /// Gets or sets the visibility state of the left panel.
    /// </summary>
    public Visibility LeftPanelVisibility { get; set; }

    /// <summary>
    /// Gets the command that executes the line property action.
    /// </summary>
    /// <remarks>This command can be used to trigger logic related to line property changes, typically in
    /// response to user interaction. The command does not require input parameters and does not produce a result value.
    /// It is intended for use in reactive UI scenarios where command binding is supported.</remarks>
    public ReactiveCommand<Unit, Unit> LinePropCommand { get; }

    /// <summary>
    /// Gets or sets the collection of Y-axis objects used for plotting data.
    /// </summary>
    /// <remarks>Use this property to configure multiple Y-axes for advanced charting scenarios. The
    /// collection supports up to four Y-axis instances, allowing for complex visualizations with multiple scales or
    /// units.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<IYAxis> YAxisList { get; set; }
#else
    public ReactiveList<IYAxis> YAxisList { get; set; }
#endif

    /// <summary>
    /// Gets or sets the collection of Y-axis definitions used for plotting data on the chart.
    /// </summary>
    /// <remarks>Each item in the collection represents a separate Y-axis. Modifying the collection affects
    /// how data series are mapped to Y-axes in the chart. The order of axes in the list determines their display
    /// sequence.</remarks>
    public IXAxis XAxis1 { get; set; }

    /// <summary>
    /// Creates an action that automatically adjusts the horizontal axis scaling of a plot using the specified X-axis
    /// configuration.
    /// </summary>
    /// <param name="xaxis">The X-axis configuration to use for auto-scaling. Cannot be null.</param>
    /// <returns>An action that, when invoked with a <see cref="RenderPack"/>, applies automatic scaling to the plot's X-axis
    /// based on the provided configuration.</returns>
    public static Action<RenderPack> AutoScaleX(IXAxis xaxis) =>
        rp => rp.Plot.Axes.AutoScaleX(xaxis);

    /// <summary>
    /// Creates an action that automatically adjusts the Y-axis range of a plot to fit its data within the specified
    /// <see cref="RenderPack"/>.
    /// </summary>
    /// <remarks>This method is useful for ensuring that all data points are visible along the Y-axis after
    /// data changes or updates. The X-axis range is not affected by this action.</remarks>
    /// <returns>An <see cref="Action{RenderPack}"/> that, when invoked, rescales the Y-axis of the plot contained in the
    /// provided <see cref="RenderPack"/> to encompass all visible data.</returns>
    public static Action<RenderPack> AutoScaleY() =>
        rp => rp.Plot.Axes.AutoScaleY();

    /// <summary>
    /// Creates an action that automatically adjusts all axes in a plot to fit the current data.
    /// </summary>
    /// <remarks>Use this action to ensure that all plot axes are scaled to include the full range of data.
    /// This is useful after adding, removing, or modifying data in the plot to guarantee that all content is
    /// visible.</remarks>
    /// <returns>An <see cref="Action{RenderPack}"/> that, when invoked, rescales all axes of the plot contained in the specified
    /// <see cref="RenderPack"/> to display all data points.</returns>
    public static Action<RenderPack> AutoScaleAll() =>
        rp => rp.Plot.Axes.AutoScale();

    /// <summary>
    /// Hides all Y-axis elements by setting their visibility to false.
    /// </summary>
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

    /// <summary>
    /// Configures the plot axes and grid to hide tick labels, remove tick marks, and display the plot without a frame.
    /// </summary>
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

    /// <summary>
    /// Removes the existing bottom axis from the plot and creates a new bottom axis with updated settings.
    /// </summary>
    /// <remarks>Call this method to reset the bottom axis of the plot, typically when axis configuration or
    /// appearance needs to be refreshed. The method also updates the axis color to match the current settings. If the
    /// plot view model is not initialized, the method does nothing.</remarks>
    public void CreateAxisWithPoints()
    {
        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Bottom);
        XAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        SetXAxisColour();
    }

    /// <summary>
    /// Sets the colors for the X-axis labels, ticks, and frame to predefined values for improved visual clarity.
    /// </summary>
    /// <remarks>This method updates the color scheme of the X-axis elements, including label text, tick
    /// marks, and frame lines. Call this method to apply a consistent color style to the X-axis in the chart. This
    /// operation does not affect other axes or chart elements.</remarks>
    public void SetXAxisColour()
    {
        var baseColor = Color.FromHex("#D0D0D0");

        // colors
        XAxis1.Label.ForeColor = Color.FromHex("#377eb8");
        XAxis1.FrameLineStyle.Color = baseColor;
        XAxis1.TickLabelStyle.ForeColor = baseColor;
        XAxis1.MajorTickStyle.Color = baseColor;
        XAxis1.MinorTickStyle.Color = baseColor;
    }

    /// <summary>
    /// Removes all label markers and associated text from the plot and clears the label collection.
    /// </summary>
    /// <remarks>This method updates the plot by removing all labels currently stored in the label collection
    /// and then refreshes the display. If there are no labels to clear, the method has no effect.</remarks>
    public void ClearLabels()
    {
        if (LabelCollection == null)
        {
            return;
        }

        if (LabelCollection.Count == 0)
        {
            return;
        }

        foreach (var (marker, text) in LabelCollection)
        {
            WpfPlot1vm!.Plot.PlottableList.Remove(marker!);
            WpfPlot1vm!.Plot.PlottableList.Remove(text!);
        }

        LabelCollection.Clear();

        WpfPlot1vm!.Refresh();
    }

    /// <summary>
    /// Removes all axis-related crosshair lines and markers from the plot and clears the crosshair collection.
    /// </summary>
    /// <remarks>This method disposes of each crosshair item and removes its associated visual elements from
    /// the plot. After clearing, the plot is refreshed to reflect the changes. Calling this method when no crosshair
    /// items are present has no effect.</remarks>
    public void ClearAxisCrosshairs()
    {
        if (CrosshairCollection == null)
        {
            return;
        }

        if (CrosshairCollection.Count == 0)
        {
            return;
        }

        foreach (var item in CrosshairCollection)
        {
            WpfPlot1vm!.Plot.PlottableList.Remove(item.PlotLine!);
            WpfPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.Crosshair!);
            WpfPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.Marker!);
            WpfPlot1vm!.Plot.PlottableList.Remove(item.ChartSettings.MarkerText!);
            item.Dispose();
        }

        CrosshairCollection.Clear();

        WpfPlot1vm!.Refresh();
    }

    /// <summary>
    /// Removes all axis lines from the plot and disposes of their associated resources.
    /// </summary>
    /// <remarks>This method clears the collection of axis line UI elements and removes their corresponding
    /// plottables from the plot. If there are no axis lines present, the method performs no action.</remarks>
    public void ClearAxisLines()
    {
        if (AxisLinesUI == null)
        {
            return;
        }

        if (AxisLinesUI.Count == 0)
        {
            return;
        }

        foreach (var item in AxisLinesUI)
        {
            WpfPlot1vm!.Plot.PlottableList.Remove(item.AxisLine!);
            item.Dispose();
        }

        AxisLinesUI.Clear();
    }

    /// <summary>
    /// Removes all plot data, controls, and labels from the current view, releasing associated resources.
    /// </summary>
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

    /// <summary>
    /// Configures the Y-axes of the plot using the specified axis names and colors. Existing Y-axes are removed and
    /// replaced with new right-side axes corresponding to the provided data.
    /// </summary>
    /// <remarks>The method always hides the left Y-axis and creates new right-side axes for each entry in the
    /// provided lists. Each axis label is assigned the corresponding color from the hexColors list. If the input lists
    /// differ in length, only pairs up to the shortest list are used.</remarks>
    /// <param name="data">A tuple containing the list of Y-axis names and the list of hexadecimal color strings to apply to each axis
    /// label. Both lists must have the same number of elements.</param>
    public void YAxesSetup((IList<string> yNames, IList<string> hexColors) data)
    {
        var baseColor = Color.FromHex("#D0D0D0");

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

    /// <summary>
    /// Sets the Y-axis limits of all axes in the plot to the range 0 to 100.
    /// </summary>
    /// <remarks>This method applies the specified Y-axis range to each axis in the collection and refreshes
    /// the plot to reflect the changes. Use this method to manually reset the Y-axis scaling when automatic scaling is
    /// not desired.</remarks>
    public void ManualScaleY()
    {
        foreach (var axis in YAxisList)
        {
            WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 100, axis);
        }

        WpfPlot1vm?.Refresh();
    }

    /// <summary>
    /// Selects a color name from a predefined set based on the number of items in the provided legend collection.
    /// </summary>
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
                        "Red"
                    ];

        // add data
        var n = (int)(legend.Count() % colors.Count);
        return colors[n];
    }

    /// <summary>
    /// Configures the appearance and labeling of the plot axes, including colors and units for both X and Y axes.
    /// </summary>
    /// <remarks>This method sets up the left axis, customizes the X axis color scheme, and adds three right Y
    /// axes with distinct labels and colors for phase, displacement, and temperature. It should be called during plot
    /// initialization to ensure axes are correctly styled and labeled before displaying data.</remarks>
    private void AxesSetup()
    {
        var baseColor = Color.FromHex("#D0D0D0");

        HideLeftAxis(baseColor);

        // colors
        XAxis1.Label.ForeColor = Color.FromHex("#377eb8");
        XAxis1.FrameLineStyle.Color = baseColor;
        XAxis1.TickLabelStyle.ForeColor = baseColor;
        XAxis1.MajorTickStyle.Color = baseColor;
        XAxis1.MinorTickStyle.Color = baseColor;

        // Setup Y Axis
        for (var i = 0; i < 3; i++)
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
        YAxisList[0].Label.ForeColor = Color.FromHex("#377eb8");

        YAxisList[1].Label.Text = "[um]";
        YAxisList[1].Label.ForeColor = Color.FromHex("#4daf4a");

        YAxisList[2].Label.Text = "[°C]";
        YAxisList[2].Label.ForeColor = Color.FromHex("#984ea3");

        AxisStyle();
    }

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
        var backColour = Color.FromHex("#252526");
        l!.Label.ForeColor = backColour;
        l.FrameLineStyle.Color = color;
        l.TickLabelStyle.ForeColor = backColour;
        l.MajorTickStyle.Color = backColour;
        l.MinorTickStyle.Color = backColour;
    }

    /// <summary>
    /// Sets the X-axis limits of the plot to display data from the past 60 minutes up to the current time.
    /// </summary>
    /// <remarks>Refreshes the plot after updating the axis limits. This method has no effect if the plot view
    /// model is null.</remarks>
    private void ManualScaleX()
    {
        var now = DateTime.Now;
        var doublenow = now.ToOADate();
        var limits = now.Add(TimeSpan.FromMinutes(-60));
        var doublelimits = limits.ToOADate();
        WpfPlot1vm?.Plot.Axes.SetLimitsX(doublelimits, doublenow, XAxis1);
        WpfPlot1vm?.Refresh();
    }

    /// <summary>
    /// Applies a dark mode color scheme to the plot, updating background, axis, grid, and legend colors for improved
    /// visibility in low-light environments.
    /// </summary>
    /// <remarks>This method sets multiple visual elements of the plot to colors suitable for dark mode,
    /// including the figure background, data background, axes, grid lines, and legend. The palette is also updated to
    /// match the dark theme. Use this method to enhance readability and reduce eye strain when displaying plots in
    /// dark-themed applications.</remarks>
    /// <param name="backgroundColorHex">A hexadecimal color string that specifies the background color to use for the plot. Defaults to "#252526" if not
    /// provided. Must be a valid hex color code.</param>
    private void UseDarkMode(string backgroundColorHex = "#252526")
    {
        var color = Color.FromHex(backgroundColorHex);
        var axisColor = Color.FromHex("#d7d7d7");
        WpfPlot1vm!.Plot.Add.Palette = new Penumbra();
        WpfPlot1vm.Plot.Axes.Color(axisColor);
        WpfPlot1vm.Plot.Grid.MajorLineColor = Color.FromHex("#404040");
        WpfPlot1vm.Plot.FigureBackground.Color = color;
        WpfPlot1vm.Plot.DataBackground.Color = color;
        WpfPlot1vm.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
        WpfPlot1vm.Plot.Legend.FontColor = axisColor;
        WpfPlot1vm.Plot.Legend.OutlineColor = axisColor;
    }
}
