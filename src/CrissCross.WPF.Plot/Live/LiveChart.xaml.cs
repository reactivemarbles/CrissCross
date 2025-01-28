// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CP.WPF.Controls;
using ReactiveUI;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Interaction logic for WPF Chart AICS.
/// </summary>
public partial class LiveChart
{
    private readonly CompositeDisposable _dd = [];
    private IDisposable? _crosshairDisposable;
    private bool _needLock;
    private bool _needAutoScale = true;
    private bool _needCrossHairOff = true;
    private bool _autoScaled;
    private bool _locked = true;
    private bool _crosshairOff;
    private bool _activatedView;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChart" /> class.
    /// </summary>
    public LiveChart()
    {
        InitializeComponent();
        First = false;
        ViewModel = new(MainChartGrid);
        DataContext = ViewModel;
        ExecuteLockUnlock();
        ExecuteManAutoScale();

        InitializeButtons();
        _activatedView = true;
        this.WhenActivated(d =>
        {
            ElementBinding1(d);
        });

        ////YAxisName.Subscribe(d => ViewModel!.YAxesSetup(d));
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the update.
    /// </summary>
    /// <value>
    /// The update.
    /// </value>
    public bool First { get; set; } = true;

    private void ElementBinding1(CompositeDisposable d)
    {
        this.BindCommand(ViewModel, vm => vm.GraphLocked, v => v.LiveHistory).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.AutoScale, v => v.AutoScale).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.MakeLeftPanelVisible, v => v.PlotSettings).DisposeWith(d);

        ////this.OneWayBind(ViewModel, vm => vm.DataLoggerCollectionUI, v => v.Itemscontrol1.ItemsSource).DisposeWith(d);
        ////this.OneWayBind(ViewModel, vm => vm.SignalCollectionUI, v => v.Itemscontrol2.ItemsSource).DisposeWith(d);
        ////this.OneWayBind(ViewModel, vm => vm.ScatterCollectionUI, v => v.Itemscontrol3.ItemsSource).DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.RightPropertyVisibility, v => v.RightProperties.Visibility).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.SelectedSetting!.ItemName, v => v.RightProperties.textbox1.Text).DisposeWith(d);

