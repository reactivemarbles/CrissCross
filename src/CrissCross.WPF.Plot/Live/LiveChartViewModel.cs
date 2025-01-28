// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using CP.Reactive;
using DynamicData;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Palettes;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// AICSLiveChart.
/// </summary>
public partial class LiveChartViewModel : RxObject
{
    private readonly ReactiveList<IYAxis> _yAxisList;
    private IXAxis _xAxis1;
    private WpfPlot? _wpfPlot1;
    [Reactive]
    private Settings? _selectedSetting;
    [Reactive]
    private Visibility _rightPropertyVisibility = Visibility.Collapsed;

    /// <summary>
    /// Gets or sets a value indicating whether [enable marker].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable marker]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _crossHairEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChartViewModel" /> class.
    /// </summary>
    /// <param name="grid">The grid.</param>
    public LiveChartViewModel(Grid grid)
    {
        // INITIALIZATION
        if (WpfPlot1vm == null)
        {
            WpfPlot1vm = new WpfPlot()
            {
                ////Margin = new(10, 0, 0, 0),
                Name = grid?.Name + "WpfPlot"
            };

            UseDarkMode();
            grid?.Children.Clear();
            grid?.Children.Add(WpfPlot1vm);
        }

        // INITIALIZATION
        ////DataUI = [];
        ScatterCollectionUI = [];
        SignalCollectionUI = [];
        DataLoggerCollectionUI = [];
        ControlMenu = [];
        AxisLinesUI = [];
        LeftPanelVisibility = Visibility.Visible;
        MouseCoordinatesObservable = new Subject<Coordinates>();

        _yAxisList = [];
        _xAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        CreateAxisWithTimeStamp();
        AutoScale = ReactiveCommand.Create(() => { });
        GraphLocked = ReactiveCommand.Create(() => { });
        EnableMarkerBtn = ReactiveCommand.Create(() => { });
        MakeLeftPanelVisible = ReactiveCommand.Create(() => { });

        AxesSetup(); // axes colors setup

        Initializations2();

        LinePropCommand = ReactiveCommand.Create(() =>
        {
            RightPropertyVisibility = Visibility.Collapsed;
        }).DisposeWith(Disposables);
    }

    /////// <summary>
    /////// Gets data plot.
    /////// </summary>
    ////public ReactiveList<StreamerUI> DataUI { get; }

