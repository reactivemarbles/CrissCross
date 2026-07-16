// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Plottables;
using AppBarIcons = CrissCross.WPF.UI.Controls.AppBarIcons;

namespace CrissCross.WPF.Plot;

/// <summary>Interaction logic for WPF Chart AICS.</summary>
[SupportedOSPlatform("windows")]
public partial class LiveChart : ReactiveUserControl<LiveChartViewModel>
{
    /// <summary>The boundary used to distinguish manual scale interactions.</summary>
    private const double ManualScaleBoundary = 50;

    /// <summary>The coordinate marker text offset.</summary>
    private const float CoordinateMarkerTextOffset = 7;

    /// <summary>The duration used for the plot coordinate hover tooltip.</summary>
    private const int HoverTooltipDurationMilliseconds = 60_000;

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
        ViewModel = new(MainChartGrid)
        {
            UseFixedNumberOfPoints = useFixedNumberOfPoints,
            NumberPointsPlotted = numberPointsPlotted,
        };
        DataContext = ViewModel;
        ToolTipService.SetInitialShowDelay(ViewModel.WpfPlot1vm!, 0);
        ToolTipService.SetShowDuration(ViewModel.WpfPlot1vm!, HoverTooltipDurationMilliseconds);
        _ = ViewModel
            .ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Exception in LiveChart: {ex.Message}"))
            .DisposeWith(_dd);
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
        _ = UnloadedObservable().Subscribe(_ => DisposeReactivePlotConnection()).DisposeWith(d);
        BindCommands(d);
        BindMenuVisibility(d);
        BindRightProperties(d);
        BindChartMetadata(d);
    }

    /// <summary>Binds the chart commands.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindCommands(CompositeDisposable disposables)
    {
        _ = this.BindCommand(ViewModel, vm => vm.GraphLocked, v => v.LiveHistoryBtn).DisposeWith(disposables);
        _ = this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(disposables);
        _ = this.BindCommand(ViewModel, vm => vm.RemoveLabelsBtn, v => v.RemoveLabelBtn).DisposeWith(disposables);
        _ = this.BindCommand(ViewModel, vm => vm.AddCrosshairBtn, v => v.AddCrosshairBtn).DisposeWith(disposables);
        _ = this.BindCommand(ViewModel, vm => vm.ExpandMenuBtn, v => v.PlotSettings).DisposeWith(disposables);
    }

    /// <summary>Binds menu expansion to command visibility.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindMenuVisibility(CompositeDisposable disposables)
    {
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.IsMenuExpanded,
                v => v.LiveHistoryBtn.Visibility,
                x => x ? Visibility.Visible : Visibility.Collapsed)
            .DisposeWith(disposables);
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.IsMenuExpanded,
                v => v.EnableMarkerBtn.Visibility,
                x => x ? Visibility.Visible : Visibility.Collapsed)
            .DisposeWith(disposables);
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.IsMenuExpanded,
                v => v.AddCrosshairBtn.Visibility,
                x => x ? Visibility.Visible : Visibility.Collapsed)
            .DisposeWith(disposables);
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.IsMenuExpanded,
                v => v.RemoveLabelBtn.Visibility,
                x => x ? Visibility.Visible : Visibility.Collapsed)
            .DisposeWith(disposables);
    }

    /// <summary>Binds the selected chart object to the right properties panel.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindRightProperties(CompositeDisposable disposables)
    {
        _ = this.OneWayBind(ViewModel, vm => vm.RightPropertyVisibility, v => v.RightProperties.Visibility)
            .DisposeWith(disposables);

        _ = this.WhenAnyValue(x => x.ViewModel!.SelectedSetting)
            .Where(static settings => settings is not null)
            .Select(static settings => settings!)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(settings =>
            {
                RightProperties.ViewModel!.ItemName = settings.ItemName;
                RightProperties.ViewModel!.LineWidth = settings.LineWidth;
                RightProperties.ViewModel!.LineColor = settings.Color;
                RightProperties.ViewModel!.ItemVisibility = settings.Visibility;
                RightProperties.ViewModel!.SelectedSetting = settings;
            })
            .DisposeWith(disposables);
    }

    /// <summary>Binds chart titles, legends, and point-window settings.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindChartMetadata(CompositeDisposable disposables)
    {
        _ = this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Text).DisposeWith(disposables);
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.Title,
                v => v.Title.Visibility,
                x => x == " " ? Visibility.Collapsed : Visibility.Visible)
            .DisposeWith(disposables);
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.LegendPosition,
                v => v.RightLegend.Visibility,
                x => x == LegendPosition.Right ? Visibility.Visible : Visibility.Collapsed)
            .DisposeWith(disposables);
        _ = this.OneWayBind(
                ViewModel,
                vm => vm.LegendPosition,
                v => v.TopLegend.Visibility,
                x => x == LegendPosition.Top ? Visibility.Visible : Visibility.Collapsed)
            .DisposeWith(disposables);

        _ = this.Bind(ViewModel, vm => vm.UseFixedNumberOfPoints, v => v.UseFixedNumberOfPoints)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel, vm => vm.NumberPointsPlotted, v => v.NumberPointsPlotted).DisposeWith(disposables);
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

    /// <summary>Handles the InitializeButtons operation.</summary>
    private void InitializeButtons()
    {
        // BY DEFAULT: LOCKED AND AUTOSCALED
        // LOCK GRAPH BUTTON
        ViewModel?.GraphLocked?.Subscribe(_ => ExecuteLockUnlock()).DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel?.EnableMarkerBtn?.Subscribe(_ => ExecuteMarkerOnOff()).DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel
            ?.ExpandMenuBtn?.ObserveOn(RxSchedulers.MainThreadScheduler)
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
        EnableMarkerBtn.Icon = _crosshairOff ? AppBarIcons.Md_crosshairs_off : AppBarIcons.Md_crosshairs;
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
        LiveHistoryBtn.Icon = _locked ? AppBarIcons.Md_lock : AppBarIcons.Md_lock_open;
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

        ViewModel.WpfPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleX(
            xaxis: ViewModel!.XAxis1);
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
}
