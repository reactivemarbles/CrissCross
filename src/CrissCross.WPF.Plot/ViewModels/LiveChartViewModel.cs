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
using CP.Reactive;
using DynamicData;
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
[SupportedOSPlatform("windows10.0.19041")]
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
            ClearAxisLines2();
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
    /// Gets or sets data plot.
    /// </summary>
    public ReactiveList<IYAxis> Axes { get; set; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<IPlottableUI> PlotLinesCollectionUI { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<ChartObjects> ControlMenu { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<AxisLinesUI> AxisLinesUI { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<Crosshair_UI> CrosshairCollection { get; }

    /// <summary>
    /// Gets data plot.
    /// </summary>
    public ReactiveList<(Marker marker, Text text)> LabelCollection { get; }

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
    public ISubject<Coordinates> MouseCoordinatesObservable { get; set; }

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
    public ReactiveCommand<Unit, Unit>? AddCrosshairBtn { get; }

    /// <summary>
    /// Gets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public ReactiveCommand<Unit, Unit>? RemoveLabelsBtn { get; }

    /// <summary>
    /// Gets the manage axis limits.
    /// </summary>
    /// <value>
    /// The manage axis limits.
    /// </value>
    public ReactiveCommand<Unit, Unit>? ExpandMenuBtn { get; }

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
    /// Gets or sets the y axis list.
    /// </summary>
    /// <value>
    /// The y axis list.
    /// </value>
    public ReactiveList<IYAxis> YAxisList { get; set; }

    /// <summary>
    /// Gets or sets the x axis1.
    /// </summary>
    /// <value>
    /// The x axis1.
    /// </value>
    public IXAxis XAxis1 { get; set; }

    /// <summary>
    /// Manuals the scaling.
    /// </summary>
    /// <param name="xaxis">The xaxis.</param>
    /// <returns>
    /// .
    /// </returns>
    public static Action<RenderPack> AutoScaleX(IXAxis xaxis) =>
        rp => rp.Plot.Axes.AutoScaleX(xaxis);

    /// <summary>
    /// Manuals the scaling.
    /// </summary>
    /// <returns>.</returns>
    public static Action<RenderPack> AutoScaleY() =>
        rp => rp.Plot.Axes.AutoScaleY();

    /// <summary>
    /// Manuals the scaling.
    /// </summary>
    /// <returns>.</returns>
    public static Action<RenderPack> AutoScaleAll() =>
        rp => rp.Plot.Axes.AutoScale();

    /// <summary>
    /// Hides all YAxis.
    /// </summary>
    public void HideAllYAxis()
    {
        foreach (var axis in YAxisList)
        {
            axis.IsVisible = false;
        }
    }

    /// <summary>
    /// Creates the axis with data stamp.
    /// </summary>
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
    /// Axises the style.
    /// </summary>
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
    /// Creates the axis with data stamp.
    /// </summary>
    public void CreateAxisWithPoints()
    {
        WpfPlot1vm?.Plot.Axes.Remove(WpfPlot1vm.Plot.Axes.Bottom);
        XAxis1 = WpfPlot1vm!.Plot.Axes.AddBottomAxis();
        SetXAxisColour();
    }

    /// <summary>
    /// Sets the x axis colour.
    /// </summary>
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
    /// Clears the labels.
    /// </summary>
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
    /// Clears the axis lines2.
    /// </summary>
    public void ClearAxisLines2()
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

    /// <summary>
    /// Clears the content.
    /// </summary>
    public void ClearContent()
    {
        WpfPlot1vm?.Plot.Clear();

        // SIGNAL
        if (PlotLinesCollectionUI?.Count > 0)
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
    /// Initializes the axes.
    /// </summary>
    /// <param name="data">The data contains : axes names and colors.</param>
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

        YAxisList.RemoveMany(YAxisList.Items);

        // combine yName and hexColors lists
        var combinedList = data.yNames.Zip(data.hexColors, (name, color) => (name, color));

        // create the axes
        foreach (var (name, color) in combinedList)
        {
            YAxisList.Add(WpfPlot1vm!.Plot.Axes.AddRightAxis());
            YAxisList.Last().FrameLineStyle.Color = baseColor;
            YAxisList.Last().TickLabelStyle.ForeColor = baseColor;
            YAxisList.Last().MajorTickStyle.Color = baseColor;
            YAxisList.Last().MinorTickStyle.Color = baseColor;
            YAxisList.Last().Label.Text = name;
            YAxisList.Last().Label.ForeColor = Color.FromHex(color);
        }
    }

    /// <summary>
    /// Setups the _plotAutoScaled.
    /// </summary>
    public void ManualScaleY()
    {
        foreach (var axis in YAxisList)
        {
            WpfPlot1vm?.Plot.Axes.SetLimitsY(0, 100, axis);
        }

        WpfPlot1vm?.Refresh();
    }

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
    /// Setup the axes.
    /// </summary>
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
            YAxisList.Add(WpfPlot1vm!.Plot.Axes.AddRightAxis());
            YAxisList[i].FrameLineStyle.Color = baseColor;
            YAxisList[i].TickLabelStyle.ForeColor = baseColor;
            YAxisList[i].MajorTickStyle.Color = baseColor;
            YAxisList[i].MinorTickStyle.Color = baseColor;
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
    /// Scale manually axe X.
    /// </summary>
    private void ManualScaleX()
    {
        var now = DateTime.Now;
        var doublenow = now.ToOADate();
        var limits = now.Add(TimeSpan.FromMinutes(-60));
        var doublelimits = limits.ToOADate();
        WpfPlot1vm?.Plot.Axes.SetLimitsX(doublelimits, doublenow, XAxis1);
        WpfPlot1vm?.Refresh();
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
}
