// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

////using System.Reactive;
////using System.Reactive.Disposables;
////using System.Reactive.Linq;
////using System.Windows;
////using System.Windows.Controls;
////using CP.Reactive;
////using ReactiveMarbles.ObservableEvents;
////using ReactiveUI;
////using ReactiveUI.Fody.Helpers;
////using ScottPlot;
////using ScottPlot.Plottables;
////using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// AICSLiveChart.
/// </summary>
public class ChartSettingsViewModel : RxObject
{
    ////    private readonly ReactiveList<IYAxis> _yAxis1;
    ////    private readonly IYAxis _yAxisLeft;
    ////    private readonly IXAxis _xAxis1;
    ////    private WpfPlot? _wpfPlot1;

    ////    /// <summary>
    ////    /// Initializes a new instance of the <see cref="SPChartSettingsViewModel" /> class.
    ////    /// </summary>
    ////    /// <param name="grid">The grid.</param>
    ////    public SPChartSettingsViewModel(Grid grid)
    ////    {
    ////        // INITIALIZATION
    ////        if (WpfPlot1vm == null)
    ////        {
    ////            WpfPlot1vm = new WpfPlot()
    ////            {
    ////                Margin = new(10, 0, 0, 0),
    ////                Name = "WpfPlot1"
    ////            };

    ////            WpfPlot1vm.Plot.Style.DarkMode();
    ////            grid.Children.Clear();
    ////            grid.Children.Add(WpfPlot1vm);
    ////        }

    ////        // INITIALIZATION
    ////        DataUI = [];
    ////        ControlMenu = [];

    ////        _yAxis1 = [];
    ////        _yAxisLeft = WpfPlot1vm.Plot.Axes.AddLeftAxis();

    ////        _xAxis1 = WpfPlot1vm.Plot.Axes.AddBottomAxis();
    ////        _xAxis1 = WpfPlot1vm.Plot.Axes.DateTimeTicksBottom();
    ////        AutoScale = ReactiveCommand.Create(() => { });
    ////        GraphLocked = ReactiveCommand.Create(() => { });

    ////        AxesSetup(); // axes colors setup

    ////        Initializations2();
    ////    }

    ////    /// <summary>
    ////    /// Gets data plot.
    ////    /// </summary>
    ////    public ReactiveList<StreamerUI> DataUI { get; }

    ////    /// <summary>
    ////    /// Gets data plot.
    ////    /// </summary>
    ////    public ReactiveList<SPSettings> ControlMenu { get; }

    ////    /// <summary>
    ////    /// Gets or sets the WPF plot.
    ////    /// </summary>
    ////    /// <value>
    ////    /// The WPF plot.
    ////    /// </value>
    ////    public WpfPlot? WpfPlot1vm
    ////    {
    ////        get => _wpfPlot1;
    ////        set => this.RaiseAndSetIfChanged(ref _wpfPlot1, value);
    ////    }

    ////    /// <summary>
    ////    /// Gets the manage axis limits.
    ////    /// </summary>
    ////    /// <value>
    ////    /// The manage axis limits.
    ////    /// </value>
    ////    public ReactiveCommand<Unit, Unit>? GraphLocked { get; internal set; }

    ////    /// <summary>
    ////    /// Gets the manage axis limits.
    ////    /// </summary>
    ////    /// <value>
    ////    /// The manage axis limits.
    ////    /// </value>
    ////    public ReactiveCommand<Unit, Unit>? AutoScale { get; internal set; }

    ////    /// <summary>
    ////    /// Gets or sets a value indicating whether [enable marker].
    ////    /// </summary>
    ////    /// <value>
    ////    ///   <c>true</c> if [enable marker]; otherwise, <c>false</c>.
    ////    /// </value>
    ////    [Reactive]
    ////    public bool EnableMarker { get; set; }

    ////    /////// <summary>
    ////    /////// Initializes the plot lines.
    ////    /////// </summary>
    ////    /////// <param name="observables">The observables.</param>
    ////    ////public void InitializeStreamerPlotLines(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis, bool[] visible)>> observables)
    ////    ////{
    ////    ////    //// TODO: how do if know the Streamer YAxis?
    ////    ////    int i = 0; // used temporarily for assign yAxis to the Streamer line (MyItem contained in DataUI)
    ////    ////    if (DataUI?.Count > 0)
    ////    ////    {
    ////    ////        foreach (var element in DataUI)
    ////    ////        {
    ////    ////            element.Dispose();
    ////    ////            element.Streamer.Data.Clear();
    ////    ////        }

