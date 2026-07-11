// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CP.WPF.Controls;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Plottables;
using PlotColor = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>Interaction logic for WPF Chart AICS.</summary>
[SupportedOSPlatform("windows")]
public partial class LiveChart : ReactiveUserControl<LiveChartViewModel>
{
    /// <summary>The boundary used to distinguish manual scale interactions.</summary>
    private const double ManualScaleBoundary = 50;

    /// <summary>The coordinate marker text offset.</summary>
    private const float CoordinateMarkerTextOffset = 7;

    /// <summary>Stores the dd value.</summary>
    private readonly CompositeDisposable _dd = [];

    /// <summary>Stores the reactive plot connection value.</summary>
    private IReactivePlotConnection? _reactivePlotConnection;

    /// <summary>Stores the crosshair disposable value.</summary>
    private IDisposable? _crosshairDisposable;

    /// <summary>Stores the need lock value.</summary>
    private bool _needLock;

    /// <summary>Stores the need auto scale value.</summary>
    private bool _needAutoScale = true;

    /// <summary>Stores the need cross hair off value.</summary>
    private bool _needCrossHairOff = true;

    /// <summary>Stores the auto scaled value.</summary>
    private bool _autoScaled;

    /// <summary>Stores the locked value.</summary>
    private bool _locked = true;

    /// <summary>Stores the crosshair off value.</summary>
    private bool _crosshairOff;

    /// <summary>Stores the plottable being dragged value.</summary>
    private AxisLine? _plottableBeingDragged;

    /// <summary>Initializes a new instance of the <see cref="LiveChart"/> class.</summary>
    public LiveChart()
    {
        InitializeComponent();
        First = false;
        var useFixedNumberOfPoints = UseFixedNumberOfPoints;
        var numberPointsPlotted = NumberPointsPlotted;
        ViewModel = new(MainChartGrid) { UseFixedNumberOfPoints = useFixedNumberOfPoints, NumberPointsPlotted = numberPointsPlotted };
        DataContext = ViewModel;
        _ = ViewModel.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Exception in LiveChart: {ex.Message}")).DisposeWith(_dd);
        ExecuteLockUnlock();
        ExecuteManAutoScale();
        InitializeButtons();
        _ = this.WhenActivated(ElementBinding1);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets the update.</summary>
    /// <value>
    /// The update.
    /// </value>
    public bool First { get; set; } = true;

    /// <summary>Handles the ElementBinding1 operation.</summary>
    /// <param name="d">The d value.</param>
    private void ElementBinding1(CompositeDisposable d)
    {
        _ = new ActionDisposable(DisposeReactivePlotConnection).DisposeWith(d);
        _ = UnloadedObservable()
            .Subscribe(_ => DisposeReactivePlotConnection())
            .DisposeWith(d);

        _ = this.BindCommand(ViewModel, vm => vm.GraphLocked, v => v.LiveHistoryBtn).DisposeWith(d);
        _ = this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(d);
        _ = this.BindCommand(ViewModel, vm => vm.RemoveLabelsBtn, v => v.RemoveLabelBtn).DisposeWith(d);
        _ = this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(d);
        _ = this.BindCommand(ViewModel, vm => vm.AddCrosshairBtn, v => v.AddCrosshairBtn).DisposeWith(d);

        _ = this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.LiveHistoryBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.EnableMarkerBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.AddCrosshairBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.RemoveLabelBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.EnableMarkerBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.AddCrosshairBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

        _ = this.BindCommand(ViewModel, vm => vm.ExpandMenuBtn, v => v.PlotSettings).DisposeWith(d);

        _ = this.OneWayBind(ViewModel, vm => vm.RightPropertyVisibility, v => v.RightProperties.Visibility).DisposeWith(d);

        // Populate form when selection changes (one-way to avoid conflicts)
        _ = this.WhenAnyValue(x => x.ViewModel!.SelectedSetting)
            .Where(static settings => settings is not null)
            .Select(static settings => settings!)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(settings =>
            {
                // Populate form fields for editing
                RightProperties.ViewModel!.ItemName = settings.ItemName;
                RightProperties.ViewModel!.LineWidth = settings.LineWidth;
                RightProperties.ViewModel!.LineColor = settings.Color;
                RightProperties.ViewModel!.ItemVisibility = settings.Visibility;
                RightProperties.ViewModel!.SelectedSetting = settings;
            }).DisposeWith(d);

        _ = this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Text).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Visibility, x => x == " " ? Visibility.Collapsed : Visibility.Visible).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.LegendPosition, v => v.RightLegend.Visibility, x => x == LegendPosition.Right ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        _ = this.OneWayBind(ViewModel, vm => vm.LegendPosition, v => v.TopLegend.Visibility, x => x == LegendPosition.Top ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

        // Bind other scalar settings directly
        _ = this.Bind(ViewModel, vm => vm.UseFixedNumberOfPoints, v => v.UseFixedNumberOfPoints).DisposeWith(d);
        _ = this.Bind(ViewModel, vm => vm.NumberPointsPlotted, v => v.NumberPointsPlotted).DisposeWith(d);
    }

