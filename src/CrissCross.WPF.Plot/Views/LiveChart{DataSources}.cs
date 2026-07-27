// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Interaction logic for WPF Chart AICS.</summary>
public partial class LiveChart
{
    /// <summary>Copies reactive plot sources to a stable array for rebinding.</summary>
    /// <param name="sources">The sources to copy.</param>
    /// <returns>A snapshot of the supplied sources.</returns>
    private static IReactivePlotSource[] CopyReactivePlotSources(IEnumerable<IReactivePlotSource> sources) => [.. sources];

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
        var sourceArray = sources as IReactivePlotSource[] ?? CopyReactivePlotSources(sources);
        ConfigureReactivePlotXAxis(sourceArray);
        ViewModel.HideAllYAxis();
        ReactivePlotBindingOptions options = new()
        {
            UiScheduler = RxSchedulers.MainThreadScheduler,
            MaxVisiblePoints = (UseFixedNumberOfPoints ? NumberPointsPlotted : null),
            MaxAxisCount = Math.Max(1, ViewModel.YAxisList.Count),
        };
        _reactivePlotConnection = new ReactivePlotBinder().Bind(
            ViewModel,
            sourceArray,
            options);
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
        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
            handler => Unloaded += handler,
            handler => Unloaded -= handler);

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
        _crosshairDisposable = ViewModel
            .WhenAnyValue(x => x.CrossHairEnabled)
            .Subscribe(ApplyCrosshairVisibility);
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
        _crosshairDisposable = ViewModel
            .WhenAnyValue(x => x.CrossHairEnabled)
            .Subscribe(ApplyCrosshairVisibility);
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
    private void ChangeSignalDataWithPoints(SignalXYPoints input) => ChangeSignalDataWithPoints([input.Data]);

    /// <summary>Handles the ChangeSignalDataWithPoints operation.</summary>
    /// <param name="data">The signal data points.</param>
    private void ChangeSignalDataWithPoints(
        IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> data)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalPoints(data);
        InitializeControlMenu();
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataWithPoints operation.</summary>
    private void ChangeSignalDataWithPoints() => ChangeSignalDataWithPoints([SignalWithPoints]);

    /// <summary>Handles the ChangeSignalsDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalsDataWithPoints(SignalXYEnumPoints input) => ChangeSignalDataWithPoints(input.Data);

    /// <summary>Handles the ChangeSignalsDataWithPoints operation.</summary>
    private void ChangeSignalsDataWithPoints() => ChangeSignalsDataWithPoints(new(SignalsWithPoints));

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
        ViewModel?.InitializeLinesForSignalObservablesPoints(
            input.Data,
            fs: Frequency,
            sampleCount: Convert.ToUInt32(NSamples));
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
        ViewModel?.InitializeLinesForSignalObservablesPoints(
            SignalObservablesWithPoints,
            fs: Frequency,
            sampleCount: Convert.ToUInt32(NSamples));
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
        InitializeControlMenu();
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
        InitializeControlMenu();
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Updates crosshair visibility for every active plot line.</summary>
    /// <param name="isVisible">A value indicating whether crosshairs are visible.</param>
    private void ApplyCrosshairVisibility(bool isVisible)
    {
        if (ViewModel is null)
        {
            return;
        }

        foreach (var plotLine in ViewModel.PlotLinesCollectionUI)
        {
            plotLine.ChartSettings.IsCrossHairVisible = isVisible;
        }
    }

    /// <summary>Initializes the control menu from the active plot-line settings.</summary>
    private void InitializeControlMenu()
    {
        if (ViewModel is null)
        {
            return;
        }

        List<ChartObjects> settings = [];
        foreach (var plotLine in ViewModel.PlotLinesCollectionUI)
        {
            settings.Add(plotLine.ChartSettings);
        }

        ViewModel.InitializeControlMenu(settings);
    }
}
