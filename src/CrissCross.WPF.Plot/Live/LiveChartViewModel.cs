// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using CP.Reactive;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Palettes;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// AICSLiveChart.
/// </summary>
public partial class LiveChartViewModel : RxObject
{
    private readonly ReactiveList<IYAxis> _yAxisList;
    private readonly IXAxis _xAxis1;
    private WpfPlot? _wpfLivePlot;

    /// <summary>
    /// Gets or sets a value indicating whether [enable marker].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable marker]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _enableMarker;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChartViewModel" /> class.
    /// </summary>
    /// <param name="grid">The grid.</param>
    public LiveChartViewModel(Grid grid)
    {
        // INITIALIZATION
        if (WpfLivePlot == null)
        {
            WpfLivePlot = new WpfPlot()
            {
                Margin = new(10, 0, 0, 0),
                Name = grid?.Name + "WpfPlot"
            };

            SetTheme();
            grid?.Children.Clear();
            grid?.Children.Add(WpfLivePlot);
        }

        // INITIALIZATION
        DataSignalUI = [];
        ControlMenu = [];

        _yAxisList = [];
        _xAxis1 = WpfLivePlot.Plot.Axes.AddBottomAxis();
        _xAxis1 = WpfLivePlot.Plot.Axes.DateTimeTicksBottom();
        AutoScale = ReactiveCommand.Create(() => { });
        GraphLocked = ReactiveCommand.Create(() => { });
        EnableMarkerBtn = ReactiveCommand.Create(() => { });

        AxesSetup(); // axes colors setup

        Initializations2();
    }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<SignalUI> DataSignalUI { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<Settings> ControlMenu { get; }

    /// <summary>
    /// Gets or sets the WPF plot.
    /// </summary>
    /// <value>
    /// The WPF plot.
    /// </value>
    public WpfPlot? WpfLivePlot
    {
        get => _wpfLivePlot;
        set => this.RaiseAndSetIfChanged(ref _wpfLivePlot, value);
    }

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
    /// Clears the content.
    /// </summary>
    public void ClearContent()
    {
        WpfLivePlot?.Plot.Clear();

        // SIGNAL
        if (DataSignalUI?.Count > 0)
        {
            foreach (var element in DataSignalUI)
            {
                element.Dispose();
            }

            DataSignalUI.Clear();
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="observables">The observables.</param>
    public void InitializeSignalPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> observables)
    {
        ClearContent();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        if (observables == null)
        {
            return;
        }

        var i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)

        // create new signals
        foreach (var observable in observables)
        {
            if (i < 9)
            {
                var newMyItem = new SignalUI(WpfLivePlot!, observable, GetLegendColor(DataSignalUI!));
                observable.Select(x => x.Axis)
                    .DistinctUntilChanged()
                    .Subscribe(a =>
                    {
                        for (var i = 0; i < _yAxisList.Count; i++)
                        {
                            _yAxisList[i].IsVisible = _yAxisList[i].IsVisible || a == i;
                        }

                        newMyItem.SignalXY!.Axes.YAxis = _yAxisList[a];
                    })
                    .DisposeWith(Disposables);
                newMyItem.SignalXY!.Axes.XAxis = _xAxis1;
                i++;
                DataSignalUI!.Add(newMyItem);
            }
        }
    }

    /// <summary>
    /// Initializes the plot lines.
    /// </summary>
    /// <param name="data">The data.</param>
    public void InitializeSignalPlotLines((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data)
    {
        ClearContent();

        // reset the axis
        foreach (var axis in _yAxisList)
        {
            axis.IsVisible = false;
        }

        var newMyItem = new SignalUI(WpfLivePlot!, (data.Name, data.Value, data.DateTime, data.Axis), GetLegendColor(DataSignalUI!));
        newMyItem.SignalXY!.Axes.YAxis = _yAxisList[data.Axis];

        for (var j = 0; j < _yAxisList.Count; j++)
        {
            _yAxisList[j].IsVisible = _yAxisList[j].IsVisible || data.Axis == j;
        }

        DataSignalUI!.Add(newMyItem);
    }

    /// <summary>
    /// Setups the _plotAutoScaled.
    /// </summary>
    public void ManualScaleY()
    {
        WpfLivePlot?.Plot.Axes.SetLimitsY(0, 100, _yAxisList[0]);
        WpfLivePlot?.Plot.Axes.SetLimitsY(0, 1000, _yAxisList[1]);
        WpfLivePlot?.Plot.Axes.SetLimitsY(0, 200, _yAxisList[2]);
        WpfLivePlot?.Refresh();
    }

    private static string GetLegendColor(ReactiveList<StreamerUI> legend)
    {
        List<string> colors =
                    [
                        "#377eb8", //// blue
                        "#4daf4a", //// green
                        "#984ea3", //// violet
                        "#ff7f00", //// orange
                        "#ffff33", //// yellow
                        "#a65628", //// brown
                        "#999999", //// grey
                        "#377eb8" //// light blue
                    ];

        // add data
        var n = legend!.Items.Count % colors.Count;
        return colors[n];
    }

    private static string GetLegendColor(ReactiveList<SignalUI> legend)
    {
        List<string> colors =
                    [
                        "#377eb8", //// blue
                        "#4daf4a", //// green
                        "#984ea3", //// violet
                        "#ff7f00", //// orange
                        "#ffff33", //// yellow
                        "#a65628", //// brown
                        "#999999", //// grey
                        "#377eb8" //// light blue
                    ];

        // add data
        var n = legend!.Items.Count % colors.Count;
        return colors[n];
    }

    private void SetTheme(string backgroundColorHex = "#252526", string axisColorHex = "#d7d7d7", string majorColorHex = "#404040")
    {
        var color = Color.FromHex(backgroundColorHex);
        var axisColor = Color.FromHex(axisColorHex);
        var majorColor = Color.FromHex(majorColorHex);
        WpfLivePlot!.Plot.Add.Palette = new Penumbra();
        WpfLivePlot.Plot.Axes.Color(axisColor);
        WpfLivePlot.Plot.Grid.MajorLineColor = majorColor;
        WpfLivePlot.Plot.FigureBackground.Color = color;
        WpfLivePlot.Plot.DataBackground.Color = color;
        WpfLivePlot.Plot.Legend.BackgroundColor = majorColor;
        WpfLivePlot.Plot.Legend.FontColor = axisColor;
        WpfLivePlot.Plot.Legend.OutlineColor = axisColor;
    }

    private void Initializations2()
    {
        InitializeDraggableAxisRules();
        InitializeControlMenu();
        WpfLivePlot?.Refresh();
    }

    private void InitializeDraggableAxisRules() =>
        WpfLivePlot.Events().MouseMove.Select(e => e.GetPosition(e.Device.Target))
            .CombineLatest(
            DataSignalUI.CurrentItems.Select(x =>
            {
                var l = new List<(SignalUI MyItem, (Crosshair? Crosshair, Marker? Marker, Text? Text))>();
                foreach (var d in x)
                {
                    l.Add((d, CreateCursorValues()));
                }

                return l;
            }),
            (e, x) => (e, x)).Subscribe(d =>
            {
                var mousePosition = d.e;
                var xx = Convert.ToSingle(mousePosition.X);
                var yy = Convert.ToSingle(mousePosition.Y);
                var rect = WpfLivePlot!.Plot.GetCoordinates(xx, yy);
                foreach (var x in d.x)
                {
                    WpfLivePlot!.Refresh();
                    var closestCoordinate = x.MyItem.SignalXY!.Data.GetNearestX(rect, WpfLivePlot!.Plot.LastRender).Coordinates;

                    // hide the crosshair, marker and text when no point is selected
                    var visible = x.MyItem.IsChecked && EnableMarker;
                    x.Item2.Crosshair!.IsVisible = visible;
                    x.Item2.Marker!.IsVisible = visible;
                    x.Item2.Text!.IsVisible = visible;

                    if (closestCoordinate != Coordinates.NaN)
                    {
                        x.Item2.Crosshair!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
                        x.Item2.Crosshair.Position = closestCoordinate;
                        x.Item2.Crosshair.LineColor = x.MyItem.SignalXY.Color;

                        x.Item2.Marker!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
                        x.Item2.Marker.Location = closestCoordinate;
                        x.Item2.Marker.MarkerStyle.LineColor = x.MyItem.SignalXY.Color;

                        x.Item2.Text!.Axes.YAxis = x.MyItem.SignalXY.Axes.YAxis;
                        x.Item2.Text.Location = closestCoordinate;
                        x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";
                        x.Item2.Text.LabelFontColor = x.MyItem.SignalXY.Color;
                    }

                    WpfLivePlot?.Refresh();
                }
            });

    private void InitializeControlMenu() => ControlMenu.Add(new Settings("Axis Rules"));

    private (Crosshair? Crosshair, Marker? Marker, Text? Text) CreateCursorValues()
    {
        // Create a crosshair to highlight the point under the cursor
        var crosshair = WpfLivePlot?.Plot.Add.Crosshair(0, 0);

        crosshair!.IsVisible = false;

        // Create a marker to highlight the point under the cursor
        var marker = WpfLivePlot?.Plot.Add.Marker(0, 0);
        marker!.Shape = MarkerShape.OpenCircle;
        marker.Size = 17;
        marker.LineWidth = 2;
        marker!.IsVisible = false;

        // Create a text label to place near the highlighted value
        var text = WpfLivePlot?.Plot.Add.Text(" ", 0, 0);
        text!.LabelAlignment = Alignment.LowerLeft;
        text.LabelBold = true;
        text.OffsetX = 7;
        text.OffsetY = -7;
        text!.IsVisible = false;

        return (Crosshair: crosshair, Marker: marker, Text: text);
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
            _yAxisList.Add(WpfLivePlot!.Plot.Axes.AddRightAxis());
            _yAxisList[i].FrameLineStyle.Color = baseColor;
            _yAxisList[i].TickLabelStyle.ForeColor = baseColor;
            _yAxisList[i].MajorTickStyle.Color = baseColor;
            _yAxisList[i].MinorTickStyle.Color = baseColor;
        }

        // Configure Axis Unit and Color
        _yAxisList[0].Label.Text = "[mm/s] & [g]";
        _yAxisList[0].Label.ForeColor = Color.FromHex("#377eb8");

        _yAxisList[1].Label.Text = "[um]";
        _yAxisList[1].Label.ForeColor = Color.FromHex("#4daf4a");

        _yAxisList[2].Label.Text = "[°C]";
        _yAxisList[2].Label.ForeColor = Color.FromHex("#984ea3");
    }

    private void HideLeftAxis(Color color)
    {
        var l = WpfLivePlot?.Plot.Axes.Left;
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
        var limits = now.Add(TimeSpan.FromMinutes(-60));
        var doublelimits = limits.ToOADate();
        WpfLivePlot?.Plot.Axes.SetLimitsX(doublelimits, doublenow, _xAxis1);
        WpfLivePlot?.Refresh();
    }
}