    /// <summary>Handles the IndexText_MouseUp operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void IndexText_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock { DataContext: IPlottableUI item })
        {
            return;
        }

        var setting = item.ChartSettings;
        if (ViewModel!.SelectedSetting == setting)
        {
            ViewModel!.SelectedSetting = null;
            RightProperties.Visibility = Visibility.Collapsed;
            ViewModel!.RightPropertyVisibility = Visibility.Collapsed;
            return;
        }

        ViewModel!.SelectedSetting = setting;
        RightProperties.Visibility = Visibility.Visible;
        ViewModel!.RightPropertyVisibility = Visibility.Visible;
    }

    /// <summary>Handles the ChangeReactivePlotSources operation.</summary>
    /// <param name="sources">The sources value.</param>
    private void ChangeReactivePlotSources(IEnumerable<IReactivePlotSource>? sources)
    {
        DisposeReactivePlotConnection();
        if (ViewModel is null || sources is null)
        {
            return;
        }

        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel.ClearContent();
        var sourceArray = (sources as IReactivePlotSource[]) ?? sources.ToArray();
        ConfigureReactivePlotXAxis(sourceArray);
        _reactivePlotConnection = new ReactivePlotBinder().Bind(
            ViewModel,
            sourceArray,
            new ReactivePlotBindingOptions
            {
                UiScheduler = RxSchedulers.MainThreadScheduler,
                MaxVisiblePoints = (UseFixedNumberOfPoints ? NumberPointsPlotted : null),
                MaxAxisCount = Math.Max(1, ViewModel.YAxisList.Count),
            });
        ViewModel.InitializeAxisLines();
    }

    /// <summary>Handles the DisposeReactivePlotConnection operation.</summary>
    private void DisposeReactivePlotConnection()
    {
        _reactivePlotConnection?.Dispose();
        _reactivePlotConnection = null;
    }

    /// <summary>Handles the UnloadedObservable operation.</summary>
    /// <returns>The result.</returns>
    private IObservable<EventPattern<RoutedEventArgs>> UnloadedObservable() =>
        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(handler => Unloaded += handler, handler => Unloaded -= handler);

    /// <summary>Handles the ChangeScatterObserver operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeScatterObserver(ScatterEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeScatterPlotLines(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeScatterObserver operation.</summary>
    private void ChangeScatterObserver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeScatterPlotLines(ScatterObservablesWithTimeStamp);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalObserver operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalObserver(SignalEnumObsTicks input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
        _crosshairDisposable = ViewModel.WhenAnyValue(x => x.CrossHairEnabled)
            .Subscribe(d => ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings.IsCrossHairVisible = d));
    }

    /// <summary>Handles the ChangeSignalObserver operation.</summary>
    private void ChangeSignalObserver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(SignalObservablesWithTimeStamp);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
        _crosshairDisposable = ViewModel.WhenAnyValue(x => x.CrossHairEnabled)
            .Subscribe(d => ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings.IsCrossHairVisible = d));
    }

    /// <summary>Handles the ChangeDataLoggerObserver operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeDataLoggerObserver(DataLoggerEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeDataLoggerPlotLinesWithPoints(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeDataLoggerObserver operation.</summary>
    private void ChangeDataLoggerObserver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeDataLoggerPlotLinesWithPoints(DataLoggerObservablesWithPoints);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalData operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalData(SignalXYTimestamp input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalData operation.</summary>
    private void ChangeSignalData()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(DataWithTimeStamp);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalDataWithPoints(SignalXYPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalPoints(input.Data);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataWithPoints operation.</summary>
    private void ChangeSignalDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalPoints(SignalWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalsDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalsDataWithPoints(SignalXYEnumPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();

        ViewModel?.InitializeLinesForSignalPoints(input.Data);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalsDataWithPoints operation.</summary>
    private void ChangeSignalsDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();

        ViewModel?.InitializeLinesForSignalPoints(SignalsWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataObserverWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalDataObserverWithPoints(StreamerEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(input.Data, fs: Frequency, sampleCount: Convert.ToUInt32(NSamples));
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataObserverWithPoints operation.</summary>
    private void ChangeSignalDataObserverWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(SignalObservablesWithPoints, fs: Frequency, sampleCount: Convert.ToUInt32(NSamples));
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeScatterDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeScatterDataWithPoints(ScatterPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForScatterPoints(input.Data);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeScatterDataWithPoints operation.</summary>
    private void ChangeScatterDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForScatterPoints(ScatterWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the InitializeButtons operation.</summary>
    private void InitializeButtons()
    {
        // BY DEFAULT: LOCKED AND AUTOSCALED
        // LOCK GRAPH BUTTON
        ViewModel?.GraphLocked?
            .Subscribe(_ => ExecuteLockUnlock())
            .DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel?.EnableMarkerBtn?
            .Subscribe(_ => ExecuteMarkerOnOff())
            .DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel?.ExpandMenuBtn?
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ => ToggleLeftPanelVisibility())
            .DisposeWith(_dd);
    }

    /// <summary>Handles the ExecuteMarkerOnOff operation.</summary>
    private void ExecuteMarkerOnOff()
    {
        if (_needCrossHairOff && !_crosshairOff)
        {
            ViewModel!.CrossHairEnabled = false;
            _crosshairOff = true;
        }
        else if (!_needCrossHairOff && _crosshairOff)
        {
            ViewModel!.CrossHairEnabled = true;
            _crosshairOff = false;
        }

        _needCrossHairOff = !(_needCrossHairOff && _crosshairOff);
        EnableMarkerBtn.ToolTip = _crosshairOff ? "Marker off" : "Marker";
        EnableMarkerBtn.Icon = _crosshairOff ? AppBarIcons.md_crosshairs_off : AppBarIcons.md_crosshairs;
        ViewModel!.WpfPlot1vm?.Refresh();
    }

    /// <summary>Handles the ExecuteLockUnlock operation.</summary>
    private void ExecuteLockUnlock()
    {
        if (_needLock)
        {
            LockedPlotSetup();
            EnsureAutoScaleAfterLock();
        }
        else
        {
            UnockedPlotSetup();
        }

        _needLock = !(_needLock && _locked);
        LiveHistoryBtn.ToolTip = _locked ? "Locked" : "Interact";
        LiveHistoryBtn.Icon = _locked ? AppBarIcons.md_lock : AppBarIcons.md_lock_open;
        ViewModel!.WpfPlot1vm?.Refresh();
    }

    /// <summary>Handles the ExecuteManAutoScale operation.</summary>
    private void ExecuteManAutoScale()
    {
        if (_needAutoScale)
        {
            AutoScaledSetup();
        }
        else
        {
            ManualScaledSetup();
        }

        _needAutoScale = !(_needAutoScale && _autoScaled);
        if (!_locked)
        {
            _needLock = true;
            ExecuteLockUnlock();
        }

        ViewModel!.WpfPlot1vm?.Refresh();
    }

    /// <summary>Lockeds the plot setup.</summary>
    private void LockedPlotSetup()
    {
        if (_locked)
        {
            return;
        }

        ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
        ViewModel!.WpfPlot1vm?.UserInputProcessor.Disable();
        _locked = true;
    }

    /// <summary>Unockeds the plot setup.</summary>
    private void UnockedPlotSetup()
    {
        if (!_locked)
        {
            return;
        }

        ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = false;
        ViewModel!.WpfPlot1vm?.UserInputProcessor.Enable();
        _locked = false;
    }

    /// <summary>Manuals the scaled setup.</summary>
    private void ManualScaledSetup()
    {
        if (!_autoScaled)
        {
            return;
        }

        ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
        foreach (var verticalAxis in ViewModel.YAxisList)
        {
            ViewModel!.WpfPlot1vm?.Plot.Axes.SetLimitsY(-ManualScaleBoundary, ManualScaleBoundary, verticalAxis);
        }

        ViewModel.WpfPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleX(xaxis: ViewModel!.XAxis1);
        _autoScaled = false;
    }

    /// <summary>Automatics the scaled setup.</summary>
    private void AutoScaledSetup()
    {
        if (_autoScaled)
        {
            return;
        }

        ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
        ViewModel.WpfPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleAll();
        _autoScaled = true;
    }

    /// <summary>Handles the YAxisSetup operation.</summary>
    private void YAxisSetup()
    {
        var (yNames, hexColors) = YAxisName;
        if (ViewModel is null || yNames is null || hexColors is null || yNames.Count == 0 || hexColors.Count == 0)
        {
            return;
        }

        ViewModel.YAxesSetup(YAxisName);
    }

    /// <summary>Handles the ConfigureReactivePlotXAxis operation.</summary>
    /// <param name="sources">The sources value.</param>
    private void ConfigureReactivePlotXAxis(IEnumerable<IReactivePlotSource> sources)
    {
        var axisKinds = sources
            .OfType<ReactivePlotSource>()
            .Select(source => source.XAxisKind)
            .Where(kind => kind is not null)
            .Select(kind => kind!.Value)
            .Distinct()
            .ToArray();

        if (axisKinds.Length == 1 && axisKinds[0] is PlotXAxisKind.OADate or PlotXAxisKind.Ticks)
        {
            ViewModel!.CreateAxisWithTimeStamp();
            return;
        }

        ViewModel!.CreateAxisWithPoints();
    }

    /// <summary>Handles the MainChartGrid_MouseDown operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            AddCoordinateMarker(e);
            return;
        }

        BeginAxisLineDrag(e);
    }

    /// <summary>Toggles the left panel visibility.</summary>
    private void ToggleLeftPanelVisibility() =>
        ViewModel!.LeftPanelVisibility = ViewModel.LeftPanelVisibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;

    /// <summary>Enables autoscale after locking if needed.</summary>
    private void EnsureAutoScaleAfterLock()
    {
        if (_autoScaled)
        {
            return;
        }

        _needAutoScale = true;
        ExecuteManAutoScale();
    }

    /// <summary>Adds a coordinate marker at the current mouse position.</summary>
    /// <param name="e">The mouse event arguments.</param>
    private void AddCoordinateMarker(MouseEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        Pixel mousePixel = new(adjustedX, adjustedY);
        var mouseLocation = ViewModel!.WpfPlot1vm!.Plot.GetCoordinates(mousePixel, ViewModel!.XAxis1, ViewModel.YAxisList[0]);
        var horizontalCoordinate = mouseLocation.X;
        var verticalCoordinate = mouseLocation.Y;
        var text = ViewModel.IsXAxisDateTime
            ? "X : " + DateTime.FromOADate(horizontalCoordinate).ToLongTimeString() + "\nY : " + verticalCoordinate.ToString("F2")
            : "X : " + horizontalCoordinate.ToString("F2") + "\nY : " + verticalCoordinate.ToString("F2");

        var marker = ViewModel.WpfPlot1vm.Plot.Add.Marker(mouseLocation);
        var markerText = ViewModel.WpfPlot1vm.Plot.Add.Text(text, mouseLocation);
        markerText.OffsetX = CoordinateMarkerTextOffset;
        markerText.OffsetY = -CoordinateMarkerTextOffset;
        markerText!.LabelFontColor = PlotColor.FromColor(System.Drawing.Color.FromName("White"));
        marker.Axes.XAxis = ViewModel!.WpfPlot1vm!.Plot.Axes.Bottom;
        marker.Axes.YAxis = ViewModel.YAxisList[0];
        markerText.Axes.XAxis = ViewModel!.WpfPlot1vm!.Plot.Axes.Bottom;
        markerText.Axes.YAxis = ViewModel.YAxisList[0];
        ViewModel.LabelCollection.Add((marker, markerText));
    }

    /// <summary>Begins dragging an axis line when one is under the mouse pointer.</summary>
    /// <param name="e">The mouse event arguments.</param>
    private void BeginAxisLineDrag(MouseEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        var lineUnderMouse = ViewModel?.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
        if (lineUnderMouse is null)
        {
            return;
        }

        _plottableBeingDragged = lineUnderMouse;
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Disable();
    }

    /// <summary>Handles the MainChartGrid_MouseUp operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseUp(object sender, MouseEventArgs e)
    {
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Enable(); // enable panning again
        _plottableBeingDragged = null;
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Enable(); // enable panning again
        ViewModel!.WpfPlot1vm!.Refresh();
    }

    /// <summary>Handles the MainChartGrid_MouseMove operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void MainChartGrid_MouseMove(object sender, MouseEventArgs e)
    {
        var (adjustedX, adjustedY) = GetAdjustedMousePosition(e);
        var rect = ViewModel!.WpfPlot1vm!.Plot.GetCoordinateRect((float)adjustedX, (float)adjustedY, radius: 5, ViewModel!.XAxis1, ViewModel.YAxisList[0]);
        if (_plottableBeingDragged is null)
        {
            UpdateMouseCursor(adjustedX, adjustedY);
            return;
        }

        if (_plottableBeingDragged is HorizontalLine horizontalLine)
        {
            horizontalLine.Y = rect.VerticalCenter;
            horizontalLine.Text = $"{horizontalLine.Y:0.00}";
        }
        else if (_plottableBeingDragged is VerticalLine verticalLine)
        {
            verticalLine.X = rect.HorizontalCenter;
            verticalLine.Text = ViewModel.IsXAxisDateTime ? DateTime.FromOADate(Convert.ToDouble(verticalLine.X)).ToLongTimeString() : $"{verticalLine.X:0.00}";
        }

        ViewModel!.WpfPlot1vm!.Refresh();
    }

    /// <summary>Gets the DPI-adjusted mouse position.</summary>
    /// <param name="e">The mouse event arguments.</param>
    /// <returns>The adjusted position.</returns>
    private (double X, double Y) GetAdjustedMousePosition(MouseEventArgs e)
    {
        var position = e.GetPosition(MainChartGrid);
        var dpiInfo = VisualTreeHelper.GetDpi(MainChartGrid);
        return (position.X * dpiInfo.DpiScaleX, position.Y * dpiInfo.DpiScaleY);
    }

    /// <summary>Updates the mouse cursor for the line under the pointer.</summary>
    /// <param name="adjustedX">The adjusted X coordinate.</param>
    /// <param name="adjustedY">The adjusted Y coordinate.</param>
    private void UpdateMouseCursor(double adjustedX, double adjustedY)
    {
        var lineUnderMouse = ViewModel!.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
        if (lineUnderMouse is null)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }

        if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine)
        {
            Mouse.OverrideCursor = Cursors.SizeWE;
            return;
        }

        if (!lineUnderMouse.IsDraggable || lineUnderMouse is not HorizontalLine)
        {
            return;
        }

        Mouse.OverrideCursor = Cursors.SizeNS;
    }
}
