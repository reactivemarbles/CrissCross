// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveMarbles.ObservableEvents;
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
[SupportedOSPlatform("windows10.0.19041")]
public partial class LiveChartViewModel : RxObject
{
    [Reactive]
    private bool _isXAxisDateTime;

    [Reactive]
    private int _numberPointsPlotted;

    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>
    /// Initializes and configures up to 16 plot lines in the chart using the provided data and UI creation logic,
    /// assigning each plot line to the appropriate Y-axis and setting up the X-axis as either date/time or numeric
    /// points.
    /// </summary>
    /// <remarks>If more than 16 data items are provided, only the first 16 will be plotted. Each plot line is
    /// assigned to a Y-axis based on the value returned by <paramref name="getYAxis"/>. If an observable is returned,
    /// the plot line will update its Y-axis assignment dynamically as the observable emits new values. The method
    /// clears existing chart content before initializing new plot lines.</remarks>
    /// <typeparam name="T">The type of the data items to be plotted. Each item represents a single plot line.</typeparam>
    /// <param name="data">The collection of data items to be plotted. Cannot be null. A maximum of 16 items will be processed.</param>
    /// <param name="getYAxis">A function that determines the Y-axis assignment for each data item. Returns either an integer index or an
    /// observable of integer indices. Cannot be null.</param>
    /// <param name="createPlotUI">A function that creates the plot line UI element for each data item. Cannot be null.</param>
    /// <param name="isXAxisDateTime">Specifies whether the X-axis should be configured for date/time values (<see langword="true"/>) or numeric
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
        // check if null
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (getYAxis is null)
        {
            throw new ArgumentNullException(nameof(getYAxis));
        }

        if (createPlotUI is null)
        {
            throw new ArgumentNullException(nameof(createPlotUI));
        }

        // clear
        ClearContent();

        // configure XAxis in Chart
        if (isXAxisDateTime)
        {
            IsXAxisDateTime = true;
            CreateAxisWithTimeStamp();
        }
        else
        {
            IsXAxisDateTime = false;
            CreateAxisWithPoints();
        }

        // hide axis
        HideAllYAxis();

        // each plot line should be latched to a XAxis and YAxis
        var i = 0;
        foreach (var plotLine in data)
        {
            // maximum ploted lines : 16
            if (i >= 16)
            {
                break;
            }

            // create the plot line
            var newMyItem = createPlotUI(plotLine);

            // Configure axis for each plot
            IPlottable? line = newMyItem switch
            {
                SignalUI signal => signal.PlotLine!,
                ScatterUI scatter => scatter.PlotLine!,
                DataLoggerUI logger => logger.PlotLine!,
                SignalXY_UI signalXY => signalXY.PlotLine!,
                StreamerUI streamer => streamer.PlotLine!,
                _ => throw new ArgumentNullException(nameof(newMyItem)),
            };
            var lineUI = (IPlottableUI?)newMyItem;

            // axis visibility
            if (getYAxis(plotLine) is IObservable<int> observable)
            {
                // subscribe to a YAxis changes
                observable.DistinctUntilChanged().Subscribe(a =>
                {
                    // check axis is in range
                    if (a < 0 || a >= YAxisList.Count)
                    {
                        throw new IndexOutOfRangeException($"Invalid Y-axis index: {a}");
                    }

                    // visibility
                    for (var j = 0; j < YAxisList.Count; j++)
                    {
                        YAxisList[j].IsVisible = YAxisList[j].IsVisible || a == j;
                    }

                    // latch line to the axis
                    var yaxisListItem = YAxisList[a];
                    lineUI!.ChartSettings!.Marker!.Axes.YAxis = yaxisListItem;
                    lineUI!.ChartSettings.MarkerText!.Axes.YAxis = yaxisListItem;
                    lineUI!.ChartSettings.Crosshair!.Axes.YAxis = yaxisListItem;
                    line!.Axes.YAxis = yaxisListItem;
                }).DisposeWith(Disposables);
            }
            else if (getYAxis(plotLine) is int a)
            {
                // check axis is in range
                if (a < 0 || a >= YAxisList.Count)
                {
                    throw new IndexOutOfRangeException($"Invalid Y-axis index: {a}");
                }

                // visibility
                for (var j = 0; j < YAxisList.Count; j++)
                {
                    YAxisList[j].IsVisible = YAxisList[j].IsVisible || a == j;
                }

                // latch line to the axis
                var yaxisListItem = YAxisList[a];
                lineUI!.ChartSettings!.Marker!.Axes.YAxis = yaxisListItem;
                lineUI!.ChartSettings.MarkerText!.Axes.YAxis = yaxisListItem;
                lineUI!.ChartSettings.Crosshair!.Axes.YAxis = yaxisListItem;
                line!.Axes.YAxis = yaxisListItem;
            }
            else
            {
                // latch line to the axis
                var yaxisListItem = YAxisList[0];
                yaxisListItem.IsVisible = true;
                lineUI!.ChartSettings!.Marker!.Axes.YAxis = yaxisListItem;
                lineUI!.ChartSettings.MarkerText!.Axes.YAxis = yaxisListItem;
                lineUI!.ChartSettings.Crosshair!.Axes.YAxis = yaxisListItem;
                line!.Axes.YAxis = yaxisListItem;
            }

            PlotLinesCollectionUI!.Add(lineUI!);
            i++;
        }