    ////    ////        DataUI.Clear();
    ////    ////    }

    ////    ////    foreach (var observable in observables)
    ////    ////    {
    ////    ////        if (i < 3)
    ////    ////        {
    ////    ////            var newMyItem = new StreamerUI(WpfPlot1vm!, observable, SetColorLegend(DataUI!));
    ////    ////            observable.Select(x => x.Axis)
    ////    ////                .DistinctUntilChanged()
    ////    ////                .Subscribe(a => newMyItem.Streamer.Axes.YAxis = _yAxis1[a])
    ////    ////                .DisposeWith(Disposables);
    ////    ////            newMyItem.Streamer.Axes.XAxis = _xAxis1;
    ////    ////            i++;
    ////    ////            DataUI!.Add(newMyItem);
    ////    ////        }
    ////    ////    }
    ////    ////}

    ////    /// <summary>
    ////    /// Setups the _plotAutoScaled.
    ////    /// </summary>
    ////    public void ManualScaleY()
    ////    {
    ////        WpfPlot1vm?.Plot.Axes.SetLimitsY(-20, 20, _yAxis1[0]);
    ////        WpfPlot1vm?.Plot.Axes.SetLimitsY(-20_000, 20_000, _yAxis1[1]);
    ////        WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 50_000, _yAxis1[2]);
    ////        WpfPlot1vm?.Refresh();
    ////    }

    ////    private static string SetColorLegend(ReactiveList<StreamerUI> legend)
    ////    {
    ////        List<string> colors =
    ////                    [
    ////                        "#377eb8", //// blue
    ////                        "#4daf4a", //// green
    ////                        "#984ea3", ////
    ////                        "#ff7f00", ////
    ////                        "#ffff33", ////
    ////                        "#a65628", ////
    ////                        "#999999", ////
    ////                        "#377eb8" ////
    ////                    ];

    ////        // add data
    ////        var n = legend!.Items.Count % colors.Count;
    ////        return colors[n];
    ////    }

    ////    private void Initializations2()
    ////    {
    ////        InitializeDraggableAxisRules();
    ////        InitializeControlMenu();
    ////        WpfPlot1vm?.Refresh();
    ////    }

    ////    private void InitializeDraggableAxisRules()
    ////    {
    ////        // MOUSE EVENT
    ////        WpfPlot1vm.Events().MouseMove.Select(e => e.GetPosition(e.Device.Target))
    ////            .CombineLatest(
    ////            DataUI.CurrentItems.Select(x =>
    ////            {
    ////                var l = new List<(StreamerUI MyItem, (Crosshair? Crosshair, Marker? Marker, Text? Text))>();
    ////                foreach (var d in x)
    ////                {
    ////                    l.Add((d, CreateCursorValues()));
    ////                }

    ////                return l;
    ////            }),
    ////            (e, x) => (e, x)).Subscribe(d =>
    ////            {
    ////                Point mousePosition = d.e;
    ////                float xx = Convert.ToSingle(mousePosition.X);
    ////                float yy = Convert.ToSingle(mousePosition.Y);
    ////                var rect = WpfPlot1vm?.Plot.GetCoordinates(xx, yy);
    ////                foreach (var x in d.x)
    ////                {
    ////                    var closestCoordinate = x.MyItem.Streamer.Data.Coordinates
    ////                    .OrderBy(coordinate => Math.Abs(coordinate.X - rect!.Value.X)) ////(useMouse ? rect.X : _vl.X)))
    ////                    .FirstOrDefault();

    ////                    // hide the crosshair, marker and text when no point is selected
    ////                    var visible = x.MyItem.IsChecked && EnableMarker;

    ////                    x.Item2.Crosshair!.Axes.YAxis = x.MyItem.Streamer.Axes.YAxis;
    ////                    x.Item2.Crosshair.IsVisible = visible;
    ////                    x.Item2.Crosshair.Position = closestCoordinate;
    ////                    x.Item2.Crosshair.LineColor = x.MyItem.Streamer.Color;

    ////                    x.Item2.Marker!.Axes.YAxis = x.MyItem.Streamer.Axes.YAxis;
    ////                    x.Item2.Marker.IsVisible = visible;
    ////                    x.Item2.Marker.Location = closestCoordinate;
    ////                    x.Item2.Marker.MarkerStyle.LineColor = x.MyItem.Streamer.Color;

