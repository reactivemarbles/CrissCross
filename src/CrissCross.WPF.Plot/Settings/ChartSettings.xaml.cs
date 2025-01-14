// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

////using System.Reactive.Disposables;
////using System.Reactive.Linq;
////using CP.WPF.Controls;
////using ReactiveUI;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Interaction logic for WPF Chart.
/// </summary>
public partial class ChartSettings
{
    ////private bool _needLock = true;
    ////private bool _needAutoScale = true;
    ////private bool _autoScaled;
    ////private bool _locked;
    ////private CompositeDisposable _dd = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSettings" /> class.
    /// </summary>
    public ChartSettings()
    {
        InitializeComponent();
        ////this.WhenActivated(d =>
        ////{
        ////    ElementBinding1(d);
        ////    First = false;
        ////    ViewModel = new(MainChartGrid);
        ////    DataContext = ViewModel;
        ////    ExecuteLockUnlock();
        ////    ExecuteManAutoScale();

        ////    InitializeButtons();
        ////});
    }

    /////// <summary>
    /////// Gets or sets a value indicating whether gets or sets the update.
    /////// </summary>
    /////// <value>
    /////// The update.
    /////// </value>
    ////public bool First { get; set; } = true;

    ////private void ElementBinding1(CompositeDisposable d)
    ////{
    ////    this.BindCommand(ViewModel, vm => vm.GraphLocked, v => v.LiveHistory).DisposeWith(d);
    ////    this.BindCommand(ViewModel, vm => vm.AutoScale, v => v.AutoScale).DisposeWith(d);
    ////    this.Bind(ViewModel, vm => vm.EnableMarker, v => v.EnableMarker.IsChecked).DisposeWith(d);
    ////}

    ////private void ChangePlotOberver()
    ////{
    ////    ////ViewModel?.InitializeStreamerPlotLines(StreamerObservablesWithTimeStamp);
    ////    _needLock = true;
    ////    ExecuteLockUnlock();
    ////    _needAutoScale = true;
    ////    ExecuteManAutoScale();
    ////}

    ////private void InitializeButtons()
    ////{
    ////    // BY DEFAULT: LOCKED AND AUTOSCALED
    ////    // LOCK GRAPH BUTTON
    ////    ViewModel?.GraphLocked?.Subscribe(_ =>
    ////    {
    ////        ////_needLock = !_needLock;
    ////        ExecuteLockUnlock();
    ////    }).DisposeWith(_dd);

    ////    // AUTO-SCALE BUTON
    ////    ViewModel?.AutoScale?.Subscribe(_ =>
    ////    {
    ////        ////_needAutoScale = !_needAutoScale;
    ////        ExecuteManAutoScale();
    ////    }).DisposeWith(_dd);
    ////}

    ////private void ExecuteLockUnlock()
    ////{
    ////    if (_needLock)
    ////    {
    ////        LockedPlotSetup();
    ////    }
    ////    else
    ////    {
    ////        UnockedPlotSetup();
    ////    }

    ////    _needLock = !(_needLock == true && _locked == true);
    ////    LiveHistory.ToolTip = _locked ? "Locked" : "Interact";
    ////    LiveHistory.Icon = _locked ? AppBarIcons.md_lock : AppBarIcons.md_lock_open;
    ////}

    ////private void ExecuteManAutoScale()
    ////{
    ////    if (_needAutoScale)
    ////    {
    ////        AutoScaledSetup();
    ////    }
    ////    else
    ////    {
    ////        ManualScaledSetup();
    ////    }

    ////    _needAutoScale = !(_needAutoScale == true && _autoScaled == true);
    ////    AutoScale.ToolTip = _autoScaled ? "Auto Scale" : "Manual Scale";
    ////    AutoScale.Icon = _autoScaled ? AppBarIcons.md_hand_back_left_off : AppBarIcons.md_hand_back_left;
    ////    _needLock = true;
    ////    ExecuteLockUnlock();
    ////}

    /////// <summary>
    /////// Lockeds the plot setup.
    /////// </summary>
    ////private void LockedPlotSetup()
    ////{
    ////    if (!_locked)
    ////    {
    ////        ViewModel!.WpfPlot1vm?.Interaction.Disable();
    ////        _locked = true;

    ////        foreach (var item in ViewModel!.DataUI)
    ////        {
    ////            item.Streamer.ManageAxisLimits = _autoScaled;
    ////            item.ManualScale = _autoScaled == false;
    ////            item.AutoScale = _autoScaled == true;
    ////        }
    ////    }
    ////}

    /////// <summary>
    /////// Unockeds the plot setup.
    /////// </summary>
    ////private void UnockedPlotSetup()
    ////{
    ////    if (_locked)
    ////    {
    ////        ViewModel!.WpfPlot1vm?.Interaction.Enable();
    ////        _locked = false;

    ////        foreach (var item in ViewModel!.DataUI)
    ////        {
    ////            item.Streamer.ManageAxisLimits = false;
    ////            item.ManualScale = false;
    ////            item.AutoScale = false;
    ////        }
    ////    }
    ////}

    /////// <summary>
    /////// Manuals the scaled setup.
    /////// </summary>
    ////private void ManualScaledSetup()
    ////{
    ////    if (_autoScaled)
    ////    {
    ////        foreach (var item in ViewModel!.DataUI)
    ////        {
    ////            item.Streamer.ManageAxisLimits = false;
    ////            item.ManualScale = true;
    ////            item.AutoScale = false;
    ////        }

    ////        ViewModel!.ManualScaleY();
    ////        _autoScaled = false;
    ////    }
    ////}

    /////// <summary>
    /////// Automatics the scaled setup.
    /////// </summary>
    ////private void AutoScaledSetup()
    ////{
    ////    if (!_autoScaled)
    ////    {
    ////        foreach (var item in ViewModel!.DataUI)
    ////        {
    ////            item.Streamer.ManageAxisLimits = true;
    ////            item.ManualScale = false;
    ////            item.AutoScale = true;
    ////        }

    ////        ViewModel!.WpfPlot1vm?.Plot.Axes.AutoScale();
    ////        _autoScaled = true;
    ////    }
    ////}
}