        this.Bind(ViewModel, vm => vm.SelectedSetting!.LineWidth, v => v.RightProperties.LineWidth.Value).DisposeWith(d);
        this.Bind(ViewModel, vm => vm.SelectedSetting!.Color, v => v.RightProperties.colorsComboBox.SelectedItem).DisposeWith(d);
        this.Bind(ViewModel, vm => vm.SelectedSetting!.Visibility, v => v.RightProperties.visibilityComboBox.SelectedItem).DisposeWith(d);
    }

    private void IndexText_MouseUp(object sender, MouseButtonEventArgs e)
    {
        // make sure text block was clicked
        if (sender is TextBlock textBlock)
        {
            // read clicked data context and current data context
            var dataContext = textBlock.DataContext;
            var currentDataContext = RightProperties.DataContext;

            // is data context is other
            if (dataContext != null)
            {
                if (dataContext is DataLoggerUI item)
                {
                    var setting = item.ChartSettings;
                    if (ViewModel!.SelectedSetting == setting)
                    {
                        ViewModel!.SelectedSetting = null;
                        ViewModel!.RightPropertyVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ViewModel!.SelectedSetting = setting;
                        ViewModel!.RightPropertyVisibility = Visibility.Visible;
                    }
                }

                if (dataContext is ScatterUI item2)
                {
                    var setting = item2.ChartSettings;
                    if (ViewModel!.SelectedSetting == setting)
                    {
                        ViewModel!.SelectedSetting = null;
                        ViewModel!.RightPropertyVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ViewModel!.SelectedSetting = setting;
                        ViewModel!.RightPropertyVisibility = Visibility.Visible;
                    }
                }

                if (dataContext is StreamerUI item3)
                {
                    var setting = item3.ChartSettings;
                    if (ViewModel!.SelectedSetting == setting)
                    {
                        ViewModel!.SelectedSetting = null;
                        ViewModel!.RightPropertyVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ViewModel!.SelectedSetting = setting;
                        ViewModel!.RightPropertyVisibility = Visibility.Visible;
                    }
                }
            }
        }
    }

    private void ChangeScatterOberver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeScatterPlotLines(ScatterObservablesWithTimeStamp);
        ViewModel?.InitializeControlMenu(ViewModel?.ScatterCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }
    }

    private void ChangeSignalOberver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(SignalObservablesWithTimeStamp);
        ViewModel?.InitializeControlMenu(ViewModel?.SignalCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }

        _crosshairDisposable = ViewModel.WhenAnyValue(x => x.CrossHairEnabled).Subscribe(d =>
        {
            ViewModel?.SignalCollectionUI.Select(x => x.ChartSettings.IsCrossHairVisible = d);
        });
    }

    private void ChangeDataLoggerOberver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeDataLoggerPlotLinesWithPoints(DataLoggerObservablesWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.DataLoggerCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }
    }

    private void ChangeSignalData()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(DataWithTimeStamp);
        ViewModel?.InitializeControlMenu(ViewModel?.SignalCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }
    }

    private void ChangeSignalDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalPoints(SignalWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.SignalCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }
    }

    private void ChangeSignalDataObserverWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(SignalObservablesWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.SignalCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }
    }

    private void ChangeScatterDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForScatterPoints(ScatterWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.ScatterCollectionUI.Select(x => x.ChartSettings).ToList()!);
        ViewModel?.InitializeAxisLines();
        if (_crosshairDisposable != null)
        {
            _crosshairDisposable.Dispose();
        }
    }

    private void InitializeButtons()
    {
        // BY DEFAULT: LOCKED AND AUTOSCALED
        // LOCK GRAPH BUTTON
        ViewModel?.GraphLocked?
            .Subscribe(_ => ExecuteLockUnlock())
            .DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel?.AutoScale?
            .Subscribe(_ => ExecuteManAutoScale())
            .DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel?.EnableMarkerBtn?
            .Subscribe(_ => ExecuteMarkerOnOff())
            .DisposeWith(_dd);

        // AUTO-SCALE BUTON
        ViewModel?.MakeLeftPanelVisible?
            .ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ =>
            {
                if (ViewModel.LeftPanelVisibility == Visibility.Hidden)
                {
                    ViewModel.LeftPanelVisibility = Visibility.Visible;
                }
                else
                {
                    ViewModel.LeftPanelVisibility = Visibility.Hidden;
                }
            }).DisposeWith(_dd);
    }

    private void ExecuteMarkerOnOff()
    {
        if (_needCrossHairOff)
        {
            if (!_crosshairOff)
            {
                ViewModel!.CrossHairEnabled = false;
                _crosshairOff = true;
            }
        }
        else if (_crosshairOff)
        {
            ViewModel!.CrossHairEnabled = true;
            _crosshairOff = false;
        }

        _needCrossHairOff = !(_needCrossHairOff && _crosshairOff);
        EnableMarkerBtn.ToolTip = _crosshairOff ? "Marker off" : "Marker";
        EnableMarkerBtn.Icon = _crosshairOff ? AppBarIcons.md_crosshairs_off : AppBarIcons.md_crosshairs;
        ViewModel!.WpfPlot1vm?.Refresh();
    }

    private void ExecuteLockUnlock()
    {
        if (_needLock)
        {
            LockedPlotSetup();
        }
        else
        {
            UnockedPlotSetup();
        }

        _needLock = !(_needLock && _locked);
        LiveHistory.ToolTip = _locked ? "Locked" : "Interact";
        LiveHistory.Icon = _locked ? AppBarIcons.md_lock : AppBarIcons.md_lock_open;
        ViewModel!.WpfPlot1vm?.Refresh();
    }

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
        AutoScale.ToolTip = _autoScaled ? "Auto Scale" : "Manual Scale";
        AutoScale.Icon = _autoScaled ? AppBarIcons.md_hand_back_left_off : AppBarIcons.md_hand_back_left;
        _needLock = true;
        ExecuteLockUnlock();
        ViewModel!.WpfPlot1vm?.Refresh();
    }

    /// <summary>
    /// Lockeds the plot setup.
    /// </summary>
    private void LockedPlotSetup()
    {
        if (!_locked)
        {
            ViewModel!.WpfPlot1vm?.UserInputProcessor.Disable();
            _locked = true;

            ////// STREAMER
            ////foreach (var item in ViewModel!.DataUI)
            ////{
            ////    item.Streamer!.ManageAxisLimits = _autoScaled;
            ////    item.ManualScale = !_autoScaled;
            ////    item.AutoScale = _autoScaled;
            ////}

            // SIGNAL
            foreach (var item in ViewModel!.SignalCollectionUI)
            {
                ////item.SignalXY!.Axes.YAxis. = _autoScaled;
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                //// = _autoScaled;
                item.ManualScale = !_autoScaled;
                item.AutoScale = _autoScaled;
            }

            // SCATTER
            foreach (var item in ViewModel!.ScatterCollectionUI)
            {
                ////item.SignalXY!.Axes.YAxis. = _autoScaled;
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                //// = _autoScaled;
                item.ManualScale = !_autoScaled;
                item.AutoScale = _autoScaled;
            }

            // DATA LOGGER
            foreach (var item in ViewModel!.DataLoggerCollectionUI)
            {
                ////item.SignalXY!.Axes.YAxis. = _autoScaled;
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                //// = _autoScaled;
                item.ManualScale = !_autoScaled;
                item.AutoScale = _autoScaled;
            }
        }
    }

    /// <summary>
    /// Unockeds the plot setup.
    /// </summary>
    private void UnockedPlotSetup()
    {
        if (_locked)
        {
            ViewModel!.WpfPlot1vm?.UserInputProcessor.Enable();
            _locked = false;

            ////// STREAMER
            ////foreach (var item in ViewModel!.DataUI)
            ////{
            ////    item.Streamer!.ManageAxisLimits = false;
            ////    item.ManualScale = false;
            ////    item.AutoScale = false;
            ////}

            // SIGNAL
            foreach (var item in ViewModel!.SignalCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Signal!.ManageAxisLimits = false;
                item.ManualScale = false;
                item.AutoScale = false;
            }

            // SCATTER
            foreach (var item in ViewModel!.ScatterCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Signal!.ManageAxisLimits = false;
                item.ManualScale = false;
                item.AutoScale = false;
            }

            // DATA LOGGER
            foreach (var item in ViewModel!.DataLoggerCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Signal!.ManageAxisLimits = false;
                item.ManualScale = false;
                item.AutoScale = false;
            }
        }
    }

    /// <summary>
    /// Manuals the scaled setup.
    /// </summary>
    private void ManualScaledSetup()
    {
        if (_autoScaled)
        {
            ////// STREAMER
            ////foreach (var item in ViewModel!.DataUI)
            ////{
            ////    item.Streamer!.ManageAxisLimits = false;
            ////    item.ManualScale = true;
            ////    item.AutoScale = false;
            ////}

            // SIGNAL
            foreach (var item in ViewModel!.SignalCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Streamer!.ManageAxisLimits = false;
                item.ManualScale = true;
                item.AutoScale = false;
            }

            // SCATTER
            foreach (var item in ViewModel!.ScatterCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Streamer!.ManageAxisLimits = false;
                item.ManualScale = true;
                item.AutoScale = false;
            }

            // DATA LOGGER
            foreach (var item in ViewModel!.DataLoggerCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Streamer!.ManageAxisLimits = false;
                item.ManualScale = true;
                item.AutoScale = false;
            }

            ViewModel!.ManualScaleY();
            _autoScaled = false;
        }
    }

    /// <summary>
    /// Automatics the scaled setup.
    /// </summary>
    private void AutoScaledSetup()
    {
        if (!_autoScaled)
        {
            ////// STREAMER
            ////foreach (var item in ViewModel!.DataUI)
            ////{
            ////    item.Streamer!.ManageAxisLimits = true;
            ////    item.ManualScale = false;
            ////    item.AutoScale = true;
            ////}

            // SIGNAL
            foreach (var item in ViewModel!.SignalCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Streamer!.ManageAxisLimits = true;
                item.ManualScale = false;
                item.AutoScale = true;
            }

            // SCATTER
            foreach (var item in ViewModel!.ScatterCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Streamer!.ManageAxisLimits = true;
                item.ManualScale = false;
                item.AutoScale = true;
            }

            // DATA LOGGER
            foreach (var item in ViewModel!.DataLoggerCollectionUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale(false, false);
                }
                ////item.Streamer!.ManageAxisLimits = true;
                item.ManualScale = false;
                item.AutoScale = true;
            }

            ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale();
            _autoScaled = true;
        }
    }

    private void YAxisSetup()
    {
        ViewModel!.YAxesSetup(YAxisName);
    }
}
