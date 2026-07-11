// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Provides functionality for managing and initializing live chart plot lines, axes, and control menus in a reactive
/// charting environment.
/// </summary>
/// <remarks>This class supports dynamic creation and configuration of various plot types, including signal,
/// scatter, and data logger plots, with flexible axis assignment and control menu management. It is intended for use on
/// Windows 10 version 19041 or later, as indicated by the platform requirement. Thread safety and UI update
/// considerations depend on the underlying RxObject and charting components.</remarks>
[SupportedOSPlatform("windows")]
public partial class LiveChartViewModel : RxObject
{
    /// <summary>Stores the is xaxis date time value.</summary>
    [Reactive]
    private bool _isXAxisDateTime;

    /// <summary>Stores the number points plotted value.</summary>
    [Reactive]
    private int _numberPointsPlotted;

    /// <summary>Stores the use fixed number of points value.</summary>
    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>Initializes and configures up to 16 plot lines in the chart using the provided data and UI creation logic, assigning each plot line to the appropriate Y-axis and setting up the X-axis as either date/time or numeric points.</summary>
    /// <typeparam name="T">The T type.</typeparam>
    /// <param name="data">The data value.</param>
    /// <param name="getYAxis">The getYAxis value.</param>
    /// <param name="createPlotUI">The createPlotUI value.</param>
    /// <param name="isXAxisDateTime">The isXAxisDateTime value.</param>
    /// points (<see langword="false"/>).</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/>, <paramref name="getYAxis"/>, or <paramref name="createPlotUI"/> is null, or
    /// if a plot UI element cannot be created for a data item.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if the Y-axis index provided by <paramref name="getYAxis"/> is less than zero or greater than or equal to
    /// the number of available Y-axes.</exception>
    public void InitializeGenericPlotLines<T>(
    IEnumerable<T> data,
    Func<T, object>? getYAxis,
    Func<T, object> createPlotUI,
    bool isXAxisDateTime)
    {
        ValidateGenericPlotLineArguments(data, getYAxis, createPlotUI);
        ClearContent();
        ConfigureXAxis(isXAxisDateTime);
        HideAllYAxis();

        foreach (var plotLine in data.Take(16))
        {
            AddGenericPlotLine(plotLine, getYAxis!, createPlotUI);
        }

        UpdateChartObjectsCollection();
    }

    /// <summary>Initializes scatter plot lines using the provided collection of observable data series.</summary>
    /// <param name="observables">The observables value.</param>
    public void InitializeScatterPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new ScatterUI(WpfPlot1vm!, obs, SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: true);

    /// <summary>Initializes plot lines for scatter plots using a collection of observable point sequences.</summary>
    /// <param name="observables">The observables value.</param>
    public void InitializeLinesForScatterObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new ScatterUI(WpfPlot1vm!, obs, SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>Initializes plot lines for signal data using the provided collection of observables.</summary>
    /// <param name="observables">The observables value.</param>
    public void InitializeSignalPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new SignalUI(WpfPlot1vm!, observable: obs, coordinatesObs: MouseCoordinatesObservable, SetColorLegend(PlotLinesCollectionUI!), fixedPoints: this.WhenAnyValue(x => x.UseFixedNumberOfPoints), numberPointsPlotted: this.WhenAnyValue(x => x.NumberPointsPlotted)),
        isXAxisDateTime: true);

