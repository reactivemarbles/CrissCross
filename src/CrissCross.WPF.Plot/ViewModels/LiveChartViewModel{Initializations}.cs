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
/// AICSLiveChart.
/// </summary>
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
    /// Initializes the generic plot lines.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="data">The data.</param>
    /// <param name="getYAxis">The get y axis.</param>
    /// <param name="createPlotUI">The create plot UI.</param>
    /// <param name="isXAxisDateTime">if set to <c>true</c> [is x axis date time].</param>
    /// <exception cref="System.ArgumentNullException">
    /// data
    /// or
    /// getYAxis
    /// or
    /// createPlotUI
    /// or
    /// newMyItem.
    /// </exception>
    /// <exception cref="System.IndexOutOfRangeException">Invalid Y-axis index: {a}.</exception>
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
                    lineUI!.ChartSettings!.Marker!.Axes.YAxis = YAxisList[a];
                    lineUI!.ChartSettings.MarkerText!.Axes.YAxis = YAxisList[a];
                    lineUI!.ChartSettings.Crosshair!.Axes.YAxis = YAxisList[a];
                    line!.Axes.YAxis = YAxisList[a];
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
                lineUI!.ChartSettings!.Marker!.Axes.YAxis = YAxisList[a];
                lineUI!.ChartSettings.MarkerText!.Axes.YAxis = YAxisList[a];
                lineUI!.ChartSettings.Crosshair!.Axes.YAxis = YAxisList[a];
                line!.Axes.YAxis = YAxisList[a];
            }
            else
            {
                // latch line to the axis
                YAxisList[0].IsVisible = true;
                lineUI!.ChartSettings!.Marker!.Axes.YAxis = YAxisList[0];
                lineUI!.ChartSettings.MarkerText!.Axes.YAxis = YAxisList[0];
                lineUI!.ChartSettings.Crosshair!.Axes.YAxis = YAxisList[0];
                line!.Axes.YAxis = YAxisList[0];
            }

            PlotLinesCollectionUI!.Add(newMyItem);
            i++;
        }

        // UPDATE ChartObjects COLLECTION for external access
        UpdateChartObjectsCollection();
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeScatterPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new ScatterUI(WpfPlot1vm!, obs, SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: true);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeLinesForScatterObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new ScatterUI(WpfPlot1vm!, obs, SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeSignalPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new SignalUI(WpfPlot1vm!, observable: obs, coordinatesObs: MouseCoordinatesObservable, SetColorLegend(PlotLinesCollectionUI!), fixedPoints: this.WhenAnyValue(x => x.UseFixedNumberOfPoints), numberPointsPlotted: this.WhenAnyValue(x => x.NumberPointsPlotted)),
        isXAxisDateTime: true);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeDataLoggerPlotLinesWithPoints(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> observables) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new DataLoggerUI(plot: WpfPlot1vm!, observable: obs, color: SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeSignalPlotLines((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data) =>
        InitializeGenericPlotLines(
        data: [data],
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(WpfPlot1vm!, (data.Name, data.Value, data.DateTime, data.Axis), SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: true);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeLinesForSignalPoints((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data) =>
        InitializeGenericPlotLines(
        data: [data],
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(plot: WpfPlot1vm!, data: d, color: SetColorLegend(PlotLinesCollectionUI!), coordinatesObs: MouseCoordinatesObservable),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeLinesForSignalPoints(IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> data) =>
        InitializeGenericPlotLines(
        data: data,
        getYAxis: input => input.Axis,
        createPlotUI: d => new SignalXY_UI(plot: WpfPlot1vm!, data: d, color: SetColorLegend(PlotLinesCollectionUI!), coordinatesObs: MouseCoordinatesObservable),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes the lines for signal observables points.
    /// </summary>
    /// <param name="observables">The observables.</param>
    /// <param name="fs">The fs.</param>
    /// <param name="nSamples">The n samples.</param>
    public void InitializeLinesForSignalObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> observables, int fs, uint nSamples) =>
        InitializeGenericPlotLines(
        data: observables,
        getYAxis: input => input.Select(x => x.Axis),
        createPlotUI: obs => new StreamerUI(plot: WpfPlot1vm!, fs: fs, nSamples: nSamples, nPointsPlotted: NumberPointsPlotted, observable: obs, color: SetColorLegend(PlotLinesCollectionUI!)),
        isXAxisDateTime: false);

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
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
    /// Initializes the control menu.
    /// </summary>
    /// <param name="settings">The settings.</param>
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
    /// Initializes the control menu.
    /// </summary>
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

    private void Initializations2()
    {
        InitializeMouseObservable();
        WpfPlot1vm?.Refresh();
    }

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