        // UPDATE ChartObjects COLLECTION for external access
        UpdateChartObjectsCollection();
    }

    /// <summary>
    /// Initializes scatter plot lines using the provided collection of observable data series.
    /// </summary>
    /// <remarks>This method configures the scatter plot to use date-time values on the X-axis. Each data
    /// series is visualized as a scatter plot line and associated with the specified axis. The color legend is updated
    /// to reflect the plotted series.</remarks>
    /// <param name="observables">A collection of observables, each representing a data series for the scatter plot. Each observable provides a
    /// tuple containing an optional series name, a list of Y-axis values, a list of X-axis date-time values, and the
    /// axis index to which the series belongs.</param>
    public void InitializeScatterPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new ScatterUI(WpfPlot1vm!, obs, SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: true);

    /// <summary>
    /// Initializes plot lines for scatter plots using a collection of observable point sequences.
    /// </summary>
    /// <remarks>Use this method to set up scatter plot lines that update dynamically as the underlying
    /// observable data changes. Each observable sequence represents a distinct data series on the plot. The method does
    /// not treat X values as date/time data; X values are interpreted as numeric.</remarks>
    /// <param name="observables">A collection of observables, each providing a sequence of points to plot. Each observable emits tuples
    /// containing an optional series name, optional X values, required Y values, and the axis index for plotting.</param>
    public void InitializeLinesForScatterObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new ScatterUI(WpfPlot1vm!, obs, SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes plot lines for signal data using the provided collection of observables.
    /// </summary>
    /// <remarks>This method configures the plot to use date-time values on the X-axis and associates each
    /// signal with its specified axis. The plot lines are created with support for color legends and fixed-point
    /// options as determined by the current view model settings.</remarks>
    /// <param name="observables">A collection of observables, each representing a signal with a name, value series, date-time series, and axis
    /// index. Each observable supplies the data to be plotted as a signal line.</param>
    public void InitializeSignalPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new SignalUI(WpfPlot1vm!, observable: obs, coordinatesObs: MouseCoordinatesObservable, SetColorLegend(PlotLinesCollectionUI!), fixedPoints: this.WhenAnyValue(x => x.UseFixedNumberOfPoints), numberPointsPlotted: this.WhenAnyValue(x => x.NumberPointsPlotted)),
        isXAxisDateTime: true);

    /// <summary>
    /// Initializes plot lines with data points for the data logger using the provided observables.
    /// </summary>
    /// <remarks>Each observable in the collection is used to create a corresponding plot line in the data
    /// logger UI. The Y-axis for each plot line is determined by the 'Axis' value in the tuple. The method does not use
    /// a DateTime axis for the X-axis.</remarks>
    /// <param name="observables">A collection of observable sequences, each providing a tuple containing the series name, a list of data point
    /// values, the Y-axis index, and the maximum number of points to display. Each observable represents a data series
    /// to be plotted.</param>
    public void InitializeDataLoggerPlotLinesWithPoints(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new DataLoggerUI(plot: WpfPlot1vm!, observable: obs, color: SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes plot lines for a signal using the specified data, configuring the plot to use a date-time X axis.
    /// </summary>
    /// <remarks>This method sets up the plot lines for a single signal, using date-time values for the X
    /// axis. The plot UI is created and color legend is set automatically. If 'Value' is null or empty, no data points
    /// will be plotted.</remarks>
    /// <param name="data">A tuple containing the signal's name, values, date-time points, and the axis index to plot against. The 'Name'
    /// may be null to indicate an unnamed signal. 'Value' is the collection of Y values; may be null if no data is
    /// available. 'DateTime' is the collection of X values representing date-time points. 'Axis' specifies the Y axis
    /// index for plotting.</param>
    public void InitializeSignalPlotLines((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data) =>
        InitializeGenericPlotLines(
        data: [data],
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(WpfPlot1vm!, (data.Name, data.Value, data.DateTime, data.Axis), SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: true);

    /// <summary>
    /// Initializes plot lines for a single set of signal points using the specified data tuple.
    /// </summary>
    /// <remarks>Use this method to visualize a single signal on the plot, associating its values and
    /// timestamps with the specified axis. The method does not treat the X-axis as a date/time axis; the 'DateTime'
    /// values are used as X-coordinates. All elements of the tuple must be provided; ensure that 'Value' and 'DateTime'
    /// lists are of equal length for correct plotting.</remarks>
    /// <param name="data">A tuple containing the signal's name, value list, date/time list, and axis index. The 'Name' identifies the
    /// signal; 'Value' provides the Y-values; 'DateTime' supplies the X-values; and 'Axis' specifies which Y-axis to
    /// use for plotting.</param>
    public void InitializeLinesForSignalPoints((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data) =>
        InitializeGenericPlotLines(
        data: [data],
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(plot: WpfPlot1vm!, data: d, color: SetColorLegend(PlotLinesCollectionUI!), coordinatesObs: MouseCoordinatesObservable),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes plot lines for a collection of signal points, configuring each line based on the provided data
    /// series.
    /// </summary>
    /// <remarks>Each signal point is rendered as a line on the plot, with its axis assignment determined by
    /// the Axis value in the tuple. The method does not treat the X-axis as date-time values. Ensure that the DateTime
    /// list in each tuple contains valid values corresponding to the data series.</remarks>
    /// <param name="data">A collection of tuples representing signal point data. Each tuple contains an optional name, an optional list of
    /// values, a list of date-time values, and an axis index specifying which Y-axis to use for the series.</param>
    public void InitializeLinesForSignalPoints(IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> data) =>
        InitializeGenericPlotLines(
        data: data,
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(plot: WpfPlot1vm!, data: d, color: SetColorLegend(PlotLinesCollectionUI!), coordinatesObs: MouseCoordinatesObservable),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes plot lines for a collection of signal observables, configuring each line to display its
    /// corresponding data points on the plot.
    /// </summary>
    /// <remarks>Each observable is associated with a plot line and UI element, allowing real-time
    /// visualization of signal data. The method configures the plot for numeric X-axis values and assigns colors to
    /// each line for legend differentiation.</remarks>
    /// <param name="observables">A collection of observables, each providing a tuple containing an optional signal name, Y-values, X-values, and
    /// the axis index to plot against.</param>
    /// <param name="fs">The sampling frequency, in Hertz, used to configure the plot lines for the signal data.</param>
    /// <param name="nSamples">The number of samples to display for each signal on the plot. Must be a positive value.</param>
    public void InitializeLinesForSignalObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> observables, int fs, uint nSamples) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new StreamerUI(plot: WpfPlot1vm!, fs: fs, nSamples: nSamples, nPointsPlotted: NumberPointsPlotted, observable: obs, color: SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes and adds a new scatter plot line to the chart using the specified data points and axis.
    /// </summary>
    /// <remarks>This method updates the chart's visual elements and ensures the corresponding axis is visible
    /// for the new scatter series. After initialization, the chart objects collection is refreshed to reflect the
    /// changes for external access.</remarks>
    /// <param name="data">A tuple containing the series name, the X and Y coordinate lists, and the axis index to which the scatter points
    /// are assigned. The axis index must be within the range of available axes.</param>
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

        PlotLinesCollectionUI!.Add(newMyItem);

        // UPDATE ChartObjects COLLECTION for external access
        UpdateChartObjectsCollection();
    }

    /// <summary>
    /// Initializes the control menu with the specified chart object settings, replacing any existing items.
    /// </summary>
    /// <remarks>Existing items in the control menu are disposed before the new settings are applied. This
    /// method does not modify the control menu if <paramref name="settings"/> is <see langword="null"/>.</remarks>
    /// <param name="settings">A list of chart object settings to populate the control menu. If <paramref name="settings"/> is <see
    /// langword="null"/>, the control menu is not modified.</param>
    public void InitializeControlMenu(IList<ChartObjects>? settings)
    {
        if (settings == null)
        {
            return;
        }

        foreach (var item in ControlMenu)
        {
            item.Dispose();
        }

        ControlMenu!.Clear();
        ControlMenu!.AddRange(settings);
    }

    /// <summary>
    /// Initializes and configures axis line elements for the plot, adding them to the plottable collection if
    /// available.
    /// </summary>
    /// <remarks>This method assigns the appropriate X and Y axes to each axis line and ensures that only
    /// non-null axis lines are processed. If no axis lines are present, the method performs no action.</remarks>
    public void InitializeAxisLines()
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
            WpfPlot1vm!.Plot.PlottableList.Add(item.AxisLine!);
            item.AxisLine!.Axes.YAxis = YAxisList[item.Axis];
            item.AxisLine!.Axes.XAxis = XAxis1;
        }
    }

    /// <summary>
    /// Performs initialization tasks related to mouse observables and refreshes the plot view model.
    /// </summary>
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
        WpfPlot1vm!.Events().MouseMove.Retry().Subscribe(e =>
        {
            // MOUSE EVENT
            var position = e.GetPosition(e.Device.Target);

            //// determine where the mouse is and send the coordinates
            Pixel mousePixel = new(position.X, position.Y);
            var mouseLocation = WpfPlot1vm!.Plot.GetCoordinates(mousePixel);
            try
            {
                MouseCoordinatesObservable.OnNext(mouseLocation);

                Trace.WriteLine("Mouse Location: { X: " + position.X + " Y: " + position.Y + " }");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("mouse location error: " + ex);
            }
        }).DisposeWith(Disposables);
}
