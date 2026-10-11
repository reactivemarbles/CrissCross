// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
#if REACTIVE_SHIM
using ReactiveUI.Binding.Reactive.Observables;
#else
using ReactiveUI.Binding.Observables;
#endif
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
#if REACTIVE_SHIM
using ReactiveUI.Avalonia.Reactive;
#else
using ReactiveUI.Avalonia;
#endif
using ScottPlot;
using ScottPlot.Plottables;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ReactiveUIBindingExtensions;
#else
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Provides the interactive Avalonia live chart.</summary>
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

    /// <summary>Stores the auto scaled value.</summary>
    private bool _autoScaled;

    /// <summary>Stores the locked value.</summary>
    private bool _locked = true;

    /// <summary>Stores the plottable being dragged value.</summary>
    private AxisLine? _plottableBeingDragged;

    /// <summary>Initializes a new instance of the <see cref="LiveChart"/> class.</summary>
    public LiveChart()
    {
        InitializeComponent();
        First = false;
        PlotSettings.Click += PlotSettings_Click;
        LayoutRoot.AddHandler(PointerReleasedEvent, IndexText_MouseUp);
        MainChartGrid.AddHandler(PointerPressedEvent, MainChartGrid_MouseDown, RoutingStrategies.Tunnel);
        MainChartGrid.PointerReleased += MainChartGrid_MouseUp;
        MainChartGrid.PointerMoved += MainChartGrid_MouseMove;
        var useFixedNumberOfPoints = UseFixedNumberOfPoints;
        var numberPointsPlotted = NumberPointsPlotted;
        LiveChartViewModel viewModel = new(MainChartGrid);
        viewModel.UseFixedNumberOfPoints = useFixedNumberOfPoints;
        viewModel.NumberPointsPlotted = numberPointsPlotted;
        ViewModel = viewModel;
        DataContext = ViewModel;
        ToolTip.SetShowDelay(ViewModel.AvaPlot1vm!, 0);
        _ = ViewModel
            .ThrownExceptions.Subscribe(static ex => Debug.WriteLine($"Exception in LiveChart: {ex.Message}"))
            .DisposeWith(_dd);
        ExecuteLockUnlock();
        ExecuteManAutoScale();
        _ = this.WhenActivated(ElementBinding1);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets the update.</summary>
    /// <value>
    /// The update.
    /// </value>
    public bool First { get; set; } = true;

    /// <summary>Gets the right properties panel.</summary>
    public RightPropertiesView RightPropertiesPanel => RightProperties;

    /// <summary>Gets the chart title text block.</summary>
    public TextBlock ChartTitleTextBlock => Title;

    /// <summary>Gets the right legend scroll viewer.</summary>
    public ScrollViewer RightLegendViewer => RightLegend;

    /// <summary>Gets the top legend scroll viewer.</summary>
    public ScrollViewer TopLegendViewer => TopLegend;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_reactivePlotConnection is not null)
        {
            return;
        }

        RefreshReactivePlotSources();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DisposeReactivePlotConnection();
        base.OnDetachedFromVisualTree(e);
    }

    /// <summary>Handles the ElementBinding1 operation.</summary>
    /// <param name="d">The d value.</param>
    private void ElementBinding1(ActivationDisposable d)
    {
        _ = new ActionDisposable(DisposeReactivePlotConnection).DisposeWith(d);
        BindRightProperties(d);
        BindChartMetadata(d);
    }

    /// <summary>Toggles the plot settings popup.</summary>
    /// <param name="sender">The settings button.</param>
    /// <param name="e">The click event.</param>
    private void PlotSettings_Click(object? sender, RoutedEventArgs e) => OpenPlotSettings();

    /// <summary>Binds the selected chart object to the right properties panel.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindRightProperties(ActivationDisposable disposables)
    {
        var viewModel = ViewModel!;
        _ = new PropertyObservable<bool>(
                viewModel,
                nameof(LiveChartViewModel.RightPropertyVisibility),
                static source => ((LiveChartViewModel)source).RightPropertyVisibility,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(visibility => RightPropertiesPanel.IsVisible = visibility)
            .DisposeWith(disposables);

        _ = this.WhenAnyValue(static x => x.ViewModel!.SelectedSetting)
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
    private void BindChartMetadata(ActivationDisposable disposables)
    {
        var viewModel = ViewModel!;
        _ = new PropertyObservable<string>(
                viewModel,
                nameof(LiveChartViewModel.Title),
                static source => ((LiveChartViewModel)source).Title,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(title => ChartTitleTextBlock.Text = title)
            .DisposeWith(disposables);
        _ = new PropertyObservable<string>(
                viewModel,
                nameof(LiveChartViewModel.Title),
                static source => ((LiveChartViewModel)source).Title,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(title => ChartTitleTextBlock.IsVisible = title != " ")
            .DisposeWith(disposables);
        _ = new PropertyObservable<LegendPosition>(
                viewModel,
                nameof(LiveChartViewModel.LegendPosition),
                static source => ((LiveChartViewModel)source).LegendPosition,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(position => RightLegendViewer.IsVisible = position == LegendPosition.Right)
            .DisposeWith(disposables);
        _ = new PropertyObservable<LegendPosition>(
                viewModel,
                nameof(LiveChartViewModel.LegendPosition),
                static source => ((LiveChartViewModel)source).LegendPosition,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(position => TopLegendViewer.IsVisible = position == LegendPosition.Top)
            .DisposeWith(disposables);
        _ = new PropertyObservable<bool>(
                viewModel,
                nameof(LiveChartViewModel.UseFixedNumberOfPoints),
                static source => ((LiveChartViewModel)source).UseFixedNumberOfPoints,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(value => UseFixedNumberOfPoints = value)
            .DisposeWith(disposables);
        _ = new PropertyObservable<int>(
                viewModel,
                nameof(LiveChartViewModel.NumberPointsPlotted),
                static source => ((LiveChartViewModel)source).NumberPointsPlotted,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(value => NumberPointsPlotted = value)
            .DisposeWith(disposables);
    }

    /// <summary>Handles the IndexText_MouseUp operation.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void IndexText_MouseUp(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Source is not TextBlock { DataContext: IPlottableUI item })
        {
            return;
        }

        var setting = item.ChartSettings;
        if (ViewModel!.SelectedSetting == setting)
        {
            ViewModel!.SelectedSetting = null;
            RightProperties.IsVisible = false;
            ViewModel!.RightPropertyVisibility = false;
            return;
        }

        ViewModel!.SelectedSetting = setting;
        RightProperties.IsVisible = true;
        ViewModel!.RightPropertyVisibility = true;
    }

    /// <summary>Handles the ExecuteMarkerOnOff operation.</summary>
    private void ExecuteMarkerOnOff() => SetPointerValues(!ViewModel!.CrossHairEnabled);

    /// <summary>Sets pointer markers for all supported series.</summary>
    /// <param name="enabled">Whether pointer values should be visible.</param>
    private void SetPointerValues(bool enabled)
    {
        ViewModel!.CrossHairEnabled = enabled;
        SynchronizePointerValues();
        ViewModel!.AvaPlot1vm?.Refresh();
    }

    /// <summary>Synchronizes supported per-series pointer markers.</summary>
    private void SynchronizePointerValues()
    {
        foreach (var settings in ViewModel!.SeriesSettings)
        {
            if (settings.PlotType is PlotType.Signal or PlotType.SignalXY)
            {
                settings.IsCrossHairVisible = ViewModel.CrossHairEnabled;
            }
        }

        foreach (var line in ViewModel.PlotLinesCollectionUI)
        {
            if (line is SignalUI or SignalXY_UI)
            {
                line.ChartSettings.IsCrossHairVisible = ViewModel.CrossHairEnabled;
            }
        }
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
        ViewModel!.AvaPlot1vm?.Refresh();
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

        ViewModel!.AvaPlot1vm?.Refresh();
    }

    /// <summary>Lockeds the plot setup.</summary>
    private void LockedPlotSetup()
    {
        if (_locked)
        {
            return;
        }

        ViewModel!.AvaPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
        ViewModel!.AvaPlot1vm?.UserInputProcessor.Disable();
        _locked = true;
    }

    /// <summary>Unockeds the plot setup.</summary>
    private void UnockedPlotSetup()
    {
        if (!_locked)
        {
            return;
        }

        ViewModel!.AvaPlot1vm!.Plot.Axes.ContinuouslyAutoscale = false;
        ViewModel!.AvaPlot1vm?.UserInputProcessor.Enable();
        _locked = false;
    }

    /// <summary>Manuals the scaled setup.</summary>
    private void ManualScaledSetup()
    {
        if (!_autoScaled)
        {
            return;
        }

        ViewModel!.AvaPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
        foreach (var verticalAxis in ViewModel.YAxisList)
        {
            ViewModel!.AvaPlot1vm?.Plot.Axes.SetLimitsY(-ManualScaleBoundary, ManualScaleBoundary, verticalAxis);
        }

        ViewModel.AvaPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleX(
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

        ViewModel!.AvaPlot1vm!.Plot.Axes.ContinuouslyAutoscale = true;
        ViewModel.AvaPlot1vm!.Plot.Axes.ContinuousAutoscaleAction = LiveChartViewModel.AutoScaleAll();
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