    /// <summary>
    /// Gets or sets data plot.
    /// </summary>
    public ReactiveList<IYAxis> Axes { get; set; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<SignalUI> SignalCollectionUI { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<DataLoggerUI> DataLoggerCollectionUI { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<ScatterUI> ScatterCollectionUI { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<Settings> ControlMenu { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<AxisLinesUI> AxisLinesUI { get; }

    /// <summary>
    /// Gets or sets the WPF plot.
    /// </summary>
    /// <value>
    /// The WPF plot.
    /// </value>
    public WpfPlot? WpfPlot1vm
    {
        get => _wpfPlot1;
        set => this.RaiseAndSetIfChanged(ref _wpfPlot1, value);
    }

    /// <summary>
    /// Gets or sets the mouse coordinates observable.
    /// </summary>
    /// <value>
    /// The mouse coordinates observable.
    /// </value>
    public ISubject<Coordinates> MouseCoordinatesObservable { get; set;  }

    /// <summary>
    /// Gets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public ReactiveCommand<Unit, Unit>? GraphLocked { get; }

    /// <summary>
    /// Gets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public ReactiveCommand<Unit, Unit>? EnableMarkerBtn { get; }

    /// <summary>
    /// Gets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public ReactiveCommand<Unit, Unit>? AutoScale { get; }

    /// <summary>
    /// Gets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public ReactiveCommand<Unit, Unit>? MakeLeftPanelVisible { get; }

    /// <summary>
    /// Gets or sets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public Visibility LeftPanelVisibility { get; set; }

    /// <summary>
    /// Gets the line property command.
    /// </summary>
    /// <value>
    /// The line property command.
    /// </value>
    public ReactiveCommand<Unit, Unit> LinePropCommand { get; }

    /// <summary>
    /// Creates the axis with data stamp.
    /// </summary>
    public void CreateAxisWithTimeStamp()
    {
        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Bottom);
        _xAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        _xAxis1 = WpfPlot1vm!.Plot.Axes.DateTimeTicksBottom();
        SetXAxisColour();
    }

    /// <summary>
    /// Creates the axis with data stamp.
    /// </summary>
    public void CreateAxisWithPoints()
    {
        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Bottom);
        _xAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        SetXAxisColour();
    }

    /// <summary>
    /// Sets the x axis colour.
    /// </summary>
    public void SetXAxisColour()
    {
        var baseColor = Color.FromHex("#D0D0D0");

        // colors
        _xAxis1.Label.ForeColor = Color.FromHex("#377eb8");
        _xAxis1.FrameLineStyle.Color = baseColor;
        _xAxis1.TickLabelStyle.ForeColor = baseColor;
        _xAxis1.MajorTickStyle.Color = baseColor;
        _xAxis1.MinorTickStyle.Color = baseColor;
    }

    /// <summary>
    /// Clears the content.
    /// </summary>
    public void ClearContent()
    {
        WpfPlot1vm?.Plot.Clear();

        ////// STREAMER
        ////if (DataUI?.Count > 0)
        ////{
        ////    foreach (var element in DataUI)
        ////    {
        ////        element.Dispose();
        ////        element.Streamer?.Data.Clear();
        ////    }

        ////    DataUI.Clear();
        ////}

        // SIGNAL
        if (SignalCollectionUI?.Count > 0)
        {
            foreach (var element in SignalCollectionUI)
            {
                element.Dispose();
            }

            SignalCollectionUI.Clear();
        }

        // SIGNAL
        if (ScatterCollectionUI?.Count > 0)
        {
            foreach (var element in ScatterCollectionUI)
            {
                element.Dispose();
            }

            ScatterCollectionUI.Clear();
        }

        // DATA LOGGER
        if (DataLoggerCollectionUI?.Count > 0)
        {
            foreach (var element in DataLoggerCollectionUI)
            {
                element.Dispose();
                element.ChartSettings.Dispose();
            }

            DataLoggerCollectionUI.Clear();
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
    }

    /////// <summary>
    /////// Initializes the plot lines.
    /////// </summary>
    /////// <param name="observables">The observables.</param>
    ////public void InitializeStreamerPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables)
    ////{
    ////    //// TODO: how do if know the Streamer YAxis?
    ////    int i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

    ////    ClearContent();

    ////    WpfPlot1vm?.Plot.Clear();

    ////    if (DataUI?.Count > 0)
    ////    {
    ////        foreach (var element in DataUI)
    ////        {
    ////            element.Dispose();
    ////            element.Streamer!.Data.Clear();
    ////        }

    ////        DataUI.Clear();
    ////    }

    ////    // reset the axis
    ////    foreach (var axis in _yAxisList)
    ////    {
    ////        axis.IsVisible = false;
    ////    }

    ////    // create new streamers
    ////    foreach (var observable in observables)
    ////    {
    ////        if (i < 9)
    ////        {
    ////            var newMyItem = new StreamerUI(WpfPlot1vm!, observable, SetColorLegend(DataUI!));
    ////            observable.Select(x => x.Axis)
    ////                .DistinctUntilChanged()
    ////                .Subscribe(a =>
    ////                {
    ////                    for (int i = 0; i < _yAxisList.Count; i++)
    ////                    {
    ////                        _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
    ////                    }

    ////                    newMyItem.Streamer!.Axes.YAxis = _yAxisList[a];
    ////                })
    ////                .DisposeWith(Disposables);
    ////            newMyItem.Streamer!.Axes.XAxis = _xAxis1;
    ////            i++;
    ////            DataUI!.Add(newMyItem);
    ////        }
    ////    }
    ////}

    /// <summary>
    /// Initializes the axes.
    /// </summary>
    /// <param name="data">The data contains : axes names and colors.</param>
    public void YAxesSetup((IList<string> yNames, IList<string> hexColors) data)
    {
        var baseColor = Color.FromHex("#D0D0D0");

        // hide left axis always, i just want right ones
        HideLeftAxis(baseColor);

        // remove axes from graph and from the list
        foreach (var axis in _yAxisList)
        {
            WpfPlot1vm?.Plot.Axes.Remove(axis);
        }

        _yAxisList.RemoveMany(_yAxisList.Items);

        // combine yName and hexColors lists
        var combinedList = data.yNames.Zip(data.hexColors, (name, color) => (name: name, color: color));

        // create the axes
        foreach (var item in combinedList)
        {
            _yAxisList.Add(WpfPlot1vm!.Plot.Axes.AddRightAxis());
            _yAxisList.Last().FrameLineStyle.Color = baseColor;
            _yAxisList.Last().TickLabelStyle.ForeColor = baseColor;
            _yAxisList.Last().MajorTickStyle.Color = baseColor;
            _yAxisList.Last().MinorTickStyle.Color = baseColor;
            _yAxisList.Last().Label.Text = item.name;
            _yAxisList.Last().Label.ForeColor = Color.FromHex(item.color);
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeScatterPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables)
    {
        //// TODO: how do if know the Streamer YAxis?
        var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();
        CreateAxisWithPoints();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        // create new signals
        foreach (var (observable, newMyItem) in
        from observable in observables// the lines plotted are limited to 9
        where i < 9
        let newMyItem = new ScatterUI(WpfPlot1vm!, observable, SetColorLegend(ScatterCollectionUI!))
        select (observable, newMyItem))
        {
            observable.Select(x => x.Axis)
                    .DistinctUntilChanged()
                    .Subscribe(a =>
                    {
                        for (var i = 0; i < _yAxisList.Count; i++)
                        {
                            _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
                        }

                        newMyItem.Scatter!.Axes.YAxis = _yAxisList[a];
                    })
                    .DisposeWith(Disposables);
            newMyItem.Scatter!.Axes.XAxis = _xAxis1;
            i++;
            ScatterCollectionUI!.Add(newMyItem);
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeLinesForScatterObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> observables)
    {
        //// TODO: how do if know the Streamer YAxis?
        var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();
        CreateAxisWithPoints();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        // create new signals
        foreach (var (observable, newMyItem) in
        from observable in observables// the lines plotted are limited to 9
        where i < 9
        let newMyItem = new ScatterUI(WpfPlot1vm!, observable, SetColorLegend(ScatterCollectionUI!))
        select (observable, newMyItem))
        {
            observable.Select(x => x.Axis)
                    .DistinctUntilChanged()
                    .Subscribe(a =>
                    {
                        for (var i = 0; i < _yAxisList.Count; i++)
                        {
                            _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
                        }

                        newMyItem.Scatter!.Axes.YAxis = _yAxisList[a];
                    })
                    .DisposeWith(Disposables);
            newMyItem.Scatter!.Axes.XAxis = _xAxis1;
            i++;
            ScatterCollectionUI!.Add(newMyItem);
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeSignalPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables)
    {
        //// TODO: how do i know the Streamer YAxis?
        var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        // create new signals
        foreach (var (observable, newMyItem) in
        from observable in observables// the lines plotted are limited to 9
        where i < 9
        let newMyItem = new SignalUI(WpfPlot1vm!, observable: observable, coordinatesObs: MouseCoordinatesObservable, SetColorLegend(SignalCollectionUI!))
        select (observable, newMyItem))
        {
            observable.Select(x => x.Axis)
                    .DistinctUntilChanged()
                    .Subscribe(a =>
                    {
                        for (var i = 0; i < _yAxisList.Count; i++)
                        {
                            _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
                        }

                        newMyItem.DataLogger!.Axes.YAxis = _yAxisList[a];
                        ////newMyItem.SignalXY!.Axes.YAxis = _yAxisList[a];
                    })
                    .DisposeWith(Disposables);
            newMyItem.DataLogger!.Axes.XAxis = _xAxis1;
            ////newMyItem.SignalXY!.Axes.XAxis = _xAxis1;
            i++;
            SignalCollectionUI!.Add(newMyItem);
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeDataLoggerPlotLinesWithPoints(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> observables)
    {
        //// TODO: how do if know the Streamer YAxis?
        var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();
        CreateAxisWithPoints();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        // create new signals
        foreach (var (observable, newMyItem) in
        from observable in observables// the lines plotted are limited to 9
        where i < 9
        let newMyItem = new DataLoggerUI(WpfPlot1vm!, observable, SetColorLegend(DataLoggerCollectionUI!))
        select (observable, newMyItem))
        {
            observable.Select(x => x.Axis)
                    .DistinctUntilChanged()
                    .Subscribe(a =>
                    {
                        for (var i = 0; i < _yAxisList.Count; i++)
                        {
                            _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
                        }

                        newMyItem.DataLogger!.Axes.YAxis = _yAxisList[a];
                        ////newMyItem.SignalXY!.Axes.YAxis = _yAxisList[a];
                    })
                    .DisposeWith(Disposables);
            newMyItem.DataLogger!.Axes.XAxis = _xAxis1;
            ////newMyItem.SignalXY!.Axes.XAxis = _xAxis1;
            i++;
            DataLoggerCollectionUI!.Add(newMyItem);
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeSignalPlotLines((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data)
    {
        //// TODO: how do if know the Streamer YAxis?
        ////var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        var newMyItem = new SignalUI(WpfPlot1vm!, (data.Name, data.Value, data.DateTime, data.Axis), SetColorLegend(SignalCollectionUI!));
        newMyItem.SignalXY!.Axes.YAxis = _yAxisList[data.Axis];

        for (var j = 0; j < _yAxisList.Count; j++)
        {
            _yAxisList[j].IsVisible = _yAxisList[j].IsVisible || data.Axis == j;
        }

        SignalCollectionUI!.Add(newMyItem);
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeLinesForSignalPoints((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data)
    {
        //// TODO: how do i know the Streamer YAxis?
        ////var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();
        CreateAxisWithPoints();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        var newMyItem = new SignalUI(WpfPlot1vm!, (data.Name, data.Value, data.DateTime, data.Axis), SetColorLegend(SignalCollectionUI!));
        newMyItem.SignalXY!.Axes.YAxis = _yAxisList[data.Axis];
        newMyItem.SignalXY.MarkerSize = 2;

        for (var j = 0; j < _yAxisList.Count; j++)
        {
            _yAxisList[j].IsVisible = _yAxisList[j].IsVisible || data.Axis == j;
        }

        SignalCollectionUI!.Add(newMyItem);
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The data.</param>
    public void InitializeLinesForSignalObservablesPoints(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> observables)
    {
        //// TODO: need to be finished and tested
        var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        ClearContent();
        CreateAxisWithPoints();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        // create new signals
        foreach (var (observable, newMyItem) in
        from observable in observables// the lines plotted are limited to 9
        where i < 9
        let newMyItem = new SignalUI(WpfPlot1vm!, observable: observable, coordinatesObs: MouseCoordinatesObservable, SetColorLegend(SignalCollectionUI!), fixedPoints: true)
        select (observable, newMyItem))
        {
            observable.Select(x => x.Axis)
                    .DistinctUntilChanged()
                    .Subscribe(a =>
                    {
                        for (var i = 0; i < _yAxisList.Count; i++)
                        {
                            _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
                        }

                        newMyItem.Streamer!.Axes.YAxis = _yAxisList[a];
                    })
                    .DisposeWith(Disposables);
            newMyItem.Streamer!.Axes.XAxis = _xAxis1;
            i++;
            SignalCollectionUI!.Add(newMyItem);
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeLinesForScatterPoints((string? Name, IList<double>? X, IList<double> Y, int Axis) data)
    {
        //// TODO: how do i know the Streamer YAxis?

        ClearContent();
        CreateAxisWithPoints();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        var newMyItem = new ScatterUI(WpfPlot1vm!, (data.Name, data.X, data.Y, data.Axis), SetColorLegend(ScatterCollectionUI!));
        newMyItem.Scatter!.Axes.YAxis = _yAxisList[data.Axis];
        ////newMyItem.Scatter!.LineStyle.IsVisible = false;
        newMyItem.Scatter!.MarkerSize = 1f;
        newMyItem.Scatter!.LineWidth = 0.3f;

        for (var j = 0; j < _yAxisList.Count; j++)
        {
            _yAxisList[j].IsVisible = _yAxisList[j].IsVisible || data.Axis == j;
        }

        ScatterCollectionUI!.Add(newMyItem);
    }

    /// <summary>
    /// Setups the _plotAutoScaled.
    /// </summary>
    public void ManualScaleY()
    {
        foreach (var axis in _yAxisList)
        {
            WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 100, axis);
        }
        ////WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 100, _yAxisList[0]);
        ////WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 1000, _yAxisList[1]);
        ////WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 200, _yAxisList[2]);
        WpfPlot1vm?.Refresh();
    }

    /// <summary>
    /// Initializes the control menu.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void InitializeControlMenu(IList<Settings>? settings)
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
            item.AxisLine!.Axes.YAxis = _yAxisList[item.Axis];
            item.AxisLine!.Axes.XAxis = _xAxis1;
        }
    }

    /// <summary>
    /// Clears the axis lines.
    /// </summary>
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

    private static string SetColorLegend<T>(IList<T> legend)
    {
        List<string> colors =
                    [
                        "Blue", // "#377eb8", //// blue
                        "Green", // "#4daf4a", //// green
                        "Violet", // "#984ea3", //// violet
                        "Orange", // "#ff7f00", //// orange
                        "Yellow", // #ffff33", //// yellow
                        "Brown", // #a65628", //// brown
                        "Grey", // #999999", //// grey
                        "Light Blue", // #377eb8" //// light blue
                    ];

        // add data
        var n = legend!.Count % colors.Count;
        return colors[n];
    }

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

    private void Initializations2()
    {
        ////InitializeDraggableAxisRules();
        InitializeMouseObservable();

        // InitializeControlMenu();
        WpfPlot1vm?.Refresh();
    }

    ////private void InitializeDraggableAxisRules()
    ////{
    ////    // MOUSE EVENT
    ////    WpfPlot1vm.Events().MouseMove.Select(e => e.GetPosition(e.Device.Target))
    ////        .CombineLatest(
    ////        DataUI.CurrentItems.Select(x =>
    ////        {
    ////            var l = new List<(StreamerUI MyItem, (Crosshair? Crosshair, Marker? Marker, Text? Text))>();
    ////            foreach (var d in x)
    ////            {
    ////                l.Add((d, CreateCursorValues()));
    ////            }

    ////            return l;
    ////        }),
    ////        (e, x) => (e, x)).Subscribe(d =>
    ////        {
    ////            Point mousePosition = d.e;
    ////            float xx = Convert.ToSingle(mousePosition.X);
    ////            float yy = Convert.ToSingle(mousePosition.Y);
    ////            var rect = WpfPlot1vm?.Plot.GetCoordinates(xx, yy);
    ////            foreach (var x in d.x)
    ////            {
    ////                var closestCoordinate = x.MyItem.Streamer!.Data.Coordinates
    ////                .OrderBy(coordinate => Math.Abs(coordinate.X - rect!.Value.X))
    ////                .FirstOrDefault();

    ////                // hide the crosshair, marker and text when no point is selected
    ////                var visible = x.MyItem.IsChecked && EnableMarker;

    ////                x.Item2.Crosshair!.Axes.YAxis = x.MyItem.Streamer.Axes.YAxis;
    ////                x.Item2.Crosshair.IsVisible = visible;
    ////                x.Item2.Crosshair.Position = closestCoordinate;
    ////                x.Item2.Crosshair.LineColor = x.MyItem.Streamer.Color;

    ////                x.Item2.Marker!.Axes.YAxis = x.MyItem.Streamer.Axes.YAxis;
    ////                x.Item2.Marker.IsVisible = visible;
    ////                x.Item2.Marker.Location = closestCoordinate;
    ////                x.Item2.Marker.MarkerStyle.LineColor = x.MyItem.Streamer.Color;

    ////                x.Item2.Text!.Axes.YAxis = x.MyItem.Streamer.Axes.YAxis;
    ////                x.Item2.Text.IsVisible = visible;
    ////                x.Item2.Text.Location = closestCoordinate;
    ////                x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";
    ////                x.Item2.Text.LabelFontColor = x.MyItem.Streamer.Color;

    ////                WpfPlot1vm?.Refresh();
    ////            }
    ////        });
    ////}

    private void InitializeMouseObservable()
    {
        // MOUSE EVENT
        WpfPlot1vm!.MouseMove += (s, e) =>
        {
            var position = e.GetPosition(e.Device.Target);

            //// determine where the mouse is and send the coordinates
            Pixel mousePixel = new(position.X, position.Y);
            var mouseLocation = WpfPlot1vm.Plot.GetCoordinates(mousePixel);
            try
            {
                MouseCoordinatesObservable.OnNext(mouseLocation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            ////foreach (var x in SignalCollectionUI)
            ////{
            ////    WpfPlot1vm!.Refresh();
            ////    var closestCoordinate = x.MyItem.SignalXY!.Data.GetNearestX(rect, WpfPlot1vm!.Plot.LastRender).Coordinates;
            ////    ////.OrderBy(coordinate => Math.Abs(coordinate.X - rect!.Value.X))
            ////    ////.FirstOrDefault();

            ////    // hide the crosshair, marker and text when no point is selected
            ////    var visible = x.MyItem.IsChecked && EnableMarker;
            ////    x.Item2.Crosshair!.IsVisible = visible;
            ////    x.Item2.Marker!.IsVisible = visible;
            ////    x.Item2.Text!.IsVisible = visible;

            ////    if (closestCoordinate != Coordinates.NaN)
            ////    {
            ////        x.Item2.Crosshair!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
            ////        x.Item2.Crosshair.Position = closestCoordinate;
            ////        x.Item2.Crosshair.LineColor = x.MyItem.SignalXY.Color;

            ////        x.Item2.Marker!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
            ////        x.Item2.Marker.Location = closestCoordinate;
            ////        x.Item2.Marker.MarkerStyle.LineColor = x.MyItem.SignalXY.Color;

            ////        x.Item2.Text!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
            ////        x.Item2.Text.Location = closestCoordinate;
            ////        try
            ////        {
            ////            x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";
            ////        }
            ////        catch
            ////        {
            ////            x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{closestCoordinate.X}";
            ////        }

            ////        x.Item2.Text.LabelFontColor = x.MyItem.SignalXY.Color;
            ////    }

            ////    WpfPlot1vm?.Refresh();
            ////}
        };
        ////WpfPlot1vm.Events().MouseMove.Select(e => e.GetPosition(e.Device.Target))
        ////    .CombineLatest(
        ////    SignalCollectionUI.CurrentItems.Select(x =>
        ////    {
        ////        var l = new List<(SignalUI MyItem, (Crosshair? Crosshair, Marker? Marker, Text? Text))>();
        ////        foreach (var d in x)
        ////        {
        ////            l.Add((d, CreateCursorValues()));
        ////        }

        ////        return l;
        ////    }),
        ////    (e, x) => (e, x)).Subscribe(d =>
        ////    {
        ////        Point mousePosition = d.e;
        ////        float xx = Convert.ToSingle(mousePosition.X);
        ////        float yy = Convert.ToSingle(mousePosition.Y);
        ////        var rect = WpfPlot1vm!.Plot.GetCoordinates(xx, yy);
        ////        foreach (var x in d.x)
        ////        {
        ////            WpfPlot1vm!.Refresh();
        ////            var closestCoordinate = x.MyItem.SignalXY!.Data.GetNearestX(rect, WpfPlot1vm!.Plot.LastRender).Coordinates;
        ////            ////.OrderBy(coordinate => Math.Abs(coordinate.X - rect!.Value.X))
        ////            ////.FirstOrDefault();

        ////            // hide the crosshair, marker and text when no point is selected
        ////            var visible = x.MyItem.IsChecked && EnableMarker;
        ////            x.Item2.Crosshair!.IsVisible = visible;
        ////            x.Item2.Marker!.IsVisible = visible;
        ////            x.Item2.Text!.IsVisible = visible;

        ////            if (closestCoordinate != Coordinates.NaN)
        ////            {
        ////                x.Item2.Crosshair!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
        ////                x.Item2.Crosshair.Position = closestCoordinate;
        ////                x.Item2.Crosshair.LineColor = x.MyItem.SignalXY.Color;

        ////                x.Item2.Marker!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
        ////                x.Item2.Marker.Location = closestCoordinate;
        ////                x.Item2.Marker.MarkerStyle.LineColor = x.MyItem.SignalXY.Color;

        ////                x.Item2.Text!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
        ////                x.Item2.Text.Location = closestCoordinate;
        ////                try
        ////                {
        ////                    x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";
        ////                }
        ////                catch
        ////                {
        ////                    x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{closestCoordinate.X}";
        ////                }

        ////                x.Item2.Text.LabelFontColor = x.MyItem.SignalXY.Color;
        ////            }

        ////            WpfPlot1vm?.Refresh();
        ////        }
        ////    });

        ////// MOUSE EVENT
        ////WpfPlot1vm.Events().MouseMove.Select(e => e.GetPosition(e.Device.Target))
        ////    .CombineLatest(
        ////    ScatterCollectionUI.CurrentItems.Select(x =>
        ////    {
        ////        var l = new List<(ScatterUI MyItem, (Crosshair? Crosshair, Marker? Marker, Text? Text))>();
        ////        foreach (var d in x)
        ////        {
        ////            l.Add((d, CreateCursorValues()));
        ////        }

        ////        return l;
        ////    }),
        ////    (e, x) => (e, x)).Subscribe(d =>
        ////    {
        ////        Point mousePosition = d.e;
        ////        float xx = Convert.ToSingle(mousePosition.X);
        ////        float yy = Convert.ToSingle(mousePosition.Y);
        ////        var rect = WpfPlot1vm!.Plot.GetCoordinates(xx, yy);
        ////        foreach (var x in d.x)
        ////        {
        ////            WpfPlot1vm!.Refresh();
        ////            var closestCoordinate = x.MyItem.Scatter!.Data.GetNearestX(rect, WpfPlot1vm!.Plot.LastRender).Coordinates;
        ////            ////.OrderBy(coordinate => Math.Abs(coordinate.X - rect!.Value.X))
        ////            ////.FirstOrDefault();

        ////            // hide the crosshair, marker and text when no point is selected
        ////            var visible = x.MyItem.IsChecked && EnableMarker;
        ////            x.Item2.Crosshair!.IsVisible = visible;
        ////            x.Item2.Marker!.IsVisible = visible;
        ////            x.Item2.Text!.IsVisible = visible;

        ////            if (closestCoordinate != Coordinates.NaN)
        ////            {
        ////                x.Item2.Crosshair!.Axes.YAxis = x.MyItem.Scatter.Axes.YAxis;
        ////                x.Item2.Crosshair.Position = closestCoordinate;
        ////                x.Item2.Crosshair.LineColor = x.MyItem.Scatter.Color;

        ////                x.Item2.Marker!.Axes.YAxis = x.MyItem.Scatter.Axes.YAxis;
        ////                x.Item2.Marker.Location = closestCoordinate;
        ////                x.Item2.Marker.MarkerStyle.LineColor = x.MyItem.Scatter.Color;

        ////                x.Item2.Text!.Axes.YAxis = x.MyItem.Scatter.Axes.YAxis;
        ////                x.Item2.Text.Location = closestCoordinate;

        ////                x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{closestCoordinate.X}";

        ////                x.Item2.Text.LabelFontColor = x.MyItem.Scatter.Color;
        ////            }

        ////            WpfPlot1vm?.Refresh();
        ////        }
        ////    });
    }

    /// <summary>
    /// Setup the axes.
    /// </summary>
    private void AxesSetup()
    {
        var baseColor = Color.FromHex("#D0D0D0");

        HideLeftAxis(baseColor);

        // colors
        _xAxis1.Label.ForeColor = Color.FromHex("#377eb8");
        _xAxis1.FrameLineStyle.Color = baseColor;
        _xAxis1.TickLabelStyle.ForeColor = baseColor;
        _xAxis1.MajorTickStyle.Color = baseColor;
        _xAxis1.MinorTickStyle.Color = baseColor;

        // Setup Y Axis
        for (var i = 0; i < 3; i++)
        {
            _yAxisList.Add(WpfPlot1vm!.Plot.Axes.AddRightAxis());
            _yAxisList[i].FrameLineStyle.Color = baseColor;
            _yAxisList[i].TickLabelStyle.ForeColor = baseColor;
            _yAxisList[i].MajorTickStyle.Color = baseColor;
            _yAxisList[i].MinorTickStyle.Color = baseColor;
        }

        // Configure Axis Unit and Color
        _yAxisList[0].Label.Text = "Phase [ ° ]";
        _yAxisList[0].Label.ForeColor = Color.FromHex("#377eb8");

        _yAxisList[1].Label.Text = "[um]";
        _yAxisList[1].Label.ForeColor = Color.FromHex("#4daf4a");

        _yAxisList[2].Label.Text = "[°C]";
        _yAxisList[2].Label.ForeColor = Color.FromHex("#984ea3");
    }

    private void HideLeftAxis(Color color)
    {
        ////WpfPlot1vm?.Plot.Axes.AddLeftAxis();
        ////WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Left);
        var l = WpfPlot1vm?.Plot.Axes.Left;
        var backColour = Color.FromHex("#252526");
        l!.Label.ForeColor = backColour;
        l.FrameLineStyle.Color = color;
        l.TickLabelStyle.ForeColor = backColour;
        l.MajorTickStyle.Color = backColour;
        l.MinorTickStyle.Color = backColour;
    }

    /// <summary>
    /// Scale manually axe X.
    /// </summary>
    private void ManualScaleX()
    {
        var now = DateTime.Now;
        var doublenow = now.ToOADate();
        ////DateTime limits = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute - 1, now.Second);
        var limits = now.Add(TimeSpan.FromMinutes(-60));
        var doublelimits = limits.ToOADate();
        WpfPlot1vm?.Plot.Axes.SetLimitsX(doublelimits, doublenow, _xAxis1);
        WpfPlot1vm?.Refresh();
    }
}