    /// <summary>Initializes plot lines with data points for the data logger using the provided observables.</summary>
    /// <param name="observables">The observables value.</param>
    public void InitializeDataLoggerPlotLinesWithPoints(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new DataLoggerUI(plot: WpfPlot1vm!, observable: obs, color: SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>Initializes plot lines for a signal using the specified data, configuring the plot to use a date-time X axis.</summary>
    /// <param name="data">The data value.</param>
    public void InitializeSignalPlotLines((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data) =>
        InitializeGenericPlotLines(
        data: [data],
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(WpfPlot1vm!, (data.Name, data.Value, data.DateTime, data.Axis), SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: true);

    /// <summary>Initializes plot lines for a single set of signal points using the specified data tuple.</summary>
    /// <param name="data">The data value.</param>
    public void InitializeLinesForSignalPoints((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data) =>
        InitializeGenericPlotLines(
        data: [data],
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(plot: WpfPlot1vm!, data: d, color: SetColorLegend(PlotLinesCollectionUI!), coordinatesObs: MouseCoordinatesObservable),
        isXAxisDateTime: false);

    /// <summary>Initializes plot lines for a collection of signal points, configuring each line based on the provided data series.</summary>
    /// <param name="data">The data value.</param>
    public void InitializeLinesForSignalPoints(IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> data) =>
        InitializeGenericPlotLines(
        data: data,
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(plot: WpfPlot1vm!, data: d, color: SetColorLegend(PlotLinesCollectionUI!), coordinatesObs: MouseCoordinatesObservable),
        isXAxisDateTime: false);

    /// <summary>Initializes plot lines for a collection of signal observables, configuring each line to display its corresponding data points on the plot.</summary>
    /// <param name="observables">The observables value.</param>
    /// <param name="fs">The fs value.</param>
    /// <param name="sampleCount">The sample count value.</param>
    public void InitializeLinesForSignalObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> observables, int fs, uint sampleCount) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new StreamerUI(plot: WpfPlot1vm!, observable: obs, fs: fs, sampleCount: sampleCount, plottedPointCount: NumberPointsPlotted, color: SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>Initializes and adds a new scatter plot line to the chart using the specified data points and axis.</summary>
    /// <param name="data">The data value.</param>
    public void InitializeLinesForScatterPoints((string? Name, IList<double> X, IList<double> Y, int Axis) data)
    {
        var newMyItem = new ScatterUI(WpfPlot1vm!, (data.Name, data.X, data.Y, data.Axis), SetColorLegend(PlotLinesCollectionUI!));
        newMyItem.PlotLine!.Axes.YAxis = YAxisList[data.Axis];
        newMyItem.PlotLine!.MarkerSize = 1f;
        newMyItem.PlotLine!.LineWidth = 0.3f;

        for (var j = 0; j < YAxisList.Count; j++)
        {
            YAxisList[j].IsVisible = YAxisList[j].IsVisible || data.Axis == j;
        }

        PlotLinesCollectionUI.Add(newMyItem);

        // UPDATE ChartObjects COLLECTION for external access
        UpdateChartObjectsCollection();
    }

    /// <summary>Initializes the control menu with the specified chart object settings, replacing any existing items.</summary>
    /// <param name="settings">The settings value.</param>
    public void InitializeControlMenu(IList<ChartObjects>? settings)
    {
        if (settings is null)
        {
            return;
        }

        foreach (var item in ControlMenu)
        {
            item.Dispose();
        }

        ControlMenu.ReplaceAll(settings);
    }

    /// <summary>Initializes and configures axis line elements for the plot, adding them to the plottable collection if available.</summary>
    /// <remarks>This method assigns the appropriate X and Y axes to each axis line and ensures that only
    /// non-null axis lines are processed. If no axis lines are present, the method performs no action.</remarks>
    public void InitializeAxisLines()
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
            WpfPlot1vm!.Plot.PlottableList.Add(item.AxisLine!);
            item.AxisLine!.Axes.YAxis = YAxisList[item.Axis];
            item.AxisLine!.Axes.XAxis = XAxis1;
        }
    }

    /// <summary>Gets the plottable from a plot UI object.</summary>
    /// <param name="newItem">The plot UI object.</param>
    /// <returns>The plottable.</returns>
    private static IPlottable GetPlottable(object newItem) =>
        newItem switch
        {
            SignalUI signal => signal.PlotLine!,
            ScatterUI scatter => scatter.PlotLine!,
            DataLoggerUI logger => logger.PlotLine!,
            SignalXY_UI signalXY => signalXY.PlotLine!,
            StreamerUI streamer => streamer.PlotLine!,
            _ => throw new ArgumentNullException(nameof(newItem)),
        };

    /// <summary>Throws when a required argument is null.</summary>
    /// <typeparam name="T">The argument type.</typeparam>
    /// <param name="value">The argument value.</param>
    /// <param name="name">The argument name.</param>
    private static void ThrowIfNull<T>(T? value, string name)
        where T : class =>
        _ = value ?? throw new ArgumentNullException(name);

    /// <summary>Validates generic plot line initialization arguments.</summary>
    /// <typeparam name="T">The data item type.</typeparam>
    /// <param name="data">The plot data.</param>
    /// <param name="getYAxis">The axis selector.</param>
    /// <param name="createPlotUI">The plot UI factory.</param>
    private static void ValidateGenericPlotLineArguments<T>(IEnumerable<T> data, Func<T, object>? getYAxis, Func<T, object> createPlotUI)
    {
        ThrowIfNull(data, nameof(data));
        ThrowIfNull(getYAxis, nameof(getYAxis));
        ThrowIfNull(createPlotUI, nameof(createPlotUI));
    }

    /// <summary>Configures the X axis mode.</summary>
    /// <param name="isXAxisDateTime">A value indicating whether the X axis is date/time based.</param>
    private void ConfigureXAxis(bool isXAxisDateTime)
    {
        IsXAxisDateTime = isXAxisDateTime;
        if (isXAxisDateTime)
        {
            CreateAxisWithTimeStamp();
            return;
        }

        CreateAxisWithPoints();
    }

    /// <summary>Adds a single generic plot line.</summary>
    /// <typeparam name="T">The data item type.</typeparam>
    /// <param name="plotLine">The plot line data.</param>
    /// <param name="getYAxis">The axis selector.</param>
    /// <param name="createPlotUI">The plot UI factory.</param>
    private void AddGenericPlotLine<T>(T plotLine, Func<T, object> getYAxis, Func<T, object> createPlotUI)
    {
        var newItem = createPlotUI(plotLine);
        var line = GetPlottable(newItem);
        var lineUI = (IPlottableUI?)newItem ?? throw new ArgumentNullException(nameof(newItem));
        AssignYAxis(getYAxis(plotLine), line, lineUI);
        PlotLinesCollectionUI.Add(lineUI);
    }

    /// <summary>Assigns the Y axis to a plot line.</summary>
    /// <param name="axisSource">The axis source.</param>
    /// <param name="line">The plottable line.</param>
    /// <param name="lineUI">The plot UI element.</param>
    private void AssignYAxis(object axisSource, IPlottable line, IPlottableUI lineUI)
    {
        switch (axisSource)
        {
            case IObservable<int> observable:
                {
                    _ = observable.DistinctUntilChanged().Subscribe(axis => AssignYAxis(axis, line, lineUI)).DisposeWith(Disposables);
                    break;
                }

            case int axis:
                {
                    AssignYAxis(axis, line, lineUI);
                    break;
                }

            default:
                {
                    AssignDefaultYAxis(line, lineUI);
                    break;
                }
        }
    }

    /// <summary>Assigns a Y axis by index.</summary>
    /// <param name="axis">The Y axis index.</param>
    /// <param name="line">The plottable line.</param>
    /// <param name="lineUI">The plot UI element.</param>
    private void AssignYAxis(int axis, IPlottable line, IPlottableUI lineUI)
    {
        var verticalAxis = GetVisibleYAxis(axis);
        lineUI.ChartSettings!.Marker!.Axes.YAxis = verticalAxis;
        lineUI.ChartSettings.MarkerText!.Axes.YAxis = verticalAxis;
        lineUI.ChartSettings.Crosshair!.Axes.YAxis = verticalAxis;
        line.Axes.YAxis = verticalAxis;
    }

    /// <summary>Assigns the default Y axis.</summary>
    /// <param name="line">The plottable line.</param>
    /// <param name="lineUI">The plot UI element.</param>
    private void AssignDefaultYAxis(IPlottable line, IPlottableUI lineUI) => AssignYAxis(0, line, lineUI);

    /// <summary>Gets a visible Y axis by index.</summary>
    /// <param name="axis">The Y axis index.</param>
    /// <returns>The visible Y axis.</returns>
    private IYAxis GetVisibleYAxis(int axis)
    {
        if (axis < 0 || axis >= YAxisList.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Y-axis index.");
        }

        for (var j = 0; j < YAxisList.Count; j++)
        {
            YAxisList[j].IsVisible = YAxisList[j].IsVisible || axis == j;
        }

        return YAxisList[axis];
    }

    /// <summary>Performs initialization tasks related to mouse observables and refreshes the plot view model.</summary>
    private void Initializations2()
    {
        InitializeMouseObservable();
        WpfPlot1vm?.Refresh();
    }

    /// <summary>
    /// Initializes the observable sequence that monitors mouse movement events over the plot and publishes the
    /// corresponding plot coordinates.
    /// </summary>
    /// <remarks>This method sets up event handling so that each mouse movement over the plot surface results
    /// in the current mouse coordinates being emitted to observers. The observable sequence automatically retries on
    /// transient errors and disposes of its subscription when the parent disposables are disposed. This method should
    /// be called during view model initialization to ensure mouse coordinate updates are available.</remarks>
    private void InitializeMouseObservable() =>
        EventSignal
            .From<MouseEventHandler, MouseEventArgs>(handler => WpfPlot1vm!.MouseMove += handler, handler => WpfPlot1vm!.MouseMove -= handler)
            .Retry(int.MaxValue)
            .Subscribe(e =>
        {
            // MOUSE EVENT
            var position = e.GetPosition(e.Device.Target);

            //// determine where the mouse is and send the coordinates
            Pixel mousePixel = new(position.X, position.Y);
            var mouseLocation = WpfPlot1vm!.Plot.GetCoordinates(mousePixel);
            try
            {
                MouseCoordinatesObservable.OnNext(mouseLocation);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("mouse location error: " + ex);
            }
        }).DisposeWith(Disposables);
}
