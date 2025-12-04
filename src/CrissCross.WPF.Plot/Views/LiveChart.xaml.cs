// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CP.WPF.Controls;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Plottables;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Interaction logic for WPF Chart AICS.
/// </summary>
[SupportedOSPlatform("windows10.0.19041")]
public partial class LiveChart : ReactiveUI.ReactiveUserControl<LiveChartViewModel>
{
    private readonly CompositeDisposable _dd = [];
    private IDisposable? _crosshairDisposable;
    private bool _needLock;
    private bool _needAutoScale = true;
    private bool _needCrossHairOff = true;
    private bool _autoScaled;
    private bool _locked = true;
    private bool _crosshairOff;
    private AxisLine? _plottableBeingDragged;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChart"/> class.
    /// </summary>
    public LiveChart()
    {
        InitializeComponent();
        First = false;
        ViewModel = new(MainChartGrid) { UseFixedNumberOfPoints = UseFixedNumberOfPoints, NumberPointsPlotted = NumberPointsPlotted };
        DataContext = ViewModel;
        ViewModel.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Exception in LiveChart: {ex.Message}")).DisposeWith(_dd);
        ExecuteLockUnlock();
        ExecuteManAutoScale();
        InitializeButtons();
        this.WhenActivated(ElementBinding1);
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
        this.BindCommand(ViewModel, vm => vm.GraphLocked, v => v.LiveHistoryBtn).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.RemoveLabelsBtn, v => v.RemoveLabelBtn).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.AddCrosshairBtn, v => v.AddCrosshairBtn).DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.LiveHistoryBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.EnableMarkerBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.AddCrosshairBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.RemoveLabelBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.EnableMarkerBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.IsMenuExpanded, v => v.AddCrosshairBtn.Visibility, x => x ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

        this.BindCommand(ViewModel, vm => vm.ExpandMenuBtn, v => v.PlotSettings).DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.RightPropertyVisibility, v => v.RightProperties.Visibility).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.SelectedSetting!.ItemName, v => v.RightProperties.textbox1.Text).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Text).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Visibility, x => x == " " ? Visibility.Collapsed : Visibility.Visible).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.LegendPosition, v => v.RightLegend.Visibility, x => x == LegendPosition.Right ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
        this.OneWayBind(ViewModel, vm => vm.LegendPosition, v => v.TopLegend.Visibility, x => x == LegendPosition.Top ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

        this.Bind(ViewModel, vm => vm.SelectedSetting!.LineWidth, v => v.RightProperties.LineWidth.Value).DisposeWith(d);
        this.Bind(ViewModel, vm => vm.SelectedSetting!.Color, v => v.RightProperties.colorsComboBox.SelectedItem).DisposeWith(d);
        this.Bind(ViewModel, vm => vm.SelectedSetting!.Visibility, v => v.RightProperties.visibilityComboBox.SelectedItem).DisposeWith(d);
        this.Bind(ViewModel, vm => vm.UseFixedNumberOfPoints, v => v.UseFixedNumberOfPoints).DisposeWith(d);
        this.Bind(ViewModel, vm => vm.NumberPointsPlotted, v => v.NumberPointsPlotted).DisposeWith(d);
    }

    private void IndexText_MouseUp(object sender, MouseButtonEventArgs e)
    {
        // make sure text block was clicked
        if (sender is TextBlock textBlock)
        {
            // read clicked data context and current data context
            var dataContext = textBlock.DataContext;

            // is data context is other
            if (dataContext != null)
            {
                // if dataContext is IPlottable
                if (dataContext is IPlottableUI item)
                {
                    var setting = item.ChartSettings;
                    if (ViewModel!.SelectedSetting == setting)
                    {
                        ViewModel!.SelectedSetting = null;
                        RightProperties.Visibility = Visibility.Collapsed;
                        ViewModel!.RightPropertyVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ViewModel!.SelectedSetting = setting;
                        RightProperties.Visibility = Visibility.Visible;
                        ViewModel!.RightPropertyVisibility = Visibility.Visible;
                    }
                }
            }
        }
    }

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

    private void ChangeSignalDataObserverWithPoints(StreamerEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(input.Data, fs: (int)Frequency, nSamples: (uint)NSamples);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    private void ChangeSignalDataObserverWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(SignalObservablesWithPoints, fs: (int)Frequency, nSamples: (uint)NSamples);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

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
            .ObserveOn(RxSchedulers.MainThreadScheduler).Subscribe(_ =>
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
            if (!_autoScaled)
            {
                _needAutoScale = true;
                ExecuteManAutoScale();
            }
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

    /// <summary>
    /// Lockeds the plot setup.
    /// </summary>
    private void LockedPlotSetup()
    {
        if (!_locked)
        {
            ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
            ViewModel!.WpfPlot1vm?.UserInputProcessor.Disable();
            _locked = true;
        }
    }

    /// <summary>
    /// Unockeds the plot setup.
    /// </summary>
    private void UnockedPlotSetup()
    {
        if (_locked)
        {
            ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = false;
            ViewModel!.WpfPlot1vm?.UserInputProcessor.Enable();
            _locked = false;
        }
    }

    /// <summary>
    /// Manuals the scaled setup.
    /// </summary>
    private void ManualScaledSetup()
    {
        if (_autoScaled)
        {
            ////ViewModel!.ManualScaleY();
            ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
            foreach (var yAxe in ViewModel.YAxisList)
            {
                ViewModel!.WpfPlot1vm?.Plot.Axes.SetLimitsY(-50, 50, yAxe);
            }

            ViewModel.WpfPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleX(xaxis: ViewModel!.XAxis1);

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
            ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
            ViewModel.WpfPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleAll();
            _autoScaled = true;
        }
    }

    private void YAxisSetup() => ViewModel!.YAxesSetup(YAxisName);

    private void MainChartGrid_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            // MOUSE EVENT
            var position = e.GetPosition(MainChartGrid);

            // Obtener el DPI Scaling del Grid actual
            var dpiInfo = VisualTreeHelper.GetDpi(MainChartGrid);
            var dpiScaleX = dpiInfo.DpiScaleX;
            var dpiScaleY = dpiInfo.DpiScaleY;

            // Ajustar las coordenadas para que sean precisas
            var adjustedX = position.X * dpiScaleX;
            var adjustedY = position.Y * dpiScaleY;

            //// determine where the mouse is and send the coordinates
            Pixel mousePixel = new(adjustedX, adjustedY);
            var mouseLocation = ViewModel!.WpfPlot1vm!.Plot.GetCoordinates(mousePixel, ViewModel!.XAxis1, ViewModel.YAxisList[0]);
            var xAxe = mouseLocation.X;
            var yAxe = mouseLocation.Y;

            // Create a crosshair to highlight the point under the cursor
            string? text;
            if (ViewModel.IsXAxisDateTime)
            {
                text = "X : " + DateTime.FromOADate(xAxe).ToLongTimeString() + "\nY : " + yAxe.ToString("F2");
            }
            else
            {
                text = "X : " + xAxe.ToString("F2") + "\nY : " + yAxe.ToString("F2");
            }

            var marker = ViewModel.WpfPlot1vm.Plot.Add.Marker(mouseLocation);
            var markerText = ViewModel.WpfPlot1vm.Plot.Add.Text(text, mouseLocation);
            markerText.OffsetX = 7;
            markerText.OffsetY = -7;
            markerText!.LabelFontColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName("White"));
            marker.Axes.XAxis = ViewModel!.WpfPlot1vm!.Plot.Axes.Bottom;
            marker.Axes.YAxis = ViewModel.YAxisList[0];
            markerText.Axes.XAxis = ViewModel!.WpfPlot1vm!.Plot.Axes.Bottom;
            markerText.Axes.YAxis = ViewModel.YAxisList[0];
            ViewModel.LabelCollection.Add((marker, markerText));
        }
        else
        {
            // MOUSE EVENT
            var position = e.GetPosition(MainChartGrid);

            // Obtener el DPI Scaling del Grid actual
            var dpiInfo = VisualTreeHelper.GetDpi(MainChartGrid);
            var dpiScaleX = dpiInfo.DpiScaleX;
            var dpiScaleY = dpiInfo.DpiScaleY;

            // Ajustar las coordenadas para que sean precisas
            var adjustedX = position.X * dpiScaleX;
            var adjustedY = position.Y * dpiScaleY;

            //// determine where the mouse is and send the coordinates
            ////Pixel mousePixel = new(adjustedX, adjustedY);
            ////var mouseLocation = ViewModel.WpfPlot1vm!.Plot.GetCoordinates(mousePixel, ViewModel!.XAxis1, ViewModel.YAxisList[0]);
            ////var xAxe = mouseLocation.X;
            ////var yAxe = mouseLocation.Y;

            var lineUnderMouse = ViewModel?.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
            if (lineUnderMouse is not null)
            {
                _plottableBeingDragged = lineUnderMouse;
                ViewModel!.WpfPlot1vm!.UserInputProcessor.Disable(); // disable panning while dragging
            }
        }
    }

    private void MainChartGrid_MouseUp(object sender, MouseEventArgs e)
    {
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Enable(); // enable panning again
        _plottableBeingDragged = null;
        ViewModel!.WpfPlot1vm!.UserInputProcessor.Enable(); // enable panning again
        ViewModel!.WpfPlot1vm!.Refresh();
    }

    private void MainChartGrid_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(MainChartGrid);
        var dpiInfo = VisualTreeHelper.GetDpi(MainChartGrid);
        var adjustedX = position.X * dpiInfo.DpiScaleX;
        var adjustedY = position.Y * dpiInfo.DpiScaleY;
        var rect = ViewModel!.WpfPlot1vm!.Plot.GetCoordinateRect((float)adjustedX, (float)adjustedY, radius: 5, ViewModel!.XAxis1, ViewModel.YAxisList[0]);
        if (_plottableBeingDragged is null)
        {
            var lineUnderMouse = ViewModel.GetLineUnderMouse((float)adjustedX, (float)adjustedY);
            if (lineUnderMouse is null)
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }
            else if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine)
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.SizeWE;
            }
            else if (lineUnderMouse.IsDraggable && lineUnderMouse is HorizontalLine)
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.SizeNS;
            }
        }
        else
        {
            if (_plottableBeingDragged is HorizontalLine hl)
            {
                hl.Y = rect.VerticalCenter;
                hl.Text = $"{hl.Y:0.00}";
            }
            else if (_plottableBeingDragged is VerticalLine vl)
            {
                vl.X = rect.HorizontalCenter;
                vl.Text = ViewModel.IsXAxisDateTime ? DateTime.FromOADate(Convert.ToDouble(vl.X)).ToLongTimeString() : $"{vl.X:0.00}";
            }

            ViewModel!.WpfPlot1vm!.Refresh();
        }
    }
}