    ////                    x.Item2.Text!.Axes.YAxis = x.MyItem.Streamer.Axes.YAxis;
    ////                    x.Item2.Text.IsVisible = visible;
    ////                    x.Item2.Text.Location = closestCoordinate;
    ////                    x.Item2.Text.LabelText = $"{closestCoordinate.Y:0.##}\n{DateTime.FromOADate(closestCoordinate.X)}";
    ////                    x.Item2.Text.LabelFontColor = x.MyItem.Streamer.Color;

    ////                    WpfPlot1vm?.Refresh();
    ////                }
    ////            });
    ////    }

    ////    private void InitializeControlMenu() => ControlMenu.Add(new SPSettings("Axis Rules"));

    ////    private (Crosshair? Crosshair, Marker? Marker, Text? Text) CreateCursorValues()
    ////    {
    ////        // Create a crosshair to highlight the point under the cursor
    ////        var crosshair = WpfPlot1vm?.Plot.Add.Crosshair(0, 0);
    ////        crosshair!.IsVisible = false;

    ////        // Create a marker to highlight the point under the cursor
    ////        var marker = WpfPlot1vm?.Plot.Add.Marker(0, 0);
    ////        marker!.Shape = MarkerShape.OpenCircle;
    ////        marker.Size = 17;
    ////        marker.LineWidth = 2;
    ////        marker!.IsVisible = false;

    ////        // Create a text label to place near the highlighted value
    ////        var text = WpfPlot1vm?.Plot.Add.Text(" ", 0, 0);
    ////        text!.LabelAlignment = Alignment.LowerLeft;
    ////        text.LabelBold = true;
    ////        text.OffsetX = 7;
    ////        text.OffsetY = -7;
    ////        text!.IsVisible = false;

    ////        return (Crosshair: crosshair, Marker: marker, Text: text);
    ////    }

    ////    /// <summary>
    ////    /// Setup the axes.
    ////    /// </summary>
    ////    private void AxesSetup()
    ////    {
    ////        var baseColor = Color.FromHex("#D0D0D0");
    ////        ////WpfPlot1vm.Plot.Axes.AutoScaleX(_xAxis1);
    ////        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Left);
    ////        _yAxisLeft.FrameLineStyle.Color = baseColor;

    ////        // colors
    ////        _xAxis1.Label.ForeColor = Color.FromHex("#377eb8");
    ////        _xAxis1.FrameLineStyle.Color = baseColor;
    ////        _xAxis1.TickLabelStyle.ForeColor = baseColor;
    ////        _xAxis1.MajorTickStyle.Color = baseColor;
    ////        _xAxis1.MinorTickStyle.Color = baseColor;

    ////        // Setup Y Axis
    ////        for (int i = 0; i < 3; i++)
    ////        {
    ////            _yAxis1.Add(WpfPlot1vm!.Plot.Axes.AddRightAxis());
    ////            _yAxis1[i].FrameLineStyle.Color = baseColor;
    ////            _yAxis1[i].TickLabelStyle.ForeColor = baseColor;
    ////            _yAxis1[i].MajorTickStyle.Color = baseColor;
    ////            _yAxis1[i].MinorTickStyle.Color = baseColor;
    ////        }

    ////        // Configure Axis Unit and Color
    ////        _yAxis1[0].Label.Text = "mm/s";
    ////        _yAxis1[0].Label.ForeColor = Color.FromHex("#377eb8");

    ////        _yAxis1[1].Label.Text = "um";
    ////        _yAxis1[1].Label.ForeColor = Color.FromHex("#4daf4a");

    ////        _yAxis1[2].Label.Text = "g";
    ////        _yAxis1[2].Label.ForeColor = Color.FromHex("#984ea3");
    ////    }

    ////    /// <summary>
    ////    /// Scale manually axe X.
    ////    /// </summary>
    ////    private void ManualScaleX()
    ////    {
    ////        DateTime now = DateTime.Now;
    ////        var doublenow = now.ToOADate();
    ////        ////DateTime limits = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute - 1, now.Second);
    ////        DateTime limits = now.Add(TimeSpan.FromMinutes(-60));
    ////        var doublelimits = limits.ToOADate();
    ////        WpfPlot1vm?.Plot.Axes.SetLimitsX(doublelimits, doublenow, _xAxis1);
    ////        WpfPlot1vm?.Refresh();
    ////    }
}
