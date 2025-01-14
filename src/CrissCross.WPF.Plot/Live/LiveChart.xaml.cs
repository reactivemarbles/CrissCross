// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using CP.WPF.Controls;
using ReactiveUI;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Interaction logic for WPF Chart AICS.
/// </summary>
[SupportedOSPlatform("windows10.0.17763.0")]
public partial class LiveChart
{
    private bool _needLock = true;
    private bool _needAutoScale = true;
    private bool _needMarkerOff = true;
    private bool _autoScaled;
    private bool _locked;
    private bool _markerOff;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChart" /> class.
    /// </summary>
    public LiveChart()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            ViewBindings(d);
            First = false;
            ViewModel = new(MainChartGrid);
            DataContext = ViewModel;
            ExecuteLockUnlock();
            ExecuteManAutoScale();
            InitializeButtons(d);
        });
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the update.
    /// </summary>
    /// <value>
    /// The update.
    /// </value>
    public bool First { get; set; } = true;

    private void ViewBindings(CompositeDisposable d)
    {
        this.BindCommand(ViewModel, vm => vm.GraphLocked, v => v.LiveHistory).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.AutoScale, v => v.AutoScale).DisposeWith(d);
        this.BindCommand(ViewModel, vm => vm.EnableMarkerBtn, v => v.EnableMarkerBtn).DisposeWith(d);
    }

    private void ChangeSignalOberver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needMarkerOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(SignalObservablesWithTimeStamp);
    }

    private void ChangeSignalData()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needMarkerOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(DataWithTimeStamp);
    }

    private void InitializeButtons(CompositeDisposable d)
    {
        // BY DEFAULT: LOCKED AND AUTOSCALED
        // LOCK GRAPH BUTTON
        ViewModel?.GraphLocked?
            .Subscribe(_ => ExecuteLockUnlock())
            .DisposeWith(d);

        // AUTO-SCALE BUTON
        ViewModel?.AutoScale?
            .Subscribe(_ => ExecuteManAutoScale())
            .DisposeWith(d);

        // AUTO-SCALE BUTON
        ViewModel?.EnableMarkerBtn?
            .Subscribe(_ => ExecuteMarkerOnOff())
            .DisposeWith(d);
    }

    private void ExecuteMarkerOnOff()
    {
        if (_needMarkerOff)
        {
            if (!_markerOff)
            {
                ViewModel!.EnableMarker = false;
                _markerOff = true;
            }
        }
        else if (_markerOff)
        {
            ViewModel!.EnableMarker = true;
            _markerOff = false;
        }

        _needMarkerOff = !(_needMarkerOff && _markerOff);
        EnableMarkerBtn.ToolTip = _markerOff ? "Marker off" : "Marker";
        EnableMarkerBtn.Icon = _markerOff ? AppBarIcons.md_crosshairs_off : AppBarIcons.md_crosshairs;
        ViewModel!.WpfLivePlot?.Refresh();
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
        ViewModel!.WpfLivePlot?.Refresh();
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
        ViewModel!.WpfLivePlot?.Refresh();
    }

    /// <summary>
    /// Lockeds the plot setup.
    /// </summary>
    private void LockedPlotSetup()
    {
        if (!_locked)
        {
            ViewModel!.WpfLivePlot?.UserInputProcessor.Disable();
            _locked = true;

            // SIGNAL
            foreach (var item in ViewModel!.DataSignalUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfLivePlot?.Plot.Axes.AutoScale(false, false);
                }

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
            ViewModel!.WpfLivePlot?.UserInputProcessor.Enable();
            _locked = false;

            // SIGNAL
            foreach (var item in ViewModel!.DataSignalUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfLivePlot?.Plot.Axes.AutoScale(false, false);
                }

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
            // SIGNAL
            foreach (var item in ViewModel!.DataSignalUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfLivePlot?.Plot.Axes.AutoScale(false, false);
                }

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
            // SIGNAL
            foreach (var item in ViewModel!.DataSignalUI)
            {
                if (_autoScaled)
                {
                    ViewModel!.WpfLivePlot?.Plot.Axes.AutoScale(false, false);
                }

                item.ManualScale = false;
                item.AutoScale = true;
            }

            ViewModel!.WpfLivePlot?.Plot.Axes.AutoScale();
            _autoScaled = true;
        }
    }
}
